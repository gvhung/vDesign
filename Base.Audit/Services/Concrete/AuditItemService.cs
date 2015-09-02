using Base.Ambient;
using Base.Audit.Entities;
using Base.DAL;
using Base.Entities.Complex;
using Base.Helpers;
using Base.QueryableExtensions;
using Base.Service;
using Base.Settings;
using Base.UI.Service;
using Framework.Wrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Config = Base.Audit.Entities.Config;

namespace Base.Audit.Services
{
    public class AuditItemService : BaseObjectService<AuditItem>, IAuditItemService
    {
        private const string Key = "{863DFE45-C33D-48EC-A7F7-5C0F809E7AFE}";

        private readonly ICacheWrapper _cacheWrapper;
        private readonly ISettingItemService _settingService;
        private readonly IDetailViewService _detailViewService;
        private readonly IHelperJsonConverter _helperJsonConverter;

        public AuditItemService(ICacheWrapper cacheWrapper, ISettingItemService settingService, IHelperJsonConverter helperJsonConverter, IBaseObjectServiceFacade facade, IDetailViewService detailViewService)
            : base(facade)
        {
            _cacheWrapper = cacheWrapper;
            _settingService = settingService;
            _helperJsonConverter = helperJsonConverter;
            _detailViewService = detailViewService;
        }

        private Config GetConfig()
        {
            if (_cacheWrapper[Key] == null)
            {
                _cacheWrapper[Key] = _settingService.GetValue(Consts.Settings.KEY_CONFIG, null) as Config;
            }

            return _cacheWrapper[Key] as Config;
        }

        private ICollection<DiffItem> GetDiff(Type type, BaseObject objOld, BaseObject objNew)
        {
            if (objOld == null || objNew == null) return null;

            var diff = new List<DiffItem>();

            var editors = _detailViewService.GetEditors(type);

            foreach (var editor in editors.Where(m => !m.JsonIgnore && !m.IsSystemPropery))
            {
                var pi = type.GetProperty(editor.PropertyName);

                var diffItem = new DiffItem()
                {
                    Property = editor.Title
                };

                var pold = pi.GetValue(objOld);
                var pnew = pi.GetValue(objNew);

                if (editor.PropertyType.IsBaseObject())
                {
                    int idold = 0;
                    int idnew = 0;

                    if (pold != null)
                        idold = (pold as BaseObject).ID;

                    if (pnew != null)
                        idnew = (pnew as BaseObject).ID;

                    if (idold != idnew)
                    {
                        if (pold != null)
                            diffItem.OldValue = pold.GetType().GetProperty(editor.ViewModelConfig.LookupProperty).GetValue(pold).ToString();

                        if (pnew != null)
                            diffItem.NewValue = pnew.GetType().GetProperty(editor.ViewModelConfig.LookupProperty).GetValue(pnew).ToString();

                        diff.Add(diffItem);
                    }
                }
                else if (editor.PropertyType.IsBaseCollection())
                {
                    //TODO: доделать!!!
                }
                else
                {
                    bool isdiff = false;

                    if (editor.PropertyType == typeof(decimal) || editor.PropertyType == typeof(decimal?))
                    {
                        isdiff = (decimal?)pold != (decimal?)pnew;
                    }
                    else if (editor.PropertyType == typeof(int) || editor.PropertyType == typeof(int?))
                    {
                        isdiff = (int?)pold != (int?)pnew;
                    }
                    else if (editor.PropertyType == typeof(long) || editor.PropertyType == typeof(long?))
                    {
                        isdiff = (long?)pold != (long?)pnew;
                    }
                    else if (editor.PropertyType == typeof(double) || editor.PropertyType == typeof(double?))
                    {
                        isdiff = (double?)pold != (double?)pnew;
                    }
                    else
                    {
                        isdiff = (pold != null ? pold.ToString() : "") != (pnew != null ? pnew.ToString() : "");
                    }

                    if (isdiff)
                    {
                        diffItem.OldValue = (pold ?? "").ToString();
                        diffItem.NewValue = (pnew ?? "").ToString();

                        diff.Add(diffItem);
                    }
                }
            }

            return diff;
        }

        public bool IsAudit(Type type)
        {
            if (type == null) return false;

            var config = this.GetConfig();

            if (config != null && config.Entities != null)
            {
                return config.Entities.Any(m => m.FullName == type.FullName);
            }

            return false;
        }

        public bool IsAudit<T>() where T : BaseObject
        {
            return this.IsAudit(typeof(T));
        }

        public async Task ToJornalAsync(IUnitOfWork unitOfWork, TypeAuditItem type, BaseObject entity, string desc = null)
        {
            if (unitOfWork is ISystemUnitOfWork) return;

            Type typeObj = null;

            if (entity != null)
                typeObj = entity.GetType().GetBaseObjectType();

            if (!IsAudit(typeObj)) return;

            string jsonObj = null;

            var auditItem = new AuditItem()
            {
                Type = type,
                Date = DateTime.Now,
                UserID = AppContext.SecurityUser.ID,
                Description = desc
            };

            if (entity != null)
            {
                jsonObj = _helperJsonConverter.SerializeObject(entity);

                auditItem.Entity = new LinkBaseObject
                {
                    ID = entity.ID,
                    FullName = typeObj.FullName,
                    Assembly = typeObj.Assembly.FullName,
                };

                entity =
                    _helperJsonConverter.DeserializeObject(_helperJsonConverter.SerializeObject(entity), typeObj) as
                        BaseObject;
            }

            await ToJornalEx(typeObj, auditItem, type, entity, jsonObj, desc);
        }

        private async Task ToJornalEx(Type typeObj, AuditItem auditItem, TypeAuditItem type, BaseObject entity, string jsonObj, string desc = null)
        {
            using (var uofw = UnitOfWorkFactory.CreateSystem())
            {
                var repositoty = uofw.GetRepository<AuditItem>();

                var config = this.GetConfig();

                switch (type)
                {
                    case TypeAuditItem.LogOf:
                    case TypeAuditItem.LogOn:
                    case TypeAuditItem.LogOnError:
                        if (config.RegisterLogIn)
                        {
                            repositoty.Create(auditItem);

                            await uofw.SaveChangesAsync();
                        }

                        break;
                    case TypeAuditItem.CreateObject:
                        if (this.IsAudit(typeObj))
                        {
                            auditItem.JsonObj = jsonObj;

                            repositoty.Create(auditItem);

                            await uofw.SaveChangesAsync();
                        }

                        break;
                    case TypeAuditItem.UpdateObject:
                        if (this.IsAudit(typeObj))
                        {
                            BaseObject objOld = null;

                            var item = await 
                                repositoty.All()
                                    .Where(m => m.Entity.ID == entity.ID && m.Entity.FullName == auditItem.Entity.FullName && m.Type != TypeAuditItem.DeleteObject)
                                    .OrderByDescending(m => m.Date)
                                    .FirstOrDefaultAsync();

                            if (item != null && item.JsonObj != null)
                            {
                                objOld =
                                    _helperJsonConverter.DeserializeObject(item.JsonObj, typeObj) as
                                        BaseObject;

                                item.JsonObj = null;

                                repositoty.Update(item);
                            }

                            auditItem.JsonObj = jsonObj;

                            auditItem.Diff = this.GetDiff(typeObj, objOld, entity);

                            repositoty.Create(auditItem);

                            await uofw.SaveChangesAsync();
                        }
                        break;
                    case TypeAuditItem.DeleteObject:
                        if (this.IsAudit(typeObj))
                        {

                            foreach (var item in await repositoty.All()
                                    .Where(m => m.Entity.ID == entity.ID && m.Entity.FullName == auditItem.Entity.FullName && m.JsonObj != null).ToGenericListAsync())
                            {
                                item.JsonObj = null;
                                repositoty.Update(item);
                            }

                            repositoty.Create(auditItem);

                            await uofw.SaveChangesAsync();
                        }

                        break;
                }
            }
        }

        public void ResetCache()
        {
            _cacheWrapper.Remove(Key);
        }
    }
}
