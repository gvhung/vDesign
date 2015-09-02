using Base.BusinessProcesses.Entities;
using Base.DAL;
using Base.Security;
using Base.Service;
using System.Collections.Generic;

namespace Base.BusinessProcesses.Services.Abstract
{
    public interface IStageUserService : IBaseObjectService<StageUser>
    {
        List<User> GetStakeholders(IUnitOfWork unitOfWork, TaskStep taskStep, BaseObject obj);
        List<User> GetStakeholders(IUnitOfWork unitOfWork, Stage stage, BaseObject obj);        
        List<User> GetStakeholders(IUnitOfWork unitOfWork, IBPObject obj);
    }
}
