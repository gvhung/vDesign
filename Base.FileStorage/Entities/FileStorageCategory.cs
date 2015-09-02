using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Base.Security.ObjectAccess;
using Base.Security.ObjectAccess.Policies;

namespace Base.FileStorage
{
    [AccessPolicy(typeof(CreatorOnly))]
    public class FileStorageCategory : HCategory, IAccessibleObject
    {
        [JsonIgnore]
        [ForeignKey("ParentID")]
        public virtual FileStorageCategory Parent_ { get; set; }
        [JsonIgnore]
        public virtual ICollection<FileStorageCategory> Children_ { get; set; }

        #region HCategory
        [NotMapped]
        public override HCategory Parent
        {
            get { return this.Parent_; }
        }
        [NotMapped]
        public override IEnumerable<HCategory> Children
        {
            get { return Children_ ?? new List<FileStorageCategory>(); }
        }
        #endregion
    }
}
