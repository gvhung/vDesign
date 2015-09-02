using Base;
using Base.Attributes;
using Framework.Attributes;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Data.Entities.Workgroup
{
    [EnableFullTextSearch]
    public class WorkGroup : BaseObject
    {        
        [ListView]
        [MaxLength(255)]
        [FullTextSearchProperty]
        [DetailView(Name = "Наименование", Required = true)]
        public string Title { get; set; }

        [ListView]
        [DetailView(Name = "Активна")]

        public bool Active { get; set; }

        [DetailView(Name = "Эксперты")]
        public virtual ICollection<WorkGroupExpert> WorkGroupExperts { get; set; }
    }
}
