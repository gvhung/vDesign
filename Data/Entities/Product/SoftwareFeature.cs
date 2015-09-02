using Base;
using Base.Attributes;
using Framework.Attributes;
using System.ComponentModel.DataAnnotations;

namespace Data.Entities.Product
{
    [EnableFullTextSearch]
    public class SoftwareFeature : BaseObject
    {
        [ListView]
        [MaxLength(255)]
        [FullTextSearchProperty]
        [DetailView(Name = "Наименование", Required = true)]
        public string Title { get; set; }

        public int SoftwareTypeID { get; set; }
        [DetailView(Name = "Тип программного обеспечения", Required = true)]
        public virtual SoftwareType SoftwareType { get; set; }
    }
}
