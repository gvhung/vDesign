using Base.DAL;
using Base.Service;
using Framework.Wrappers;
using System;
using System.Linq;

namespace Base.Settings
{
    public class SettingItemService : BaseCategorizedItemService<SettingItem>, ISettingItemService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICacheWrapper _cacheWrapper;

        private static readonly object _CacheLock = new object();

        public SettingItemService(IBaseObjectServiceFacade facade, IUnitOfWorkFactory unitOfWorkFactory, ICacheWrapper cacheWrapper)
            : base(facade)
        {
            _cacheWrapper = cacheWrapper;
            _unitOfWork = unitOfWorkFactory.CreateSystem();
        }

        public override IQueryable<SettingItem> GetAllCategorizedItems(IUnitOfWork unitOfWork, int categoryID, bool? hidden = false)
        {
            string strID = HCategory.IdToString(categoryID);

            return GetAll(unitOfWork, hidden).Cast<SettingItem>().Where(a => (a.Category_.sys_all_parents != null && a.Category_.sys_all_parents.Contains(strID)) || a.Category_.ID == categoryID);
        }

        private string GetKeyForCache(Guid key)
        {
            return System.String.Format("{0}:{1}", "{A6768732-354F-4005-94A5-1F1798A2C8F6}", key);
        }

        private void RemoveCache(Guid key)
        {
            string keycache = GetKeyForCache(key);

            if (_cacheWrapper != null && _cacheWrapper.IsInitialized)
            {
                lock (_CacheLock)
                {
                    _cacheWrapper.Remove(keycache);    
                }
            }
        }

        public object GetValue(Guid key, object bydef)
        {
            SettingItem settingItem = null;

            if (_cacheWrapper != null && _cacheWrapper.IsInitialized)
            {
                string keycache = GetKeyForCache(key);

                lock (_CacheLock)
                {
                    if (_cacheWrapper[keycache] == null)
                    {
                        var val =
                            _unitOfWork.GetRepository<SettingItem>()
                                .All()
                                .FirstOrDefault(m => m.Key == key && !m.Hidden);

                        _cacheWrapper.Add(keycache, val);
                    }

                    settingItem = _cacheWrapper[keycache] as SettingItem;
                }
            }
            else
            {
                settingItem = _unitOfWork.GetRepository<SettingItem>().All().FirstOrDefault(m => m.Key == key && !m.Hidden);    
            }
            
            return settingItem != null ? settingItem.Value.Value : bydef;
        }

        public override SettingItem Update(IUnitOfWork unitOfWork, SettingItem obj)
        {
            RemoveCache(obj.Key);

            return base.Update(unitOfWork, obj);
        }

        public override void Delete(IUnitOfWork unitOfWork, SettingItem obj)
        {
            RemoveCache(obj.Key);

            base.Delete(unitOfWork, obj);
        }
    }
}
