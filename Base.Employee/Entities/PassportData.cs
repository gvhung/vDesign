using System;
using System.ComponentModel.DataAnnotations.Schema;
using Base.Attributes;

namespace Base.Employee.Entities
{
    [ComplexType]
    public class PassportData
    {
        [DetailView("Серия")]
        public string Series { get; set; }
        [DetailView("Номер")]
        public string Number { get; set; }
        [DetailView("Дата выдачи")]
        public DateTime? IssueDate{ get; set; }
    }
}