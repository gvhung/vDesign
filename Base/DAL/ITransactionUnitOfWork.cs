
namespace Base.DAL
{
    public interface ITransactionUnitOfWork : IUnitOfWork
    {   
        void Commit();
        void Rollback();
    }

    public interface ISystemTransactionUnitOfWork : ISystemUnitOfWork, ITransactionUnitOfWork
    {
        
    }
}
