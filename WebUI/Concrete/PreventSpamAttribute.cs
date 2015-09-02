using Base.Ambient;
using Framework;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web.Caching;
using System.Web.Mvc;

namespace WebUI.Concrete
{
    public class PreventSpamAttribute : ActionFilterAttribute
    {
        public string errorMessage = "Слишком много обращений";
        public int delayRequest;
        public PreventSpamMode mode;

        public PreventSpamAttribute(int delayRequest = 10, PreventSpamMode mode = PreventSpamMode.GuestsAndUsers, string errorMessage = null)
        {
            this.delayRequest = delayRequest;
            this.mode = mode;

            if (!String.IsNullOrWhiteSpace(errorMessage))
                this.errorMessage = errorMessage;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if(!((mode.HasFlag(PreventSpamMode.Guests) && AppContext.SecurityUser.IsGuest) ||
               (mode.HasFlag(PreventSpamMode.Admin) && AppContext.SecurityUser.IsAdmin) ||
               (mode.HasFlag(PreventSpamMode.Users) && !AppContext.SecurityUser.IsAdmin && !AppContext.SecurityUser.IsGuest)))
            {
                base.OnActionExecuting(filterContext);
                return;
            }

            // Store our HttpContext (for easier reference and code brevity)
            var request = filterContext.HttpContext.Request;
            // Store our HttpContext.Cache (for easier reference and code brevity)
            var cache = filterContext.HttpContext.Cache;

            // Grab the IP Address from the originating Request (example)
            var originationInfo = request.ServerVariables["HTTP_X_FORWARDED_FOR"] ?? request.UserHostAddress;

            // Append the User Agent
            originationInfo += request.UserAgent;

            // Now we just need the target URL Information
            var targetInfo = request.RawUrl + request.QueryString + request.Form;

            // Generate a hash for your strings (appends each of the bytes of
            // the value into a single hashed string
            var hashValue = string.Join("", MD5.Create().ComputeHash(Encoding.ASCII.GetBytes(originationInfo + targetInfo)).Select(s => s.ToString("x2")));

            // Checks if the hashed value is contained in the Cache (indicating a repeat request)
            if (cache[hashValue] != null)
            {
                // Adds the Error Message to the Model and Redirect
                // filterContext.Controller.ViewData.ModelState.AddModelError("ExcessiveRequests", errorMessage);
                filterContext.Result = new JsonNetResult(new {error = errorMessage});
            }
            else
            {
                // Adds an empty object to the cache using the hashValue
                // to a key (This sets the expiration that will determine
                // if the Request is valid or not)
                cache.Add(hashValue, new object(), null, DateTime.Now.AddSeconds(delayRequest), Cache.NoSlidingExpiration, CacheItemPriority.Default, null);
            }
            base.OnActionExecuting(filterContext);
        }
    }

    [Flags]
    public enum PreventSpamMode
    {
        None = 0,
        Guests = 2,
        Users = 4,
        Admin = 8,
        GuestsAndUsers = Guests | Users,
        All = GuestsAndUsers | Admin
    }
}