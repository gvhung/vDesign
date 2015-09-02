using System.Collections.Generic;
using System.Threading.Tasks;
using Base.Conference.Entities;
using Base.DAL;
using Base.Service;

namespace Base.Conference.Service
{
    public interface IConferenceMessageService : IBaseObjectService<Entities.ConferenceMessage>
    {
    }

    public interface ISimpleMessageService
    {
        Task<MessageResult> CreateMessage(IUnitOfWork uofw, ConferenceMessage message, int targetId);
    }
}