using System.Web.Mvc;

namespace Shipment49Web.Areas.MooringOperation
{
    public class MooringOperationAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "MooringOperation";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "MooringOperation_default",
                "MooringOperation/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}