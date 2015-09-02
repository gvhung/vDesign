using Base.Notification.Consts;
using Base.Notification.Entities;
using Base.Notification.Service.Abstract;
using Base.Settings;
using System;
using System.Collections.Specialized;
using System.Configuration;

namespace WebUI.Concrete
{
    public class WebConfigEmailSettingsService : IEmailSettingsService
    {
        private readonly NameValueCollection _manager = ConfigurationManager.AppSettings;

        public WebConfigEmailSettingsService()
        {
            EnableEmail = bool.Parse(_manager["EnableEmail"]);
            SmtpServer = _manager["SMTPServer"];
            SmtpPort = int.Parse(_manager["SMTPport"]);
            AccountName = _manager["EmailAccountName"];
            AccountPassword = _manager["EmailAccountPassword"];
            AccountTitle = _manager["EmailAccountTitle"];
            UseSSL = bool.Parse(_manager["UseSSL"]);
            SendDelay = (int)Double.Parse(_manager["SendDelay"]);
        }

        public bool EnableEmail { get; set; }
        public string SmtpServer { get; set; }
        public int SmtpPort { get; set; }
        public string AccountName { get; set; }
        public string AccountPassword { get; set; }
        public string AccountTitle { get; set; }
        public bool UseSSL { get; set; }
        public int SendDelay { get; set; }
    }

    public class EmailSettingsService : IEmailSettingsService
    {
        public EmailSettingsService(ISettingItemService settingItemService)
        {
            var emailConfig = settingItemService.GetValue(Settings.KEY_CONFIG, null) as EmailConfig;
            if (emailConfig == null) return;

            EnableEmail = emailConfig.EnableEmail;
            SmtpServer = emailConfig.SmtpServerAddress;
            SmtpPort = emailConfig.SmtpServerPort;
            AccountName = emailConfig.AccountLogin;
            AccountPassword = emailConfig.AccountPassword;
            AccountTitle = emailConfig.AccountTitle;
            UseSSL = emailConfig.UseSsl;
            SendDelay = emailConfig.Delay;
        }
        public bool EnableEmail { get; set; }
        public string SmtpServer { get; set; }
        public int SmtpPort { get; set; }
        public string AccountName { get; set; }
        public string AccountPassword { get; set; }
        public string AccountTitle { get; set; }
        public bool UseSSL { get; set; }
        public int SendDelay { get; set; }
    }
}