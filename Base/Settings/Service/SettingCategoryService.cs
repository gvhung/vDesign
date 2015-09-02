using Base.Service;

namespace Base.Settings
{
    public class SettingCategoryService : BaseCategoryService<SettingCategory>, ISettingCategoryService
    {
        public SettingCategoryService(IBaseObjectServiceFacade facade) : base(facade) { }

    }
}
