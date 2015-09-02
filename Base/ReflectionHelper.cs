using Framework.Attributes;
using Framework.Maybe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Base
{
    public static class ReflectionHelper
    {
        public static bool IsEnum(this Type type)
        {
            return type.IsEnum || (type.IsGenericType && Nullable.GetUnderlyingType(type) != null && type.GetGenericArguments()[0].IsEnum);
        }

        public static bool IsBaseObject(this Type type)
        {
            return typeof(BaseObject).IsAssignableFrom(type);
        }

        public static Type GetBaseObjectType(this Type type)
        {
            if (type.IsBaseObject())
            {
                if (type.Namespace == "System.Data.Entity.DynamicProxies")
                {
                    return type.BaseType;
                }

                return type;
            }

            return null;
        }

        public static bool IsTypeCollection(this Type collection, Type type)
        {
            Type collectionType = collection.GetGenericType();

            return collectionType != null && type.IsAssignableFrom(collectionType.GenericTypeArguments[0]);
        }

        public static bool IsBaseCollection(this Type type)
        {
            Type genericType = type.GetGenericType();

            return genericType != null && IsBaseObject(genericType.GenericTypeArguments[0]);
        }

        public static Type GetGenericType(this Type collection)
        {
            Type genericType = null;

            if (collection.IsGenericType && collection.GetGenericTypeDefinition() == typeof(IEnumerable<>))
            {
                genericType = collection;
            }
            else
            {
                genericType = collection.GetInterface("IEnumerable`1");
            }

            return genericType;
        }

        public static bool IsAssignableFromBase(this Type type)
        {
            return type.IsBaseObject() || type.IsBaseCollection();
        }


        private static List<PropertyInfo> GetAllProperties(this Type container, Type type, BindingFlags flags, List<PropertyInfo> props = null, List<Type> analyzed = null)
        {
            if (props == null)
            {
                props = new List<PropertyInfo>();
            }

            if (analyzed == null)
            {
                analyzed = new List<Type>();
            }

            foreach (PropertyInfo prop in container.GetProperties(flags))
            {
                if (!analyzed.Contains(prop.PropertyType))
                {
                    analyzed.Add(prop.PropertyType);

                    if (type.IsAssignableFrom(prop.PropertyType))
                    {
                        props.Add(prop);
                    }
                    else
                    {
                        props = prop.PropertyType.GetAllProperties(type, flags, props, analyzed);
                    }
                }
            }

            return props;
        }

        public static bool HasTypeProperty(this Type container, Type type)
        {
            return container.GetAllProperties(type, BindingFlags.Public | BindingFlags.Instance).Count != 0;
        }

        public static bool IsFullTextSearchEnabled(this Type type)
        {
            var attr = type.GetCustomAttribute<EnableFullTextSearchAttribute>();

            if (attr != null)
            {
                return attr.Enabled;
            }

            return false;
        }

        public static Type GetEntryOfUnboundedTypeOfCollection(this Type target, Type type)
        {
            return target
                .GetInterfaces()
                .FirstOrDefault(t => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                .With(x => x.GetGenericArguments())
                .With(x => x[0])
                .With(x => TypeHierarchy(x).FirstOrDefault(t => t.IsGenericType && t.GetGenericTypeDefinition() == type))
                .With(x => x.GetGenericArguments())
                .With(x => x[0]);
        }

        public static Type GetEntryOfUnboundedTypeOf(this Type target, Type type, Type check)
        {
            return target
                .GetInterfaces()
                .FirstOrDefault(t => t.IsGenericType && t.GetGenericTypeDefinition() == check)
                .With(x => x.GetGenericArguments())
                .With(x => x[0])
                .With(x => TypeHierarchy(x).FirstOrDefault(t => t.IsGenericType && t.GetGenericTypeDefinition() == type))
                .With(x => x.GetGenericArguments())
                .With(x => x[0]);
        }

        public static IEnumerable<Type> TypeHierarchy(this Type type)
        {
            yield return type;
         
            if (type.BaseType != null)
                foreach (var t in TypeHierarchy(type.BaseType))
                    yield return t;
        }

        public static bool IsAssignableToGenericType(this Type givenType, Type genericType)
        {
            if (givenType == null || genericType == null)
            {
                return false;
            }

            return givenType == genericType
              || givenType.MapsToGenericTypeDefinition(genericType)
              || givenType.HasInterfaceThatMapsToGenericTypeDefinition(genericType)
              || givenType.BaseType.IsAssignableToGenericType(genericType);
        }

        private static bool HasInterfaceThatMapsToGenericTypeDefinition(this Type givenType, Type genericType)
        {
            return givenType
              .GetInterfaces()
              .Where(it => it.IsGenericType)
              .Any(it => it.GetGenericTypeDefinition() == genericType);
        }

        private static bool MapsToGenericTypeDefinition(this Type givenType, Type genericType)
        {
            return genericType.IsGenericTypeDefinition
              && givenType.IsGenericType
              && givenType.GetGenericTypeDefinition() == genericType;
        }

        public static string GetTypeName(this Type type)
        {
            return String.Format("{0}, {1}", type.FullName, type.Assembly.FullName.Split(',')[0]);
        }
    }
}