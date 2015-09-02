using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Base.UI;
using WebUI.Areas.Public.Models;
using WebUI.Areas.Public.Service;
using WebUI.Controllers;

namespace WebUI.Areas.Public.Controllers
{
    public class PublicBaseController : BaseController
    {
        private readonly PublicMenuService _publicMenuService;
        public PublicBaseController(IBaseControllerServiceFacade serviceFacade, PublicMenuService publicMenuService) : base(serviceFacade)
        {
            _publicMenuService = publicMenuService;
        }

        public List<MenuItemVm> PublicMenuItems
        {
            get { return _publicMenuService.GetMenu(); }
        }

        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            var model = filterContext.Controller.ViewData.Model as BasePageViewModel;

            if (model != null)
            {
                model.MenuItems = PublicMenuItems;
            }

            base.OnActionExecuted(filterContext);
        }
    }
}