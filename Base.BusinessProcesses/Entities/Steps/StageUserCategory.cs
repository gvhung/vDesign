using System.ComponentModel.DataAnnotations.Schema;
using Base.Attributes;
using Base.Security;

namespace Base.BusinessProcesses.Entities.Steps
{
    [Table("StageUserCategory")]
    public class StageUserCategory : EasyCollectionEntry<UserCategory>
    {
        public int StageID { get; set; }
        public virtual Stage Stage { get; set; }
    }


}