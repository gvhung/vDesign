using Framework.Maybe;
using System;
using System.Reflection;

namespace Base.UI
{
    public class ColumnViewModel
    {
        private bool? _lockable;

        public ViewModelConfig ViewModelConfig { get; set; }

        private string _mnemonic;
        public string Mnemonic
        {
            get { return _mnemonic ?? ViewModelConfig.With(x => x.Mnemonic); }
            set { _mnemonic = value; }
        }

        public string Title { get; set; }

        public string PropertyName { get; set; }

        public Type PropertyType
        {
            get
            {
                return this.PropertyInfo.PropertyType;
            }
        }

        public PropertyInfo PropertyInfo { get; set; }
        public Type ColumnType { get; set; }
        public bool Hidden { get; set; }
        public bool Visible { get { return !this.Hidden; } set { this.Hidden = !value; } }
        public bool Filterable { get; set; }
        public bool Sortable { get; set; }
        public int Order { get; set; }
        public int? MaxLength { get; set; }
        public int? Width { get; set; }
        public int? Height { get; set; }
        public bool IsSystemPropery { get; set; }
        public bool Locked { get; set; }
        public bool Lockable
        {
            get
            {
                return _lockable ?? true;
            }
            set
            {
                _lockable = value;
            }
        }
        public bool Groupable { get; set; }
    }
}