using Base.Attributes;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Base.Entities.Complex
{
    [ComplexType]
    public class Period
    {
        public Period()
        {
            Start = DateTime.MinValue;
        }

        [SystemProperty]
        public DateTime Start { get; set; }

        [SystemProperty]
        public DateTime? End { get; set; }

        [SystemProperty]
        public int Days
        {
            get
            {
                return End.HasValue ? (End.Value - Start).Days : 0;
            }
        }
    }
}
