using Base.Helpers;
using Framework.Wrappers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using Base;
using Base.UI;
using Base.UI.Service;
using Newtonsoft.Json.Serialization;
using WebUI.Helpers;

namespace WebUI.Concrete
{
    public class HelperJsonConverter : IHelperJsonConverter
    {
        private readonly IViewModelConfigService _viewModelConfigService;
        private readonly IContractResolverFactory _contractResolverFactory;

        public HelperJsonConverter(IContractResolverFactory contractResolverFactory, IViewModelConfigService viewModelConfigService)
        {
            _contractResolverFactory = contractResolverFactory;
            _viewModelConfigService = viewModelConfigService;
        }

        public JsonSerializerSettings GetSettings()
        {
            return new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                TypeNameHandling = TypeNameHandling.All,
                Converters = new List<JsonConverter> { new DbGeographyGeoJsonConverter() { } },
                Binder = new DynamicProxySerializationBinder(),
            };
        }

        public string SerializeObject(BaseObject obj, bool completeGraph = false)
        {
            if (completeGraph)
            {
                return JsonConvert.SerializeObject(obj, Formatting.None, GetSettings());
            }
            else
            {
                if (obj == null) return JsonConvert.SerializeObject(null);

                var config = _viewModelConfigService.Get(obj.GetType().GetBaseObjectType());

                var settings = GetSettings();

                settings.ContractResolver = _contractResolverFactory.GetBoContractResolver(config.Mnemonic);

                return JsonConvert.SerializeObject(obj, Formatting.None, settings);
            }
        }

        public BaseObject DeserializeObject(string value, Type type)
        {
            return JsonConvert.DeserializeObject(value, type, GetSettings()) as BaseObject;
        }
        public T DeserializeObject<T>(string value) where T : BaseObject
        {
            return JsonConvert.DeserializeObject<T>(value, GetSettings());
        }
    }

    class DynamicProxySerializationBinder : DefaultSerializationBinder
    {
        public override void BindToName(Type serializedType, out string assemblyName, out string typeName)
        {
            var type = serializedType.Namespace == "System.Data.Entity.DynamicProxies"
                ? serializedType.BaseType
                : serializedType;

            base.BindToName(type, out assemblyName, out typeName);
        }
    }
}