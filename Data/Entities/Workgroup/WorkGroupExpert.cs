using Base;
using Base.Attributes;

namespace Data.Entities.Workgroup
{
    public class WorkGroupExpert : BaseObject
    {
        public int WorkGroupID { get; set; }
        public virtual WorkGroup WorkGroup { get; set; }

        public int ExpertID { get; set; }

        [ListView]
        [DetailView(Name = "Эксперт")]
        public virtual Expert Expert { get; set; }

        public int ExpertStatusInWorkgorupID { get; set; }

        [ListView]
        [DetailView(Name = "Статус эксперта")]
        public virtual ExpertStatusInWorkGroup ExpertStatusInWorkGroup { get; set; }
    }
}
