using Base.Audit.Entities;
using Base.DAL;
using Base.Settings;
using System.Linq;

namespace Base.Audit
{
    public interface IAuditInitializer : IBaseInitializer { }

    public class Initializer : IAuditInitializer
    {
        private readonly ISettingItemService _settingItemService;
        private readonly ISettingCategoryService _settingCategoryService;

        public Initializer(ISettingItemService settingItemService, ISettingCategoryService settingCategoryService)
        {
            _settingItemService = settingItemService;
            _settingCategoryService = settingCategoryService;
        }

        public void Init(IUnitOfWork uofw)
        {
            if (!_settingItemService.GetAll(uofw, hidden: null).Any(x => x.Key == Consts.Settings.KEY_CONFIG))
            {
                _settingItemService.Create(uofw, new SettingItem()
                {
                    CategoryID = _settingCategoryService.GetAll(uofw).Where(x => x.SysName == "main").Select(x => x.ID).FirstOrDefault(),
                    Key = Consts.Settings.KEY_CONFIG,
                    Text = "Аудит",
                    Value = new Setting(new Config()
                    {
                        RegisterLogIn = false,
                        MaxRecordCount = -1
                    })
                });
            }
        }
    }
}
