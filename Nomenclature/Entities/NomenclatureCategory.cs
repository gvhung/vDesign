using Base;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nomenclature.Entities
{
    public class NomenclatureCategory : HCategory
    {
        public string sys_name { get; set; }

        [JsonIgnore]
        [ForeignKey("ParentID")]
        public virtual NomenclatureCategory Parent_ { get; set; }
        [JsonIgnore]
        public virtual ICollection<NomenclatureCategory> Children_ { get; set; }

        #region HCategory
        [NotMapped]
        public override HCategory Parent
        {
            get { return this.Parent_; }
        }
        [NotMapped]
        public override IEnumerable<HCategory> Children
        {
            get { return Children_ ?? new List<NomenclatureCategory>(); }
        }
        #endregion
    }
}
