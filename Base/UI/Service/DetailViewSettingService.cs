using Base.DAL;
using Base.Service;

namespace Base.UI.Service
{
    public class DetailViewSettingService : BaseObjectService<DetailViewSetting>, IDetailViewSettingService
    {
        public DetailViewSettingService(IBaseObjectServiceFacade facade) : base(facade)
        {

        }

        protected override IObjectSaver<DetailViewSetting> GetForSave(IUnitOfWork unitOfWork, IObjectSaver<DetailViewSetting> objectSaver)
        {
            return base.GetForSave(unitOfWork, objectSaver)
                .SaveOneToMany(x => x.Fields, x => x
                        .SaveOneToMany(f => f.EnableRoles, r => r.SaveOneObject(o => o.Object))
                        .SaveOneToMany(f => f.VisibleRoles, r => r.SaveOneObject(o => o.Object)));
        }
    }
}
