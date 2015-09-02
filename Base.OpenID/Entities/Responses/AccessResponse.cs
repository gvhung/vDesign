namespace Base.OpenID.Entities.Responses
{
    public class AccessResponse
    {
        public string access_token { get; set; }
        public string token_type { get; set; }
        public int expires_in { get; set; }
        public string id_token { get; set; }
    }
}
