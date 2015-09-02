using Base.Attributes;
using Framework.Attributes;
using Framework.Maybe;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Base.Security
{
    [NotMapped]
    [EnableFullTextSearch]
    public class AccessUser : Employee, IAccessUser, IEmployee, IUserCategorizedItem
    {
        public AccessUser()
        {
            Roles = new List<Role>();
        }

        public AccessUser(User user)
            : base(user)
        {
            this.SortOrder = user.SortOrder;
        }

        [MaxLength(100)]
        [DetailView("Логин", Required = true, Order = 2)]
        [FullTextSearchProperty]
        public string Login
        {
            get { return this.GetUserProperty(x => x.Login, null); }
            set { this.SetUserProperty(x => x.Login, value); }
        }

        [JsonIgnore]
        [MaxLength(100)]
        [DetailView(Name = "Пароль", Order = 7)]
        [PropertyDataType("ChangePassword")]
        public string Password
        {
            get { return this.User.With(x => x.Password); }
            set { this.SetUserProperty(x => x.Password, value); }
        }
        

        [JsonIgnore]
        [PropertyDataType("ManyToMany")]
        [DetailView(Name = "Роли", TabName = "[3]Права доступа", DeferredLoading = true)]
        public virtual ICollection<Role> Roles
        {
            get { return this.GetUserProperty(x => x.Roles, null); }
            set { this.SetUserProperty(x => x.Roles, value); }
        }

        [DetailView(Name = "Активный", TabName = "[3]Права доступа")]
        public bool IsActive
        {
            get { return this.GetUserProperty(x => x.IsActive, false); }
            set { this.SetUserProperty(x => x.IsActive, value); }
        }

        [DetailView(Name = "Изменить пароль при первом входе", TabName = "[3]Права доступа")]
        public bool ChangePasswordOnFirstLogon
        {
            get { return this.GetUserProperty(x => x.ChangePasswordOnFirstLogon, false); }
            set { this.SetUserProperty(x => x.ChangePasswordOnFirstLogon, value); }
        }
    }
}