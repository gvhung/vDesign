using System;
using System.Collections.Generic;
using System.Reflection;
using Base.BusinessProcesses.Entities;
using Base.UI;
using Base.Validation;


namespace WebUI.Models.Validation
{
    public class PropertyEditorVm
    {
        public string PropertyType { get; set; }
        public string PropertyName { get; set; }
        public string Member { get; set; }


        public PropertyEditorVm(string name, PropertyInfo info)
        {
            PropertyName = name;
            Member = info.Name;
            PropertyType = info.PropertyType.FullName;
        }
    }

    public class ValidationVm
    {
        public ICollection<PropertyEditorVm> Editors { get; set; }
        public Type ObjectType { get; set; }
        public string Mnemonic { get; set; }
        public ICollection<ValidationRuleVm> ObjectValidationRules { get; set; }
    }

    public class ValidationRuleVm
    {
        public string Title { get; set; }
        public string Type { get; set; }
    }





}