using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Base.Content.Service.Abstract;
using Base.DAL;
using Base.QueryableExtensions;
using WebUI.Areas.Public.Models;

namespace WebUI.Areas.Public.Service
{
    public class ContentPageService
    {
        private readonly IContentCategoryService _contentCategoryService;
        private readonly IContentItemService _contentItemService;

        public ContentPageService(IContentCategoryService contentCategoryService, IContentItemService contentItemService)
        {
            _contentCategoryService = contentCategoryService;
            _contentItemService = contentItemService;
        }

        public async Task<CategoryItemVm> GetCurrentCategory(IUnitOfWork uofw, int id)
        {
            var categoryItem = new CategoryItemVm();

            var cat = await _contentCategoryService.GetAll(uofw).Where(x => x.ID == id).FirstOrDefaultAsync();

            if (cat != null)
            {
                categoryItem.Id = cat.ID;
                categoryItem.Title = cat.Name;
                categoryItem.Mnemonic = cat.CategoryItemMnemonic;
                categoryItem.ImageId = cat.ImageID.HasValue ? cat.Image.FileID.ToString() : string.Empty;
                categoryItem.Expand = cat.Expanded;
                categoryItem.Desciption = cat.PublicTitle;
                categoryItem.Color = string.IsNullOrEmpty(cat.Color) ? "#FCC10C" : cat.Color;
            }

            return categoryItem;
        }

        public async Task<List<ContentItemVm>> GetContentItems(IUnitOfWork uofw, int id)
        {
            return
                await
                    _contentItemService.GetAll(uofw)
                        .Where(x => x.CategoryID == id)
                        .OrderByDescending(x => x.Top)
                        .ThenBy(x => x.ID)
                        .Select(x => new ContentItemVm()
                        {
                            Id = x.ID,
                            Title = x.Title,
                            Desciption = x.ShortDescription,
                            ImageId = x.ImagePreviewID.HasValue ? x.ImagePreview.FileID.ToString() : "",
                            CategoryId = x.CategoryID,
                            Content = x.Content.Html
                        }).ToGenericListAsync();
        }

        public async Task<ContentItemVm> GetContentItem(IUnitOfWork uofw, int id)
        {
            var contentItem = new ContentItemVm();

            var content = await _contentItemService.GetAll(uofw).Where(x => x.ID == id).FirstOrDefaultAsync();

            if (content != null)
            {
                contentItem.Id = content.ID;
                contentItem.Title = content.Title;
                contentItem.ImageId = content.ImagePreviewID.HasValue ? content.ImagePreview.FileID.ToString() : string.Empty;
                contentItem.CategoryId = content.CategoryID;
                contentItem.Desciption = content.Description;
                contentItem.Content = content.Content.Html;
            }

            return contentItem;
        }

        public async Task<List<CategoryItemVm>> GetSubcategories(IUnitOfWork uofw, int id)
        {
            return
                await
                    _contentCategoryService.GetAll(uofw)
                        .Where(x => x.ParentID.HasValue && x.ParentID == id)
                        .OrderBy(x => x.SortOrder)
                        .Select(x => new CategoryItemVm()
                        {
                            Id = x.ID,
                            Title = x.Name,
                            Expand = x.Expanded,
                            Desciption = x.PublicTitle,
                            ImageId = x.ImageID.HasValue ? x.Image.FileID.ToString() : ""
                        }).ToGenericListAsync();
        }
    }
}