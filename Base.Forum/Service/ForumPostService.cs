using Base.Ambient;
using Base.DAL;
using Base.Forum.Entities;
using Base.Security;
using Base.Service;
using System;
using System.Linq;

namespace Base.Forum.Service
{
    public class ForumPostService : BaseObjectService<ForumPost>, IForumPostService
    {
        public ForumPostService(IBaseObjectServiceFacade facade) : base(facade) { }

        public override IQueryable<ForumPost> GetAll(IUnitOfWork unitOfWork, bool? hidden = false)
        {
            var posts = base.GetAll(unitOfWork, hidden);

            bool isModerator = AppContext.SecurityUser.IsPermission(typeof(ForumPost), TypePermission.Delete);
            if (!isModerator)
            {
                posts = posts.Where(x => x.Status == ForumPostStatus.Normal || x.CreateUserID == AppContext.SecurityUser.ID);
            }

            return posts;
        }

        public ForumPost Create(IUnitOfWork unitOfWork, int topicID, string message, bool isFirst = false)
        {
            ForumPost post = this.Create(unitOfWork,
                new ForumPost
                {
                    TopicID = topicID,
                    isFirst = isFirst,
                    Message = message,
                    CreateDate = DateTime.Now,
                    CreateUserID = AppContext.SecurityUser.ID,
                    Status = ForumPostStatus.Premoderate
                });

            return post;
        }

        public ForumPost Publish(IUnitOfWork unitOfWork, int id)
        {
            ForumPost post = this.Get(unitOfWork, id);
            post.Status = ForumPostStatus.Normal;
            this.Update(unitOfWork, post);

            return post;
        }

        public int CalcPostPage(IUnitOfWork unitOfWork, int topicID, int? postID, int itemsPerPage)
        {
            if (postID == null) //last
            {
                return (int)Math.Ceiling((double)this.GetAll(unitOfWork).Count(x => x.TopicID == topicID) / itemsPerPage);
            }
            else
            {
                return (int)Math.Ceiling(((double)this.GetAll(unitOfWork).Where(x => x.TopicID == topicID)
                                .Select(x => x.ID).OrderBy(x => x).ToList().FindIndex(x => x == postID.Value) + 1) / itemsPerPage);
            }
        }

        public int CalcPostPage(IUnitOfWork unitOfWork, int topicID, int itemsPerPage)
        {
            return CalcPostPage(unitOfWork, topicID, null, itemsPerPage);
        }
    }
}
