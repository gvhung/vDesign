using Base.Attributes;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

// ReSharper disable once CheckNamespace
namespace Base.BusinessProcesses.Entities
{
    [Table("RestoreSteps")]
    public class RestoreStep : Step
    {
        public RestoreStep() 
        {
            BaseOutputs = new List<Output>();
        }

        public RestoreStep(RestoreStep src) : base(src)
        {
            BackStepCount = src.BackStepCount;
            BaseOutputs = new List<Output>();
        }

        public override FlowStepType StepType { get { return FlowStepType.RestoreStep; } }

        [DetailView("Количество шагов назад по истории")]
        public int BackStepCount { get; set; }
    }
}