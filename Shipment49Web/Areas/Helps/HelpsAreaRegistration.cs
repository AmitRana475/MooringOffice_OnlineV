using System.Web.Mvc;

namespace Shipment49Web.Areas.Helps
{
    public class HelpsAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "helps";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "helps_default",
                "helps/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}