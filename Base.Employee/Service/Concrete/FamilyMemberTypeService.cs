using Base.Employee.Entities;
using Base.Employee.Service.Abstract;
using Base.Security.Service.Abstract;
using Base.Service;

namespace Base.Employee.Service.Concrete
{
    public class FamilyMemberTypeService : BaseObjectService<FamilyMemberType>, IFamilyMemberTypeService
    {
        public FamilyMemberTypeService(IEmployeeUnitOfWork context, ISecurityService securityService) : base(context, securityService) { }
    }
}
