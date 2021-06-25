using Reports;
using Shipment49Web.Areas.MooringLine.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Shipment49Web.Areas.LineStatus.Models
{
    public class LineStatusClass
    {
        public int Id { get; set; }    

        public int? RopeTypeId { get; set; }
        public string RopeConstruction { get; set; }
        public decimal? DiaMeter { get; set; }
        public decimal? Length { get; set; }
     
        public decimal? MBL { get; set; }
        public decimal? LDBF { get; set; }
        public decimal? WLL { get; set; }
     
        public int? ManufacturerId { get; set; }
        public string CertificateNumber { get; set; }
       
        public string ReceivedDate1 { get; set; }
       // public DateTime? InstalledDate { get; set; }
        public string RopeTagging { get; set; }
      
        public string MooringOperation { get; set; }
        public int? MooringOperationID { get; set; }
      
       
        public bool IsActive { get; set; }
      

        public decimal? CurrentRunningHours { get; set; } = 0;


        public decimal? CurrentLeadRunningHours { get; set; } = 0;

        public int? MaxRunningHours { get; set; }
        public int? MaxMonthsAllowed { get; set; }

     
        public decimal? StartCounterHours { get; set; } = 0;
        public int? RopeTail { get; set; }
        public bool DeleteStatus { get; set; }

        public string Remarks { get; set; }
        public string UniqueID { get; set; }
 

        public string RopeType { get; set; }
        public string OutofServiceDate1 { get; set; }

        public string ManufacturerName { get; set; }

        public string Location { get; set; }

        public string AssignedWinch { get; set; }

        public int WinchId { get; set; }


        public string InstalledDate1 { get; set; }


        public string InspectionDueDate1 { get; set; }

       // public string InspectionDueDate { get; set; }


        public string IsRopeInstalled { get; set; }

        public int VesselID { get; set; }

        public decimal? CurrentLength { get; set; }

        public List<LineStatusClass> MooringLineList { get; set; }    

        public List<Mooringinspection> InspectionDetail { get; set; }
        public List<RopeSplicing> RopeSplicingList { get; set; }

        public List<RopeCroppingClass> RopeCroppingList { get; set; }

        public List<RopeDamage> RopeDamageList { get; set; }

        public List<RopeDiscardClass> MooringLineDiscardList { get; set; }

        public List<RopeDisposals> RopeDisposalList { get; set; }

        public List<RopeEndtoEnd> RopeEndtoEndList { get; set; }
        public List<WinchRotationClass> WinchRotationList { get; set; }
    }


    public class WinchRotationClass
    {
        public string Location { get; set; }
        public string AssignedNumber { get; set; }
        public string AssignedDate { get; set; }
        public string Lead { get; set; }
        public decimal? RunningHours { get; set; }
    }
    public class RopeDiscardClass
    {
        public int Id { get; set; }
        public int RopeId { get; set; }
        public string RopeType { get; set; }
        public string AssignedLocation { get; set; }
        public string AssignedNumber { get; set; }
        public string ReasonOutofService { get; set; }
        public string CertificateNumber { get; set; }
        public string OutofServiceDate1 { get; set; }
    }
    public class Mooringinspection
    {
        public int? Id { get; set; }
        public string InspectBy { get; set; }
        public string InspectDate1 { get; set; }
        public string CertificateNumber { get; set; }
        public int RopeId { get; set; }
        public int WinchId { get; set; }
        public int InspectionId { get; set; }
        public int ExternalRating_A { get; set; }
        public int InternalRating_A { get; set; }
        public int AverageRating_A { get; set; }
        public decimal? LengthOFAbrasion_A { get; set; }
        public decimal? DistanceOutboard_A { get; set; }
        public decimal? CutYarnCount_A { get; set; }
        public decimal? LengthOFGlazing_A { get; set; }
        public int RopeTail { get; set; }
        public int ExternalRating_B { get; set; }
        public int InternalRating_B { get; set; }
        public int AverageRating_B { get; set; }
        public decimal? LengthOFAbrasion_B { get; set; }
        public decimal? DistanceOutboard_B { get; set; }
        public decimal? CutYarnCount_B { get; set; }
        public decimal? LengthOFGlazing_B { get; set; }
        public string Chafe_guard_condition { get; set; }
        public int Twist { get; set; }
        public string Photo1 { get; set; }
        public string Photo2 { get; set; }
        public string Image1 { get; set; }
        public string Image2 { get; set; }
        public int VesselID { get; set; }
    }
}