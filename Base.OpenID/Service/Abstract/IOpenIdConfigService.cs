using Base.OpenID.Entities;
using Base.Service;
using System.Collections.Generic;

namespace Base.OpenID.Service.Abstract
{
    public interface IOpenIdConfigService : IService
    {
        IEnumerable<OpenIdConfig> GetConfig();
        OpenIdConfig GetConfig(ServiceType type);
    }
}
