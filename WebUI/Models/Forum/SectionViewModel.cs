
namespace WebUI.Models.Forum
{
    public class SectionViewModel : IForumItem
    {
        public int ID { get; set; }

        //public IEnumerable<TopicViewModel> Topics { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public RecordUserInfo CreateRecord { get; set; }
        
        public int TopicsCount { get; set; }

        public int PostsCount { get; set; }

        public int PremodPostsCount { get; set; }
    }
}