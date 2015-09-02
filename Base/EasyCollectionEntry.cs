using System.ComponentModel.DataAnnotations.Schema;

namespace Base
{
    public abstract class EasyCollectionEntry<TEntity> : BaseObject where TEntity : BaseObject
    {
        [ForeignKey("ObjectID")]
        public virtual TEntity Object { get; set; }
        public int? ObjectID { get; set; }
    }
}