using System;
using System.Collections.Generic;

namespace Base.OpenID.Entities.Responses
{
    public class UserInfoYandexResponse : IUserInfoResponse
    {
        public string id { get; set; }
        public string login { get; set; }
        public string default_email { get; set; }
        public List<string> emails { get; set; }
        public string display_name { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string real_name { get; set; }
        public string sex { get; set; }
        public DateTime? birthday { get; set; }

        public ExtAccount ToAccount()
        {
            return new ExtAccount
            {
                Type = ServiceType.Yandex,
                ExternalId = id,
                Login = default_email,
                Email = default_email,
                FirstName = first_name,
                LastName = last_name,
                ProfileLink = "",
                ProfilePicture = String.Format("http://avatars.yandex.net/get-yapic/{0}/islands-200", id)
            };
        }
    }
}
