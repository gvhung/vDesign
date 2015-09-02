using Base.Security;
using Framework.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Base.Forum.Entities
{
    [EnableFullTextSearch]
    public class ForumSection : BaseObject
    {
        public ForumSection()
        { }

        [FullTextSearchProperty]
        public virtual ICollection<ForumTopic> Topics { get; set; }

        [MaxLength(100)]
        public string Title { get; set; }

        [MaxLength(300)]
        public string Description { get; set; }

        public DateTime CreateDate { get; set; }

        public virtual User CreateUser { get; set; }

        [ForeignKey("CreateUser")]
        public int? CreateUserID { get; set; }
    }
}
