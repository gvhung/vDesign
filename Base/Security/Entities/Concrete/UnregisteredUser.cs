using Base.Attributes;
using System.ComponentModel.DataAnnotations;

namespace Base.Security
{
    public class UnregisteredUser : BaseObject
    {
        [MaxLength(100)]
        [DetailView("Имя", Required = true)]
        public string LastName { get; set; }

        [MaxLength(100)]
        [DetailView("Фамилия", Required = true)]
        public string FirstName { get; set; }

        [MaxLength(100)]
        [DetailView("Отчество")]
        public string MiddleName { get; set; }

        [MaxLength(100)]
        [ListView]
        [DetailView("Адрес электронной почты", Required = true)]
        public string Email { get; set; }

        [PropertyDataType(PropertyDataType.PhoneNumber)]
        [MaxLength(100)]
        [DetailView("Рабочий телефон")]
        public string OfficePhone { get; set; }

        [PropertyDataType(PropertyDataType.PhoneNumber)]
        [MaxLength(100)]
        [DetailView("Личный телефон")]
        public string PersonPhone { get; set; }

        [PropertyDataType(PropertyDataType.MultilineText)]
        [MaxLength(255)]
        [ListView]
        [DetailView("Почтовый адрес")]
        public string MailAddress { get; set; }
    }
}