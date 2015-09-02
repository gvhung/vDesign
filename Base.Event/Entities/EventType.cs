using Base.Attributes;
using Base.Entities.Complex;
using Framework.Attributes;
using System.ComponentModel.DataAnnotations;

namespace Base.Event.Entities
{
    [EnableFullTextSearch]
    public class EventType: BaseObject
    {
        public EventType()
        {
            this.Color = new Color();
        }

        [ListView]
        [FullTextSearchProperty]
        [MaxLength(255)]
        [DetailView(Name = "Наименование", Required = true)]
        public string Title { get; set; }

        [ListView]
        [DetailView(Name = "Цвет")]
        public Color Color { get; set; }
    }
}
