using Base.Attributes;
using Base.Security;
using Base.Task.Entities;
using Framework.Maybe;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Base.BusinessProcesses.Entities.Steps;

// ReSharper disable once CheckNamespace
namespace Base.BusinessProcesses.Entities
{
    [Table("TaskSteps")]
    public class TaskStep : Step
    {
        public TaskStep() { }

        public TaskStep(TaskStep src)
            : base(src)
        {
            TitleTemplate = src.TitleTemplate;
            DescriptionTemplate = src.DescriptionTemplate;
            PerformancePeriod = src.PerformancePeriod;
            AssignedToCategory = src.AssignedToCategory;
            ObjectType = src.ObjectType;
        }

        public void LoadAssignedToUsers(IEnumerable<TaskStepUser> srcTaskStepUsers)
        {
            AssignedToUsers = new List<TaskStepUser>();
            foreach (var src in srcTaskStepUsers)
            {
                AssignedToUsers.Add(new TaskStepUser
                {
                    PerformerID =  src.PerformerID,
                
                });
            }
        }

        [DetailView(Name = "Шаблон заголовка напоминания", Required = true)]
        [MaxLength(255)]
        public string TitleTemplate { get; set; }

        [DetailView(Name = "Шаблон описания напоминания")]
        [PropertyDataType(PropertyDataType.MultilineText)]
        public string DescriptionTemplate { get; set; }

        [PropertyDataType("Duration"), DetailView(Name = "Срок исполнения")]
        public int PerformancePeriod { get; set; }

        public int? CategoryID { get; set; }

        [ForeignKey("CategoryID"), DetailView(Name = "Категория", Required = true)]
        public virtual TaskCategory TaskCategory { get; set; }

        [DetailView(Name = "Отдел исполнителей", TabName = "[1]Ответственные")]
        public virtual List<StageUserCategory> AssignedToCategory { get; set; }

        public string ObjectType { get; set; }

        [DetailView("Условие выполнения напоминания")]
        public ConditionalOperator ConditionalOperator { get; set; }

        [DetailView(Name = "Исполнители", TabName = "[1]Ответственные")]
        public virtual ICollection<TaskStepUser> AssignedToUsers { get; set; }

        [NotMapped]
        public ICollection<Output> Outputs
        {
            get { return BaseOutputs; }
            set { BaseOutputs = value.With(x => x.ToList()); }
        }

        public override FlowStepType StepType { get { return FlowStepType.TaskStep; } }
    }
}