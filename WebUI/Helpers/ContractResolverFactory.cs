using Base.UI.Service;
using Framework.Wrappers;
using System;

namespace WebUI.Helpers
{
    public interface IContractResolverFactory
    {
        BoContractResolver GetBoContractResolver(string mnemonic);
        ListBoContractResolver GetListBoContractResolver(string mnemonic);
    }

    public class ContractResolverFactory : IContractResolverFactory
    {
        private readonly ICacheWrapper _cache;
        private readonly IUiFasade _uiFasade;

        private const string KeyCache = "{B267917C-B89E-4699-9633-41BEEAF3CB1B}";
        
        private static readonly object LockCache = new object();
        private static readonly object LockCache2 = new object();

        public ContractResolverFactory(ICacheWrapper cache, IUiFasade uiFasade)
        {
            _cache = cache;
            _uiFasade = uiFasade;
        }

        public BoContractResolver GetBoContractResolver(string mnemonic)
        {
            string key = String.Format("GetBoContractResolver:{0}:{1}", KeyCache, mnemonic);

            if (_cache[key] != null)
                return _cache[key] as BoContractResolver;

            lock (LockCache)
            {
                _cache[key] = new BoContractResolver(mnemonic, _uiFasade);
            }

            return (BoContractResolver)_cache[key];
        }

        public ListBoContractResolver GetListBoContractResolver(string mnemonic)
        {
            string key = String.Format("GetListBoContractResolver:{0}:{1}", KeyCache, mnemonic);

            if (_cache[key] != null)
                return _cache[key] as ListBoContractResolver;

            lock (LockCache2)
            {
                _cache[key] = new ListBoContractResolver(mnemonic, _uiFasade);
            }

            return (ListBoContractResolver)_cache[key];
        }
    }
}