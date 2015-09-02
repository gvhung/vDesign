using Base.DAL;
using Base.Registers.Entities;
using Base.Service;
using System.Linq;

namespace Base.Registers.Service
{
    public class MeasureService : BaseCategorizedItemService<Measure>, IMeasureService
    {
        public MeasureService(IBaseObjectServiceFacade facade) : base(facade) { }

        public override IQueryable<Measure> GetAllCategorizedItems(IUnitOfWork unitOfWork, int categoryID, bool? hidden = false)
        {
            string strID = HCategory.IdToString(categoryID);

            return this.GetAll(unitOfWork, hidden).Where(a => (a.Category_.sys_all_parents != null && a.Category_.sys_all_parents.Contains(strID)) || a.Category_.ID == categoryID);
        }
    }
}
