using Base.Ambient;
using Base.DAL;
using Base.Security;
using Base.UI;
using Base.UI.Presets;
using System;
using System.Linq;
using WebUI.Models.Dashboard.Widgets;

namespace WebUI.Concrete
{
    public class PresetFactory : IPresetFactory
    {
        private readonly IViewModelConfigService _viewModelConfigService;

        public PresetFactory(IViewModelConfigService viewModelConfigService)
        {
            _viewModelConfigService = viewModelConfigService;
        }

        public Preset CreatePreset(IUnitOfWork unitOfWork, string mnemonic, string ownerMnemonic)
        {
            var configPreset = _viewModelConfigService.Get(mnemonic);

            if (configPreset.TypeEntity == typeof(DashboardPreset))
            {
                var preset = new DashboardPreset() { Mnemonic = mnemonic, OwnerMnemonic = ownerMnemonic };

                var wConfigs =
                    _viewModelConfigService.GetAll()
                        .Where(x => typeof(DashboardWidgetVm).IsAssignableFrom(x.TypeEntity));

                bool isDeveloperUser = AppContext.SecurityUser.IsSysRole(SystemRole.Developer);

                foreach (var c in wConfigs.Where(c => AppContext.SecurityUser.IsPermission(c.TypeEntity, TypePermission.Read)))
                {
                    var widget = new DashboardWidget(c);

                    //if (isDeveloperUser && c.Mnemonic != "DashboardWidget_MyNpa" && c.Mnemonic != "DashboardWidget_News")
                    //{
                    //    widget.Hidden = true;
                    //}

                    preset.Widgets.Add(widget);
                }

                return preset;
            }
            else
            {
                var preset = Activator.CreateInstance(configPreset.TypeEntity) as Preset;

                if (preset != null)
                {
                    preset.Mnemonic = mnemonic;
                    preset.OwnerMnemonic = ownerMnemonic;
                }

                return preset;
            }
        }

        public Preset CreatePreset<T>(IUnitOfWork unitOfWork, string mnemonic, string ownerMnemonic) where T : Preset
        {
            return this.CreatePreset(unitOfWork, mnemonic, ownerMnemonic);
        }
    }
}