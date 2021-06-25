using System.Web.Mvc;

namespace Shipment49Web.Areas.MooringTail
{
    public class MooringTailAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "MooringTail";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "MooringTail_default",
                "MooringTail/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}