using Base.Security;
using System;

namespace Base.Ambient
{
    public interface IAppContextBootstrapper
    {
        ISecurityUser GetSecurityUser();
        void SetSecurityUser(ISecurityUser securityUser);
        void RemoveSecurityUser();
        IDisposable LocalContextSecurity(ISecurityUser securityUser);
        IDateTimeProvider GetDateTimeProvider();
        void SetDateTimeProvider(IDateTimeProvider dateTimeProvider);
    }
}