using Base.Entities.Complex;
using Framework.Attributes;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;

namespace Base.EntityFrameworkTypes.Complex
{

    [Flags]
    public enum LayerType : uint
    {
        Point = 1 << 0,
        PolyLine = 1 << 1,
        Polygon = 1 << 2,
        All = Point | Polygon | PolyLine
    }

    [ComplexType]
    public class Location
    {
        public Location()
        {
            this.Address = new MultilanguageText();
        }
        [FullTextSearchProperty(2)]
        public MultilanguageText Address { get; set; }
        [Column(TypeName = "geography")]
        public DbGeography Disposition { get; set; }

        public override string ToString()
        {
            return String.Format("{0}; {1}", this.Address != null ? this.Address.ToString() : "", this.Disposition != null ? this.Disposition.AsText() : "");
        }
    }
}
