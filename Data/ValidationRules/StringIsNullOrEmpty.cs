using System.Collections.Generic;
using Base.Validation;

namespace Data.ValidationRules
{
    public class StringIsNullOrEmpty : IValidationRule<string>
    {
        public string Title
        {
            get { return "ПРоверка на пустоту"; }
        }

        public string Description
        {
            get { return "ПРоверка на пустоту"; }
        }

        public IEnumerable<ValidationResult> Validate(string prop, string propName)
        {
            List<ValidationResult> results = new List<ValidationResult>();
            if (prop == null)
            {
                ValidationResult res = new ValidationResult
                {
                    ErrorMessage = "Property is null",
                    PropetyName = propName,
                    ValidationRuleName = this.Title,
                    ValidationRuleDescription = this.Description
                };
                results.Add(res);
            }
            else
            {
                if(prop.Trim().Length == 0)
                {
                    ValidationResult res = new ValidationResult
                    {
                        ErrorMessage = "Property trimed length == 0",
                        PropetyName = propName,
                        ValidationRuleName = this.Title,
                        ValidationRuleDescription = this.Description
                    };
                    results.Add(res);
                }
            }


            return results;
        }
    }
}
