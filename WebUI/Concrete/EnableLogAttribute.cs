using Base.Ambient;
using Data.Entities;
using Data.Service.Abstract;
using System;
using System.Web.Mvc;

namespace Framework.Attributes
{
    public class EnableLogAttribute : ActionFilterAttribute
    {
        private readonly IRequestLogService _logService;

        private RequestLog _request;

        public EnableLogAttribute()
        {
            _logService = DependencyResolver.Current.GetService<IRequestLogService>();
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            string url = filterContext.RequestContext.HttpContext.Request.RawUrl;
            var random = new Random(unchecked(url.GetHashCode() + DateTime.Now.GetHashCode()));
            int requestID = random.Next(Int32.MinValue, Int32.MaxValue);

            _request = new RequestLog
            {
                ID = requestID,
                Start = DateTime.Now,
                User = AppContext.SecurityUser.Login,
                Request = url
            };

            try
            {
                _logService.Create(null, _request);
            }
            catch { }

            base.OnActionExecuting(filterContext);
        }

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            _request.End = DateTime.Now;

            try
            {
                _logService.Update(null, _request);
            }
            catch { }

            base.OnActionExecuted(filterContext);
        }
    }

}
