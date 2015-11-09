using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WebUI.Areas.Public.Models;
using WebUI.Areas.Public.Service;
using WebUI.Controllers;

namespace WebUI.Areas.Public.Controllers
{
    public class ContentController : PublicBaseController
    {
        private readonly ContentPageService _contentPageService;
        public ContentController(IBaseControllerServiceFacade serviceFacade, PublicMenuService publicMenuService, ContentPageService contentPageService) : base(serviceFacade, publicMenuService)
        {
            _contentPageService = contentPageService;
        }

        public async Task<ActionResult> Category(int id)
        {
            var viewModel = new ContentCategoryPageViewModel(this);

            using (var uofw = CreateSystemUnitOfWork())
            {
                viewModel.Category = await _contentPageService.GetCurrentCategory(uofw, id);
                viewModel.ContentItems = await _contentPageService.GetContentItems(uofw, id);
                viewModel.CategoryItems = await _contentPageService.GetSubcategories(uofw, id);
            }

            viewModel.ThemeColor = viewModel.Category.Color;

            return View(viewModel);
        }

        public async Task<ActionResult> Read(int id)
        {
            var viewModel = new ContentPageViewModel(this);

            using (var uofw = CreateSystemUnitOfWork())
            {
                viewModel.ContentItem = await _contentPageService.GetContentItem(uofw, id);
            }

            return View(viewModel);
        }
    }
}