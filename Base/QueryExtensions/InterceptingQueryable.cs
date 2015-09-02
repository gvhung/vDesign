using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Base.QueryExtensions
{
    public class InterceptingQueryable : IOrderedQueryable
    {
        private readonly IQueryable _internalQuery;
        private readonly InterceptingQueryProvider _queryProvider;

        public InterceptingQueryable(IQueryable internalQuery)
        {
            _internalQuery = internalQuery;
            _queryProvider = new InterceptingQueryProvider(_internalQuery.Provider);
        }

        public Type ElementType
        {
            get { return _internalQuery.ElementType; }
        }

        public Expression Expression
        {
            get { return _internalQuery.Expression; }
        }

        public IQueryProvider Provider
        {
            get { return InterceptingProvider; }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _internalQuery.GetEnumerator();
        }

        protected InterceptingQueryProvider InterceptingProvider
        {
            get { return _queryProvider; }
        }
    }

    public class InterceptingQueryable<TEntity> : InterceptingQueryable, IOrderedQueryable<TEntity>
    {
        private readonly IQueryable<TEntity> _internalQuery;

        public InterceptingQueryable(IQueryable<TEntity> internalQuery)
            : base(internalQuery)
        {
            _internalQuery = internalQuery;
        }

        public IEnumerator<TEntity> GetEnumerator()
        {
            return _internalQuery.GetEnumerator();
        }
    }
}