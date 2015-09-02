using Base.Security;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Base.BusinessProcesses.Entities
{
    [Table("StagePerforms")]
    public class StagePerform : BaseObject
    {
        public int StageId { get; set; }
        public virtual Stage Stage { get; set; }

        public int? PerformUserID { get; set; }
        public virtual User PerformUser { get; set; }

        public int? FromUserID { get; set; }
        public virtual User FromUser { get; set; }

        public virtual ICollection<BPTask> Tasks { get; set; }

        public DateTime BeginDate { get; set; }

        public int? ObjectID { get; set; }
        public string ObjectType { get; set; }

        public int? PositionID { get; set; }
        public virtual WorkflowHierarchyPosition Position { get; set; }
    }


    public class WorkflowHierarchyPosition : BaseObject
    {
        public int CurrentID { get; set; }
        public virtual Workflow Current { get; set; }

        public int? ParentID { get; set; }
        public virtual WorkflowHierarchyPosition Parent { get; set; }


        public int? CurrentWorkflowContainerID { get; set; }
        public virtual WorkflowOwnerStep CurrentWorkflowContainer { get; set; }

        public string GetStageWorkflowHierarchy()
        {
            return GetStageWorkflowHierarchy(this);
        }

        private string GetStageWorkflowHierarchy(WorkflowHierarchyPosition parent)
        {
            string retVal = parent.Current.Title;
            if (parent.Parent != null)
            {
                retVal = retVal.Insert(0, GetStageWorkflowHierarchy(parent.Parent) + ".");
            }
            return retVal;
        }

        public string GetStageContainer()
        {
            return GetStageContainer(this);
        }

        private string GetStageContainer(WorkflowHierarchyPosition position)
        {
            string retVal = position.CurrentWorkflowContainer != null ? position.CurrentWorkflowContainer.Title : "";
            if (position.Parent != null)
            {
                retVal = retVal.Insert(0, GetStageContainer(position.Parent));
            }
            return retVal;
        }
    }
}