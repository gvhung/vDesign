using Base.BusinessProcesses.Services.Abstract;
using Framework.Wrappers;
using System.Collections.Generic;

namespace Base.BusinessProcesses.Services.Concrete
{


    public class WorkflowCacheService : IWorkflowCacheService
    {
        private readonly ICacheWrapper _cacheWrapper;
        private const string CacheKey = "{A5EC7C63-F628-48DE-8CE0-9CBF49D1B7C4}";


        public WorkflowCacheService(ICacheWrapper cacheWrapper)
        {
            _cacheWrapper = cacheWrapper;
            InitCache();
        }

        private Dictionary<string, CacheSubCollection> Cache
        {
            get
            {
                return _cacheWrapper.Get(CacheKey) as Dictionary<string, CacheSubCollection> ?? InitCache();
            }
        }

        private Dictionary<string, CacheSubCollection> InitCache()
        {
            var res = new Dictionary<string, CacheSubCollection>();
            _cacheWrapper[CacheKey] = res;
            return res;
        }

        public string Get(string key)
        {
            if (Cache.ContainsKey(key))
            {
                var coll = Cache[key];
                return coll.Get(key);
            }
            return null;
        }

        public string Get(string key, string wfID)
        {
            if (Cache.ContainsKey(key))
            {
                var coll = Cache[key];
                return coll.Get(wfID);
            }
            return null;
        }

        public void Add(string key, string json)
        {
            var sub = getSubCollection(key); 
            sub.Add(key, json);
            Cache[key] = sub;
        }

        public void Add(string key, string json, int wfID)
        {
            var sub = getSubCollection(key);
            sub.Add(wfID.ToString(), json);
            Cache[key] = sub;
        }

        public void Clear(string key)
        {
            if(Cache.ContainsKey(key))
            Cache.Remove(key);
        }

        private CacheSubCollection getSubCollection(string key)
        {
            return Cache.ContainsKey(key) ? Cache[key] : new CacheSubCollection();
        }
    }

    internal class CacheSubCollection
    {
        private static readonly object Locker = new object();
        public Dictionary<string, string> Cache = new Dictionary<string, string>();

        public string Get(string key)
        {
            if (Cache.ContainsKey(key))
            {
                return Cache[key];
            }
            return null;
        }

        public void Add(string key, string value)
        {
            lock (Locker)
            {
                Cache[key] = value;
            }
        }

        public void Clear(string key)
        {
            lock (Locker)
            {
                Cache.Remove(key);
            }
        }

    }

    public enum WorkflowCacheType
    {
        Toolbar,
        TimeLine,
    }
}
