using Base.UI;
using Base.Wizard.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using WebUI.Controllers;

namespace WebUI.Models
{
    public class WizardDialogViewModel : DialogViewModel
    {
        public WizardDialogViewModel(IBaseController controller, string mnemonic, TypeDialog type = TypeDialog.Frame) : base(controller, mnemonic, type)
        {
        }

        public WizardDialogViewModel(BaseViewModel baseViewModel, string mnemonic, TypeDialog type = TypeDialog.Frame) : base(baseViewModel, mnemonic, type)
        {
        }
    }

    public class WizardFormModel : StandartViewModel
    {
        public CommonEditorViewModel CommonEditorViewModel { get; set; }

        public WizardFormModel(BaseViewModel baseViewModel, string mnemonic, string dialogID, TypeDialog type = TypeDialog.Frame)
            : base(baseViewModel, mnemonic, dialogID, type)
        {
        }

        public WizardFormModel(DialogViewModel dialogViewModel, CommonEditorViewModel commonEditorViewModel)
            : base(dialogViewModel)
        {
            this.CommonEditorViewModel = commonEditorViewModel;
            this.FormName = "f_" + Guid.NewGuid().ToString("N");
        }

        public List<Step> Steps
        {
            get { return ((WizardDetailView)ViewModelConfig.DetailView).Steps; }
        }

        public List<EditorViewModel> Editors
        {
            get { return CommonEditorViewModel.Editors; }
        }

        public string FormName { get; set; }

        public string CompleteText
        {
            get { return ((WizardDetailView) ViewModelConfig.DetailView).CompleteText; }
        }
    }

    public class StepViewModel
    {
        public StepViewModel()
        {
        }

        public StepViewModel(WizardFormModel view, Step step)
        {
            Wizard = view;
            Step = step;
        }

        public WizardFormModel Wizard { get; set; }
        public Step Step { get; set; }

        public int StepIndex
        {
            get { return Step.Index; }
        }

        public string StepName
        {
            get { return Step.Name; }
        }

        public string StepDescription
        {
            get { return Step.Description; }
        }

        public string StepTitle
        {
            get { return Step.Title; }
        }

        public ViewModelConfig ViewModelConfig
        {
            get { return Wizard != null ? Wizard.ViewModelConfig : null; }
        }

        public bool HasDetailView
        {
            get { return Wizard != null && Wizard.HasDetailView; }
        }

        public string DialogID
        {
            get { return Wizard != null ? Wizard.DialogID : ""; }
        }

        public string WidgetID
        {
            get { return Wizard != null ? Wizard.WidgetID + "_" + StepIndex : ""; }
        }

        public TypeDialog Type
        {
            get { return Wizard != null ? Wizard.Type : TypeDialog.Modal; }
        }

        public string FormName
        {
            get { return Wizard != null ? Wizard.FormName + "_" + StepIndex : ""; }
        }

        public string Mnemonic
        {
            get { return Wizard != null ? Wizard.Mnemonic : ""; }
        }

        private List<EditorViewModel> editors { get; set; }
        public List<EditorViewModel> Editors
        {
            get
            {
                if (editors == null && Wizard != null)
                {
                    editors = new List<EditorViewModel>();
                    Step.StepProperties.ForEach(x => editors.Add(Wizard.Editors.FirstOrDefault(y => y.PropertyName == x.Name)));
                }
                    
                return editors;
            }
        }

        public int StepCount
        {
            get { return Wizard != null ? Wizard.Steps.Count : 0; }
        }
    }

}