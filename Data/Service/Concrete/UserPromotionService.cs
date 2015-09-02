using Base;
using Base.Ambient;
using Base.BusinessProcesses.Entities;
using Base.DAL;
using Base.Notification.Service.Abstract;
using Base.Security;
using Base.Security.Service;
using Base.Service;
using Data.Entities;
using Data.Service.Abstract;
using Framework;
using System;
using System.Linq;

namespace Data.Service.Concrete
{
    public class UserPromotionService : BaseObjectService<UserPromotion>, IUserPromotionService
    {
        private readonly IUserCategoryService _categoryService;
        private readonly IRoleService _roleService;
        private readonly IManagerNotification _managerNotification;

        public UserPromotionService(IBaseObjectServiceFacade facade, IUserCategoryService categoryService, IRoleService roleService, IManagerNotification managerNotification)
            : base(facade)
        {
            _categoryService = categoryService;
            _roleService = roleService;
            _managerNotification = managerNotification;
        }

        public override UserPromotion Create(IUnitOfWork unitOfWork, UserPromotion obj)
        {
            if (this.GetAll(unitOfWork).Any(x => x.UserID == AppContext.SecurityUser.ID && x.State == PromotionState.New))
            {
                throw new InvalidOperationException("Ваша предыдущая заявка находится на рассмотрении. Дождитесь решения.");
            }

            obj.UserID = AppContext.SecurityUser.ID;
            obj.SubmitTime = DateTime.Now;

            User user = unitOfWork.GetRepository<User>().Find(x => x.ID == obj.UserID);
            user.ChangeTypePending = true;

            return base.Create(unitOfWork, obj);
        }

        public override UserPromotion Update(IUnitOfWork unitOfWork, UserPromotion obj)
        {
            if(obj.State != PromotionState.New)
            {
                if (obj.State == PromotionState.Approve)
                {
                    obj.User.UserType = obj.RequestedType;

                    string name = obj.RequestedType.GetDescription();
                    string systemName = obj.RequestedType.ToString();

                    var cat = _categoryService.GetAll(unitOfWork).FirstOrDefault(x => x.SystemName == systemName);
                    if (cat == null)
                        throw new Exception("Отсутствует категория для пользователей " + obj.RequestedType.GetDescription());

                    obj.User.CategoryID = cat.ID;

                    Role role = _roleService.GetRoleByUserType(unitOfWork, obj.RequestedType);

                    if (role == null)
                        throw new Exception("Отсутствует роль для пользователей " + obj.RequestedType.GetDescription());

                    obj.User.Roles.Clear();
                    obj.User.Roles.Add(role);
                }

                obj.User.ChangeTypePending = false;

                _managerNotification.CreateNotice(unitOfWork, obj, BaseEntityState.Modified);
            }

            return base.Update(unitOfWork, obj);
        }

        protected override IObjectSaver<UserPromotion> GetForSave(IUnitOfWork unitOfWork, IObjectSaver<UserPromotion> objectSaver)
        {
            return objectSaver.SaveOneObject(x => x.Department);
        }

        public void BeforeInvoke(BaseObject obj)
        {
        }

        public void OnActionExecuting(ActionExecuteArgs args)
        {
        }

        public int GetWorkflowID(IUnitOfWork unitOfWork, BaseObject obj)
        {
            return Workflow.Default;
        }
    }
}
