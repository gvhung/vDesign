using Base.DAL;
using System.Linq;

namespace Base.Service
{
    public interface IBaseCategorizedItemService<T> : IBaseObjectService<T>, ICategorizedItemCRUDService
        where T : BaseObject, ICategorizedItem 
    {
        IQueryable<T> GetCategorizedItems(IUnitOfWork unitOfWork, int categoryID, bool? hidden = false);
        IQueryable<T> GetAllCategorizedItems(IUnitOfWork unitOfWork, int categoryID, bool? hidden = false);
        void ChangeCategory(IUnitOfWork unitOfWork, int id, int newCategoryID);
    }

    public interface ICategorizedItemCRUDService : IBaseObjectCRUDService
    {
        IQueryable<BaseObject> GetCategorizedItems(IUnitOfWork unitOfWork, int categoryID, bool? hidden = false);
        IQueryable<BaseObject> GetAllCategorizedItems(IUnitOfWork unitOfWork, int categoryID, bool? hidden = false);
        void ChangeCategory(IUnitOfWork unitOfWork, int id, int newCategoryID);
    }
}
