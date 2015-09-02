using Base.BusinessProcesses.Entities;
using Base.UI;
using Framework.Maybe;
using System;
using System.Collections.Generic;
using System.Linq;
using Base.Security;

namespace WebUI.Models.BusinessProcess
{
    public abstract class StageTimeLineElementVm : TimeLineElementVm
    {
        protected StageTimeLineElementVm(ChangeHistory history)
            : base(history)
        {
            Date = history.Date;
            AgreementItem = history.AgreementItems.FirstOrDefault();
            EndDateFact = history.AgreementItems.FirstOrDefault(x => x.ActionID != null).With(x=>x.Date);
            ElapsedTime = (EndDateFact ?? DateTime.Now) - Date;
            ElapsedString += ElapsedTime.Days != 0 ? ElapsedTime.Days + " д " : "";
            ElapsedString += ElapsedTime.Hours != 0 ? ElapsedTime.Hours + " ч " : "";
            if (Date.Date == EndDateFact.GetValueOrDefault().Date)
            {
                ElapsedString += ElapsedTime.Minutes != 0 ? ElapsedTime.Minutes + " м " : "";
            }
        }
        protected AgreementItem AgreementItem { get; set; }

        public TimeSpan ElapsedTime { get; set; }
        public string ElapsedString { get; set; }

        public DateTime? EndDateFact { get; set; }
    }

    public class TerminatedTimeLineElementVm : StageTimeLineElementVm
    {
        public TerminatedTimeLineElementVm(ChangeHistory history)
            : base(history)
        {
         
        }
    }

    public class ClosedTimeLineElementVm : StageTimeLineElementVm
    {
        public ClosedTimeLineElementVm(ChangeHistory history)
            : base(history)
        {
            Comment = AgreementItem.Comment;
            Color = AgreementItem.Action.Color;
            Action = AgreementItem.Action;
            Performer = AgreementItem.User;
            FromUser = AgreementItem.FromUser;
        }



        public string Comment { get; set; }
        public StageAction Action { get; set; }

        public User Performer { get; set; }
        public User FromUser { get; set; }
    }
}