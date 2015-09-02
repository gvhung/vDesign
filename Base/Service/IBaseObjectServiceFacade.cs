using Base.DAL;
using Base.Security.Service.Abstract;

namespace Base.Service
{
    public interface IBaseObjectServiceFacade
    {
        ISecurityService SecurityService { get; }
        IUnitOfWorkFactory UnitOfWorkFactory { get; }
    }
}
