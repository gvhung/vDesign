using Base.Help.Entities;
using Base.Service;

namespace Base.Help.Services
{
    public class HelpItemTagService : BaseObjectService<HelpItemTag>, IHelpItemTagService
    {
        public HelpItemTagService(IBaseObjectServiceFacade facade) : base(facade) { }
    }
}
