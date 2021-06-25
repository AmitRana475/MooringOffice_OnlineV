using System.Web.Mvc;

namespace Shipment49Web.Areas.Notification
{
    public class NotificationAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Notification";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "notification_default",
                "notification/{controller}/{action}/{id}",
                new { controller = "notification/deviations", action = "index", id = UrlParameter.Optional }
            );
        }
    }
}