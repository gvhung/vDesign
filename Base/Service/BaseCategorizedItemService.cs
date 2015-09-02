using Base.DAL;
using System.Linq;

namespace Base.Service
{
    public abstract class BaseCategorizedItemService<T> : BaseObjectService<T>, IBaseCategorizedItemService<T>
        where T : BaseObject, ICategorizedItem 
    {
        public BaseCategorizedItemService(IBaseObjectServiceFacade facade) : base(facade) { }

        public virtual IQueryable<T> GetCategorizedItems(IUnitOfWork unitOfWork, int categoryID, bool? hidden = false)
        {
            return this.GetAll(unitOfWork, hidden).Where(a => a.CategoryID == categoryID);
        }

        // NOTE: Необходимо наложить фильтрацию на Category.sys_all_parents
        public abstract IQueryable<T> GetAllCategorizedItems(IUnitOfWork unitOfWork, int categoryID, bool? hidden = false);

        public virtual void ChangeCategory(IUnitOfWork unitOfWork, int id, int newCategoryID)
        {
            var obj = this.Get(unitOfWork, id);

            if (obj == null) return;

            obj.CategoryID = newCategoryID;

            this.Update(unitOfWork, obj);
        }

        #region CRUD
        void ICategorizedItemCRUDService.ChangeCategory(IUnitOfWork unitOfWork, int id, int newCategoryID)
        {
            this.ChangeCategory(unitOfWork, id, newCategoryID);
        }

        IQueryable<BaseObject> ICategorizedItemCRUDService.GetCategorizedItems(IUnitOfWork unitOfWork, int categoryID, bool? hidden = false)
        {
            return this.GetCategorizedItems(unitOfWork, categoryID, hidden) as IQueryable<BaseObject>;
        }

        IQueryable<BaseObject> ICategorizedItemCRUDService.GetAllCategorizedItems(IUnitOfWork unitOfWork, int categoryID, bool? hidden = false)
        {
            return this.GetAllCategorizedItems(unitOfWork, categoryID, hidden) as IQueryable<BaseObject>;
        }
        #endregion CRUD
    }
}
