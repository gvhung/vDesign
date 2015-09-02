using Base.Attributes;
using Base.Security;
using System.ComponentModel.DataAnnotations.Schema;

namespace Base.Content.Entities
{
    public class ContentSubscriber : BaseObject
    {
        public ContentSubscriber()
        {
            IsActive = true;
        }

        public int? SubscriberId { get; set; }
        [DetailView(Name = "Пользователь")]
        [ForeignKey("SubscriberId")]
        [ListView(Name = "Подписчик", Order = 0)]
        public virtual User SubscriberUser { get; set; }

        [DetailView(Name = "Email (неавторизованный пользователь)")]
        [ListView(Name = "Email (неавторизованный пользователь)")]
        public string Email { get; set; }

        public int? ContentCategoryId { get; set; }

        [ForeignKey("ContentCategoryId")]
        [ListView(Name = "Категория", Order = 1)]
        [InverseProperty("ContentSubscribers")]
        [DetailView(Name = "Категория", ReadOnly = true)]
        public virtual ContentCategory ContentCategory { get; set; }

        [DetailView(Name = "Активен")]
        [ListView(Name = "Активен", Order = 3)]
        public bool IsActive { get; set; }

        [NotMapped]
        [SystemProperty]
        public bool IsAuthorizedUser 
        {
            get { return this.SubscriberId.HasValue; }
        }
    }
}
