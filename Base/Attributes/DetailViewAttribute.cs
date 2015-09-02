using System;

namespace Base.Attributes
{
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = true)]
    public sealed class DetailViewAttribute : Attribute
    {
        private bool? _visible;
        private int? _width;
        private int? _height;

        public DetailViewAttribute() { }

        public DetailViewAttribute(string name)
        {
            Name = name;
        }

        public string Name { get; set; }

        public string Mnemonic { get; set; }

        public string Description { get; set; }

        public string TabName { get; set; }

        public string GroupName { get; set; }

        public bool Visible
        {
            get { return _visible ?? !String.IsNullOrEmpty(Name) || !String.IsNullOrEmpty(Description) || !String.IsNullOrEmpty(TabName); }
            set { _visible = value; }
        }

        public bool ReadOnly { get; set; }

        public bool Required { get; set; }

        public int Order { get; set; }

        public bool HideLabel { get; set; }

        public bool DeferredLoading { get; set; }

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
    }
}
