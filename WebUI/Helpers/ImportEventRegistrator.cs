using Base.Audit.Services;
using Base.BusinessProcesses.Entities;
using Base.BusinessProcesses.Services.Abstract;
using Base.Security.ObjectAccess;
using Base.Security.Service.Abstract;
using Base.Service;
using Base.Settings;
using Ninject;
using Ninject.Activation;
using Ninject.Modules;
using Ninject.Planning.Bindings;
using System;
using System.Linq;
namespace WebUI.Helpers
{
    public class ImportEventRegistrator
    {
        private readonly NinjectModule _ninjectModelule;

        public ImportEventRegistrator(NinjectModule ninjectModule)
        {
            _ninjectModelule = ninjectModule;
        }

        private ImportEventRegistrator RegisterEventForAll<T>(Action<IContext, T> action, params Type[] exclude)
        {
            foreach (IBinding bind in _ninjectModelule.Bindings.Where(x => typeof(T).IsAssignableFrom(x.Service) && !exclude.Contains(x.Service)))
            {
                bind.ActivationActions.Add((Action<IContext, object>)((context, instance) => action(context, (T)instance)));
            }

            return this;
        }

        private ImportEventRegistrator RegisterEventFor<T>(Action<IContext, T> action)
        {
            foreach (IBinding bind in _ninjectModelule.Bindings.Where(x => x.Service == typeof(T)))
            {
                bind.ActivationActions.Add((Action<IContext, object>)((context, instance) => action(context, (T)instance)));
            }

            return this;
        }

        public void RegisterEvents()
        {
            
            this.RegisterEventForAll<IWFObjectService>((c, s) =>
            {
                s.OnCreate += (sender, e) => c.Kernel.Get<IWorkflowService>().OnBPObjectCreate(sender, e);
            });

            this.RegisterEventForAll<ISettingItemService>((c, s) =>
            {
                s.OnUpdate += (sender, e) => c.Kernel.Get<IAuditItemService>().ResetCache();
            });

            this.RegisterEventForAll<IBaseObjectCRUDService>((c, s) =>
            {
                s.OnCreate += (sender, e) =>
                {
                    if (e.Object is IAccessibleObject)
                        c.Kernel.Get<ISecurityService>().OnObjectCreated(e.UnitOfWork, e.Object);
                };

            });

        }
    }
}