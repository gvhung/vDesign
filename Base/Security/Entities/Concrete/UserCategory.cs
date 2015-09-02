using Base.Attributes;
using Base.Security.ObjectAccess;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Base.Security
{
    public class UserCategory : HCategory, ITreeNodeImage, IAccessibleObject
    {
        [DetailView("Орган гос. власти", Order = 2)]
        public bool Company { get; set; }

        public Guid? CompanyGuid { get; set; }

        [JsonIgnore]
        [ForeignKey("ParentID")]
        public virtual UserCategory Parent_ { get; set; }
        [JsonIgnore]
        public virtual ICollection<UserCategory> Children_ { get; set; }

        #region HCategory
        [NotMapped]
        public override HCategory Parent
        {
            get { return this.Parent_; }
        }
        [NotMapped]
        public override IEnumerable<HCategory> Children
        {
            get { return Children_ ?? new List<UserCategory>(); }
        }
        #endregion

        public int? ImageID { get; set; }

        [PropertyDataType("Image")]
        [DetailView("Изображение", Width = 24, Height = 24)]
        public virtual FileData Image { get; set; }

        public string SystemName { get; set; }

    }
}
