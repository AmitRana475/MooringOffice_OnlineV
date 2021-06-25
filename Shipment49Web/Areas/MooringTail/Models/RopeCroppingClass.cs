using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Shipment49Web.Areas.MooringTail.Models
{
    public class RopeCroppingClass
    {
        public int Id { get; set; }
      
        public string CroppedDate { get; set; }
        public string CroppedOutboardEnd { get; set; } 
        public string AssignedLocation { get; set; }
        public string AssignedNumber { get; set; }
        public string CertificateNumber { get; set; }
        public string UniqueId { get; set; }
        public string ReasonofCropping { get; set; }
        public decimal LengthofCroppedRope { get; set; }
        public int VesselID { get; set; }
        public int WinchId { get; set; }

        public List<RopeCroppingClass> RopeCroppingList { get; set; }
    }
}