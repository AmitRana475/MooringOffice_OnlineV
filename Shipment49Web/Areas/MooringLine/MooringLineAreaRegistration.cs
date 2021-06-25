using System.Web.Mvc;

namespace Shipment49Web.Areas.MooringLine
{
    public class MooringLineAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "MooringLine";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "MooringLine_default",
                "MooringLine/{controller}/{action}/{id}",
                new {  action = "Index", id = UrlParameter.Optional }
                  //new { controller = "(MooringLine)" }
            );
        }
    }
}