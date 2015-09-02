using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Base.Registers.Entities
{
    public class MeasureCategory : HCategory
    {
        #region HCategory
        [ForeignKey("ParentID")]
        [JsonIgnore]
        public virtual MeasureCategory Parent_ { get; set; }
        [JsonIgnore]
        public virtual ICollection<MeasureCategory> Children_ { get; set; }

        public override HCategory Parent
        {
            get { return this.Parent_; }
        }
        public override IEnumerable<HCategory> Children
        {
            get { return Children_ ?? new List<MeasureCategory>(); }
        }
        #endregion
    }
}
