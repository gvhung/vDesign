
namespace Base.Security.ObjectAccess
{
    public interface IEnabledState
    {
        bool IsEnabled(ISecurityUser user);
    }
}
