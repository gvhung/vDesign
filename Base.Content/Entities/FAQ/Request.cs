using Base;
using Base.Attributes;
using Base.BusinessProcesses.Entities;
using Base.Security;
using Framework.Attributes;
using System;
using System.ComponentModel.DataAnnotations;

namespace Base.Content.Entities
{
    public class Request : BaseObject, IBPObject
    {
        [DetailView("Имя", ReadOnly = true, Order = 0)]
        [MaxLength(255)]
        [ListView]
        public string Name { get; set; }

        [DetailView("Электронная почта", ReadOnly = true, Order = 1)]
        [MaxLength(255)]
        [ListView]
        public string EMail { get; set; }

        [DetailView("Тема", ReadOnly = true, Order = 2)]
        [MaxLength(255)]
        [ListView]
        public string Subject { get; set; }

        [DetailView("Текст сообщения", ReadOnly = true, Order = 3)]
        [ListView]
        public string Text { get; set; }

        [DetailView("Дата", ReadOnly = true, Order = 6)]
        [ListView]
        public DateTime Date { get; set; }

        #region IBPObject
        public int? CurrentStageID { get; set; }

        [DetailView("Этап", Required = true, Order = 4, ReadOnly = true)]
        [ListView]
        public virtual Stage CurrentStage { get; set; }

        public int? PerformerID { get; set; }

        [DetailView("Исполнитель", Order = 5, ReadOnly = true)]
        [ListView]
        public virtual User Performer { get; set; }
        #endregion
    }
}
