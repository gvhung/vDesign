using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Base.Attributes;
using Base.Security;
using Framework;
using Newtonsoft.Json;

namespace Base.Content.Entities
{
    public class ContentSubscriber : BaseObject
    {
        public ContentSubscriber()
        {
            IsActive = true;
        }

        public int SubscriberId { get; set; }

        [DetailView(Name = "Пользователь")]
        [ForeignKey("SubscriberId")]
        [ListView(Name = "Подписчик", Order = 0)]
        public virtual User SubscriberUser { get; set; }

        [DetailView(Name = "Активен")]
        [ListView(Name = "Активен", Order = 0)]
        public bool IsActive { get; set; }

        public int ContentCategoryId { get; set; }

        [ForeignKey("ContentCategoryId")]
        [JsonIgnore]
        public ContentCategory ContentCategory { get; set; }
    }
}
