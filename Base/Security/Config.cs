using Base.Attributes;
using System;

namespace Base.Security
{
    [Serializable]
    public class Config : BaseObject
    {
        [DetailView("Минимальная длина логина")]
        public int MinLenLogin { get; set; }

        [DetailView("Минимальная длина пароля")]
        public int MinLenPassword { get; set; }

        [DetailView("Пароль не должен содержать более трех последовательных на клавиатуре символов")]
        public bool PasswordCheckKeyboard { get; set; }

        [DetailView("Разрешить регистрацию новых пользователей")]
        public bool AllowRegistration { get; set; }
    }
}
