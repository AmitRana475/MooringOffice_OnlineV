using System;
using System.Web.Mvc;
using System.Web.Mvc.Filters;

namespace Shipment49Web.Common
{
    public class UserAuthenticationFilter : ActionFilterAttribute, IAuthenticationFilter
    {
        public void OnAuthentication(AuthenticationContext filterContext)
        {
            //Check Session is Empty Then set as Result is HttpUnauthorizedResult 
            if (string.IsNullOrEmpty(Convert.ToString(filterContext.HttpContext.Session["LoggedInUserID"])))
            {
                filterContext.Result = new HttpUnauthorizedResult();
            }
        }

        // Runs after the OnAuthentication method  
        // OnAuthenticationChallenge:- if Method gets called when Authentication or Authorization is failed and this method is called after
        // Execution of Action Method but before rendering of View
        public void OnAuthenticationChallenge(AuthenticationChallengeContext filterContext)
        {
            //We are checking Result is null or Result is HttpUnauthorizedResult 
            // if yes then we are Redirect to Error View
            if (filterContext.Result == null || filterContext.Result is HttpUnauthorizedResult)
            {
                filterContext.Result = new RedirectResult("~/account/login");
                //filterContext.Result = new ViewResult
                //{
                //    ViewName = "login"
                //};
            }
        }
    }
}