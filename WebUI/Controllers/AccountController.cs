using Base.Ambient;
using Base.DAL;
using Base.Security;
using Base.Security.Service;
using Base.Security.Service.Abstract;
using Base.UI;
using Framework;
using Framework.Attributes;
using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Security;
using WebUI.Models;

namespace WebUI.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAccountManager _accountManager;
        private readonly IMenuService _menuService;
        private readonly IPresetService _presetService;
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly ISecurityUserService _securityUserService;

        public AccountController(ISecurityUserService securityUserService, IMenuService menuService,
            IAccountManager accountManager, IPresetService presetService, IUnitOfWorkFactory unitOfWorkFactory)
        {
            _securityUserService = securityUserService;
            _menuService = menuService;
            _accountManager = accountManager;
            _presetService = presetService;
            _unitOfWorkFactory = unitOfWorkFactory;
        }

        [HttpGet]
        [AllowAnonymous]
        [ImportModelStateFromTempData]
        public ActionResult LogOn(string returnUrl)
        {
            Session["LogOnAttempt"] = 0;

            const int logOnAttemptLock = 6;
            const int logOnAttemptCaptcha = 3;

            ViewBag.ReturnUrl = returnUrl;
            ViewBag.LOG_ON_ATTEMPT_LOCK = logOnAttemptLock;
            ViewBag.LOG_ON_ATTEMPT_CAPTCHA = logOnAttemptCaptcha;

            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult LogOn(LogOnModel model, string returnUrl)
        {
            int logOnAttempt = Convert.ToInt32(Session["LogOnAttempt"]);

            const int logOnAttemptLock = 6;
            const int logOnAttemptCaptcha = 3;

            ViewBag.LOG_ON_ATTEMPT_LOCK = logOnAttemptLock;
            ViewBag.LOG_ON_ATTEMPT_CAPTCHA = logOnAttemptCaptcha;

            if (ModelState.IsValid)
            {
                if ((logOnAttempt >= logOnAttemptCaptcha) &&
                    (Session["Captcha"] == null || Session["Captcha"].ToString() != model.Captcha))
                {
                    ModelState.AddModelError("Captcha", @"Неверно указан результат вычисления");
                }

                if (ModelState.IsValid)
                {
                    if (_securityUserService.ValidateUser(model.Login, model.Password))
                    {
                        FormsAuthentication.SetAuthCookie(model.Login, model.RememberMe);

                        Session["LogOnAttempt"] = 0;

                        if (Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/")
                            && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\"))
                        {
                            return Redirect(returnUrl);
                        }
                        return RedirectToAction("Index", "Home");
                    }
                    ModelState.AddModelError("", @"Введено неправильное имя пользователя или пароль");
                }
            }
            else
            {
                ModelState.AddModelError("", @"Некорректный логин или пароль");
            }

            if (logOnAttempt >= logOnAttemptLock)
            {
                ModelState.AddModelError("",
                    String.Format(
                        "После {0} неудачных попыток войти, браузер должен быть закрыт. Пожалуйста, обратись к администратору за детальной информации о вашем входе.",
                        logOnAttemptLock));
            }


            Session["LogOnAttempt"] = logOnAttempt + 1;
            model.Captcha = "";
            ViewBag.ReturnUrl = returnUrl;

            return View(model);
        }

        public ActionResult LogOff()
        {
            if (AppContext.SecurityUser != null)
            {
                _securityUserService.Clear(AppContext.SecurityUser);
                _menuService.Clear(AppContext.SecurityUser);
                _presetService.SessionClear();
            }

            FormsAuthentication.SignOut();

            return RedirectToAction("Index", "Home", new { area = "Regulation" });
        }

        [Authorize]
        [HttpPost]
        public JsonNetResult ChangePassword(int id, string newPass)
        {
            int error = 0;
            string message;
            try
            {
                _securityUserService.ChangePassword(id, newPass);
                message = "Пароль успешно изменен!";
            }
            catch (Exception e)
            {
                error = 1;
                message = e.Message;
            }

            return new JsonNetResult(new
            {
                error,
                message
            });
        }

        [Authorize]
        [HttpGet]
        public PartialViewResult ChangePasswordOnFirstLogon()
        {
            return PartialView();
        }

        [Authorize]
        [HttpPost]
        public ActionResult ChangePasswordOnFirstLogon(ChangePasswordModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _securityUserService.ChangePassword(AppContext.SecurityUser.ID, model.NewPassword);
                    _securityUserService.Clear(AppContext.SecurityUser);

                    return RedirectToAction("GetViewModel", "Standart",
                        new { mnemonic = "ExtProfile", typeDialog = TypeDialog.Frame, id = AppContext.SecurityUser.ID });
                }
                catch (Exception e)
                {
                    ModelState.AddModelError("", e.Message);
                }
            }

            return View(model);
        }

        public PartialViewResult AccessDenied()
        {
            return PartialView();
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult PasswordRecovery(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> PasswordRecovery(PasswordRecoveryModel model)
        {
            if (ModelState.IsValid)
            {
                var capcha = Session["Captcha"];
                if (capcha == null || capcha.ToString() != model.Captcha)
                {
                    ModelState.AddModelError("Captcha", @"Неверно указан результат вычисления");
                }

                if (ModelState.IsValid)
                {
                    using (var uow = _unitOfWorkFactory.CreateSystem())
                    {
                        try
                        {
                            await _accountManager.ResetPassword(uow, Request.Url, model.Login);
                        }
                        catch (Exception e)
                        {
                            ModelState.AddModelError("", e.Message);
                        }

                        if (ModelState.IsValid)
                        {
                            return View("Info", new AccountInfoModel
                            {
                                Title = "Восстановление пароля",
                                Message = "Для восстановления пароля, пожалуйста, перейдите по ссылке, отравленной вам на почту."
                            });
                        }
                    }
                }
            }
            model.Captcha = "";
            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult VerifyConfirmation(ConfirmType type, int userId, string code)
        {
            using (var uow = _unitOfWorkFactory.Create())
            {
                try
                {
                    var user = _accountManager.ConfirmationVerify(uow, type, userId, code);
                    FormsAuthentication.SetAuthCookie(user.Login, true);

                    switch (type)
                    {
                        case ConfirmType.ResetPassword:
                            return RedirectToAction("ChangePasswordOnFirstLogon", "Account");

                        case ConfirmType.NewUser:
                            return RedirectToAction("GetViewModel", "Standart",
                                new { mnemonic = "ExtProfile", typeDialog = TypeDialog.Frame, id = user.ID });

                        default:
                            throw new NotImplementedException();
                    }
                }
                catch (Exception e)
                {
                    return View("Info", new AccountInfoModel { Title = type.GetDescription(), Message = e.Message });
                }
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Register(NewUserModel model)
        {
            string message;
            bool isValid = _securityUserService.IsValidPassword(model.Password, out message);

            if (!isValid) ModelState.AddModelError("Password", message);
            if (!ModelState.IsValid) return View(model);

            using (var uow = _unitOfWorkFactory.CreateSystem())
            {
                try
                {
                    _accountManager.Register(uow, Request.Url, model.Email, model.Password, model.FirstName, model.LastName);

                    return View("Info", new AccountInfoModel
                    {
                        Title = "Регистрация",
                        Message = "На указанный вами почтовый адрес отправлено письмо. Для завершения регистрации перейдите по содержащейся в нем ссылке."
                    });
                }
                catch (Exception e)
                {
                    ModelState.AddModelError("", e.Message);
                }
            }
            return View(model);
        }



    }
}