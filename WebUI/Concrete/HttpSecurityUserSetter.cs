using Base.Ambient;
using Base.Security;
using Base.Security.Service;
using Base.Security.Service.Abstract;
using System;
using System.Web;

namespace WebUI.Concrete
{
    public class HttpSecurityUserResolver
    {
        private readonly ISecurityUserService _securityUserService;
        private readonly IGuestUserService _guestUserService;
        private readonly IAppContextBootstrapper _appContextBootstrapper;
        private const string GuestAuthCookieKey = "GUEST_AUTH";

        public HttpSecurityUserResolver(
            ISecurityUserService securityUserService,
            IGuestUserService guestUserService,
            IAppContextBootstrapper appContextBootstrapper)
        {
            _securityUserService = securityUserService;
            _guestUserService = guestUserService;
            _appContextBootstrapper = appContextBootstrapper;
        }

        public void SetSecurityUser(HttpContextBase httpContext)
        {
            ISecurityUser securityUser = null;

            if (httpContext.User.Identity.IsAuthenticated)
            {
                securityUser = _securityUserService.GetSecurityUser(httpContext.User.Identity.Name);
            }
            
            if (securityUser == null)
            {
                string authValue = null;

                var guestCookie = httpContext.Request.Cookies[GuestAuthCookieKey];

                if (guestCookie != null)
                {
                    Guid temp;

                    if(Guid.TryParse(guestCookie.Value, out temp))
                    {
                        authValue = guestCookie.Value;
                    }
                }

                securityUser = _guestUserService.GetGuestUser(authValue);

                httpContext.Response.Cookies.Add(new HttpCookie(GuestAuthCookieKey, securityUser.Login)
                {
                    Expires = DateTime.Now.AddDays(20)
                });
            }

            _appContextBootstrapper.SetSecurityUser(securityUser);
        }
    }
}