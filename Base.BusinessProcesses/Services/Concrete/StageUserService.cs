using Base.BusinessProcesses.Entities;
using Base.BusinessProcesses.Services.Abstract;
using Base.BusinessProcesses.Strategies;
using Base.DAL;
using Base.Security;
using Base.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using Base.BusinessProcesses.Entities.Steps;

namespace Base.BusinessProcesses.Services.Concrete
{
    public class StageUserService : BaseObjectService<StageUser>, IStageUserService
    {
        private readonly IWorkflowStrategyService _strategyService;

        public StageUserService(
            IBaseObjectServiceFacade facade,
            IWorkflowStrategyService strategyService)
            : base(facade)
        {
            _strategyService = strategyService;
        }

        public List<User> GetStakeholders(IUnitOfWork unitOfWork, IBPObject obj)
        {
            return new List<User>();
        }

        public List<User> GetStakeholders(IUnitOfWork unitOfWork, Stage stage, BaseObject obj)
        {
            var users = GetPermittedUsers(unitOfWork, stage, x => x.AssignedToUsers.Select(z => z.Performer), x => x.AssignedToCategory, obj);
            return users != null ? users.ToList() : new List<User>();
        }

        public List<User> GetStakeholders(IUnitOfWork unitOfWork, TaskStep taskStep, BaseObject obj)
        {
            var users = GetPermittedUsers(unitOfWork, taskStep, x => x.AssignedToUsers.Select(z => z.Performer), x => x.AssignedToCategory, obj);
            return users != null ? users.ToList() : new List<User>();
        }

        private IEnumerable<User> GetPermittedUsers<TStep>(IUnitOfWork unitOfWork, TStep step, Func<TStep, IEnumerable<User>> usersSelector, Func<TStep, ICollection<StageUserCategory>> categorySelector, BaseObject obj) where TStep : Step
        {
            var users = usersSelector(step);
            var assignedToCategories = categorySelector(step);
            if (assignedToCategories != null && assignedToCategories.Any())
            {
                users = (from assignedToCategory in assignedToCategories
                         let strId = HCategory.IdToString(assignedToCategory.ObjectID.GetValueOrDefault())
                         let catId = assignedToCategory.ObjectID.GetValueOrDefault()
                         select unitOfWork.GetRepository<User>().All()
                         .Where(a => a.IsActive && !a.Hidden)
                         .Where(x => (x.UserCategory.sys_all_parents != null && x.UserCategory.sys_all_parents.Contains(strId)) || x.UserCategory.ID == catId))
                         .Aggregate(users, (current, range) => current.Union(FilterByStrategy(unitOfWork, range, step, obj)));
            }
            return users;
        }

        private IEnumerable<User> FilterByStrategy(IUnitOfWork unitOfWork, IEnumerable<User> users, Step step, BaseObject obj)
        {
            if (users != null)
            {
                var stage = step as Stage;
                if (stage != null && !string.IsNullOrEmpty(stage.StakeholdersSelectionStrategy))
                {
                    var strategy =
                        _strategyService.GetStrategyInstance<IStakeholdersSelectionStrategy>(
                            stage.StakeholdersSelectionStrategy);

                    if (strategy != null)
                        return strategy.FilterStackholders(unitOfWork, users, obj);
                }
            }
            return users;
        }
    }
}
