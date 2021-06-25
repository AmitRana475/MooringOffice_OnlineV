using Shipment49Web.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Caching;
using System.Web.Mvc;
using System.Security.Cryptography;
using System.Text;

namespace Shipment49Web.Models
{
    public class PreventSpamAttribute : ActionFilterAttribute
    {
        private readonly int RateLimit = 0;
        private readonly int RateLimitTimeOut = 0;
        private readonly string RateLimitOnMethod = string.Empty;
        //private EFContext objContext = null;
        public PreventSpamAttribute(string RateLimitOnMethod = "", int RateLimit = 3, int RateLimitTimeOut = 20)
        {
            this.RateLimit = RateLimit;
            this.RateLimitTimeOut = RateLimitTimeOut;
            this.RateLimitOnMethod = RateLimitOnMethod;
           
        }

        //
        // Summary:
        //     Called by the ASP.NET MVC framework after the action method executes.
        //
        // Parameters:
        //   filterContext:
        //     The filter context.
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            base.OnActionExecuted(filterContext);
        }
        //
        // Summary:
        //     Called by the ASP.NET MVC framework before the action method executes.
        //
        // Parameters:
        //   filterContext:
        //     The filter context.
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            //Get IP Address from HttpRequest to check if the ip exists in database 
            string IpAddress = CommonClass.GetIpAddress(filterContext.HttpContext.Request);
            //Get the current DateTime minus the minutes set in the RateLimitTimeOut variable
            DateTime checkTime = DateTime.Now.AddMinutes(RateLimitTimeOut * -1);
            //Get number of rows in the table with respect to the IP Address and the method name set in the argument of the constructor
            //and the CreatedOn datetime should be greater than the checkTime variable.
            //int Count = objContext.LoggedIPAddresses.AsQueryable().Where(x => x.IPAddress == IpAddress && x.CreatedOn > checkTime && x.MethodName == RateLimitOnMethod).OrderByDescending(x => x.CreatedOn).Take(RateLimit).Count();
            int Count = CommonClass.LoggedIPAddressesGet(IpAddress, RateLimitOnMethod, checkTime);
            if (Count >= RateLimit)
            {
                //if count exceeds the ratelimit then we need to show error with the Http Status Code "429" which stands for "Too Many Requests".
                filterContext.Result = new HttpStatusCodeResult(429, "Too many requests");
            }
            else
            {
                //create a log in the table to verify in future requests.
                // objContext.LoggedIPAddresses.Add(new LoggedIPAddress { CreatedOn = DateTime.Now, MethodName = this.RateLimitOnMethod, IPAddress = IpAddress, LogID = 0 });
                CommonClass.LoggedIPAddressesAdd(IpAddress,this.RateLimitOnMethod, DateTime.Now);
            }
            base.OnActionExecuting(filterContext);
        }
        //
        // Summary:
        //     Called by the ASP.NET MVC framework after the action result executes.
        //
        // Parameters:
        //   filterContext:
        //     The filter context.
        public override void OnResultExecuted(ResultExecutedContext filterContext)
        {
            base.OnResultExecuted(filterContext);
        }
        //
        // Summary:
        //     Called by the ASP.NET MVC framework before the action result executes.
        //
        // Parameters:
        //   filterContext:
        //     The filter context.
        public override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            base.OnResultExecuting(filterContext);
        }
    }
}