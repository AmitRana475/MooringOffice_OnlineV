using System.Web.Mvc;

namespace Shipment49Web.Areas.CommonTypes
{
    public class CommonTypesAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "CommonTypes";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "CommonTypes_default",
                "CommonTypes/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}