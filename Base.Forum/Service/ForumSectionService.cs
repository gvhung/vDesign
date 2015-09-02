using Base.Ambient;
using Base.DAL;
using Base.Forum.Entities;
using Base.Service;
using System;

namespace Base.Forum.Service
{
    public class ForumSectionService : BaseObjectService<ForumSection>, IForumSectionService
    {
        public ForumSectionService(IBaseObjectServiceFacade facade) : base(facade) { }
        
        public ForumSection Create(IUnitOfWork unitOfWork, string title, string description)
        {
            ForumSection section = this.Create(unitOfWork,
                new ForumSection
                {
                    Title = title,
                    Description = description,
                    CreateDate = DateTime.Now,
                    CreateUserID = AppContext.SecurityUser.ID
                });

            return section;
        }
    }
}
