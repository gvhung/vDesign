using System;
using System.Collections.Generic;

namespace Base.Validation
{
    public interface IValidationConfigManager
    {
        ICollection<Type> GetRulesType(Type type);

        ICollection<IValidationRule> GetRules(Type type);
    }
}
