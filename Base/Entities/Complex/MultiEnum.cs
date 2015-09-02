using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace Base.Entities.Complex
{
    [ComplexType]
    [Serializable]
    public class MultiEnum
    {
        public MultiEnum() { }

        public MultiEnum(Type enumType)
        {
            if (!enumType.IsEnum)
                throw new ArgumentException("enumType");

            this.SetType(enumType);
        }

        [Column]
        [JsonIgnore]
        private string type { get; set; }
        [Column]
        [JsonIgnore]
        private string values { get; set; }

        private void SetType(Type enumType)
        {
            this.type = enumType.GetTypeName();
        }

        [JsonIgnore]
        public Type Type
        {
            get { return Type.GetType(this.type); }
        }

        public int[] GetValue()
        {
            if (!String.IsNullOrEmpty(this.values))
                return this.values.Split(';').Select(x => Int32.Parse(x)).ToArray();

            return null;
        }

        public string[] Value
        {
            get
            {
                if (!String.IsNullOrEmpty(this.values))
                    return this.values.Split(';');

                return null;
            }
            set
            {
                if (value != null)
                    this.values = String.Join(";", value);
                else
                    this.values = null;
            }
        }

        public bool HasValue { get { return Value != null; } }
    }
}
