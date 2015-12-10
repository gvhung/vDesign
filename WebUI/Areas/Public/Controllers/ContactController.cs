using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebUI.Areas.Public.Models;
using WebUI.Areas.Public.Service;
using WebUI.Controllers;

namespace WebUI.Areas.Public.Controllers
{
    public class ContactController : PublicBaseController
    {
        public ContactController(IBaseControllerServiceFacade serviceFacade, PublicMenuService publicMenuService) : base(serviceFacade, publicMenuService)
        {
        }

        public ActionResult GetPanel()
        {
            var model = new ContactVm();
            return PartialView(model);
        }
    }
}