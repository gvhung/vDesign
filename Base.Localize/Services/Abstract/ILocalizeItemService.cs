using Base.Entities.Complex;
using Base.Localize.Entities;
using Base.Service;
using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Base.Localize.Services.Abstract
{
    public interface ILocalizeItemService : IBaseCategorizedItemService<LocalizeItem>
    {
        string GetValueByKey(string key, string lang);
        string GetValueByKey(PropertyInfo property, string lang);
        string GetValueByKey(PropertyInfo property, string suffix, string lang);
        string GetPropertyTitle<TObject, TProperty>(Expression<Func<TObject, TProperty>> property, string lang);
        string GetPropertyTitle<TObject, TProperty>(TObject obj, Expression<Func<TObject, TProperty>> property, string lang);
        string GetValueByKey<TObject, TProperty>(TObject obj, Func<TObject, TProperty> property, string lang) where TProperty : MultilanguageText;
        LocalizePairString GetLocalizePairString<TObject, TProperty>(TObject obj, Expression<Func<TObject, TProperty>> property, string lang) where TProperty : MultilanguageText;
        LocalizePair<TProperty> GetLocalizePair<TObject, TProperty>(TObject obj, Expression<Func<TObject, TProperty>> property, string lang);
        void ResetCacheOnUpdate();
        string SetValueByKey(string key, string lang);
        string SetValueByKey(PropertyInfo property, string lang);
        void CheckCache();
    }
}
