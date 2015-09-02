using Base;
using Base.Attributes;
using System;

namespace Data.Entities
{
    [Serializable]
    public class RequestLog: BaseObject
    {
        [DetailView("Запрос"), ListView]
        public string Request { get; set; }

        [DetailView("Пользователь"), ListView]
        public string User { get; set; }

        [PropertyDataType(PropertyDataType.DateTime)]
        [DetailView("Начало"), ListView]
        public DateTime Start { get; set; }

        [DetailView("Конец"), ListView(Visible = false)]
        [PropertyDataType(PropertyDataType.DateTime)]
        public DateTime? End { get; set; }

        [DetailView("Длительность, мсек"), ListView]
        public int? Duration 
        {
            get
            {
                return End == null
                    ? (int?) null
                    : (End.Value - Start).Milliseconds;
            }
        }
    }
}
