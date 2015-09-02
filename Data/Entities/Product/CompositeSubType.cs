using Base;
using Base.Attributes;
using Framework.Attributes;
using System.ComponentModel.DataAnnotations;

namespace Data.Entities.Product
{
    [EnableFullTextSearch]
    public class CompositeSubType : BaseObject
    {
        [ListView]
        [MaxLength(255)]
        [FullTextSearchProperty]
        [DetailView(Name = "Наименование", Required = true)]
        public string Title { get; set; }

        public int CompositeTypeID { get; set; }
        [DetailView(Name = "Тип композита")]
        public virtual CompositeType CompositeType { get; set; }
    }
}
