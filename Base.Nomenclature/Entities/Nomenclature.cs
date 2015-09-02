using Base.Attributes;
using Base.Registers.Entities;
using Framework.Attributes;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Base.Nomenclature.Entities
{
    [EnableFullTextSearch]
    [Table("Nomenclature")]
    public class Nomenclature : BaseObject, ICategorizedItem
    {
        #region ICategorizedItem
        public int CategoryID { get; set; }
        [JsonIgnore]
        [ForeignKey("CategoryID")]
        public virtual NomenclatureCategory Category_ { get; set; }

        HCategory ICategorizedItem.Category
        {
            get { return this.Category_; }
        }
        #endregion

        public int? ImageID { get; set; }
        [DetailView("Изображение")]
        [PropertyDataType("Image")]
        [ListView]
        public virtual FileData Image { get; set; }

        [ListView]
        [FullTextSearchProperty]
        [DetailView(Name = "Код", Required = true)]
        [MaxLength(20)]
        public string Code { get; set; }

        public int? MeasureID { get; set; }
        [ListView("Ед.изм.")]
        [DetailView("Единица измерения")]
        public virtual Measure Measure { get; set; }

        [FullTextSearchProperty]
        [ListView(Hidden = true)]
        [DetailView(Name = "Описание")]
        public string Description { get; set; }


        [FullTextSearchProperty]
        [DetailView(Name = "ОКПД")]
        public virtual OkpdHierarchy Okpd { get; set; }

        [ForeignKey("Okpd")]
        public int? OkpdID { get; set; }
    }
}
