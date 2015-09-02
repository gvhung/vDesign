using Base.OpenID.Entities;
using Base.OpenID.Service.Abstract;
using Base.Service;
using Framework.Wrappers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Base.OpenID.Service.Concrete
{
    public class OpenIdConfigService : IOpenIdConfigService
    {
        private static readonly string CacheKey;

        private readonly IPathHelper _pathHelper;
        private readonly ICacheWrapper _cache;

        static OpenIdConfigService()
        {
            CacheKey = Guid.NewGuid().ToString("N");
        }

        public OpenIdConfigService(IPathHelper pathHelper, ICacheWrapper cache)
        {
            _pathHelper = pathHelper;
            _cache = cache;
        }

        public IEnumerable<OpenIdConfig> GetConfig()
        {
            return _cache.Get(CacheKey) as List<OpenIdConfig> ?? _InitConfig();
        }

        public OpenIdConfig GetConfig(ServiceType type)
        {
            var config = GetConfig();
            return config.FirstOrDefault(x => x.Type == type);
        }

        private List<OpenIdConfig> _InitConfig()
        {
            string path = Path.Combine(_pathHelper.GetAppDataDirectory(), "OpenID.json");

            string configContent;
            using (var fs = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (var sr = new StreamReader(fs))
            {
                fs.Position = 0;
                configContent = sr.ReadToEnd();
            }

            //TODO: need rework

            List<OpenIdConfig> config = 
                JsonConvert.DeserializeObject<IEnumerable<OpenIdConfig>>(configContent)
                    .Where(x => x.Type != ServiceType.Esia).ToList();

            var esia =
                JsonConvert.DeserializeObject<IEnumerable<EsiaConfig>>(configContent)
                    .FirstOrDefault(x => x.Type == ServiceType.Esia);

            if (esia != null) config.Insert(0, esia);

            _cache.Add(CacheKey, config);

            return config;
        }
    }
}
