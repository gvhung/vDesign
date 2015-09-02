using Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Base.DAL.SoapClient
{
    public class WebServerQueryProvider : IQueryProvider
    {
        readonly IBaseWebServerQueryContext _context;

        public WebServerQueryProvider(IBaseWebServerQueryContext context)
        {
            _context = context;
        }

        public IQueryable CreateQuery(Expression expression)
        {
            Type elementType = TypeSystem.GetElementType(expression.Type);

            try
            {
                return (IQueryable)Activator.CreateInstance(typeof(QueryableWebServerData<>).MakeGenericType(elementType), new object[] { this, expression });
            }
            catch (System.Reflection.TargetInvocationException tie)
            {
                throw tie.InnerException;
            }
        }

        public IQueryable<TResult> CreateQuery<TResult>(Expression expression)
        {
            return new QueryableWebServerData<TResult>(this, expression);
        }

        public object Execute(Expression expression)
        {
            return _context.Execute(expression, false);
        }

        public TResult Execute<TResult>(Expression expression)
        {
            Type typeRes = typeof(TResult);

            bool IsEnumerable = (typeRes.Name == "IEnumerable`1" || typeRes.Name == "IEnumerable");

            return (TResult)_context.Execute(expression, IsEnumerable);
        }
    }
}
