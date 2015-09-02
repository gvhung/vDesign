using Base;
using Base.Attributes;
using Framework.Attributes;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Data.Entities.Product
{
    [EnableFullTextSearch]
    public class ApplicationSummary : BaseObject
    {
        [ListView]
        [MaxLength(255)]
        [FullTextSearchProperty]
        [DetailView(Name = "Наименование", Required = true)]
        public string Title { get; set; }

        [DetailView(Name = "Области применения")]
        public virtual ICollection<ApplicationArea> ApplicationAreas { get; set; }
        
        //NOTE: ManyToMany
        public ICollection<Product> Product { get; set; }
    }
}
