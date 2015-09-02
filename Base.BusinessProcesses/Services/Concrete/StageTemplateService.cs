using Base.BusinessProcesses.Entities;
using Base.BusinessProcesses.Services.Abstract;
using Base.DAL;
using Base.Service;
using System.Linq;

namespace Base.BusinessProcesses.Services.Concrete
{
    public class StageTemplateService : BaseCategorizedItemService<StageTemplate>, IStageTemplateService
    {
        public StageTemplateService(IBaseObjectServiceFacade facade) : base(facade) { }

        public override IQueryable<StageTemplate> GetAllCategorizedItems(IUnitOfWork unitOfWork, int categoryID, bool? hidden = false)
        {
            string strID = HCategory.IdToString(categoryID);
            return GetAll(unitOfWork, hidden).Where(a => (a.TaskCategory.sys_all_parents != null && a.TaskCategory.sys_all_parents.Contains(strID)) || a.TaskCategory.ID == categoryID);
        }

        protected override IObjectSaver<StageTemplate> GetForSave(IUnitOfWork unitOfWork, IObjectSaver<StageTemplate> objectSaver)
        {
            return base.GetForSave(unitOfWork, objectSaver)
                .SaveOneObject(x => x.TaskCategory)
                .SaveOneToMany(x => x.Actions);
        }
    }
}
