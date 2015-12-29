using Base;
using Base.BusinessProcesses.Entities;
using Base.DAL;
using Base.Service;
using Data.Entities;
using Data.Service.Abstract;

namespace Data.Service.Concrete
{
    public class ContactRequestService : BaseObjectService<ContactRequest>, IContactRequestService
    {
        public ContactRequestService(IBaseObjectServiceFacade facade) : base(facade)
        {
        }

        protected override IObjectSaver<ContactRequest> GetForSave(IUnitOfWork unitOfWork, IObjectSaver<ContactRequest> objectSaver)
        {
            return base.GetForSave(unitOfWork, objectSaver);
        }

        public void BeforeInvoke(BaseObject obj)
        {
            
        }

        public void OnActionExecuting(ActionExecuteArgs args)
        {
            
        }

        public int GetWorkflowID(IUnitOfWork unitOfWork, BaseObject obj)
        {
            return Workflow.Default;
        }
    }
}