using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Base.Attributes;
using Newtonsoft.Json;

namespace Base.BusinessProcesses.Entities
{
    [Table("WorkflowContexts")]
    public class WorkflowContext : BaseObject
    {
        [DetailView(Name = "ИД бизнес процесса",Visible = false)]
        public int WorkflowID { get; set; }
        [JsonIgnore]
        public virtual Workflow Workflow { get; set; }
        [JsonIgnore]
        public virtual ICollection<StagePerform> CurrentStages { get; set; }        
    }
}
