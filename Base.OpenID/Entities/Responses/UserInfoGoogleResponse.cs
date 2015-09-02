namespace Base.OpenID.Entities.Responses
{
    public class UserInfoGoogleResponse : IUserInfoResponse
    {
        public string id { get; set; }
        public string email { get; set; }
        public bool? verified_email { get; set; }
        public string name { get; set; }
        public string given_name { get; set; }
        public string family_name { get; set; }
        public string link { get; set; }
        public string picture { get; set; }
        public string gender { get; set; }
        public string locale { get; set; }

        public ExtAccount ToAccount()
        {
            return new ExtAccount
            {
                Type = ServiceType.Google,
                ExternalId = id,
                Login = email,
                Email = email,
                FirstName = given_name,
                LastName = family_name,
                ProfileLink = link,
                ProfilePicture = picture
            };
        }
    }
}
