using Base.Security;
using System;
using System.Runtime.Remoting.Messaging;

namespace Base.Ambient
{
    public class AppContextBootstrapper : IAppContextBootstrapper
    {
        private static readonly string s_userKey = Guid.NewGuid().ToString("N");
        private static readonly string s_dateTimeProviderKey = Guid.NewGuid().ToString("N");
        
        public AppContextBootstrapper(IDateTimeProvider dateTimeProvider)
        {
            if (dateTimeProvider == null)
                throw new ArgumentNullException("DefaultDateTimeProvider");

            SetDateTimeProvider(dateTimeProvider);

            AppContext.SetContextService(this);
        }

        public void SetSecurityUser(ISecurityUser securityUser)
        {
            if (securityUser == null)
                throw new ArgumentNullException("securityUser");

            //var user = GetSecurityUser();
            //if (user != null)
            //    throw new InvalidOperationException("The user should be set only once");

            CallContext.LogicalSetData(s_userKey, securityUser);
        }

        public void RemoveSecurityUser()
        {
            CallContext.LogicalSetData(s_userKey, null);
        }

        public ISecurityUser GetSecurityUser()
        {
            return CallContext.LogicalGetData(s_userKey) as ISecurityUser;
        }

        public IDisposable LocalContextSecurity(ISecurityUser securityUser)
        {
            return new LocalContextSecurityUser(securityUser, GetSecurityUser(), this);
        }

        public IDateTimeProvider GetDateTimeProvider()
        {
            return CallContext.LogicalGetData(s_dateTimeProviderKey) as IDateTimeProvider;
        }
        public void SetDateTimeProvider(IDateTimeProvider dateTimeProvider)
        {
            if (dateTimeProvider == null)
                throw new ArgumentNullException("dateTimeProvider");

            CallContext.LogicalSetData(s_dateTimeProviderKey, dateTimeProvider);
        }

        public class LocalContextSecurityUser : IDisposable
        {
            private readonly ISecurityUser _restoreUser;
            private readonly AppContextBootstrapper _appContextBootstrapper;

            public LocalContextSecurityUser(ISecurityUser newUser, ISecurityUser restoreUser, AppContextBootstrapper appContextBootstrapper)
            {
                _restoreUser = restoreUser;
                _appContextBootstrapper = appContextBootstrapper;
                _appContextBootstrapper.RemoveSecurityUser();
                _appContextBootstrapper.SetSecurityUser(newUser);
            }

            public void Dispose()
            {
                _appContextBootstrapper.RemoveSecurityUser();

                if (_restoreUser != null)
                {
                    _appContextBootstrapper.SetSecurityUser(_restoreUser);
                }
            }
        }
    }
}