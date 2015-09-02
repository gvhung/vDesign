using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Base.Attributes;
using Base.Security;
using Base.Task.Entities;
using Framework.Maybe;

namespace Base.BusinessProcesses.Entities
{
    [Table("TaskSteps")]
    public class TaskStep : Step
    {
        [DetailView(Name = "Шаблон заголовка задачи", Required = true)]
        [MaxLength(255)]
        public string TitleTemplate { get; set; }

        [DetailView(Name = "Шаблон описания задачи")]
        [PropertyDataType(PropertyDataType.MultilineText)]
        public string DescriptionTemplate { get; set; }

        [PropertyDataType("Duration"), DetailView(Name = "Срок исполнения")]
        public int PerformancePeriod { get; set; }

        public int? CategoryID { get; set; }

        [ForeignKey("CategoryID"), DetailView(Name = "Категория", Required = true)]
        public virtual TaskCategory TaskCategory { get; set; }

        public int? AssignedToCategoryID { get; set; }
        [DetailView(Name = "Отдел исполнителей", TabName = "[1]Ответственные")]
        public virtual UserCategory AssignedToCategory { get; set; }

        public string ObjectType { get; set; }

        [DetailView("Условие выполнения задачи")]
        public ConditionalOperator ConditionalOperator { get; set; }

        [DetailView(Name = "Исполнители", TabName = "[1]Ответственные")]
        public virtual ICollection<TaskStepUser> AssignedToUsers { get; set; }

        [NotMapped]
        public ICollection<Output> Outputs
        {
            get { return this.BaseOutputs ?? new List<Output> { new Output() }; }
            set { this.BaseOutputs = value.With(x => x.ToList()); }
        }

        public override FlowStepType StepType { get { return FlowStepType.TaskStep; } }
    }
}