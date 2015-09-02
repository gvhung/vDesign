using Base;
using System;

namespace WebUI.Models
{
    public class SchedulerViewModel : BaseObject, Kendo.Mvc.UI.ISchedulerEvent, Base.IScheduler
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public string StartTimezone { get; set; }
        public string EndTimezone { get; set; }
        public string Recurrence { get; set; }
        public string RecurrenceRule { get; set; }
        public int? RecurrenceID { get; set; }
        public string RecurrenceException { get; set; }
        public bool IsAllDay { get; set; }
        public string Color { get; set; }
    }
}