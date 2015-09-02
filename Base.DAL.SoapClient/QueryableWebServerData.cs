using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Base.DAL.SoapClient
{

    public interface IQueryableWebServerData
    {

    }

    public class QueryableWebServerData<T> : IQueryableWebServerData, IOrderedQueryable<T>
    {
        public QueryableWebServerData(IBaseWebServerQueryContext context)
        {
            Provider = new WebServerQueryProvider(context);
            Expression = Expression.Constant(this);
        }

        public QueryableWebServerData(WebServerQueryProvider provider, Expression expression)
        {
            if (provider == null)
            {
                throw new ArgumentNullException("provider");
            }

            if (expression == null)
            {
                throw new ArgumentNullException("expression");
            }

            if (!typeof(IQueryable<T>).IsAssignableFrom(expression.Type))
            {
                throw new ArgumentOutOfRangeException("expression");
            }

            Provider = provider;
            Expression = expression;
        }


        public IQueryProvider Provider { get; private set; }
        public Expression Expression { get; private set; }

        public Type ElementType
        {
            get { return typeof(T); }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return (Provider.Execute<IEnumerable<T>>(Expression)).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return (Provider.Execute<IEnumerable>(Expression)).GetEnumerator();
        }
    }
}
