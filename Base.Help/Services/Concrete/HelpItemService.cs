using Base.DAL;
using Base.Help.Entities;
using Base.Service;

namespace Base.Help.Services
{
    public class HelpItemService : BaseCategoryService<HelpItem>, IHelpItemService
    {
        public HelpItemService(IBaseObjectServiceFacade facade) : base(facade) { }

        protected override IObjectSaver<HelpItem> GetForSave(IUnitOfWork unitOfWork, IObjectSaver<HelpItem> objectSaver)
        {
            return base.GetForSave(unitOfWork, objectSaver).SaveManyToMany(x => x.Tags);
        }
    }
}
