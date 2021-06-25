using System.Web.Mvc;

namespace Shipment49Web.Areas.ImportExport
{
    public class ImportExportAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "ImportExport";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "ImportExport_default",
                "importexport/{controller}/{action}/{id}",
                new { controller = "importexport", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}