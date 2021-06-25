using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Shipment49Web.Areas.MooringTail.Models
{
    public class RopeSplicing
    {
        public int Id { get; set; }
        public int RopeId { get; set; }
        public string SplicingDoneDate { get; set; }
        public string SplicingMethod { get; set; }
        public string SplicingDoneBy { get; set; }
        public int RopeTail { get; set; }
        public int NotificationId { get; set; }
        public int DamageId { get; set; }
        public int MOpid { get; set; }

        public string AssignedLocation { get; set; }

        public string AssignedNumber { get; set; }

        public string CertificateNumber { get; set; }

        public string UniqueId { get; set; }

        public string ReasonofCropping { get; set; }

        public decimal LengthofCroppedRope { get; set; }

        public int VesselID { get; set; }
        public int  WinchId { get; set; }

        public List<RopeSplicing> RopeSplicingList { get; set; }
    }
}