using Reports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Shipment49Web.Areas.LooseEquipment.Models
{
    public class LooseEquipmentDetails
    {
        public LooseEquipmentDetails()
        {
            JoiningShackleList = new List<JoiningShackle>();
            ChainStopperList = new List<ChainStopper>();
            ChafeGuardList = new List<ChafeGuard>();
            WinchBreakTestKitList = new List<WinchBreakTestKit>();
            MessengerRopeList = new List<RopeTail>();
            RopeStopperList = new List<RopeTail>();
            FireWireList = new List<RopeTail>();
            TowingRopeList = new List<RopeTail>();
            SuezRopeRopeList = new List<RopeTail>();
        }
        public List<JoiningShackle> JoiningShackleList { get; set; }
        public List<ChainStopper> ChainStopperList { get; set; }
        public List<ChafeGuard> ChafeGuardList { get; set; }
        public List<WinchBreakTestKit> WinchBreakTestKitList { get; set; }
        public List<RopeTail> MessengerRopeList { get; set; }
        public List<RopeTail> RopeStopperList { get; set; }
        public List<RopeTail> FireWireList { get; set; }
        public List<RopeTail> TowingRopeList { get; set; }
        public List<RopeTail> SuezRopeRopeList { get; set; }

    }
}