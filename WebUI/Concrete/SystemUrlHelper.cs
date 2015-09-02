using Base.Notification.Service.Abstract;
using System.Collections.Specialized;
using System.Configuration;

namespace WebUI.Concrete
{
    public class SystemUrlHelper : ISystemUrlHelper
    {
        private readonly NameValueCollection _manager = ConfigurationManager.AppSettings;

        public SystemUrlHelper()
        {
            PublicUrl = _manager["PublicUrl"];
            AdminUrl = _manager["AdminUrl"];
        }

        public string PublicUrl { get; private set; }

        public string AdminUrl { get; private set; }
    }
}