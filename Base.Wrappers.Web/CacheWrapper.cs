using Framework.Wrappers;
using System.Web.Caching;

namespace Base.Wrappers.Web
{
    public class CacheWrapper : ICacheWrapper
    {
        private Cache _cache;

        public void SetItem(object cache)
        {
            _cache = (Cache)cache;
        }

        public bool IsInitialized
        {
            get { return _cache != null; }
        }

        public int Count
        {
            get { return _cache.Count; }
        }

        public object this[string key]
        {
            get
            {
                return _cache[key];
            }
            set
            {
                if (value != null)
                {
                    _cache[key] = value;
                }
                else
                {
                    _cache.Remove(key);
                }
            }
        }

        public object Add(string key, object value)
        {
            // 
            _cache.Insert(key, value);

            return _cache[key];
        }

        public object Get(string key)
        {
            return _cache[key];
        }

        public void Insert(string key, object value)
        {
            _cache.Insert(key, value);
        }

        public void Remove(string key)
        {
            _cache.Remove(key);
        }
    }
}