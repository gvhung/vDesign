using Base.Attributes;
using Framework;
using Framework.Attributes;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;

namespace Base.Security
{
    public abstract class WrapUser : BaseUser
    {
        protected User User;

        protected WrapUser() {
            this.User = new User();
        }

        protected WrapUser(User user)
        {
            this.ID = user.ID;
            this.User = user;
        }

        [ListView]
        [MaxLength(100)]
        [PropertyDataType(PropertyDataType.EmailAddress)]
        [DetailView("Адрес электронной почты", ReadOnly = true, Order = 5, Required = true)]
        public string Email
        {
            get { return this.GetUserProperty(x => x.Email, null); }
            set { this.SetUserProperty(x => x.Email, value); }
        }

        public int? ImageID
        {
            get { return this.GetUserProperty(x => x.ImageID, null); }
            set { this.SetUserProperty(x => x.ImageID, value); }
        }

        [DetailView("Фотография", Width = 200, Height = 200, Order = 0)]
        [PropertyDataType("Image", Params = "hideSelect")]
        [SystemProperty]
        [ListView]
        public virtual FileData Image
        {
            get { return this.GetUserProperty(x => x.Image, null); }
            set { this.SetUserProperty(x => x.Image, value); }
        }

        [ListView(Name = "ФИО", Order = 1)]
        [FullTextSearchProperty]
        public string FullName { get { return this.GetUserProperty(x => x.FullName, null); } }

        [MaxLength(100)]
        [DetailView("Фамилия", Order = 10)]
        public string LastName
        {
            get { return this.GetUserProperty(x => x.LastName, null); }
            set { this.SetUserProperty(x => x.LastName, value); }
        }

        [MaxLength(100)]
        [DetailView("Имя", Order = 20)]
        public string FirstName
        {
            get { return this.GetUserProperty(x => x.FirstName, null); }
            set { this.SetUserProperty(x => x.FirstName, value); }
        }

        [MaxLength(100)]
        [DetailView("Отчество", Order = 30)]
        public string MiddleName
        {
            get { return this.GetUserProperty(x => x.MiddleName, null); }
            set { this.SetUserProperty(x => x.MiddleName, value); }
        }
        
        
        protected TProperty GetUserProperty<TProperty>(Expression<Func<User, TProperty>> memberLamda, TProperty def)
        {
            if (this.User != null)
            {
                return (TProperty)this.User.GetPropertyValue(memberLamda);
            }

            return def;
        }

        protected void SetUserProperty<TProperty>(Expression<Func<User, TProperty>> memberLamda, object value)
        {
            if (this.User != null)
            {
                this.User.SetPropertyValue(memberLamda, value);
            }
        }

        public override void BeforeModelBinding()
        {
            this.User = new User();
        }
    }
}
