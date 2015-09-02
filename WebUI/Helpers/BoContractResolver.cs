using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using System.Web;
using Base;
using Base.UI;
using Base.UI.Service;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using WebUI.Controllers;

namespace WebUI.Helpers
{
    public class BoContractResolver : DefaultContractResolver
    {
        private readonly ViewModelConfig _config;
        private readonly List<EditorViewModel> _editors;
        private readonly IUiFasade _uiFasade;

        private readonly Dictionary<Type, IList<JsonProperty>> _propertyies = new Dictionary<Type, IList<JsonProperty>>();

        public BoContractResolver(string mnemonic, IUiFasade uiFasade)
        {
            _uiFasade = uiFasade;

            if (!String.IsNullOrEmpty(mnemonic))
            {
                _config = uiFasade.GetViewModelConfig(mnemonic);
                _editors = uiFasade.GetEditors(mnemonic);
            }
        }

        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            var objectType = type.GetBaseObjectType() ?? type;

            if (!_propertyies.ContainsKey(objectType))
            {
                var props = base.CreateProperties(objectType, memberSerialization);

                if (!objectType.IsBaseObject())
                {
                    _propertyies.Add(objectType, props);

                    return _propertyies[objectType];
                }

                if (_config.TypeEntity == objectType)
                {
                    var resProps = new List<JsonProperty>();

                    foreach (var prop in props)
                    {
                        if (prop.PropertyType.IsBaseObject() || prop.PropertyType.IsBaseCollection())
                        {
                            if (_editors.All(x => x.PropertyName != prop.PropertyName)) continue;
                        }

                        resProps.Add(prop);
                    }

                    _propertyies.Add(objectType, resProps);
                }
                else
                {
                    var config = _uiFasade.GetViewModelConfig(objectType);

                    if (config == null) return props;

                    if (objectType.IsDefined(typeof(ComplexTypeAttribute), false)) return props;

                    var editor = _editors.FirstOrDefault(x => x.EditorType == objectType);

                    if (editor == null) return props;

                    if (editor.EditorTemplate == "EasyCollection")
                    {
                        var typeEasyCollection = editor.ViewModelConfig.TypeEntity;

                        if (!_propertyies.ContainsKey(typeEasyCollection))
                        {
                            var properties = new JsonPropertyCollection(typeEasyCollection);

                            properties.AddProperty(CreateProperty(typeEasyCollection.GetProperty("ID"),
                                memberSerialization));

                            if (editor.ViewModelConfig.LookupProperty != "ID")
                                properties.AddProperty(CreateProperty(
                                    typeEasyCollection.GetProperty(editor.ViewModelConfig.LookupProperty), memberSerialization));

                            _propertyies.Add(typeEasyCollection, properties);
                        }
                    }

                    if (editor.Relationship == Relationship.OneToMany) return props;

                    var sysColumns = _uiFasade.GetEditors(objectType)
                        .Where(
                            c =>
                                c.IsSystemPropery &&
                                (c.PropertyName != "RowVersion" && c.PropertyName != "Hidden" &&
                                 c.PropertyName != "SortOrder")).ToList();

                    props =
                        props.Where(
                            p =>
                                p.PropertyName == config.LookupProperty ||
                                sysColumns.Any(x => x.PropertyName == p.PropertyName && x.IsSystemPropery)).ToList();

                    _propertyies.Add(objectType, props);
                }
            }

            return _propertyies[objectType];
        }
    }
}