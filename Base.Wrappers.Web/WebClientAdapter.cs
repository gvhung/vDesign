using Framework.Wrappers;
using System;
using System.Net;

namespace Base.Wrappers.Web
{
    public class WebClientAdapter : WebClient, IWebClientAdapter
    {
        public WebClientAdapter()
        {
            this.Headers["User-Agent"] = "Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 6.2; Trident/4.0; SLCC2; .NET CLR 2.0.50727; .NET CLR 3.5.30729; .NET CLR 3.0.30729; Media Center PC 6.0)";
        }

        protected override WebRequest GetWebRequest(Uri address)
        {
            var request = base.GetWebRequest(address);
            request.Timeout = 60000;
            return request;
        }
    }
}
