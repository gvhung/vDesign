using Base.BusinessProcesses.Entities;
using System.Collections.Generic;
using System.Linq;

namespace WebUI.Models.BusinessProcess
{
    public class WorkflowReportVm
    {
        public IEnumerable<CountOfObjectsReportVm> CountOfObjectsByStage { get; set; }
        public Workflow Workflow { get; set; }

        public WorkflowReportVm(Workflow wfl, IEnumerable<ChangeHistory> histories)
        {
            this.Workflow = wfl;

            this.CountOfObjectsByStage = histories.GroupBy(x => x.ObjectID).Select(x => x.OrderByDescending(h => h.SortOrder).FirstOrDefault()).GroupBy(x => x.Step).Select(x => new CountOfObjectsReportVm
            {
                StepTitle = x.Key.Title,
                CountOfObjects = x.Count(),
                Color = x.Key is Stage ?  (x.Key as Stage).Color : string.Empty,
            });
        } 
    }

    public class CountOfObjectsReportVm
    {
        public string StepTitle { get; set; }
        public int CountOfObjects { get; set; }
        public string Color { get; set; }
    }
}