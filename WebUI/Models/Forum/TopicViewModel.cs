using Base.Forum.Entities;

namespace WebUI.Models.Forum
{
    public class TopicViewModel : IForumItem
    {
        public int ID { get; set; }

        public int SectionID { get; set; }

        //public IEnumerable<PostViewModel> Posts { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }
        
        public RecordUserInfo CreateRecord { get; set; }
        
        public RecordUserInfo LastRecord { get; set; }

        public int PostsCount { get; set; }

        public int ViewsCount { get; set; }

        public ForumTopicStatus Status { get; set; }

        public int PremodPostsCount { get; set; }
    }
}