using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Base.Entities.Complex
{
    [ComplexType]
    public class LinkBaseObject
    {
        public LinkBaseObject() { }

        public LinkBaseObject(BaseObject obj)
        {
            if (obj != null)
            {
                Type objType = obj.GetType().GetBaseObjectType();

                this.ID = obj.ID;
                this.FullName = objType.FullName;
                this.Assembly = objType.Assembly.FullName;
            }
        }

        public LinkBaseObject(Type type, int id)
        {
            Type objType = type.GetBaseObjectType();

            this.ID = id;
            this.FullName = objType.FullName;
            this.Assembly = objType.Assembly.FullName;
        }

        public int ID { get; set; }
        public string FullName { get; set; }
        [JsonIgnore]
        public string Assembly { get; set; }
        public string Mnemonic { get; set; }
        public Type GetTypeBO()
        {
            return Type.GetType(String.Format("{0}, {1}", this.FullName, this.Assembly));
        }

        public override string ToString()
        {
            return String.Format("{0}_{1}", this.FullName, this.ID);
        }
    }
}
