using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Base.Content.Service.Abstract;
using Base.QueryableExtensions;
using Framework;
using WebUI.Areas.Public.Models;
using WebUI.Areas.Public.Service;
using WebUI.Controllers;

namespace WebUI.Areas.Public.Controllers
{
    [AllowAnonymous]
    public class HomeController : PublicBaseController
    {
        private readonly HomePageService _homePageService;
        
        public HomeController(IBaseControllerServiceFacade serviceFacade, PublicMenuService publicMenuService, HomePageService homePageService) : base(serviceFacade, publicMenuService)
        {
            _homePageService = homePageService;
        }

        //[OutputCache(Duration = 6000)]
        public async Task<ActionResult> Index()
        {
            var viewModel = new HomePageViewModel(this);

            using (var uofw = CreateSystemUnitOfWork())
            {
                viewModel.ContentItems = await _homePageService.GetContentListItems(uofw);
            }

            return View(viewModel);
        }

        [ChildActionOnly]
        public ActionResult ContentItems(List<ContentListItemVm> model)
        {
            return PartialView(model);
        }

        public async Task<JsonNetResult> InfinateScrollGetItems(int page)
        {
            List<ContentListItemVm> items;

            using (var uofw = CreateSystemUnitOfWork())
            {
                items = await _homePageService.GetContentListItems(uofw, page);
            }

            return new JsonNetResult(new
            {
                NoMoreData = items.Count < _homePageService.pageSize,
                HTMLString = RenderPartialViewToString("ContentItems", items)
            });
        }
    }
}