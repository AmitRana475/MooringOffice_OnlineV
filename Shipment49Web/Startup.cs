using MenuLayer;
using Microsoft.Owin;
using Owin;
using System.Web.Security;
using Shipment49Web.Models;
using System.Linq;
using System.Web.Helpers;
using System.Collections.Generic;

[assembly: OwinStartupAttribute(typeof(Shipment49Web.Startup))]
namespace Shipment49Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            AntiForgeryConfig.SuppressXFrameOptionsHeader = true; // It's using for Export file (Http)

            ConfigureAuth(app);
                     
        }
    }
}
