using Base;
using Base.Attributes;
using Framework.Attributes;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Data.Entities.Product
{
    [EnableFullTextSearch]
    public class ApplicationArea : BaseObject
    {

        [ListView]
        [MaxLength(255)]
        [FullTextSearchProperty]
        [DetailView(Name = "Наименование", Required = true)]
        public string Title { get; set; }

        // NOTE : many to many        
        public ICollection<ApplicationSummary> ApplicationSummary { get; set; }
    }
}
