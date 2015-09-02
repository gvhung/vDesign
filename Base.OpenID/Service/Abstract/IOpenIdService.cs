using Base.DAL;
using Base.OpenID.Entities;
using Base.Security;
using Base.Service;
using System.Threading.Tasks;

namespace Base.OpenID.Service.Abstract
{
    public interface IOpenIdService : IService
    {
        Task<ExtAccount> GetAccountInfo(ServiceType type, string code);
        Task<User> Authorize(ISystemUnitOfWork unitOfWork, ServiceType type, string code);
        Task<ExtAccount> AddAccount(IUnitOfWork unitOfWork, ServiceType type, string code);
    }
}
