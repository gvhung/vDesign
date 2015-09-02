using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Base.Entities.Complex
{
    [ComplexType]
    public class Url
    {
        [MaxLength(255)]
        public string Text { get; set; }
        [MaxLength(255)]
        public string Path { get; set; }
    }
}
