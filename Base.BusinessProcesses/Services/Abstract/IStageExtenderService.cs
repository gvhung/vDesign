using Base.BusinessProcesses.Entities;
using Base.DAL;
using Base.Security;
using Base.Service;

namespace Base.BusinessProcesses.Services.Abstract
{
    public interface IStageExtenderService
    {
        void OnStageEnter(ISecurityUser securityUser, ExtendedStage stage, StageExtender stageExtender, BaseObject obj);
        void OnStageLeave(ISecurityUser securityUser, ExtendedStage stage, StageExtender stageExtender, BaseObject obj);
        StageExtender CloneExtender(StageExtender stageExtender);
    }

    public interface IStageExtenderService<TStageExtender> : IBaseObjectService<TStageExtender> where TStageExtender : BaseObject
    {
        void ExternalSave(IUnitOfWork uow, IObjectSaver<TStageExtender> objectSaver);
        TStageExtender CloneExtender(TStageExtender stageExtender);
    }

    public interface IStageExtenderService<TStageExtender, in TEntity> : IStageExtenderService, IStageExtenderService<TStageExtender>
        where TStageExtender : BaseObject
    {
        void OnStageEnter(ISecurityUser securityUser, ExtendedStage stage, TStageExtender stageExtender, TEntity obj);
        void OnStageLeave(ISecurityUser securityUser, ExtendedStage stage, TStageExtender stageExtender, TEntity obj);
    }
}