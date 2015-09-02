using Base.Notification.Service.Abstract;
using Base.Service;
using Base.Service.Log;
using Framework.Wrappers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;

namespace Base.Notification.Service.Concrete
{
    public class DummyEmailService : IEmailService
    {
        public bool SendMail(string mailto, string caption, string message, bool isBodyHtml = false)
        {
            return false;
        }

        public int SendMail(IEnumerable<string> mailto, string caption, string message, bool isBodyHtml = false)
        {
            return 0;
        }

        public bool SendMail(string mailto, string caption, string templateName, Dictionary<string, string> placeholders, bool isBodyHtml = false)
        {
            return false;
        }

        public int SendMail(IEnumerable<string> mailto, string caption, string templateName, Dictionary<string, string> placeholders, bool isBodyHtml = false)
        {
            return 0;
        }
    }

    public class EmailService : IEmailService
    {
        private const string TEMPLATE_CACHE_KEY = "{56C7C354-FFDF-446F-8E71-CF389CAF6B05}";
        private static readonly object CacheLocker = new object();

        private readonly IEmailSettingsService _emailSettingsService;
        private readonly IPathHelper _pathHelper;
        private readonly ISystemUrlHelper _urlHelper;
        private readonly ICacheWrapper _cache;
        private readonly ILogService _logService;

        public EmailService(IEmailSettingsService emailSettingsService, IPathHelper pathHelper, ISystemUrlHelper urlHelper, ICacheWrapper cache, ILogService logService)
        {
            _emailSettingsService = emailSettingsService;
            _pathHelper = pathHelper;
            _urlHelper = urlHelper;
            _cache = cache;
            _logService = logService;
        }

        public bool SendMail(string mailto, string caption, string message, bool isBodyHtml = false)
        {
            return SendMail(new[] { mailto }, caption, message, isBodyHtml) > 0;
        }

        public int SendMail(IEnumerable<string> mailto, string caption, string message, bool isBodyHtml = false)
        {
            int count = 0;

            var mails = mailto.Where(IsValidEmail).Select(to => CreateMailMessage(to, caption, message, isBodyHtml)).ToList();

            var client = new SmtpClient
            {
                Host = _emailSettingsService.SmtpServer,
                Port = _emailSettingsService.SmtpPort,
                EnableSsl = _emailSettingsService.UseSSL,
                Credentials = new NetworkCredential(_emailSettingsService.AccountName/*.Split('@').First()*/, _emailSettingsService.AccountPassword),
                DeliveryMethod = SmtpDeliveryMethod.Network
            };

            //_logService.Log(String.Format("SendMail -- mails.count: {0}", mails.Count()));

            foreach (var mail in mails)
            {
                try
                {
                    client.Send(mail);

                    count++;
                }
                catch(Exception e)
                {
                    //_logService.Log(String.Format("SendMail -- Mail: {0}; Error: {1}", mail, e.Message));
                }
            }

            client.Dispose();
            
            return count;
        }

        private MailMessage CreateMailMessage(string mailto, string caption, string message, bool isBodyHtml)
        {
            var mail = new MailMessage
            {
                From = String.IsNullOrEmpty(_emailSettingsService.AccountTitle)
                    ? new MailAddress(_emailSettingsService.AccountName)
                    : new MailAddress(_emailSettingsService.AccountName, _emailSettingsService.AccountTitle),
                Subject = caption,
                Body = message,
                IsBodyHtml = isBodyHtml,
                BodyEncoding = Encoding.UTF8,
                HeadersEncoding = Encoding.UTF8,
                SubjectEncoding = Encoding.UTF8
            };
            mail.To.Add(mailto);

            return mail;
        }


        public bool SendMail(string mailto, string caption, string templateName, Dictionary<string, string> placeholders, bool isBodyHtml = false)
        {
            return SendMail(new[] { mailto }, caption, templateName, placeholders, isBodyHtml) > 0;
        }

        public int SendMail(IEnumerable<string> mailto, string caption, string templateName, Dictionary<string, string> placeholders, bool isBodyHtml = false)
        {
            string body = BuildBody(templateName, placeholders);

            return SendMail(mailto, caption, body, isBodyHtml);
        }

        private string BuildBody(string templateName, Dictionary<string, string> placeholders)
        {
            AddUrlsToDictionary(placeholders);

            string template = (GetTemplateContent(templateName)).ReplacePlaceholders(placeholders).DeleteImages();

            return template;
        }

        private string GetTemplateContent(string templateName)
        {
            Dictionary<string, string> emailTemplates;
            lock (CacheLocker)
            {
                emailTemplates = _cache.Get(TEMPLATE_CACHE_KEY) as Dictionary<string, string>;

                if (emailTemplates == null)
                {
                    emailTemplates = new Dictionary<string, string>();
                    _cache.Add(TEMPLATE_CACHE_KEY, emailTemplates);
                }
                else if (emailTemplates.ContainsKey(templateName))
                {
                    return emailTemplates[templateName];
                }
            }

            FileInfo fi = new FileInfo(Path.Combine(_pathHelper.GetAppDataDirectory(), "Templates", "Email", templateName + ".html"));
            if (!fi.Exists) return "";

            string template;
            using (var fs = File.OpenText(fi.FullName))
            {
                template = fs.ReadToEnd();
            }

            lock (CacheLocker)
            {
                emailTemplates[templateName] = template;
            }

            return template;
        }

        private void AddUrlsToDictionary(IDictionary<string, string> dic)
        {
            if (!dic.ContainsKey("AdminUrl")) dic.Add("AdminUrl", _urlHelper.AdminUrl);
            if (!dic.ContainsKey("PublicUrl")) dic.Add("PublicUrl", _urlHelper.PublicUrl);
        }

        private static bool IsValidEmail(string email)
        {
            return new EmailAddressAttribute().IsValid(email);
        }
    }

    public static class CustomMailerExtensions
    {
        public static string ReplacePlaceholders(this string template, Dictionary<string, string> dic)
        {
            return dic.Keys.Aggregate(template, (current, key) => current.Replace(String.Format("[{0}]", key), dic[key]));
        }

        public static string DeleteImages(this string template)
        {
            return Regex.Replace(template, @"<img[^>]*>", String.Empty, RegexOptions.IgnoreCase);
        }
    }
}