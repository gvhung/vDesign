using Base.Content.Entities;
using Base.Content.Service.Abstract;
using Base.Service;

namespace Base.Content.Service.Concrete
{
    public class TagCategoryService : BaseCategoryService<TagCategory>, ITagCategoryService
    {
        public TagCategoryService(IBaseObjectServiceFacade facade) : base(facade) { }
    }
}
