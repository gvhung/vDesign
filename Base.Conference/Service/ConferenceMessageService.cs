using System.Collections.Generic;
using Base.Conference.Entities;
using Base.DAL;
using Base.Service;

namespace Base.Conference.Service
{
    public class ConferenceMessageService : BaseObjectService<Entities.ConferenceMessage>, IConferenceMessageService
    {
        public ConferenceMessageService(IBaseObjectServiceFacade facade) : base(facade)
        {
        }
    }
}