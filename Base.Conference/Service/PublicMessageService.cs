using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Base.Conference.Entities;
using Base.DAL;
using Base.Service;

namespace Base.Conference.Service
{
    public class PublicMessageService : BaseObjectService<Entities.PublicMessage>, IPublicMessageService
    {
        public PublicMessageService(IBaseObjectServiceFacade facade) : base(facade)
        {
        }

        public async Task<MessageResult> CreateMessage(IUnitOfWork uofw, ConferenceMessage message, int targetId)
        {
            var result = new List<int>();

            var publicMessage = new PublicMessage(message)
            {
                ToConferenceId = targetId
            };

            Create(uofw, publicMessage);

            await uofw.SaveChangesAsync();

            var dbConference = uofw.GetRepository<Entities.Conference>().Find(publicMessage.ToConferenceId);

            if (dbConference != null && dbConference.Members.Any())
            {
                result.AddRange(dbConference.Members.Where(x => x.ObjectID.HasValue).Select(conferenceMember => conferenceMember.ObjectID.Value));
            }

            return new MessageResult()
            {
                Message = publicMessage,
                Targets = result
            };
        }

    }
}