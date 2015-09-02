using System;

namespace Base.OpenID.Entities.Responses
{
    public class UserInfoFacebookResponse : IUserInfoResponse
    {
        public string id { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string gender { get; set; }
        public string link { get; set; }
        public string name { get; set; }
        public string email { get; set; }

        public ExtAccount ToAccount()
        {
            if (String.IsNullOrEmpty(email))
                throw new InvalidOperationException("В полученных данных пользователя Facebook отсутствует информация об электронной почте. Регистрация в системе невозможна.");

            return new ExtAccount
            {
                Type = ServiceType.Facebook,
                ExternalId = id,
                Login = email,
                Email = email,
                FirstName = first_name,
                LastName = last_name,
                ProfileLink = link,
                ProfilePicture = String.Format("http://graph.facebook.com/{0}/picture?type=large", id)
            };
        }
    }
}
