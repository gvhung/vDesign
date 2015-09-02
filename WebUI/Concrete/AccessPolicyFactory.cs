using Base.Security.ObjectAccess.Policies;
using Base.Security.ObjectAccess.Services;
using Ninject;
using Ninject.Syntax;
using System;

namespace WebUI.Concrete
{
    public class AccessPolicyFactory : IAccessPolicyFactory
    {
        private readonly IResolutionRoot _resolutionRoot;

        public AccessPolicyFactory(IResolutionRoot resolutionRoot)
        {
            _resolutionRoot = resolutionRoot;
        }

        public IAccessPolicy GetAccessPolicy(Type type)
        {
            if (!typeof(IAccessPolicy).IsAssignableFrom(type))
                throw new ArgumentException("Invalid argument");

            return _resolutionRoot.Get(type) as IAccessPolicy;
        }
    }
}