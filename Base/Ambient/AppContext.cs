using Base.Security;
using System;

namespace Base.Ambient
{
    public class AppContext
    {
        private static IAppContextBootstrapper s_contextBootstrapper;

        public static ISecurityUser SecurityUser
        {
            get
            {
                return s_contextBootstrapper.GetSecurityUser();
            }
        }

        public static IDateTimeProvider DateTime
        {
            get { return s_contextBootstrapper.GetDateTimeProvider(); }
        }

        internal static void SetContextService(AppContextBootstrapper appContextBootstrapper)
        {
            if (appContextBootstrapper == null)
                throw new ArgumentNullException("appContextBootstrapper");

            s_contextBootstrapper = appContextBootstrapper;
        }
    }
}