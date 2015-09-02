using Base.Nomenclature.Entities;
using Base.Service;

namespace Base.Nomenclature.Service
{
    public class NomenclatureCategoryService : BaseCategoryService<NomenclatureCategory>, INomenclatureCategoryService
    {
        public NomenclatureCategoryService(IBaseObjectServiceFacade facade) : base(facade) { }
    }
}
