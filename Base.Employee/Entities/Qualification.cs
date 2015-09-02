using System;
using System.ComponentModel.DataAnnotations;
using Base.Attributes;

namespace Base.Employee.Entities
{
    public class Qualification : BaseObject
    {
        public int? EmployeeID { get; set; }
        public Employee Employee { get; set; }

        [DetailView("Место учебы")]
        [ListView]
        public string StudyPlace { get; set; }

        [DetailView("Факультет")]
        [ListView]
        [MaxLength(255)]
        public string Faculty { get; set; }

        [DetailView("Специальность")]
        [ListView]
        [MaxLength(255)]
        public string Specialty { get; set; }

        [DetailView("Дата начала")]
        [ListView]
        public DateTime? StartDate { get; set; }

        [DetailView("Дата окончания")]
        [ListView]
        public DateTime? EndDate { get; set; }

        [DetailView("Заметка")]
        [ListView]
        public string Notes { get; set; }
    }
}