using System;
using System.Web.Mvc;

namespace WebUI.Helpers
{
    public class SettingValueModelBinder : DefaultModelBinder
    {
        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            Type entityType = Type.GetType(bindingContext.ValueProvider.GetValue("model.Value.Type").AttemptedValue);

            object obj = Activator.CreateInstance(entityType);

            bindingContext.ModelMetadata = ModelMetadataProviders.Current.GetMetadataForType(() => obj, entityType);

            return base.BindModel(controllerContext, bindingContext);
        }
    }
}