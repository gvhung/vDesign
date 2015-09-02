using System.Collections.Generic;
using WebUI.Controllers;

namespace WebUI.Models.Forum
{
    public class ForumViewModel : BaseViewModel
    {
        public int? SectionID { get; set; }

        public int? TopicID { get; set; }

        public ForumVMType Type { get; set; }

        public string Title { get; set; }

        public IEnumerable<IForumItem> Items { get; set; }

        public int CurrentPage { get; set; }

        public int PageCount { get; set; }

        public UserRole UserRole { get; set; }
        
        #region ctors
        public ForumViewModel(IBaseController controller)
            : base(controller)
        { }

        public ForumViewModel(BaseViewModel baseViewModel)
            : base(baseViewModel)
        { }
        #endregion
    }

    public enum ForumVMType
    {
        Root,
        Section,
        Topic,
        Search
    }

    public enum UserRole {
        Guest,
        User,
        Moderator
    }
}