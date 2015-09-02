using Base.Entities;
using Base.Projects.Base;
using Base.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Base.Projects
{
    public interface IProjectTask : ITask, ITreeNode
    {
        bool AutoStart { get; set; }

        int TaskID { get; set; }

        ITask BaseTask { get; set; }

        Priority Priority { get; set; }

        bool IsGroup { get; set; }

        long ViewID { get; set; }

        long? ParentViewID { get; set; }

        IEnumerable<IBaseProject> BaseProjects { get; set; }

        ObjectStatus SysStatus { get; set; }
    }
}
