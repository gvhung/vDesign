using System.Collections.Generic;
using Base.Conference.Entities;
using Base.DAL;
using Base.Service;

namespace Base.Conference.Service
{
    public interface IPrivateMessageService : IBaseObjectService<Entities.PrivateMessage>, ISimpleMessageService
    {
    }
}