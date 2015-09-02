using System.ComponentModel.DataAnnotations.Schema;

namespace Base.Entities.Complex
{
    [ComplexType]
    public class Color
    {
        public string Value { get; set; }

        public override string ToString()
        {
            return this.Value;
        }
    }
}
