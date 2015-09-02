using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Base.Attributes;

namespace Base.Employee.Entities
{
    public abstract class BasePhone : BaseObject
    {

        [DetailView("Тип")]
        [ListView]
        public PhoneType PhoneType { get; set; }
        [DetailView("Номер")]
        [ListView]
        [MaxLength(255)]
        public string Number { get; set; }
        [ListView]
        [DetailView("Заметка")]
        public string Note { get; set; }
    }

    public class EmployeePhone : BasePhone
    {
        public int? EmployeeID { get; set; }
        public virtual Employee Employee { get; set; }
    }

    public class FamilyMemberPhone : BasePhone
    {
        public int? FamilyMemberID { get; set; }
        public virtual FamilyMember FamilyMember { get; set; }
    }

    public enum PhoneType
    {
        [Description("Стационарный")]
        LandLine = 0,
        [Description("Сотовый")]
        Cellular = 1
    }
}