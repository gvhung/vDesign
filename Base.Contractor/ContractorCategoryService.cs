using Base.Service;

namespace Base.Contractor
{
    public class ContractorCategoryService : BaseCategoryService<ContractorCategory>, IContractorCategoryService
    {
        public ContractorCategoryService(IBaseObjectServiceFacade facade) : base(facade) { }
    }
}
