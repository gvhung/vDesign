namespace Base.Security.Service.Abstract
{
    public interface IGuestUserService
    {
        void CreateGuestUser(string login);
        ISecurityUser GetGuestUser(string login);
    }
}