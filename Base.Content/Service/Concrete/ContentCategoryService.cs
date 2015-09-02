using Base.Content.Entities;
using Base.Content.Service.Abstract;
using Base.DAL;
using Base.Service;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Base.Content.Service.Concrete
{
    public class ContentCategoryService : BaseCategoryService<ContentCategory>, IContentCategoryService
    {
        public ContentCategoryService(IBaseObjectServiceFacade facade) : base(facade) { }

        protected override IObjectSaver<ContentCategory> GetForSave(IUnitOfWork unitOfWork, IObjectSaver<ContentCategory> objectSaver)
        {
            var current = objectSaver.Dest;

            if (current.ContentCategoryType == ContentCategoryType.ContentRegular)
            {
                if (string.IsNullOrEmpty(current.CategoryItemMnemonic))
                {
                    throw new Exception("Мнемоника - обязательное поле");
                }
            }
            else
            {
                current.CategoryItemMnemonic = null;
            }

            if (current.ContentCategoryType == ContentCategoryType.ContentExtended)
            {
                if (string.IsNullOrEmpty(current.Action) || string.IsNullOrEmpty(current.Controller))
                {
                    throw new Exception("Action и Controller обязательные поля");
                }
            }
            else
            {
                current.Action = current.Controller = null;
            }

            //current.Action = current.Controller = null;

            if (current.ContentCategoryType != ContentCategoryType.ContentExtended
                && current.ContentCategoryType != ContentCategoryType.Banner)
            {
                current.Params = null;
            }

            return base.GetForSave(unitOfWork, objectSaver)
                .SaveOneObject(x => x.Image)
                .SaveOneToMany(x => x.ContentSubscribers,
                    saver => saver
                        .SaveOneObject(u => u.SubscriberUser));
        }

        public string GetCategoryMnemonic(IUnitOfWork unitOfWork, int categoryID)
        {
            var category = this.Get(unitOfWork, categoryID);
            return category == null ? null : category.CategoryItemMnemonic;
        }

        public List<ContentSubscriber> GetCategorySubscribers(IUnitOfWork unitOfWork, int categoryId, int? regionId = null)
        {
            var category = this.Get(unitOfWork, categoryId);

            return category == null ? null : category.ContentSubscribers.Where(x => x.IsActive && !x.Hidden && (!x.SubscriberId.HasValue || !x.SubscriberUser.Hidden)).ToList();
        }

        public void DeleteUnauthorizedSubscriber(IUnitOfWork unitOfWork, int categoryID, string email)
        {
            var repo = unitOfWork.GetRepository<ContentSubscriber>();

            foreach (var s in repo.All().Where(x => x.ContentCategoryId == categoryID && x.Email == email))
            {
                repo.Delete(s);
            }

            unitOfWork.SaveChanges();
        }

        public IQueryable<ContentCategory> GetSubscribeAvailable(IUnitOfWork unitOfWork)
        {
            return GetAll(unitOfWork).Where(x => x.SubscribeAvailable);
        }
    }
}
