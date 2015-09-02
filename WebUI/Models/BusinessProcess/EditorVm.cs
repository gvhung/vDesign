using Base;
using Base.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace WebUI.Models.BusinessProcess
{
    public class ObjectInitializerVm
    {
        public Type Type { get; set; }
        public IEnumerable<EditorVm> Editors { get; set; }
        public string Mnemonic { get; set; }
    }

    public class WithCustomEditorVm
    {
        public string Property { get; set; }
        public ViewModelConfig Config { get; set; }
        public IEnumerable<EditorVm> Editors { get; set; }
        
    }

    public class EditorVm
    {
        public string Name { get; private set; }
        public string Member { get; private set; }
        public Type Type { get; private set; }
        public string Property { get; set; }

        public EditorVm(string name, string member)
        {
            this.Name = name;
            this.Member = member;
        }

        public EditorVm(string name, PropertyInfo property)
            : this(name, property.Name)
        {
            this.Type = property.PropertyType;
        }

        protected bool Equals(EditorVm other)
        {
            return string.Equals(Member, other.Member) && Equals(Type, other.Type);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((EditorVm) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Member != null ? Member.GetHashCode() : 0)*397) ^ (Type != null ? Type.GetHashCode() : 0);
            }
        }
    }

    public class BaseObjectEditorVm : EditorVm
    {
        public ViewModelConfig Config { get; set; }
        public BaseObjectEditorVm(string name, PropertyInfo property, IEnumerable<ViewModelConfig> configs)
            : base(name, property)
        {
            this.Config = this.Type.TypeHierarchy().Join(configs, type => type, config => config.TypeEntity, (t, c) => c).FirstOrDefault();
        }
    }
}