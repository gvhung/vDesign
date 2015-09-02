using Base.Attributes;
using Base.Security.ObjectAccess;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Base.Content.Entities
{
    // Упражнение - состоит в сущности CourseCategory
    public class Exercise : BaseObject, ICategorizedItem, IAccessibleObject
    {
        public Exercise()
        {
            Content = new Content();
        }

        [ListView]
        [DetailView(Name = "Наименование", Required = true, Order = 0)]
        [MaxLength(255)]
        public string Title { get; set; }

        [ListView]
        [DetailView(Name = "Описание", Order = 1)]
        public string Description { get; set; }

        [DetailView(TabName = "[1]Контент")]
        public Content Content { get; set; }

        #region ICategorizedItem
        public int CategoryID { get; set; }

        // это НЕ курс - это УРОК (упражнение не может напрямую ссылаться на курс)
        [JsonIgnore]
        [ForeignKey("CategoryID")]
        public virtual CourseCategory CourseCategory { get; set; }

        HCategory ICategorizedItem.Category
        {
            get { return this.CourseCategory; }
        }
        #endregion
    }
}
