using Base.Security;
using Framework.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Base.Forum.Entities
{
    [EnableFullTextSearch]
    public class ForumTopic : BaseObject
    {
        public ForumTopic()
        { }

        public virtual ForumSection Section { get; set; }

        [ForeignKey("Section")]
        public int SectionID { get; set; }

        [FullTextSearchProperty]
        public virtual ICollection<ForumPost> Posts { get; set; }


        [FullTextSearchProperty]
        [MaxLength(100)]
        public string Title { get; set; }

        [FullTextSearchProperty]
        [MaxLength(300)]
        public string Description { get; set; }


        public DateTime CreateDate { get; set; }

        public virtual User CreateUser { get; set; }

        [ForeignKey("CreateUser")]
        public int? CreateUserID { get; set; }


        public DateTime? LastDate { get; set; }

        public virtual User LastUser { get; set; }

        [ForeignKey("LastUser")]
        public int? LastUserID { get; set; }


        public int ViewsCount { get; set; }

        public ForumTopicStatus Status { get; set; }
    }

    public enum ForumTopicStatus
    {
        Normal,
        Premoderate
    }
}
