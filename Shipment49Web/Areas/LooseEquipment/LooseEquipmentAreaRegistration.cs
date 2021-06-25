using System.Web.Mvc;

namespace Shipment49Web.Areas.LooseEquipment
{
    public class LooseEquipmentAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "LooseEquipment";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "LooseEquipment_default",
                "LooseEquipment/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}