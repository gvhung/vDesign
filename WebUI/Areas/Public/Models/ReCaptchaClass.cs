using System.Collections.Generic;
using Newtonsoft.Json;

namespace WebUI.Areas.Public.Models
{
    public class ReCaptchaClass
    {
        const string PrivateKey = "6LcF9RMTAAAAAG-nruV4Gxe09I-Qn8pbBP0p0-gu";

        public static string Validate(string encodedResponse, string address = "")
        {
            var client = new System.Net.WebClient();

            var googleReply = client.DownloadString(string.IsNullOrEmpty(address) ?
                $"https://www.google.com/recaptcha/api/siteverify?secret={PrivateKey}&response={encodedResponse}" :
                $"https://www.google.com/recaptcha/api/siteverify?secret={PrivateKey}&response={encodedResponse}&remoteip={address}");

            var captchaResponse = JsonConvert.DeserializeObject<ReCaptchaClass>(googleReply);

            return captchaResponse.Success;
        }

        [JsonProperty("success")]
        public string Success { get; set; }

        [JsonProperty("error-codes")]
        public List<string> ErrorCodes { get; set; }
    }
}