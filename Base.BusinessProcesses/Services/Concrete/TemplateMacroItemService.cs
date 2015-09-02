using Base.BusinessProcesses.Entities;
using Base.BusinessProcesses.Services.Abstract;
using Base.Service;

namespace Base.BusinessProcesses.Services.Concrete
{
    public class TemplateMacroItemService : BaseObjectService<TemplateMacroItem>, ITemplateMacroItemService
    {
        public TemplateMacroItemService(IBaseObjectServiceFacade facade)
            : base(facade)
        {
        }
    }
}