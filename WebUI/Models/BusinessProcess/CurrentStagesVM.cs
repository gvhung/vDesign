using System.Collections.Generic;

namespace WebUI.Models.BusinessProcess
{
    public class CurrentStagesVM
    {
        public string ObjectType { get; set; }
        public int ObjectID { get; set; }
        public ICollection<StageVM> CurrnetStages { get; set; }

        public CurrentStagesVM()
        {
            CurrnetStages = new List<StageVM>();
        }
    }
}