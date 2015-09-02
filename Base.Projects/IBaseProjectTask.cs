using Base.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Base.Projects
{
    public interface IBaseProjectTask : IBaseTask, ITreeNode
    {
        BaseTaskCategory BaseCategory { get; set; }
        IBaseCategorizedTask BaseTask { get; set; }
    }
}
