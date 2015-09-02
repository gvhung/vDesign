using Base.BusinessProcesses.Entities;
using Base.BusinessProcesses.Services.Abstract;
using Base.DAL;
using Base.Security;
using Base.Service;
using System;

namespace Base.BusinessProcesses.Services.Concrete
{
    public abstract class StageExtenderService<TStageExtender, TEntity> : BaseObjectService<TStageExtender>, IStageExtenderService<TStageExtender, TEntity> 
        where TStageExtender : StageExtender
        where TEntity : BaseObject
    {
        protected StageExtenderService(IBaseObjectServiceFacade facade)
            : base(facade)
        {
        }

        public void ExternalSave(IUnitOfWork uow, IObjectSaver<TStageExtender> objectSaver)
        {
            GetForSave(uow, objectSaver);
        }

        public abstract TStageExtender CloneExtender(TStageExtender stageExtender);

        public virtual void OnStageEnter(ISecurityUser securityUser, ExtendedStage stage, TStageExtender stageExtender, TEntity obj)
        {
        }

        public virtual void OnStageLeave(ISecurityUser securityUser, ExtendedStage stage, TStageExtender stageExtender, TEntity obj)
        {
        }

        void IStageExtenderService.OnStageEnter(ISecurityUser securityUser, ExtendedStage stage, StageExtender stageExtender, BaseObject obj)
        {
            OnStageEnter(securityUser, stage, stageExtender as TStageExtender, obj as TEntity);
        }

        void IStageExtenderService.OnStageLeave(ISecurityUser securityUser, ExtendedStage stage, StageExtender stageExtender, BaseObject obj)
        {
            OnStageLeave(securityUser, stage, stageExtender as TStageExtender, obj as TEntity);
        }

        StageExtender IStageExtenderService.CloneExtender(StageExtender stageExtender)
        {
            var extender = stageExtender as TStageExtender;
            if (extender != null)
                return CloneExtender(extender);

            throw new Exception("type of stage extender incorrect");
        }
    }
}