using Base.Attributes;
using Base.Entities;
using Framework.Attributes;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

// ReSharper disable once CheckNamespace
namespace Base.BusinessProcesses.Entities
{
    [Table("Steps")]
    public class Step : BaseObject, IRuntimeBindingType
    {
        public Step()
        {
            BaseOutputs = new List<Output>();
        }

        public Step(Step src)
        {
            Title = src.Title;
            Description = src.Description;
            IsEntryPoint = src.IsEntryPoint;
            ViewID = src.ViewID;
            StepName = src.StepName;
        }

        public virtual void LoadOutputs(IEnumerable<Output> srcOutputs)
        {
            if (BaseOutputs != null && BaseOutputs.Any()) return;

            BaseOutputs = new List<Output>();
            if (srcOutputs == null) return;
            foreach (var src in srcOutputs.Where(src => !src.Hidden))
            {
                BaseOutputs.Add(new Output(src));
            }
        }


        [ListView, MaxLength(255), FullTextSearchProperty]
        [DetailView(Name = "Наименование", Required = true, Order = 0)]
        public string Title { get; set; }

        [ListView, DetailView(Name = "Описание", Order = 1)]
        public string Description { get; set; }

        public bool IsEntryPoint { get; set; }

        public string ViewID { get; set; }

        public int WorkflowID { get; set; }

        [JsonIgnore]
        public virtual Workflow Workflow { get; set; }

        public virtual FlowStepType StepType { get { return FlowStepType.Step; } }

        [NotMapped]
        public string RuntimeType { get { return GetType().GetBaseObjectType().FullName; } }

        [JsonIgnore]
        public virtual ICollection<Output> BaseOutputs { get; set; } // For mapping

        [ListView, MaxLength(255)]
        [DetailView(Name = "Служебное имя", Order = 3)]
        public string StepName { get; set; } // Для жесткого возврата по истории gotostep
    }
}