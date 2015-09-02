using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Base.Project.Entities
{
    public class ProjectTaskDependency: BaseObject
    {
        public int ProjectId { get; set; }
        public int PredecessorId { get; set; }
        public int SuccessorId { get; set; }
        public int Type { get; set; }
    }
}
