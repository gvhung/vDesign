using Base.DAL;
using Base.Project.Entities;
using Base.Security.Service.Abstract;
using Base.Service;
using Base.Task.Entities;
using Base.Task.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Base.Project.Service
{
    public class ProjectCategoryService : BaseCategoryService<ProjectCategory>, IProjectCategoryService
    {
        public ProjectCategoryService(IBaseObjectServiceFacade facade) : base(facade)
        {
        }


    }
}
