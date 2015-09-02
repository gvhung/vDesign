using Base.BusinessProcesses.Entities;
using Base.Content.Entities;
using Base.Content.Service.Abstract;
using Base.DAL;
using Base.Helpers;
using Base.Service;
using Framework.Wrappers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Base.Content.Service.Concrete
{
    public class ContentItemService : BaseCategorizedItemService<ContentItem>, IContentItemService
    {
        private readonly IContentCategoryService _categoryService;
        
        public ContentItemService(IBaseObjectServiceFacade facade, IContentCategoryService categoryService)
            : base(facade)
        {
            _categoryService = categoryService;
        }


        public override ContentItem CreateOnGroundsOf(IUnitOfWork unitOfWork, BaseObject obj)
        {
            var dtm = DateTime.Now;
            dtm = new DateTime(dtm.Year, dtm.Month, dtm.Day, dtm.Hour, dtm.Minute, 0);

            var contentItem = new ContentItem
            {
                Created = dtm
            };

            return contentItem;
        }

        public override IQueryable<ContentItem> GetAllCategorizedItems(IUnitOfWork unitOfWork, int categoryID, bool? hidden = false)
        {
            string strID = HCategory.IdToString(categoryID);

            return this.GetAll(unitOfWork, hidden).Where(a => (a.ContentCategory.sys_all_parents != null && a.ContentCategory.sys_all_parents.Contains(strID)) || a.ContentCategory.ID == categoryID);
        }

        protected override IObjectSaver<ContentItem> GetForSave(IUnitOfWork unitOfWork, IObjectSaver<ContentItem> objectSaver)
        {
            return base.GetForSave(unitOfWork, objectSaver)
                .SaveOneObject(x => x.ImagePreview)
                .SaveManyToMany(x => x.Tags);
        }

        public virtual IQueryable<ContentItem> GetAllNews(IUnitOfWork unitOfWork)
        {
            return GetAllByType(unitOfWork, ContentTypes.News);
        }

        public virtual IQueryable<ContentItem> GetAllPersons(IUnitOfWork unitOfWork)
        {
            return GetAllByType(unitOfWork, ContentTypes.Person);
        }

        public virtual IQueryable<ContentItem> GetAllVideo(IUnitOfWork unitOfWork)
        {
            return GetAllByType(unitOfWork, ContentTypes.Video);
        }

        public IQueryable<ContentItem> GetAllPublications(IUnitOfWork unitOfWork)
        {
            return GetAllByType(unitOfWork, ContentTypes.Article);
        }

        private IQueryable<ContentItem> GetAllByType(IUnitOfWork unitOfWork, string contentType)
        {
            var categories =
                _categoryService.GetAll(unitOfWork)
                    .Where(x => x.CategoryItemMnemonic == contentType)
                    .Select(x => x.ID)
                    .ToList();

            return GetAll(unitOfWork).Where(x => categories.Contains(x.CategoryID)).Distinct().OrderByDescending(x => x.SortOrder);
        }


        public int GetWorkflowID(IUnitOfWork unitOfWork, BaseObject obj)
        {
            return Workflow.Default;
        }


        public void BeforeInvoke(BaseObject obj)
        {

        }

        public void OnActionExecuting(ActionExecuteArgs args)
        {

        }
    }
}
