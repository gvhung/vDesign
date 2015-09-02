using Framework;
using Microsoft.Linq.Translations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Base.DAL
{
    public abstract class ExtendedQueryable<T> : IExtendedQueryable<T>
    {
        protected IQueryable<T> Query { get; private set; }

        public ExtendedQueryable(IQueryable<T> query)
        {
            Query = query;
            Provider = this.CreateProvider();
        }

        public ExtendedQueryable(IQueryable<T> query, IQueryProvider provider)
        {
            Query = query;
            Provider = provider;
        }

        protected abstract ExtendedQueryableProvider CreateProvider();

        public IEnumerator<T> GetEnumerator()
        {
            return Query.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return Query.GetEnumerator();
        }

        public Type ElementType
        {
            get { return Query.ElementType; }
        }

        public Expression Expression
        {
            get { return Query.Expression; }
        }

        public IQueryProvider Provider { get; private set; }
        public abstract Task<List<T>> ToGenericListAsync();
        public abstract Task<IList> ToListAsync();
        public abstract Task<IEnumerable> ToEnumerableAsync();
        public abstract Task<List<T>> ToGenericListAsync(CancellationToken token);
        public abstract Task<int> CountAsync();
        public abstract Task<T> FirstOrDefaultAsync();
    }

    public abstract class ExtendedQueryableProvider : IQueryProvider
    {
        private readonly IQueryProvider _queryProvider;

        public ExtendedQueryableProvider(IQueryProvider queryProvider)
        {
            _queryProvider = queryProvider;
        }

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            return (IQueryable<TElement>)this.CreateQuery(expression);
        }
        
        public IQueryable CreateQuery(Expression expression)
        {
            Type elementType = TypeSystem.GetElementType(expression.Type);

            IQueryable q = _queryProvider.CreateQuery(ExpressiveExtensions.WithTranslations(expression));

            try
            {
                return this.CreateQueryEx(q, elementType);
            }
            catch (System.Reflection.TargetInvocationException tie)
            {
                throw tie.InnerException;
            }
        }

        public TResult Execute<TResult>(Expression expression)
        {
            return _queryProvider.Execute<TResult>(expression);
        }

        public object Execute(Expression expression)
        {
            return _queryProvider.Execute(expression);
        }


        protected abstract IQueryable CreateQueryEx(IQueryable sourse, Type type);
        //return (IQueryable)Activator.CreateInstance(typeof(TYPE<>).MakeGenericType(type), new object[] { sourse, this });
    }
}
