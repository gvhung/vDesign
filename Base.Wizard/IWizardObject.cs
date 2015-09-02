using System.Collections.Generic;

namespace Base.Wizard
{
    public interface IWizardObject: IBaseObject
    {
        //int StepIndex { get; set; }
        List<string> PreviousSteps { get; set; }
        string Step { get; set; }
        int StepCount { get; set; }
    }
}
