using Base.Attributes;
using Base.Entities.Complex;
using Base.Security;
using Base.Security.ObjectAccess;
using Framework.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace Base.Task.Entities
{
    [EnableFullTextSearch]
    public class Task : BaseObject, ITask, ICategorizedItem, IEnabledState
    {
        public Task()
        {
            this.Period = new Period();
            this.Priority = Priority.Normal;
        }

        [FullTextSearchProperty]
        [ListView(Order = 1, Width = 600)]
        [DetailView(Name = "Наименование", Required = true)]
        //[MaxLength(int.MaxValue)]
        public string Title { get; set; }

        [ListView(Order = 4)]
        [DetailView(Name = "Период", Required = true)]
        public Period Period { get; set; }

        [ListView(Name = "% выполнения", Order = 5)]
        [DetailView(Name = "Процент выполнения")]
        [PropertyDataType("Percent")]
        public double PercentComplete { get; set; }

        [ListView(Hidden = true)]
        [DetailView(Name = "Дата завершения", ReadOnly = true)]
        public DateTime? CompliteDate { get; set; }

        public int? AssignedFromID { get; set; }
        [FullTextSearchProperty]
        [ListView(Hidden = true, Order = 0)]
        [DetailView(Name = "Автор", ReadOnly = true)]
        public virtual User AssignedFrom { get; set; }

        public int? AssignedToID { get; set; }
        [FullTextSearchProperty]
        [ListView(Hidden = true, Order = 0)]
        [DetailView(Name = "Исполнитель")]
        public virtual User AssignedTo { get; set; }

        [ListView(Order = 3)]
        [DetailView(Name = "Приоритет")]
        public Priority Priority { get; set; }

        [ListView(Order = 2)]
        [DetailView(Name = "Статус", ReadOnly = true)]
        public TaskStatus Status { get; set; }

        [FullTextSearchProperty]
        [DetailView("Описание", TabName = "[2]Описание", HideLabel = true)]
        [PropertyDataType(PropertyDataType.SimpleHtml)]
        [ListView(Hidden = true)]
        public string Description { get; set; }

        [FullTextSearchProperty]
        [DetailView(TabName = "[3]Файлы")]
        public virtual ICollection<TaskFile> Files { get; set; }

        [DetailView(TabName = "[4]История", ReadOnly = true)]
        public virtual ICollection<TaskChangeHistory> TaskChangeHistory { get; set; }

        public bool Auto { get; set; }

        //public TaskType TaskType { get; set; }
        [SystemProperty]
        public bool System { get; set; }

        #region Statuses
        [NotMapped]
        [DetailView("Статус просрочки")]
        [ListView(Order = 3, Filterable = false, Sortable = false, Width = 240)]
        public string StatusMessage
        {
            get
            {
                if (!this.Period.End.HasValue)
                    return "Срок окончания не определен";

                DateTime comparisonDate = (this.CompliteDate ?? DateTime.Now).Date;

                int delay = Math.Abs((comparisonDate - this.Period.End.Value.Date).Days);

                switch (this.TaskDelay)
                {
                    case TaskDelayStatus.FullTime:
                        return String.Format("Осталось {0} д.", delay);
                    case TaskDelayStatus.HalfTime:
                        return String.Format("Прошло больше половины времени, осталось {0} д.", delay);
                    case TaskDelayStatus.NoTime:
                    case TaskDelayStatus.OneDayDelay:
                    case TaskDelayStatus.TwoDayDelay:
                    case TaskDelayStatus.ManyDaysDelay:
                        return String.Format("Срок {1}исчерпан, просрочка {0} д.", delay, delay == 0 ? "почти " : "");

                    default:
                        return "Срок окончания не определен";
                }
            }
        }

        [SystemProperty]
        [NotMapped]
        public TaskDelayStatus TaskDelay
        {
            get
            {
                DateTime comparisonDate = (this.CompliteDate ?? DateTime.Now).Date;

                DateTime start = this.Period.Start.Date;

                if (this.Period.Start >= comparisonDate || !this.Period.End.HasValue)
                    return TaskDelayStatus.FullTime;

                DateTime end = this.Period.End.Value.Date;

                TimeSpan interval = end - start;

                if (start + new TimeSpan(interval.Ticks / 2) >= comparisonDate)
                    return TaskDelayStatus.FullTime;

                if (end >= comparisonDate)
                    return TaskDelayStatus.HalfTime;

                if (end.AddDays(1) <= comparisonDate && end.AddDays(2) > comparisonDate)
                    return TaskDelayStatus.OneDayDelay;

                if (end.AddDays(2) <= comparisonDate && end.AddDays(3) > comparisonDate)
                    return TaskDelayStatus.TwoDayDelay;

                return TaskDelayStatus.ManyDaysDelay;
            }
        }
        #endregion

        public string LastComment
        {
            get
            {
                if (this.TaskChangeHistory != null)
                {
                    TaskChangeHistory itemHistory = this.TaskChangeHistory.LastOrDefault();

                    if (itemHistory != null)
                        return itemHistory.Сomment;

                }

                return null;
            }
        }

        [ForeignKey("CategoryID")]
        public virtual TaskCategory TaskCategory { get; set; }


        public int CategoryID { get; set; }

        #region ICategorizedItem
        HCategory ICategorizedItem.Category
        {
            get { return this.TaskCategory; }
        }
        #endregion

        public bool IsEnabled(ISecurityUser user)
        {
            return !this.Auto &&
                   (this.AssignedFromID == user.ID &&
                    (this.Status == TaskStatus.New || this.Status == TaskStatus.Redirection ||
                     this.Status == TaskStatus.Refinement))
                   ||
                   (this.AssignedToID == user.ID &&
                    (this.Status == TaskStatus.Viewed || this.Status == TaskStatus.InProcess ||
                     this.Status == TaskStatus.Rework));
        }
    }

    public enum TaskDelayStatus
    {
        FullTime = 0,
        HalfTime = 1,
        NoTime = 2,
        OneDayDelay = 3,
        TwoDayDelay = 4,
        ManyDaysDelay = 5
    }
}
