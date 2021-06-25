using Elmah;
using System;
using System.Web.Mvc;

namespace Shipment49Web.Models
{
    public class ErrorClass: HandleErrorAttribute
    {
        public override void OnException(ExceptionContext filterContext)
        {
            Exception ex = filterContext.Exception;
            ErrorSignal.FromCurrentContext().Raise(ex);
            filterContext.ExceptionHandled = true;
            var model = new HandleErrorInfo(filterContext.Exception, "Controller", "Action");

            filterContext.Result = new ViewResult()
            {
                ViewName = "Error",
                ViewData = new ViewDataDictionary(model)
            };
        }
    }
}