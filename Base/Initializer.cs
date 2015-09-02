using Base.DAL;
using Base.Settings;
using System.Linq;

namespace Base
{
    public class Initializer : IBaseInitializer
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
            if (!_settingCategoryService.GetAll(uofw, hidden: null).Any())
            {
                _settingCategoryService.Create(uofw,
                    new SettingCategory()
                    {
                        Name = "Общие",
                        SysName = "main",
                    });
            }
        }
    }
}
