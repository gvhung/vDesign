using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Base.Attributes;
using Framework.Maybe;

namespace Base.BusinessProcesses.Entities
{
    [Table("BranchingSteps")]
    public class BranchingStep : Step
    {
        public string ObjectType { get; set; }

        public override FlowStepType StepType { get { return FlowStepType.BranchingStep; } }

        [NotMapped]
        [DetailView(Name = "Действия", HideLabel = true, TabName = "[1]Действия")]
        public ICollection<Branch> Outputs
        {
            get { return this.BaseOutputs.With(x => x.OfType<Branch>()).With(x => x.ToList()); }
            set { this.BaseOutputs = value.With(x => x.OfType<Output>()).With(x => x.ToList()); }
        }

    }

    public class Branch : Output
    {
        [DetailView(Name = "Макрос", HideLabel = true, TabName = "[2]Макрос")]
        [PropertyDataType("LuaCode")]
        public string Macros { get; set; }

        public Branch()
        {
            this.Color = "#6f5499";
        }

        [DetailView(Name = "Построитель макросов", HideLabel = true, TabName = "[1]Конструктор действий")]
        [PropertyDataType("Macro")]
        public virtual ICollection<BranchConditionItem> BranchConditions { get; set; }
    }

    public class BranchConditionItem : BaseMacroItem
    {
        public int BrunchID { get; set; }
        public virtual Branch Brunch { get; set; }
    }
}