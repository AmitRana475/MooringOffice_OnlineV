using System.Web.Mvc;

namespace Shipment49Web.Areas.LooseEquipInspectionSettings
{
    public class LooseEquipInspectionSettingsAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "LooseEquipInspectionSettings";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "LooseEquipInspectionSettings_default",
                "LooseEquipInspectionSettings/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}