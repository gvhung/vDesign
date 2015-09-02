using Base.Attributes;
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
using System.Threading.Tasks;
using Framework.Maybe;
using Framework;
using System.Linq.Expressions;
using Base.Security.ObjectAccess;
using Base.Task.Entities;
using TaskStatus = Base.Task.Entities.TaskStatus;
using System.ComponentModel;

namespace Base.Project.Entities
{
    public class Project : BaseObject, ICategorizedItem, IAccessibleObject
    {
        [SystemProperty]
        public string SysName { get; set; }

        [SystemProperty]
        public int? RootId { get; set; }

        public int CategoryID { get; set; }
        [JsonIgnore]
        [ForeignKey("CategoryID")]
        public virtual ProjectCategory ProjectCategory { get; set; }

        #region ICategorizedItem
        HCategory ICategorizedItem.Category
        {
            get { return this.ProjectCategory; }
        }
        #endregion

        public int GeneralTaskID { get; set; }
        [JsonIgnore]
        public virtual ProjectTask GeneralTask { get; set; }

        [NotMapped, SystemProperty]
        public TaskStatus? GeneralTaskStatus
        {
            get { return this.GeneralTask.With(x => (TaskStatus?)x.Status); }
        }

        [NotMapped]
        [SystemProperty]
        public int GeneralTaskID_
        {
            get
            {
                return this.GeneralTask.ReturnOrDefault(x => x.ID);
            }
            set
            {
                this.SetGeneralTaskProperty(x => x.ID, value);
            }
        }

        [NotMapped]
        [FullTextSearchProperty]
        [ListView]
        [DetailView(Name = "Наименование", Required = true)]
        [MaxLength(int.MaxValue)]
        public string Title
        {
            get
            {
                return this.GeneralTask.With(x => x.Title);
            }
            set
            {
                this.SetGeneralTaskProperty(x => x.Title, value);
            }
        }

        [NotMapped]
        [ListView]
        [DetailView(Name = "Период", Required = true)]
        public Period Period
        {
            get
            {
                return this.GeneralTask.With(x => x.Period);
            }
            set
            {
                this.SetGeneralTaskProperty(x => x.Period, value);
            }
        }

        [NotMapped]
        [PropertyDataType("Percent")]
        [ListView]
        [DetailView(Name = "Процент исполнения", ReadOnly = true)]
        public double PercentComplete
        {
            get
            {
                return this.GeneralTask.ReturnOrDefault(x => x.PercentComplete);
            }
            set
            {
                this.SetGeneralTaskProperty(x => x.PercentComplete, value);
            }
        }

        [NotMapped]
        public int? CreatedById
        {
            get
            {
                return this.GeneralTask.With(x => x.AssignedFromId);
            }
            set
            {
                this.SetGeneralTaskProperty(x => x.AssignedFromId, value);
            }
        }

        [NotMapped]
        [ForeignKey("CreatedByID")]
        [FullTextSearchProperty]
        [ListView]
        [DetailView(Name = "Куратор", Required = true)]
        public virtual User CreatedBy
        {
            get
            {
                return this.GeneralTask.With(x => x.AssignedFrom);
            }
            set
            {
                this.SetGeneralTaskProperty(x => x.AssignedFrom, value);
            }
        }

        [NotMapped]
        [ForeignKey("ChargeUserID")]
        [FullTextSearchProperty]
        [ListView(Hidden = true)]
        [DetailView(Name = "Руководитель проекта")]
        public virtual User ChargeUser
        {
            get
            {
                return this.GeneralTask.With(x => x.AssignedTo);
            }
            set
            {
                this.SetGeneralTaskProperty(x => x.AssignedTo, value);
            }
        }

        [NotMapped]
        public int? ChargeUserId
        {
            get
            {
                return this.GeneralTask.With(x => x.AssignedToID);
            }
            set
            {
                this.SetGeneralTaskProperty(x => x.AssignedToID, value);
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
                return this.GeneralTask.With(x => x.Description);
            }
            set
            {
                this.SetGeneralTaskProperty(x => x.Description, value);
            }
        }

        [NotMapped]
        [FullTextSearchProperty]
        [DetailView(TabName = "[9]Файлы")]
        public virtual ICollection<TaskFile> Files
        {
            get
            {
                return this.GeneralTask.With(x => x.Files);
            }
            set
            {
                this.SetGeneralTaskProperty(x => x.Files, value);
            }
        }

        [DetailView(TabName = "[5]Заинтересованные лица", Name = "Заинтересованные контрагенты")]
        public virtual ICollection<InterestedContractor> InterestedContractors { get; set; }

        [DetailView(TabName = "[5]Заинтересованные лица", Name = "Заинтересованные пользователи")]
        public virtual ICollection<InterestedEmployee> InterestedEmployees { get; set; }


        public override void BeforeModelBinding()
        {
            this.GeneralTask = new ProjectTask();
            this.GeneralTask.BeforeModelBinding();
        }

        private void SetGeneralTaskProperty<TProperty>(Expression<Func<ProjectTask, TProperty>> memberLamda, object value)
        {
            if (this.GeneralTask != null)
            {
                this.GeneralTask.SetPropertyValue(memberLamda, value);
            }
        }
    }
}
