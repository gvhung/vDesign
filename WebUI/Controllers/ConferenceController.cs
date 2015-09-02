using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Async;
using Base.Ambient;
using Base.Conference.Entities;
using Base.Conference.Service;
using Base.QueryableExtensions;
using Base.Security;
using Base.Security.Service;
using Framework;
using WebUI.Models;
using QueryableExtensions = System.Data.Entity.QueryableExtensions;

namespace WebUI.Controllers
{
    public class ConferenceController : BaseController
    {

        private readonly IUserService _userService;
        private readonly IConferenceMessageService _conferenceMessageService;
        private readonly IPrivateMessageService _privateMessageService;
        private readonly IPublicMessageService _publicMessageService;
        private readonly IConferenceService _conferenceService;

        public ConferenceController(IBaseControllerServiceFacade serviceFacade, IUserService userService, IPrivateMessageService privateMessageService, IConferenceService conferenceService, IPublicMessageService publicMessageService, IConferenceMessageService conferenceMessageService) : base(serviceFacade)
        {
            _userService = userService;
            _privateMessageService = privateMessageService;
            _conferenceService = conferenceService;
            _publicMessageService = publicMessageService;
            _conferenceMessageService = conferenceMessageService;
        }

        public ActionResult Index()
        {
            var model = new BaseViewModel(this);

            return View(model);
        }


        public async Task<JsonNetResult> GetUsers()
        {
            List<User> users;

            using (var uofw = CreateSystemUnitOfWork())
            {
                users = await _userService.GetAll(uofw).ToGenericListAsync();
            }

            return new JsonNetResult(users);
        }

        public async Task<JsonNetResult> GetConferences()
        {
            List<Conference> conferences;

            using (var uofw = CreateSystemUnitOfWork())
            {
                conferences =
                    await
                        _conferenceService.GetAll(uofw)
                            .Where(x => x.Members.Select(y => y.ObjectID).Contains(AppContext.SecurityUser.ID))
                            .ToGenericListAsync();
            }

            return new JsonNetResult(conferences);
        }


        public async Task<JsonNetResult> CreateConference(Conference conference)
        {
            conference.CreateDate = DateTime.Now;
            conference.CreatorId = AppContext.SecurityUser.ID;

            using (var uofw = CreateSystemUnitOfWork())
            {
                var user = _userService.Get(uofw, AppContext.SecurityUser.ID);
                
                conference.Members.Add(new ConferenceMember()
                {
                    Object = user,
                    ObjectID = user.ID
                });

                _conferenceService.Create(uofw, conference);

                await uofw.SaveChangesAsync();
            }

            return new JsonNetResult(conference);
        }

        public async Task<JsonNetResult> GetMessages(int id)
        {
            List<PrivateMessage> messages;

            using (var uofw = CreateSystemUnitOfWork())
            {
                var data = _privateMessageService.GetAll(uofw)
                            .Where(x => (x.FromId == id && x.ToUserId == AppContext.SecurityUser.ID) || (x.FromId == AppContext.SecurityUser.ID && x.ToUserId == id))
                            .OrderBy(x => x.ID);

                var countNew = await data.Where(x => x.ToUserId == AppContext.SecurityUser.ID && x.IsNew).CountAsync();

                if (countNew > 50)
                {
                    var firstNew = await QueryableExtensions.MinAsync(data.Where(x => x.IsNew));

                    messages = await data.Where(x => x.ID >= firstNew.ID).ToGenericListAsync();
                }
                else
                {
                    messages =
                        await data.OrderByDescending(x => x.ID).Take(50).OrderBy(x => x.ID).ToGenericListAsync();
                }

            }

            return new JsonNetResult(messages);
        }


        public async Task<JsonNetResult> GetConferenceMessages(int id)
        {
            List<PublicMessage> messages;

            using (var uofw = CreateSystemUnitOfWork())
            {
                var data = _publicMessageService.GetAll(uofw)
                            .Where(x => x.ToConferenceId == id)
                            .OrderBy(x => x.ID);

                var countNew = await data.Where(x => x.IsNew).CountAsync();

                if (countNew > 50)
                {
                    var firstNew = await QueryableExtensions.MinAsync(data.Where(x => x.IsNew));

                    messages = await data.Where(x => x.ID >= firstNew.ID).ToGenericListAsync();
                }
                else
                {
                    messages =
                        await data.OrderByDescending(x => x.ID).Take(50).OrderBy(x => x.ID).ToGenericListAsync();
                }

            }

            return new JsonNetResult(messages);

        }

        public async Task<JsonNetResult> UnreadMessages()
        {
            List<UnreadMessage> messages;

            using (var uofw = CreateSystemUnitOfWork())
            {
                var privatedata = _privateMessageService.GetAll(uofw).Where(x => x.ToUserId == AppContext.SecurityUser.ID && x.IsNew);

                var conferences = await _conferenceService.GetAll(uofw)
                        .Where(x => x.Members.Select(y => y.ObjectID).Contains(AppContext.SecurityUser.ID)).Select(x => x.ID)
                        .ToListAsync() as List<int>;

                var publicdata = _publicMessageService.GetAll(uofw)
                    .Where(x => x.ToConferenceId.HasValue && conferences.Contains(x.ToConferenceId.Value) && x.IsNew && x.FromId != AppContext.SecurityUser.ID);

                messages = await privatedata.GroupBy(x => x.FromId).Select(x => new UnreadMessage()
                {
                    DialogId = x.Key,
                    Count = x.Count(),
                    Type = "PrivateMessage"
                }).Union(publicdata.GroupBy(x => x.ToConferenceId).Select(x => new UnreadMessage()
                {
                    DialogId = x.Key.Value,
                    Count = x.Count(),
                    Type = "PublicMessage"
                })).ToGenericListAsync();
            }

            return new JsonNetResult(messages.ToDictionary(x => x.DialogId + "_" + x.Type, x => x));
        }

        public async Task<JsonNetResult> ReadMessages(int[] messages)
        {
            using (var uofw = CreateSystemUnitOfWork())
            {
                var data =
                    await _conferenceMessageService.GetAll(uofw).Where(x => messages.Contains(x.ID)).ToGenericListAsync();
                data.ForEach(x =>
                {
                    x.IsNew = false;
                    _conferenceMessageService.Update(uofw, x);
                });

                await uofw.SaveChangesAsync();
            }

            return new JsonNetResult(new { Status = "OK" });
        }

    }

    public class UnreadMessage
    {
        public int DialogId { get; set; }
        public int Count { get; set; }
        public string Type { get; set; }
    }
}