using Base.BusinessProcesses.Entities;
using Base.Service;
using Data.Entities;

namespace Data.Service.Abstract
{
    public interface IUserPromotionService : IBaseObjectService<UserPromotion>, IWFObjectService
    {
    }
}
