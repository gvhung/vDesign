using System;
using System.Collections.Generic;

namespace WebUI.Models.BusinessProcess
{
    public class TimeLineVm
    {

        public string GetAllUsersString()
        {
            return "";
        }

        public List<TimeLineElementVm> Elements { get; set; }
        public int WorkflowId { get; set; }


        public IEnumerable<TimeLineElementVm> GetElements()
        {
            bool isOdd = false;

            DateTime? date = null;

            foreach (TimeLineElementVm element in Elements)
            {
                element.IsOdd = isOdd = !isOdd;

                ClosedTimeLineElementVm closedElementVm = element as ClosedTimeLineElementVm;

                if (closedElementVm != null)
                {
                    if (date.HasValue && date.Value.Date != closedElementVm.Date.Date)
                        closedElementVm.ShowedDate = closedElementVm.Date;

                    date = closedElementVm.Date;

                    closedElementVm.ElapsedTime = closedElementVm.Date - date.Value;
                }

                yield return element;
            }
        }

        public CurrentStagesVM CurrentStages { get; set; }

        public bool ShowCurrentStages { get; set; }
    }

}