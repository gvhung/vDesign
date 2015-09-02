using Base.DAL;
using Base.Security;
using System.Collections.Generic;

namespace Base.BusinessProcesses.Strategies
{
    public interface IStakeholdersSelectionStrategy : IWorkflowStrategy
    {
        IEnumerable<User> FilterStackholders(IUnitOfWork unitOfWork, IEnumerable<User> users, BaseObject obj);
    }
}