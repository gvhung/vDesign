using System.Collections.Generic;
using Base.UI;
using Base.Validation;

namespace WebUI.Models.Validation
{
    public class PropertyValidationVm
    {       
        public ICollection<IValidationRule> ValidationRules { get; set; }
    }
}