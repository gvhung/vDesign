namespace Base.OpenID.Entities.Responses
{
    public class AuthResponse
    {
        public string code { get; set; }
        public string token { get; set; }
        public string prompt { get; set; }
        public string state { get; set; }
        public string error { get; set; }
        public string error_description { get; set; }
    }
}
