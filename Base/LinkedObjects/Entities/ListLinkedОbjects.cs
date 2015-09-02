using Base.Attributes;
using Base.Entities.Complex;
using System.Collections.Generic;

namespace Base.LinkedObjects.Entities
{
    //NotMapped
    public class ListLinkedОbjects : BaseObject
    {
        public LinkBaseObject Obj { get; set; }

        [DetailView("Связи", HideLabel = true)]
        public ICollection<Link> Links { get; set; }
    }

    //NotMapped
    public class Link : BaseObject
    {
        [DetailView("Объект")]
        [ListView]
        public LinkBaseObject Obj { get; set; }

        [DetailView("Описание")]
        [ListView]
        public string Description { get; set; }
    }
}
