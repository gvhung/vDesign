using Base.Entities;
using Base.UI;
using System;
using System.Linq;
using System.Web.Mvc;

namespace WebUI.Helpers
{
    public class RuntimeTypeModelBinder : DefaultModelBinder
    {
        protected override object CreateModel(ControllerContext controllerContext, ModelBindingContext bindingContext, Type modelType)
        {
            if (typeof(IRuntimeBindingType).IsAssignableFrom(modelType))
            {
                //ValueProviderResult result = bindingContext.ValueProvider.GetValue(bindingContext.ModelName +
                //    ((Func<IRuntimeBindingType>)null).NameOf(x => x.RuntimeType));

                ValueProviderResult result = bindingContext.ValueProvider.GetValue(bindingContext.ModelName + ".RuntimeType");

                if (result != null)
                {
                    var configs = DependencyResolver.Current.GetService<IViewModelConfigService>().GetAll();

                    var instantiationType = configs.Select(x => x.TypeEntity)
                        .FirstOrDefault(x => x.FullName == result.AttemptedValue);

                    if (instantiationType != null)
                    {
                        object obj = Activator.CreateInstance(instantiationType);

                        bindingContext.ModelMetadata = ModelMetadataProviders.Current.GetMetadataForType(null, instantiationType);

                        return (bindingContext.ModelMetadata.Model = obj);
                    }
                }
            }

            return base.CreateModel(controllerContext, bindingContext, modelType);
        }
    }
}