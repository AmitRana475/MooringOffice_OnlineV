using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Shipment49Web.Areas.MooringTail.Models
{
    public  class AssignLinetoWinch
    {
        public int Id { get; set; }
        public int RopeId { get; set; }
        public bool Outboard { get; set; }
        public int WinchId { get; set; }
        public string AssignedLocation { get; set; }
        public string AssignedDate { get; set; }
        public string Lead { get; set; }
        public int RopeTail { get; set; }
   
        public string CreatedBy { get; set; }
     
        public string ModifiedBy { get; set; }
      
        public int VesselID { get; set; }    

        public string AssignedNumber { get; set; }

        public string CertificateNumber { get; set; }

        public string UniqueId { get; set; }

        public string Status { get; set; }

        
    
       
        public List<AssignLinetoWinch> AssignMooringLineList { get; set; }
        public List<AssignLinetoWinch> AssignMooringLineList1 { get; set; }
    }
}