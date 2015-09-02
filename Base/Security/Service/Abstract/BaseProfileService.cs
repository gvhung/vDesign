using Base.Ambient;
using Base.DAL;
using Base.Events;
using Base.Service;
using System;
using System.Linq;
using System.Threading;

namespace Base.Security.Service.Abstract
{
    public abstract class BaseProfileService<T> : BaseObjectService<T> where T : Profile
    {
        private readonly ISecurityUserService _securityUserService;

        public BaseProfileService(ISecurityUserService securityUserService, IBaseObjectServiceFacade facade)
            : base(facade)
        {
            _securityUserService = securityUserService;
        }

        public override T Get(IUnitOfWork unitOfWork, int id)
        {
            if (AppContext.SecurityUser.ID != id)
                throw new Exception("Отказано в доступе.");

            var userdb = unitOfWork.GetRepository<User>().All().FirstOrDefault(m => m.ID == id);

            if (userdb != null)
                //return new Profile(userdb);
                return (T)Activator.CreateInstance(typeof(T), userdb);
            else
                throw new Exception("Пользователь не найден.");
        }

        public override T Update(IUnitOfWork unitOfWork, T obj)
        {
            if (AppContext.SecurityUser.ID != obj.ID)
                throw new Exception("Отказано в доступе.");

            var repository = unitOfWork.GetRepository<User>();

            var userDb = repository.Find(obj.ID);

            var user = userDb.ToObject<User>();

            obj.ToObject(user, systemProperties: false);
            user.Image = obj.Image;

            user.UserType = userDb.UserType;

            _securityUserService.Clear(userDb.Login);
            _securityUserService.ValidateLogin(unitOfWork, user, userDb);

            var userUpdate = unitOfWork.GetObjectSaver(user, userDb).SaveOneObject(x => x.Image).Dest;

            repository.Update(userUpdate);

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

            return (T)Activator.CreateInstance(typeof(T), userUpdate);
        }

        public override event EventHandler<BaseObjectEventArgs> OnUpdate;
    }

}
