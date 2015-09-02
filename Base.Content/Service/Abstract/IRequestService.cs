using Base.Security;
using Base.Service;
using Base.Content.Entities;
using Base.BusinessProcesses.Entities;

namespace Base.Content.Service.Abstract
{
    public interface IRequestService : IBaseObjectService<Request>, IBaseWFObjectService
    {
        void Create(ISecurityUser securityUser, string name, string email, string subject, string text);
    }
}
