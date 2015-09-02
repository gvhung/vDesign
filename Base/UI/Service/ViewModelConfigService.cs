using Base.Service;
using Framework.Wrappers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;

namespace Base.UI
{
    public class ViewModelConfigService : IViewModelConfigService
    {
        private const string Key = "50E6008E-0A02-404C-8E06-4E1F0B642101";

        private static readonly object CacheLock = new object();

        private readonly IViewModelConfigLoader _loder;

        private readonly ICacheWrapper _cacheWrapper;

        public ViewModelConfigService(IViewModelConfigLoader loder, ICacheWrapper cacheWrapper)
        {
            _loder = loder;
            _cacheWrapper = cacheWrapper;
        }

        public List<ViewModelConfig> GetAll()
        {
            List<ViewModelConfig> res = null;

            if (_cacheWrapper == null)
            {
                res = GetAll();
            }
            else
            {
                lock (CacheLock)
                {
                    if (_cacheWrapper[Key] == null)
                    {
                        _cacheWrapper[Key] = _loder.Load(CreateViewModelConfig);
                    }

                    res = (List<ViewModelConfig>) _cacheWrapper[Key];
                }
            }

            return res ?? (new List<ViewModelConfig>());
        }

        public ViewModelConfig Get(string mnemonic)
        {
            if (String.IsNullOrEmpty(mnemonic)) return null;

            return GetAll().FirstOrDefault(m => m.Mnemonic == mnemonic) ?? GetAll().FirstOrDefault(x => x.TypeEntity.FullName == mnemonic) ?? this.Get(Type.GetType(mnemonic));
        }

        public ViewModelConfig Get(Type type)
        {
            if (type == null) return null;

            return GetAll().FirstOrDefault(x => x.TypeEntity == type) ?? CreateViewModelConfig(type);
        }

        public Dictionary<Type, string> GetAllObjects()
        {
            return GetAllObjects(x => true);
        }

        public ViewModelConfig CreateViewModelConfig(Type type)
        {
            if (!type.IsBaseObject() && !type.IsDefined(typeof(ComplexTypeAttribute))) return null;

            string serviceName = "";

            if (type.IsBaseObject())
            {
                if (type.IsAssignableFrom(typeof(HCategory)))
                    serviceName = typeof(IBaseCategoryService<>).GetTypeName();
                else if (type.IsAssignableFrom(typeof(ICategorizedItem)))
                    serviceName = typeof(IBaseCategorizedItemService<>).GetTypeName();
                else
                    serviceName = typeof(IBaseObjectService<>).GetTypeName();
            }

            return new ViewModelConfig(
                mnemonic: type.GetTypeName(),
                entity: type.GetTypeName(),
                listView: new ListView(),
                detailView: new DetailView(),
                lookupProperty:
                    type.IsBaseObject()
                        ? (type.GetProperty("Title") ?? type.GetProperty("Name") ?? type.GetProperty("ID")).Name
                        : "",
                service: serviceName);
        }

        public Dictionary<Type, string> GetAllObjects(Func<ViewModelConfig, bool> predicate)
        {
            return GetAll().Where(predicate)
                .Where(x => x.TypeEntity != typeof(Security.AccessUser) && x.TypeEntity != typeof(Security.Role) && x.TypeEntity != typeof(Security.ChildRole) && x.TypeEntity != typeof(Security.Permission))
                .GroupBy(m => m.TypeEntity)
                .Select(m => new { Type = m.Key, Name = m.FirstOrDefault().Title })
                .OrderBy(m => m.Name)
                .ToDictionary(x => x.Type, x => x.Name);
        }

        public Dictionary<string, string> GetAllMnemonics(Func<ViewModelConfig, bool> predicate)
        {
            return GetAll().Where(predicate)
                .Where(x => x.TypeEntity != typeof(Security.AccessUser) && x.TypeEntity != typeof(Security.Role) && x.TypeEntity != typeof(Security.ChildRole) && x.TypeEntity != typeof(Security.Permission))
                .ToDictionary(x => x.Mnemonic, x => x.DetailView.Title);
        }
    }
}
