using Base.Entities.Complex;

namespace Base.LinkedObjects.Entities
{
    public class LinkBaseObjects : BaseObject
    {
        public LinkBaseObject Obj1 { get; set; }
        public LinkBaseObject Obj2 { get; set; }
        public bool System { get; set; }
    }
}
