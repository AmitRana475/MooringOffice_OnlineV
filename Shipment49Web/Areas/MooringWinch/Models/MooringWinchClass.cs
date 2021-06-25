using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Shipment49Web.Areas.MooringWinch.Models
{
    public class MooringWinchClass
    {
        public int Id { get; set; }
        //public int SortingOrder { get; set; }
        public string AssignedNumber { get; set; }
        public string Location { get; set; }
        public decimal MBL { get; set; }     
        
        public int VesselID { get; set; }
        public string Lead { get; set; }

        public List<MooringWinchClass> MooringWinchList { get; set; }
    }
}