using Base.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Base.Projects.Base
{
    public abstract class BaseLink : BaseObject, IBaseLink
    {
        public abstract long? Source { get; set; }

        public abstract long? Target { get; set; }

        public string Type { get; set; }

        [NotMapped]
        private long _viewID { get; set; }

        [NotMapped]
        public long ViewID
        {
            get
            {
                if (this.ID == 0 || this._viewID != 0)
                    return this._viewID;
                else
                    return this.ID;
            }
            set
            {
                this._viewID = value;
            }
        }

        public int ProjectId { get; set; }

        public abstract IBaseProject BaseProject { get; set; }

        public int? SourceId { get; set; }
        public int? TargetId { get; set; }

        public abstract IProjectTask BaseSourceTask { get; set; }
        public abstract IProjectTask BaseTargetTask { get; set; }

        [NotMapped]
        public ObjectStatus SysStatus { get; set; }
    }
}
