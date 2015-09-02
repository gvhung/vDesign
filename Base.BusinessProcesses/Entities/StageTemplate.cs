using Base.Attributes;
using Base.Task.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Base.BusinessProcesses.Entities
{
    [Table("StageTemplates")]
    public class StageTemplate : BaseObject, ICategorizedItem
    {
        public StageTemplate()
        {
            CreateTask = true;
        }

        [DetailView("Наименование", Required = true), ListView]
        public string Title { get; set; }

        [DetailView(Name = "Шаблон заголовка напоминания", Required = true)]
        [MaxLength(255)]
        public string TitleTemplate { get; set; }

        [DetailView(Name = "Шаблон описания напоминания")]
        [PropertyDataType(PropertyDataType.MultilineText)]
        public string DescriptionTemplate { get; set; }

        [NotMapped]
        [PropertyDataType("StepHelp")]
        [DetailView("Доступные свойства", TabName = "Помощь")]
        public string Help { get; set; }

        [ListView]
        [DetailView(Name = "Описание")]
        public string Description { get; set; }

        [PropertyDataType("Color")]
        [DetailView(Name = "Цвет")]
        [ListView]
        public string Color { get; set; }

        [DetailView(Name = "Создавать напоминание")]
        [ListView]
        public bool CreateTask { get; set; }

        [DetailView(Name = "Выбор исполнителя", TabName = "[1]Ответственные")]
        public bool IsCustomPerformer { get; set; }

        public int CategoryID { get; set; }

        [ForeignKey("CategoryID")]
        public virtual TaskCategory TaskCategory { get; set; }

        [DetailView(Name = "Срок исполнения")]
        [PropertyDataType("Duration")]
        public int PerformancePeriod { get; set; }

        [Obsolete]
        //[DetailView(Name = "На инициатора (имеет приоритет)", TabName = "[1]Ответственные")]
        public bool IsLoop { get; set; }

        [ListView, DetailView("Тип объекта", Required = true), PropertyDataType("ListWFObjects")]
        public string ObjectType { get; set; }

        [DetailView(Name = "Действия", HideLabel = true, TabName = "[1]Действия")]
        public virtual ICollection<TemplateAction> Actions { get; set; }

        #region ICategorizedItem

        HCategory ICategorizedItem.Category
        {
            get { return TaskCategory; }
        }

        #endregion
    }
}