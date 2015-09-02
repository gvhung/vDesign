using Base.Attributes;
using Framework.Maybe;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

// ReSharper disable once CheckNamespace
namespace Base.BusinessProcesses.Entities
{
    [Table("CreateObjectSteps")]
    public class CreateObjectStep : Step
    {
        public CreateObjectStep()
        { }

        public CreateObjectStep(CreateObjectStep src)
            : base(src)
        {
            ObjectType = src.ObjectType;
            ParentObjectType = src.ParentObjectType;            
        }

        public void LoadInitItems(IEnumerable<CreateObjectStepMemberInitItem> srcItems)
        {
            InitItems = new List<CreateObjectStepMemberInitItem>();
            foreach (var src in srcItems)
            {
                InitItems.Add(new CreateObjectStepMemberInitItem(src));
            }
        }

        [ListView, DetailView("Тип объекта", Required = true), PropertyDataType("ListBaseObjects")]
        public string ObjectType { get; set; }

        [SystemProperty]
        public string ParentObjectType { get; set; }

        [PropertyDataType("BPObjectEditButton")]
        [DetailView("Инициализатор объекта")]
        public virtual ICollection<CreateObjectStepMemberInitItem> InitItems { get; set; }

        [NotMapped]
        public ICollection<Output> Outputs
        {
            get { return BaseOutputs; }
            set { BaseOutputs = value.With(x => x.ToList()); }
        }

        public override FlowStepType StepType { get { return FlowStepType.CreateObjectTask; } }
    }
}