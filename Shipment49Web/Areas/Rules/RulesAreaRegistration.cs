using System.Web.Mvc;

namespace Shipment49Web.Areas.Rules
{
    public class RulesAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Rules";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "rules_default",
                "rule/{controller}/{action}/{id}",
                new { controller = "rule", action = "Index", id = UrlParameter.Optional }
            );

            //context.MapRoute(
            //    "setting_default",
            //    "setting/{controller}/{action}/{id}",
            //    new { controller = "setting", action = "Index", id = UrlParameter.Optional }
            //);
        }
    }
}