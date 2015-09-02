using Base.DAL;
using Base.Notification.Service.Abstract;
using Base.Security;
using Base.Security.Service;
using Base.Security.Service.Abstract;
using Base.Service.Log;
using Framework;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace WebUI.Concrete
{
    public class AccountManager : IAccountManager
    {
        private static readonly TimeSpan ResetPasswordRequestLifeTime = TimeSpan.FromMinutes(15);
        private static readonly TimeSpan RegisterRequestLifeTime = TimeSpan.FromDays(1);

        private readonly ISecurityUserService _securityUserService;
        private readonly IEmailService _emailService;
        private readonly ILogService _logService;

        public AccountManager(ISecurityUserService securityUserService, IEmailService emailService, ILogService logService)
        {
            _securityUserService = securityUserService;
            _emailService = emailService;
            _logService = logService;
        }

        public async Task ResetPassword(IUnitOfWork unitOfWork, Uri systemUrl, string login)
        {
            var user = _securityUserService.GetUser(unitOfWork, login);

            if (user == null)
            {
                throw new InvalidOperationException(String.Format("Отсутствует пользователь с логином '{0}'", login));
            }

            if (String.IsNullOrEmpty(user.Email))
            {
                throw new InvalidOperationException(String.Format("У пользователя '{0}' не задан email. Обратитесь к администратору.", login));
            }

            if (user.ConfirmRequests != null && user.ConfirmRequests.Any(x => x.ValidUntil > DateTime.Now && x.Type == ConfirmType.ResetPassword && !x.Used))
            {
                throw new InvalidOperationException("Запрос на смену пароля уже был отправлен ранее. Проверьте свой почтовый ящик.");
            }

            var request = new UserConfirmRequest()
            {
                Type = ConfirmType.ResetPassword,
                UserID = user.ID,
                RequestTime = DateTime.Now,
                ValidUntil = DateTime.Now.Add(ResetPasswordRequestLifeTime),
                Code = Guid.NewGuid().ToString("N")
            };

            _logService.Log(String.Format("ResetPassword >> user: {0}; email {1}", user.FullName, user.Email));

            if (!await _SendMail(systemUrl, request, user.Email))
            {
                throw new InvalidOperationException("Произошла ошибка при отправке письма на электронную почту. Свяжитесь с администратором портала.");
            }

            if (user.ConfirmRequests == null) user.ConfirmRequests = new List<UserConfirmRequest>();

            user.ConfirmRequests.Add(request);

            if (user.IsUnregistered)
            {
                user.IsUnregistered = false;

                var roleRep = unitOfWork.GetRepository<Role>();
                var catRep = unitOfWork.GetRepository<UserCategory>();

                var externalRole = roleRep.Find(x => x.SystemRole == SystemRole.Base);
                if (externalRole == null)
                    throw new InvalidOperationException("В системе отсутствует базовая роль.");

                user.Roles = new Collection<Role> { externalRole };

                string systemName = UserType.Base.ToString();
                string name = UserType.Base.GetDescription();

                var userCategory = catRep.Find(x => x.SystemName == systemName);
                if (userCategory != null)
                {
                    user.CategoryID = userCategory.ID;
                }
                else
                {
                    user.UserCategory = new UserCategory { Name = name, SystemName = systemName };
                }
            }

            await unitOfWork.SaveChangesAsync();
        }

        private async Task<bool> _SendMail(Uri systemUrl, UserConfirmRequest request, string email)
        {
            var verifyUrl = new Uri(new Uri(systemUrl.GetLeftPart(UriPartial.Authority)),
                                    String.Format("Account/VerifyConfirmation?type={0}&userId={1}&code={2}",
                                                  (int)request.Type, request.UserID, request.Code));

            //TODO: [scheduler]
            switch(request.Type)
            {
                case ConfirmType.NewUser:
                    return _emailService.SendMail(
                        email,
                        "Федеральный портал проектов НПА: регистрация",
                        "registration",
                        new Dictionary<string, string> { { "url", verifyUrl.ToString() } },
                        true);

                case ConfirmType.ResetPassword:
                    return _emailService.SendMail(
                        email, 
                        "Федеральный портал проектов НПА: восстановление пароля", 
                        "recoverPassword", 
                        new Dictionary<string, string> { { "url", verifyUrl.ToString() } },
                        true);

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public User ConfirmationVerify(IUnitOfWork unitOfWork, ConfirmType type, int userId, string code)
        {
            string operationName;
            switch (type)
            {
                case ConfirmType.NewUser:
                    operationName = "регистрации";
                    break;
                case ConfirmType.ResetPassword:
                    operationName = "восстановления пароля";
                    break;
                default:
                    throw new NotImplementedException();
            }

            User user = _securityUserService.GetUser(unitOfWork, userId);
            if (user == null)
            {
                throw new InvalidOperationException("Не найден пользователь");
            }
            if (user.ConfirmRequests == null || !user.ConfirmRequests.Any())
            {
                throw new InvalidOperationException(String.Format("Отсутствуют запросы {0} от пользователя {1}", operationName, user.Login));
            }

            UserConfirmRequest request = user.ConfirmRequests.FirstOrDefault(x => x.Code == code);

            if (request == null)
            {
                throw new InvalidOperationException(String.Format("Данный запрос {0} отсутствует в системе. Обратитесь к администратору.", operationName));
            }
            if (request.Used)
            {
                throw new InvalidOperationException(String.Format("Запрос {0} уже был выполнен ранее.", operationName));
            }
            if (request.ValidUntil < DateTime.Now.AddMinutes(5))
            {
                throw new InvalidOperationException(String.Format("Запрос просрочен. Воспользуйтесь функцией {0} еще раз.", operationName));
            }

            request.Used = true;
            request.UseTime = DateTime.Now;

            switch (type)
            {
                case ConfirmType.ResetPassword:
                    _ResetPasswordVerify(user);
                    break;

                case ConfirmType.NewUser:
                    _RegisterVerify(user);
                    break;
            }

            unitOfWork.SaveChanges();

            return user;
        }

        private User _ResetPasswordVerify(User user)
        {
            user.ChangePasswordOnFirstLogon = true;
            return user;
        }

        private User _RegisterVerify(User user)
        {
            user.Hidden = false;
            user.IsUnregistered = false;

            return user;
        }

        public void Register(ISystemUnitOfWork unitOfWork, Uri systemUrl, string email, string password, string firstName, string lastName)
        {
            User user = _securityUserService.GetUser(unitOfWork, email, includeAwaitConfirm: true);
            if (user != null && user.Hidden)
            {
                throw new Exception("Вы уже регистрировались ранее. На указанный вами почтовый адрес было отправлено письмо. " +
                                    "Для завершения регистрации перейдите по содержащейся в нем ссылке.");
            }

            UserConfirmRequest request = new UserConfirmRequest()
            {
                Type = ConfirmType.NewUser,
                RequestTime = DateTime.Now,
                ValidUntil = DateTime.Now.Add(RegisterRequestLifeTime),
                Code = Guid.NewGuid().ToString("N")
            };

            if (user == null)
            {
                user = new User
                {
                    Login = email,
                    Email = email,
                    Password = password,
                    FirstName = firstName,
                    LastName = lastName,
                    Hidden = true,
                    ConfirmRequests = new List<UserConfirmRequest> { request }
                };
            }
            else
            {
                if (user.IsUnregistered) // registered by someone else
                {
                    user.Email = email;
                    user.Password = password;
                    user.FirstName = firstName;
                    user.LastName = lastName;
                    user.MiddleName = "";

                    if (user.ConfirmRequests == null)
                        user.ConfirmRequests = new List<UserConfirmRequest>();
                    user.ConfirmRequests.Add(request);
                }
            }



            _securityUserService.RegisterUser(unitOfWork, user);

            _SendMail(systemUrl, request, email);
        }
    }
}