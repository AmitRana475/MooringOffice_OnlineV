using System.Web.Mvc;

namespace Shipment49Web.Areas.TrainingContent
{
    public class TrainingContentAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "TrainingContent";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "TrainingContent_default",
                "TrainingContent/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}