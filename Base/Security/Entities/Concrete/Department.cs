using Base.Attributes;
using Framework.Attributes;
using System;
using System.ComponentModel.DataAnnotations;

namespace Base.Security
{
    [Serializable]
    [EnableFullTextSearch]
    public class Department : BaseObject
    {
        public int? ImageID { get; set; }
        [DetailView("Изображение", Width = 200, Height = 200)]
        [PropertyDataType("Image")]
        [ListView(Width = 80, Height = 80)]
        [SystemProperty]
        public virtual FileData Image { get; set; }

        [DetailView("Наименование")]
        [ListView]
        [MaxLength(255)]
        [FullTextSearchProperty]
        public string Title { get; set; }

        [MaxLength(100)]
        [FullTextSearchProperty]
        [DetailView("Адрес электронной почты"), ListView]
        public string Email { get; set; }
    }
}
