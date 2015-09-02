using Base.Security.ObjectAccess;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Base.Document.Entities
{
    public class DocumentTemplateCategory : HCategory, IAccessibleObject
    {
        [JsonIgnore]
        [ForeignKey("ParentID")]
        public virtual DocumentTemplateCategory Parent_ { get; set; }
        [JsonIgnore]
        public virtual ICollection<DocumentTemplateCategory> Children_ { get; set; }

        #region HCategory
        [NotMapped]
        public override HCategory Parent
        {
            get { return this.Parent_; }
        }
        [NotMapped]
        public override IEnumerable<HCategory> Children
        {
            get { return Children_ ?? new List<DocumentTemplateCategory>(); }
        }
        #endregion
    }
}
