using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Base.Content.Entities;
using Base.Content.Service.Abstract;
using Base.DAL;
using Base.QueryableExtensions;
using Base.Task.Entities;
using Kendo.Mvc.Extensions;
using WebUI.Controllers;

namespace WebUI.Areas.Public.Controllers
{
    [AllowAnonymous]
    public class CommonController : Controller
    {

        private readonly IContentCategoryService _contentCategoryService;
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;

        public CommonController(IContentCategoryService contentCategoryService, IUnitOfWorkFactory unitOfWorkFactory)
        {
            _contentCategoryService = contentCategoryService;
            _unitOfWorkFactory = unitOfWorkFactory;
        }

        public ActionResult Menu()
        {
            List<ContentCategory> menuItems;

            using (var uofw = _unitOfWorkFactory.CreateSystem())
            {
                menuItems = _contentCategoryService.GetAll(uofw).Where(x => x.ShowInMenu).ToList();
            }

            return PartialView(menuItems);
        }
    }
}