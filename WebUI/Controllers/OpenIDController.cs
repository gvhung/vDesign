using Base.OpenID;
using Base.OpenID.Entities;
using Base.OpenID.Entities.Responses;
using Base.OpenID.Service.Abstract;
using Framework.Attributes;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Security;
using WebUI.Authorize;
using WebUI.Models;

namespace WebUI.Controllers
{
    [AllowAnonymous]
    [AllowGuest]
    public class OpenIDController : BaseController
    {
        private static readonly string AntiForgeryKey = Guid.NewGuid().ToString("N");

        private readonly IOpenIdService _openIdService;
        private readonly IOpenIdConfigService _openIdConfigService;

        public OpenIDController(IBaseControllerServiceFacade serviceFacade, IOpenIdService openIdService, 
                                IOpenIdConfigService openIdConfigService)
            : base(serviceFacade)
        {
            _openIdService = openIdService;
            _openIdConfigService = openIdConfigService;
        }
        
        [ChildActionOnly]
        public PartialViewResult OpenIdMenu(OpenIdAuthMode mode = OpenIdAuthMode.LogOn, bool isVertical = false)
        {
            string antiForgeryToken = Guid.NewGuid().ToString("N");
            Session[AntiForgeryKey] = antiForgeryToken;

            var model = new OpenIdMenu(_openIdConfigService.GetConfig(), mode, antiForgeryToken, isVertical);
            return PartialView("_OpenIdMenu", model);
        }

        [HttpGet]
        [ExportModelStateToTempData]
        public async Task<ActionResult> OAuth2Callback(AuthResponse response)
        {
            var respParams = response.state.Split('|');

            ServiceType type;
            OpenIdAuthMode mode;

            if (respParams.Count() != 3 || !Enum.TryParse(respParams[0], out type) || !Enum.TryParse(respParams[1], out mode))
                return _LogOnError("Wrong parameter value: 'state'");

            if (Session[AntiForgeryKey] as string != respParams[2])
                return _LogOnError("Forgery attack protection");
            
            string error = !String.IsNullOrEmpty(response.error_description)
                ? response.error_description
                : response.error;

            switch (mode)
            {
                case OpenIdAuthMode.LogOn:
                    if (!String.IsNullOrEmpty(error))
                        return _LogOnError("Вход с помощью внешнего аккаунта выполнен с ошибкой: " + error);

                    try
                    {
                        using (var uow = CreateSystemTransactionUnitOfWork())
                        {
                            var user = await _openIdService.Authorize(uow, type, response.code);
                            uow.Commit();

                            FormsAuthentication.SetAuthCookie(user.Login, true);
                            return RedirectToAction("Index", "Home");
                        }
                    }
                    catch (Exception e)
                    {
                        return _LogOnError(e.Message);
                    }

                case OpenIdAuthMode.AddToExisting:
                    if (!Request.IsAuthenticated)
                    {
                        return _LogOnError("Ошибка аутентификации пользователя");
                    }

                    if (String.IsNullOrEmpty(error))
                    {
                        try
                        {
                            using(var uow = CreateUnitOfWork())
                            {
                                await _openIdService.AddAccount(uow, type, response.code);
                            }

                        }
                        catch (Exception e)
                        {
                            TempData["OpenIDError"] = e.Message;
                        }
                    }
                    else
                    {
                        TempData["OpenIDError"] = "Добавление внешнего аккаунта выполнено с ошибкой: " + error;
                    }
                    
                    return RedirectToAction("GetViewModel", "Standart", new { mnemonic = typeof(ExtProfile).Name, TypeDialog = TypeDialog.Frame, id = SecurityUser.ID });
                    

                default:
                    return _LogOnError("Wrong parameter value: 'mode'");
            }
        }
        
        private RedirectToRouteResult _LogOnError(string message)
        {
            ModelState.AddModelError("", message);
            return RedirectToAction("LogOn", "Account");
        }
    }

}
