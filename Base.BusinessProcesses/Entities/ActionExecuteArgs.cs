using Base.BusinessProcesses.Services.Concrete;
using Base.DAL;
using System;
using System.Collections.Generic;

namespace Base.BusinessProcesses.Entities
{
    public class ActionExecuteArgs
    {
        public IUnitOfWork UnitOfWork { get; set; }
        public BaseObject OldObject { get; set; }
        public BaseObject NewObject { get; set; }
        public Stage CurrentStage { get; set; }
        public Workflow Workflow { get; set; }
        internal Func<StageAction, BaseObject, WorkflowService.StageJumper> GetStageJumper { get; set; }
        public StageAction EvaluatedAction { get; set; }
        public ICollection<Stage> FindNextStages(StageAction action = null)
        {            
            var jumper = GetStageJumper(action ?? EvaluatedAction, NewObject);
            if (jumper != null && jumper.Stages != null)
                return jumper.Stages;
            return new List<Stage>();
        }

        public string Comment { get; set; }
    }
}