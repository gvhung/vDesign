using Base.DAL;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Base.Service
{
    public interface IBaseCategoryService<T> : IBaseObjectService<T>, ICategoryCRUDService
        where T : HCategory
    {
        IQueryable<T> GetRoots(IUnitOfWork unitOfWork, bool? hidden = false);
        Task<IEnumerable<T>> GetRootsAsync(IUnitOfWork unitOfWork, bool? hidden = false);
        IQueryable<T> GetAllChildren(IUnitOfWork unitOfWork, int parentID, bool? hidden = false);
        Task<IEnumerable<T>> GetAllChildrenAsync(IUnitOfWork unitOfWork, int parentID, bool? hidden = false);
        IQueryable<T> GetChildren(IUnitOfWork unitOfWork, int parentID, bool? hidden = false);
        Task<IEnumerable<T>> GetChildrenAsync(IUnitOfWork unitOfWork, int parentID, bool? hidden = false);
        void ChangePosition(IUnitOfWork unitOfWork, T obj, int? posChangeID, string typePosChange);
    }

    public interface ICategoryCRUDService : IBaseObjectCRUDService
    {
        IQueryable<HCategory> GetRoots(IUnitOfWork unitOfWork, bool? hidden = false);
        Task<IEnumerable<HCategory>> GetRootsAsync(IUnitOfWork unitOfWork, bool? hidden = false);
        IQueryable<HCategory> GetAllChildren(IUnitOfWork unitOfWork, int parentID, bool? hidden = false);
        Task<IEnumerable<HCategory>> GetAllChildrenAsync(IUnitOfWork unitOfWork, int parentID, bool? hidden = false);
        IQueryable<HCategory> GetChildren(IUnitOfWork unitOfWork, int parentID, bool? hidden = false);
        Task<IEnumerable<HCategory>> GetChildrenAsync(IUnitOfWork unitOfWork, int parentID, bool? hidden = false);
        void ChangePosition(IUnitOfWork unitOfWork, HCategory obj, int? posChangeID, string typePosChange);
    }
}
