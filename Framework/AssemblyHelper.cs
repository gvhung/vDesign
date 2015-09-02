using System;
using System.Linq;

namespace Framework
{
    public static class AssemblyHelper
    {
        public static Type GetTypeByFullName(string fullName)
        {
            Type type = Type.GetType(fullName);

            if (type != null)
            {
                return type;
            }

            return AppDomain.CurrentDomain.GetAssemblies().Where(a => !a.IsDynamic).SelectMany(a => a.GetTypes()).FirstOrDefault(x => x.FullName == fullName);
        }
    }
}
