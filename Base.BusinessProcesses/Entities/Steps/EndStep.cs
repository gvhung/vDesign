using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

// ReSharper disable once CheckNamespace
namespace Base.BusinessProcesses.Entities
{
    [Table("EndSteps")]
    public sealed class EndStep : Stage
    {
        public EndStep()
        {
            BaseOutputs = new List<Output>();
        }

        public EndStep(EndStep src)
        {
            Title = src.Title;
            Description = src.Description;
            IsEntryPoint = src.IsEntryPoint;
            ViewID = src.ViewID;
            StepName = src.StepName;
            Hidden = src.Hidden;
            BaseOutputs = new List<Output>();
        }

        public override FlowStepType StepType
        {
            get
            {
                return FlowStepType.EndStep;
            }
        }
    }
}
