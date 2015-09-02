using System;
using System.Collections.Generic;
using System.Linq;
using Base;
using Base.Validation;
using Data.ValidationRules;

namespace WebUI.Concrete
{
    public class ValidationConfigManager : ValidationConfigBuilder, IValidationConfigManager
    {
        public override void Load()
        {
            Bind<StringIsNullOrEmpty>().To<string>();
            Bind<BigStringValidation>().To<string>();
        }




        public ICollection<Type> GetRulesType(Type type)
        {
            return ValidationConfig.ConfigItems.Where(x => x.Target == type).Select(x => x.Source).ToList();
        }

        public ICollection<IValidationRule> GetRules(Type type)
        {
            return ValidationConfig.ConfigItems.Where(x => x.Target == type).Select(x => (IValidationRule)Activator.CreateInstance(x.Source)).ToList();
        }
    }
}
