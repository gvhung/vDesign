using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Base.Content.Entities
{
    // Категория для Tag
    public class TagCategory : HCategory
    {
        [ForeignKey("ParentID")]
        [JsonIgnore]
        public virtual TagCategory Parent_ { get; set; }

        [JsonIgnore]
        public virtual ICollection<TagCategory> Children_ { get; set; }

        #region HCategory
        public override HCategory Parent
        {
            get { return this.Parent_; }
        }

        public override IEnumerable<HCategory> Children
        {
            get { return Children_ ?? new List<TagCategory>(); }
        }
        #endregion
    }
}
