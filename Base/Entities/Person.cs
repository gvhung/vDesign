using Base.Attributes;
using Framework.Attributes;
using System.ComponentModel.DataAnnotations;

namespace Base.Entities
{
    [EnableFullTextSearch]
    public abstract class Person : BaseObject
    {
        public int? ImageID { get; set; }

        [ListView]
        [PropertyDataType("Image")]
        [FullTextSearchProperty]
        [DetailView(Name = "Фотография", Order = 0)]
        public virtual FileData Image { get; set; }

        [ListView]
        [MaxLength(100)]
        [DetailView(Name = "Фамилия", Order = 1, Required = true )]
        public string LastName { get; set; }

        [ListView]
        [MaxLength(100)]
        [DetailView(Name = "Имя", Order = 2)]
        public string FirstName { get; set; }

        [ListView]
        [MaxLength(100)]
        [DetailView(Name = "Отчество", Order = 3)]
        public string MiddleName { get; set; }

        [ListView]
        [MaxLength(255)]
        [DetailView(Name = "Адрес электронной почты", Order = 5)]
        [PropertyDataType(PropertyDataType.EmailAddress)]
        public string Email { get; set; }

        [ListView]
        [MaxLength(30)]
        [PropertyDataType(PropertyDataType.PhoneNumber)]
        [DetailView(Name = "Телефон", Order = 6)]
        public string Phone { get; set; }

        [ListView(Hidden = true)]
        [MaxLength(30)]
        [PropertyDataType(PropertyDataType.PhoneNumber)]
        [DetailView(Name = "Факс", Order = 7)]
        public string Fax { get; set; }
    }
}
