using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Base.DAL.SoapClient
{
    //TODO: доделать!!!
    public sealed class EntityKey : IEquatable<EntityKey>
    {
        private readonly int _hashCode;
        
        public EntityKey(object entity)
        {
            _hashCode = entity.GetType().ToString().GetHashCode();

            BaseObject bo = entity as BaseObject;

            if (bo != null)
            {
                if (bo.ID != 0)
                {
                    _hashCode ^= bo.ID;
                }
                else
                {
                    _hashCode ^= entity.GetHashCode();
                }
            }
        }

        public override bool Equals(object obj)
        {
            return InternalEquals(this, obj as EntityKey, compareEntitySets: true);
        }

        public override int GetHashCode()
        {
            return _hashCode;
        }

        public bool Equals(EntityKey other)
        {
            return InternalEquals(this, other, compareEntitySets: true);
        }

        internal static bool InternalEquals(EntityKey key1, EntityKey key2, bool compareEntitySets)
        {
            if (ReferenceEquals(key1, key2))
            {
                return true;
            }

            if (key1.GetHashCode() == key2.GetHashCode())
            {
                return true;
            }

            return false;
        }
    }
}
