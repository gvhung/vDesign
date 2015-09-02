using Base;
using Base.Attributes;
using Framework.Attributes;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Data.Entities
{
    [EnableFullTextSearch]
    public class Organization : BaseObject
    {
        [ListView]
        [MaxLength(255)]
        [FullTextSearchProperty]
        [DetailView(Name = "Наименование", Required = true)]
        public string Title { get; set; }
        [ListView]
        [DetailView(Name = "Производственные площадки")]
        public virtual ICollection<Manufacturing> Manufacturings { get; set; }

    }
}
