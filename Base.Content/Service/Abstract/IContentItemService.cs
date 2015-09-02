using Base.BusinessProcesses.Entities;
using Base.Content.Entities;
using Base.DAL;
using Base.Service;
using System.Linq;

namespace Base.Content.Service.Abstract
{
    public interface IContentItemService : IBaseCategorizedItemService<ContentItem>, IWFObjectService
    {
        IQueryable<ContentItem> GetAllNews(IUnitOfWork unitOfWork);
        IQueryable<ContentItem> GetAllPersons(IUnitOfWork unitOfWork);
        IQueryable<ContentItem> GetAllVideo(IUnitOfWork unitOfWork);
        IQueryable<ContentItem> GetAllPublications(IUnitOfWork unitOfWork);
    }
}
    