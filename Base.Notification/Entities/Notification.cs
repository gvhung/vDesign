using Base.Attributes;
using Base.Entities.Complex;
using Base.Security;
using Framework.Attributes;
using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;

namespace Base.Notification.Entities
{
    [EnableFullTextSearch]
    public class Notification : BaseObject
    {
        public string sys_name { get; set; }
        public NotificationStatus Status { get; set; }

        [ListView]
        [DetailView(Name = "Дата", Required = true)]
        [PropertyDataType(PropertyDataType.DateTime)]
        public DateTime Date { get; set; }

        public int? UserID { get; set; }
        public string UserEmail { get; set; }
        [JsonIgnore]
        public User User { get; set; }

        [DetailView(Name = "Cущность")]
        [ListView]
        public LinkBaseObject Entity { get; set; }

        [FullTextSearchProperty]
        [ListView]
        [DetailView(Name = "Наименование", Required = true)]
        [MaxLength(255)]
        public string Title { get; set; }

        [FullTextSearchProperty]
        [ListView]
        [DetailView(Name = "Описание")]
        public string Description { get; set; }
    }
}
