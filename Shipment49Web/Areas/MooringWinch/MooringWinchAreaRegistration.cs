using System.Web.Mvc;

namespace Shipment49Web.Areas.MooringWinch
{
    public class MooringWinchAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "MooringWinch";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "MooringWinch_default",
                "MooringWinch/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}