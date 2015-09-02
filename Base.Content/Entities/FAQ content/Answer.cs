using Base.Attributes;
using Base.Security;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Base.Content.Entities
{
    public class Answer : BaseObject, ICategorizedItem
    {
        [ListView(Hidden = true)]
        [DetailView(Name = "Автор", Order = 0, ReadOnly = true)]
        public virtual User Author { get; set; }

        [ListView]
        [DetailView(Name = "Вопрос", Order = 1)]
        [PropertyDataType(PropertyDataType.SimpleHtml)]
        public string MutualQuestion { get; set; }

        [ListView]
        [DetailView(Name = "Ответ", Order = 2)]
        [PropertyDataType(PropertyDataType.SimpleHtml)]
        public string Value { get; set; }

        [ListView(Hidden = true)]
        [DetailView(Name = "Дата", Order = 3, ReadOnly = true)]
        [PropertyDataType(PropertyDataType.DateTime)]
        public DateTime Date { get; set; }

        [DetailView(TabName = "[1]Прикрепленные вопросы", ReadOnly = true)]
        public virtual ICollection<Question> AttachedQuestions { get; set; }

        #region ICategorizedItem
        public int CategoryID { get; set; }

        [JsonIgnore]
        [ForeignKey("CategoryID")]
        public virtual ContentCategory ContentCategory { get; set; }

        HCategory ICategorizedItem.Category
        {
            get { return this.ContentCategory; }
        }
        #endregion
    }
}
