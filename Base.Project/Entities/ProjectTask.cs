using Base.Attributes;
using Base.Entities;
using Base.Entities.Complex;
using Base.Security;
using Framework.Attributes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using Framework.Maybe;
using Framework;
using Base.Task;
using Base.Task.Entities;
using System.Linq.Expressions;

namespace Base.Project.Entities
{
    [EnableFullTextSearch]
    public class ProjectTask : HCategory, ITask
    {
        public int TaskId { get; set; }
        [JsonIgnore]
        public virtual Base.Task.Entities.Task Task { get; set; }

        #region HCategory
        [NotMapped]
        [ListView]
        [DetailView(Name = "Наименование", Required = true)]
        [MaxLength(int.MaxValue)]
        public override string Name
        {
            get
            {
                return this.Task.With(x => x.Title);
            }
            set
            {
                this.SetTaskProperty(x => x.Title, value);
            }
        }

        [ForeignKey("ParentID")]
        [JsonIgnore]
        public virtual ProjectTask Parent_ { get; set; }
        [JsonIgnore]
        public virtual ICollection<ProjectTask> Children_ { get; set; }

        public override HCategory Parent
        {
            get { return this.Parent_; }
        }
        public override IEnumerable<HCategory> Children
        {
            get { return Children_ ?? new List<ProjectTask>(); }
        }

        #endregion

        [NotMapped]
        [SystemProperty]
        public int TaskID_
        {
            get
            {
                return this.Task.ReturnOrDefault(x => x.ID);
            }
            set
            {
                this.SetTaskProperty(x => x.ID, value);
            }
        }




        

        [NotMapped]
        [JsonIgnore]
        public string Title
        {
            get
            {
                return this.Name;
            }
            set
            {
                this.Name = value;
            }
        }

       

        [NotMapped]
        [DetailView(Name = "Период", Required = true)]
        public Period Period
        {
            get
            {
                return this.Task.With(x => x.Period);
            }
            set
            {
                this.SetTaskProperty(x => x.Period, value);
            }
        }

        [NotMapped]
        [PropertyDataType("Percent")]
        [ListView]
        [DetailView(Name = "Процент исполнения")]
        public double PercentComplete
        {
            get
            {
                return this.Task.ReturnOrDefault(x => x.PercentComplete);
            }
            set
            {
                this.SetTaskProperty(x => x.PercentComplete, value);
            }
        }

        [NotMapped]
        public int? AssignedFromId
        {
            get
            {
                return this.Task.ReturnOrDefault(x => x.AssignedFromID);
            }
            set
            {
                this.SetTaskProperty(x => x.AssignedFromID, value);
            }
        }


        [NotMapped]
        [FullTextSearchProperty]
        [ListView]
        [DetailView(Name = "Автор", Required = true)]
        public virtual User AssignedFrom
        {
            get
            {
                return this.Task.With(x => x.AssignedFrom);
            }
            set
            {
                this.SetTaskProperty(x => x.AssignedFrom, value);
            }
        }

        [NotMapped]
        public int? AssignedToID
        {
            get
            {
                return this.Task.ReturnOrDefault(x => x.AssignedToID);
            }
            set
            {
                this.SetTaskProperty(x => x.AssignedToID, value);
            }
        }

        [NotMapped]
        [FullTextSearchProperty]
        [ListView]
        [DetailView(Name = "Исполнитель")]
        public virtual User AssignedTo
        {
            get
            {
                return this.Task.With(x => x.AssignedTo);
            }
            set
            {
                this.SetTaskProperty(x => x.AssignedTo, value);
            }
        }

        [ListView]
        [DetailView(Name = "Тип", Required = false)]
        public TaskType TaskType { get; set; }

        [SystemProperty]
        [NotMapped]
        public bool System
        {
            get
            {
                return this.Task.ReturnOrDefault(x => x.System);
            }
            set
            {
                this.SetTaskProperty(x => x.System, value);
            }
        }

        [ListView]
        [DetailView(Name = "Приоритет")]
        public Priority Priority
        {
            get
            {
                return this.Task.ReturnOrDefault(x => x.Priority);
            }
            set
            {
                this.SetTaskProperty(x => x.Priority, value);
            }
        }

        [ListView]
        [DetailView(Name = "Статус",  Required = false)]
        [NotMapped]
        public TaskStatus Status
        {
            get
            {
                return this.Task.ReturnOrDefault(x => x.Status);
            }
            set
            {
                this.SetTaskProperty(x => x.Status, value);
            }
        }

        [NotMapped]
        [FullTextSearchProperty]
        [DetailView(TabName = "[2]Описание")]
        [PropertyDataType(PropertyDataType.SimpleHtml)]
        public string Description
        {
            get
            {
                return this.Task.With(x => x.Description);
            }
            set
            {
                this.SetTaskProperty(x => x.Description, value);
            }
        }

        [NotMapped]
        [FullTextSearchProperty]
        [DetailView(TabName = "[4]Файлы")]
        public virtual ICollection<TaskFile> Files
        {
            get
            {
                return this.Task.With(x => x.Files);
            }
            set
            {
                this.SetTaskProperty(x => x.Files, value);
            }
        }

        [NotMapped]
        [FullTextSearchProperty]
        [DetailView(TabName = "[4]История")]
        public virtual ICollection<TaskChangeHistory> TaskChangeHistory
        {
            get
            {
                return this.Task.With(x => x.TaskChangeHistory);
            }
            set
            {
                this.SetTaskProperty(x => x.TaskChangeHistory, value);
            }
        }

        public bool Auto { get; set; }

        [NotMapped]
        public int CategoryID
        {
            get
            {
                return this.Task.ReturnOrDefault(x => x.CategoryID);
            }
            set
            {
                this.SetTaskProperty(x => x.CategoryID, value);
            }
        }

        [NotMapped]
        public DateTime? CompliteDate
        {
            get
            {
                return this.Task.With(x => x.CompliteDate);
            }
            set
            {
                this.SetTaskProperty(x => x.CompliteDate, value);
            }
        }

        [SystemProperty]
        public bool Summary
        {
            get
            {
                return this.Children_ != null ? this.Children_.Any() : false; 
            }
        }

        [SystemProperty]
        public bool Expanded { get; set; }

        [DetailView(Name = "Подсветка", Required = false)]
        [PropertyDataType("Palette")]
        public String Highlight { get; set; }

        public override void BeforeModelBinding()
        {
            this.Task = new Task.Entities.Task();
        }
        
        private void SetTaskProperty(Expression<Func<Base.Task.Entities.Task, object>> memberLamda, object value)
        {
            if (this.Task != null)
            {
                this.Task.SetPropertyValue(memberLamda, value);
            }
        }
    }

}
