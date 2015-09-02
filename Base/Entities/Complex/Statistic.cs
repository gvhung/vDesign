using System.ComponentModel.DataAnnotations.Schema;

namespace Base.Entities.Complex
{
    [ComplexType]
    public class Statistic
    {
        public int Views { get; set; }
        public double Rating { get; set; }
        public int RatingUp { get; set; }
        public int RatingDown { get; set; }
        public int Comments { get; set; }
    }
}
