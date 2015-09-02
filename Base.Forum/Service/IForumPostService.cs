using Base.DAL;
using Base.Forum.Entities;
using Base.Service;

namespace Base.Forum.Service
{
    public interface IForumPostService : IBaseObjectService<ForumPost>
    {
        ForumPost Create(IUnitOfWork unitOfWork, int topicID, string message, bool isFirst = false);
        ForumPost Publish(IUnitOfWork unitOfWork, int id);
        int CalcPostPage(IUnitOfWork unitOfWork, int topicID, int itemsPerPage);
        int CalcPostPage(IUnitOfWork unitOfWork, int topicID, int? postID, int itemsPerPage);
    }
}
