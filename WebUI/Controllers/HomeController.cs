using System.Web.Mvc;
using WebUI.Models;

namespace WebUI.Controllers
{
    public class HomeController : BaseController
    {
        public HomeController(IBaseControllerServiceFacade serviceFacade)
            : base(serviceFacade)
        {
        }

        public ActionResult Index()
        {
            return View(new BaseViewModel(this));
        }
    }
}
