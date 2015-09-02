using Base.Ambient;
using Base.DAL;
using Base.Events;
using Base.Service;
using Base.Settings;
using Framework;
using System;
using System.Linq;
using System.Threading;

namespace Base.Security.Service
{
    public class AccessUserService : BaseCategorizedItemService<AccessUser>, IAccessUserService
    {
        private readonly ISettingItemService _settingsService;
        private readonly ISecurityUserService _securityUserService;
        private readonly IUserService _userService;

        public AccessUserService(ISettingItemService settingsService, ISecurityUserService securityUserService, IBaseObjectServiceFacade facade, IUserService userService)
            : base(facade)
        {
            _settingsService = settingsService;
            _securityUserService = securityUserService;
            _userService = userService;
        }
        private IQueryable<User> GetAllUsers(IUnitOfWork unitOfWork, bool? hidden = false)
        {
            if (!AppContext.SecurityUser.IsAdmin)
                throw new Exception("Отказано в доступе.");

            IQueryable<User> q = unitOfWork.GetRepository<User>().All().OrderBy(x => x.SortOrder);

            if (hidden == null) return q;

            if ((bool)hidden)
                q = q.Where(a => a.Hidden);
            else
                q = q.Where(a => a.Hidden == false);

            return q;
        }

        public override IQueryable<AccessUser> GetAll(IUnitOfWork unitOfWork, bool? hidden = false)
        {
            return this.GetAllUsers(unitOfWork, hidden).ToList().Select(x => new AccessUser(x)).AsQueryable();
        }

        public override IQueryable<AccessUser> GetCategorizedItems(IUnitOfWork unitOfWork, int categoryID, bool? hidden)
        {
            return this.GetAllUsers(unitOfWork, hidden).Where(a => a.CategoryID == categoryID).ToList().Select(x => new AccessUser(x)).AsQueryable();
        }

        public override IQueryable<AccessUser> GetAllCategorizedItems(IUnitOfWork unitOfWork, int categoryID, bool? hidden = false)
        {
            string strId = HCategory.IdToString(categoryID);

            return this.GetAllUsers(unitOfWork, hidden).Where(a => (a.UserCategory.sys_all_parents != null && a.UserCategory.sys_all_parents.Contains(strId)) || a.UserCategory.ID == categoryID).ToList().Select(x => new AccessUser(x)).AsQueryable();
        }

        public override AccessUser Get(IUnitOfWork unitOfWork, int id)
        {
            if (!AppContext.SecurityUser.IsAdmin)
                throw new Exception("Отказано в доступе.");

            var userdb = unitOfWork.GetRepository<User>().All().FirstOrDefault(m => m.ID == id);

            if (userdb != null)
                return new AccessUser(userdb);
            else
                throw new Exception("Пользователь не найден.");
        }

        public override AccessUser Create(IUnitOfWork unitOfWork, AccessUser obj)
        {
            if (!AppContext.SecurityUser.IsAdmin)
                throw new Exception("Отказано в доступе.");

            var repository = unitOfWork.GetRepository<User>();

            var user = obj.ToObject<User>();

            user.Password = obj.Password;
            user.Image = obj.Image;
            user.Post = obj.Post;
            user.Department = obj.Department;
            user.Roles = obj.Roles;
            user.SortOrder = (repository.All().Max(m => (int?)m.SortOrder) ?? 0) + 1;

            _securityUserService.ValidateLogin(unitOfWork, user, user);

            int minLenPassword = 6;
            bool passwordCheckKeyboard = false;
            string validationMessage = "";

            var config = _settingsService.GetValue(Consts.KEY_CONFIG, null) as Config;

            if (config != null)
            {
                minLenPassword = config.MinLenPassword;
                passwordCheckKeyboard = config.PasswordCheckKeyboard;
            }

            if (obj.Password == null || !PasswordValidator.Check(obj.Password, out validationMessage, minLenPassword, 2, passwordCheckKeyboard))
            {
                throw new Exception(validationMessage);
            }

            var passwordCryptographer = new PasswordCryptographer();
            user.Password = passwordCryptographer.GenerateSaltedPassword(user.Password);

            var userNew = unitOfWork.GetObjectSaver(user, null)
                .SaveOneObject(x => x.Image)
                .SaveOneObject(x => x.Post)
                .SaveOneObject(x => x.Department)
                .SaveManyToMany(x => x.Roles).Dest;

            repository.Create(userNew);

            unitOfWork.SaveChanges();

            var eventHandler = Volatile.Read(ref this.OnCreate);

            if (eventHandler != null)
            {
                eventHandler(this, new BaseObjectEventArgs()
                {
                    Type = TypeEvent.OnCreate,
                    Object = userNew,
                    UnitOfWork = unitOfWork
                });
            }

            return new AccessUser(userNew);
        }

        public override AccessUser Update(IUnitOfWork unitOfWork, AccessUser obj)
        {
            if (!AppContext.SecurityUser.IsAdmin)
                throw new Exception("Отказано в доступе.");

            var repository = unitOfWork.GetRepository<User>();

            var userDb = repository.Find(obj.ID);

            var user = userDb.ToObject<User>();

            obj.ToObject(user, systemProperties: false);

            user.SortOrder = obj.SortOrder;
            user.Image = obj.Image;
            user.Post = obj.Post;
            user.Department = obj.Department;
            user.Roles = obj.Roles;

            _securityUserService.Clear(userDb.Login);
            _securityUserService.ValidateLogin(unitOfWork, user, userDb);

            var userUpdate = unitOfWork
                .GetObjectSaver(user, userDb)
                .SaveOneObject(x => x.Image)
                .SaveOneObject(x => x.Post)
                .SaveOneObject(x => x.Department)
                .SaveManyToMany(x => x.Roles).Dest;

            repository.Update(userUpdate);

            unitOfWork.SaveChanges();

            var eventHandler = Volatile.Read(ref this.OnUpdate);

            if (eventHandler != null)
            {
                eventHandler(this, new BaseObjectEventArgs()
                {
                    Type = TypeEvent.OnUpdate,
                    Object = userUpdate,
                    UnitOfWork = unitOfWork
                });
            }

            return new AccessUser(userUpdate);
        }

        public override void Delete(IUnitOfWork unitOfWork, AccessUser obj)
        {
            if (!AppContext.SecurityUser.IsAdmin)
                throw new Exception("Отказано в доступе.");

            var repository = unitOfWork.GetRepository<User>();

            var userDb = repository.Find(obj.ID);

            if (userDb == null) return;

            userDb.Hidden = true;
            
            _securityUserService.Clear(userDb.Login);

            repository.Update(userDb);

            unitOfWork.SaveChanges();

            var eventHandler = Volatile.Read(ref this.OnDelete);

            if (eventHandler != null)
            {
                eventHandler(this, new BaseObjectEventArgs()
                {
                    Type = TypeEvent.OnDelete,
                    Object = userDb,
                    UnitOfWork = unitOfWork
                });
            }
        }

        public override void ChangeSortOrder(IUnitOfWork unitOfWork, AccessUser obj, int newOrder)
        {
            var repository = unitOfWork.GetRepository<User>();

            var userDb = repository.Find(obj.ID);

            if(userDb != null)
                _userService.ChangeSortOrder(unitOfWork, userDb, newOrder);
        }

        public override void ChangeCategory(IUnitOfWork unitOfWork, int id, int newCategoryID)
        {
            _userService.ChangeCategory(unitOfWork, id, newCategoryID);
        }

        public override event EventHandler<BaseObjectEventArgs> OnCreate;
        public override event EventHandler<BaseObjectEventArgs> OnUpdate;
        public override event EventHandler<BaseObjectEventArgs> OnDelete;
    }
}
