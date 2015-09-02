using Base.DAL;
using Base.Service;
using System.Linq;

namespace Base.Contractor
{
    public class ContractorService : BaseCategorizedItemService<Contractor>, IContractorService
    {
        public ContractorService(IBaseObjectServiceFacade facade) : base(facade) { }

        public override IQueryable<Contractor> GetAllCategorizedItems(IUnitOfWork unitOfWork, int categoryID, bool? hidden = false)
        {
            string strID = HCategory.IdToString(categoryID);

            return this.GetAll(unitOfWork, hidden).Where(a => (a.Category_.sys_all_parents != null && a.Category_.sys_all_parents.Contains(strID)) || a.Category_.ID == categoryID);
        }

        protected override IObjectSaver<Contractor> GetForSave(IUnitOfWork unitOfWork, IObjectSaver<Contractor> objectSaver)
        {
            return base.GetForSave(unitOfWork, objectSaver)
                .SaveOneToMany(x => x.Contacts, saver => saver.SaveOneObject(c => c.Image));
        }
    }
}
