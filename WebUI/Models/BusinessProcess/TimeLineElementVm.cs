using Base.BusinessProcesses.Entities;
using Base.UI;
using System;
using System.Collections.Generic;

namespace WebUI.Models.BusinessProcess
{
    public abstract class TimeLineElementVm
    {
        protected TimeLineElementVm(ChangeHistory history)
        {
            this.Title = history.Step.Title;
            this.Step = history.Step;
            this.Description = history.Step.Description;          
            this.Date = history.Date;
            this.ID = "te_" + Guid.NewGuid().ToString("D").Split('-')[0];
            this.ObjectType = history.ObjectType;
            this.ObjectID = history.ObjectID;
        }

        public string ObjectType { get; set; }
        public int ObjectID { get; set; }

        public bool IsOdd { get; set; }

        public string ID { get; private set; }

        public string Color { get; set; }

        public DateTime Date { get; set; }
        public TimeLineElementVm Previous { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
  
        public DateTime? ShowedDate { get; set; }
        public Step Step { get; set; }
    }
}