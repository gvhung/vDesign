using Base.DAL;
using Base.Security.Service.Abstract;
using System;
using System.Linq;

namespace Base.Security.Service.Concrete
{
    public class GuestUserService : IGuestUserService
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly IGuestRoleProvider _guestRoleProvider;
        private readonly IUserCategoryService _userCategoryService;
        
        public GuestUserService(
            IGuestRoleProvider guestRoleProvider, IUnitOfWorkFactory unitOfWorkFactory, IUserCategoryService userCategoryService)
        {
            _guestRoleProvider = guestRoleProvider;
            _unitOfWorkFactory = unitOfWorkFactory;
            _userCategoryService = userCategoryService;
        }

        private static readonly FileData s_image = new FileData
        {
            FileID = Guid.Parse("8a2ff9f8-7bb9-43e8-b171-f7760e4529a1"),
        };

        public void CreateGuestUser(string login)
        {
            using (var uofw = _unitOfWorkFactory.CreateSystem())
            {
                var guestsCat = _userCategoryService.GetAll(uofw)
                    .FirstOrDefault(x => x.SystemName == "Guests");

                var user = new User
                {
                    Login = login,
                    Hidden = true,
                    LastName = "Гость",
                    IsUnregistered = true,
                };

                if (guestsCat != null)
                {
                    user.CategoryID = guestsCat.ID;
                }
                else
                {
                    user.UserCategory = new UserCategory
                    {
                        Name = "Гости",
                        SystemName = "Guests"
                    };
                }

                uofw.GetRepository<User>().Create(user);

                uofw.SaveChanges();
            }
        }

        public ISecurityUser GetGuestUser(string login)
        {
            return new SecurityUser(new User()
            {
                ID = -1,
                Image = s_image,
                Login = login ?? Guid.NewGuid().ToString(),
                Hidden = true,
                LastName = "Гость",
                IsUnregistered = true,
                Roles = _guestRoleProvider.GetRoles(),
            })
            {
                IsGuest = true
            };
        }
    }
}