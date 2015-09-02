using System.Web.Mvc;
using WebUI.Models;

namespace WebUI.Controllers
{
    public class CaptchaController : Controller
    {
        [HttpGet]
        public ActionResult CaptchaImage()
        {

            CaptchaModel Captcha = new CaptchaModel();

            //Записываем в сессию результат каптчи
            Session["Captcha"] = Captcha.Result;

            //Генерируем каптчу
            FileContentResult img = this.File(Captcha.getCaptcha(Captcha.CaptchaString, CaptchaModel.CaptchaType.Jpeg, 130, 30, true), "image/Jpeg");

            return img;
        }
    }
}
