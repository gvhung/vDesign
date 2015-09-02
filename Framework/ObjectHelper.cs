using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace Framework
{
    public static class ObjectHelper
    {
        public static void CopyObject(object obj_src, object obj_dest, Type[] exceptTypes = null, string[] exceptProperties = null)
        {
            PropertyInfo[] dest_props = obj_dest.GetType().GetProperties();

            PropertyInfo[] src_props = obj_src.GetType().GetProperties().Where(p =>
            {
                ReadOnlyAttribute readOnlyAttr = p.GetCustomAttribute<ReadOnlyAttribute>();

                return readOnlyAttr == null || !readOnlyAttr.IsReadOnly;
            }).ToArray();

            if (exceptTypes != null)
            {
                foreach (Type type in exceptTypes)
                {
                    //TODO: переписать!!!
                    src_props = src_props.Where(p => !type.IsAssignableFrom(p.PropertyType)).ToArray();
                    
                    List<PropertyInfo> list = src_props.ToList();

                    foreach (PropertyInfo pi in src_props)
                    {
                        Type genericType = null;

                        if (pi.PropertyType.IsGenericType && pi.PropertyType.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                        {
                            genericType = pi.PropertyType;
                        }
                        else
                        {
                            genericType = pi.PropertyType.GetInterface("IEnumerable`1");
                        }

                        if (genericType != null && type.IsAssignableFrom(genericType.GenericTypeArguments[0]))
                        {
                            list.Remove(pi);
                        }
                    }

                    src_props = list.ToArray();
                }
            }

            if (exceptProperties != null)
            {
                src_props = src_props.Where(p => !exceptProperties.Contains(p.Name)).ToArray();
            }

            foreach (PropertyInfo pr_src in src_props)
            {
                if (pr_src.CanRead)
                {
                    PropertyInfo pr_dest = dest_props.FirstOrDefault(p => p.Name == pr_src.Name);

                    if (pr_dest != null && pr_dest.CanWrite)
                    {
                        if (pr_dest.PropertyType.Name == pr_src.PropertyType.Name)
                        {
                            pr_dest.SetValue(obj_dest, pr_src.GetValue(obj_src));
                        }
                    }
                }
            }
        }

        public static object CreateAndCopyObject(object obj_src, Type type, Type[] exceptTypes = null, string[] exceptProperties = null)
        {
            object obj_dest = Activator.CreateInstance(type);

            CopyObject(obj_src, obj_dest, exceptTypes, exceptProperties);

            return obj_dest;
        }

        public static T CreateAndCopyObject<T>(object obj_src, Type[] exceptTypes = null, string[] exceptProperties = null)
        {
            return (T)CreateAndCopyObject(obj_src, typeof(T), exceptTypes, exceptProperties);
        }
    }
}
