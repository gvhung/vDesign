using Base.Ambient;
using Base.Content.Entities;
using Base.Content.Service.Abstract;
using Base.DAL;
using Base.Service;
using System;
using System.Linq;

namespace Base.Content.Service.Concrete
{
    public class ContentSubscriberService : BaseObjectService<ContentSubscriber>, IContentSubscriberService
    {
        public ContentSubscriberService(IBaseObjectServiceFacade facade)
            : base(facade)
        {
        }

        protected override IObjectSaver<ContentSubscriber> GetForSave(IUnitOfWork unitOfWork, IObjectSaver<ContentSubscriber> objectSaver)
        {
            if (objectSaver.Dest.SubscriberId.HasValue &&
                !string.IsNullOrEmpty(objectSaver.Dest.SubscriberUser.Email))
            {
                objectSaver.Dest.SubscriberUser.Email = objectSaver.Dest.SubscriberUser.Email.ToLower();
            }
            else if (!string.IsNullOrEmpty(objectSaver.Dest.Email))
            {
                objectSaver.Dest.Email = objectSaver.Dest.Email.ToLower();
            }

            return base.GetForSave(unitOfWork, objectSaver)
                .SaveOneObject(x => x.SubscriberUser)
                .SaveOneObject(x => x.ContentCategory);
        }

        #region AUTHORIZED_ONLY
        public bool CanSubscribe(IUnitOfWork unitOfWork, int categoryID)
        {
            var category = unitOfWork.GetRepository<ContentCategory>().Find(categoryID);
            
            if (category == null) throw new Exception("Категория отсутствует в базе данных");

            if (!category.SubscribeAvailable) return false;

            return !unitOfWork.GetRepository<ContentSubscriber>()
                .All().Any(x =>
                    x.ContentCategoryId == categoryID &&
                    x.SubscriberId == AppContext.SecurityUser.ID &&
                    x.IsActive && !x.Hidden);
        }

        public void Subscribe(IUnitOfWork unitOfWork, int categoryId)
        {
            if (!CanSubscribe(unitOfWork, categoryId)) return;

            var subscribeRepository = unitOfWork.GetRepository<ContentSubscriber>();

            var subscribe = subscribeRepository
                .Find(x =>
                    x.ContentCategoryId == categoryId &&
                    x.SubscriberId == AppContext.SecurityUser.ID &&
                    !x.Hidden);

            if (subscribe != null)
            {
                subscribe.IsActive = true;

                subscribeRepository.Update(subscribe);
            }
            else
            {
                subscribeRepository.Create(new ContentSubscriber()
                {
                    SubscriberId = AppContext.SecurityUser.ID,
                    ContentCategoryId = categoryId
                });
            }

            unitOfWork.SaveChanges();
        }

        public void UnSubscribe(IUnitOfWork unitOfWork, int categoryId)
        {
            if (CanSubscribe(unitOfWork, categoryId)) return;

            var subscribeRepository = unitOfWork.GetRepository<ContentSubscriber>();

            var subscribe = subscribeRepository
                .Find(x =>
                    x.SubscriberId == AppContext.SecurityUser.ID &&
                    x.ContentCategoryId == categoryId &&
                    !x.Hidden);

            if (subscribe == null) return;
            
            subscribe.IsActive = false;
            subscribeRepository.Update(subscribe);

            unitOfWork.SaveChanges();
        }

        public IQueryable<ContentSubscriber> GetSubscribes(IUnitOfWork unitOfWork)
        {
            return unitOfWork.GetRepository<ContentSubscriber>().All().Where(x => x.SubscriberId == AppContext.SecurityUser.ID && !x.Hidden);
        }
        #endregion

        #region UNAUTHORIZED_ONLY
        public bool CanSubscribe(IUnitOfWork unitOfWork, string email, int categoryID)
        {
            var category = unitOfWork.GetRepository<ContentCategory>().Find(categoryID);
            if (category == null) throw new Exception("Категория отсутствует в базе данных");
            if (!category.SubscribeAvailable) return false;

            return !unitOfWork.GetRepository<ContentSubscriber>()
                .All()
                .Any(x =>
                    x.ContentCategoryId == categoryID &&
                    x.SubscriberId == null &&
                    x.Email.ToLower() == email.ToLower() &&
                    x.IsActive &&
                    !x.Hidden);
        }

        public void Subscribe(IUnitOfWork unitOfWork, string email, int categoryId)
        {
            if (!CanSubscribe(unitOfWork, email, categoryId)) return;

            var subscribeRepository = unitOfWork.GetRepository<ContentSubscriber>();

            var subscribe = subscribeRepository
                .Find(x =>
                    x.ContentCategoryId == categoryId &&
                    x.SubscriberId == null &&
                    x.Email.ToLower() == email.ToLower() &&
                    !x.Hidden);

            if (subscribe != null)
            {
                subscribe.IsActive = true;
                subscribeRepository.Update(subscribe);
            }
            else
            {
                subscribeRepository.Create(new ContentSubscriber()
                {
                    ContentCategoryId = categoryId,
                    Email = email
                });
            }

            unitOfWork.SaveChanges();
        }

        public void UnSubscribe(IUnitOfWork unitOfWork, string email, int categoryId)
        {
            if (CanSubscribe(unitOfWork, email, categoryId)) return;

            var subscribeRepository = unitOfWork.GetRepository<ContentSubscriber>();

            var subscribe = subscribeRepository
                .Find(x =>
                    x.SubscriberId == null &&
                    x.Email.ToLower() == email.ToLower() &&
                    x.ContentCategoryId == categoryId &&
                    !x.Hidden);

            if (subscribe == null) return;
            
            subscribe.IsActive = false;
            subscribeRepository.Update(subscribe);

            unitOfWork.SaveChanges();
        }

        public IQueryable<ContentSubscriber> GetSubscribes(IUnitOfWork unitOfWork, string email)
        {
            return unitOfWork.GetRepository<ContentSubscriber>().All().Where(x => x.SubscriberId == null && x.Email.ToLower() == email.ToLower() && !x.Hidden);
        }
        #endregion

        public int SubscribersCount(IUnitOfWork unitOfWork, int categoryID)
        {
            return GetSubscribers(unitOfWork, categoryID).Count();
        }

        public int SubscribersCount(IUnitOfWork unitOfWork, int categoryID, bool authorized)
        {
            return GetSubscribers(unitOfWork, categoryID, authorized).Count();
        }

        public IQueryable<ContentSubscriber> GetSubscribers(IUnitOfWork unitOfWork)
        {
            return unitOfWork.GetRepository<ContentSubscriber>().All().Where(x => x.IsActive && !x.Hidden);
        }

        public IQueryable<ContentSubscriber> GetSubscribers(IUnitOfWork unitOfWork, int categoryID)
        {
            return GetSubscribers(unitOfWork).Where(x => x.ContentCategoryId == categoryID);
        }

        public IQueryable<ContentSubscriber> GetSubscribers(IUnitOfWork unitOfWork, bool authorized)
        {
            return GetSubscribers(unitOfWork).Where(x => x.SubscriberId.HasValue == authorized);
        }

        public IQueryable<ContentSubscriber> GetSubscribers(IUnitOfWork unitOfWork, int categoryID, bool authorized)
        {
            return GetSubscribers(unitOfWork, categoryID).Where(x => x.SubscriberId.HasValue == authorized);
        }
    }
}
