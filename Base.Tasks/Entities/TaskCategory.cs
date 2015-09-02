using Base.Attributes;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Base.Task.Entities
{
    using Base.UI;

    public class TaskCategory : HCategory, ITreeNodeIcon
    {
        [ForeignKey("ParentID")]
        [JsonIgnore]
        public virtual TaskCategory Parent_ { get; set; }
        [JsonIgnore]
        public virtual ICollection<TaskCategory> Children_ { get; set; }

        [SystemProperty]
        public string SysName { get; set; }

        #region HCategory
        public override HCategory Parent
        {
            get { return this.Parent_; }
        }
        public override IEnumerable<HCategory> Children
        {
            get { return Children_ ?? new List<TaskCategory>(); }
        }

        #endregion

        [DetailView(Name = "Иконка")]
        public Icon Icon { get; set; }

        public TaskCategory()
        {
            this.Icon = new Icon();
        }
    }
}
