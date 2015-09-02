using Base.DAL;
using Base.Service;

namespace Base.Security.Service
{
    public class DepartmentService : BaseObjectService<Department>, IDepartmentService
    {
        public DepartmentService(IBaseObjectServiceFacade facade) : base(facade) { }

        protected override IObjectSaver<Department> GetForSave(IUnitOfWork unitOfWork, IObjectSaver<Department> objectSaver)
        {
            return base.GetForSave(unitOfWork, objectSaver).SaveOneObject(x => x.Image);
        }
    }
}
