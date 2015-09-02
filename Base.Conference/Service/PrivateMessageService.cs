using System.Collections.Generic;
using System.Threading.Tasks;
using Base.Conference.Entities;
using Base.DAL;
using Base.Service;

namespace Base.Conference.Service
{
    public class PrivateMessageService : BaseObjectService<Entities.PrivateMessage>, IPrivateMessageService
    {
        public PrivateMessageService(IBaseObjectServiceFacade facade) : base(facade)
        {
        }

        public async Task<MessageResult> CreateMessage(IUnitOfWork uofw, ConferenceMessage message, int targetId)
        {
            var targets = new List<int>();

            var privateMessage = new PrivateMessage(message)
                        {
                            ToUserId = targetId
                        };

            Create(uofw, privateMessage);

            await uofw.SaveChangesAsync();

            targets.Add(privateMessage.FromId);
            
            if(privateMessage.ToUserId.HasValue)
                targets.Add(privateMessage.ToUserId.Value);

            return new MessageResult()
            {
                Message = privateMessage,
                Targets = targets
            };
        }

        protected override IObjectSaver<PrivateMessage> GetForSave(IUnitOfWork unitOfWork, IObjectSaver<PrivateMessage> objectSaver)
        {
            return base.GetForSave(unitOfWork, objectSaver).SaveOneObject(x => x.File);
        }
    }
}