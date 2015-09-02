using System.Collections.Generic;

namespace Base.Wizard
{
    public class WizardObject : BaseObject, IWizardObject
    {
        //public int StepIndex { get; set; }
        public List<string> PreviousSteps { get; set; }
        public string Step { get; set; }
        public int StepCount { get; set; }
    }
}
