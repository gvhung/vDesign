using Base.Attributes;
using System.ComponentModel.DataAnnotations;

namespace Base.Security
{
    public class Post : BaseObject
    {
        [DetailView("Наименование")]
        [ListView]
        [MaxLength(255)]
        public string Title { get; set; }
    }
}
