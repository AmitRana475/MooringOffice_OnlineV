using System.Web.Mvc;

namespace Shipment49Web.Areas.Data
{
    public class DataAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Data";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "Data_default",
                "Data/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}