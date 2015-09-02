using Base.Content.Entities;
using Base.DAL;
using Base.Service;
using System.Linq;

namespace Base.Content.Service.Abstract
{
    public interface IContentSubscriberService : IBaseObjectService<ContentSubscriber>
    {
        #region AUTHORIZED_ONLY
        bool CanSubscribe(IUnitOfWork unitOfWork, int categoryID);
        void Subscribe(IUnitOfWork unitOfWork, int categoryID);
        void UnSubscribe(IUnitOfWork unitOfWork, int categoryID);
        IQueryable<ContentSubscriber> GetSubscribes(IUnitOfWork unitOfWork);
        #endregion

        #region UNAUTHORIZED_ONLY
        bool CanSubscribe(IUnitOfWork unitOfWork, string email, int categoryID);
        void Subscribe(IUnitOfWork unitOfWork, string email, int categoryID);
        void UnSubscribe(IUnitOfWork unitOfWork, string email, int categoryID);
        IQueryable<ContentSubscriber> GetSubscribes(IUnitOfWork unitOfWork, string email);
        #endregion

        int SubscribersCount(IUnitOfWork unitOfWork, int categoryID);
        int SubscribersCount(IUnitOfWork unitOfWork, int categoryID, bool authorized);
        IQueryable<ContentSubscriber> GetSubscribers(IUnitOfWork unitOfWork);
        IQueryable<ContentSubscriber> GetSubscribers(IUnitOfWork unitOfWork, int categoryID);
        IQueryable<ContentSubscriber> GetSubscribers(IUnitOfWork unitOfWork, bool authorized);
        IQueryable<ContentSubscriber> GetSubscribers(IUnitOfWork unitOfWork, int categoryID, bool authorized);
    }
}
