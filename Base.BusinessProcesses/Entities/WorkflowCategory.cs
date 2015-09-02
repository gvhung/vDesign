using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Base.BusinessProcesses.Entities
{
    public class WorkflowCategory : HCategory
    {

        [JsonIgnore]
        [ForeignKey("ParentID")]
        // ReSharper disable once InconsistentNaming
        public virtual WorkflowCategory Parent_ { get; set; }
        [JsonIgnore]
        // ReSharper disable once InconsistentNaming
        public virtual ICollection<WorkflowCategory> Children_ { get; set; }

        #region HCategory
        [NotMapped]
        public override HCategory Parent
        {
            get { return Parent_; }
        }
        [NotMapped]
        public override IEnumerable<HCategory> Children
        {
            get { return Children_ ?? new List<WorkflowCategory>(); }
        }
        #endregion
    }
}
