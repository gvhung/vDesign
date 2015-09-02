using Base.Security;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Base.BusinessProcesses.Entities
{
    [Table("AgreementItems")]
    public class AgreementItem : BaseObject
    {
        public DateTime? Date { get; set; }
        public string Comment { get; set; }
        
        public int ChangeHistoryID { get; set; }
        public virtual ChangeHistory ChangeHistory { get; set; }

        public int UserID { get; set; }
        public virtual User User { get; set; }

        public int? FromUserID { get; set; }
        public virtual User FromUser { get; set; }

        public int? ActionID { get; set; }
        public virtual StageAction Action { get; set; }
    }
}
