using System;

namespace Framework.Attributes
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    public sealed class EnableFullTextSearchAttribute : Attribute
    {
        private readonly bool _enableFullTextSearch;

        public EnableFullTextSearchAttribute(bool enableFullTextSearch)
        {
            _enableFullTextSearch = enableFullTextSearch;
        }

        public EnableFullTextSearchAttribute()
        {
            _enableFullTextSearch = true;
        }

        public bool Enabled
        {
            get { return _enableFullTextSearch; }
        }
    }
}
