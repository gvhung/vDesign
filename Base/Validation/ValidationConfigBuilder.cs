using System;

namespace Base.Validation
{
    public class ValidationConfigBuilder
    {
        protected ValidationConfig ValidationConfig = new ValidationConfig();

        public BindingTargetBuilder Bind<T>() where T : IValidationRule
        {
            ValidationBinding binding = new ValidationBinding {Source = typeof (T)};
            ValidationConfig.Add(binding);

            BindingTargetBuilder tb = new BindingTargetBuilder(binding);
            return tb;
        }

        public virtual void Load() { }



    }
    public class BindingTargetBuilder
    {
        private ValidationBinding Binding { get; set; }

        public BindingTargetBuilder(ValidationBinding binding)
        {
            Binding = binding;
        }

        public ValidationBinding To<T1>()
        {
            Type t = typeof(T1);
            Binding.Target = t;
            return Binding;
        }
    }
}