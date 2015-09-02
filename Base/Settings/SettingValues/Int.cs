using Base.Attributes;
using System;

namespace Base.Settings.SettingValues
{
    [Serializable]
    public class Int: ISettingValue
    {
        public Int() { }
        public Int(int val) { Value = val; }

        [DetailView("Значение")]
        public int Value { get; set; }

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
