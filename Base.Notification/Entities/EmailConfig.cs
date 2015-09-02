using Base.Attributes;
using System;
using System.ComponentModel.DataAnnotations;

namespace Base.Notification.Entities
{
    [Serializable]
    public class EmailConfig: BaseObject
    {
        [DetailView("Адрес SMTP-сервера")]
        [MaxLength(255)]
        public string SmtpServerAddress { get; set; }

        [DetailView("Порт SMTP-сервера")]
        public int SmtpServerPort { get; set; }

        [DetailView("Использовать SSL")]
        public bool UseSsl { get; set; }

        [DetailView("Логин")]
        [MaxLength(255)]
        public string AccountLogin { get; set; }

        [DetailView("Пароль")]
        [MaxLength(255)]
        public string AccountPassword { get; set; }

        [DetailView("Отображаемое имя отправителя")]
        [MaxLength(255)]
        public string AccountTitle { get; set; }

        [DetailView("Отправлять уведомления на почту", Order = 20)]
        public bool EnableEmail { get; set; }

        [DetailView("Задержка перед отправкой (мин)", Order = 21)]
        public int Delay { get; set; }
    }
}
