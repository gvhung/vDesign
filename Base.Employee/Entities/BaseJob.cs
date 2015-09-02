using System;
using Base.Attributes;

namespace Base.Employee.Entities
{
    public abstract class BaseJob : BaseObject
    {
        [ListView]
        [DetailView("Дата начала")]
        public DateTime? StartDate { get; set; }
        
        [ListView]
        [DetailView("Дата окончания")]
        public DateTime? EndDate { get; set; }

        [ListView]
        [DetailView("Организация")]
        public string Organization { get; set; }

        [DetailView("Должностная инструкция")]
        public string JobDescription { get; set; }
    }

    public class EmployeeJob : BaseJob
    {
        public int? EmployeeID { get; set; }
        public virtual Employee Employee { get; set; }
    }

    public class FamilyMemberJob : BaseJob
    {
        public int? FamilyMemberID { get; set; }
        public virtual FamilyMember FamilyMember { get; set; }
    }
}