using Base.BusinessProcesses.Entities;
using Base.BusinessProcesses.Services.Abstract;
using Base.DAL;
using Base.Security;
using Base.Service;
using System.Linq;

namespace Base.BusinessProcesses.Services.Concrete
{
    public class BPTaskService : BaseCategorizedItemService<BPTask>, IBPTaskService
    {
        public BPTaskService(IBaseObjectServiceFacade facade) : base(facade) { }

        public override IQueryable<BPTask> GetAllCategorizedItems(IUnitOfWork unitOfWork, int categoryID, bool? hidden = false)
        {
            string strID = HCategory.IdToString(categoryID);
            return GetAll(unitOfWork, hidden).Where(a => (a.TaskCategory.sys_all_parents != null && a.TaskCategory.sys_all_parents.Contains(strID)) || a.TaskCategory.ID == categoryID);
        }

        protected override IObjectSaver<BPTask> GetForSave(IUnitOfWork unitOfWork, IObjectSaver<BPTask> objectSaver)
        {
            return base.GetForSave(unitOfWork, objectSaver)
                .SaveOneObject(x => x);
        }

        public BPTask CreateTask(ISecurityUser securityUser, BaseObject obj, Stage stage, bool saveChanges = true)
        {
            return null;
        }
    }
}
