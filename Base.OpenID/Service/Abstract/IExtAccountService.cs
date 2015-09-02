using Base.DAL;
using Base.OpenID.Entities;
using Base.Service;
using System.Collections.Generic;

namespace Base.OpenID.Service.Abstract
{
    public interface IExtAccountService : IBaseObjectService<ExtAccount>
    {
        IEnumerable<ExtAccount> GetLinked(IUnitOfWork unitOfWork, int userID);
    }
}
