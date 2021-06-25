using System.Web.Mvc;

namespace Shipment49Web.Areas.Certificate
{
    public class CertificateAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Certificate";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "Certificate_default",
                "certificatelist/{controller}/{action}/{id}",
                new {controller= "certificatelist", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}