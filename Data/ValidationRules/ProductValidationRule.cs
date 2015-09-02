using System.Collections.Generic;
using Base.Validation;
using Data.Entities.Product;

namespace Data.ValidationRules
{
    public class ProductValidationRule : IValidationRule<Product>
    {
        public IEnumerable<ValidationResult> Validate(Product prop, string propName)
        {
            return new List<ValidationResult>();
        }

        public string Title
        {
            get { return "Total product validation"; }
        }

        public string Description
        {
            get { return "Total product validation"; }
        }
    }
}
