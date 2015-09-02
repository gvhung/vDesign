using System.Collections.Generic;

namespace Base.Validation
{
    public interface IValidationRule
    {
        string Title { get; }
        string Description { get; }
    }

    public interface IValidationRule<in T> : IValidationRule
    {
        IEnumerable<ValidationResult> Validate(T prop, string propertyName);
    }
}
