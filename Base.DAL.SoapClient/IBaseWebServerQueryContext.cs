using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Base.DAL.SoapClient
{
    public interface IBaseWebServerQueryContext : IDisposable
    {
        object Execute(Expression expression, bool IsEnumerable);

        TEntity Attach<TEntity>(TEntity entity) where TEntity : class;

        WebEntityEntry Entry<TEntity>(TEntity entity) where TEntity : class;

        IEnumerable<WebEntityEntry> Entries();

        int SaveChanges();

        Task<int> SaveChangesAsync();
    }
}
