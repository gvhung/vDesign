using Base.Service;
using System.Collections.Generic;

namespace Base.Notification.Service.Abstract
{
    public interface IEmailService : IService
    {
        bool SendMail(string mailto, string caption, string message, bool isBodyHtml = false);
        int SendMail(IEnumerable<string> mailto, string caption, string message, bool isBodyHtml = false);
        bool SendMail(string mailto, string caption, string templateName, Dictionary<string, string> placeholders, bool isBodyHtml = false);
        int SendMail(IEnumerable<string> mailto, string caption, string templateName, Dictionary<string, string> placeholders, bool isBodyHtml = false);
    }
}
