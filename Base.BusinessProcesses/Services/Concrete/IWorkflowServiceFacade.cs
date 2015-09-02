using Base.BusinessProcesses.Entities;
using Base.DAL;
using Base.Security;
using Base.Security.ObjectAccess;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Base.BusinessProcesses.Services.Concrete
{
    public interface  IWorkflowServiceFacade
    {
        ObjectAccessItem CreateAccessItem(IUnitOfWork uow, BaseObject obj);
        string Render(string template, BaseObject obj, IDictionary<string, string> additional = null);
        IWFObjectService GetService(string objectTypeStr, IUnitOfWork unitOfWork = null);
        void InitializeObject(ISecurityUser securityUser, BaseObject src, BaseObject dest, IEnumerable<InitItem> inits);
        void ModifyObject(ISecurityUser securityUser, BaseObject src, IEnumerable<InitItem> inits);
        Workflow CloneWorkflow(Workflow initWorkflow);
        void CreateChildAccessItem(IUnitOfWork unitOfWork, Workflow wf);
        void SaveExtendedStage(IUnitOfWork uow, ExtendedStage extendedStage, IObjectSaver<ExtendedStage> asObjectSaver);
        void OnEnterToExtendedStage(ISecurityUser securityUser, ExtendedStage extendedStage, BaseObject baseObject);
        void OnLeaveFromExtendedStage(ISecurityUser securityUser, ExtendedStage extendedStage, BaseObject baseObject);
        IQueryable<Workflow> GetWorkflowList(ISecurityUser securityUser, Type type, BaseObject model, IQueryable<Workflow> all);
        BaseObject CloneObject(BaseObject obj, Type objType);
    }
}