using Base.Ambient;
using System;
using System.Web.Mvc;

namespace WebUI.Authorize
{
    [AttributeUsage(AttributeTargets.All)]
    public sealed class AllowGuestAttribute : Attribute
    {
    }

    //public class AllowGuestAttributeImpl : AuthorizeAttribute
    //{
    //    private readonly IGuestUserService _guestUserService;
    //    private readonly RijndaelWrapper _encrypter;
    //    private readonly IAppContextBootstrapper _appContextBootstrapper;
    //    private readonly string tempIV = "89071157-AC01-49FA-ADB3-A4E083638573";
    //    private readonly string tempSalt = @"/sXZWLKSB6mc7bWXkDRTJXZd/66xGCm3cwyE/UsZj5hOrfT9a+jSgc4L5VE7CEjbKWPaOxViNTitjsj2IHrQg6czdw/oKuos0vpkgWQ3as0=";
    //    private ISecurityUser _guest;

    //    private const string GuestAuthCookieKey = "GUEST_AUTH";

    //    public AllowGuestAttributeImpl(IGuestUserService guestUserService, RijndaelWrapper encrypter, IAppContextBootstrapper appContextBootstrapper)
    //    {
    //        _guestUserService = guestUserService;
    //        _encrypter = encrypter;
    //        _appContextBootstrapper = appContextBootstrapper;
    //    }

    //    public override void OnAuthorization(AuthorizationContext filterContext)
    //    {
    //        if (!filterContext.HttpContext.User.Identity.IsAuthenticated)
    //        {
    //            if (_guest == null)
    //            {
    //                string authValue = null;

    //                var guestCookie = filterContext.HttpContext.Request.Cookies[GuestAuthCookieKey];
    //                if (guestCookie != null)
    //                {
    //                    authValue = guestCookie.Value;

    //                    try
    //                    {
    //                        _guest = _guestUserService.GetGuestUser(
    //                            _encrypter.Decrypt(authValue, tempIV, tempSalt));
    //                    }
    //                    catch
    //                    {
    //                        // ignored
    //                    }
    //                }

    //                if (_guest == null)
    //                {
    //                    var login = Guid.NewGuid().ToString("N");
    //                    authValue = _encrypter.Encrypt(login, tempIV, tempSalt);

    //                    _guestUserService.CreateGuestUser(login);
    //                    _guest = _guestUserService.GetGuestUser(login);
    //                }

    //                filterContext.HttpContext.Response.Cookies.Add(new HttpCookie(GuestAuthCookieKey, authValue)
    //                {
    //                    Expires = DateTime.Now.AddDays(20)
    //                });
    //            }


    //            _appContextBootstrapper.SetSecurityUser(_guest);

    //            //var controller = filterContext.Controller as ISecurityUserController;
    //            //if (controller != null)
    //            //    controller.SecurityUser = _guest;
    //        }
    //    }
    //}

    public class AuthorizeCustomAttribute : AuthorizeAttribute
    {
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            //if (!filterContext.HttpContext.User.Identity.IsAuthenticated)
            //{
            var action = filterContext.ActionDescriptor;
            if (action.IsDefined(typeof(AllowGuestAttribute), true))
                return;

            var controller = filterContext.ActionDescriptor.ControllerDescriptor;
            if (controller.IsDefined(typeof(AllowGuestAttribute), true))
                return;
            //}

            //string login = filterContext.HttpContext.User.Identity.Name;

            //ISecurityUserService securityUserService = DependencyResolver.Current.GetService<ISecurityUserService>();

            var securityUser = AppContext.SecurityUser;
            if (securityUser == null || securityUser.IsGuest)
            {
                filterContext.Result = new RedirectResult("/Account/LogOff");
            }
            else if(securityUser.ChangePasswordOnFirstLogon)
            {
                filterContext.Result = new RedirectResult("/Account/ChangePasswordOnFirstLogon");
            }

            base.OnAuthorization(filterContext);
        }
    }
}