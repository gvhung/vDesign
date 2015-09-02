using Base.DAL;
using Base.OpenID.Entities;
using Base.OpenID.Service.Abstract;
using Base.Service;
using System.Collections.Generic;
using System.Linq;

namespace Base.OpenID.Service.Concrete
{
    public class ExtAccountService : BaseObjectService<ExtAccount>, IExtAccountService
    {
        public ExtAccountService(IBaseObjectServiceFacade facade) : base(facade)
        {}

        public IEnumerable<ExtAccount> GetLinked(IUnitOfWork unitOfWork, int userID)
        {
            return this.GetAll(unitOfWork).Where(x => x.UserID == userID).ToList();
        }
    }
}
