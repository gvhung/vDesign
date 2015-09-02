using Base.Attributes;
using Base.Entities.Complex;
using Base.Security.ObjectAccess;
using Framework.Attributes;
using Framework.Maybe;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Base.Event.Entities
{
    [EnableFullTextSearch]
    public class Event : BaseObject, IScheduler, IAccessibleObject
    {
        [ListView]
        [FullTextSearchProperty]
        [MaxLength(255)]
        [DetailView(Name = "Наименование", Required = true)]
        public string Title { get; set; }

        [ListView]
        [DetailView(Name = "Начало", Required = true)]
        [PropertyDataType(PropertyDataType.DateTime)]
        public DateTime Start { get; set; }

        [ListView]
        [DetailView(Name = "Окончание", Required = true)]
        [PropertyDataType(PropertyDataType.DateTime)]
        public DateTime End { get; set; }

        public int? TypeID { get; set; }
        [ListView]
        [DetailView(Name = "Тип")]
        public virtual EventType Type { get; set; }

        [ListView]
        [FullTextSearchProperty]
        [DetailView("Описание")]
        public string Description { get; set; }

        [DetailView(Name = "Весь день")]
        [PropertyDataType("Scheduler_IsAllDay")]
        public bool IsAllDay { get; set; }

        [SystemProperty]
        public string StartTimezone { get; set; }
        [SystemProperty]
        public string EndTimezone { get; set; }
        [SystemProperty]
        public string Recurrence { get; set; }
        
        [ListView]
        [DetailView(TabName = "[1]Периодичность")]
        [PropertyDataType("Scheduler_RecurrenceRule")]
        public string RecurrenceRule { get; set; }

        [SystemProperty]
        public int? RecurrenceID { get; set; }
        [SystemProperty]
        public string RecurrenceException { get; set; }

        [NotMapped]
        [SystemProperty]
        public string Color { get { return this.Type.With(x => x.Color, new Color() { }).ToString(); } set { } }
    }
}
