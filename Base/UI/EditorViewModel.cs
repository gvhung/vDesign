using Framework.Maybe;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Base.UI
{
    public class EditorViewModel
    {
        public string UID { get; set; }
        public ViewModelConfig ParentViewModelConfig { get; set; }
        public ViewModelConfig ViewModelConfig { get; set; }

        private string _mnemonic;

        private bool? _allowMultiple;

        public string Mnemonic
        {
            get { return _mnemonic ?? ViewModelConfig.With(x => x.Mnemonic); }
            set { _mnemonic = value; }
        }

        public string Title { get; set; }
        public string Description { get; set; }
        public string PropertyName { get; set; }
        public Type PropertyType { get; set; }
        public bool AllowMultiple
        {
            get
            {
                return
                    (bool)(_allowMultiple ?? (_allowMultiple = PropertyType.GetInterfaces()
                        .Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IEnumerable<>))));
            }
        }
        public bool IsLabelVisible { get; set; }
        public string EditorTemplate { get; set; }
        public Relationship Relationship { get; set; }
        public string EditorTemplateParams { get; set; }
        public string TabName { get; set; }
        public string GroupName { get; set; }
        public Type EditorType { get; set; }
        public bool IsReadOnly { get; set; }
        public bool IsRequired { get; set; }
        public bool JsonIgnore { get; set; }
        public int Order { get; set; }
        public int? MaxLength { get; set; }
        public bool DeferredLoading { get; set; }
        public int? Width { get; set; }
        public int? Height { get; set; }
        public bool IsSystemPropery { get; set; }
        public bool Visible { get; set; }
    }

    public enum Relationship
    {
        None = 0,
        One = 1,
        OneToMany = 2,
        ManyToMany = 3
    }
}
