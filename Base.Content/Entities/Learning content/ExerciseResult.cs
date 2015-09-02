using Base.Attributes;
using System;

namespace Base.Content.Entities
{
    // Результат выполненного упражнения
    public class ExerciseResult : BaseObject
    {
        public int? ExerciseID { get; set; }

        [ListView]
        [DetailView(Name = "Упражнение")]
        public virtual Exercise Exercise { get; set; }

        [ListView]
        [DetailView(Name = "Количество баллов")]
        public int Points { get; set; }

        [ListView]
        [DetailView(Name = "Дата начала")]
        [PropertyDataType(PropertyDataType.DateTime)]
        public DateTime Start { get; set; }

        [ListView]
        [DetailView(Name = "Дата завершения")]
        [PropertyDataType(PropertyDataType.DateTime)]
        public DateTime? End { get; set; }

        [DetailView(TabName = "[1]Результат")]
        [PropertyDataType(PropertyDataType.Html)]
        public string Html { get; set; }
    }
}
