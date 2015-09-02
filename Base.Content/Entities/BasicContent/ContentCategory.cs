using Base.Attributes;
using Base.Security.ObjectAccess;
using Base.UI;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Base.Entities.Complex;

namespace Base.Content.Entities
{
    public class ContentCategory : HCategory, IExtendedCategory, ITreeNodeImage
    {
        public ContentCategory()
        {
            this.ShowInMenu = true;
            this.PublicTitle = new MultilanguageText();
        }

        #region ITreeNodeImage
        public int? ImageID { get; set; }

        [ListView(Width = 24, Height = 24)]
        [DetailView(Name = "Изображение", Order = 1)]
        [PropertyDataType("Image")]
        public virtual FileData Image { get; set; }
        #endregion
        
        [ListView]
        [DetailView(Name = "Наименование на сайте", Order = 2)]
        public MultilanguageText PublicTitle { get; set; }

        [ForeignKey("ParentID")]
        [JsonIgnore]
        public virtual ContentCategory Parent_ { get; set; }

        [JsonIgnore]
        public virtual ICollection<ContentCategory> Children_ { get; set; }

        [DetailView(Name = "Группа", Required = true, Order = 3)]
        [PropertyDataType("ContentCategoryType")]
        public ContentCategoryType ContentCategoryType { get; set; }

        [SystemPropery]
        public string Action { get; set; }

        [SystemPropery]
        public string Controller { get; set; }

        [SystemPropery]
        public string Params { get; set; }

        #region HCategory
        public override HCategory Parent
        {
            get { return this.Parent_; }
        }

        public override IEnumerable<HCategory> Children
        {
            get { return this.Children_ ?? new List<ContentCategory>(); }
        }
        #endregion

        #region IExtendedCategory
        public string CategoryItemMnemonic { get; set; }
        #endregion
      
        [DetailView(Name = "Показывать в меню", Order = 4)]
        public bool ShowInMenu { get; set; }

        [DetailView("Разворачивать контент", Order = 4)]
        public bool Expanded { get; set; }

        [DetailView(Name = "Доступен для подписки", Order = 5)]
        public bool SubscribeAvailable { get; set; }

        [DetailView(TabName = "[1]Виджет", Name = "Controller")]
        [MaxLength(255)]
        public string WidgetController { get; set; }

        [DetailView(TabName = "[1]Виджет", Name = "Action")]
        [MaxLength(255)]
        public string WidgetAction { get; set; }

        [DetailView(TabName = "[2]Дополнительно", Name = "Идентификатор")]
        public int UID
        {
            get { return this.ID; }
        }
    }

    public enum ContentCategoryType
    {
        [Description("Раздел")]
        ContentFolder = 0,

        [Description("Контент")]
        ContentRegular = 10,

        [Description("Расширенное")]
        ContentExtended = 20,

        [Description("Баннер")]
        Banner = 30,
    }

    //public enum WidgetType
    //{
    //    [Description("Персона недели")]
    //    PersonOfTheWeek = 0,
    //    [Description("Новости")]
    //    News,
    //    [Description("НПА недели")]
    //    NPAOfTheWeek,
    //    [Description("Видео")]
    //    Video,
    //    [Description("Статистика")]
    //    Statistic,
    //    [Description("Рейтинги")]
    //    Rating,
    //    [Description("Регион месяца")]
    //    RegionOfTheMonth
    //}
}
