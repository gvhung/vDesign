using Base.Attributes;
using System.ComponentModel.DataAnnotations;

namespace WebUI.Models
{
    public class LogOnModel
    {
        [Required(ErrorMessage = "Обязательное поле")]
        [Display(Name = "Имя пользователя")]
        public string Login { get; set; }

        [Required(ErrorMessage = "Обязательное поле")]
        [PropertyDataType(PropertyDataType.Password)]
        [Display(Name = "Пароль")]
        public string Password { get; set; }

        [Display(Name = "Запомнить меня")]
        public bool RememberMe { get; set; }

        [Display(Name = "Укажите результат вычисления")]
        public string Captcha { get; set; }
    }

    public class ChangePasswordModel
    {
        [Required(ErrorMessage = "Введите пароль")]
        [PropertyDataType(PropertyDataType.Password)]
        [Display(Name = "Новый пароль")]
        public string NewPassword { get; set; }

        [PropertyDataType(PropertyDataType.Password)]
        [Display(Name = "Подтвердите пароль")]
        [Compare("NewPassword", ErrorMessage = "Пароли не совпадают")]
        public string ConfirmPassword { get; set; }
    }

    public class PasswordRecoveryModel
    {
        [Required(ErrorMessage = "Логин - обязательное поле")]
        public string Login { get; set; }
        
        public string Captcha { get; set; }
    }

    public class AccountInfoModel
    {
        public string Title { get; set; }
        public string Message { get; set; }
    }

    public class NewUserModel
    {
        [Required(ErrorMessage = "Обязательное поле")]
        [EmailAddress(ErrorMessage = "Недопустимый адрес электронной почты")]
        [Display(Name = "Адрес электронной почты")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Обязательное поле")]
        [PropertyDataType(PropertyDataType.Password)]
        [Display(Name = "Новый пароль")]
        public string Password { get; set; }

        [PropertyDataType(PropertyDataType.Password)]
        [Display(Name = "Подтвердите пароль")]
        [Compare("Password", ErrorMessage = "Пароли не совпадают")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Обязательное поле")]
        [Display(Name = "Имя")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Обязательное поле")]
        [Display(Name = "Фамилия")]
        public string LastName { get; set; }
    }
}