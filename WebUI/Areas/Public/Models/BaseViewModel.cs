using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Base.Security;
using Base.UI;
using WebUI.Controllers;

namespace WebUI.Areas.Public.Models
{
    public class BaseViewModel
    {
        private readonly ISecurityUser _securityUser;
        private readonly IReadOnlyList<ViewModelConfig> _viewModelConfigs;

        public ISecurityUser SecurityUser { get { return _securityUser; } }
        public IReadOnlyList<ViewModelConfig> ViewModelConfigs { get { return _viewModelConfigs; } }

        public ViewModelConfig DefaultViewModelConfig(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            return ViewModelConfigs.First(x => x.TypeEntity == type);
        }

        #region Ctors

        public BaseViewModel(ISecurityUser securityUser, IReadOnlyList<ViewModelConfig> viewModelConfigs)
        {
            _securityUser = securityUser;
            _viewModelConfigs = viewModelConfigs;
        }

        public BaseViewModel(IBaseController controller)
        {
            _securityUser = controller.SecurityUser;

            _viewModelConfigs = controller.ViewModelConfigs;
        }

        public BaseViewModel(BaseViewModel baseViewModel)
        {
            _securityUser = baseViewModel.SecurityUser;

            _viewModelConfigs = baseViewModel.ViewModelConfigs;
        }
        #endregion

        #region Methods

        public ViewModelConfig GetViewModelConfig(string mnemonic)
        {
            if (String.IsNullOrEmpty(mnemonic))
            {
                throw new ArgumentException("IsNullOrEmpty", "mnemonic");
            }

            if (
                ViewModelConfigs.Any(m => String.Equals(m.Mnemonic, mnemonic, StringComparison.CurrentCultureIgnoreCase)))
                return
                    ViewModelConfigs.FirstOrDefault(
                        x => String.Equals(x.Mnemonic, mnemonic, StringComparison.CurrentCultureIgnoreCase));

            var config = ViewModelConfigs.FirstOrDefault(m => m.Entity.Split(',')[0].Trim() == mnemonic.Split(',')[0].Trim());

            if (config != null)
                mnemonic = config.Mnemonic;

            return ViewModelConfigs.FirstOrDefault(x => String.Equals(x.Mnemonic, mnemonic, StringComparison.CurrentCultureIgnoreCase));
        }

        #endregion

    }

    public class BasePageViewModel : BaseViewModel
    {
        public BasePageViewModel(ISecurityUser securityUser, IReadOnlyList<ViewModelConfig> viewModelConfigs) : base(securityUser, viewModelConfigs)
        {
        }

        public BasePageViewModel(IBaseController controller) : base(controller)
        {
        }

        public BasePageViewModel(BaseViewModel baseViewModel) : base(baseViewModel)
        {
        }

        public List<MenuItemVm> MenuItems { get; set; }
    }

    public class HomePageViewModel : BasePageViewModel
    {
        public HomePageViewModel(ISecurityUser securityUser, IReadOnlyList<ViewModelConfig> viewModelConfigs) : base(securityUser, viewModelConfigs)
        {
        }

        public HomePageViewModel(IBaseController controller) : base(controller)
        {
        }

        public HomePageViewModel(BaseViewModel baseViewModel) : base(baseViewModel)
        {
        }

        public List<ContentListItemVm> ContentItems { get; set; }
    }
}