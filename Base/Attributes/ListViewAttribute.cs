using System;

namespace Base.Attributes
{
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = true)]
    public sealed class ListViewAttribute : Attribute
    {
        private bool? _exportable;
        private bool? _filterable;
        private bool? _sortable;
        private int? _width;
        private int? _height;
        private int? _order;
        private bool? _groupable;

        public ListViewAttribute()
        {   
        }

        public ListViewAttribute(string name)
        {   
            Name = name;
        }

        public bool Filterable
        {
            get { return _filterable ?? true; }
            set { _filterable = value; }
        }

        public bool Sortable
        {
            get { return _sortable ?? true; }
            set { _sortable = value; }
        }

        public string Name { get; set; }

        public bool Hidden { get; set; }
        public bool Visible { get { return !this.Hidden; } set { this.Hidden = !value; } }

        public int Order
        {
            get { return _order ?? -1; }
            set { _order = value; }
        }

        public int Width
        {
            get { return _width ?? 0; }
            set { _width = value; }
        }

        public int Height
        {
            get { return _height ?? 0; }
            set { _height = value; }
        }

        public bool Groupable
        {
            get { return _groupable ?? true; }
            set { _groupable = value; }
        }
    }
}
