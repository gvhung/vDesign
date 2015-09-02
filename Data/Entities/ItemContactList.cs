using Base;
using Base.Attributes;
using Base.Security;
using Framework.Attributes;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Data.Entities
{
    public class ItemContactList: BaseObject
    {
        public int UserID { get; set; }
        [JsonIgnore]
        public virtual User User { get; set; }

        [MaxLength(255)]
        [DetailView(Name = "Наименование", Required = true), ListView]
        [FullTextSearchProperty]
        public string Title { get; set; }

        [MaxLength(255)]
        [DetailView(Name = "Email", Required = true), ListView]
        [FullTextSearchProperty]
        [PropertyDataType(PropertyDataType.EmailAddress)]
        public string Email { get; set; }
    }
}
