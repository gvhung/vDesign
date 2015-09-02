
namespace Framework.Wrappers
{
    public interface ICacheWrapper
    {
        void SetItem(object cache);

        bool IsInitialized { get; }

        int Count { get; }

        object this[string key] { get; set; }

        object Add(string key, object value);

        object Get(string key);

        void Insert(string key, object value);

        void Remove(string key);
    }
}
