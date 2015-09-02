using Base;
using Base.Attributes;
using Framework.Attributes;
using System;
using System.ComponentModel.DataAnnotations;

namespace Data.Entities
{

    [Serializable]
    [EnableFullTextSearch]
    public class Okved : BaseObject
    {
        public int? ImageID { get; set; }
        [DetailView("Изображение", Width = 200, Height = 200)]
        [PropertyDataType("Image")]
        [SystemProperty]
        [ListView(Width = 80, Height = 80)]
        public virtual FileData Image { get; set; }

        [MaxLength(100)]
        [FullTextSearchProperty]
        [DetailView(Name = "Код", Required = true), ListView]
        public string Code { get; set; }

        [MaxLength(255)]
        [FullTextSearchProperty]
        [DetailView(Name = "Наименование", Required = true), ListView]
        public string Title { get; set; }        
    }
}
