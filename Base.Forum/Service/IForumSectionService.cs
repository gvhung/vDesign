using Base.DAL;
using Base.Forum.Entities;
using Base.Service;

namespace Base.Forum.Service
{
    public interface IForumSectionService : IBaseObjectService<ForumSection>
    {
        ForumSection Create(IUnitOfWork unitOfWork, string title, string description);
    }
}
