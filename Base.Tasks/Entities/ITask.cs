using Base.Entities.Complex;
using Base.Security;
using System;
using System.Collections.Generic;


namespace Base.Task.Entities
{
    public interface ITask: IBaseObject
    {
        int CategoryID { get; set; }
        string Title { get; set; }
        string Description { get; set; }
        Priority Priority { get; set; }
        Period Period { get; set; }
        double PercentComplete { get; set; }
        TaskStatus Status { get; set; }
        bool System { get; set; }
        User AssignedFrom { get; set; }
        int? AssignedToID { get; set; }
        User AssignedTo { get; set; }
        DateTime? CompliteDate { get; set; }
        ICollection<TaskFile> Files { get; set; }
        ICollection<TaskChangeHistory> TaskChangeHistory { get; set; }
        bool Auto { get; set; }
    }
}
