using Base;
using Base.Entities.Complex;
using Base.Security;
using Base.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Base.Projects.Base
{
    public interface IBaseProject : IBaseObject
    {
        string Title { get; set; }
        string Description { get; set; }
        Period Period { get; set; }

        int? CreatedByID { get; set; }
        User CreatedBy { get; set; }

        int? ChargeUserID { get; set; }
        User ChargeUser { get; set; }

        int GeneralTaskID { get; set; }

        IProjectTask BaseGeneralTask { get; set; }

        int DefaultCategoryID { get; set; }

        BaseTaskCategory BaseDefaultCategory { get; set; }

        IEnumerable<IBaseLink> BaseLinks { get; set; }

        IEnumerable<BaseProjectFile> BaseFiles { get; set; }

        double PercentComplete { get; set; }
    }
}
