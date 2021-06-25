using System.Web.Mvc;

namespace Shipment49Web.Areas.MSPS
{
    public class MSPSAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "MSPS";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "MSPS_default",
                "MSPS/{controller}/{action}/{id}",
                new { controller = "MSMP", action = "Index", id = UrlParameter.Optional }
                
            );
        }
    }
}