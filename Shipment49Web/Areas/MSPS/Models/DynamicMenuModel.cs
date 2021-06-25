using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MenuLayer;

namespace Shipment49Web.Areas.MSPS.Models
{
    public class DynamicMenuModel
    {
        public Menu Menuss { get; set; }
        public SubMenu SubMenus { get; set; }
        public SubSubMenu SubSubMenus { get; set; }

        public SmartMenu SmartMenus { get; set; }
    }
}