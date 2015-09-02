using Base;
using Base.Attributes;
using Base.BusinessProcesses.Entities;
using Base.Security;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Entities
{
    public class UserPromotion : BaseObject, IBPObject
    {
        [ListView]
        [DetailView("Пользователь", ReadOnly = true)]
        [ForeignKey("UserID")]
        public virtual User User { get; set; }
        public int UserID { get; set; }


        [ListView]
        [DetailView("Запрашиваемый тип", Required = true)]
        public UserType RequestedType { get; set; }

        [ListView]
        [DetailView("Орган гос власти", Required = true)]
        [ForeignKey("DepartmentID")]
        public virtual Department Department { get; set; }

        public int? DepartmentID { get; set; }

        [MaxLength(500)]
        [DetailView("Комментарий")]
        [PropertyDataType(PropertyDataType.MultilineText)]
        public string Comment { get; set; }

        [ListView]
        [DetailView("Состояние", ReadOnly = true)]
        public PromotionState State { get; set; }

        [ListView]
        [DetailView("Подача заявки", ReadOnly = true)]
        [PropertyDataType(PropertyDataType.DateTime)]
        public DateTime SubmitTime { get; set; }

        [ListView]
        [DetailView("Принятие решения", ReadOnly = true)]
        [PropertyDataType(PropertyDataType.DateTime)]
        public DateTime? DecideTime { get; set; }


        #region IBPObject
       [PropertyDataType("InitWorkflow")]
        [NotMapped]
        [DetailView("Шаблон бизнес-процесса", HideLabel = true, Visible = false)]
        public Workflow InitWorkflow { get; set; }

        public int? WorkflowContextID
        {
            get;
            set;
        }

        public virtual WorkflowContext WorkflowContext
        {
            get;
            set;
        }
        #endregion


      
    }

    public enum PromotionState
    {
        [Description("Новая")]
        New,
        [Description("Одобрена")]
        Approve,
        [Description("Отклонена")]
        Decline
    }
}
