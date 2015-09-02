using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Caching;
using Base.Content.Service.Abstract;
using Base.DAL;
using Base.QueryableExtensions;
using Base.UI;
using WebUI.Areas.Public.Helpers;
using WebUI.Areas.Public.Models;

namespace WebUI.Areas.Public.Service
{
    public class PublicMenuService
    {
        private readonly IContentCategoryService _contentCategoryService;
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private static string MENU_KEY = "0b35b336-9ad9-4436-b565-b403ff5e1b7d";

        public PublicMenuService(IContentCategoryService contentCategoryService, IUnitOfWorkFactory unitOfWorkFactory)
        {
            _contentCategoryService = contentCategoryService;
            _unitOfWorkFactory = unitOfWorkFactory;
        }

        private List<MenuItemVm> Items { get; set; }
        public List<MenuItemVm> GetMenu()
        {
            return Items ?? InitMenu();
        }

        private List<MenuItemVm> InitMenu()
        {
            using (var uofw = _unitOfWorkFactory.CreateSystem())
            {
                Items = HttpRuntime.Cache.GetOrStore(MENU_KEY, () => _contentCategoryService.GetRoots(uofw).Include(x => x.Children).ToList().Select(x => new MenuItemVm()
                {
                    Id = x.ID,
                    Title = x.Name,
                    SubmenuItems = x.Children.Select(y => new MenuItemVm()
                    {
                        Id = y.ID,
                        Title = y.Name,
                    })
                }).ToList());
            }

            return Items;
        }

    }
}