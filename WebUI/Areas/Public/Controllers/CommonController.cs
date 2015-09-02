using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Base.Content.Entities;
using Base.Content.Service.Abstract;
using Base.QueryableExtensions;
using Base.Task.Entities;
using Kendo.Mvc.Extensions;
using WebUI.Controllers;

namespace WebUI.Areas.Public.Controllers
{
    public class CommonController : BaseController
    {

        private readonly IContentCategoryService _contentCategoryService;

        public CommonController(IBaseControllerServiceFacade serviceFacade, IContentCategoryService contentCategoryService) : base(serviceFacade)
        {
            _contentCategoryService = contentCategoryService;
        }

        public ActionResult Menu()
        {
            List<ContentCategory> menuItems;

            using (var uofw = CreateSystemUnitOfWork())
            {
                menuItems = _contentCategoryService.GetAll(uofw).Where(x => x.ShowInMenu).ToList();
            }

            return PartialView(menuItems);
        }
    }
}