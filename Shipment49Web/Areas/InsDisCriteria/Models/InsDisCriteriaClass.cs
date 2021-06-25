using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Shipment49Web.Areas.InsDisCriteria.Models
{
    public class InsDisCriteriaClass
    {
        public string Name { get; set; }
        public string RopeType { get; set; }

        public int MaximumRunningHours { get; set; }
        public int MaximumMonthsAllowed { get; set; }
        public int EndtoEndMonth { get; set; }
        public decimal Rating1 { get; set; }
        public decimal Rating2 { get; set; }
        public decimal Rating3 { get; set; }
        public decimal Rating4 { get; set; }
        public decimal Rating5 { get; set; }
        public decimal Rating6 { get; set; }
        public decimal Rating7 { get; set; }
       

        public List<InsDisCriteriaClass> InsDisCriteriaList { get; set; }
        public List<InsDisCriteriaClass> InsDisCriteriaList1 { get; set; }

    }

    public class WinchRotationClass
    {
        public int Id { get; set; }
        public string RopeType { get; set; }

        public string Name { get; set; }
        public int MaximumMonthsAllowed { get; set; }
        public int MaximumRunningHours { get; set; }
        public string LeadFrom { get; set; }
        public string LeadTo { get; set; }
        public int VesselID { get; set; }
       


        public List<WinchRotationClass> WinchRotationList { get; set; }
      

    }

    public class LEdiscardsettingClass
    {
        public int Id { get; set; }
        public int EquipmentType { get; set; }
        public int InspectionFrequency { get; set; }

        public string looseequipmenttype { get; set; }
        
        public int MaximumMonthsAllowed { get; set; }
        public int MaximumRunningHours { get; set; }
        

        public List<LEdiscardsettingClass> LEdiscardsettingList { get; set; }


    }

}