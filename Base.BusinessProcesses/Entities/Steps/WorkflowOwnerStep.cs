using Base.Attributes;
using Base.UI;
using Framework.Maybe;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

// ReSharper disable once CheckNamespace
namespace Base.BusinessProcesses.Entities
{
    [Table("WorkflowOwnerStep")]
    public class WorkflowOwnerStep : Step
    {
        public WorkflowOwnerStep() { }

        public WorkflowOwnerStep(WorkflowOwnerStep src)
            : base(src)
        {
            DetailViewSettingID = src.DetailViewSettingID;
        }

        public void LoadChildWF(Workflow src)
        {
            ChildWorkflow = new Workflow(src);
            
        }

        public int? ChildWorkflowId { get; set; }

        [DetailView(Name = "Дочерний бизнес-процесс", Order = 3)]
        [ListView]
        public virtual Workflow ChildWorkflow { get; set; }

        public override FlowStepType StepType { get { return FlowStepType.WorkflowOwnerStep; } }

        [NotMapped]
        public ICollection<Output> Outputs
        {
            get { return BaseOutputs.With(x => x.ToList()); }
            set { BaseOutputs = value.With(x => x.ToList()); }
        }

        public int? DetailViewSettingID { get; set; }

        [DetailView(Name = "Настройка формы объекта", Order = 4)]
        [PropertyDataType("DetailViewSetting")]
        public virtual DetailViewSetting DetailViewSetting { get; set; }
    }
}
