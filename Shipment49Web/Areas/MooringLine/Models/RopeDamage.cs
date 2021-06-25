using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Shipment49Web.Areas.MooringLine.Models
{
    public class RopeDamage
    {

        public string AssignedLocation { get; set; }
        public string AssignedNumber { get; set; }
        public string CertificateNumber { get; set; }
        public string UniqueId { get; set; }
        public string DamageReason { get; set; }
        public string DamageLocation { get; set; }
        public string DamageDate { get; set; }
        public string IncidentReport { get; set; }
        public string DamageObserved { get; set; }
        public int VesselID { get; set; }
        public int Id { get; set; }
        public int WinchId { get; set; }

        public List<RopeDamage> RopeDamageList { get; set; }
    }
}