using Base;
using Base.Attributes;
using Base.BusinessProcesses.Entities;
using Base.Security;
using Base.Security.ObjectAccess;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Entities
{
    [Table("SupportQA")]
    public class SupportQA : BaseObject, IBPObject, IEnabledState
    {
        [ForeignKey("Sender")]
        public int? SenderID { get; set; }

        public virtual User Sender { get; set; }

        [MaxLength(255)]
        [DetailView(Name = "Имя пользователя", Order = -2)]
        public string SenderName { get; set; }

        [MaxLength(255)]
        [EmailAddress(ErrorMessage = "Неверный формат")]
        [PropertyDataType(PropertyDataType.EmailAddress)]
        [DetailView(Name = "Email пользователя", Order = -1)]
        public string SenderEmail { get; set; }

        [ListView]
        [DetailView(Name = "Тип", Required = true, Order = 1)]
        public SupportQAType Type { get; set; }

        [ListView]
        [MaxLength(255)]
        [DetailView(Name = "Тема", Required = true, Order = 2)]
        public string Title { get; set; }

        [DetailView(Name = "Сообщение", Required = true, Order = 3)]
        public string Question { get; set; }

        [ListView]
        [DetailView(Name = "Дата создания", Order = 4, ReadOnly = true)]
        [PropertyDataType(PropertyDataType.DateTime)]
        public DateTime CreateDate { get; set; }

        [ListView(Order = -1)]
        [DetailView(Name = "Статус", Order = 5, ReadOnly = true)]
        public SupportQAStatus Status { get; set; }

        [DetailView(Name = "Ответ", Order = 6, ReadOnly = true)]
        public string Answer { get; set; }

        [ForeignKey("Respondent")]
        public int? RespondentID { get; set; }

        [DetailView(Name = "Ответил(а)", Order = 7, ReadOnly = true)]
        public virtual User Respondent { get; set; }

        [DetailView(Name = "Дата ответа", Order = 8, ReadOnly = true)]
        [PropertyDataType(PropertyDataType.DateTime)]
        public DateTime? ProcessingDate { get; set; }

        public string AnswerMessage
        {
            get
            {
                return
                    String.Format(
                        "На ваше сообщение №{0} от {1} получен ответ:\n\n{2}\n\n---\n\nИсходное сообщение:\n\n{3}",
                        ID, CreateDate.ToString("d MMMM yyyy г."), Answer, Question);
            }
        }


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

        public bool IsEnabled(ISecurityUser user)
        {
            return Status == SupportQAStatus.Created;
        }


    }

    public enum SupportQAType
    {
        [Description("Вопросы по работе с системой")]
        System,
        [Description("Вопросы по методологии")]
        Methodic,
        [Description("Предложения")]
        Proposal,
        [Description("Сообщения об ошибках")]
        Error
    }

    public enum SupportQAStatus
    {
        [Description("Новое")]
        Created,
        [Description("Отвечено")]
        Answered,
        [Description("Проигнорировано")]
        Ignored
    }
}
