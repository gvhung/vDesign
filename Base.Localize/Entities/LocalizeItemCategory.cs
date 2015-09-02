using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Base.Localize.Entities
{
    public class LocalizeItemCategory : HCategory
    {


        #region HCategory
        [ForeignKey("ParentID")]
        [JsonIgnore]
        public virtual LocalizeItemCategory Parent_ { get; set; }
        [JsonIgnore]
        public virtual ICollection<LocalizeItemCategory> Children_ { get; set; }

        public override HCategory Parent
        {
            get { return this.Parent_; }
        }
        public override IEnumerable<HCategory> Children
        {
            get { return Children_ ?? new List<LocalizeItemCategory>(); }
        }
        #endregion
    }
}
