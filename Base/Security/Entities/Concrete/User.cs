using Base.Attributes;
using Framework.Attributes;
using Framework.Maybe;
using Microsoft.Linq.Translations;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Base.Security
{
    [EnableFullTextSearch]
    public class User : BaseUser, IUserCategorizedItem, IProfile, IAccessUser, IEmployee
    {
        private static readonly CompiledExpression<User, string> _fullname =
            DefaultTranslationOf<User>.Property(x => x.FullName).Is(x => (x.LastName ?? "").Trim() == "" ? x.Login : (x.LastName ?? "").Trim() + " " + (x.FirstName ?? "").Trim() + " " + (x.MiddleName ?? "").Trim());

        public User()
        {
            IsActive = true;
            Roles = new List<Role>();
        }

        public int? ImageID { get; set; }
        [DetailView("Фотография", Width = 200, Height = 200)]
        [PropertyDataType("Image")]
        [SystemProperty]
        [ListView]
        public virtual FileData Image { get; set; }

        [JsonIgnore]
        [MaxLength(100)]
        public string Login { get; set; }

        [MaxLength(100)]
        [FullTextSearchProperty]
        public string LastName { get; set; }

        [MaxLength(100)]
        [FullTextSearchProperty]
        public string FirstName { get; set; }

        [MaxLength(100)]
        [FullTextSearchProperty]
        public string MiddleName { get; set; }

        [ListView]
        [DetailView("ФИО")]
        [FullTextSearchProperty]
        public string FullName 
        {
            get
            {
                return _fullname.Evaluate(this);
            }
        }
        
        [ListView]
        [DetailView("Тип пользователя")]
        public UserType UserType { get; set; }

        public int? DepartmentID { get; set; }
        [ListView]
        [DetailView("Орган гос власти")]
        public virtual Department Department { get; set; }

        [NotMapped]
        [DetailView("Подразделение")]
        [ListView(Sortable = false, Filterable = false)]
        public string UserCategoryName { get { return this.UserCategory.With(x => x.Name); } }

        public int? PostID { get; set; }
        [ListView]
        [DetailView("Должность")]
        public virtual Post Post { get; set; }

        [MaxLength(100)]
        [ListView]
        [FullTextSearchProperty]
        [DetailView("Адрес электронной почты", Required = true)]
        public string Email { get; set; }

        [PropertyDataType(PropertyDataType.PhoneNumber)]
        [MaxLength(100)]
        [ListView]
        [DetailView("Рабочий телефон", TabName = "[1]Контакты")]
        public string OfficePhone { get; set; }

//        [PropertyDataType(PropertyDataType.PhoneNumber)]
//        [MaxLength(100)]
//        [ListView]
//        [DetailView("Доп рабочий телефон", TabName = "[1]Контакты")]
//        public string OfficePhone2 { get; set; }
//
//        [PropertyDataType(PropertyDataType.PhoneNumber)]
//        [MaxLength(100)]
//        [ListView]
//        [DetailView("Внутренний телефон", TabName = "[1]Контакты")]
//        public string InternalPhone { get; set; }

        [PropertyDataType(PropertyDataType.PhoneNumber)]
        [MaxLength(100)]
        [ListView]
        [DetailView("Личный телефон", TabName = "[1]Контакты")]
        public string PersonPhone { get; set; }

        [MaxLength(255)]
        [ListView]
        [DetailView("Почтовый адрес", TabName = "[1]Контакты")]
        public string MailAddress { get; set; }

        [JsonIgnore]
        public string Password { get; set; }

        [JsonIgnore]
        public DateTime? ChangePassword { get; set; }

        [JsonIgnore]
        public bool IsActive { get; set; }

        [JsonIgnore]
        public bool ChangePasswordOnFirstLogon { get; set; }

        [JsonIgnore]
        public virtual ICollection<Role> Roles { get; set; }
       
        [JsonIgnore]
        public virtual ICollection<UserConfirmRequest> ConfirmRequests { get; set; }

        public bool ChangeTypePending { get; set; }

        [JsonIgnore]
        public long? LastActivityTicks { get; set; }

        [JsonIgnore]
        public bool IsUnregistered { get; set; }

        #region ICategorizedItem
        public int CategoryID { get; set; }
        [JsonIgnore]
        [ForeignKey("CategoryID")]
        public virtual UserCategory UserCategory { get; set; }

        //[DetailView("Контакты")]
        [ForeignKey("User_ID")]
        [JsonIgnore]
        public virtual ICollection<UserFriend> Friends { get; set; }

        HCategory ICategorizedItem.Category
        {
            get { return this.UserCategory; }
        }
        #endregion
    }

    public class UserFriend : EasyCollectionEntry<User>
    {
        public int? User_ID { get; set; }
        public virtual User User { get; set; }
    }
}
