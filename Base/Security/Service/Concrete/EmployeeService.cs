using Base.DAL;
using Base.Events;
using Base.Service;
using System;
using System.Linq;
using System.Threading;

namespace Base.Security.Service
{
    public class EmployeeService : BaseCategorizedItemService<Employee>, IEmployeeService
    {
        private readonly ISecurityUserService _securityUserService;
        private readonly IUserService _userService;

        public EmployeeService(ISecurityUserService securityUserService, IBaseObjectServiceFacade facade, IUserService userService)
            : base(facade)
        {
            _securityUserService = securityUserService;
            _userService = userService;
        }

        private IQueryable<User> GetAllUsers(IUnitOfWork unitOfWork, bool? hidden = false)
        {
            this.SecurityService.ThrowIfAccessDenied(unitOfWork, typeof(Employee), TypePermission.Read);

            IQueryable<User> q = unitOfWork.GetRepository<User>().All().OrderBy(x => x.SortOrder);

            if (hidden == null) return q;

            if ((bool)hidden)
                q = q.Where(a => a.Hidden);
            else
                q = q.Where(a => a.Hidden == false);

            return q;
        }

        public override IQueryable<Employee> GetAll(IUnitOfWork unitOfWork, bool? hidden = false)
        {
            return this.GetAllUsers(unitOfWork, hidden).ToList().Select(x => new Employee(x)).AsQueryable();
        }

        public override IQueryable<Employee> GetCategorizedItems(IUnitOfWork unitOfWork, int categoryID, bool? hidden)
        {
            return this.GetAllUsers(unitOfWork, hidden).Where(a => a.CategoryID == categoryID).ToList().Select(x => new Employee(x)).AsQueryable();
        }

        public override IQueryable<Employee> GetAllCategorizedItems(IUnitOfWork unitOfWork, int categoryID, bool? hidden = false)
        {
            string strID = HCategory.IdToString(categoryID);
            return this.GetAllUsers(unitOfWork, hidden).Where(a => (a.UserCategory.sys_all_parents != null && a.UserCategory.sys_all_parents.Contains(strID)) || a.UserCategory.ID == categoryID).ToList().Select(x => new Employee(x)).AsQueryable();
        }

        public override Employee Get(IUnitOfWork unitOfWork, int id)
        {
            this.SecurityService.ThrowIfAccessDenied(unitOfWork, typeof(Employee), TypePermission.Read);

            var userdb = unitOfWork.GetRepository<User>().All().FirstOrDefault(m => m.ID == id);

            if (userdb != null)
                return new Employee(userdb);
            else
                throw new Exception("Пользователь не найден.");
        }

        public override Employee Create(IUnitOfWork unitOfWork, Employee obj)
        {
            this.SecurityService.ThrowIfAccessDenied(unitOfWork, typeof(Employee), TypePermission.Create);

            string login = obj.Email;

            if (unitOfWork.GetRepository<User>().All().Any(u => u.Login.ToUpper() == login.ToUpper()))
            {
                throw new Exception("Не пройдена проверка уникальности e-mail");
            }

            var repository = unitOfWork.GetRepository<User>();

            var user = obj.ToObject<User>();
            user.Login = login;
            user.IsActive = false;
            user.Image = obj.Image;
            user.Post = obj.Post;
            user.SortOrder = (repository.All().Max(m => (int?)m.SortOrder) ?? 0) + 1;

            var userNew = unitOfWork.GetObjectSaver(user, null)
                .SaveOneObject(x => x.Image)
                .SaveOneObject(x => x.Post).Dest;

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

            return new Employee(userNew);
        }

        public override Employee Update(IUnitOfWork unitOfWork, Employee obj)
        {
            this.SecurityService.ThrowIfAccessDenied(unitOfWork, typeof(Employee), TypePermission.Write);

            var repository = unitOfWork.GetRepository<User>();

            var userDb = repository.Find(obj.ID);

            var user = userDb.ToObject<User>();

            obj.ToObject(user, systemProperties: false);
            user.SortOrder = obj.SortOrder;
            user.Image = obj.Image;
            user.Post = obj.Post;

            _securityUserService.Clear(userDb.Login);
            _securityUserService.ValidateLogin(unitOfWork, user, userDb);

            var userUpdate = unitOfWork.GetObjectSaver(user, userDb).SaveOneObject(x => x.Image).SaveOneObject(x => x.Post).Dest;

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

            return new Employee(userUpdate);
        }

        public override void Delete(IUnitOfWork unitOfWork, Employee obj)
        {
            //TODO: !!!
            throw new Exception("Отказано в доступе.");
        }

        public override void ChangeSortOrder(IUnitOfWork unitOfWork, Employee obj, int newOrder)
        {
            var repository = unitOfWork.GetRepository<User>();

            var userDb = repository.Find(obj.ID);

            if (userDb != null)
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
