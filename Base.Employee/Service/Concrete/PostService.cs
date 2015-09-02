using Base.Employee.Entities;
using Base.Employee.Service.Abstract;
using Base.Security.Service.Abstract;
using Base.Service;

namespace Base.Employee.Service.Concrete
{
    public class PostService : BaseObjectService<Post>, IPostService
    {
        public PostService(IEmployeeUnitOfWork context, ISecurityService securityService) : base(context, securityService) { }
    }
}