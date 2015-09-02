using Base.Settings.SettingValues;
using Framework;
using System;

namespace Base.Notification.Entities
{
    [Serializable]
    public class Setting : EmailConfig, ISettingValue
    {
        public Setting() { }
        public Setting(EmailConfig val) { this.Value = val; }
        
        public string Type
        {
            get { return typeof(Setting).GetTypeName(); }
        }

        public EmailConfig Value
        {
            get
            {
                EmailConfig config = new EmailConfig();

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
