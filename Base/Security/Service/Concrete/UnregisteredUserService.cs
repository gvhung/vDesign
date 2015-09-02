using Base.DAL;
using Base.Events;
using Base.Security.Service.Abstract;
using Base.Service;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Base.Security.Service.Concrete
{
    public class UnregisteredUserService : IUnregisteredUserService
    {
        public IQueryable<UnregisteredUser> GetAll(IUnitOfWork unitOfWork, bool? hidden = false)
        {
            throw new NotImplementedException();
        }

        public UnregisteredUser Create(IUnitOfWork unitOfWork, UnregisteredUser obj)
        {
            obj.Email = obj.Email.Trim();

            var repository = unitOfWork.GetRepository<User>();

            if (repository.All().Any(x => x.Email.Equals(obj.Email, StringComparison.InvariantCultureIgnoreCase)))
            {
                throw new Exception("Пользователь с таким адресом электронной почты уже существует. Вы можете найти данный контакт через поиск.");
            }

            var catRepo = unitOfWork.GetRepository<UserCategory>();

            var cat = catRepo.All().FirstOrDefault(x => x.SystemName == "Unregistered") ?? new UserCategory
            {
                Name = "Незарегистрированные пользователи",
                SystemName = "Unregistered"
            };

            var user = new User
            {
                Login = obj.Email,
                FirstName = obj.FirstName,
                LastName = obj.LastName,
                MiddleName = obj.MiddleName,
                Email = obj.Email,
                OfficePhone = obj.OfficePhone,
                PersonPhone = obj.PersonPhone,
                MailAddress = obj.MailAddress,
                UserCategory = cat,
                IsUnregistered = true
            };

            repository.Create(user);

            unitOfWork.SaveChanges();

            obj.ID = user.ID;

            return obj;
        }

        public IList<UnregisteredUser> CreateCollection(IUnitOfWork unitOfWork, IEnumerable<UnregisteredUser> collection)
        {
            throw new NotImplementedException();
        }

        public IList<UnregisteredUser> UpdateCollection(IUnitOfWork unitOfWork, IEnumerable<UnregisteredUser> collection)
        {
            throw new NotImplementedException();
        }

        public void DeleteCollection(IUnitOfWork unitOfWork, IEnumerable<UnregisteredUser> collection)
        {
            throw new NotImplementedException();
        }

        #region Insignificantly

        IQueryable<BaseObject> IBaseObjectCRUDService.GetAll(IUnitOfWork unitOfWork, bool? hidden)
        {
            throw new NotImplementedException();
        }

        public event EventHandler<BaseObjectEventArgs> OnGetAll;

        BaseObject IBaseObjectCRUDService.Get(IUnitOfWork unitOfWork, int id)
        {
            throw new NotImplementedException();
        }

        public event EventHandler<BaseObjectEventArgs> OnGet;

        BaseObject IBaseObjectCRUDService.Create(IUnitOfWork unitOfWork, BaseObject obj)
        {
            return Create(unitOfWork, obj as UnregisteredUser);
        }

        public event EventHandler<BaseObjectEventArgs> OnCreate;

        BaseObject IBaseObjectCRUDService.Update(IUnitOfWork unitOfWork, BaseObject obj)
        {
            throw new NotImplementedException();
        }

        public event EventHandler<BaseObjectEventArgs> OnUpdate;

        void IBaseObjectCRUDService.Delete(IUnitOfWork unitOfWork, BaseObject obj)
        {
            throw new NotImplementedException();
        }

        public event EventHandler<BaseObjectEventArgs> OnDelete;

        void IBaseObjectCRUDService.ChangeSortOrder(IUnitOfWork unitOfWork, BaseObject obj, int newSortOrder)
        {
            throw new NotImplementedException();
        }

        public event EventHandler<BaseObjectEventArgs> OnChangeSortOrder;

        BaseObject IBaseObjectCRUDService.CreateOnGroundsOf(IUnitOfWork unitOfWork, BaseObject obj)
        {
            return new UnregisteredUser();
        }

        public event EventHandler<BaseObjectEventArgs> OnCreateOnGroundsOf;

        public UnregisteredUser Get(IUnitOfWork unitOfWork, int id)
        {
            throw new NotImplementedException();
        }

        public UnregisteredUser Update(IUnitOfWork unitOfWork, UnregisteredUser obj)
        {
            throw new NotImplementedException();
        }

        public void Delete(IUnitOfWork unitOfWork, UnregisteredUser obj)
        {
            throw new NotImplementedException();
        }

        public void ChangeSortOrder(IUnitOfWork unitOfWork, UnregisteredUser obj, int newSortOrder)
        {
            throw new NotImplementedException();
        }

        public UnregisteredUser CreateOnGroundsOf(IUnitOfWork unitOfWork, BaseObject obj)
        {
            return new UnregisteredUser();
        }

        #endregion
    }
}