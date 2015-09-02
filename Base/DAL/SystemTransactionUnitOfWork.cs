
namespace Base.DAL
{
    public class SystemTransactionUnitOfWork : TransactionUnitOfWork, ISystemTransactionUnitOfWork
    {
        public SystemTransactionUnitOfWork(IRepositoryFactory repositoryFactory)
            : base(repositoryFactory)
        {

        }
    }
}
