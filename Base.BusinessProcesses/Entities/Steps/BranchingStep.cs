using Base.Attributes;
using Framework.Maybe;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

// ReSharper disable once CheckNamespace
namespace Base.BusinessProcesses.Entities
{
    public class Branch : Output
    {
        public Branch()
        {
            Color = "#6f5499";
        }

        public Branch(Branch src) : base(src)
        {
            Color = "#6f5499";

            if (src.BranchConditions.Any())
            {
                // ReSharper disable once DoNotCallOverridableMethodsInConstructor
                BranchConditions = new List<BranchConditionItem>();
                foreach (var srcBc in src.BranchConditions)
                {
                    // ReSharper disable once DoNotCallOverridableMethodsInConstructor
                    BranchConditions.Add(new BranchConditionItem(srcBc));
                }
            }
        }



        [DetailView(Name = "Построитель макросов", HideLabel = true, TabName = "[1]Конструктор действий")]
        [PropertyDataType("Macro")]
        public virtual ICollection<BranchConditionItem> BranchConditions { get; set; }
    }

    public class BranchConditionItem : BaseMacroItem
    {
        public BranchConditionItem()
        {
        }

        public BranchConditionItem(BranchConditionItem src)
            : base(src)
        {

        }

        public int BrunchID { get; set; }
        public virtual Branch Brunch { get; set; }
    }

    [Table("BranchingSteps")]
    public class BranchingStep : Step
    {
        public BranchingStep() { }

        public BranchingStep(BranchingStep src)
            : base(src)
        {
            ObjectType = src.ObjectType;
        }

        public void LoadOutputs(IEnumerable<Branch> srcOutputs)
        {
            Outputs = new List<Branch>();
            foreach (var src in srcOutputs.Where(x=>!x.Hidden))
            {
                BaseOutputs.Add(new Branch(src));
            }
        }

        public string ObjectType { get; set; }

        public override FlowStepType StepType { get { return FlowStepType.BranchingStep; } }

        [NotMapped]
        [DetailView(Name = "Действия", HideLabel = true, TabName = "[1]Действия")]
        public ICollection<Branch> Outputs
        {
            get { return BaseOutputs.With(x => x.OfType<Branch>()).With(x => x.ToList()); }
            set { BaseOutputs = value.With(x => x.OfType<Output>()).With(x => x.ToList()); }
        }
    }
}