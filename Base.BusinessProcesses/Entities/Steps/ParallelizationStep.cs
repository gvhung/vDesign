using Base.Attributes;
using Framework.Maybe;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

// ReSharper disable once CheckNamespace
namespace Base.BusinessProcesses.Entities
{
    /// <summary>
    /// Шаг для распараллеливания бизнес процесса
    /// </summary>
    [Table("ParallelSteps")]
    public class ParallelizationStep : Step
    {
        public ParallelizationStep() { }

        public ParallelizationStep(ParallelizationStep src) : base(src)
        {         
        }

        public override FlowStepType StepType
        {
            get { return FlowStepType.ParalleizationStep; }
        }

        [NotMapped]
        [DetailView(Name = "Действия", HideLabel = true, TabName = "[1]Действия")]
        public ICollection<Output> Outputs
        {
            get { return BaseOutputs.With(x => x.ToList()); }
            set { BaseOutputs = value.With(x => x.ToList()); }
        }
    }
}

