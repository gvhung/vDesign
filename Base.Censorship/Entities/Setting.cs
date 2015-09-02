using Base.Settings.SettingValues;
using Framework;
using System;

namespace Base.Censorship.Entities
{
    [Serializable]
    public class Setting : CensorshipConfig, ISettingValue
    {
        public Setting() { }
        public Setting(CensorshipConfig val) { this.Value = val; }
        
        public string Type
        {
            get { return "Base.Censorship.Entities.Setting, Base.Censorship"; }
        }

        public CensorshipConfig Value
        {
            get
            {
                CensorshipConfig config = new CensorshipConfig();

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
