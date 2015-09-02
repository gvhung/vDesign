using Base;
using Base.UI;
using System;
using System.Web.Mvc;
using WebUI.Controllers;

namespace WebUI.Helpers
{
    public class BaseObjectModelBinder : DefaultModelBinder
    {
        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            string mnemonic = bindingContext.ValueProvider.GetValue("mnemonic").AttemptedValue;

            if (String.IsNullOrEmpty(mnemonic))
            {
                throw new Exception("Model binder needs for mnemonic field in request");
            }

            ViewModelConfig config = (controllerContext.Controller as BaseController).GetViewModelConfig(mnemonic);

            Type entityType = config.TypeEntity;

            BaseObject obj = Activator.CreateInstance(entityType) as BaseObject;

            obj.BeforeModelBinding();

            bindingContext.ModelMetadata = ModelMetadataProviders.Current.GetMetadataForType(() => obj, entityType);

            return base.BindModel(controllerContext, bindingContext);
        }
    }
}