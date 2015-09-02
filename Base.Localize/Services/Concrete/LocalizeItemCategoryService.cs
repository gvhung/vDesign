using Base.Localize.Entities;
using Base.Localize.Services.Abstract;
using Base.Service;

namespace Base.Localize.Services.Concrete
{
    public class LocalizeItemCategoryService : BaseCategoryService<LocalizeItemCategory>, ILocalizeItemCategoryService
    {
        public LocalizeItemCategoryService(IBaseObjectServiceFacade facade) : base(facade) { }
    }
}
