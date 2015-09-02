using Base;
using Kendo.Mvc.UI;

namespace WebUI.Models
{
    public class GanttDependencyViewModel : BaseObject, IGanttDependency
    {
        public int ProjectID { get; set; }
        public int PredecessorID { get; set; }
        public int SuccessorID { get; set; }
        public DependencyType Type { get; set; }
    }
}