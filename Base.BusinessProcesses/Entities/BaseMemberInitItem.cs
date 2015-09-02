using Base.Attributes;
using System.ComponentModel.DataAnnotations.Schema;

namespace Base.BusinessProcesses.Entities
{
    public enum MacroType
    {
        String = 0,
        Number,
        DateTime,
        TimeSpan,
        Operator,
        Boolean,
        InitObject,
        BaseObject,
        Function
    }

    public class InitItem : BaseObject
    {
        public InitItem()
        {
        }

        public InitItem(InitItem src)
        {
            Member = src.Member;
            Value = src.Value;
        }

        [SystemProperty]
        public string Member { get; set; }
        [SystemProperty]
        public string Value { get; set; }

    }

    public class CreateObjectStepMemberInitItem : InitItem
    {
        public CreateObjectStepMemberInitItem()
        {
        }

        public CreateObjectStepMemberInitItem(CreateObjectStepMemberInitItem src)
            : base(src)
        {

        }

        public int CreateObjectStepID { get; set; }
        public virtual CreateObjectStep CreateObjectStep { get; set; }        
    }

    [Table("StageActionInitItems")]
    public class StageActionInitItem : InitItem
    {
        public StageActionInitItem()
        {
        }

        public StageActionInitItem(StageActionInitItem src)
            :base(src)
        {

        }

        public int ActionID { get; set; }
        public virtual StageAction Action { get; set; }
    }

    [Table("StageInitItems")]
    public class StageInitItems : InitItem
    {
        public StageInitItems()
        {
        }

        public StageInitItems(StageInitItems src)
            : base(src)
        {
        }

        public int StageID { get; set; }
        public virtual Stage Stage { get; set; }
    }

    [Table("StageActionValidationItems")]
    public class StageActionValidationItem : BaseObject
    {
        public string Property { get; set; }
        public string ValidationRule { get; set; }
        public int StageActionID { get; set; }
        public virtual StageAction StageAction { get; set; }
    }
}