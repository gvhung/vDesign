using Base.Security;

namespace Base.BusinessProcesses.Security
{
    public interface IWorkflowUserService
    {
        ISecurityUser WorkflowManager { get; }
    }
}