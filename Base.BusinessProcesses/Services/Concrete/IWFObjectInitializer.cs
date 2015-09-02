using Base.BusinessProcesses.Entities;
using Base.Security;
using System.Collections.Generic;

namespace Base.BusinessProcesses.Services.Concrete
{
    public interface IWFObjectInitializer
    {
        void InitializeObject(ISecurityUser securityUser, BaseObject src, BaseObject dest, IEnumerable<InitItem> inits);
    }
}