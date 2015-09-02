using Base.Attributes;
using Base.Settings.SettingValues;
using Base.UI;
using Framework.Attributes;
using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Base.Settings
{
    [EnableFullTextSearch]
    public class SettingItem : BaseObject, ICategorizedItem
    {
        [Column]
        private byte[] value_ { get; set; }

        [DetailView(Required = true)]
        public Guid Key { get; set; }

        [FullTextSearchProperty]
        [DetailView(Name = "Наименование", Required = true)]
        [ListView]
        [MaxLength(255)]
        public string Text { get; set; }

        [DetailView(Name = "Значение")]
        [PropertyDataType("SettingValue")]
        [NotMapped]
        public ISettingValue Value
        {
            get
            {
                if (value_ != null)
                {
                    using (MemoryStream ms = new MemoryStream(value_))
                    {
                        BinaryFormatter bin = new BinaryFormatter();

                        return bin.Deserialize(ms) as ISettingValue;
                    }
                }

                return null;
            }
            set
            {
                using (var ms = new MemoryStream())
                {
                    BinaryFormatter bin = new BinaryFormatter();

                    bin.Serialize(ms, value);

                    value_ = ms.ToArray();
                }
            }
        }

        public int CategoryID { get; set; }

        [JsonIgnore]
        [ForeignKey("CategoryID")]
        public virtual SettingCategory Category_ { get; set; }

        #region ICategorizedItem
        HCategory ICategorizedItem.Category
        {
            get { return this.Category_; }
        }
        #endregion
    }
}
