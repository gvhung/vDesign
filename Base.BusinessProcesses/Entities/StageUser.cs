using System.ComponentModel.DataAnnotations.Schema;

namespace Base.BusinessProcesses.Entities
{
    [Table("StageUsers")]
    public class StageUser : BaseStepUser
    {
        public int StageID { get; set; }
        public virtual Stage Stage { get; set; }
    }
}