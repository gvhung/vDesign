using System.ComponentModel.DataAnnotations;
using Base.Attributes;

namespace Base.Employee.Entities
{
    public class Post : BaseObject
    {
        [DetailView("Должность")]
        [ListView]
        [MaxLength(255)]
        public string Title { get; set; }
    }
}
