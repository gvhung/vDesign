using Base;
using Base.Attributes;
using Framework.Attributes;
using System.ComponentModel.DataAnnotations;

namespace Data.Entities.Product
{
    [EnableFullTextSearch]
    public class AdditiveClass : BaseObject
    {
        [ListView]
        [MaxLength(255)]
        [FullTextSearchProperty]
        [DetailView(Name = "Наименование", Required = true)]
        public string Title { get; set; }
        [ListView]
        [DetailView(Name = "Полное наименование")]
        public string FullTitle { get; set; }

    }
}