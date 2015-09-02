using System;
using System.Linq;

namespace Base.OpenID.Entities
{
    public class OpenIdConfig
    {
        public ServiceType Type { get; set; }
        public string IconCssClass { get; set; }
        public string UserInfoObjectType { get; set; }
        public string AuthUri { get; set; }
        public string AccessUri { get; set; }
        public string UserInfoUri { get; set; }
        public string RedirectUri { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string Scope { get; set; }

        public virtual string GetAuthQuery(OpenIdAuthMode mode, string antiForgeryToken)
        {
            return GetAuthQueryUrl(mode, antiForgeryToken).Split('?').Last();
        }

        public virtual string GetAuthQueryUrl(OpenIdAuthMode mode, string antiForgeryToken)
        {
            return AuthUri
                    .Replace("{ClientId}", ClientId)
                    .Replace("{Scope}", Scope)
                    .Replace("{RedirectUri}", RedirectUri)
                    .Replace("{State}", String.Format("{0}|{1}|{2}", Type, mode, antiForgeryToken));
        }

        public virtual string GetAccessUrl()
        {
            return AccessUri.Split('?').First();
        }

        public virtual string GetAccessQuery(string code)
        {
            return GetAccessQueryUrl(code).Split('?').Last();
        }

        public virtual string GetAccessQueryUrl(string code)
        {
            return AccessUri
                    .Replace("{Code}", code)
                    .Replace("{ClientId}", ClientId)
                    .Replace("{ClientSecret}", ClientSecret)
                    .Replace("{RedirectUri}", RedirectUri);
        }

        public virtual string GetUserInfoQuery(string token)
        {
            return GetUserInfoQueryUrl(token).Split('?').Last();
        }

        public virtual string GetUserInfoQueryUrl(string token)
        {
            return UserInfoUri.Replace("{AccessToken}", token);
        }
    }

    public enum OpenIdAuthMode
    {
        LogOn,
        AddToExisting
    }
}
