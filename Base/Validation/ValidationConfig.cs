using System.Collections.Generic;

namespace Base.Validation
{
    public class ValidationConfig
    {
        public ICollection<ValidationBinding> ConfigItems { get; set; }

        public ValidationConfig()
        {
            ConfigItems = new List<ValidationBinding>();

        }

        public void Add(ValidationBinding item)
        {
            ConfigItems.Add(item);
        }
    }
}

