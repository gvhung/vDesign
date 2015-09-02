using Base.DAL;
using Base.Project.Entities;
using Base.Security.Service.Abstract;
using Base.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Base.Project.Service
{
    public class ProjectTaskDependencyService : BaseObjectService<ProjectTaskDependency>, IProjectTaskDependencyService
    {
        public ProjectTaskDependencyService(IBaseObjectServiceFacade facade) : base(facade)
        {
        }
    }
}
