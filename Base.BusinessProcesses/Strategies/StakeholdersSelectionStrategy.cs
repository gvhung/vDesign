using Base.BusinessProcesses.Attributes;
using Base.DAL;
using Base.Security;
using Base.Security.Service;
using Base.Security.Service.Abstract;
using System.Collections.Generic;
using System.Linq;

namespace Base.BusinessProcesses.Strategies
{
    [WorkflowStrategy("WritersOnlyStakeholdersSelectionStrategy", "Только пользователи имеющие права на запись")]
    public class StakeholdersSelectionStrategy : IStakeholdersSelectionStrategy
    {
        private readonly ISecurityService _securityService;
        private readonly ISecurityUserService _securityUserService;

        public StakeholdersSelectionStrategy(
            ISecurityService securityService,
            ISecurityUserService securityUserService)
        {
            _securityService = securityService;
            _securityUserService = securityUserService;
        }

        public IEnumerable<User> FilterStackholders(IUnitOfWork unitOfWork, IEnumerable<User> users, BaseObject obj)
        {
            var creatorObj = obj as ICreateObject;

            return creatorObj != null ? users.Where(x => x.ID == creatorObj.Creator.ID) : users;
        }
    }
}