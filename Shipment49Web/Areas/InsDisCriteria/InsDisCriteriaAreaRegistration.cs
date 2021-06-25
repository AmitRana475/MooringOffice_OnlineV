using System.Web.Mvc;

namespace Shipment49Web.Areas.InsDisCriteria
{
    public class InsDisCriteriaAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "InsDisCriteria";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "InsDisCriteria_default",
                "InsDisCriteria/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}