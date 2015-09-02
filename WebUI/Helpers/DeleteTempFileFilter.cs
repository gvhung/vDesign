using System.IO;
using System.Web.Mvc;

namespace WebUI.Helpers
{
    public class DeleteTempFileFilter : ActionFilterAttribute
    {
        public override void OnResultExecuted(ResultExecutedContext filterContext)
        {
            string fileName = ((FilePathResult)filterContext.Result).FileName;

            filterContext.HttpContext.Response.Flush();
            filterContext.HttpContext.Response.End();

            File.Delete(fileName);
        }
    }
}