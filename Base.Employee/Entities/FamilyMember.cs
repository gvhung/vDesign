using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Base.Attributes;

namespace Base.Employee.Entities
{
    public class FamilyMember : BaseObject
    {
        public int? EmployeeID { get; set; }
        public virtual Employee Employee { get; set; }

        [DetailView("�������")]
        [MaxLength(255)]
        [ListView]
        public string FirstName { get; set; }
        [DetailView("���")]
        [MaxLength(255)]
        [ListView]
        public string SecondName { get; set; }
        [DetailView("��������")]
        [MaxLength(255)]
        [ListView]
        public string LastName { get; set; }
        [DetailView("��������")]
        [ListView]
        public Sex Sex { get; set; }

        [DetailView("���������� ������")]
        public virtual ICollection<FamilyMemberPhone> Phones { get; set; }

        [DetailView("����� ������", TabName = "����� ������", HideLabel = true)]
        public virtual ICollection<FamilyMemberJob> Jobs { get; set; }

        public int? FamilyMembersTypeID { get; set; }

        [DetailView("��� �������")]
        public virtual FamilyMemberType FamilyMembersType { get; set; }
    }
}