using Base.Security.ObjectAccess;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Base.Project.Entities
{
    public class ProjectCategory : HCategory, IAccessibleObject
    {
        [ForeignKey("ParentID")]
        [JsonIgnore]
        public virtual ProjectCategory Parent_ { get; set; }
        [JsonIgnore]
        public virtual ICollection<ProjectCategory> Children_ { get; set; }


        #region HCategory
        public override HCategory Parent
        {
            get { return this.Parent_; }
        }
        public override IEnumerable<HCategory> Children
        {
            get { return Children_ ?? new List<ProjectCategory>(); }
        }
        #endregion
    }
}
