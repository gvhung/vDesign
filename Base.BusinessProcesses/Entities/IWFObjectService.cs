using Base.DAL;
using Base.Service;
using System;

namespace Base.BusinessProcesses.Entities
{
    public interface IWFObjectService : IBaseObjectCRUDService
    {
        [Obsolete]
        void BeforeInvoke(BaseObject obj);
        void OnActionExecuting(ActionExecuteArgs args);
        int GetWorkflowID(IUnitOfWork unitOfWork, BaseObject obj);
    }
}