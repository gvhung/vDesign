using Base.Attributes;
using Base.Entities.Complex;
using Base.Security;
using Framework.Attributes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Base.Audit.Entities
{
    [EnableFullTextSearch]
    public class AuditItem: BaseObject
    {
        public AuditItem()
        {
            this.Entity = new LinkBaseObject();
        }

        [JsonIgnore]
        public string JsonObj { get; set; }

        [DetailView(Name = "Дата")]
        [ListView]
        [PropertyDataType(PropertyDataType.DateTime)]
        public DateTime Date { get; set; }

        [DetailView(Name = "Тип")]
        [ListView]
        public TypeAuditItem Type { get; set; }

        [DetailView(Name = "Cущность")]
        [ListView]
        public LinkBaseObject Entity { get; set; }
        
        public int? UserID { get; set; }

        [DetailView("Пользователь")]
        [FullTextSearchProperty]
        [ListView]
        public virtual User User { get; set; }

        [DetailView(Name = "Описание")]
        [FullTextSearchProperty]
        public string Description { get; set; }

        [DetailView(TabName = "Изменения")]
        public virtual ICollection<DiffItem> Diff { get; set; }
    }

    public class DiffItem: BaseObject
    {
        [DetailView(Name = "Свойство")]
        [MaxLength(255)]
        [ListView]
        public string Property { get; set; }

        [DetailView(Name = "Old")]
        [ListView]
        public string OldValue { get; set; }

        [DetailView(Name = "New")]
        [ListView]
        public string NewValue { get; set; }
    }

    public enum TypeAuditItem
    {
        [Description("Отказ входа в систему")]
        LogOnError = 0,
        [Description("Успешный вход в систему")]
        LogOn = 1,
        [Description("Выход из системы")]
        LogOf = 2,
        [Description("Создание объекта")]
        CreateObject = 3,
        [Description("Редактирование объекта")]
        UpdateObject = 4,
        [Description("Удаление объекта")]
        DeleteObject = 5
    }
}
