using Base.BusinessProcesses.Entities;
using Base.BusinessProcesses.Events;
using Base.BusinessProcesses.Services.Concrete;
using Base.DAL;
using Base.Events;
using Base.Security;
using Base.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using Base.Validation;

namespace Base.BusinessProcesses.Services.Abstract
{
    public interface IWorkflowService : IBaseCategorizedItemService<Workflow>
    {
        void OnBPObjectCreate(object sender, BaseObjectEventArgs e);
        void OnBPObjectDeleted(object sender, BaseObjectEventArgs e);
        void OnBPObjectUpdate(object sender, BaseObjectEventArgs baseObjectEventArgs);

        ChangeHistory GetLastChangeHistory(IUnitOfWork unitOfWork, string objectType, int objectID);
        ChangeHistory GetChangeHistoryByID(IUnitOfWork unitOfWork, int changeHistoryID);
        IEnumerable<ChangeHistory> GetAllChangeHistory(IUnitOfWork unitOfWork, string objectType, int objectID);
        void TakeForPerform(IUnitOfWork unitOfWork, IWFObjectService objectService ,int? userID, int stageID, int objectID);
        void ReleasePerform(IUnitOfWork unitOfWork, IWFObjectService objectService, int stageID, int objectID);
        void InvokeStage(IUnitOfWork unitOfWork, IWFObjectService baseObjectService, int objectID , int actionID, string comment, int? userID);
        void AutoInvokeStage(IUnitOfWork unitOfWork,int objectID, StagePerform stage, string comment = null);
        IEnumerable<Stage> GetNextStage(IUnitOfWork unitOfWork, BaseObject baseObject, int actionID);
        IEnumerable<ChangeHistory> GetAllChangeHistory(IUnitOfWork unitOfWork, Workflow wfl);
        bool TestMacros(IUnitOfWork unitOfWork, IEnumerable<InitItem> items, Type type, Type parentType, out Exception exception);       
        IQueryable<Workflow> GetWorkflowList(IUnitOfWork unitOfWork, Type type, BaseObject model);
        void ExecuteNextStage(IUnitOfWork unitOfWork, BaseObject baseObject, StageAction action, int? assignToUserID,ref int counter);
        event EventHandler<WorkflowTaskEventArgs> WorkflowTransactionCompleted;
        IValidationContext GetValidationContext(StageAction action);
    }
}
