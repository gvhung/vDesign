using System;

namespace Base.DAL
{
    public class TransactionUnitOfWork : UnitOfWork, ITransactionUnitOfWork
    {
        private IDisposable _transaction;

        public TransactionUnitOfWork(IRepositoryFactory repositoryFactory)
            : base(repositoryFactory)
        {
        }

        public override IRepository<TObject> GetRepository<TObject>()
        {
            var repository = base.GetRepository<TObject>();

            var contex = this.GetContext<TObject>();

            if (!contex.Transaction)
            {
                _transaction = GetContext<TObject>().BeginTransaction();
            }

            return repository;
        }

        public void Commit()
        {
            foreach (var context in GetContexts())
            {
                context.Commit();
            }
        }

        public void Rollback()
        {
            foreach (var context in GetContexts())
            {
                context.Rollback();
            }
        }

        public override void Dispose()
        {
            if (_transaction != null)
            {
                _transaction.Dispose();
            }

            base.Dispose();
        }
    }
}
