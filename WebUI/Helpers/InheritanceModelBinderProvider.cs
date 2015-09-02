using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace WebUI.Helpers
{
    public class InheritanceModelBinderProvider : Dictionary<Type, IModelBinder>, IModelBinderProvider
    {
        public IModelBinder GetBinder(Type modelType)
        {
            return this.Where(x => x.Key.IsAssignableFrom(modelType)).Select(x => x.Value).FirstOrDefault();
        }
    }
}