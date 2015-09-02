using Base.Security;
using Framework.Attributes;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Base.Forum.Entities
{
    [EnableFullTextSearch]
    public class ForumPost : BaseObject
    {
        public ForumPost()
        { }

        public virtual ForumTopic Topic { get; set; }

        [ForeignKey("Topic")]
        public int TopicID { get; set; }

        [FullTextSearchProperty]
        public string Message { get; set; }

        public DateTime CreateDate { get; set; }

        public virtual User CreateUser { get; set; }

        [ForeignKey("CreateUser")]
        public int CreateUserID { get; set; }

        public DateTime? ModifyDate { get; set; }

        public virtual User ModifyUser { get; set; }

        [ForeignKey("ModifyUser")]
        public int? ModifyUserID { get; set; }

        public ForumPostStatus Status { get; set; }

        public bool isFirst { get; set; }
    }

    public enum ForumPostStatus
    {
        Normal,
        Premoderate
    }
}
