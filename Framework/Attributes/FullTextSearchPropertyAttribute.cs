using System;

namespace Framework.Attributes
{
    [AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = true)]
    public sealed class FullTextSearchPropertyAttribute : Attribute
    {
        private int _depth;

        public FullTextSearchPropertyAttribute(int depth)
        {
            _depth = depth;
        }

        public FullTextSearchPropertyAttribute()
        {
            _depth = 1;
        }

        public int Depth
        {
            get
            {
                return _depth;
            }
        }
    }
}
