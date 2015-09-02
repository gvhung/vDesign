using System.Collections.Generic;
using Base.Service;

namespace Base.Validation
{
    public interface IValidationService : IService
    {
        List<ValidationResult> Validate<T>(IValidationContext context, T obj) where T : BaseObject;
    }
}
