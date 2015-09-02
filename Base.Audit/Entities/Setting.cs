using Base.Settings.SettingValues;
using Framework;
using System;

namespace Base.Audit.Entities
{
    [Serializable]
    public class Setting : Config, ISettingValue
    {
        public Setting() { }
        public Setting(Config val) { this.Value = val; }
        
        public string Type
        {
            get { return typeof(Setting).GetTypeName(); }
        }

        public Config Value
        {
            get
            {
                Config config = new Config();

                ObjectHelper.CopyObject(this, config);

                return config;
            }

            set
            {
                ObjectHelper.CopyObject(value, this);
            }
        }

        object ISettingValue.Value
        {
            get { return Value; }
        }
    }
}
