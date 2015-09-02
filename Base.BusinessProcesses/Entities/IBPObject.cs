using Base.Attributes;
using Microsoft.Linq.Translations;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace Base.BusinessProcesses.Entities
{
    // ReSharper disable once InconsistentNaming
    public interface IBPObject : IBaseObject
    {
        int? WorkflowContextID { get; set; }
        WorkflowContext WorkflowContext { get; set; }
        [NotMapped]
        Workflow InitWorkflow { get; set; }    
    }

    public class RoadMap : BaseObject
    {
        // ReSharper disable once InconsistentNaming
        private static readonly CompiledExpression<RoadMap, ChangeHistory> _lastChangeHistory =
            DefaultTranslationOf<RoadMap>.Property(x => x.LastChangeHistory).Is(x => x.History != null ? x.History.OrderByDescending(z => z.Date).FirstOrDefault() : null);

        // ReSharper disable once InconsistentNaming
        private static readonly CompiledExpression<RoadMap, Stage> _currentStage =
            DefaultTranslationOf<RoadMap>.Property(x => x.CurrentStage).Is(x => x.LastChangeHistory != null ? x.LastChangeHistory.Step as Stage : null);

        public ChangeHistory LastChangeHistory
        {
            get { return _lastChangeHistory.Evaluate(this); }
        }

        public Stage CurrentStage
        {
            get { return _currentStage.Evaluate(this); }
        }

        public int ObjectID { get; set; }
        public string ObjectType { get; set; }

        public int? WorkflowID { get; set; }
        [DetailView("Схема")]
        public virtual Workflow Workflow { get; set; }

        public virtual ICollection<ChangeHistory> History { get; set; }
    }
}
