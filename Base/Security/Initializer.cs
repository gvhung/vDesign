using Base.DAL;
using Base.Security.Service;
using Base.Settings;
using Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Base.Security
{
    public interface ISecurityInitializer : IBaseInitializer { }

    public class Initializer : ISecurityInitializer
    {
        private readonly IUserService _userService;
        private readonly IUserCategoryService _userCategoryService;
        private readonly IRoleService _roleService;
        private readonly ISettingItemService _settingItemService;
        private readonly ISettingCategoryService _settingCategoryService;

        public Initializer(IUserService userService, IUserCategoryService userCategoryService, ISettingItemService settingItemService, IRoleService roleService, ISettingCategoryService settingCategoryService)
        {
            _userService = userService;
            _userCategoryService = userCategoryService;
            _settingItemService = settingItemService;
            _roleService = roleService;
            _settingCategoryService = settingCategoryService;
        }

        public void Init(IUnitOfWork uofw)
        {
            if (!_userService.GetAll(uofw).Any(x => x.Login == "Administrator"))
            {
                var passwordCryptographer = new PasswordCryptographer();

                var adminCategory = _userCategoryService.Create(uofw, new UserCategory
                {
                    Name = "Администраторы"
                });

                var categories = Enum.GetValues(typeof(UserType)).Cast<UserType>()
                    .Select(x => new UserCategory() { Name = x.GetDescription(), SystemName = x.ToString() });

                _userCategoryService.CreateCollection(uofw, categories);

                var roles = Enum.GetValues(typeof(SystemRole)).Cast<SystemRole>()
                    .Select(x => new Role() { Name = x.GetDescription(), SystemRole = x });

                roles = _roleService.CreateCollection(uofw, roles);

                var user = new User
                {
                    CategoryID = adminCategory.ID,
                    Login = "Administrator",
                    Email = "admin@pba.su",
                    LastName = "Администратор",
                    Password = passwordCryptographer.GenerateSaltedPassword("!QAZ2wsx"),
                    Roles = new List<Role>()
                };

                user.Roles.Add(roles.FirstOrDefault(x => x.SystemRole == SystemRole.Admin));

                _userService.Create(uofw, user);
            }

            if (!(_settingItemService).GetAll(uofw, hidden: null).Any(x => x.Key == Consts.KEY_CONFIG))
            {
                _settingItemService.Create(uofw, new SettingItem()
                {
                    CategoryID = _settingCategoryService.GetAll(uofw).Where(x => x.SysName == "main").Select(x => x.ID).FirstOrDefault(),
                    Key = Consts.KEY_CONFIG,
                    Text = "Безопасность",
                    Value = new Setting(new Config()
                    {
                        MinLenLogin = 6,
                        MinLenPassword = 6,
                        PasswordCheckKeyboard = false,
                        AllowRegistration = true
                    })
                });
            }
        }
    }
}
