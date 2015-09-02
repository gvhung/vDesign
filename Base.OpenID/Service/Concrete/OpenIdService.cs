using Base.DAL;
using Base.OpenID.Entities;
using Base.OpenID.Entities.Responses;
using Base.OpenID.Service.Abstract;
using Base.Security;
using Base.Security.Service;
using Base.Service;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Base.OpenID.Service.Concrete
{
    public class OpenIdService : IOpenIdService
    {
        private readonly ISecurityUserService _securityUserService;
        private readonly IExtAccountService _extAccountService;
        private readonly IOpenIdConfigService _openIdConfigService;
        private readonly IFileSystemService _fileSystemService;

        public OpenIdService(ISecurityUserService securityUserService, IExtAccountService extAccountService,
                             IOpenIdConfigService openIdConfigService, IFileSystemService fileSystemService)
        {
            _securityUserService = securityUserService;
            _extAccountService = extAccountService;
            _openIdConfigService = openIdConfigService;
            _fileSystemService = fileSystemService;
        }

        public async Task<ExtAccount> GetAccountInfo(ServiceType type, string code)
        {
            AccessResponse access = await _GetAccess(type, code);
            return await _GetAccountInfo(type, access);
        }

        private async Task<AccessResponse> _GetAccess(ServiceType type, string code)
        {
            if (String.IsNullOrEmpty(code)) throw new ArgumentNullException("code");

            OpenIdConfig config = _openIdConfigService.GetConfig(type);
            if (config == null) throw new InvalidOperationException("No config file was found");

            WebRequest request = WebRequest.Create(new Uri(config.GetAccessUrl())) as HttpWebRequest;

            string postData = config.GetAccessQuery(code);
            Byte[] data = Encoding.UTF8.GetBytes(postData);

            request.Method = WebRequestMethods.Http.Post;
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = data.Length;

            using (Stream stream = request.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
            }

            WebResponse response = await request.GetResponseAsync();
            string responseContent = _ReadStreamFromResponse(response);
            AccessResponse responseObj;

            try
            {
                responseObj = JsonConvert.DeserializeObject<AccessResponse>(responseContent);
            }
            catch (JsonReaderException)
            {
                var qparse = HttpUtility.ParseQueryString(responseContent);
                if (qparse["access_token"] != null)
                {
                    responseObj = new AccessResponse { access_token = qparse["access_token"] };
                }
                else
                {
                    throw;
                }
            }

            return responseObj;
        }

        private async Task<ExtAccount> _GetAccountInfo(ServiceType type, AccessResponse access)
        {
            if (access == null) throw new ArgumentNullException("access");

            var config = _openIdConfigService.GetConfig(type);

            string url = config.GetUserInfoQueryUrl(access.access_token);
            WebRequest request = WebRequest.Create(new Uri(url)) as HttpWebRequest;
            if (request == null) return null;

            request.Method = WebRequestMethods.Http.Get;

            WebResponse response = await request.GetResponseAsync();
            string responseContent = _ReadStreamFromResponse(response);

            var responseObj = JsonConvert.DeserializeObject(responseContent, Type.GetType(config.UserInfoObjectType)) as IUserInfoResponse;

            if (responseObj == null) return null;

            ExtAccount extAccount = responseObj.ToAccount();
            //externalUserInfo.AccessToken = access.access_token;
            //externalUserInfo.ExpiresIn = access.expires_in;
            //externalUserInfo.LastAccess = DateTime.Now;

            return extAccount;
        }

        private static string _ReadStreamFromResponse(WebResponse response)
        {
            using (Stream responseStream = response.GetResponseStream())
                if (responseStream != null)
                    using (StreamReader sr = new StreamReader(responseStream))
                    {
                        return sr.ReadToEnd();
                    }
            return null;
        }

        public async Task<User> Authorize(ISystemUnitOfWork unitOfWork, ServiceType type, string code)
        {
            ExtAccount extAccount = await GetAccountInfo(type, code);
            return Authorize(unitOfWork, extAccount);
        }

        private User Authorize(ISystemUnitOfWork unitOfWork, ExtAccount extAccount)
        {
            User user = _extAccountService.GetAll(unitOfWork)
                     .Where(x => x.Type == extAccount.Type && x.ExternalId == extAccount.ExternalId)
                     .Select(x => x.User).FirstOrDefault();

            if (user == null)
            {
                var securityUser = _securityUserService.GetSecurityUser(extAccount.Email);

                if (securityUser != null)
                {
                    AddAccount(unitOfWork, extAccount);
                    user = new User { Login = securityUser.Login };
                }
                else
                {
                    user = RegisterUser(unitOfWork, extAccount);
                }
            }

            return user;

        }

        private User RegisterUser(ISystemUnitOfWork unitOfWork, ExtAccount extAccount)
        {
            User user = _securityUserService.RegisterUser(unitOfWork, extAccount.ToUser());

            if (user == null) return null;

            extAccount.User = user;
            _AttachImage(unitOfWork, user, extAccount.ProfilePicture);
            _extAccountService.Create(unitOfWork, extAccount);

            return user;
        }

        private void _AttachImage(ISystemUnitOfWork unitOfWork, User user, string url)
        {
            if (!Uri.IsWellFormedUriString(url, UriKind.Absolute)) return;

            Exception error;

            var image = _fileSystemService.SaveFile(new Uri(url), out error);

            if (error == null)
            {
                user.Image = image.ToObject<FileData>();
            }
        }

        public async Task<ExtAccount> AddAccount(IUnitOfWork unitOfWork, ServiceType type, string code) //external
        {
            ExtAccount newAccount = await GetAccountInfo(type, code);
            return AddAccount(unitOfWork, newAccount);
        }

        private ExtAccount AddAccount(IUnitOfWork unitOfWork, ExtAccount newAccount)
        {
            var linkedAccounts = _extAccountService.GetLinked(unitOfWork, Ambient.AppContext.SecurityUser.ID);
            if (!linkedAccounts.Any(x => x.Type == newAccount.Type && x.ExternalId == newAccount.ExternalId))
            {
                newAccount.UserID = Ambient.AppContext.SecurityUser.ID;
                _extAccountService.Create(unitOfWork, newAccount);
            }

            return newAccount;
        }
    }
}
