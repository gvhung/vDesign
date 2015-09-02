using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Base.Attributes;

namespace Base.Employee.Entities
{
    public class FamilyMember : BaseObject
    {
        public int? EmployeeID { get; set; }
        public virtual Employee Employee { get; set; }

        [DetailView("Фамилия")]
        [MaxLength(255)]
        [ListView]
        public string FirstName { get; set; }
        [DetailView("Имя")]
        [MaxLength(255)]
        [ListView]
        public string SecondName { get; set; }
        [DetailView("Отчество")]
        [MaxLength(255)]
        [ListView]
        public string LastName { get; set; }
        [DetailView("Отчество")]
        [ListView]
        public Sex Sex { get; set; }

        [DetailView("Телефонные номера")]
        public virtual ICollection<FamilyMemberPhone> Phones { get; set; }

        [DetailView("Места работы", TabName = "Места работы", HideLabel = true)]
        public virtual ICollection<FamilyMemberJob> Jobs { get; set; }

        public int? FamilyMembersTypeID { get; set; }

        [DetailView("Тип родства")]
        public virtual FamilyMemberType FamilyMembersType { get; set; }
    }
}