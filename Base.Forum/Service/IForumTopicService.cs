using Base.DAL;
using Base.Forum.Entities;
using Base.Service;

namespace Base.Forum.Service
{
    public interface IForumTopicService : IBaseObjectService<ForumTopic>
    {
        ForumTopic GetForViewing(IUnitOfWork unitOfWork, int id);
        ForumTopic Create(IUnitOfWork unitOfWork, int sectionID, string title, string description, string postMessage);
        ForumTopic Publish(IUnitOfWork unitOfWork, int id);
    }
}
