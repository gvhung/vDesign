using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Base.DAL;
using Base.Service;
using Base.Settings;
using Framework.Wrappers;
using Framework;
using System.Linq;
using Base.Ambient;

namespace Base.Security.Service
{
    public class SecurityUserService : ISecurityUserService
    {
        private readonly ISettingItemService _settingsService;
        private readonly IUserCategoryService _userCategoryService;
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;

        private readonly ICacheWrapper _cacheWrapper;

        private readonly static object _cacheLocker = new object();

        private const string _keyCache = "0ED205A6-E715-401A-926E-3A73B5AB74E7";

        public SecurityUserService(ISettingItemService settingsService, IUserCategoryService userCategoryService, IUnitOfWorkFactory unitOfWorkFactory, ICacheWrapper cacheWrapper)
        {
            _settingsService = settingsService;
            _userCategoryService = userCategoryService;

            _unitOfWorkFactory = unitOfWorkFactory;
            _cacheWrapper = cacheWrapper;
        }

        private Dictionary<string, ISecurityUser> GetUserCache()
        {
            if (_cacheWrapper[_keyCache] != null)
                return _cacheWrapper[_keyCache] as Dictionary<string, ISecurityUser>;

            lock (_cacheLocker)
            {
                if (_cacheWrapper[_keyCache] == null)
                    _cacheWrapper[_keyCache] = new Dictionary<string, ISecurityUser>();

            }

            return _cacheWrapper[_keyCache] as Dictionary<string, ISecurityUser>;
        }

        public bool ValidateUser(IUnitOfWork unitOfWork, string login, string password, bool allowEmptyPassword = false)
        {
            if (String.IsNullOrEmpty(login) || (!allowEmptyPassword && String.IsNullOrEmpty(password)))
                return false;

            var user = unitOfWork.GetRepository<User>().Find(u => u.Login.ToUpper() == login.ToUpper());

            if (user == null) return false;

            var passwordCryptographer = new PasswordCryptographer();

            return passwordCryptographer.AreEqual(user.Password, password);
        }

        public bool ValidateUser(string login, string password, bool allowEmptyPassword = false)
        {
            using (var uow = _unitOfWorkFactory.CreateSystem())
            {
                return ValidateUser(uow, login, password, allowEmptyPassword);
            }
        }

        public ISecurityUser GetSecurityUser(string login)
        {
            var dicSecurityUsers = GetUserCache();

            if (dicSecurityUsers.ContainsKey(login))
                return dicSecurityUsers[login];

            lock (_cacheLocker)
            {
                if (!dicSecurityUsers.ContainsKey(login))
                {
                    using (var uow = _unitOfWorkFactory.CreateSystem())
                    {
                        var user = GetUser(uow, login);

                        if (user == null) return null;

                        var company = user.UserCategory != null ? _userCategoryService.GetCompany(uow, user.UserCategory) : null;

                        dicSecurityUsers.Add(login, new SecurityUser(user, company));
                    }
                }
            }

            return dicSecurityUsers[login];
        }

        public User GetUser(IUnitOfWork unitOfWork, string login, bool? hidden = false, bool includeAwaitConfirm = false)
        {
            return
                unitOfWork.GetRepository<User>()
                    .Find(
                        u =>
                            (hidden == null || u.Hidden == hidden.Value || includeAwaitConfirm) &&
                            u.Login.ToUpper() == login.ToUpper() &&
                            (!includeAwaitConfirm || !u.Hidden ||
                             u.ConfirmRequests.Any(
                                 r => !r.Used && r.Type == ConfirmType.NewUser && r.ValidUntil > DateTime.Now)));
        }

        public User GetUser(IUnitOfWork unitOfWork, int id)
        {
            return unitOfWork.GetRepository<User>().Find(id);
        }

        public void ChangePassword(int id, string newPass)
        {
            _ChangePassword(id, null, newPass, false);
        }

        public bool IsValidPassword(string password, out string message, int minLen = 6, int maxRepeats = 2, bool checkKeyboard = true)
        {
            var config = _settingsService.GetValue(Base.Security.Consts.KEY_CONFIG, null) as Config;

            if (config != null)
            {
                minLen = config.MinLenPassword;
                checkKeyboard = config.PasswordCheckKeyboard;
            }

            return PasswordValidator.Check(password, out message, minLen, maxRepeats, checkKeyboard);
        }

        private void _ChangePassword(int id, string oldPass, string newPass, bool verifyOldPass)
        {
#if !DEBUG
            if (!AppContext.SecurityUser.IsAdmin && id != AppContext.SecurityUser.ID)
            {
                throw new Exception("Отказано в доступе");
            }
#endif
            using (var unitOfWork = _unitOfWorkFactory.CreateSystem())
            {
                var user = unitOfWork.GetRepository<User>().Find(u => u.ID == id);

                if (user == null)
                {
                    throw new Exception("Пользователь не найден");
                }

                var passwordCryptographer = new PasswordCryptographer();

                if (verifyOldPass && !String.IsNullOrEmpty(user.Password))
                {
                    if (!passwordCryptographer.AreEqual(user.Password, oldPass))
                    {
                        throw new Exception("Неверный текущий пароль");
                    }
                }

                string validationMessage = "";

                if (newPass == null || !IsValidPassword(newPass, out validationMessage))
                {
                    throw new Exception(validationMessage);
                }

                user.Password = passwordCryptographer.GenerateSaltedPassword(newPass);
                user.ChangePasswordOnFirstLogon = false;
                user.ChangePassword = DateTime.Today;

                unitOfWork.GetRepository<User>().Update(user);

                unitOfWork.SaveChanges();
            }
        }

        public void ValidateLogin(IUnitOfWork unitOfWork, User objsrc, User objdest)
        {
            var config = _settingsService.GetValue(Base.Security.Consts.KEY_CONFIG, null) as Config;

            int LOGIN_MIN_LEN = config != null ? config.MinLenLogin : 6;

            if (objsrc.Login.Length < LOGIN_MIN_LEN)
            {
                throw new Exception(String.Format("Введите корректный логин (минимальная длина логина равна {0})", LOGIN_MIN_LEN));
            }

            var user = GetUser(unitOfWork, objsrc.Login);

            if (user != null && user.ID != objsrc.ID)
            {
                throw new Exception("Пользователь с таким логином уже существует");
            }
        }

        public void ClearAll()
        {
            _cacheWrapper.Remove(_keyCache);
        }

        public void Clear(string login)
        {
            GetUserCache().Remove(login);
        }

        public void Clear(ISecurityUser securityUser)
        {
            Clear(securityUser.Login);
        }

        public User RegisterUser(ISystemUnitOfWork unitOfWork, User user)
        {
            var config = _settingsService.GetValue(Base.Security.Consts.KEY_CONFIG, null) as Config;
#if !DEBUG
            if (config != null && config.AllowRegistration == false)
                throw new InvalidOperationException(String.Format(
                        "Регистрация новых пользователей запрещена администратором системы", user.Login));
#endif

            var oldUser = this.GetUser(unitOfWork, user.Login);

            if (oldUser != null && !oldUser.IsUnregistered)
                throw new InvalidOperationException(String.Format(
                    "Пользователь с логином {0} уже имеется в системе", user.Login));

            user.Password = new PasswordCryptographer().GenerateSaltedPassword(user.Password);


            if (user.Roles == null || !user.Roles.Any())
            {
                var roleRep = unitOfWork.GetRepository<Role>();

                var externalRole = roleRep.Find(x => x.SystemRole == SystemRole.Base);
                if (externalRole == null)
                    throw new InvalidOperationException("В системе отсутствует базовая роль");

                user.Roles = new Collection<Role> { externalRole };
            }


            var catRep = unitOfWork.GetRepository<UserCategory>();

            string systemName = UserType.Base.ToString();
            string name = UserType.Base.GetDescription();

            var userCategory = catRep.Find(x => x.SystemName == systemName);

            if (userCategory != null)
                user.CategoryID = userCategory.ID;
            else
                user.UserCategory = new UserCategory { Name = name, SystemName = systemName };


            if (user.ID == 0)
                unitOfWork.GetRepository<User>().Create(user);
            else
                unitOfWork.GetRepository<User>().Update(user);

            unitOfWork.SaveChanges();

            return user;
        }

        #region ISecurityService
        //        ISecurityUser ISecurityUserService.GetSecurityUser(IUnitOfWork unitOfWork, string login)
        //        {
        //            return this.GetSecurityUser(unitOfWork, login);
        //        }
        #endregion
    }

    public interface IGuestRoleProvider
    {
        ICollection<Role> GetRoles();
    }
}
