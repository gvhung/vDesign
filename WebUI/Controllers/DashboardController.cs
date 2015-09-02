using Base.Content.Entities;
using Base.Content.Service.Abstract;
using Base.Security;
using Base.UI;
using Base.UI.Presets;
using Data.Service.Abstract;
using Framework;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using WebUI.Models.Dashboard;
using WebUI.Models.Dashboard.Widgets;

namespace WebUI.Controllers
{
    public class DashboardController : BaseController
    {
        private readonly IContentItemService _contentItemService;
        private readonly IRequestLogService _requestLogService;

        public DashboardController(IBaseControllerServiceFacade baseServiceFacade, IContentItemService contentItemService, IRequestLogService requestLogService) : base(baseServiceFacade)
        {
            _contentItemService = contentItemService;
            _requestLogService = requestLogService;
        }

        public PartialViewResult GetDashboard()
        {
            var dashboardVm = new DashboardVm(this)
            {
                Widgets = new List<DashboardWidget>()
            };

            var config = this.GetViewModelConfig(typeof(DashboardVm).FullName);

            var preset = this.PresetService.GetPreset<DashboardPreset>(config.Mnemonic);

            foreach (var widget in preset.Widgets)
            {
                var widgetConfig = this.GetViewModelConfig(widget.Mnemonic);

                if (widgetConfig == null) continue;

                if (this.SecurityUser.IsPermission(widgetConfig.TypeEntity, TypePermission.Read))
                    dashboardVm.Widgets.Add(widget);
            }

            return PartialView("_Dashboard", dashboardVm);
        }

        public async Task<JsonNetResult> GetPreset()
        {
            try
            {
                var preset =
                    await
                        this.PresetService.GetPresetAsync<DashboardPreset>("Dashboard");

                if (preset.Modified)
                {
                    using (var uofw = this.CreateUnitOfWork())
                    {
                        var mnemonics = preset.Widgets.Select(x => x.Mnemonic);

                        var defaultWidgets =
                            this.PresetService.GetDefaultPreset<DashboardPreset>(uofw, "Dashboard")
                                .Widgets.Where(x => !mnemonics.Contains(x.Mnemonic));

                        foreach (var widget in defaultWidgets)
                        {
                            widget.Hidden = true;
                        }

                        preset.Widgets.AddRange(defaultWidgets);
                    }
                }

                var widgets = new List<DashboardWidget>();

                foreach (var widget in preset.Widgets)
                {
                    var widgetConfig = this.GetViewModelConfig(widget.Mnemonic);

                    if (widgetConfig == null) continue;

                    if (this.SecurityUser.IsPermission(widgetConfig.TypeEntity, TypePermission.Read))
                        widgets.Add(widget);
                }

                preset.Widgets = widgets;

                return new JsonNetResult(preset);

            }
            catch (Exception ex)
            {
                return new JsonNetResult(new { error = ex.Message });
            }
        }


        private static readonly object GetNewsLocker = new object { };
        public async Task<ActionResult> GetNews()
        {
            string key = String.Format("786AABB4-36C8-49BA-ABC8-F7A583BD6D48");
            const int categoryID = 12; 


            if (this.CacheWrapper[key] != null)
                return Content(this.CacheWrapper[key].ToString(), "text/json");

            lock (GetNewsLocker)
            {
                using (var uofw = this.CreateUnitOfWork())
                {
                    this.CacheWrapper[key] = new JsonNetResult(new
                    {
                        Data = _contentItemService.GetAll(uofw)
                            .Where(
                                x =>
                                    x.CategoryID == categoryID && x.Top &&
                                    x.ContentItemStatus == ContentItemStatus.Published)
                            .OrderByDescending(x => x.Date)
                            .Take(10)
                            .Select(x => new
                            {
                                ID = x.ID,
                                Title = x.Title,
                                Description = x.Description,
                                Date = x.Date,
                                //ImagePreview = x.ImagePreview,
                            })
                    }).ToString();
                }
            }

            return Content(this.CacheWrapper[key].ToString(), "text/json");
        }


        public ActionResult ActiveUsers_Read([DataSourceRequest] DataSourceRequest request)
        {
            return new JsonNetResult(_requestLogService.GetAll(null).Where(x => x.User != null && x.User.ToLower() != "administrator" && x.Start >= DateTime.Now.AddMinutes(-15)).GroupBy(x => x.User).Select(x => new ActiveUserItem()
            {
                Login = x.Key,
                CountRequest = x.Count(),
            }).ToDataSourceResult(request));
        }
    }
}
