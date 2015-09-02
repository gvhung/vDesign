using Base.Attributes;
using System;
using System.Collections.Generic;

namespace Base.Audit.Entities
{
    [Serializable]
    public class Config: BaseObject
    {
        [DetailView("Максимальное кол-во записей в журнале")]
        public int MaxRecordCount { get; set; }

        [DetailView("Регистрировать вход/выход в систему")]
        public bool RegisterLogIn { get; set; }

        [DetailView(TabName = "Сущности")]
        [PropertyDataType("AuditListEntities")]
        public ICollection<ConfigEntities> Entities { get; set; }
    }
    
    [Serializable]
    public class ConfigEntities: BaseObject
    {
        [DetailView(Name = "Объект")]
        [ListView]
        public string FullName { get; set; }
    }
}
