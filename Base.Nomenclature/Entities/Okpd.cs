using Base.Attributes;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Base.Nomenclature.Entities
{
    [ComplexType]
    public class Okpd
    {
        [DetailView(Name = "Код")]
        [MaxLength(20)]
        public string Value { get; set; }

        [DetailView(Name = "Название")]
        [MaxLength(200)]
        [PropertyDataType(PropertyDataType.MultilineText)]
        public string Name { get; set; }

        public string Title { get { return String.Format("[{0}] {1}", Value, Name); } }
    }
}
