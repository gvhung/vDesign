using Base.Attributes;
using Base.Security;
using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Base.Content.Entities
{
    public class Question : BaseObject, ICategorizedItem
    {
        public int? UserID { get; set; }

        [ForeignKey("UserID")]
        [ListView]
        [DetailView(Name = "Пользователь", Order = 0, ReadOnly = true)]
        public virtual User User { get; set; }

        [ListView]
        [DetailView(Name = "Вопрос", Order = 1, ReadOnly = true)]
        [PropertyDataType(PropertyDataType.SimpleHtml)]
        public string Value { get; set; }

        public int? AnswerID { get; set; }

        [DetailView(Name = "Ответ", Order = 2)]
        [InverseProperty("AttachedQuestions")]
        [PropertyDataType("Answer")]
        public virtual Answer Answer { get; set; }

        [ListView(Order = 2)]
        [DetailView(Name = "Дата", Order = 3, ReadOnly = true)]
        [PropertyDataType(PropertyDataType.DateTime)]
        public DateTime Date { get; set; }

        [NotMapped]
        [SystemProperty]
        public bool IsSolved
        {
            get { return AnswerID != null; }
        }

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
