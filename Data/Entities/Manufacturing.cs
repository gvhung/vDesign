using Base;
using Base.Attributes;
using Framework.Attributes;
using System.ComponentModel.DataAnnotations;

namespace Data.Entities
{
    [EnableFullTextSearch]
    public class Manufacturing : BaseObject
    {
        [ListView]
        [MaxLength(255)]
        [FullTextSearchProperty]
        [DetailView(Name = "Наименование", Required = true)]
        public string Title { get; set; }
        public int OrganizationID { get; set; }
        

        public virtual Organization Organization { get; set; }
    }
}
