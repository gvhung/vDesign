using System;
using System.Data.Entity.Spatial;
using System.Web.Mvc;

namespace WebUI.Helpers
{
    public class DbGeographyModelBinder : DefaultModelBinder
    {
        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
            string value = valueProviderResult != null ? valueProviderResult.AttemptedValue : null;

            if (String.IsNullOrEmpty(value))
                return (DbGeography)null;

            return DbGeography.FromText(value, 4326);
        }
    }

    public class DbGeographyModelBinderProviderMvc : IModelBinderProvider
    {
        public IModelBinder GetBinder(Type modelType)
        {
            if (modelType == typeof(DbGeography))
                return new DbGeographyModelBinder();
            return null;
        }
    }
}