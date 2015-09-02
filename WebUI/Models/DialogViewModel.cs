using Base;
using Base.Security;
using Base.UI;
using Framework.Maybe;
using System;
using System.Collections.Generic;
using WebUI.Controllers;

namespace WebUI.Models
{
    public abstract class DialogViewModel : BaseViewModel
    {
        protected readonly string _mnemonic;
        protected readonly TypeDialog _type;
        protected string _dialogID;
        protected ViewModelConfig _viewModelConfig;

        public string Mnemonic { get { return _mnemonic; } }
        public TypeDialog Type { get { return _type; } }
        public string DialogID { get { return _dialogID; } }

        public int? ParentID { get; set; }
        public int? CurrentID { get; set; }
        public string SysFilter { get; set; }
        public string SearchStr { get; set; }
        public List<Filter> Filters { get; set; }

        public ViewModelConfig ViewModelConfig
        {
            get
            {
                return UiFasade.GetViewModelConfig(Mnemonic);
            }
        }

        public bool HasDetailView
        {
            get
            {
                return !String.IsNullOrEmpty(this.ViewModelConfig.DetailView.Name);
            }
        }

        public bool HasWizard
        {
            get { return !string.IsNullOrEmpty(ViewModelConfig.DetailView.WizardName); }
        }

        public bool HasListView
        {
            get
            {
                return !String.IsNullOrEmpty(this.ViewModelConfig.ListView.With(x => x.Name));
            }
        }

        public bool HasLookupProperty
        {
            get
            {
                return !String.IsNullOrEmpty(this.ViewModelConfig.LookupProperty);
            }
        }

        public bool IsPermission(TypePermission typePermission)
        {
            return this.SecurityUser.IsPermission(this.ViewModelConfig.TypeEntity, typePermission);
        }

        public bool IsReadOnly
        {
            get { return this.ViewModelConfig.IsReadOnly || typeof(IReadOnly).IsAssignableFrom(this.ViewModelConfig.TypeService); }
        }

        protected DialogViewModel(IBaseController controller)
            : base(controller)
        {
            var request = controller.HttpContext.Request;

            _mnemonic = request["mnemonic"];

            _dialogID = request["_dialogid"];

            if (!String.IsNullOrEmpty(request["_dialogtype"]))
                _type = (TypeDialog)Enum.Parse(typeof(TypeDialog), request["_dialogtype"]);

            if (!String.IsNullOrEmpty(request["_parentid"]))
                ParentID = Int32.Parse(request["_parentid"]);

            if (!String.IsNullOrEmpty(request["_currentid"]))
                CurrentID = Int32.Parse(request["_currentid"]);
        }

        public DialogViewModel(IBaseController controller, string mnemonic, TypeDialog type = TypeDialog.Frame)
            : base(controller)
        {
            _mnemonic = mnemonic;
            _type = type;

            _dialogID = controller.HttpContext.Request["_dialogid"] ?? "dialog_" + Guid.NewGuid().ToString("N");
        }

        public DialogViewModel(BaseViewModel baseViewModel, string mnemonic, TypeDialog type = TypeDialog.Frame)
            : base(baseViewModel)
        {
            _mnemonic = mnemonic;
            _type = type;
            _dialogID = "dialog_" + Guid.NewGuid().ToString("N");
        }
    }

    public abstract class Dialog_WidgetViewModel : DialogViewModel
    {
        private readonly string _widgetID;

        public string WidgetID { get { return _widgetID; } }

        protected Dialog_WidgetViewModel(IBaseController controller, string mnemonic, string dialogID, TypeDialog type = TypeDialog.Frame)
            : base(controller, mnemonic, type)
        {
            _widgetID = "widget_" + Guid.NewGuid().ToString("N");
            _dialogID = dialogID;
        }

        protected Dialog_WidgetViewModel(BaseViewModel baseViewModel, string mnemonic, string dialogID, TypeDialog type = TypeDialog.Frame)
            : base(baseViewModel, mnemonic, type)
        {
            _widgetID = "widget_" + Guid.NewGuid().ToString("N");
            _dialogID = dialogID;
        }

        protected Dialog_WidgetViewModel(DialogViewModel dialogViewModel)
            : base(dialogViewModel, dialogViewModel.Mnemonic, dialogViewModel.Type)
        {
            this._widgetID = "widget_" + Guid.NewGuid().ToString("N");
            this._dialogID = dialogViewModel.DialogID;
            this.ParentID = dialogViewModel.ParentID;
            this.CurrentID = dialogViewModel.CurrentID;
            this.SysFilter = dialogViewModel.SysFilter;
            this.Filters = dialogViewModel.Filters;
            this.SearchStr = dialogViewModel.SearchStr;
        }

        protected Dialog_WidgetViewModel(IBaseController controller)
            : base(controller)
        {
            var request = controller.HttpContext.Request;
            this._widgetID = request["_widgetid"];
        }
    }

    public enum TypeDialog
    {
        Frame = 0,
        Modal = 1,
        Lookup = 2,
        Custom = 3,
    }
}