using Base;
using Base.Attributes;
using System.Collections.Generic;

namespace Data.Entities.Product
{
    public class Equipment : BaseObject
    {
        [DetailView(Name = "Продукты")]
        public virtual ICollection<Product> Products { get; set; }
    }

    public class ProductionEquipment : EasyCollectionEntry<Equipment>
    {

    }
}
