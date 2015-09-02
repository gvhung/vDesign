using Ninject;
using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace WebUI.Concrete
{
    public class NinjectControllerFactory : DefaultControllerFactory
    {
        protected override IController GetControllerInstance(RequestContext requestContext, Type controllerType)
        {
            if (controllerType == null)
            {
                throw new HttpException(404, "Страница не найдена");
            }

            if (!typeof(IController).IsAssignableFrom(controllerType))
            {
                throw new HttpException(500, "Ошибка сервера");
            }

            var kernel = Startup.CreateKernel();

            var userResolver = kernel.Get<HttpSecurityUserResolver>();

            userResolver.SetSecurityUser(requestContext.HttpContext);

            return kernel.Get(controllerType) as IController;
        }

        public override void ReleaseController(IController controller)
        {
            base.ReleaseController(controller);
        }
    }
}