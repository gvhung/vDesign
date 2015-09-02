using System;
using System.Collections.Generic;
using System.Linq.Dynamic;
using Base.BusinessProcesses.Entities;
using Base.Validation;
using Framework.Maybe;
using IronPython.Modules;

namespace Base.BusinessProcesses.Services.Concrete
{
    public class StageActionValidationContext : IValidationContext
    {
        public List<ValidationBindingItem> ValidationRules { get; private set; }

        public StageActionValidationContext(StageAction action)
        {
            ValidationRules = new List<ValidationBindingItem>();
            var rules = action.ValidatonRules;
            foreach (var stageActionValidationItem in rules)
            {
                var keypair = new ValidationBindingItem(stageActionValidationItem.Property, stageActionValidationItem.ValidationRule);
                ValidationRules.Add(keypair);
            }
        }
    }
}
