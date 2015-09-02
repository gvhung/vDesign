using Base.Attributes;
using Base.Entities;
using System.ComponentModel.DataAnnotations.Schema;

// ReSharper disable once CheckNamespace
namespace Base.BusinessProcesses.Entities
{
    [Table("ExtendedStages")]
    public class ExtendedStage : Stage
    {
        [DetailView("Дополнительно")]
        [PropertyDataType("StageExtender")]
        public virtual  StageExtender Extender { get; set; }
        public int ExtenderID { get; set; }

        public string Mnemonic { get; set; }
        public override FlowStepType StepType { get { return FlowStepType.ExtendedStage; } }
    }

    [Table("StageExtenders")]
    public class StageExtender : BaseObject, IRuntimeBindingType
    {
        [NotMapped]
        public string RuntimeType { get { return GetType().GetBaseObjectType().FullName; } }
    }
}