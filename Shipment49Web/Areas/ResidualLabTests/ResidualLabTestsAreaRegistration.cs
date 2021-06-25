using System.Web.Mvc;

namespace Shipment49Web.Areas.ResidualLabTests
{
    public class ResidualLabTestsAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "ResidualLabTests";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "ResidualLabTests_default",
                "ResidualLabTests/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}