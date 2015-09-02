using System;

namespace Base
{
    public interface IScheduler
    {   
        string Title { get; }
        DateTime Start { get; }
        DateTime End { get; }
        string Description { get; }
        string StartTimezone { get; }
        string EndTimezone { get; }
        string Recurrence { get; }
        string RecurrenceRule { get; }
        int? RecurrenceID { get; }
        string RecurrenceException { get; }
        bool IsAllDay { get; }
        string Color { get; }
    }
}
