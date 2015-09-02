using Base.UI;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebUI.Controllers;
using Base;
using Base.UI.Service;

namespace WebUI.Helpers
{
    public class ListBoContractResolver : DefaultContractResolver
    {
        private readonly ViewModelConfig _config;
        private readonly List<ColumnViewModel> _columns;
        private readonly IUiFasade _uiFasade;

        private readonly Dictionary<Type, IList<JsonProperty>> _propertyies = new Dictionary<Type, IList<JsonProperty>>();

        public ListBoContractResolver(string mnemonic, IUiFasade uiFasade)
        {
            _uiFasade = uiFasade;

            if (!String.IsNullOrEmpty(mnemonic))
            {
                _config = uiFasade.GetViewModelConfig(mnemonic);
                _columns = uiFasade.GetColumns(mnemonic);
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

                if (_config.TypeEntity.IsAssignableFrom(objectType))
                {
                    var cols = _columns.Select(m => m.PropertyName).ToArray();

                    props = props.Where(p => cols.Contains(p.PropertyName)).ToList();
                }
                else
                {
                    var config = _uiFasade.GetViewModelConfig(objectType);

                    if (config != null)
                    {
                        var sysColumns =
                            _uiFasade.GetColumns(objectType)
                                .Where(
                                    c =>
                                        c.IsSystemPropery &&
                                        (c.PropertyName != "RowVersion" && c.PropertyName != "Hidden" &&
                                         c.PropertyName != "SortOrder"))
                                .ToList();

                        props =
                            props.Where(
                                p =>
                                    p.PropertyName == config.LookupProperty ||
                                    sysColumns.Any(x => x.PropertyName == p.PropertyName && x.IsSystemPropery))
                                .ToList();
                    }
                    else
                    {
                        props = new List<JsonProperty>() { props.First(x => x.PropertyName == "ID") };
                    }
                }

                _propertyies.Add(objectType, props);
            }

            return _propertyies[objectType];
        }
    }
}