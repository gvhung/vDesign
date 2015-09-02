using Base.Security;
using Base.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Linq.Expressions;
using System.Text.RegularExpressions;

namespace WebUI.Helpers
{
    public static class ViewModelConfigExtensions
    {
        public static IQueryable<T> SystemFilter<T>(this IQueryable<T> q, ViewModelConfig config, ISecurityUser securityUser, string extraSysFilter = null) where T : class
        {
            var sysFilter = extraSysFilter;

            if (config.ListView != null && config.ListView.DataSource != null && !String.IsNullOrEmpty(config.ListView.DataSource.SystemFilter))
            {
                sysFilter = string.IsNullOrEmpty(sysFilter) ? config.ListView.DataSource.SystemFilter : string.Format("{0} and {1}", sysFilter, config.ListView.DataSource.SystemFilter);
            }

            if (!string.IsNullOrEmpty(sysFilter))
            {
                var indexOf = sysFilter.IndexOf("@Now.AddDays");

                if (indexOf >= 0)
                {
                    var rgx = new Regex(@"@Now.AddDays\(([\d\-]+)\)", RegexOptions.IgnoreCase);

                    var matches = rgx.Matches(sysFilter, indexOf);

                    foreach (Match match in matches)
                    {
                        var day = Int32.Parse(match.Groups[1].Value);

                        var dtm = DateTime.Today.AddDays(day);

                        sysFilter = sysFilter.Replace(match.Value, string.Format("DateTime({0}, {1}, {2})", dtm.Year, dtm.Month, dtm.Day));
                    }
                }

                sysFilter = sysFilter
                    .Replace("@CurrentUserID", securityUser.ID.ToString())
                    .Replace("@Today", String.Format("DateTime({0}, {1}, {2})", DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day));

                if (sysFilter.IndexOf("@IsAdmin", StringComparison.InvariantCultureIgnoreCase) >= 0)
                {
                    sysFilter = sysFilter.Replace("@IsAdmin", securityUser.IsAdmin ? "true" : "false");
                }

                return q.Where(sysFilter);
            }

            return q;
        }

        public static IQueryable SelectMany(this IQueryable source, string selector, params object[] values)
        {
            if (source == null)
                throw new ArgumentNullException("source");
            if (selector == null)
                throw new ArgumentNullException("selector");

            LambdaExpression lambda =
                System.Linq.Dynamic.DynamicExpression.ParseLambda(source.ElementType, null, selector, values);


            Type inputType = source.Expression.Type.GetGenericArguments()[0];
            Type resultType = lambda.Body.Type.GetGenericArguments()[0];
            Type enumerableType = typeof(IEnumerable<>).MakeGenericType(resultType);
            Type delegateType = typeof(Func<,>).MakeGenericType(inputType, enumerableType);

            lambda = Expression.Lambda(delegateType, lambda.Body, lambda.Parameters);

            return source.Provider.CreateQuery(
                Expression.Call(
                    typeof(Queryable), "SelectMany",
                    new Type[] { source.ElementType, resultType },
                    source.Expression, Expression.Quote(lambda)));
        }


    }
}