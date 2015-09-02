
namespace Base.DAL
{
    public class SystemUnitOfWork : UnitOfWork, ISystemUnitOfWork
    {
        public SystemUnitOfWork(IRepositoryFactory repositoryFactory)
            : base(repositoryFactory)
        {

        }
    }

    public class WrapSystemUnitOfWork : WrapperUnitOfWork, ISystemUnitOfWork
    {
        public WrapSystemUnitOfWork(IUnitOfWork unitOfWork)
            : base(unitOfWork)
        {

        }
    }
}
