using System.Collections.Generic;
using System.Linq;

namespace Base.DAL.EF.Extensions
{
    public static class QueryableExtensions
    {
        public static IExtendedQueryable<T> AsExtendedQueryable<T>(this IEnumerable<T> source) where T : class
        {
            return new EFExtendedQueryable<T>(source.AsQueryable());
        }
    }
}
