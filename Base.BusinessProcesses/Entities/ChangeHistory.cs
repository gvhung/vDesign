using Base.Security;
using System;
using System.Collections.Generic;

namespace Base.BusinessProcesses.Entities
{
    public class ChangeHistory : BaseObject
    {
        public DateTime Date { get; set; }
        
        public int? UserID { get; set; }
        public virtual User User { get; set; }
        
        public string ObjectType { get; set; }
        public int ObjectID { get; set; }

        public int StepID { get; set; }
        public virtual Step Step { get; set; }

        public bool IsAgreed { get; set; }
        public bool AutoInvoked { get; set; }

        // Принятые решения   переделать один к одному
        public virtual ICollection<AgreementItem> AgreementItems { get; set; }
        // Задачи что создаются на этапах
        public virtual ICollection<BPTask> Tasks { get; set; }
        // Коллекции задач что создаются между этапами (Параллельные задачи)
        
        public virtual ICollection<RelatedTask> RelatedTasks { get; set; }      

        public int? CreatedObjectID { get; set; }
        public virtual CreatedObject CreatedObject { get; set; }
    }
}