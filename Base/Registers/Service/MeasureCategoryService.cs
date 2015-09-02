using Base.Registers.Entities;
using Base.Service;

namespace Base.Registers.Service
{
    public class MeasureCategoryService : BaseCategoryService<MeasureCategory>, IMeasureCategoryService
    {
        public MeasureCategoryService(IBaseObjectServiceFacade facade) : base(facade) { }
    }
}
