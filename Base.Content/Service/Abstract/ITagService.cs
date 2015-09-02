using Base.Content.Entities;
using Base.DAL;
using Base.Service;
using System.Collections.Generic;

namespace Base.Content.Service.Abstract
{
    public interface ITagService : IBaseObjectService<Tag>
    {
        IEnumerable<Tag> GetTags(IUnitOfWork unitOfWork, int? contentCategoryID = null, string filter = null);
    }
}
