using Base.Attributes;
using System;

namespace Base.Settings.SettingValues
{
    [Serializable]
    public class Boolean: ISettingValue
    {
        public Boolean()
        {

        }

        public Boolean(bool val)
        {
            Value = val;
        }

        [DetailView("Значение")]
        public bool Value { get; set; }

        public string Type
        {
            get { return typeof(Boolean).GetTypeName(); }
        }

        object ISettingValue.Value
        {
            get { return this.Value; }
        }
    }
}
