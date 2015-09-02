using Base.Forum.Entities;

namespace WebUI.Models.Forum
{
    public class PostViewModel : IForumItem
    {
        public int ID { get; set; }

        public int TopicID { get; set; }

        public string Message { get; set; }
        
        public RecordUserInfo CreateRecord { get; set; }
        
        public ForumPostStatus Status { get; set; }

        public bool IsFirst { get; set; }
    }
}