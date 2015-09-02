using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Base.Entities.Complex.KLADR
{
    [ComplexType]
    public class AddressObject
    {
        [MaxLength(50)]
        public string ID { get; set; }
        [MaxLength(255)]
        public string Name { get; set; }
        [MaxLength(50)]
        public string TypeShort { get; set; }
        [MaxLength(50)]
        public string ContentType { get; set; }
    }
}