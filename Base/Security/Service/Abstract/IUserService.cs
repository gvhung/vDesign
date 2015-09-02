using Base.Service;

namespace Base.Security.Service
{
    public interface IUserService : IBaseCategorizedItemService<User>, IReadOnly
    {
        void AddToFriends(ISecurityUser securityUser, int id);
        void RemoveFromFriends(ISecurityUser securityUser, int id);
    }
}
