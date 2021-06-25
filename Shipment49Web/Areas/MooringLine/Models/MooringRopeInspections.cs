using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Shipment49Web.Areas.MooringLine.Models
{
    public class MooringRopeInspections
    {
        public int InspectionId  { get; set; }
        public string InspectBy { get; set; }
        public string InspectDate { get; set; }
        public string Years { get; set; }
        public List<string> YearList { get; set; }
        public List<MooringRopeInspections> RopeInspectionList { get; set; }

        //public string RopeType { get; set; }
        //public string AssignedNumber { get; set; }
        //public string CertificateNumber { get; set; }
        //public string UniqueId { get; set; }
        //public string Location { get; set; }
        //public int WinchId { get; set; }
        //public int RopeId { get; set; }
        //public int VesselId { get; set; }
        //public List<MooringRopeInspections> AddInspectionList { get; set; }
    }
}