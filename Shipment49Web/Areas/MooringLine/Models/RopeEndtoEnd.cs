using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Shipment49Web.Areas.MooringLine.Models
{
    public class RopeEndtoEnd
    {
        public int Id { get; set; }
        public int RopeId { get; set; }      
        public string CreatedBy { get; set; }    
        public string ModifiedBy { get; set; }
        public bool IsActive { get; set; }    
     
       
        public string AssignedLocation { get; set; }
       
      
     
        public string EndtoEndDoneDate { get; set; }
        
        public string AssignedNumber { get; set; }
     
        public string CertificateNumber { get; set; }
      
        public string UniqueId { get; set; }
       
        public string CurrentOutboadEndinUse1 { get; set; }

   
        public string WasRopeShifted { get; set; }
     
        public bool Outboard { get; set; }

        public List<RopeEndtoEnd> RopeEndtoEndList { get; set; }
    }
}