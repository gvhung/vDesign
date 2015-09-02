using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Base.Validation;

namespace Data.ValidationRules
{
    public class BigStringValidation : IValidationRule<string>
    {

        public IEnumerable<ValidationResult> Validate(string prop, string propName)
        {
            List<ValidationResult> results = new List<ValidationResult>();

            if (prop != null)
            {

                if (prop.Length > 250)
                    results.Add(new ValidationResult { ErrorMessage = "Больше 250 символов", PropetyName = propName });
            }
            else
            {
                results.Add(new ValidationResult() { ErrorMessage = "Argument is null ", PropetyName = propName });
            }
            return results;

        }

        public string Title
        {
            get { return "Проверка на длину"; }
        }

        public string Description
        {
            get { return "Проверка на длину"; }
        }
    }
}
