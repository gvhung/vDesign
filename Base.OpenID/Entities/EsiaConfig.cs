using System;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace Base.OpenID.Entities
{
    public class EsiaConfig : OpenIdConfig
    {

        public override string GetAuthQueryUrl(OpenIdAuthMode mode, string antiForgeryToken)
        {
            string state = String.Format("{0}|{1}|{2}", Type, mode, antiForgeryToken);
            string timestamp = CreateTimestamp();

            string clientSecret = CreateClientSecret(state, timestamp);

            return AuthUri
                    .Replace("{ClientId}", ClientId)
                    .Replace("{Scope}", Scope)
                    .Replace("{RedirectUri}", RedirectUri)
                    .Replace("{State}", state)
                    .Replace("{ClientSecret}", clientSecret)
                    .Replace("{Timestamp}", timestamp);
        }

        private string CreateTimestamp()
        {
            return DateTime.Now.ToString("yyyy.MM.dd HH:mm:ss zzz");
            //return DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss");
        }

        private string CreateClientSecret(string state, string timestamp)
        {
            var data = Encoding.UTF8.GetBytes(Scope + timestamp + ClientId + state);

            var signature = Sign(data);

            return HttpServerUtility.UrlTokenEncode(signature);
        }

        public static byte[] Sign(byte[] data)
        {
            if (data == null) throw new ArgumentNullException("data");

            CspParameters cspParam = new CspParameters(75, null);
            cspParam.KeyContainerName = "89211541@2015-05-19-Драчёв Александр Сергеевич";//rsacsp.CspKeyContainerInfo.KeyContainerName;
            cspParam.Flags = CspProviderFlags.NoPrompt;

            try
            {
                return null;
                //var aescsp = new Gost3410CryptoServiceProvider(cspParam);
                //return aescsp.SignData(data, "GOST3411");
            }
            catch
            {
                return new byte[0];
            }
        }
    }
}
