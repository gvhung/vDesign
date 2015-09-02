using Base.Attributes;
using Framework.Attributes;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Base.Security
{
    [NotMapped]
    [EnableFullTextSearch]
    public class Employee : WrapUser, IEmployee, IUserCategorizedItem
    {
        public Employee() { }

        public Employee(User user)
            : base(user)
        {
            this.SortOrder = user.SortOrder;
        }

        public int? DepartmentID
        {
            get { return this.GetUserProperty(x => x.DepartmentID, null); }
            set { this.SetUserProperty(x => x.DepartmentID, value); }
        }


        [DetailView("Огран гос власти", Order = 61, ReadOnly = true)]
        [ListView(Hidden = true)]
        public Department Department
        {
            get { return this.GetUserProperty(x => x.Department, null); }
            set { this.SetUserProperty(x => x.Department, value); }
        }

        public int? PostID
        {
            get { return this.GetUserProperty(x => x.PostID, null); }
            set { this.SetUserProperty(x => x.PostID, value); }
        }

        [DetailView("Должность", Order = 62)]
        [ListView]
        public virtual Post Post
        {
            get { return this.GetUserProperty(x => x.Post, null); }
            set { this.SetUserProperty(x => x.Post, value); }
        }

        [DetailView("Тип аккаунта", ReadOnly = true, Order = 65)]
        public UserType UserType
        {
            get { return this.GetUserProperty(x => x.UserType, UserType.Base); }
            set { this.SetUserProperty(x => x.UserType, value); }
        }
        
        [PropertyDataType(PropertyDataType.PhoneNumber)]
        [MaxLength(100)]
        [ListView]
        [DetailView("Рабочий телефон", Order = 100, TabName = "[1]Контакты")]
        public string OfficePhone {
            get { return this.GetUserProperty(x => x.OfficePhone, null); }
            set { this.SetUserProperty(x => x.OfficePhone, value); }
        }

//        [PropertyDataType(PropertyDataType.PhoneNumber)]
//        [MaxLength(100)]
//        [ListView]
//        [DetailView("Доб рабочий телефон", Order = 110, TabName = "[1]Контакты")]
//        public string OfficePhone2
//        {
//            get { return this.GetUserProperty(x => x.OfficePhone2, null); }
//            set { this.SetUserProperty(x => x.OfficePhone2, value); }
//        }
//
//        [PropertyDataType(PropertyDataType.PhoneNumber)]
//        [MaxLength(100)]
//        [ListView]
//        [DetailView("Внутренний телефон", Order = 120, TabName="[1]Контакты")]
//        public string InternalPhone
//        {
//            get { return this.GetUserProperty(x => x.InternalPhone, null); }
//            set { this.SetUserProperty(x => x.InternalPhone, value); }
//        }

        [PropertyDataType(PropertyDataType.PhoneNumber)]
        [MaxLength(100)]
        [ListView]
        [DetailView("Личный телефон", Order = 130, TabName = "[1]Контакты")]
        public string PersonPhone
        {
            get { return this.GetUserProperty(x => x.PersonPhone, null); }
            set { this.SetUserProperty(x => x.PersonPhone, value); }
        }

        [MaxLength(255)]
        [ListView]
        [DetailView("Почтовый адрес", Order = 140, TabName = "[1]Контакты")]
        public string MailAddress
        {
            get { return this.GetUserProperty(x => x.MailAddress, null); }
            set { this.SetUserProperty(x => x.MailAddress, value); }
        }

        #region IUserCategorizedItem
        public int CategoryID
        {
            get { return this.GetUserProperty(x => x.CategoryID, 0); }
            set { this.SetUserProperty(x => x.CategoryID, value); }
        }
        public UserCategory UserCategory { get; set; }
        HCategory ICategorizedItem.Category
        {
            get { return this.GetUserProperty(x => x.UserCategory, null); }
        }
        #endregion
    }
}
