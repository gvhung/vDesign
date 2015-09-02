using Base.Attributes;
using Framework.Attributes;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Base.Content.Entities
{
    // Тэг для сущностей ContentItem (Article, EventArticle, News)
    [EnableFullTextSearch]
    public class Tag : BaseObject, ICategorizedItem
    {
        [ListView]
        [FullTextSearchProperty]
        [MaxLength(255)]
        [DetailView(Name = "Наименование")]
        public string Title { get; set; }

        [JsonIgnore]
        public ICollection<ContentItem> ContentItems { get; set; }

        #region ICategorizedItem
        public int CategoryID { get; set; }

        [JsonIgnore]
        [ForeignKey("CategoryID")]
        public virtual TagCategory TagCategory { get; set; }
        
        HCategory ICategorizedItem.Category
        {
            get { return this.TagCategory; }
        }
        #endregion
    }
}
