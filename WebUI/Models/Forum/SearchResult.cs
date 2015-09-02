
namespace WebUI.Models.Forum
{
    public class SearchResult : IForumItem
    {
        public int SectionID { get; set; }

        public int TopicID { get; set; }
        
        public string Title { get; set; }

        public string Description { get; set; }

        public RecordUserInfo CreateRecord { get; set; }

        public RecordUserInfo LastRecord { get; set; }

        public int PostsCount { get; set; }

        public int ViewsCount { get; set; }
    }
}