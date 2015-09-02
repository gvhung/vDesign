using System.ComponentModel.DataAnnotations.Schema;
using Base.Attributes;

namespace Base.Employee.Entities
{
    [Table("EmployeeFile")]
    public class EmployeeFile : FileData
    {
        public int? EmployeeID { get; set; }
        public virtual Employee Employee { get; set; }

        [DetailView(Name = "Описание", Order = 4)]
        [ListView]
        public string Description { get; set; }
    }
}