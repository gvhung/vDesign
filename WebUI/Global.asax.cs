using Base.Ambient;
using Base.DAL;
using Base.Entities;
using Base.Helpers;
using Base.UI;
using Base.UI.Service;
using Framework;
using Framework.Wrappers;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Ninject;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Ninject;
using Ninject.Web.Common;
using System;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using WebUI.Concrete;
using WebUI.Helpers;

namespace WebUI
{
    public class MvcApplication : NinjectHttpApplication
    {
    protected override void OnApplicationStarted()
        {
            var kernel = CreateKernel();

            var cacheWrapper = kernel.Get<ICacheWrapper>();
            
            cacheWrapper.SetItem(Context.Cache);

            Data.Config.Init(kernel.Get<IEntityConfiguration>());

            DataConversion.Run(kernel);

            var configs = kernel.Get<IViewModelConfigService>().GetAll();

            var detailViewService = kernel.Get<IDetailViewService>();
            var listViewService = kernel.Get<IListViewService>();

            foreach (var config in configs)
            {
                try
                {
                    detailViewService.GetEditors(config);
                    listViewService.GetColumns(config);
                }
                catch (Exception e)
                {
#if DEBUG
                    throw;
#endif
                }
            }

            AreaRegistration.RegisterAllAreas();

            //WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            ControllerBuilder.Current.SetControllerFactory(new NinjectControllerFactory());

            ModelBinders.Binders.Add(typeof(Base.BaseObject), new BaseObjectModelBinder());
            ModelBinders.Binders.Add(typeof(Base.HCategory), new BaseObjectModelBinder());
            ModelBinders.Binders.Add(typeof(Base.Settings.SettingValues.ISettingValue), new SettingValueModelBinder());

            ModelBinderProviders.BinderProviders.Add(new InheritanceModelBinderProvider
            {
                { typeof (IRuntimeBindingType), new RuntimeTypeModelBinder() }
            });

#if DEBUG
            BundleTable.EnableOptimizations = false;
#else
            BundleTable.EnableOptimizations = true;
#endif

            var settings = new JsonSerializerSettings()
            {
                DateFormatString = "dd.MM.yyyy HH:mm:ss",
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                StringEscapeHandling = StringEscapeHandling.EscapeHtml,
            };

            GlobalHost.DependencyResolver = new NinjectDependencyResolver(kernel);

            GlobalHost.DependencyResolver.Register(typeof(JsonSerializer), () => JsonSerializer.Create(settings));
        }

        protected override IKernel CreateKernel()
        {
            return Startup.CreateKernel();
        }

        protected void Session_Start(object sender, EventArgs e)
        {   
        }

        protected void Session_End(object sender, EventArgs e)
        {
        }

        protected void Application_EndRequest(object sender, EventArgs e)
        {
            var userResolver = CreateKernel().Get<IAppContextBootstrapper>();
            userResolver.RemoveSecurityUser();
        }
    }
}