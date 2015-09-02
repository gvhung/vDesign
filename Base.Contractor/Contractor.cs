using Base.Attributes;
using Base.Entities.Complex.KLADR;
using Framework.Attributes;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Base.Contractor
{
    [EnableFullTextSearch]
    public class Contractor : BaseObject, ICategorizedItem
    {
        public Contractor()
        {
            LegalAddress = new Address();
            FactualAddress = new Address();
        }

        [ListView(Hidden = true)]
        [DetailView(Name = "Тип", Order = 0)]
        public TypeContractor Type { get; set; }

        [ListView]
        [FullTextSearchProperty]
        [DetailView(Name = "Сокращенное наименование", Order = 1, Required = true)]
        [MaxLength(255)]
        public string Title { get; set; }

        [ListView(Hidden = true)]
        [FullTextSearchProperty]
        [DetailView(Name = "Полное наименование", Order = 2)]
        public string FullName { get; set; }

        [FullTextSearchProperty]
        [ListView(Hidden = true)]
        [DetailView(Name = "Юридический адрес", Order = 3)]
        public Address LegalAddress { get; set; }

        [FullTextSearchProperty]
        [ListView(Hidden = true)]
        //[DetailView(Name = "Фактический адрес", Order = 4)]
        public Address FactualAddress { get; set; }

        [FullTextSearchProperty]
        [ListView(Hidden = true)]
        [DetailView(Name = "Описание", Order = 8)]
        public string Description { get; set; }

        [FullTextSearchProperty]
        [ListView(Hidden = true)]
        [DetailView(Name = "ИНН", Order = 9)]
        [MaxLength(12)]
        public string INN { get; set; }

        [FullTextSearchProperty]
        [ListView(Hidden = true)]
        [DetailView(Name = "КПП", Order = 10)]
        [MaxLength(10)]
        public string KPP { get; set; }

        [FullTextSearchProperty]
        [ListView(Hidden = true)]
        [DetailView(Name = "ОКПО", Order = 11)]
        [MaxLength(10)]
        public string OKPO { get; set; }

        [FullTextSearchProperty]        
        [ListView(Hidden = true)]
        [DetailView(Name = "ОГРН", Order = 12)]
        [MaxLength(13)]
        public string OGRN { get; set; }

        [DetailView(TabName = "[1]Контактные лица")]
        public virtual ICollection<ContactPerson> Contacts { get; set; }

        #region ICategorizedItem
        public int CategoryID { get; set; }
        [ForeignKey("CategoryID")]
        [JsonIgnore]
        public virtual ContractorCategory Category_ { get; set; }

        HCategory ICategorizedItem.Category
        {
            get { return this.Category_; }
        }
        #endregion
    }

    public enum TypeContractor
    {
        [Description("Юридическое лицо")]
        LegalEntity,
        [Description("Физическое лицо")]
        Individual
    }
}
