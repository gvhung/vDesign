using Base;
using Base.DAL;
using Base.UI;
using Base.UI.Presets;
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using WebUI.Controllers;

namespace WebUI.Models
{
    public class StandartDialogViewModel : DialogViewModel
    {
        public StandartDialogViewModel(IBaseController controller)
            : base(controller)
        {
        }

        public StandartDialogViewModel(IBaseController controller, string mnemonic, TypeDialog type = TypeDialog.Frame)
            : base(controller, mnemonic, type)
        {
        }

        public StandartDialogViewModel(BaseViewModel baseViewModel, string mnemonic, TypeDialog type = TypeDialog.Frame)
            : base(baseViewModel, mnemonic, type)
        {
        }
    }

    public class StandartViewModel : Dialog_WidgetViewModel
    {
        public StandartViewModel(BaseViewModel baseViewModel, string mnemonic, string dialogID, TypeDialog type = TypeDialog.Frame)
            : base(baseViewModel, mnemonic, dialogID, type)
        {
        }

        public StandartViewModel(DialogViewModel dialogViewModel)
            : base(dialogViewModel)
        {
        }

        public StandartViewModel(IBaseController controller)
            : base(controller)
        {
        }
    }

    public class StandartFormModel : StandartViewModel
    {
        public CommonEditorViewModel CommonEditorViewModel { get; set; }

        public StandartFormModel(BaseViewModel baseViewModel, string mnemonic, string dialogID, TypeDialog type = TypeDialog.Frame)
            : base(baseViewModel, mnemonic, dialogID, type)
        {
        }

        public StandartFormModel(DialogViewModel dialogViewModel, CommonEditorViewModel commonEditorViewModel)
            : base(dialogViewModel)
        {
            this.CommonEditorViewModel = commonEditorViewModel;
            this.FormName = "f_" + Guid.NewGuid().ToString("N");
        }

        public StandartFormModel(IBaseController controller)
            : this(controller, controller.HttpContext.Request["mnemonic"])
        {

        }

        public StandartFormModel(IBaseController controller, string mnemonic)
            : base(controller)
        {
            this.CommonEditorViewModel = controller.GetCommonEditor(mnemonic);
            this.FormName = "f_" + Guid.NewGuid().ToString("N");
        }

        public StandartFormModel(IBaseController controller, CommonEditorViewModel commonEditorViewModel)
            : base(controller)
        {
            this.CommonEditorViewModel = commonEditorViewModel;
            this.FormName = "f_" + Guid.NewGuid().ToString("N");
        }


        public int TabsCount
        {
            get { return this.CommonEditorViewModel.Tabs.Count; }
        }

        public List<TabVm> Tabs
        {
            get { return this.CommonEditorViewModel.Tabs; }
        }

        public string FormName { get; set; }

        public BaseObject Model { get; set; }
    }

    public class StandartTreeView : Dialog_WidgetViewModel
    {
        public StandartTreeView(BaseViewModel baseViewModel, string mnemonic, string dialogID, TypeDialog type = TypeDialog.Frame)
            : base(baseViewModel, mnemonic, dialogID, type)
        {
        }

        public StandartTreeView(DialogViewModel dialogViewModel)
            : base(dialogViewModel)
        {
        }
    }

    public class StandartGridView : Dialog_WidgetViewModel
    {
        private GridPreset _preset;

        public StandartGridView(IBaseController controller, string mnemonic, string dialogID, TypeDialog type = TypeDialog.Frame)
            : base(controller, mnemonic, dialogID, type)
        {
        }

        public StandartGridView(BaseViewModel baseViewModel, string mnemonic, string dialogID, TypeDialog type = TypeDialog.Frame)
            : base(baseViewModel, mnemonic, dialogID, type)
        {
        }

        public StandartGridView(DialogViewModel dialogViewModel)
            : base(dialogViewModel)
        {
        }

        public StandartGridView(IBaseController controller)
            : base(controller)
        {
        }

        public GridPreset Preset
        {
            get
            {
                if (_preset != null) return _preset;

                IPresetService presetService = DependencyResolver.Current.GetService<IPresetService>();
                IUnitOfWorkFactory unitOfWorkFactory = DependencyResolver.Current.GetService<IUnitOfWorkFactory>();

                using (var unitOfWork = unitOfWorkFactory.Create())
                {
                    _preset = presetService.GetPreset<GridPreset>(unitOfWork, this.Mnemonic);
                }

                return _preset;
            }
        }
    }

    public class CustomDialogView : Dialog_WidgetViewModel
    {
        public CustomDialogView(BaseViewModel baseViewModel, string mnemonic, string dialogID, TypeDialog type = TypeDialog.Frame)
            : base(baseViewModel, mnemonic, dialogID, type)
        {
        }

        public CustomDialogView(DialogViewModel dialogViewModel)
            : base(dialogViewModel)
        {
        }

        public CustomDialogView(IBaseController controller)
            : base(controller)
        {
        }
    }

    public class StandartScheduler : Dialog_WidgetViewModel
    {
        public StandartScheduler(BaseViewModel baseViewModel, string mnemonic, string dialogID, TypeDialog type = TypeDialog.Frame)
            : base(baseViewModel, mnemonic, dialogID, type)
        {
        }

        public StandartScheduler(DialogViewModel dialogViewModel)
            : base(dialogViewModel)
        {
        }
    }

    public class StandartGantt : Dialog_WidgetViewModel
    {
        public StandartGantt(BaseViewModel baseViewModel, string mnemonic, string dialogID, TypeDialog type = TypeDialog.Frame)
            : base(baseViewModel, mnemonic, dialogID, type)
        {
        }

        public StandartGantt(DialogViewModel dialogViewModel)
            : base(dialogViewModel)
        {
        }
    }
}