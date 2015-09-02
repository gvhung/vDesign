using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Reflection;
using System.Runtime.Remoting;
using Base;
using Base.Validation;
using Framework;

namespace WebUI.Concrete
{
    public class ValidationService : IValidationService
    {
        public List<ValidationResult> Validate<T>(IValidationContext context, T obj) where T : BaseObject
        {
            List<ValidationResult> results = new List<ValidationResult>();

            foreach (var valBinding in context.ValidationRules)
            {
                var args = GetArgs(valBinding.PropertyName, obj);
                var rule = CreateRuleInstance(valBinding.ValidationRuleName).Unwrap();
                var ruleType = rule.GetType();
                results.AddRange((List<ValidationResult>)ruleType.InvokeMember("Validate", BindingFlags.InvokeMethod, null, rule, args));
            }
            return results;
        }

        private object[] GetArgs<T>(string propertyName, T obj)
        {
            var args = new object[2];
            if (propertyName == null)
                args[0] = obj;

            else
                args[0] = obj.GetType().GetProperty(propertyName).GetValue(obj);
            args[1] = propertyName;
            return args;
        }

        private ObjectHandle CreateRuleInstance(string valRuleName)
        {
            string assembly = valRuleName.Split(',')[1].Trim();
            string type = valRuleName.Split(',')[0].Trim();
            return Activator.CreateInstance(assembly, type);
        }
    }
}