using Microsoft.Linq.Translations;
using System.Linq;
using System.Linq.Expressions;

namespace Base.QueryExtensions
{
    public class InterceptingQueryProvider : IQueryProvider
    {
        private readonly IQueryProvider _internalProvider;

        public InterceptingQueryProvider(IQueryProvider internalProvider)
        {
            _internalProvider = internalProvider;
        }

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            return new InterceptingQueryable<TElement>(_internalProvider.CreateQuery<TElement>(expression).WithTranslations());
        }

        public IQueryable CreateQuery(Expression expression)
        {
            var query = _internalProvider.CreateQuery(ExpressiveExtensions.WithTranslations(expression));

            var queryableType = typeof(InterceptingQueryable<>).MakeGenericType(query.ElementType);

            var ctor = queryableType.GetConstructor(new[] { queryableType });

            return ctor.Invoke(new object[] { query }) as IQueryable;
        }

        public TResult Execute<TResult>(Expression expression)
        {
            return _internalProvider.Execute<TResult>(ExpressiveExtensions.WithTranslations(expression));
        }

        public object Execute(Expression expression)
        {
            return _internalProvider.Execute(ExpressiveExtensions.WithTranslations(expression));
        }
    }
}