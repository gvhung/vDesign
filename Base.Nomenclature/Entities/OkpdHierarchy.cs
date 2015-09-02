using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Base.Nomenclature.Entities
{
    [Table("Okpd")]
    public class OkpdHierarchy : HCategory
    {
        [MaxLength(20)]
        public string Code { get; set; }

        public string Title { get { return String.Format("[{0}] {1}", Code, Name); } }

        [JsonIgnore]
        [ForeignKey("ParentID")]
        public virtual OkpdHierarchy Parent_ { get; set; }
        [JsonIgnore]
        public virtual ICollection<OkpdHierarchy> Children_ { get; set; }

        #region HCategory
        [NotMapped]
        public override HCategory Parent
        {
            get { return this.Parent_; }
        }
        [NotMapped]
        public override IEnumerable<HCategory> Children
        {
            get { return Children_ ?? new List<OkpdHierarchy>(); }
        }
        #endregion
    }
}
