using Base.UI;
using System.Collections.Generic;

namespace Base.Wizard.UI
{
    public interface IWizardDetailView : IDetailView
    {
        string FirstStep { get; set; }
        List<Step> Steps { get; set; }
        string CompleteText { get; set; }
    }

    public class WizardDetailView : DetailView, IWizardDetailView
    {
        public WizardDetailView() { }

        public WizardDetailView(IDetailView view)
        {
            Title = view.Title;
            Name = view.Name;
            Width = view.Width;
            Height = view.Height;
            isMaximaze = view.isMaximaze;
            HideToolbar = view.HideToolbar;
            DataSource = view.DataSource;
            Toolbars = view.Toolbars;
            Editors = view.Editors;
            CSHtmlHelper = view.CSHtmlHelper;
            WizardName = view.WizardName;
            AjaxForm = view.AjaxForm;

        }
        public string FirstStep { get; set; }

        public List<Step> Steps { get; set; }

        public string CompleteText { get; set; }
    }

    public class Step
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int Index { get; set; }
        public string Title { get; set; }
        public List<StepProperty> StepProperties { get; set; }
        public AjaxFormAction AjaxForm { get; set; }
    }

    public class StepProperty
    {
        public string Name { get; set; }
    }
}
