using Base.Attributes;
using Framework.Maybe;
using Framework.Wrappers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace Base.UI.Service
{
    public class ListViewService : IListViewService
    {
        private const string Key = "A99F7BF7-634F-4405-B426-8B74856A694D";

        private static readonly object CacheLock = new object();
        
        private readonly ICacheWrapper _cacheWrapper;
        private readonly IViewModelConfigService _viewModelConfigService;

        public ListViewService(ICacheWrapper cacheWrapper, IViewModelConfigService viewModelConfigService)
        {
            _cacheWrapper = cacheWrapper;
            _viewModelConfigService = viewModelConfigService;
        }

        public List<ColumnViewModel> GetColumns(string mnemonic)
        {
            return GetColumns(_viewModelConfigService.Get(mnemonic));
        }
        public List<ColumnViewModel> GetColumns(Type type)
        {
            return GetColumns(_viewModelConfigService.Get(type));
        }

        private Dictionary<string, List<ColumnViewModel>> GetCache()
        {
            if (_cacheWrapper[Key] != null)
                return _cacheWrapper[Key] as Dictionary<string, List<ColumnViewModel>>;

            lock (CacheLock)
            {
                if (_cacheWrapper[Key] == null)
                    _cacheWrapper[Key] = new Dictionary<string, List<ColumnViewModel>>();
            }

            return _cacheWrapper[Key] as Dictionary<string, List<ColumnViewModel>>;
        }

        public List<ColumnViewModel> GetColumns(ViewModelConfig viewModelConfig)
        {
            var cache = GetCache();

            if (cache.ContainsKey(viewModelConfig.Mnemonic))
                return cache[viewModelConfig.Mnemonic];

            lock (CacheLock)
            {
                if (!cache.ContainsKey(viewModelConfig.Mnemonic))
                {
                    var type = viewModelConfig.TypeEntity;

                    var columns = new List<ColumnViewModel>();

                    foreach (
                        var prop in
                            type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                .Where(p => !p.IsDefined(typeof (JsonIgnoreAttribute), false)))
                    {
                        var listViewAttr = prop.GetCustomAttribute<ListViewAttribute>();
                        var detailViewAttr = prop.GetCustomAttribute<DetailViewAttribute>();

                        var systemPropertyAttr = prop.GetCustomAttribute<SystemPropertyAttribute>();

                        if (listViewAttr == null && systemPropertyAttr != null)
                        {
                            listViewAttr = new ListViewAttribute()
                            {
                                Name = prop.Name,
                                Hidden = true
                            };
                        }

                        if (listViewAttr != null)
                        {
                            var column = new ColumnViewModel()
                            {
                                Mnemonic = detailViewAttr.With(x => x.Mnemonic),
                                PropertyName = prop.Name,
                                PropertyInfo = prop,
                                Hidden = listViewAttr.Hidden,
                                Order = listViewAttr.Order,
                                Filterable = listViewAttr.Filterable,
                                Sortable = listViewAttr.Sortable,
                                Width = listViewAttr.Width != 0 ? (int?) listViewAttr.Width : null,
                                Height = listViewAttr.Height != 0 ? (int?) listViewAttr.Height : null,
                                IsSystemPropery = systemPropertyAttr != null,
                                Groupable = listViewAttr.Groupable,
                            };

                            var maxLengthAttr = prop.GetCustomAttribute<MaxLengthAttribute>();

                            if (maxLengthAttr != null)
                            {
                                column.MaxLength = maxLengthAttr.Length;
                            }

                            column.ColumnType = column.PropertyType.IsBaseCollection()
                                ? column.PropertyType.GetGenericType().GenericTypeArguments[0]
                                : column.PropertyType;

                            column.Title = listViewAttr.Name;

                            if (String.IsNullOrEmpty(column.Title))
                            {
                                if (detailViewAttr != null)
                                {
                                    column.Title = detailViewAttr.Name;
                                }

                                if (String.IsNullOrEmpty(column.Title))
                                {
                                    column.Title = prop.Name;
                                }
                            }

                            if (column.Order < 0 && detailViewAttr != null)
                            {
                                column.Order = detailViewAttr.Order;
                            }

                            var configColumn =
                                viewModelConfig.ListView.Columns.FirstOrDefault(
                                    m => m.PropertyName == column.PropertyName);

                            if (configColumn != null)
                            {
                                if (configColumn.Mnemonic != null)
                                    column.Mnemonic = configColumn.Mnemonic;

                                if (configColumn.Title != null)
                                    column.Title = configColumn.Title;

                                if (configColumn.Hidden.HasValue)
                                    column.Hidden = configColumn.Hidden.Value;

                                if (configColumn.Filterable.HasValue)
                                    column.Filterable = configColumn.Filterable.Value;

                                if (configColumn.Order.HasValue)
                                    column.Order = configColumn.Order.Value;

                                if (configColumn.Locked.HasValue)
                                    column.Locked = configColumn.Locked.Value;

                                if (configColumn.Lockable.HasValue)
                                    column.Lockable = configColumn.Lockable.Value;
                            }

                            column.ViewModelConfig = _viewModelConfigService.Get(column.Mnemonic) ??
                                                     _viewModelConfigService.Get(column.ColumnType);

                            if (column.ViewModelConfig == null && prop.PropertyType.IsAssignableFromBase())
                                throw new Exception(
                                    String.Format("The configuration file is not configured for type \"{0}\"",
                                        column.ColumnType.Name));

                            if (String.IsNullOrEmpty(column.Mnemonic))
                                column.Mnemonic = type.GetTypeName();

                            columns.Add(column);
                        }
                    }

                    cache.Add(viewModelConfig.Mnemonic, columns.OrderBy(m => m.Order).ToList());
                }
            }

            return cache[viewModelConfig.Mnemonic];
        }
    }
}
