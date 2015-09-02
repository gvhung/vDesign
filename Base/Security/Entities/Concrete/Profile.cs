using Base.Attributes;
using Framework.Attributes;
using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Base.Security
{
    [NotMapped]
    [EnableFullTextSearch]
    public class Profile : WrapUser, IProfile
    {
        public Profile() { }

        public Profile(User user)
            : base(user)
        { }

        [DetailView("Логин", Required = true, Order = 4, ReadOnly = true)]
        public string LoginEmail
        {
            get
            {
                string login = this.GetUserProperty(x => x.Login, null);
                string email = this.GetUserProperty(x => x.Email, null);
                return login == email || String.IsNullOrEmpty(email) ? login : String.Format("{0} ({1})", login, email);
            }

        }

        [MaxLength(90)]
        /*[DetailView("Логин", Required = true, Order = 4, ReadOnly = true)]*/
        [FullTextSearchProperty]
        public string Login
        {
            get { return this.GetUserProperty(x => x.Login, null); }
            set { this.SetUserProperty(x => x.Login, value); }
        }


        [PropertyDataType(PropertyDataType.PhoneNumber)]
        [DetailView("Рабочий телефон")]
        public string OfficePhone
        {
            get { return this.GetUserProperty(x => x.OfficePhone, null); }
            set { this.SetUserProperty(x => x.OfficePhone, value); }
        }

        [PropertyDataType(PropertyDataType.PhoneNumber)]
        [DetailView("Личный телефон")]
        public string PersonPhone
        {
            get { return this.GetUserProperty(x => x.PersonPhone, null); }
            set { this.SetUserProperty(x => x.PersonPhone, value); }
        }

        [DetailView("Почтовый адрес")]
        public string MailAddress
        {
            get { return this.GetUserProperty(x => x.MailAddress, null); }
            set { this.SetUserProperty(x => x.MailAddress, value); }
        }


        [JsonIgnore]
        [MaxLength(110)]
        [DetailView(Name = "Пароль", Order = 100)]
        [PropertyDataType("ChangePassword")]
        public string Password { get; set; }



        [DetailView("Орган гос власти", Order = 110)]
        public Department Department
        {
            get { return this.GetUserProperty(x => x.Department, null); }
            set { this.SetUserProperty(x => x.Department, value); }
        }

        [DetailView("Должность", Order = 120)]
        public Post Post
        {
            get { return this.GetUserProperty(x => x.Post, null); }
            set { this.SetUserProperty(x => x.Post, value); }
        }

        [DetailView("Тип аккаунта", Order = 200)]
        [PropertyDataType("ChangeUserType")]
        public UserType UserType
        {
            get { return this.GetUserProperty(x => x.UserType, UserType.Expert); }
            set { this.SetUserProperty(x => x.UserType, value); }
        }

        public bool ChangeTypePending 
        {
            get { return this.GetUserProperty(x => x.ChangeTypePending, false); }
            set { this.SetUserProperty(x => x.ChangeTypePending, value); }
        }
    }
}
