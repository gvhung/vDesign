
namespace Base.Localize.Entities
{
    public class LocalizePair<T>
    {
        public string Key { get; set; }
        public T Value { get; set; }
        public LocalizePair() { }

        public LocalizePair(string key, T value)
        {
            this.Key = key;

            this.Value = value;
        }
    }

    public class LocalizePairString : LocalizePair<string>
    {
        public LocalizePairString(string key, string value) : base(key, value) { }

        public LocalizePairString() { }
    }
}
