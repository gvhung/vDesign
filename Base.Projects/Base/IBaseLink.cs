using Base.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Base.Projects.Base
{
    public interface IBaseLink : IBaseObject
    {
        long? Source { get; set; }
        long? Target { get; set; }
        string Type { get; set; }
        long ViewID { get; set; }
        int ProjectId { get; set; }
        IBaseProject BaseProject { get; set; }

        int? SourceId { get; set; }

        IProjectTask BaseSourceTask { get; set; }

        int? TargetId { get; set; }

        IProjectTask BaseTargetTask { get; set; }

        ObjectStatus SysStatus { get; set; }
    }
}
