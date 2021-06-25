using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Shipment49Web.ViewModels
{
    public class NotificationSearchViewModel
    {
        //public int FleetName { get; set; }
        //public int FleetType { get; set; }
        //public int VesselInfo { get; set; }
        //public int TradePlatform { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }


        public List<long> FleetIDs { get; set; }
        public List<long> FleetTypeIDs { get; set; }
        public List<long> VesselIDs { get; set; }
        public List<long> TradeIDs { get; set; }
        public List<Reports.tblCommon> FleetNameList { get; set; }
        public List<Reports.tblCommon> FleetTypeList { get; set; }
        public List<Reports.tblCommon> TradePlatformList { get; set; }
        public List<Reports.VesselDetail> VesselList { get; set; }
        public List<Reports.View_Notifications> NotificationsList { get; set; }
    }
}