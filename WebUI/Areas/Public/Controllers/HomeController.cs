using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Base.Content.Service.Abstract;
using Base.QueryableExtensions;
using WebUI.Areas.Public.Models;
using WebUI.Areas.Public.Service;
using WebUI.Controllers;

namespace WebUI.Areas.Public.Controllers
{
    public class HomeController : PublicBaseController
    {
        private readonly IContentCategoryService _contentCategoryService;
        public HomeController(IBaseControllerServiceFacade serviceFacade, PublicMenuService publicMenuService, IContentCategoryService contentCategoryService) : base(serviceFacade, publicMenuService)
        {
            _contentCategoryService = contentCategoryService;
        }

        //[OutputCache(Duration = 6000)]
        public ActionResult Index()
        {
            var viewModel = new HomePageViewModel(this);

            using (var uofw = CreateSystemUnitOfWork())
            {
                viewModel.Categories =
                    _contentCategoryService.GetAll(uofw).Where(x => x.ShowOnHomePage).Select(x => new CategoryItemVm()
                    {
                        Id = x.ID,
                        Title = x.Name,
                        Desciption = x.PublicTitle,
                        ImageId = x.ImageID.HasValue ? x.Image.FileID.ToString() : ""
                    }).ToList();
            }

            return View(viewModel);
        }
    }
}