
namespace Base.Notification.Service.Abstract
{
    public interface IEmailSettingsService
    {
        bool EnableEmail { get; set; }
        string SmtpServer { get; set; }
        int SmtpPort { get; set; }
        string AccountName { get; set; }
        string AccountPassword { get; set; }
        string AccountTitle { get; set; }
        bool UseSSL { get; set; }
        int SendDelay { get; set; }
    };

}
