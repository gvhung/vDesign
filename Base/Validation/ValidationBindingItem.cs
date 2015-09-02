namespace Base.Validation
{
    public class ValidationBindingItem
    {
        public string PropertyName { get; private set; }
        public string ValidationRuleName { get; private set; }

        public ValidationBindingItem(string propertyName, string validationRuleName)
        {
            PropertyName = propertyName;
            ValidationRuleName = validationRuleName;
        }
    }
}
