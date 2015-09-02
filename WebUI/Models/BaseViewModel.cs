using Base.Security;
using Base.UI;
using Base.UI.Service;
using System;
using WebUI.Controllers;

namespace WebUI.Models
{
    public class BaseViewModel
    {
        private readonly IUiFasade _uiFasade;
        private readonly ISecurityUser _securityUser;

        public ISecurityUser SecurityUser { get { return _securityUser; } }

        public ViewModelConfig DefaultViewModelConfig(Type type)
        {
            return _uiFasade.GetViewModelConfig(type);
        }

        public IUiFasade UiFasade
        {
            get { return _uiFasade; }
        }

        #region Ctors
        public BaseViewModel(IBaseController controller)
        {
            _securityUser = controller.SecurityUser;
            _uiFasade = controller.UiFasade;
        }

        public BaseViewModel(BaseViewModel baseViewModel)
        {
            _securityUser = baseViewModel.SecurityUser;
            _uiFasade = baseViewModel.UiFasade;
        }
        #endregion

        public ViewModelConfig GetViewModelConfig(string mnemonic)
        {
            return _uiFasade.GetViewModelConfig(mnemonic);
        }
    }
}