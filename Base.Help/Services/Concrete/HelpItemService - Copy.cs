using Base.DAL;
using Base.Entities.Complex;
using Base.Help.Entities;
using Base.Security;
using Base.Security.Service.Abstract;
using Base.Service;
using System.Linq;

namespace Base.Help.Services
{
    public class HelpItemTagService : BaseObjectService<HelpItemTag>, IHelpItemTagService
    {
        public HelpItemTagService(IUnitOfWork context, ISecurityService securityService) : base(context, securityService) { }
    }
}
