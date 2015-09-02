using Base.Attributes;
using Base.UI;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Base.Content.Entities
{
    public class ContentCategory : HCategory, IExtendedCategory, ITreeNodeImage
    {
        [ForeignKey("ParentID")]
        [JsonIgnore]
        public virtual ContentCategory Parent_ { get; set; }

        [JsonIgnore]
        public virtual ICollection<ContentCategory> Children_ { get; set; }

        [DetailView(Name = "Группа", Required = true, Order = 2)]
        [PropertyDataType("ContentCategoryType")]
        public ContentCategoryType ContentCategoryType { get; set; }

        [SystemProperty]
        public string Action { get; set; }

        [SystemProperty]
        public string Controller { get; set; }

        [SystemProperty]
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

        #region ITreeNodeImage
        public int? ImageID { get; set; }

        [ListView(Width = 24, Height = 24)]
        [DetailView(Name = "Изображение", Order = 1)]
        [PropertyDataType("Image")]
        public virtual FileData Image { get; set; }
        #endregion


        [ListView]
        [DetailView(Name = "Краткое описание", Order = 2)]
        public string PublicTitle { get; set; }

        [DetailView(Name = "Показывать в меню", Order = 4)]
        public bool ShowInMenu { get; set; }

        [DetailView(Name = "Показывать на главной", Order = 4)]
        public bool ShowOnHomePage { get; set; }

        [DetailView("Разворачивать контент", Order = 4)]
        public bool Expanded { get; set; }

        [DetailView("Показывать ссылки на социальные сети", Order = 4)]
        public bool IsShowSocialLinks { get; set; }

        [DetailView(Name = "Идентификатор", Order = 5)]
        public int UID
        {
            get { return this.ID; }
        }

        [DetailView(Name = "Доступен для подписки", Order = 6)]
        public bool SubscribeAvailable { get; set; }

        [DetailView(TabName = "[1]Виджет", Name = "Controller")]
        [MaxLength(255)]
        public string WidgetController { get; set; }

        [DetailView(TabName = "[1]Виджет", Name = "Action")]
        [MaxLength(255)]
        public string WidgetAction { get; set; }

        [InverseProperty("ContentCategory")]
        [JsonIgnore]
        public virtual ICollection<ContentSubscriber> ContentSubscribers { get; set; }
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
}
