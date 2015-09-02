using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;
using Base.Security;
using Base.Validation;

namespace Data.ValidationRules
{
    public class UserValidator : IValidationRule<User>
    {
        public IEnumerable<ValidationResult> Validate(User prop, string propName)
        {
            return new List<ValidationResult>();
        }

        public string Title
        {
            get { return "Проверка пользователя "; }
        }

        public string Description
        {
            get { return "Проверка пользователя "; }
        }
    }
}
