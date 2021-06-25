using System.Web.Mvc;

namespace Shipment49Web.Areas.Analysis
{
    public class AnalysisAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Analysis";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "Analysis_default",
                "analysis/{controller}/{action}/{id}",
                new { controller = "analysis/graphictrendview", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}