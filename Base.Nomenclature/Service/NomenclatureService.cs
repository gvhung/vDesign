using Base.DAL;
using Base.Service;
using System.Linq;

namespace Base.Nomenclature.Service
{
    public class NomenclatureService : BaseCategorizedItemService<Entities.Nomenclature>, INomenclatureService
    {
        public NomenclatureService(IBaseObjectServiceFacade facade) : base(facade) { }

        public override IQueryable<Entities.Nomenclature> GetAllCategorizedItems(IUnitOfWork unitOfWork, int categoryID, bool? hidden = false)
        {
            string strID = HCategory.IdToString(categoryID);

            return this.GetAll(unitOfWork, hidden)
                .Where(a => (a.Category_.sys_all_parents != null && a.Category_.sys_all_parents.Contains(strID)) || a.Category_.ID == categoryID);
        }

        protected override IObjectSaver<Entities.Nomenclature> GetForSave(IUnitOfWork unitOfWork, IObjectSaver<Entities.Nomenclature> objectSaver)
        {
            if(!objectSaver.IsNew && objectSaver.Src.Okpd != null && objectSaver.Src.Okpd.Code != objectSaver.Dest.Code)
            {
                objectSaver.Dest.Code = objectSaver.Src.Okpd.Code;
            }
            return base.GetForSave(unitOfWork, objectSaver).SaveOneObject(x => x.Image);
        }
    }
}
