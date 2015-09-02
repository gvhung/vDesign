using Base.Attributes;
using Base.Security;

namespace Base.BusinessProcesses.Entities
{
    public abstract class BaseStepUser : BaseObject
    {
        public int PerformerID { get; set; }

        [DetailView("�����������")]
        [ListView]
        public virtual User Performer { get; set; }
    }
}