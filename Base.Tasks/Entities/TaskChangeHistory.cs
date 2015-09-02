using Base.Attributes;
using Base.Security;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Base.Task.Entities
{
    [Table("TaskChangeHistory")]
    public class TaskChangeHistory: BaseObject
    {
        public int TaskID { get; set; }
        public Task Task { get; set; }

        [ListView]
        [DetailView(Name = "Дата")]
        [PropertyDataType(PropertyDataType.DateTime)]
        public DateTime Date { get; set; }

        [ListView]
        [DetailView(Name = "Статус")]
        public TaskStatus Status { get; set; }

        public int? UserID { get; set; }
        [ListView]
        [DetailView(Name = "Пользователь")]
        public virtual User User { get; set; }

        [ListView]
        [DetailView(Name = "Комментарий")]
        [PropertyDataType(PropertyDataType.SimpleHtml)]
        public string Сomment { get; set; }
    }
}
