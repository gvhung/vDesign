using Base.Attributes;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


// ReSharper disable once CheckNamespace
namespace Base.BusinessProcesses.Entities
{
    [Table("GotoSteps")]
    public class GotoStep : Step
    {
        public GotoStep()
        {
            BaseOutputs = new List<Output>();
        }

        public GotoStep(GotoStep src)
            : base(src)
        {
            ReturnToStep = src.ReturnToStep;
            BaseOutputs = new List<Output>();
        }

        [MaxLength(255)]
        [DetailView("Перейти к шагу", Required = true, Order = 10)]
        public string ReturnToStep { get; set; }

        public override FlowStepType StepType
        {
            get { return FlowStepType.GotoStep; }
        }
    }
}
