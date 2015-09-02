using Base.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Base.DAL;

namespace Base.Project.Service
{
    public interface IProjectService : IBaseCategorizedItemService<Entities.Project>
    {
        Entities.Project CreateProjectFromTask(IUnitOfWork unitOfWork, int projectTaskId);
    }
}
