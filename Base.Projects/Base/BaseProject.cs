using Base;
using Base.Attributes;
using Base.Entities.Complex;
using Base.Security;
using Base.Tasks;
using Framework.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Framework.Maybe;

namespace Base.Projects.Base
{
    public abstract class BaseProject : BaseObject, IBaseProject
    {
        public int GeneralTaskID { get; set; }

        public abstract IProjectTask BaseGeneralTask { get; set; }

        [NotMapped]
        [FullTextSearchProperty]
        [ListView(Order = 0)]
        [DetailView(Name = "Наименование", Order = 0, Required = true)]
        [MaxLength(int.MaxValue)]
        public string Title
        {
            get
            {
                return this.BaseGeneralTask.With(x => x.Title);
            }
            set
            {
                this.BaseGeneralTask.Title = value;
            }
        }

        [NotMapped]
        [FullTextSearchProperty]
        [DetailView(Name = "Описание", Order = 1, Required = false)]
        public string Description
        {
            get
            {
                return this.BaseGeneralTask.With(x => x.Description);
            }
            set
            {
                this.BaseGeneralTask.Description = value;
            }
        }

        [NotMapped]
        [ListView(Order = 1)]
        [DetailView(Name = "Период", Order = 2, Required = true)]
        public Period Period
        {
            get
            {
                return this.BaseGeneralTask.With(x => x.Period);
            }
            set
            {
                this.BaseGeneralTask.Period = value;
            }
        }

        [NotMapped]
        public int? CreatedByID
        {
            get
            {
                return this.BaseGeneralTask.With(x => x.AssignedFromID);
            }
            set
            {
                this.BaseGeneralTask.AssignedFromID = value;
            }
        }

        [NotMapped]
        [ForeignKey("CreatedByID")]
        [FullTextSearchProperty]
        [ListView(Order = 3)]
        [DetailView(Name = "Автор", Order = 4, Required = true)]
        public virtual User CreatedBy
        {
            get
            {
                return this.BaseGeneralTask.With(x => x.AssignedFrom);
            }
            set
            {
                this.BaseGeneralTask.AssignedFrom = value;
            }
        }

        [NotMapped]
        public int? ChargeUserID
        {
            get
            {
                return this.BaseGeneralTask.With(x => x.AssignedToID);
            }
            set
            {
                this.BaseGeneralTask.AssignedToID = value;
            }
        }

        [NotMapped]
        [ForeignKey("ChargeUserID")]
        [FullTextSearchProperty]
        [DetailView(Name = "Ответственный", Order = 5, Required = true)]
        public virtual User ChargeUser
        {
            get
            {
                return this.BaseGeneralTask.With(x => x.AssignedTo);
            }
            set
            {
                this.BaseGeneralTask.AssignedTo = value;
            }
        }

        public abstract IEnumerable<IBaseLink> BaseLinks { get; set; }

        public abstract int DefaultCategoryID { get; set; }
        public abstract BaseTaskCategory BaseDefaultCategory { get; set; }

        public abstract IEnumerable<BaseProjectFile> BaseFiles { get; set; }

        [NotMapped]
        [DataType("Percent")]
        [ListView(Order = 4)]
        [DetailView(Name = "Процент исполнения", Order = 6, Required = false)]
        public double PercentComplete
        {
            get
            {
                return this.BaseGeneralTask.ReturnOrDefault(X => X.PercentComplete);
            }
            set
            {
                this.BaseGeneralTask.PercentComplete = value;
            }
        }

    }
}
