using System.Web.Mvc;

namespace Shipment49Web.Areas.RopeTailInspectionSettings
{
    public class RopeTailInspectionSettingsAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "RopeTailInspectionSettings";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "RopeTailInspectionSettings_default",
                "RopeTailInspectionSettings/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}