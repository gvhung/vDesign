using Base.Attributes;
using System;
using System.ComponentModel.DataAnnotations;

namespace Base.Settings.SettingValues
{
    [Serializable]
    public class String : ISettingValue
    {
        public String() { }

        public String(string val)
        {
            this.Value = val;
        }
       
        [MaxLength(255)]
        [DetailView("Значение")]
        public string Value { get; set; }

        public string Type
        {
            get { return this.GetType().GetTypeName(); }
        }

        object ISettingValue.Value
        {
            get { return this.Value; }
        }
    }
}
