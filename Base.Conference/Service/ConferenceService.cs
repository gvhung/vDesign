using System;
using Base.Ambient;
using Base.DAL;
using Base.Service;

namespace Base.Conference.Service
{
    public class ConferenceService : BaseObjectService<Entities.Conference>, IConferenceService
    {
        public ConferenceService(IBaseObjectServiceFacade facade) : base(facade)
        {
        }

        protected override void BeforeCreateOnGroundsOf(IUnitOfWork unitOfWork, BaseObject srcObj, BaseObject destObj)
        {
            base.BeforeCreateOnGroundsOf(unitOfWork, srcObj, destObj);
        }

        protected override IObjectSaver<Entities.Conference> GetForSave(IUnitOfWork unitOfWork, IObjectSaver<Entities.Conference> objectSaver)
        {
            objectSaver.SaveOneToMany(x => x.Members, saver => saver.SaveOneObject(x => x.Object)).SaveOneObject(x => x.Creator);

            return base.GetForSave(unitOfWork, objectSaver);
        }
    }
}