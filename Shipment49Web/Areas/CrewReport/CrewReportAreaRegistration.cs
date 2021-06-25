using System.Web.Mvc;

namespace Shipment49Web.Areas.CrewReport
{
    public class CrewReportAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "CrewReport";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "CrewReport_default",
                "crewreport/{controller}/{action}/{id}",
                new { controller = "crewreport", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}