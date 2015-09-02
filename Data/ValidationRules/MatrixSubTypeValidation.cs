using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Base.Validation;
using Data.Entities.Product;

namespace Data.ValidationRules
{
    public class MatrixSubTypeValidation : IValidationRule<MatrixSubType>
    {
        public IEnumerable<ValidationResult> Validate(MatrixSubType prop, string propName)
        {
            List<ValidationResult> results = new List<ValidationResult>();
            if (prop == null)
                results.Add(new ValidationResult() { ErrorMessage = "Is null", PropetyName = "MatrixSubtype", ValidationRuleName = Title, ValidationRuleDescription = Description });
            else
            {
                if(prop.FullTitle.Length > 100 )
                    results.Add(new ValidationResult() { ErrorMessage = "Matix sub type have full title length over 250"}); 
            }
            return results;
        }

        public string Title
        {
            get { return "Проверка подтипа матрицы"; }
        }

        public string Description
        {
            get { return "Проверка подтипа матрицы"; }
        }
    }
}
