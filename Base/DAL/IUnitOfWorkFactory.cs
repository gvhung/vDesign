
namespace Base.DAL
{
    public interface IUnitOfWorkFactory
    {
        IUnitOfWork Create();
        ISystemUnitOfWork CreateSystem();
        ISystemUnitOfWork CreateSystem(IUnitOfWork unitOfWork);
        ITransactionUnitOfWork CreateTransaction();
        ISystemTransactionUnitOfWork CreateSystemTransaction();
    }
}
