using Base.BusinessProcesses.Entities;
using Base.Service;



namespace Data.Service.Abstract.Product
{
    public interface IProductService : IBaseObjectService<Data.Entities.Product.Product>, IWFObjectService
    {
    }
}
