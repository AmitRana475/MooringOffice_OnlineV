using System.Web.Mvc;

namespace Shipment49Web.Areas.UserHelp
{
    public class UserHelpAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "UserHelp";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "UserHelp_default",
                "userhelp/{controller}/{action}/{id}",
                new { controller= "userhelp", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}