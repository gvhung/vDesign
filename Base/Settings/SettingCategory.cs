using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Base.Settings
{
    public class SettingCategory : HCategory
    {
        public string SysName { get; set; }
        
        [JsonIgnore]
        [ForeignKey("ParentID")]
        public virtual SettingCategory Parent_ { get; set; }
        [JsonIgnore]
        public virtual ICollection<SettingCategory> Children_ { get; set; }

        #region HCategory
        [NotMapped]
        public override HCategory Parent
        {
            get { return this.Parent_; }
        }
        [NotMapped]
        public override IEnumerable<HCategory> Children
        {
            get { return Children_ ?? new List<SettingCategory>(); }
        }
        #endregion
    }
}
