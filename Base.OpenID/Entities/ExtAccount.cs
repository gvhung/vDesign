
using Base.Attributes;
using Base.Security;
using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Base.OpenID.Entities
{
    [Table("UsersExternal")]
    public class ExtAccount : BaseObject
    {
        [ForeignKey("User")]
        public int UserID { get; set; }
        public User User { get; set; }

        [ListView]
        [DetailView("Сервис", ReadOnly = true)]
        public ServiceType Type { get; set; }

        public string TypeName
        {
            get { return Type.ToString(); }
        }

        public string IconCssClass { get; set; }

        //        public string AccessToken { get; set; }
        //        public DateTime LastAccess { get; set; }
        //        public int ExpiresIn { get; set; }

        [JsonIgnore]
        public string ExternalId { get; set; }

        [ListView]
        [DetailView("Логин", ReadOnly = true)]
        public string Login { get; set; }

        [ListView]
        [DetailView("Адрес электронной почты", ReadOnly = true)]
        public string Email { get; set; }

        [ListView]
        [DetailView("Имя", ReadOnly = true)]
        public string FirstName { get; set; }

        [ListView]
        [DetailView("Фамилия", ReadOnly = true)]
        public string LastName { get; set; }

        [DetailView("Ссылка на профиль", ReadOnly = true)]
        [PropertyDataType(PropertyDataType.Url)]
        public string ProfileLink { get; set; }

        [JsonIgnore]
        public string ProfilePicture { get; set; }

        public string FullName
        {
            get
            {
                return FirstName != null && LastName != null
                    ? String.Format("{0} {1}", FirstName, LastName)
                    : FirstName ?? LastName;
            }
        }

        public User ToUser()
        {
            User user = new User
            {
                Login = Login,
                FirstName = FirstName,
                LastName = LastName,
                Email = Email
            };
            
            return user;
        }
    }
}
