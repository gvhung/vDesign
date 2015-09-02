using Base.Attributes;
using Newtonsoft.Json;

namespace Base.Wizard
{
    public class DecoratedWizardObject<TObject> : WizardObject where TObject : new()
    {
        [SystemProperty]
        [JsonIgnore]
        public TObject DecoratedObject { get; set; }

        public DecoratedWizardObject()
        {
            DecoratedObject = new TObject();
        }

        public virtual void Configure(DecoratorConfiguration<TObject> configuration)
        {
        }

        public static implicit operator TObject(DecoratedWizardObject<TObject> dec)
        {
            return dec.DecoratedObject;
        }
    }
}