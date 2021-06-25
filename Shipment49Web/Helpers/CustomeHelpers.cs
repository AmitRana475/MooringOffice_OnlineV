using MenuLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Shipment49Web.Helpers
{
    public static class CustomHelpers
    {
        public static string GetCommonType(this HtmlHelper helper, int id)
        {
            using (ShipmentContaxt sc1 = new ShipmentContaxt())
            {
                var obj = sc1.MasterCommons.FirstOrDefault(e => e.Id==id);
                if (obj != null)
                    return obj.Name;
                return string.Empty;
            }
        }

        
    }
}