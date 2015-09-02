using Base.Attributes;
using Base.Security;
using Framework.Maybe;
using System.Collections.Generic;

namespace Base.Task.Entities
{
    public class TaskReport : BaseObject, ICategorizedItem
    {
        #region ICategorizedItem
        public int CategoryID { get; set; }
        public TaskCategory TaskCategory { get; set; }
        HCategory ICategorizedItem.Category
        {
            get { return this.TaskCategory; }
        }
        #endregion

        [ListView("Отдел", Filterable = false)]
        public virtual UserCategory UserCategory { get { return this.AssignedTo.With(x => x.UserCategory); } }

        [ListView("Исполнитель")]
        public User AssignedTo { get; set; }

        [ListView("Новые")]
        public int CountNew { get; set; }

        [ListView("В работе")]
        public int CountActive { get; set; }

        [ListView("Просрочено")]
        public int CountExpired { get; set; }

        [ListView("На проверке")]
        public int CountRevise { get; set; }

        [ListView("Выполнено")]
        public int CountComplete { get; set; }

        [DetailView(TabName="[1]Новые")]
        public List<Task> NewTasks { get; set; }

        [DetailView(TabName = "[2]В работе")]
        public List<Task> ActiveTasks { get; set; }

        [DetailView(TabName = "[3]Просроченные")]
        public List<Task> ExpiredTasks { get; set; }

        [DetailView(TabName = "[4]На проверке")]
        public List<Task> ReviseTasks { get; set; }

        [DetailView(TabName = "[5]Выполненные (за последние 3 месяца)")]
        public List<Task> CompleteTasks { get; set; }
    }
}
