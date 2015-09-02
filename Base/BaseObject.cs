using Base.Attributes;
using Framework;
using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;

namespace Base
{
    public interface IBaseObject
    {
        int ID { get; set; }
        bool Hidden { get; set; }
        int SortOrder { get; set; }

        byte[] RowVersion { get; set; }

        object ToObject(Type type);
        object ToObject(Type type, string[] exceptProperties = null);
        void ToObject<T>(T obj_dest, string[] exceptProperties = null) where T : IBaseObject;

        T ToObject<T>() where T : BaseObject;
    }

    [JsonObject]
    [Serializable]
    [DataContract]
    public abstract class BaseObject : IBaseObject
    {
        protected BaseObject()
        {
            SortOrder = -1;
        }

        [Key]
        [DataMember]
        [SystemProperty]
        [DetailView("Идентификатор", Visible = false, Order = -1), ListView(Visible = false)]
        public int ID { get; set; }

        [DataMember]
        [SystemProperty]
        public bool Hidden { get; set; }
        
        [DataMember]
        [SystemProperty]
        public int SortOrder { get; set; }

        [Timestamp]
        [SystemProperty]
        public byte[] RowVersion { get; set; }

        public object ToObject(Type type)
        {
            return ObjectHelper.CreateAndCopyObject(this, type);
        }
        public object ToObject(Type type, string[] exceptProperties = null)
        {
            return ObjectHelper.CreateAndCopyObject(this, type, new Type[] { typeof(IBaseObject) }, exceptProperties);
        }
        public T ToObject<T>() where T : BaseObject
        {
            return ObjectHelper.CreateAndCopyObject<T>(this);
        }

        public virtual void ToObject<T>(T obj_dest, string[] exceptProperties = null) where T : IBaseObject
        {
            ObjectHelper.CopyObject(this, obj_dest, new Type[] { typeof(IBaseObject) }, exceptProperties);
        }

        public virtual void ToObject<T>(T obj_dest, bool systemProperties) where T : IBaseObject
        {
            string[] exceptProperties = null;

            if (!systemProperties)
            {
                exceptProperties = BaseObject.GetSystemProperties().Select(p => p.Name).ToArray();
            }

            ObjectHelper.CopyObject(this, obj_dest, new Type[] { typeof(IBaseObject) }, exceptProperties);
        }

        public static PropertyInfo[] GetSystemProperties()
        {
            return typeof(BaseObject).GetProperties()
                .Where(m => m.GetCustomAttribute<SystemPropertyAttribute>() != null).ToArray();
        }

        public virtual void BeforeModelBinding() { }
    }
}
