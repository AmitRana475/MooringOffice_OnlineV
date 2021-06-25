using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MenuLayer
{
    public class NotificationDashboard
    {
        public int VesselId { get; set; }
        public string VesselName { get; set; }
        public string FleetName { get; set; }
        public string FleetType { get; set; }
        public string NCsLastImport { get; set; }
        public string NCslast7Days { get; set; }
        public string NCsLastMonth { get; set; }
        public string NCsTotal { get; set; }
        public DateTime LastImport { get; set; }
        public DateTime DataAvailableTill { get; set; }

    }
}