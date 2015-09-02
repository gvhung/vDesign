using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls.WebParts;
using Base;
using Base.Ambient;
using Base.Conference.Entities;
using Base.Conference.Service;
using Base.DAL;
using Base.Security;
using Base.Security.Service;
using Base.Service;
using Base.UI;
using Framework;
using Kendo.Mvc.Infrastructure;
using Microsoft.AspNet.SignalR.Hubs;

namespace WebUI.Hubs
{
    [HubName("conferenceHub")]
    public class ConferenceHub : BaseHub
    {
        private static readonly List<ConferenceUser> _connections = new List<ConferenceUser>();

        private readonly ISecurityUserService _securityUserService;
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;

        private readonly IPrivateMessageService _privateMessageService;
        private readonly IPublicMessageService _publicMessageService;
        private readonly IConferenceService _conferenceService;

        public ConferenceHub(ISecurityUserService securityUserService, IUnitOfWorkFactory unitOfWorkFactory, IPrivateMessageService privateMessageService, IPublicMessageService publicMessageService, IConferenceService conferenceService, IViewModelConfigService viewModelConfigService)
            : base(viewModelConfigService)
        {
            _securityUserService = securityUserService;
            _unitOfWorkFactory = unitOfWorkFactory;
            _privateMessageService = privateMessageService;
            _publicMessageService = publicMessageService;
            _conferenceService = conferenceService;
        }

        public async Task WebRtcSend(string message)
        {
            await Clients.All.onMessageReceived(message);
        }


        public async Task SendTextMessage(int dialogId, string message, string dialogType)
        {
            var connection = _connections.FirstOrDefault(x => x.ConnectionId == Context.ConnectionId);

            var baseMessage = new ConferenceMessage()
            {
                Date = DateTime.Now,
                IsNew = true,
                TextMessage = message,
                FromId = connection.Id
            };

            using (var uofw = _unitOfWorkFactory.CreateSystem())
            {
                var serv = (ISimpleMessageService)GetService<IBaseObjectCRUDService>(dialogType);

                var messageResult = await serv.CreateMessage(uofw, baseMessage, dialogId);

                var result = new JsonNetResult(messageResult.Message).ToString();

                await _sendTextMessage(messageResult.Targets, result, dialogType);
            }

        }


        public async Task SendFileMessage(int dialogId, FileData file, string dialogType)
        {
            var connection = _connections.FirstOrDefault(x => x.ConnectionId == Context.ConnectionId);

            var baseMessage = new ConferenceMessage()
            {
                Date = DateTime.Now,
                IsNew = true,
                File = file,
                MessageType = MessageContentType.File,
                FromId = connection.Id
            };

            using (var uofw = _unitOfWorkFactory.CreateSystem())
            {
                var serv = (ISimpleMessageService)GetService<IBaseObjectCRUDService>(dialogType);

                var messageResult = await serv.CreateMessage(uofw, baseMessage, dialogId);

                var result = new JsonNetResult(messageResult.Message).ToString();

                await _sendTextMessage(messageResult.Targets, result, dialogType);
            }
        }

        private async Task _sendTextMessage(IEnumerable<int> targets, string result, string dialogType)
        {
            foreach (var target in targets)
            {
                var member = _connections.FirstOrDefault(x => x.Id == target);
                if(member != null)
                    await Clients.Client(member.ConnectionId).OnTextMessageSend(result, dialogType);
            }
        }

        [HubMethodName("updateConferences")]
        public async Task UpdateConferences(int conferenceId)
        {
            using (var uofw = _unitOfWorkFactory.CreateSystem())
            {
                var conference = _conferenceService.Get(uofw, conferenceId);

                if (conference != null && conference.Members.Any())
                {
                    foreach (var member in conference.Members)
                    {
                        var connection =
                            _connections.FirstOrDefault(x => x.Id == member.ObjectID && member.ObjectID != conference.CreatorId);

                        if (connection != null)
                        {
                            await Clients.Client(connection.ConnectionId).OnNewConference(conference);
                        }
                    }
                }

            }
        }

        public async Task SendVideoRequest(int dialogId, string dialogType)
        {
            var connection = _connections.FirstOrDefault(x => x.ConnectionId == Context.ConnectionId);
            if (connection != null)
            {
                await _sendVideoRequest(dialogId, connection.Id, connection.Id, dialogType);

                switch (dialogType)
                {
                    case "PrivateMessage":
                        await _sendVideoRequest(dialogId, dialogId, connection.Id, dialogType);
                        break;
                    case "PublicMessage":
                        using (var uofw = _unitOfWorkFactory.CreateSystem())
                        {
                            var targets =
                                await _conferenceService.GetAll(uofw)
                                    .Where(x => x.ID == dialogId)
                                    .SelectMany(x => x.Members)
                                    .Where(x => x.ObjectID != connection.Id)
                                    .Select(x => x.ObjectID)
                                    .ToListAsync();

                            if (targets.Any())
                            {
                                foreach (var targetId in targets)
                                {
                                    await _sendVideoRequest(dialogId, targetId.Value, connection.Id, dialogType);
                                }
                            }


                        }
                        break;
                }
            }
        }

        private async Task _sendVideoRequest(int dialogId, int targetId, int requestorId, string dialogType)
        {
            var targetConnection = _connections.FirstOrDefault(x => x.Id == targetId);
            if (targetConnection != null)
            {
                await Clients.Client(targetConnection.ConnectionId).OnSendVideoRequest(dialogId, requestorId, dialogType);
            }
        }


        public async Task SuccessCall(int dialogId, int requestorId, string dialogType)
        {
            var responserConnection = _connections.FirstOrDefault(x => x.ConnectionId == Context.ConnectionId);
            var requestorConnection = _connections.FirstOrDefault(x => x.Id == requestorId);

            if (responserConnection != null && requestorConnection != null)
            {
                await Clients.Client(requestorConnection.ConnectionId).OnCallSuccess(dialogId, responserConnection.Id, dialogType);
                //await Clients.Client(responserConnection.ConnectionId).OnSendVideoRequest(dialogId, requestorId, dialogType);
            }
        }

        public async Task CancelCall(int dialogId, int requestorId, string dialogType)
        {
            var responserConnection = _connections.FirstOrDefault(x => x.ConnectionId == Context.ConnectionId);
            var requestorConnection = _connections.FirstOrDefault(x => x.Id == requestorId);

            if (responserConnection != null && requestorConnection != null)
            {
                await Clients.Client(requestorConnection.ConnectionId).OnCallCancel(dialogId, responserConnection.Id, dialogType);
                //await Clients.Client(responserConnection.ConnectionId).OnSendVideoRequest(dialogId, requestorId, dialogType);
            }
        }


        public async Task StartVideoConference(int userId, string key, string dialogType)
        {
            var targetConnection = _connections.FirstOrDefault(x => x.Id == userId);
            var responserConnection = _connections.FirstOrDefault(x => x.ConnectionId == Context.ConnectionId);

            if (targetConnection != null && responserConnection != null)
            {
                await Clients.Client(targetConnection.ConnectionId).OnStartVideoConference(responserConnection.Id, key, dialogType);
            }
        }

        public async Task ConfirmConnection()
        {
            await Clients.Client(Context.ConnectionId).Confirmed(new { Status = "OK", ServerDate = DateTime.Now });
        }

        public async Task SignIn()
        {
            var user = _securityUserService.GetSecurityUser(Context.User.Identity.Name);

            _connections.Add(new ConferenceUser()
            {
                Id = user.ID,
                UserName = user.UserName,
                ConnectionId = Context.ConnectionId
            });

            await Clients.All.OnSignIn(_connections.ToDictionary(x => x.Id, x => x));
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            var connectionUser = _connections.FirstOrDefault(x => x.ConnectionId == Context.ConnectionId);

            _connections.Remove(connectionUser);

            //var user = _securityUserService.GetSecurityUser(Context.User.Identity.Name);

            //Clients.All.OnSignOut(_connections.ToDictionary(x => x.Id, x => x));
            Clients.AllExcept(Context.ConnectionId).OnSignOut(_connections.ToDictionary(x => x.Id, x => x));

            return base.OnDisconnected(stopCalled);
        }



    }


    public class ConferenceUser
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string ConnectionId { get; set; }
    }
}