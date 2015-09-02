using Base.Attributes;
using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Base.Security;
using System.ComponentModel;
using Framework.Attributes;
using Newtonsoft.Json;
using Base.BusinessProcesses.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using Base.Security.ObjectAccess;
using Base.Registers.Entities;
using Base.Entities.Complex;

namespace Base.Content.Entities
{
    [EnableFullTextSearch]
    public class ContentItem : BaseObject, ICategorizedItem, IBPObject
    {
        public ContentItem()
        {
            this.Title = new MultilanguageText();
            this.Value = new MultilanguageText();
            this.Description = new MultilanguageTextArea();
            this.Content = new MultilanguageContent();
        }

        #region COMMON
        public int? ImagePreviewID { get; set; }

        [ListView]
        [DetailView(Name = "Изображение", Order = 0)]
        [PropertyDataType("Image")]
        public virtual FileData ImagePreview { get; set; }

        [ListView]
        [FullTextSearchProperty]
        [DetailView(Name = "Наименование", Required = true, Order = 1)]
        public MultilanguageText Title { get; set; }

        [ListView]
        [FullTextSearchProperty]
        [DetailView(Name = "Значение", Order = 1)]
        public MultilanguageText Value { get; set; }

        [ListView]
        [FullTextSearchProperty]
        [DetailView(Name = "Топ", Order = 1)]
        public bool Top { get; set; }

        [ListView(Hidden = true)]
        [DetailView(Name = "Описание", Order = 2)]
        [FullTextSearchProperty]
        public MultilanguageTextArea Description { get; set; }

        //public int? RegionID { get; set; }

        //[FullTextSearchProperty]
        //[ListView(Name = "МО")]
        //[DetailView(Name = "Муниципальное образование", Order = 3, Required = true)]
        //public virtual Region Region { get; set; }

        [ListView(Order = 8)]
        [DetailView(Name = "Дата", Order = 4, Required = true)]
        [PropertyDataType(PropertyDataType.DateTime)]
        public DateTime? Date { get; set; }

        [ListView]
        [DetailView(Name = "Ссылка", Order = 5, Required = true)]
        [PropertyDataType(PropertyDataType.Url)]
        public string Src { get; set; }

        // STATUS
        // PERFORMER

        [DetailView(TabName = "[1]Контент")]
        [PropertyDataType("Multilanguage")]
        public MultilanguageContent Content { get; set; }

        [ListView(Hidden = true)]
        [DetailView(Name = "Статус в системе", TabName = "[3]Дополнительно", ReadOnly = true, Order = 0)]
        public ContentItemStatus ContentItemStatus { get; set; }

        [ListView(Hidden = true)]
        [DetailView(Name = "Дата создания", TabName = "[3]Дополнительно", ReadOnly = true, Order = 1)]
        [PropertyDataType(PropertyDataType.DateTime)]
        public DateTime Created { get; set; }

        [JsonIgnore]
        [DetailView(Name = "Тэги", TabName = "[3]Дополнительно", DeferredLoading = true, Order = 2)]
        [FullTextSearchProperty]
        public virtual ICollection<Tag> Tags { get; set; }
        #endregion

        #region IBPObject
        public int? CurrentStageID { get; set; }

        [ListView(Order = 1)]
        [DetailView(Name = "Этап", ReadOnly = true, Order = 6)]
        public virtual Stage CurrentStage { get; set; }

        public int? PerformerID { get; set; }

        [ListView(Order = 2)]
        [DetailView(Name = "Исполнитель", ReadOnly = true, Order = 7)]
        public virtual User Performer { get; set; }
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
