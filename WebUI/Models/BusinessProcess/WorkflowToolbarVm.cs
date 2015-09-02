using Base.Security;
using System.Collections.Generic;

namespace WebUI.Models.BusinessProcess
{
    public class WorkflowToolbarVm
    {
        public ISecurityUser CurrentUser { get; set; }
        public string ObjectType { get; set; }
        public int ObjectID { get; set; }
        public string LastComment { get; set; }
        public ICollection<StageVM> Stages { get; set; }

        public WorkflowToolbarVm(ISecurityUser securityUser, int objectID, string objectType)
        {
            this.CurrentUser = securityUser;
            this.ObjectType = objectType;
            this.ObjectID = objectID;
            this.Stages = new List<StageVM>();
        }
    }
}