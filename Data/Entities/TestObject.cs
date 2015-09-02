using Base;
using Base.Attributes;
using Base.BusinessProcesses.Attributes;
using Base.BusinessProcesses.Entities;
using Base.Security;
using Base.Security.ObjectAccess;
using Base.Security.ObjectAccess.Policies;
using Base.Validation;
using Microsoft.Linq.Translations;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Entities
{
    [AccessPolicy(typeof(EditCreatorOnlyAccessPolicy))]
    public class TestObject : BaseObject, IBPObject, IAccessibleObject
    {
        public TestObject()
        {
        }

        [DetailView(Name = "Наименование документа", Required = true, Description = "Каждый веб-разработчик знает, что такое текст-«рыба». Текст этот, несмотря на название, не имеет никакого отношения к обитателям водоемов. Используется он веб-дизайнерами для вставки на интернет-страницы и демонстрации внешнего вида контента, просмотра шрифтов, абзацев, отступов и т.д. Так как цель применения такого текста исключительно демонстрационная, то и смысловую нагрузку ему нести совсем необязательно. Более того, нечитабельность текста сыграет на руку при оценке качества восприятия макета.")]
        [ListView]
        public string Title { get; set; }

        [DetailView(Name = "Дата", Description = "Каждый веб-разработчик знает, что такое текст-«рыба».")]
        [NotMapped]
        public DateTime? Date { get; set; }

        [DetailView(Name = "Дабл",  Description = "Каждый веб-разработчик знает, что такое текст-«рыба».")]
        [NotMapped]
        public double Double { get; set; }

        [DetailView(Name = "Итерация")]
        [PropertyDataType(PropertyDataType.Number)]
        [ListView]
        [BusinessProcessProperty]
        public int Iteration { get; set; }

        [DetailView(Name = "Этап")]
        [ListView]
        [BusinessProcessProperty]
        public State State { get; set; }

        [DetailView(Name = "Владелец")]
        [ListView]
        public virtual User User { get; set; }
        public int? UserID { get; set; }

        private static readonly CompiledExpression<TestObject, string> _userName =
            DefaultTranslationOf<TestObject>.Property(x => x.UserName).Is(x => x.User != null ? x.User.FirstName : ""); 

        [DetailView(Name = "Владелец")]
        [ListView]
        public string UserName 
        {
            get
            {
                return _userName.Evaluate(this);
            }
        }

        private static readonly CompiledExpression<TestObject, Post> _userPost =
            DefaultTranslationOf<TestObject>.Property(x => x.UserPost).Is(x => x.User != null ? x.User.Post : null);

        [DetailView(Name = "Должность")]
        [ListView]
        public Post UserPost
        {
            get
            {
                return _userPost.Evaluate(this);
            }
        }

        [DetailView(Name = "Bool")]
        [NotMapped]
        public bool Bullshit { get; set; }


        //private static readonly CompiledExpression<TestObject, User> s_workflowAdmin =
        //    DefaultTranslationOf<TestObject>.Property(x => x.WorkflowAdmin).Is(x => x.WorkflowContext.Workflow != null ? x.WorkflowContext.Workflow.Curator : null);

        //[DetailView(Name = "Воркфлоу админ")]
        //[ListView]
        //public User WorkflowAdmin
        //{
        //    get
        //    {
        //        return s_workflowAdmin.Evaluate(this);
        //    }
        //}

        [PropertyDataType("InitWorkflow")]
        [NotMapped]
        [DetailView("Шаблон бизнес-процесса", TabName = "Бизнес процесс", HideLabel = true)]
        public Workflow InitWorkflow { get; set; }

        public int? DocumentEntryID { get; set; }

        [DetailView("Записи")]
        public virtual ICollection<TestObjectEntry> TestObjectEntries { get; set; }

        [DetailView(Name = "Продолжительность следующего этапа")]
        [PropertyDataType("Duration")]
        //[NotMapped]
        public int? NextStageDuration { get; set; }

        public int? WorkflowContextID
        {
            get; set;
        }

        public virtual WorkflowContext WorkflowContext
        {
            get; set;
        }
    }
    

    public class TestObjectEntry : EasyCollectionEntry<TestObjectNestedEntry>
    {

    }

    public class TestObjectNestedEntry : BaseObject
    {
        [DetailView(Name = "System Name")]
        [ListView]
        public string Title { get; set; }
    }

    public enum State
    {
        [Description("Новый")]
        New = 0,
        [Description("В разработке")]
        Stage1 = 1,
        [Description("Новый 2")]
        Stage2 = 2
    }

    [Flags]
    enum Days
    {
        None = 0x0,
        Sunday = 0x1,
        Monday = 0x2,
        Tuesday = 0x4,
        Wednesday = 0x8,
        Thursday = 0x10,
        Friday = 0x20,
        Saturday = 0x40
    }
}
