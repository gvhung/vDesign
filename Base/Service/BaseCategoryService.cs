using Base.DAL;
using Base.QueryableExtensions;
using Base.Security;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Base.Service
{
    public abstract class BaseCategoryService<TH> : BaseObjectService<TH>, IBaseCategoryService<TH>
        where TH : HCategory
    {
        protected BaseCategoryService(IBaseObjectServiceFacade facade) : base(facade) { }

        public virtual IQueryable<TH> GetRoots(IUnitOfWork unitOfWork, bool? hidden = false)
        {
            return this.GetAll(unitOfWork, hidden).Where(node => node.sys_all_parents == null).OrderBy(m => m.SortOrder);
        }

        public virtual async Task<IEnumerable<TH>> GetRootsAsync(IUnitOfWork unitOfWork, bool? hidden = false)
        {
            return await this.GetRoots(unitOfWork, hidden).ToGenericListAsync();
        }

        public virtual IQueryable<TH> GetAllChildren(IUnitOfWork unitOfWork, int parentID, bool? hidden = false)
        {
            string strID = HCategory.IdToString(parentID);

            return this.GetAll(unitOfWork, hidden).Where(node => (node.sys_all_parents != null && node.sys_all_parents.Contains(strID))).OrderBy(m => m.SortOrder);
        }

        public virtual async Task<IEnumerable<TH>> GetAllChildrenAsync(IUnitOfWork unitOfWork, int parentID, bool? hidden = false)
        {
            return await this.GetAllChildren(unitOfWork, parentID, hidden).ToGenericListAsync();
        }

        public virtual IQueryable<TH> GetChildren(IUnitOfWork unitOfWork, int parentID, bool? hidden = false)
        {
            return this.GetAll(unitOfWork, hidden).Where(node => node.ParentID == parentID).OrderBy(m => m.SortOrder);
        }

        public virtual async Task<IEnumerable<TH>> GetChildrenAsync(IUnitOfWork unitOfWork, int parentID, bool? hidden = false)
        {
            return await this.GetChildren(unitOfWork, parentID, hidden).ToGenericListAsync();
        }

        protected override int GetMaxSortOrder()
        {
            using (var uofw = UnitOfWorkFactory.CreateSystem())
            {
                return uofw.GetRepository<TH>().All().Max(m => (int?)m.SortOrder) ?? 0;
            }
        }

        public override TH Create(IUnitOfWork unitOfWork, TH obj)
        {
            if (obj.ParentID == 0) obj.ParentID = null;

            if (obj.ParentID != null)
            {
                var parent  = unitOfWork.GetRepository<TH>().Find(x => x.ID == obj.ParentID);

                obj.SetParent(parent);
            }

            return base.Create(unitOfWork, obj);
        }

        public override TH Update(IUnitOfWork unitOfWork, TH obj)
        {
            if (obj.ParentID == 0) obj.ParentID = null;

            if (obj.ParentID != null)
            {
                var parent = unitOfWork.GetRepository<TH>().Find(x => x.ID == obj.ParentID);

                obj.SetParent(parent);
            }

            return base.Update(unitOfWork, obj);
        }

        public override void Delete(IUnitOfWork unitOfWork, TH obj)
        {
            this.GetAllChildren(unitOfWork, obj.ID).ToList().ForEach(node =>
            { 
                node.Hidden = true;
                unitOfWork.GetRepository<TH>().Update(node);
            });

            base.Delete(unitOfWork, obj);
        }

        public virtual void ChangePosition(IUnitOfWork unitOfWork, TH obj, int? posChangeID, string typePosChange)
        {
            SecurityService.ThrowIfAccessDenied(unitOfWork, typeof(TH), TypePermission.Write);

            var repository = unitOfWork.GetRepository<TH>();

            if (posChangeID != null && posChangeID != 0)
            {
                if (typePosChange == "over")
                {
                    var parent = this.Get(unitOfWork, (int)posChangeID);

                    if (parent != null)
                    {
                        obj.SortOrder = (parent.Children.Max(m => (int?)m.SortOrder) ?? 0) + 1;
                    }

                    this.ChangeParent(unitOfWork, obj, parent);
                }
                else
                {
                    var hc = this.Get(unitOfWork, (int)posChangeID);

                    if (hc != null)
                    {
                        if (hc.ParentID != obj.ParentID)
                        {
                            this.ChangeParent(unitOfWork, obj, hc.Parent as TH);
                        }

                        if (typePosChange == "after")
                        {
                            obj.SortOrder = hc.SortOrder + 1;
                        }
                        else
                        {
                            obj.SortOrder = hc.SortOrder;
                            hc.SortOrder++;
                        }

                        int sort = hc.SortOrder + 1;

                        this.GetAll(unitOfWork)
                            .Where(m => m.ParentID == hc.ParentID)
                            .Where(m => m.ID != hc.ID && m.ID != obj.ID && m.SortOrder >= sort).ToList().ForEach(m =>
                            {
                                m.SortOrder = sort;
                                sort++;

                                repository.Update(obj);
                            });

                        repository.Update(obj);
                        repository.Update(hc);
                    }
                }
            }

            unitOfWork.SaveChanges();
        }

        private void ChangeParent(IUnitOfWork unitOfWork, TH obj, TH parent)
        {
            obj.SetParent(parent);

            unitOfWork.GetRepository<TH>().Update(obj);

            this.GetChildren(unitOfWork, obj.ID).ToList().ForEach(node =>
            {
                this.ChangeParent(unitOfWork, node, obj);
            });
        }

        #region CRUD

        IQueryable<HCategory> ICategoryCRUDService.GetRoots(IUnitOfWork unitOfWork, bool? hidden = false)
        {
            return this.GetRoots(unitOfWork, hidden);
        }

        IQueryable<HCategory> ICategoryCRUDService.GetAllChildren(IUnitOfWork unitOfWork, int parentID, bool? hidden = false)
        {
            return this.GetAllChildren(unitOfWork, parentID, hidden);
        }

        IQueryable<HCategory> ICategoryCRUDService.GetChildren(IUnitOfWork unitOfWork, int parentID, bool? hidden = false)
        {
            return this.GetChildren(unitOfWork, parentID, hidden);
        }

        void ICategoryCRUDService.ChangePosition(IUnitOfWork unitOfWork, HCategory obj, int? posChangeID, string typePosChange)
        {
            this.ChangePosition(unitOfWork, obj as TH, posChangeID, typePosChange);
        }

        async Task<IEnumerable<HCategory>> ICategoryCRUDService.GetRootsAsync(IUnitOfWork unitOfWork, bool? hidden = false)
        {
            return await this.GetRootsAsync(unitOfWork, hidden);
        }

        async Task<IEnumerable<HCategory>> ICategoryCRUDService.GetAllChildrenAsync(IUnitOfWork unitOfWork, int parentID, bool? hidden = false)
        {
            return await this.GetAllChildrenAsync(unitOfWork, parentID, hidden);
        }

        async Task<IEnumerable<HCategory>> ICategoryCRUDService.GetChildrenAsync(IUnitOfWork unitOfWork, int parentID, bool? hidden = false)
        {
            return await this.GetChildrenAsync(unitOfWork, parentID, hidden);
        }
        #endregion
    }
}
