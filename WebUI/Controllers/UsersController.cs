using Base;
using Base.Ambient;
using Base.DAL;
using Base.QueryableExtensions;
using Base.Security;
using Base.Security.Service;
using Framework;
using Framework.FullTextSearch;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Routing;
using WebUI.Authorize;

namespace WebUI.Controllers
{
    public class UsersController : BaseController
    {
        private const int s_count = 10;
        private readonly IUserService _userService;
        private readonly IUserCategoryService _userCategoryService;
        private CancellationTokenSource _tokenSource;

        public UsersController(
            IBaseControllerServiceFacade serviceFacade,
            IUserService userService, 
            IUserCategoryService userCategoryService)
            : base(serviceFacade)
        {
            _userService = userService;
            _userCategoryService = userCategoryService;
        }

        protected override void Initialize(RequestContext requestContext)
        {
            base.Initialize(requestContext);

            _tokenSource = CancellationTokenSource.CreateLinkedTokenSource(
                Response.ClientDisconnectedToken, Request.TimedOutToken);
        }

        public async Task<ActionResult> Search(string q)
        {
            using (var uofw = this.CreateUnitOfWork())
            {
                var friendsIDs =
                    _userService.Get(uofw, SecurityUser.ID).Friends.Select(x => x.ObjectID).ToList();

                var allWithMore =
                    (await LookupUsers(uofw, q, s_count, friendsIDs)).GroupBy(x => x.IsFriend).ToList();

                return JsonNet(new
                {
                    friends = allWithMore.Where(x => x.Key).SelectMany(x => x),
                    users = allWithMore.Where(x => !x.Key).SelectMany(x => x),
                });
            }
        }

        private Task<List<UserLookup>> LookupUsers(IUnitOfWork unitOfWork, string q, int count, List<int?> friendsIDs)
        {
            var query = _userService.GetAll(unitOfWork);

            if (!SecurityUser.IsAdmin)
            {
                var categories = _userCategoryService.GetAll(unitOfWork).Select(x => x.ID).ToList();

                query = query.Where(x => categories.Contains(x.CategoryID));
            }

            var selectorQuery = query.FullTextSearch(q, CacheWrapper)
                .Select(
                    x =>
                        new UserLookup
                        {
                            ID = x.ID,
                            FullName = x.FullName,
                            Image = x.Image,
                            Email = x.Email,
                            //InternalPhone = x.InternalPhone,
                            IsFriend = friendsIDs.Contains(x.ID)
                        });

            if (string.IsNullOrEmpty(q))
                selectorQuery = selectorQuery.Where(x => x.IsFriend);

            var resultQuery = selectorQuery.OrderByDescending(x => x.IsFriend)
                .ThenBy(x => x.FullName)
                .Take(count);

            return resultQuery.ToListAsync(_tokenSource.Token);
        }

        public async Task<ActionResult> Filter_Read(string startswith, string ids)
        {
            using (var uofw = this.CreateUnitOfWork())
            {
                var friendsIDs =
                    _userService.Get(uofw, SecurityUser.ID).Friends.Select(x => x.ObjectID).ToList();

                IEnumerable<UserLookup> result =
                    await LookupUsers(uofw, startswith, s_count, friendsIDs);

                if (!string.IsNullOrEmpty(ids))
                {
                    int tmp;
                    var idsArray = ids.Split(';').Select(x => int.TryParse(x, out tmp) ? tmp : 0).ToArray();

                    var additional = await _userService.GetAll(uofw)
                        .Where(x => idsArray.Contains(x.ID)).Select(
                            x =>
                                new UserLookup
                                {
                                    ID = x.ID,
                                    FullName = x.FullName,
                                    Image = x.Image,
                                    Email = x.Email,
                                    //InternalPhone = x.InternalPhone,
                                    IsFriend = friendsIDs.Contains(x.ID)
                                })
                        .ToListAsync(_tokenSource.Token);

                    result = result.Union(additional);
                }

                return JsonNet(result);
            }
        }

        public ActionResult Add(int id)
        {
            _userService.AddToFriends(SecurityUser, id);

            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }

        public ActionResult Remove(int id)
        {
            _userService.RemoveFromFriends(SecurityUser, id);

            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }

        [AllowGuest]
        public ActionResult GetUser(int id)
        {
            using (var uof = this.CreateUnitOfWork())
            {
                //TODO: ++фильтр
                var user = _userService.Get(uof, id);

                if (user != null)
                {
                    if (AppContext.SecurityUser.IsGuest)
                        return new JsonNetResult(new
                        {
                            model = new
                            {
                                ID = user.ID,
                                Image = user.Image,
                                UserCategoryName = user.UserCategoryName,
                                Post = user.Post,
                                FullName = user.FullName,
                            }
                        });
                    else
                        return new JsonNetResult(new
                        {
                            model = new
                            {
                                ID = user.ID,
                                Image = user.Image,
                                UserCategoryName = user.UserCategoryName,
                                Post = user.Post,
                                Email = user.Email,
                                OfficePhone = user.OfficePhone,
                                PersonPhone = user.PersonPhone,
                                FullName = user.FullName,
                            }
                        });

                }

                return new JsonNetResult(new
                {
                    error = "Пользователь не найден"
                });

            }
        }

        private class UserLookup
        {
            public int ID { get; set; }
            public string FullName { get; set; }
            public FileData Image { get; set; }
            public string Email { get; set; }
            public string InternalPhone { get; set; }
            public bool IsFriend { get; set; }

            protected bool Equals(UserLookup other)
            {
                return ID == other.ID;
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != this.GetType()) return false;
                return Equals((UserLookup)obj);
            }

            public override int GetHashCode()
            {
                return ID;
            }
        }
    }
}