using Base.Attributes;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Base.BusinessProcesses.Entities
{
    public class Weekend: BaseObject, IScheduler
    {
        public Weekend()
        {
            Title = "Выходной";
        }

        [DetailView(Name = "Наименование", Required = true), ListView]
        public string Title { get; set; }
         
        [DetailView(Name = "Дата", Required = true), ListView]
        [PropertyDataType(PropertyDataType.Date)]
        public DateTime Start { get; set; }

        [SystemProperty]
        public DateTime End
        {
            get { return Start; }
        }

        public string Description { get; set; }

        [SystemProperty]
        public bool IsAllDay {
            get { return true; }
        }

        [SystemProperty]
        public string StartTimezone { get; set; }
        [SystemProperty]
        public string EndTimezone { get; set; }
        [SystemProperty]
        public string Recurrence { get; set; }
        [SystemProperty]
        public string RecurrenceRule { get; set; }
        [SystemProperty]
        public int? RecurrenceID { get; set; }
        [SystemProperty]
        public string RecurrenceException { get; set; }

        [NotMapped]
        [SystemProperty]
        public string Color
        {
            get
            {
                return "#FA5858";
            }
        }
    }
}
