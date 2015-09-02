using Base.Content.Entities;
using Base.DAL;
using Base.Service;
using System.Collections.Generic;
using System.Linq;

namespace Base.Content.Service.Abstract
{
    public interface IContentCategoryService : IBaseCategoryService<ContentCategory>
    {
        string GetCategoryMnemonic(IUnitOfWork unitOfWork, int categoryID);

        List<ContentSubscriber> GetCategorySubscribers(IUnitOfWork unitOfWork, int categoryId, int? regionId = null);

        void DeleteUnauthorizedSubscriber(IUnitOfWork unitOfWork, int categoryID, string email);

        IQueryable<ContentCategory> GetSubscribeAvailable(IUnitOfWork unitOfWork);
    }
}
