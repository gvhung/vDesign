using Base.Attributes;
using Base.Security;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace Base.Content.Entities
{
    // Запись в журнале
    // Пользователь -> результаты выполнения заданий по уроку/занятию
    public class JournalEntry : BaseObject, ICategorizedItem
    {
        public int? UserID { get; set; }

        [ListView]
        [DetailView(Name = "Пользователь", Order = 0)]
        public virtual User User { get; set; }

        [ListView]
        [DetailView(Name = "Дата начала", Order = 1)]
        [PropertyDataType(PropertyDataType.DateTime)]
        public DateTime Start { get; set; }

        [ListView]
        [DetailView(Name = "Дата окончания", Order = 2)]
        [PropertyDataType(PropertyDataType.DateTime)]
        public DateTime? End { get; set; }

        [NotMapped]
        [ListView(Name = "Кол-во баллов", Order = 3)]
        public int Points
        {
            get 
            {
                if (this.ExerciseResults == null)
                {
                    return 0;
                }
                else
                {
                    return this.ExerciseResults.Sum(x => x.Points);
                }
            }
        }

        [DetailView(TabName = "[1]Результаты")]
        public virtual ICollection<ExerciseResult> ExerciseResults { get; set; }

        #region ICategorizedItem
        public int CategoryID { get; set; }

        [JsonIgnore]
        [ForeignKey("CategoryID")]
        public virtual CourseCategory CourseCategory { get; set; }

        HCategory ICategorizedItem.Category
        {
            get { return this.CourseCategory; }
        }
        #endregion
    }
}
