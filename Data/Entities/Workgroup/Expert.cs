using Base;
using Base.Attributes;
using Base.Security;
using Framework.Attributes;
using Microsoft.Linq.Translations;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Entities.Workgroup
{
    [EnableFullTextSearch]
    [Table("Experts")]
    public class Expert : BaseObject
    {
        private static readonly CompiledExpression<Expert, string> _fullname =
          DefaultTranslationOf<Expert>.Property(x => x.FullName).Is(x => (x.LastName ?? "").Trim() == "" ? x.Login : (x.LastName ?? "").Trim() + " " + (x.FirstName ?? "").Trim() + " " + (x.MiddleName ?? "").Trim());


        private User _user;
        private string _login;
        private string _lastName;
        private string _firstName;
        private string _middleName;
        private string _phone;
        private string _email;

        public Expert()
        {

        }

        public Expert(User user)
        {
            _user = user;
        }

        [JsonIgnore]
        [MaxLength(100)]
        public string Login
        {
            get
            {
                return _user == null ? _login : _user.Login;
            }
            set
            {
                if (_user != null)
                    _user.Login = value;
                else
                    _login = value;
            }
        }

        [DetailView(Name = "Фамилия")]
        [MaxLength(100)]
        [FullTextSearchProperty]
        public string LastName
        {
            get
            {
                return _user == null ? _lastName : _user.LastName;
            }
            set
            {
                if (_user != null)
                    _user.LastName = value;
                else
                    _lastName = value;
            }
        }

        [DetailView(Name = "Имя")]
        [MaxLength(100)]
        [FullTextSearchProperty]
        public string FirstName
        {
            get { return _user == null ? _firstName : _user.FirstName; }
            set
            {
                if (_user != null)
                    _user.FirstName = value;
                else
                    _firstName = value;
            }
        }

        [MaxLength(100)]
        [FullTextSearchProperty]
        [DetailView(Name = "Отчество")]
        public string MiddleName
        {
            get { return _user == null ? _middleName : _user.MiddleName; }
            set
            {
                if (_user != null)
                    _user.MiddleName = value;
                else
                {
                    _middleName = value;
                }
            }
        }


        [ListView(Name = "ФИО")]
        [FullTextSearchProperty]
        public string FullName
        {
            get { return _fullname.Evaluate(this); }
        }

        [MaxLength(100)]
        [FullTextSearchProperty]
        [ListView]
        [DetailView(Name = "Телефон")]
        public string PersonPhone
        {
            get { return _user == null ? _phone : _user.PersonPhone; }
            set
            {
                if (_user != null)
                    _user.PersonPhone = value;
                else
                {
                    _phone = value;
                }
            }
        }

        [MaxLength(100)]
        [FullTextSearchProperty]
        [ListView]
        [DetailView(Name = "Email")]
        public string Email
        {
            get { return _user == null ? _email : _user.Email; }
            set
            {
                if (_user != null)
                    _user.Email = value;
                else
                {
                    _email = value;
                }
            }
        }

        [DetailView(Name = "Пользователь")]
        public User User
        {
            get
            {
                return _user;
            }
            set
            {
                _user = value;
            }
        }

        public int ExpertStatusID { get; set; }
        [ListView]
        [DetailView(Name = "Статус эксперта")]
        public virtual ExpertStatus ExpertStatus { get; set; }

        //[JsonIgnore]
        ////public ICollection<WorkGroupExpert> WorkGroupExpert { get; set; }
    }
}
