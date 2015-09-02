using System.Linq;

namespace Base.Security
{
    public abstract class BaseUser: BaseObject
    {
        public override void ToObject<T>(T obj_dest, string[] exceptProperties = null)
        {
            exceptProperties = exceptProperties == null ? new[] { "Password" } : exceptProperties.Concat(new[] { "Password" }).ToArray();

            base.ToObject(obj_dest, exceptProperties);
        }

        public override void ToObject<T>(T obj_dest, bool systemProperties)
        {
            this.ToObject<T>(obj_dest, BaseObject.GetSystemProperties().Select(p => p.Name).ToArray());
        }
    }
}
