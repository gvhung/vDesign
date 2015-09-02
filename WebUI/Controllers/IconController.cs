using Base.UI.Service;
using System.Web.Mvc;

namespace WebUI.Controllers
{
    public class IconController : Controller
    {
        private readonly IIconService _iconService;

        public IconController(IIconService iconService)
        {
            this._iconService = iconService;
        }

        [OutputCache(Duration = int.MaxValue)]
        public ActionResult GetIcons()
        {
            return this.PartialView(_iconService.GetIcons());
        }
    }
}