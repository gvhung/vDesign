using Base.DAL;
using Base.Entities.Complex;
using Base.Localize.Entities;
using Base.Localize.Services.Abstract;
using Base.Service;
using Framework.Maybe;
using Framework.Wrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Base.Localize.Services.Concrete
{
    public class LocalizeItemService : BaseCategorizedItemService<LocalizeItem>, ILocalizeItemService
    {
        private const string Key = "{F38E349B-561D-459C-96B1-DF4D8876C733}";

        private readonly ICacheWrapper _cacheWrapper;
        private readonly ISystemUnitOfWork _systemUnitOfWork;

        public LocalizeItemService(ICacheWrapper cacheWrapper, IBaseObjectServiceFacade facade, IUnitOfWorkFactory unitOfWorkFactory)
            : base(facade)
        {
            _cacheWrapper = cacheWrapper;
            _systemUnitOfWork = unitOfWorkFactory.CreateSystem();
        }

        public override IQueryable<LocalizeItem> GetAllCategorizedItems(IUnitOfWork unitOfWork, int categoryID, bool? hidden = false)
        {
            string strID = HCategory.IdToString(categoryID);
            return this.GetAll(unitOfWork, hidden).Where(a => (a.LocalizeItemCategory.sys_all_parents != null && a.LocalizeItemCategory.sys_all_parents.Contains(strID)) || a.LocalizeItemCategory.ID == categoryID);
        }

        public string GetValueByKey(string key, string lang)
        {
            if (_cacheWrapper[Key] == null)
            {
                var version = _systemUnitOfWork.GetRepository<LocalizationVersion>().All().FirstOrDefault();
                _cacheWrapper[Key] = new LocalizationCacheWrapper
                {
                    Items = GetItems(),
                    Version = version.WithStruct(x => x.Version, 0)
                };
            }

            var wrapper = _cacheWrapper[Key] as LocalizationCacheWrapper;
            if (wrapper != null && wrapper.Items.ContainsKey(key))
                return wrapper.Items[key][lang];

            return key;
        }

        public string GetValueByKey<TObject, TProperty>(TObject obj, Func<TObject, TProperty> property, string lang)
            where TProperty : MultilanguageText
        {
            return property(obj)[lang];
        }

        public LocalizePair<TProperty> GetLocalizePair<TObject, TProperty>(TObject obj, Expression<Func<TObject, TProperty>> property, string lang)
        {
            var propertyExpression = property.Body as MemberExpression;

            if (propertyExpression == null)
                return new LocalizePair<TProperty>(String.Empty, default(TProperty));

            var propertyInfo = propertyExpression.Member as PropertyInfo;

            var prop = property.Compile()(obj);

            return new LocalizePair<TProperty>(this.GetValueByKey(propertyInfo, lang), prop);
        }

        public LocalizePairString GetLocalizePairString<TObject, TProperty>(TObject obj, Expression<Func<TObject, TProperty>> property, string lang)
            where TProperty : MultilanguageText
        {
            var propertyExpression = property.Body as MemberExpression;

            if (propertyExpression == null)
                return new LocalizePairString(String.Empty, String.Empty);

            var propertyInfo = propertyExpression.Member as PropertyInfo;

            var multilanguageText = property.Compile()(obj);

            string propTitle = this.GetValueByKey(propertyInfo, lang);

            return new LocalizePairString(propTitle, multilanguageText[lang]);
        }

        public string GetPropertyTitle<TObject, TProperty>(TObject obj, Expression<Func<TObject, TProperty>> property, string lang)
        {
            return this.GetPropertyTitle(property, lang);
        }

        public string GetPropertyTitle<TObject, TProperty>(Expression<Func<TObject, TProperty>> property, string lang)
        {
            var propertyExpression = property.Body as MemberExpression;

            if (propertyExpression == null)
                return String.Empty;

            var propertyInfo = propertyExpression.Member as PropertyInfo;

            return this.GetValueByKey(propertyInfo, lang);
        }

        public string GetValueByKey(PropertyInfo property, string lang)
        {
            string key = String.Format("{0}{1}{2}", property.DeclaringType.FullName.Replace('.', LocalizeItem.Separator), LocalizeItem.Separator, property.Name);

            return this.GetValueByKey(key, lang);
        }

        public string GetValueByKey(PropertyInfo property, string suffix, string lang)
        {
            string key = String.Format("{0}{1}{2}{3}", property.DeclaringType.FullName.Replace('.', LocalizeItem.Separator), LocalizeItem.Separator, property.Name, suffix);

            return this.GetValueByKey(key, lang);
        }

        public void ResetCacheOnUpdate()
        {
            var repository = _systemUnitOfWork.GetRepository<LocalizationVersion>();

            var version = repository.All().FirstOrDefault();
            if (version == null)
                repository.Create(version = new LocalizationVersion());

            version.Version++;

            _systemUnitOfWork.SaveChanges();

            _cacheWrapper.Remove(Key);
        }

        public void CheckCache()
        {
            var version = _systemUnitOfWork.GetRepository<LocalizationVersion>().All().FirstOrDefault();
            if (version != null)
            {
                var wrapper = _cacheWrapper[Key] as LocalizationCacheWrapper;
                if (wrapper != null && wrapper.Version != version.Version)
                {
                    _cacheWrapper.Remove(Key);
                }
            }
        }

        public string SetValueByKey(string key, string lang)
        {
            throw new NotImplementedException();
        }

        public string SetValueByKey(PropertyInfo property, string lang)
        {
            throw new NotImplementedException();
        }

        private Dictionary<string, MultilanguageTextArea> GetItems()
        {
            var dic = new Dictionary<string, MultilanguageTextArea>(StringComparer.InvariantCultureIgnoreCase);

            var catDictionary = _systemUnitOfWork.GetRepository<LocalizeItemCategory>().All().Select(m => new { id = m.ID, name = m.Name })
                .ToDictionary(x => x.id, x => x.name);

            var items = _systemUnitOfWork.GetRepository<LocalizeItem>().Filter(x => !x.Hidden && !x.LocalizeItemCategory.Hidden).OrderBy(x => x.LocalizeItemCategory.ID).ToList();

            foreach (var localizeItem in items)
            {
                string key = "";

                if (!String.IsNullOrEmpty(localizeItem.LocalizeItemCategory.sys_all_parents))
                    key = String.Join(LocalizeItem.Separator.ToString(), localizeItem.LocalizeItemCategory.sys_all_parents.Split(HCategory.Seperator).Select(x => catDictionary[HCategory.IdToInt(x)])) + LocalizeItem.Separator;

                key += localizeItem.LocalizeItemCategory.Name + LocalizeItem.Separator + localizeItem.Key;

                if (!dic.ContainsKey(key))
                    dic.Add(key, localizeItem.Value);
            }

            return dic;
        }

        internal class LocalizationCacheWrapper
        {
            public int Version { get; set; }
            public Dictionary<string, MultilanguageTextArea> Items { get; set; }
        }
    }
}
