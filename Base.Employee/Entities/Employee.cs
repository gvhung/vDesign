using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Base.Attributes;
using Base.Entities.Complex.KLADR;
using Base.Security;

namespace Base.Employee.Entities
{
    public class Employee
    {
        public int? PostID { get; set; }

        [DetailView("Должность", TabName = "Анкета")]
        public virtual Post Post { get; set; }

        [DetailView("Пол", TabName = "Анкета")]
        public Sex? Sex { get; set; }

        [DetailView("День рождения", TabName = "Анкета")]
        public DateTime? Bithday { get; set; }

        [DataType("Address")]
        [DetailView("Домашний адрес", TabName = "Анкета")]
        public Address Address { get; set; }

        [DetailView("Место рождения", TabName = "Анкета")]
        public string Bithplace { get; set; }

        [DetailView("Телефонные номера", TabName = "Анкета")]
        public virtual ICollection<EmployeePhone> Phones { get; set; }

        [DetailView("Места работы", TabName = "Места работы", HideLabel = true)]
        public virtual ICollection<EmployeeJob> Jobs { get; set; }

        [DetailView("Квалификация", TabName = "Квалификация", HideLabel = true)]
        public virtual ICollection<Qualification> Qualifications { get; set; }

        [DetailView("Семья", TabName = "Семья", HideLabel = true)]
        public virtual ICollection<FamilyMember> FamilyMembers { get; set; }

        [DetailView(TabName = "Файлы")]
        [DataType("Files")]
        public virtual ICollection<EmployeeFile> Files { get; set; }
    }
}
