using Base.BusinessProcesses.Entities;
using Base.UI;
using System;
using System.Collections.Generic;

namespace WebUI.Models.BusinessProcess
{
    public class WorkflowOwnerStepTimeLineLementVm : TimeLineElementVm 
    {
        public WorkflowOwnerStepTimeLineLementVm(DateTime? endDate , ChangeHistory history)
            : base(history)
        {

            var workflowOwnerStep = history.Step as WorkflowOwnerStep;
            // ReSharper disable once PossibleNullReferenceException
            ChildWorkflowID = workflowOwnerStep.ChildWorkflow.ID;
            Date = history.Date;
            Color = "#f0ad4e";
            EndDate = endDate;
        }

        public int ChildWorkflowID { get; set; }
        
        public DateTime Date { get; set; }

        public DateTime? EndDate { get; set; }
    }
}