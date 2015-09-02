using Base.Attributes;
using Framework.Maybe;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

// ReSharper disable once CheckNamespace
namespace Base.BusinessProcesses.Entities
{

    [Table("ParallelEndSteps")]
    public class ParallelEndStep : Step
    {
        public ParallelEndStep() { }

        public ParallelEndStep(ParallelEndStep src)
            : base(src)
        {
            WaitAllThreads = src.WaitAllThreads;
        }


        public override FlowStepType StepType { get { return FlowStepType.ParallelEndStep; } }

        [ListView]
        [DetailView(Name = "Ждать завершения всех потоков", Order = 3)]
        public bool WaitAllThreads { get; set; }

        [NotMapped]
        public ICollection<Output> Outputs
        {
            get { return BaseOutputs.With(x => x.ToList()); }
            set { BaseOutputs = value.With(x => x.ToList()); }
        }
    }
}