using System.Web.Mvc;

namespace Shipment49Web.Areas.LineStatus
{
    public class LineStatusAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "LineStatus";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "LineStatus_default",
                "LineStatus/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}