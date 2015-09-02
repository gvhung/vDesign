using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Base.UI
{
    [Table("PresetRegistor")]
    public class PresetRegistor: BaseObject
    {
        [Column]
        private byte[] value_ { get; set; }

        public string Key { get; set; }

        [NotMapped]
        public Preset Preset
        {
            get
            {
                if (value_ != null)
                {
                    using (MemoryStream ms = new MemoryStream(value_))
                    {
                        BinaryFormatter bin = new BinaryFormatter();

                        try
                        {
                            return bin.Deserialize(ms) as Preset;
                        }
                        catch (Exception e)
                        {
                            return null;
                        }
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
    }
}
