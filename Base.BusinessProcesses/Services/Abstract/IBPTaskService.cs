using Base.BusinessProcesses.Entities;
using Base.Security;
using Base.Service;

namespace Base.BusinessProcesses.Services.Abstract
{
    public interface IBPTaskService : IBaseCategorizedItemService<BPTask>
    {
        BPTask CreateTask(ISecurityUser securityUser, BaseObject obj, Stage stage, bool saveChanges = true);
    }
}
