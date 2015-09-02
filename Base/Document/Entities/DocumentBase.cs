using Base.Attributes;
using Framework.Attributes;
using System.ComponentModel.DataAnnotations;

namespace Base.Document.Entities
{
    public abstract class DocumentBase: BaseObject
    {
        [ListView]
        [DetailView("Тип", Visible = false)]
        public DocumentType? Type { get; set; }

        [ListView]
        [MaxLength(255)]
        [DetailView("Наименование", Required = true)]
        [FullTextSearchProperty]
        public string Title { get; set; }

        [FullTextSearchProperty]
        [DetailView(Name = "Описание")]
        [FullTextSearchProperty]
        public string Description { get; set; }

        [DetailView(TabName = "[1]Редактор")]
        [PropertyDataType("Document")]
        [SystemProperty]
        public string Value { get; set; }
    }
}
