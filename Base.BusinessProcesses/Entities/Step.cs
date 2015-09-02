using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Base.Attributes;
using Base.Entities;
using Framework.Attributes;
using Newtonsoft.Json;

namespace Base.BusinessProcesses.Entities
{
    [Table("Steps")]
    public class Step : BaseObject, IRuntimeBindingType
    {
        [ListView, MaxLength(255), FullTextSearchProperty]
        [DetailView(Name = "Наименование", Required = true, Order = 0)]
        public string Title { get; set; }

        [ListView, DetailView(Name = "Описание", Order = 1)]
        public string Description { get; set; }

        public bool IsEntryPoint { get; set; }

        public string ViewID { get; set; }

        public int WorkflowID { get; set; }

        public virtual FlowStepType StepType { get { return FlowStepType.Step; } }

        [NotMapped]
        public string RuntimeType { get { return this.GetType().GetBaseObjectType().FullName; } }

        [JsonIgnore]
        public virtual ICollection<Output> BaseOutputs { get; set; } // For mapping

        [JsonIgnore]
        public virtual Workflow Workflow { get; set; }
    }
}