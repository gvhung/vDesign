using Base.DAL;
using Base.Security.Service.Abstract;

namespace Base.Service
{
    public class BaseObjectServiceFacade: IBaseObjectServiceFacade
    {
        private readonly ISecurityService _securityService;
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;

        public BaseObjectServiceFacade(ISecurityService securityService, IUnitOfWorkFactory unitOfWorkFactory)
        {
            _securityService = securityService;
            _unitOfWorkFactory = unitOfWorkFactory;
        }

        public ISecurityService SecurityService {
            get { return _securityService; }
        }
        public IUnitOfWorkFactory UnitOfWorkFactory
        {
            get { return _unitOfWorkFactory; }
        }
    }
}
