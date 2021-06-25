using System.Web.Mvc;

namespace Shipment49Web.Areas.DynamicMenus
{
    public class DynamicMenusAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "DynamicMenus";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "DynamicMenus_default",
                "DynamicMenus/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}