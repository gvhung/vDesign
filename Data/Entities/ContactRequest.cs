using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Base;
using Base.Attributes;
using Base.BusinessProcesses.Entities;

namespace Data.Entities
{
    public class ContactRequest : BaseObject, IBPObject
    {
        [DetailView("Ф.И.О", Required = true, ReadOnly = true)]
        [ListView]
        [MaxLength(int.MaxValue)]
        public string Name { get; set; }
        [DetailView("E-mail", Required = true, ReadOnly = true)]
        [ListView]
        [MaxLength(int.MaxValue)]
        public string Email { get; set; }
        [DetailView("Телефон", Required = true, ReadOnly = true)]
        [ListView]
        [MaxLength(int.MaxValue)]
        public string Phone { get; set; }
        [DetailView("Сообщение", Required = true, ReadOnly = true)]
        public string Message { get; set; }

        [DetailView("Дата создания", ReadOnly = true)]
        [ListView]
        public DateTime Date { get; set; }

        [DetailView("Выполнено")]
        [ListView]
        public bool Complete { get; set; }

        public int? WorkflowContextID { get; set; }
        public virtual WorkflowContext WorkflowContext { get; set; }

        [PropertyDataType("InitWorkflow")]
        [NotMapped]
        public Workflow InitWorkflow { get; set; }
    }
}