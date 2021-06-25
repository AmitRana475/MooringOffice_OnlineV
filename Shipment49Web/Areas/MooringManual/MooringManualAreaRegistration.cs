using System.Web.Mvc;

namespace Shipment49Web.Areas.MooringManual
{
    public class MooringManualAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "MooringManual";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "MooringManual_default",
                "MooringManual/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}