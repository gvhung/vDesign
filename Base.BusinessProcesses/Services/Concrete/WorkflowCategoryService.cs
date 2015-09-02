using Base.BusinessProcesses.Entities;
using Base.BusinessProcesses.Services.Abstract;
using Base.Service;

namespace Base.BusinessProcesses.Services.Concrete
{
    public class WorkflowCategoryService : BaseCategoryService<WorkflowCategory>, IWorkflowCategoryService
    {
        public WorkflowCategoryService(IBaseObjectServiceFacade facade)
            : base(facade)
        {
        }
    }
}