using Base.Attributes;
using Base.BusinessProcesses.Entities;
using Base.Security;
using Base.Security.ObjectAccess;
using Framework.Attributes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Base.Content.Entities
{
    [EnableFullTextSearch]
    public class ContentItem : BaseObject, ICategorizedItem, IBPObject, IEnabledState
    {

        public ContentItem()
        {
            this.Content = new Content();
        }

        #region COMMON
        public int? ImagePreviewID { get; set; }

        [ListView]
        [DetailView(Name = "Изображение", Order = 0)]
        [PropertyDataType("Image")]
        public virtual FileData ImagePreview { get; set; }

        [ListView]
        [MaxLength(255)]
        [FullTextSearchProperty]
        [DetailView(Name = "Наименование", Required = true, Order = 1)]
        public string Title { get; set; }

        [ListView]
        [MaxLength(255)]
        [FullTextSearchProperty]
        [DetailView(Name = "Значение", Order = 1)]
        public string Value { get; set; }

        [ListView]
        [FullTextSearchProperty]
        [DetailView(Name = "Топ", Order = 1)]
        public bool Top { get; set; }

        [DetailView(Name = "Показывать на главной", Order = 1)]
        public bool OnHome { get; set; }

        [ListView(Hidden = true)]
        [DetailView(Name = "Краткое описание", Order = 2)]
        [MaxLength(255)]
        [FullTextSearchProperty]
        public string ShortDescription { get; set; }

        [ListView(Hidden = true)]
        [DetailView(Name = "Описание", Order = 2)]
        [FullTextSearchProperty]
        public string Description { get; set; }

        [ListView(Order = 8)]
        [DetailView(Name = "Дата", Order = 4, Required = true)]
        [PropertyDataType(PropertyDataType.DateTime)]
        public DateTime? Date { get; set; }

        [ListView]
        [DetailView(Name = "Ссылка", Order = 5, Required = true)]
        [PropertyDataType("String")]
        //[PropertyDataType(PropertyDataType.Url)]
        public string Src { get; set; }

        // STATUS
        // PERFORMER

        [DetailView(TabName = "[1]Контент")]
        public Content Content { get; set; }

        [ListView]
        [DetailView(Name = "Статус", ReadOnly = true, Order = 6)]
        public ContentItemStatus ContentItemStatus { get; set; }

        [ListView]
        [DetailView(Name = "Дата создания", ReadOnly = true, Order = 7)]
        [PropertyDataType(PropertyDataType.DateTime)]
        public DateTime Created { get; set; }

        [JsonIgnore]
        [DetailView(Name = "Тэги", TabName = "[3]Дополнительно", DeferredLoading = true, Order = 2)]
        [FullTextSearchProperty]
        public virtual ICollection<Tag> Tags { get; set; }
        #endregion

        #region IBPObject
        public int? WorkflowID { get; set; }

        public Workflow Workflow { get; set; }

        [PropertyDataType("InitWorkflow")]
        [NotMapped]
        //[DetailView("Шаблон бизнес-процесса", HideLabel = true, TabName = "[9]Бизнес процесс")]
        public Workflow InitWorkflow { get; set; }
        #endregion

        #region ICategorizedItem
        public int CategoryID { get; set; }

        [JsonIgnore]
        [ForeignKey("CategoryID")]
        public virtual ContentCategory ContentCategory { get; set; }

        HCategory ICategorizedItem.Category
        {
            get { return this.ContentCategory; }
        }
        #endregion

        #region IEnabledState
        public bool IsEnabled(ISecurityUser user)
        {
            return true;
            //return this.ContentItemStatus == ContentItemStatus.New
            //    || this.ContentItemStatus == ContentItemStatus.Rework;
        }
        #endregion

        public int? WorkflowContextID { get; set; }

        public virtual WorkflowContext WorkflowContext
        {
            get; set; 
        }
    }

    public enum ContentItemStatus
    {
        [Description("Запись создана")]
        New = 0,

        [Description("На модерации")]
        Moderating = 10,

        [Description("Доработка")]
        Rework = 20,

        [Description("Запись опубликована")]
        Published = 30,

        [Description("Архив")]
        Archive = 40,
    }
}
