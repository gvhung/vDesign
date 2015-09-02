using System;
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
    [Table("Stages")]
    public class Stage : Step
    {
        [PropertyDataType("Color")]
        [DetailView(Name = "Цвет", Order = 2)]
        [ListView]
        public string Color { get; set; }

        public int? CategoryID { get; set; }

        [DetailView(Name = "Создавать задачу", Order = 3)]
        [ListView]
        public bool CreateTask { get; set; }

        [ForeignKey("CategoryID"), DetailView(Name = "Категория задачи", Required = true, Order = 4)]
        public virtual TaskCategory TaskCategory { get; set; }

        [DetailView(Name = "Шаблон заголовка задачи", Required = true, Order = 5)]
        [MaxLength(255)]
        public string TitleTemplate { get; set; }

        [DetailView(Name = "Шаблон описания задачи", Order = 6)]
        [PropertyDataType(PropertyDataType.MultilineText)]
        public string DescriptionTemplate { get; set; }

        [DetailView("Доступные свойства", TabName = "Помощь")]
        [NotMapped]
        [PropertyDataType("StepHelp")]
        public string Help { get; set; }

        [PropertyDataType("Duration"), DetailView(Name = "Срок исполнения", Order = 7)]
        public int PerformancePeriod { get; set; }

        [DetailView("Автопереход по истечении времени", Order = 8)]
        public bool AutoProcess { get; set; }

        [Obsolete]
        //[DetailView(Name = "На инициатора (имеет приоритет)", TabName = "[1]Ответственные", Order = 8)]
        public bool IsLoop { get; set; }

        public int? AssignedToCategoryID { get; set; }
        [DetailView(Name = "Отдел исполнителей", TabName = "[1]Ответственные")]
        public virtual UserCategory AssignedToCategory { get; set; }

        [PropertyDataType("EasyCollection", Params = "Performer;User;FullName")]
        [DetailView(Name = "Исполнители", TabName = "[1]Ответственные")]
        public virtual ICollection<StageUser> AssignedToUsers { get; set; }

        public string ObjectType { get; set; }

        [DetailView(Name = "Выбор исполнителя", TabName = "[1]Ответственные")]
        public bool IsCustomPerformer { get; set; }

        [NotMapped]
        [DetailView(Name = "Действия", HideLabel = true, TabName = "[1]Действия")]
        public ICollection<StageAction> Outputs
        {
            get { return this.BaseOutputs.With(x => x.OfType<StageAction>()).With(x => x.ToList()); }
            set { this.BaseOutputs = value.With(x => x.OfType<Output>()).With(x => x.ToList()); }
        }

        public override FlowStepType StepType { get { return FlowStepType.Stage; } }

        [DetailView(Name = "Стратегия выбора исполнителей", TabName = "[1]Ответственные")]
        [PropertyDataType("StakeholdersSelectionStrategy")]
        public string StakeholdersSelectionStrategy { get; set; }
    }
}