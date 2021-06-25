using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Shipment49Web.Areas.MooringLine.Models
{
    public partial class MooringRopeDetails
    {
        public int Id { get; set; }
        //public string RopeType { get; set; }

        public int? RopeTypeId { get; set; }
        public string RopeConstruction { get; set; }
        public decimal? DiaMeter { get; set; }
        public decimal? Length { get; set; }
        // public decimal? TtlCroppedLength { get; set; }
        public decimal? MBL { get; set; }
        public decimal? LDBF { get; set; }
        public decimal? WLL { get; set; }
        //public string ManufacturerName { get; set; }
        public int? ManufacturerId { get; set; }
        public string CertificateNumber { get; set; }
        public DateTime? ReceivedDate { get; set; }
        public DateTime? InstalledDate { get; set; }
        public string RopeTagging { get; set; }
        //public DateTime? OutofServiceDate { get; set; }
        public string ReasonOutofService { get; set; }
        public string OtherReason { get; set; }
        public string DamageObserved { get; set; }
        public string IncidentReport { get; set; }


        public string MooringOperation { get; set; }
        public int? MooringOperationID { get; set; }
        // public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        // public DateTime? ModifiedDate { get; set; }
        public string ModifiedBy { get; set; }
        public bool IsActive { get; set; }
        //public int? CurrentRunningHours { get; set; }

        public decimal? CurrentRunningHours { get; set; } = 0;


        public decimal? CurrentLeadRunningHours { get; set; } = 0;

        public int? MaxRunningHours { get; set; }
        public int? MaxMonthsAllowed { get; set; }

        // public int? StartCounterHours { get; set; }
        public decimal? StartCounterHours { get; set; } = 0;
        public int? RopeTail { get; set; }
        public bool DeleteStatus { get; set; }

        public string Remarks { get; set; }
        public string attachment { get; set; }

        public string UniqueID { get; set; }
        // public DateTime? InspectionDueDate { get; set; }

        public string IsRopeOutOfS { get; set; }

        public string RopeType { get; set; }
        public string OutofServiceDate { get; set; }

        public string ManufacturerName { get; set; }

        public string Location { get; set; }

        public string AssignedWinch { get; set; }

        public int WinchId { get; set; }

        public int RopeId { get; set; }
        public string InstalledDate1 { get; set; }


        public string InspectionDueDate1 { get; set; }

        public string InspectionDueDate { get; set; }


        public string IsRopeInstalled { get; set; }

        public int RowId { get; set; }

        public decimal? CurrentLength { get; set; }

        public List<MooringRopeDetails> MooringLineDiscardList { get; set; }
        public List<MooringRopeDetails> MooringLineList { get; set; }
        public List<MooringRopeDetails> MooringLineList1 { get; set; }

      
        public List<SelectListItem> MooringRopeTypeLists { get; set; }
        public string MooringRopeTypeName { get; set; }
        public List<SelectListItem> ManufacturerTypeLists { get; set; }
        public string ManufacturerTypeName { get; set; }

        public List<SelectListItem> RopeTaggings { get; set; }
    }

    //public partial class MooringRopeDetails
    //{
    //    public MooringRopeDetails()
    //    {
    //        MooringRopeTypeLists = new List<SelectListItem>();
    //        ManufacturerTypeLists = new List<SelectListItem>();

    //        RopeTaggings = new List<SelectListItem>
    //        {
    //            new SelectListItem { Text = "No", Value = "No" },
    //            new SelectListItem { Text = "Yes", Value = "Yes" }
    //        };
    //    }
    //    public List<SelectListItem> MooringRopeTypeLists { get; set; }
    //    public string MooringRopeTypeName { get; set; }
    //    public List<SelectListItem> ManufacturerTypeLists { get; set; }
    //    public string ManufacturerTypeName { get; set; }

    //    public List<SelectListItem> RopeTaggings { get; set; }
    //}
}