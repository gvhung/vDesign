using Base.Attributes;
using Base.DAL;
using Base.Entities.Complex;
using Framework.Wrappers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace Base.UI.Service
{
    public class DetailViewService : IDetailViewService
    {
        private const string Key = "605C3CC7-5B5F-4C68-8564-045871CA88DE";

        private static readonly object _CacheLock = new object();

        private readonly ICacheWrapper _cacheWrapper;
        private readonly IViewModelConfigService _viewModelConfigService;
        private readonly IDetailViewSettingManager _detailViewSettingManager;
        
        public DetailViewService(ICacheWrapper cacheWrapper, IViewModelConfigService viewModelConfigService, IDetailViewSettingManager detailViewSettingManager)
        {
            _cacheWrapper = cacheWrapper;
            _viewModelConfigService = viewModelConfigService;
            _detailViewSettingManager = detailViewSettingManager;
        }

        public EditorViewModel GetEditorViewModel(string mnemonic, string member)
        {
            return GetEditors(mnemonic).FirstOrDefault(x => x.PropertyName == member);
        }

        public List<EditorViewModel> GetEditors(string mnemonic)
        {
            return GetEditors(_viewModelConfigService.Get(mnemonic));
        }

        public List<EditorViewModel> GetEditors(Type type)
        {
            return GetEditors(_viewModelConfigService.Get(type));
        }

        private Dictionary<string, List<EditorViewModel>> GetCache()
        {
            if (_cacheWrapper[Key] != null)
                return _cacheWrapper[Key] as Dictionary<string, List<EditorViewModel>>;

            lock (_CacheLock)
            {
                if (_cacheWrapper[Key] == null)
                    _cacheWrapper[Key] = new Dictionary<string, List<EditorViewModel>>();
            }

            return _cacheWrapper[Key] as Dictionary<string, List<EditorViewModel>>;
        }

        public List<EditorViewModel> GetEditors(ViewModelConfig viewModelConfig)
        {
            if (viewModelConfig == null)
            {
                return new List<EditorViewModel>();
            }

            var cache = GetCache();

            if (cache.ContainsKey(viewModelConfig.Mnemonic))
                return cache[viewModelConfig.Mnemonic];

            lock (_CacheLock)
            {
                if (!cache.ContainsKey(viewModelConfig.Mnemonic))
                {
                    var type = viewModelConfig.TypeEntity;

                    var editors = new List<EditorViewModel>();

                    editors.AddRange(type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                        .Select(x => GetEditor(viewModelConfig, x, type))
                        .Where(x => x != null));

                    cache.Add(viewModelConfig.Mnemonic, editors.OrderBy(m => m.Order).ToList());    
                }

                return cache[viewModelConfig.Mnemonic];
            }
        }

        public CommonEditorViewModel GetCommonEditor(string mnemonic)
        {
            return new CommonEditorViewModel(_viewModelConfigService.Get(mnemonic), GetEditors(mnemonic));
        }

        public CommonEditorViewModel GetCommonEditor(IUnitOfWork unitOfWork, string mnemonic, BaseObject obj)
        {
            var commonEditor = GetCommonEditor(mnemonic);

            var settings = _detailViewSettingManager.GetSettings(unitOfWork, mnemonic, obj);

            if (settings == null) return commonEditor;

            foreach (var setting in settings)
            {
                commonEditor.ApplySetting(setting);
            }

            return commonEditor;
        }

        //Private
        private EditorViewModel GetEditor(ViewModelConfig viewModelConfig, PropertyInfo prop, Type type)
        {
            var detailViewAttr = prop.GetCustomAttribute<DetailViewAttribute>();

            var systemPropertyAttr = prop.GetCustomAttribute<SystemPropertyAttribute>();

            if (detailViewAttr == null && systemPropertyAttr != null)
            {
                detailViewAttr = new DetailViewAttribute()
                {
                    Name = prop.Name,
                    Visible = false
                };
            }

            if (detailViewAttr != null)
            {
                var maxLengthAttr = prop.GetCustomAttribute<MaxLengthAttribute>();
                var jsonIgnoreAttr = prop.GetCustomAttribute<JsonIgnoreAttribute>();

                bool hideLabel = detailViewAttr.HideLabel;

                if (!hideLabel)
                    hideLabel = String.IsNullOrEmpty(detailViewAttr.Name);

                var editor = new EditorViewModel
                {
                    Mnemonic = detailViewAttr.Mnemonic,
                    ParentViewModelConfig = viewModelConfig,
                    Title = detailViewAttr.Name ?? prop.Name,
                    Description = detailViewAttr.Description,
                    PropertyName = prop.Name,
                    PropertyType = prop.PropertyType,
                    IsLabelVisible = !hideLabel,
                    TabName = detailViewAttr.TabName ?? "[0]Основное",
                    GroupName = detailViewAttr.GroupName ?? Guid.NewGuid().ToString("N"),
                    IsReadOnly = !prop.CanWrite || detailViewAttr.ReadOnly,
                    IsRequired = detailViewAttr.Required,
                    Order = detailViewAttr.Order,
                    MaxLength = maxLengthAttr != null ? (int?)maxLengthAttr.Length : null,
                    DeferredLoading = detailViewAttr.DeferredLoading,
                    Width = detailViewAttr.Width != 0 ? (int?)detailViewAttr.Width : null,
                    Height = detailViewAttr.Height != 0 ? (int?)detailViewAttr.Height : null,
                    JsonIgnore = jsonIgnoreAttr != null,
                    IsSystemPropery = systemPropertyAttr != null,
                    Visible = detailViewAttr.Visible,
                    EditorType =
                        prop.PropertyType.IsBaseCollection()
                            ? prop.PropertyType.GetGenericType().GenericTypeArguments[0]
                            : prop.PropertyType,
                    Relationship = GetRelationship(prop.PropertyType, type),
                };

                var dataTypeAttr = prop.GetCustomAttribute<PropertyDataTypeAttribute>();

                if (dataTypeAttr == null)
                {
                    if (editor.EditorType == typeof(string) && editor.MaxLength == null)
                        editor.EditorTemplate = "MultilineText";
                    else if (editor.EditorType == typeof(int) || editor.EditorType == typeof(int?))
                        editor.EditorTemplate = "Integer";
                    else if (editor.EditorType == typeof(decimal) || editor.EditorType == typeof(decimal?))
                        editor.EditorTemplate = "Currency";
                    else if (editor.EditorType == typeof(double) || editor.EditorType == typeof(double?))
                        editor.EditorTemplate = "Double";
                    else if (editor.EditorType == typeof(DateTime) || editor.EditorType == typeof(DateTime?))
                        editor.EditorTemplate = "Date";
                    else if (editor.EditorType.IsEnum())
                        editor.EditorTemplate = "Enum";
                    else if (editor.EditorType == typeof(Period))
                        editor.EditorTemplate = "Period";
                    else if (editor.EditorType == typeof(Icon))
                        editor.EditorTemplate = "Icon";
                    else if (typeof(MultilanguageText).IsAssignableFrom(editor.EditorType))
                        editor.EditorTemplate = "Multilanguage";
                    else if (editor.EditorType == typeof(Url))
                        editor.EditorTemplate = "ComplexUrl";
                    else if (editor.EditorType == typeof(UrlMultilanguageText))
                        editor.EditorTemplate = "UrlMultilanguageText";
                    else if (editor.EditorType == typeof(LinkBaseObject))
                        editor.EditorTemplate = "LinkBaseObject";
                    else if (editor.EditorType == typeof(MultiEnum))
                        editor.EditorTemplate = "MultiEnum";
                    //else if (editor.EditorType == typeof(ValidationObjectBinding))
                    //    editor.EditorTemplate = "ValidationObjectBinding";
                    else
                    {
                        Type easyCollectionEntry =
                            prop.PropertyType.GetEntryOfUnboundedTypeOfCollection(typeof(EasyCollectionEntry<>));

                        if (easyCollectionEntry != null)
                        {
                            editor.EditorTemplate = "EasyCollection";
                            editor.ViewModelConfig = _viewModelConfigService.Get(easyCollectionEntry);
                        }
                        else
                        {
                            switch (editor.Relationship)
                            {
                                case Relationship.OneToMany:
                                    if (typeof(FileData).IsAssignableFrom(editor.EditorType))
                                        editor.EditorTemplate = "Files";
                                    else if (typeof(Base.LinkedObjects.Entities.Link).IsAssignableFrom(editor.EditorType))
                                        editor.EditorTemplate = "ListLinkedОbjects";
                                    else
                                        editor.EditorTemplate = "OneToMany";

                                    break;

                                case Relationship.ManyToMany:
                                    editor.EditorTemplate = "ManyToMany";
                                    break;

                                case Relationship.One:
                                    editor.EditorTemplate = "BaseObjectOne";
                                    break;

                                case Relationship.None:
                                    editor.EditorTemplate = editor.EditorType.Name;
                                    break;
                            }
                        }
                    }
                }
                else
                {
                    editor.EditorTemplate = dataTypeAttr.DataType != PropertyDataType.Custom
                        ? dataTypeAttr.DataType.ToString()
                        : dataTypeAttr.CustomDataType;

                    editor.EditorTemplateParams = dataTypeAttr.Params;
                }


                var configEditor = viewModelConfig.DetailView.Editors.FirstOrDefault(m => m.PropertyName == editor.PropertyName);

                if (configEditor != null)
                {
                    if (configEditor.Mnemonic != null)
                        editor.Mnemonic = configEditor.Mnemonic;

                    if (configEditor.Title != null)
                        editor.Title = configEditor.Title;

                    if (configEditor.IsLabelVisible != null)
                        editor.IsLabelVisible = configEditor.IsLabelVisible.Value;

                    if (configEditor.EditorTemplate != null)
                        editor.EditorTemplate = configEditor.EditorTemplate;

                    if (configEditor.TabName != null)
                        editor.TabName = configEditor.TabName;

                    if (configEditor.IsReadOnly != null)
                        editor.IsReadOnly = configEditor.IsReadOnly.Value;

                    if (configEditor.IsRequired != null)
                        editor.IsRequired = configEditor.IsRequired.Value;

                    if (configEditor.Order != null)
                        editor.Order = configEditor.Order.Value;

                    if (configEditor.Visible != null)
                        editor.Visible = configEditor.Visible.Value;
                }

                if (editor.ViewModelConfig == null)
                {
                    editor.ViewModelConfig = _viewModelConfigService.Get(editor.Mnemonic) ?? _viewModelConfigService.Get(editor.EditorType);

                    if (editor.ViewModelConfig == null && prop.PropertyType.IsAssignableFromBase())
                        throw new Exception(String.Format("The configuration file is not configured for type \"{0}\"", editor.EditorType.Name));
                }


                if (String.IsNullOrEmpty(editor.Mnemonic))
                    editor.Mnemonic = type.GetTypeName();

                return editor;
            }

            return null;
        }

        private Relationship GetRelationship(Type propertyType, Type containerType)
        {
            if (propertyType.IsBaseObject())
            {
                return Relationship.One;
            }
            else if (propertyType.IsGenericType)
            {
                var collectionType = propertyType.GetGenericType();

                if (collectionType != null)
                {
                    var genericType = collectionType.GenericTypeArguments[0];

                    if (genericType != null && genericType.IsBaseObject())
                    {
                        IEnumerable<PropertyInfo> props = genericType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

                        if (props.Any(x =>
                        {
                            var gType = x.PropertyType.GetGenericType();

                            return gType != null && gType.GenericTypeArguments[0].IsAssignableFrom(containerType);
                        }))
                        {
                            return Relationship.ManyToMany;
                        }

                        return Relationship.OneToMany;
                    }
                }
            }

            return Relationship.None;
        }
    }
}
