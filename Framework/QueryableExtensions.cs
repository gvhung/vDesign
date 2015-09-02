using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Framework
{
    public static class QueryableExtensions
    {
        public static IQueryable<GroupCounter> SelectGroupCounts<TEntity>(this IQueryable<TEntity> query, string property)
        {
            var propertyInfo = typeof(TEntity).GetProperty(property);
            if (propertyInfo == null)
                throw new ArgumentNullException("propertyInfo");

            var param = Expression.Parameter(typeof(TEntity));
            var propertyExpression = Expression.Property(param, property);

            var groupExpression = Expression.Lambda(propertyExpression, param);

            var groupInfo = typeof(Queryable).GetMethods().FirstOrDefault(x => x.Name == "GroupBy");
            if (groupInfo == null)
                throw new ArgumentNullException("groupInfo");

            var groupType = typeof(IGrouping<,>).MakeGenericType(propertyInfo.PropertyType, typeof(TEntity));

            var groupMethod = groupInfo.MakeGenericMethod(typeof(TEntity), propertyInfo.PropertyType);
            var groupResult = groupMethod.Invoke(null, new object[] { query, groupExpression });

            var lambda = typeof (QueryableExtensions).GetMethod("BuildSelectLambda", BindingFlags.Static | BindingFlags.NonPublic)
                .MakeGenericMethod(new[] { groupType, typeof (GroupCounter), typeof (TEntity) }).Invoke(null, null);

            var selectInfo = typeof(Queryable).GetMethods().FirstOrDefault(x => x.Name == "Select");
            if (selectInfo == null)
                throw new ArgumentNullException("selectInfo");

            var selectResult = selectInfo.MakeGenericMethod(groupType, typeof(GroupCounter)).Invoke(null, new object[] { groupResult, lambda });

            return selectResult as IQueryable<GroupCounter>;
        }

        private static Expression<Func<TGroupType, TViewModel>> BuildSelectLambda<TGroupType, TViewModel, TEntity>()
        {
            var createdType = typeof(TViewModel);
            var groupParam = Expression.Parameter(typeof(TGroupType));

            var ctor = Expression.New(createdType);
            var keyProperty = createdType.GetProperty("Key");
            var keyValueAssignment = Expression.Bind(keyProperty, Expression.Property(groupParam, "Key"));

            var countMethod = typeof(Enumerable).GetMethods()
                .FirstOrDefault(x => x.Name == "Count")
                .MakeGenericMethod(typeof(TEntity));

            var countProperty = createdType.GetProperty("Count");
            var countValueAssignment = Expression.Bind(countProperty, Expression.Call(null, countMethod, groupParam));

            var memberInit = Expression.MemberInit(ctor, keyValueAssignment, countValueAssignment);

            return
                Expression.Lambda<Func<TGroupType, TViewModel>>(memberInit, groupParam);
        }
    }

    public class GroupCounter
    {
        public object Key { get; set; }
        public int Count { get; set; }
    }
}