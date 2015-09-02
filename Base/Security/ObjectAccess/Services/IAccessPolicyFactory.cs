using Base.Security.ObjectAccess.Policies;
using System;

namespace Base.Security.ObjectAccess.Services
{
    public interface IAccessPolicyFactory
    {
        IAccessPolicy GetAccessPolicy(Type type);
    }
}