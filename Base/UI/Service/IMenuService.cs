using Base.Security;
using Base.Service;

namespace Base.UI
{
    public interface IMenuService : IService
    {
        Menu Get();
        void Clear(ISecurityUser user);
    }
}
