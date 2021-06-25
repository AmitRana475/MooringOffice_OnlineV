using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnalysisLayer
{
    public class ChartData
    {
        public int VesselId { get; set; }
        public string VesselName { get; set; }
        public string FleetName { get; set; }
        public string FleetType { get; set; }
        public string Rank { get; set; }
        public int Deviation { get; set; }
        public string DeviationType { get; set; }
        public decimal Work { get; set; }
        public decimal Rest { get; set; }
        public string Months { get; set; }
        public DateTime Months1 { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }

    }
}
