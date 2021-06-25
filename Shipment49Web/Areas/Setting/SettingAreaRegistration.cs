using System.Web.Mvc;

namespace Shipment49Web.Areas.Setting
{
    public class SettingAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Setting";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "setting_default",
                "setting/{controller}/{action}/{id}",
                new { controller = "setting", action = "Index", id = UrlParameter.Optional }
            );

            context.MapRoute(
                "Company_default",
                "setting/{controller}/{action}/{id}",
                new { area = "Setting", controller = "companyprofile", action = "Index", id = UrlParameter.Optional }
               
            );
            //context.MapRoute(
            //    "Vessel_default",
            //    "setting/{controller}/{action}/{id}",
            //    new { area = "Setting", controller = "vessel", action = "Index", id = UrlParameter.Optional }
            //);
        }
    }
}