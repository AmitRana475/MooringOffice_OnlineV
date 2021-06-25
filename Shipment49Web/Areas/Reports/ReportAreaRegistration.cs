using System.Web.Mvc;

namespace Shipment49Web.Areas.CrewReport
{
    public class ReportAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Reports";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "Reports_default",
                "reports/{controller}/{action}/{id}",
                new { controller = "reports/workhours", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}