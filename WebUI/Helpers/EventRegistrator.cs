using Base;
using Base.Audit.Services;
using Base.BusinessProcesses.Entities;
using Base.BusinessProcesses.Services.Abstract;
using Base.DAL;
using Base.Notification.Service.Abstract;
using Base.Security.ObjectAccess;
using Base.Security.Service.Abstract;
using Base.Service;
using Base.Settings;
using Base.Task.Services;
using Ninject;
using Ninject.Activation;
using Ninject.Modules;
using Ninject.Planning.Bindings;
using System;
using System.Linq;

namespace WebUI.Helpers
{
    public class EventRegistrator
    {
        private readonly NinjectModule _ninjectModelule;

        public EventRegistrator(NinjectModule ninjectModule)
        {
            _ninjectModelule = ninjectModule;
        }

        private EventRegistrator RegisterEventForAll<T>(Action<IContext, T> action, params Type[] exclude)
        {
            foreach (IBinding bind in _ninjectModelule.Bindings.Where(x => typeof(T).IsAssignableFrom(x.Service) && !exclude.Contains(x.Service)))
            {
                bind.ActivationActions.Add((Action<IContext, object>)((context, instance) => action(context, (T)instance)));
            }

            return this;
        }

        private EventRegistrator RegisterEventFor<T>(Action<IContext, T> action)
        {
            foreach (IBinding bind in _ninjectModelule.Bindings.Where(x => x.Service == typeof(T)))
            {
                bind.ActivationActions.Add((Action<IContext, object>)((context, instance) => action(context, (T)instance)));
            }

            return this;
        }

        public void RegisterEvents()
        {
            this.RegisterEventForAll<IWorkflowService>((c, s) =>
            {
                s.WorkflowTransactionCompleted += (sender, args) =>
                {
                    var manager = c.Kernel.Get<IManagerNotification>();

                    manager.CreateNotices(args.UnitOfWork,
                        args.TasksToCreate.ToDictionary(x => x as BaseObject, x => BaseEntityState.Added));

                    manager.CreateNotices(args.UnitOfWork,
                        args.TasksToUpdate.ToDictionary(x => x as BaseObject, x => BaseEntityState.Modified));
                };
            });

            this.RegisterEventForAll<IWFObjectService>((c, s) =>
            {
                s.OnCreate += (sender, e) => c.Kernel.Get<IWorkflowService>().OnBPObjectCreate(sender, e);
                s.OnUpdate += (sender, e) => c.Kernel.Get<IWorkflowService>().OnBPObjectUpdate(sender, e);
                s.OnDelete += (sender, e) => c.Kernel.Get<IWorkflowService>().OnBPObjectDeleted(sender, e);
            });

            //this.RegisterEventForAll<ILocalizeItemService>((c, s) =>
            //{
            //    s.OnCreate += (sender, e) => c.Kernel.Get<ILocalizeItemService>().ResetCache();
            //    s.OnDelete += (sender, e) => c.Kernel.Get<ILocalizeItemService>().ResetCache();
            //    s.OnUpdate += (sender, e) => c.Kernel.Get<ILocalizeItemService>().ResetCache();
            //});

            this.RegisterEventForAll<ISettingItemService>((c, s) =>
            {
                s.OnUpdate += (sender, e) => c.Kernel.Get<IAuditItemService>().ResetCache();
            });

            this.RegisterEventForAll<ITaskService>((c, s) =>
            {
                s.OnCreate += (sender, e) => c.Kernel.Get<IManagerNotification>().CreateNotice(e);
                s.OnUpdate += (sender, e) => c.Kernel.Get<IManagerNotification>().CreateNotice(e);
                s.OnDelete += (sender, e) => c.Kernel.Get<IManagerNotification>().CreateNotice(e);
                s.OnGet += (sender, e) => c.Kernel.Get<IManagerNotification>().CreateNotice(e);
            });

            this.RegisterEventForAll<IBaseObjectCRUDService>((c, s) =>
            {
                s.OnCreate += (sender, e) =>
                {
                    //c.Kernel.Get<IAuditItemService>().ToJornalAsync(e.UnitOfWork, Base.Audit.Entities.TypeAuditItem.CreateObject, e.Object);

                    if (e.Object is IAccessibleObject)
                        c.Kernel.Get<ISecurityService>().OnObjectCreated(e.UnitOfWork, e.Object);
                };

                //s.OnUpdate += (sender, e) => c.Kernel.Get<IAuditItemService>().ToJornalAsync(e.UnitOfWork, Base.Audit.Entities.TypeAuditItem.UpdateObject, e.Object);

                s.OnDelete += (sender, e) =>
                {
                    //c.Kernel.Get<IAuditItemService>()
                    //    .ToJornalAsync(e.UnitOfWork, Base.Audit.Entities.TypeAuditItem.DeleteObject, e.Object);
                    if (e.Object is IAccessibleObject)
                        c.Kernel.Get<ISecurityService>().OnObjectDeleted(e.UnitOfWork, e.Object);
                    //if (e.Object is ILinkedObject)
                    //    c.Kernel.Get<ILinkedObjectsService>().OnObjectDeleted(e.UnitOfWork, e.Object);
                };
            });
        }
    }
}