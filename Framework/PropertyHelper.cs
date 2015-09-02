using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Framework
{
    public static class PropertyHelper<T>
    {
        public static PropertyInfo GetProperty<TValue>(Expression<Func<T, TValue>> selector)
        {
            Expression body = selector;

            if (body is LambdaExpression)
            {
                body = ((LambdaExpression)body).Body;
            }
            
            switch (body.NodeType)
            {
                case ExpressionType.MemberAccess:
                    return (PropertyInfo)((MemberExpression)body).Member;
                default:
                    throw new InvalidOperationException();
            }
        }
    }


    public static class PropertyHelper
    {
        public static string NameOf<TObject, TProperty>(this Func<TObject> objectSelector, Expression<Func<TObject, TProperty>> propertySelector)
        {
            if (propertySelector == null) throw new ArgumentNullException("propertySelector");

            MemberExpression propertyExpression = propertySelector.Body as MemberExpression;

            if (propertyExpression == null) throw new InvalidOperationException("propertyExpression");

            PropertyInfo propertyInfo = propertyExpression.Member as PropertyInfo;

            if (propertyInfo == null) throw new InvalidOperationException("propertyInfo");

            return propertyInfo.Name;
        }

        public static object GetPropertyValue<T, TProperty>(this T target, Expression<Func<T, TProperty>> memberLamda)
        {
            MemberExpression memberExpression = null;

            if (memberLamda.Body.NodeType == ExpressionType.Convert)
            {
                UnaryExpression body = (UnaryExpression)memberLamda.Body;
                memberExpression = body.Operand as MemberExpression;
            }
            else if (memberLamda.Body.NodeType == ExpressionType.MemberAccess)
            {
                memberExpression = memberLamda.Body as MemberExpression;
            }

            if (memberExpression != null)
            {
                PropertyInfo property = memberExpression.Member as PropertyInfo;

                if (property != null)
                {
                    return property.GetValue(target, null) ;
                }
            }

            return null;
        }

        public static void SetPropertyValue<T, TProperty>(this T target, Expression<Func<T, TProperty>> memberLamda, object value)
        {
            MemberExpression memberExpression = null;

            if (memberLamda.Body.NodeType == ExpressionType.Convert)
            {
                UnaryExpression body = (UnaryExpression)memberLamda.Body;
                memberExpression = body.Operand as MemberExpression;
            }
            else if (memberLamda.Body.NodeType == ExpressionType.MemberAccess)
            {
                memberExpression = memberLamda.Body as MemberExpression;
            }

            if (memberExpression != null)
            {
                PropertyInfo property = memberExpression.Member as PropertyInfo;

                if (property != null)
                {
                    property.SetValue(target, value, null);
                }
            }
        }
    } 
}
