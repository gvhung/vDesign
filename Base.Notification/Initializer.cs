using Base.DAL;
using Base.Notification.Entities;
using Base.Settings;
using System.Linq;

namespace Base.Notification
{
    public interface INotificationInitializer: IBaseInitializer
    {}

    public class Initializer : INotificationInitializer
    {
        private readonly ISettingItemService _settingItemService;
        private readonly ISettingCategoryService _settingCategoryService;

        public Initializer(ISettingItemService settingItemService, ISettingCategoryService settingCategoryService)
        {
            _settingItemService = settingItemService;
            _settingCategoryService = settingCategoryService;
        }

        public void Init(IUnitOfWork unitOfWork)
        {   
            if (!(_settingItemService).GetAll(unitOfWork).Any(x => x.Key == Consts.Settings.KEY_CONFIG))
            {
                _settingItemService.Create(unitOfWork, new SettingItem()
                {
                    CategoryID = _settingCategoryService.GetAll(unitOfWork).Where(x => x.SysName == "main").Select(x => x.ID).FirstOrDefault(),
                    Key = Consts.Settings.KEY_CONFIG,
                    Text = "Уведомления",
                    Value = new Setting(new EmailConfig()
                    {
                        EnableEmail = false,
                        SmtpServerAddress = "mail.pba.su",
                        SmtpServerPort = 25,
                        UseSsl = false,
                        AccountLogin = "erp@pba.su",
                        AccountPassword = "92990aJe4",
                        AccountTitle = "ERP",
                        Delay = 30
                    })
                });
            }
        }
    }
}
