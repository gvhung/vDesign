using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Base.QueryableExtensions
{
    public static class Extensions
    {
        public static async Task<IList> ToListAsync(this IQueryable source)
        {
            if (source == null) throw new ArgumentNullException("source");

            MethodInfo method = source.GetType().GetMethod("ToListAsync");

            if (method != null)
                return await (dynamic)method.Invoke(source, null);
            else
                return await Task.Run(() =>
                {
                    return source.Provider.Execute<IList>(
                               Expression.Call(
                                   typeof(Queryable), "ToList",
                                   new Type[] { source.ElementType }, source.Expression));
                });
        }

        public static async Task<List<T>> ToGenericListAsync<T>(this IQueryable<T> source) where T : class
        {
            if (source == null) throw new ArgumentNullException("source");

            MethodInfo method = source.GetType().GetMethod("ToGenericListAsync", Type.EmptyTypes);

            if (method != null)
                return await (dynamic)method.Invoke(source, null);
            else
                return await Task.Run(() =>
                {
                    return source.ToList();
                });
        }

        public static async Task<IEnumerable> ToEnumerableAsync(this IQueryable source)
        {
            if (source == null) throw new ArgumentNullException("source");

            MethodInfo method = source.GetType().GetMethod("ToEnumerableAsync", Type.EmptyTypes);

            if (method != null)
                return await (dynamic)method.Invoke(source, null);
            else
                return await Task.Run(() =>
                {
                    if (source.IsEnumerableQuery())
                        return source;
                    else
                        return source.Provider.Execute<IEnumerable>(
                                    Expression.Call(
                                        typeof(Queryable), "ToList",
                                        new Type[] { source.ElementType }, source.Expression));

                });
        }

        private static ConcurrentDictionary<Type, Delegate> s_delegates = new ConcurrentDictionary<Type, Delegate>();

        public static Task<List<TSource>> ToListAsync<TSource>(this IQueryable<TSource> source, CancellationToken token)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            var method = source.GetType().GetMethod("ToGenericListAsync", new[] { typeof (CancellationToken) });
            if (method != null)
            {
                return method.Invoke(source, new object[] { token }) as Task<List<TSource>>;
            }

            return Task.FromResult(source.ToList());
        }



        public static async Task<int> CountAsync<T>(this IQueryable<T> source) where T : class
        {
            if (source == null) throw new ArgumentNullException("source");

            MethodInfo method = source.GetType().GetMethod("CountAsync");

            if (method != null)
                return await (dynamic)method.Invoke(source, null);
            else
                return await Task.Run(() =>
                {
                    return source.Count();
                });
        }

        public static async Task<int> CountAsync(this IQueryable source)
        {
            if (source == null) throw new ArgumentNullException("source");

            MethodInfo method = source.GetType().GetMethod("CountAsync");

            if (method != null)
                return await (dynamic)method.Invoke(source, null);
            else
                return await Task.Run(() =>
                {
                    return source.Provider.Execute<int>(
                              Expression.Call(
                                  typeof(Queryable), "Count",
                                  new Type[] { source.ElementType }, source.Expression)); ;
                });
        }

        public static async Task<T> FirstOrDefaultAsync<T>(this IQueryable<T> source) where T : class
        {
            if (source == null) throw new ArgumentNullException("source");

            MethodInfo method = source.GetType().GetMethod("FirstOrDefaultAsync");

            if (method != null)
                return await (dynamic)method.Invoke(source, null);
            else
                return await Task.Run(() =>
                {
                    return source.FirstOrDefault();
                });
        }

        private static bool IsEnumerableQuery(this IQueryable source)
        {
            return source.GetType().BaseType.Name == "EnumerableQuery";
        }
    }
}
