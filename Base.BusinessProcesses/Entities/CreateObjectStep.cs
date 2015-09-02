using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Base.Attributes;
using Framework.Maybe;

namespace Base.BusinessProcesses.Entities
{
    [Table("CreateObjectSteps")]
    public class CreateObjectStep : Step
    {
        [ListView, DetailView("Тип объекта", Required = true), PropertyDataType("ListBaseObjects")]
        public string ObjectType { get; set; }

        [SystemPropery]
        public string ParentObjectType { get; set; }

        [PropertyDataType("BPObjectEditButton")]
        [DetailView("Инициализатор объекта")]
        public virtual ICollection<CreateObjectStepMemberInitItem> InitItems { get; set; }

        [NotMapped]
        public ICollection<Output> Outputs
        {
            get { return this.BaseOutputs ?? new List<Output> { new Output() }; }
            set { this.BaseOutputs = value.With(x => x.ToList()); }
        }

        public override FlowStepType StepType { get { return FlowStepType.CreateObjectTask; } }
    }
}