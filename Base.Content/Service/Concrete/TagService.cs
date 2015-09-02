using Base.Content.Entities;
using Base.Content.Service.Abstract;
using Base.DAL;
using Base.Service;
using Framework.FullTextSearch;
using Framework.Wrappers;
using System.Collections.Generic;
using System.Linq;

namespace Base.Content.Service.Concrete
{
    public class TagService : BaseCategorizedItemService<Tag>, ITagService
    {
        private readonly ICacheWrapper _cacheWrapper;
        private readonly IContentItemService _contentItemService;

        public TagService(
            IBaseObjectServiceFacade facade, 
            ICacheWrapper cacheWrapper,
            IContentItemService contentItemService)
            : base(facade) 
        {
            _cacheWrapper = cacheWrapper;
            _contentItemService = contentItemService;
        }

        public override IQueryable<Tag> GetAllCategorizedItems(IUnitOfWork unitOfWork, int categoryID, bool? hidden = false)
        {
            string strID = HCategory.IdToString(categoryID);
            return this.GetAll(unitOfWork, hidden).Where(a => (a.TagCategory.sys_all_parents != null && a.TagCategory.sys_all_parents.Contains(strID)) || a.TagCategory.ID == categoryID);
        }

        public IEnumerable<Tag> GetTags(IUnitOfWork unitOfWork, int? contentCategoryID = null, string filter = null)
        {
            if (contentCategoryID == null && filter == null)
                return new Tag[0];

            var q = !string.IsNullOrEmpty(filter) ? _contentItemService.GetAll(unitOfWork).FullTextSearch(filter, _cacheWrapper).SelectMany(x => x.Tags).Distinct() : this.GetAll(unitOfWork);

            if (contentCategoryID.HasValue)
                q = q.Where(tag => tag.ContentItems.Any(item => !item.Hidden && item.CategoryID == (int)contentCategoryID));

            return q.ToArray();
        }
    }
}
