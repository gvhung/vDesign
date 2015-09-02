using System;
using System.Linq.Expressions;
using System.Web.Mvc;

namespace Framework
{
    public static class MetadataHelper<TModel>
    {
        public static string GetDisplayName<TProperty>(Expression<Func<TModel, TProperty>> expression)
        {
            return ModelMetadata.FromLambdaExpression<TModel, TProperty>(expression, new ViewDataDictionary<TModel>()).DisplayName;
        }
    }
}
