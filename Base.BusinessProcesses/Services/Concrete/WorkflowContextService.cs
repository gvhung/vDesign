using System;
using System.Collections.Generic;
using System.Linq;
using Base.BusinessProcesses.Entities;
using Base.BusinessProcesses.Services.Abstract;
using Base.DAL;
using Base.Security;
using Base.Service;
using Base.Ambient;

namespace Base.BusinessProcesses.Services.Concrete
{
    public class WorkflowContextService : BaseObjectService<WorkflowContext>, IWorkflowContextService
    {
        private readonly IStageUserService _stageUserService;
        public WorkflowContextService(IBaseObjectServiceFacade facade, IStageUserService userService)
            : base(facade)
        {
            _stageUserService = userService;
        }

        public List<StagePerform> GetCurrentStages(IUnitOfWork uinitOfWork, IBPObject obj)
        {
            int ctxID = obj.WorkflowContextID.GetValueOrDefault();
            var context = uinitOfWork.GetRepository<WorkflowContext>().All().FirstOrDefault(x => x.ID == ctxID);
            if (context == null)
                throw new Exception("Не удалось найти контекст исполнения");

            var currentStages = context.CurrentStages.ToList();

            if (!AppContext.SecurityUser.IsAdmin)
            {
                var outputs = currentStages.SelectMany(x => x.Stage.Outputs);
                foreach (var stageAction in outputs)
                {
                    if (!CurrentUserInActionRoles(stageAction))
                        stageAction.Hidden = true;
                }
            }
            return currentStages;
        }

        private bool CurrentUserInActionRoles(StageAction action)
        {
            var rolesID = action.Roles.Select(x => x.ObjectID).ToList();
            if (rolesID.Any())
            {
                return rolesID.Any(roleid => AppContext.SecurityUser.IsRole(roleid.GetValueOrDefault()));
            }
            return true;
        }

        public List<User> GetPermittedUsers(IUnitOfWork uow, Stage stage, BaseObject obj)
        {
            var users = _stageUserService.GetStakeholders(uow, stage, obj);
            return users;
        }

        public PerformerType GetPerformerType(IUnitOfWork unitOfWork, StagePerform perform, BaseObject obj)
        {
            var performer = PerformerType.Denied;
            var stage = perform.Stage;
            if (AppContext.SecurityUser.IsAdmin || AppContext.SecurityUser.IsSysRole(SystemRole.AdminWF))
            {
                performer = PerformerType.Admin;
            }
            else if (stage.Workflow.CuratorID == AppContext.SecurityUser.ID)
            {
                performer = PerformerType.Curator;
            }
            else if (perform.PerformUserID == AppContext.SecurityUser.ID)
            {
                performer = PerformerType.Performer;
            }
            else if (GetPermittedUsers(unitOfWork, stage, obj).Select(x => x.ID).Any(x => x == AppContext.SecurityUser.ID))
            {
                performer = PerformerType.Regular;
            }
            return performer;
        }
    }
}
