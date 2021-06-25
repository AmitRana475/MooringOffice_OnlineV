using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Shipment49Web.Areas.MooringTail.Models
{
    public class RopeDisposals
    {
        public string ReceptionFacilityName { get; set; }
        public string DisposalPortName { get; set; }
                      
        public string CertificateNumber { get; set; }
        public string UniqueId { get; set; }      
        public string DisposalDate { get; set; }  
        public int VesselID { get; set; }
        public int Id { get; set; }
        public int RopeId { get; set; }

        public List<RopeDisposals> RopeDisposalList { get; set; }
    }
}