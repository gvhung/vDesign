using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace Base.Security
{
    public class UserConfirmRequest : BaseObject
    {
        [ForeignKey("UserID")]
        public User User { get; set; }
        public int UserID { get; set; }
        public DateTime RequestTime { get; set; }
        public DateTime ValidUntil { get; set; }
        public string Code { get; set; }
        public bool Used { get; set; }
        public DateTime? UseTime { get; set; }
        public ConfirmType Type { get; set; }
    }

    public enum ConfirmType
    {
        [Description("Регистрация")]
        NewUser = 0,
        [Description("Восстановление пароля")]
        ResetPassword = 1
    }
}
