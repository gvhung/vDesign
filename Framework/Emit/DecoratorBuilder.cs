using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace Framework.Emit
{
    public class DecoratorBuilder
    {
        public static Type CreateDynamicDecorator(
            Type decoratedType,
            IEnumerable<PropertyInfo> properties,
            Type baseType,
            string decoratedProperty = "DecoratedObject",
            string typeName = null,
            string assemblyName = null,
            string moduleName = null,
            bool createDecoratedProperty = false)
        {
            typeName = typeName ?? "DynamicDecorator_" + Guid.NewGuid().ToString("N");
            var assName = new AssemblyName(decoratedType.FullName + ".DynamicDecorator");

            var assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(assName, AssemblyBuilderAccess.Run);
            var moduleBuilder = assemblyBuilder.DefineDynamicModule(decoratedType.Module.Name);

            baseType = baseType ?? typeof (object);

            var tb = moduleBuilder.DefineType(typeName,
                TypeAttributes.Public | TypeAttributes.Class | TypeAttributes.AutoClass | TypeAttributes.AnsiClass |
                TypeAttributes.BeforeFieldInit | TypeAttributes.AutoLayout, baseType);

            foreach (var prop in properties.Distinct())
                CreateDecoratedProperty(tb, prop, baseType, decoratedProperty, decoratedType);

            return tb.CreateType();
        }

        private static void CreateDecoratedProperty(TypeBuilder tb, PropertyInfo prop, Type decoratorType, string decoratedPropertyName, Type decoratedType)
        {
            var propertyName = prop.Name;
            var propertyType = prop.PropertyType;

            var propertyBuilder = tb.DefineProperty(propertyName, PropertyAttributes.None, propertyType, null);

            foreach (var builder in prop.GetCustomAttributesData().Select(GetAttributeCopy))
                propertyBuilder.SetCustomAttribute(builder);

            var getPropMthdBldr = tb.DefineMethod("get_" + propertyName,
                MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig, propertyType,
                Type.EmptyTypes);

            var getIl = getPropMthdBldr.GetILGenerator();
            getIl.Emit(OpCodes.Ldarg_0);
            getIl.Emit(OpCodes.Castclass, decoratorType);
            getIl.Emit(OpCodes.Callvirt, decoratorType.GetProperty(decoratedPropertyName,
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).GetGetMethod(true));
            getIl.Emit(OpCodes.Callvirt, decoratedType.GetProperty(propertyName,
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).GetGetMethod(true));
            getIl.Emit(OpCodes.Ret);

            var setPropMthdBldr = tb.DefineMethod("set_" + propertyName,
                MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig, null,
                new[] { propertyType });

            propertyBuilder.SetGetMethod(getPropMthdBldr);

            var setIl = setPropMthdBldr.GetILGenerator();

            //var modifyProperty = setIl.DefineLabel();
            //var exitSet = setIl.DefineLabel();
            //setIl.MarkLabel(modifyProperty);
            setIl.Emit(OpCodes.Ldarg_0);
            //setIl.Emit(OpCodes.Castclass, decoratorType);
            setIl.Emit(OpCodes.Callvirt, decoratorType.GetProperty(decoratedPropertyName,
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).GetGetMethod(true));
            setIl.Emit(OpCodes.Ldarg_1);
            setIl.Emit(OpCodes.Callvirt, decoratedType.GetProperty(propertyName,
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).GetSetMethod(true));
            //setIl.Emit(OpCodes.Nop);
            //setIl.MarkLabel(exitSet);
            setIl.Emit(OpCodes.Ret);

            propertyBuilder.SetSetMethod(setPropMthdBldr);
        }

        public static CustomAttributeBuilder GetAttributeCopy(CustomAttributeData attrData)
        {
            if (attrData.NamedArguments == null)
            {
                var attrBuilder = new CustomAttributeBuilder(
                    attrData.Constructor,
                    attrData.ConstructorArguments.Select(ca => ca.Value).ToArray());

                return attrBuilder;
            }
            else
            {
                var attrBuilder = new CustomAttributeBuilder(
                    attrData.Constructor,
                    attrData.ConstructorArguments
                        .Select(ca => ca.Value)
                        .ToArray(),
                    attrData.NamedArguments.Where(na => na.MemberInfo is PropertyInfo)
                        .Select(na => na.MemberInfo as PropertyInfo)
                        .ToArray(),
                    attrData.NamedArguments.Where(na => na.MemberInfo is PropertyInfo)
                        .Select(na => na.TypedValue.Value)
                        .ToArray(),
                    attrData.NamedArguments.Where(na => na.MemberInfo is FieldInfo)
                        .Select(na => na.MemberInfo as FieldInfo)
                        .ToArray(),
                    attrData.NamedArguments.Where(na => na.MemberInfo is FieldInfo)
                        .Select(na => na.TypedValue.Value)
                        .ToArray());

                return attrBuilder;
            }
        }
    }
}