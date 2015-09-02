using Base.DAL;
using System.Web.Mvc;

namespace WebUI.Concrete
{
    public class UnitOfWorkFactory : IUnitOfWorkFactory
    {
        public IUnitOfWork Create()
        {
            return DependencyResolver.Current.GetService<IUnitOfWork>();
        }

        public ISystemUnitOfWork CreateSystem()
        {
            return DependencyResolver.Current.GetService<ISystemUnitOfWork>();
        }

        public ISystemUnitOfWork CreateSystem(IUnitOfWork unitOfWork)
        {
            return new WrapSystemUnitOfWork(unitOfWork);
        }

        public ITransactionUnitOfWork CreateTransaction()
        {
            return DependencyResolver.Current.GetService<ITransactionUnitOfWork>();
        }

        public ISystemTransactionUnitOfWork CreateSystemTransaction()
        {
            return DependencyResolver.Current.GetService<ISystemTransactionUnitOfWork>();
        }
    }
}