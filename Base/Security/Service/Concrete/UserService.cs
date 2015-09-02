using Base.DAL;
using Base.Service;
using System.Linq;

namespace Base.Security.Service
{
    public class UserService : BaseCategorizedItemService<User>, IUserService
    {
        private readonly ISecurityUserService _securityUserService;

        public UserService(IBaseObjectServiceFacade facade, ISecurityUserService securityUserService)
            : base(facade)
        {
            _securityUserService = securityUserService;
        }

        public override IQueryable<User> GetAllCategorizedItems(IUnitOfWork unitOfWork, int categoryID, bool? hidden = false)
        {
            string strID = HCategory.IdToString(categoryID);
            return GetAll(unitOfWork, hidden).Cast<User>().Where(a => (a.UserCategory.sys_all_parents != null && a.UserCategory.sys_all_parents.Contains(strID)) || a.UserCategory.ID == categoryID);
        }

        protected override IObjectSaver<User> GetForSave(IUnitOfWork unitOfWork, IObjectSaver<User> objectSaver)
        {
            return base.GetForSave(unitOfWork, objectSaver)
                .SaveManyToMany(x => x.Roles)
                .SaveOneToMany(x => x.Friends, x => x.SaveOneObject(o => o.Object))
                .SaveOneObject(x => x.Image);
        }

        public override User Update(IUnitOfWork unitOfWork, User obj)
        {
            var userDb = unitOfWork.GetRepository<User>().Find(obj.ID);

            if (userDb != null)
                _securityUserService.Clear(userDb.Login);
            
            return base.Update(unitOfWork, obj);
        }

        public override void Delete(IUnitOfWork unitOfWork, User obj)
        {
            if (obj != null)
                _securityUserService.Clear(obj.Login);

            base.Delete(unitOfWork, obj);
        }

        public void AddToFriends(ISecurityUser securityUser, int id)
        {
            using (var uofw = UnitOfWorkFactory.CreateSystem())
            {
                var repository = uofw.GetRepository<UserFriend>();

                if (repository.All().Any(x => x.User_ID == securityUser.ID && x.ObjectID == id)) return;

                repository.Create(new UserFriend
                {
                    User_ID = securityUser.ID,
                    ObjectID = id
                });

                uofw.SaveChanges();
            }
        }

        public void RemoveFromFriends(ISecurityUser securityUser, int id)
        {
            using (var uofw = UnitOfWorkFactory.CreateSystem())
            {
                var repository = uofw.GetRepository<UserFriend>();

                var friend = repository.All().FirstOrDefault(x => x.User_ID == securityUser.ID && x.ObjectID == id);

                if (friend == null) return;

                repository.Delete(friend);

                uofw.SaveChanges();
            }
        }
    }
}
