using Base.Ambient;
using Base.DAL;
using Base.Forum.Entities;
using Base.Security;
using Base.Service;
using System;
using System.Linq;

namespace Base.Forum.Service
{
    public class ForumTopicService : BaseObjectService<ForumTopic>, IForumTopicService
    {
        private readonly IForumPostService _postService;

        public ForumTopicService(IBaseObjectServiceFacade facade, IForumPostService postService) : base(facade) 
        {
            _postService = postService;
        }

        public override IQueryable<ForumTopic> GetAll(IUnitOfWork unitOfWork, bool? hidden = false)
        {
            var topics = base.GetAll(unitOfWork, hidden);

            bool isModerator = AppContext.SecurityUser.IsPermission(typeof(ForumTopic), TypePermission.Delete);
            if (!isModerator)
            {
                topics = topics.Where(x => x.CreateUserID == AppContext.SecurityUser.ID || x.Status == ForumTopicStatus.Normal);
            }
            return topics;
        }

        public ForumTopic GetForViewing(IUnitOfWork unitOfWork, int id)
        {   
            ForumTopic topic = base.Get(unitOfWork, id);

            topic.ViewsCount++;
            base.Update(unitOfWork, topic);
            
            return topic;
        }

        public ForumTopic Create(IUnitOfWork unitOfWork, int sectionID, string title, string description, string postMessage)
        {
            ForumTopic topic = base.Create(unitOfWork,
                new ForumTopic
                {
                    SectionID = sectionID,
                    Title = title,
                    Description = description,
                    CreateDate = DateTime.Now,
                    CreateUserID = AppContext.SecurityUser.ID,
                    Status = ForumTopicStatus.Premoderate
                });

            _postService.Create(unitOfWork, topic.ID, postMessage, true);
            
            return topic;
        }

        public ForumTopic Publish(IUnitOfWork unitOfWork, int id)
        {
            ForumTopic topic = this.Get(unitOfWork, id);
            topic.Status = ForumTopicStatus.Normal;
            this.Update(unitOfWork, topic);

            int? firstPostID = topic.Posts.Where(x => x.isFirst).Select(x => x.ID).FirstOrDefault();
            if (firstPostID != null)
            {
                _postService.Publish(unitOfWork, firstPostID.Value);
            }

            return topic;
        }
    }
}
