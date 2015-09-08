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
    public class HomePageService
    {
        private readonly IContentItemService _contentItemService;
        public readonly int pageSize = 8;

        public HomePageService(IContentItemService contentItemService)
        {
            _contentItemService = contentItemService;
        }

        public async Task<List<ContentListItemVm>> GetContentListItems(IUnitOfWork uofw, int page = 1)
        {
            var startIndex = (page - 1) * pageSize;
            return await _contentItemService.GetAll(uofw).Where(x => x.OnHome).OrderByDescending(x => x.Top).ThenBy(x => x.ID).Select(x => new ContentListItemVm()
            {
                Id = x.ID,
                Title = x.Title,
                Desciption = x.ShortDescription,
                ImageId = x.ImagePreviewID.HasValue ? x.ImagePreview.FileID.ToString() : "",
                CategoryId = x.CategoryID
            }).Skip(startIndex).Take(pageSize).ToGenericListAsync();
        }
    }
}