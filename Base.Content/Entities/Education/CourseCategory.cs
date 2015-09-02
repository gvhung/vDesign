using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using Base.UI;
using Base.Security.ObjectAccess;
using Base.Content.Service;
using Base.Attributes;

namespace Base.Content.Entities
{
    // Курс или урок/занятие (в зависимости от вложенности в дереве)
    // root/parent - курс, !root/children - урок/занятие
    // для отображения учебных заданий
    public class CourseCategory : HCategory, IAccessibleObject, ITreeNodeImage
    {
        public int? ImageID { get; set; }

        [ListView(Width = 24, Height = 24)]
        [DetailView(Name = "Изображение", Order = 0)]
        [PropertyDataType("Image")]
        public virtual FileData Image { get; set; }

        [ListView]
        public string Title
        {
            get
            {
                return this.IsRoot ? "Курс: " + this.Name : "Урок: " + this.Name;
            }
        }

        [ListView]
        [DetailView(Name = "Описание", Order = 2)]
        public string Description { get; set; }

        [JsonIgnore]
        [ForeignKey("ParentID")]
        public virtual CourseCategory Parent_ { get; set; }

        [JsonIgnore]
        public virtual ICollection<CourseCategory> Children_ { get; set; }

        #region HCategory
        public override HCategory Parent
        {
            get { return this.Parent_; }
        }

        public override IEnumerable<HCategory> Children
        {
            get { return this.Children_ ?? new List<CourseCategory>(); }
        }
        #endregion
    }
}
