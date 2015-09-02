using Base.DAL;
using System;
using System.Collections.Generic;

namespace Base.BusinessProcesses.Events
{
    public class WorkflowTaskEventArgs : EventArgs
    {
        public WorkflowTaskEventArgs(IUnitOfWork unitOfWork, IEnumerable<Task.Entities.Task> tasksToCreate, IEnumerable<Task.Entities.Task> tasksToUpdate)
        {
            UnitOfWork = unitOfWork;
            TasksToUpdate = tasksToUpdate;
            TasksToCreate = tasksToCreate;
        }

        public IEnumerable<Task.Entities.Task> TasksToCreate { get; set; }
        public IEnumerable<Task.Entities.Task> TasksToUpdate { get; set; }
        public IUnitOfWork UnitOfWork { get; set; }
    }
}