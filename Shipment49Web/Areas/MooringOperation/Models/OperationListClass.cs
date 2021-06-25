using Reports;
using Shipment49Web.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Shipment49Web.Areas.MooringOperation.Models
{
    public class OperationListClass
    {
        public OperationListClass()
        {
            PortNameList = new List<SelectListItem>();
            OperationList = new List<MOperationBirthDetail>();
            FacilityNameList = new List<SelectListItem>() { new SelectListItem() { Text = "None Selected", Value = "None Selected" } };
        }

        public List<SelectListItem> PortNameList { get; set; }
        public List<SelectListItem> FacilityNameList { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public string PortName { get; set; }

        public string FacilityName { get; set; }
        public List<MOperationBirthDetail> OperationList { get; set; }
    }

    public class MODamageRopeClass
    {
        public MODamageRopeClass()
        {
            IncidentReports = new List<SelectListItem>
                {
                 new SelectListItem { Text = "Yes", Value = "Yes" },
                    new SelectListItem { Text = "No", Value = "No" }

                };
            IsCroppedList = new List<SelectListItem>
                {
                 //new SelectListItem { Text = "None Selected", Value = null },
                 new SelectListItem { Text = "No", Value = "No" },
                 new SelectListItem { Text = "Yes", Value = "Yes" }
                   
                };
            CroppedOutboardEndList  = new List<SelectListItem>
                {
                 new SelectListItem { Text = "None Selected", Value = null },
                 new SelectListItem { Text = "A", Value = "A" },
                 new SelectListItem { Text = "B", Value = "B" }

                };
            SplicedByList = new List<SelectListItem>
                {
                // new SelectListItem { Text = "None Selected", Value = null },
                 new SelectListItem { Text = "Shore assistance", Value = "Shore assistance" },
                    new SelectListItem { Text = "Onboard", Value = "Onboard" }

                };
            IncidentActionList = new List<SelectListItem>
                {
                    new SelectListItem { Text = "None Selected", Value = null },
                    new SelectListItem { Text = "Spliced", Value = "Spliced" },
                    new SelectListItem { Text = "Cropped", Value = "Cropped" },
                    new SelectListItem { Text = "Discarded", Value = "Discarded" },
                    new SelectListItem { Text = "End-to-end", Value = "End-to-end" }

                };
            SubDates = new List<SelectListItem>();
            RopeListUsingOp = new List<SelectListItem>();
            CroppingReasonList = new List<SelectListItem>();
        }
        public int WinchId { get; set; }
        public string DamageLocation { get; set; }
        public string DamageObserved { get; set; }
        public string DamageObserved1 { get; set; }
        public string DamageReason { get; set; }
        public int RopeId { get; set; }
        public DateTime? SplicedDate { get; set; }
        public string IncidentReport { get; set; }
        public string SplicedMethod { get; set; }
        public string ActionAfterDamage { get; set; }
        public string IncidentAction { get; set; }
        public int MOPId { get; set; }

        public string SplicingDoneBy { get; set; }

        public DateTime? CroppedDate { get; set; }
        public string CroppedOutboardEnd { get; set; }
        public decimal? LengthofCroppedRope { get; set; }

        public string RsCropping { get; set; }

        public List<string> ReasonofCropping { get; set; }


        public DateTime? OutofServiceDate { get; set; }
        public string ReasonOutofService { get; set; }

        public string otherReason { get; set; }
        public string MooringOperation { get; set; }
        public DateTime? EndtoEndDoneDate { get; set; }
        public bool? CurrentOutboadEndinUse { get; set; }

        public decimal? LengthofCroppedRope1 { get; set; }
        public DateTime? DiscaredDate { get; set; }



        public string IsCropped { get; set; }
        public int VesselID { get; set; }


        public List<SelectListItem> RopeListUsingOp { get; set; }
        public List<SelectListItem> SubDates { get; set; }
        public List<SelectListItem> IncidentReports { get; set; }
        public List<SelectListItem> IncidentActionList { get; set; }
        public List<DamageR> DamageReasons { get; set; }
        public List<DamageL> DamageLocations { get; set; }
        public List<DamageObserved> DamageObservedLists { get; set; }
        public List<SelectListItem> SplicedByList { get; set; }
        public List<SelectListItem> IsCroppedList { get; set; }
        public List<SelectListItem> CroppedOutboardEndList { get; set; }
        public List<OutofServiceR> OutofServiceReasonList { get; set; }

        public List<SelectListItem> CroppingReasonList { get; set; }
        // add
        public string outboard { get; set; }
        public string outboard1 { get; set; }


        //public int Id { get; set; }
        //public Nullable<int> RopeId { get; set; }
        //public string DamageObserved { get; set; }
        //public string IncidentReport { get; set; }
        //public string DamageLocation { get; set; }
        //public string DamageReason { get; set; }
        //public Nullable<int> MOPId { get; set; }
        //public Nullable<int> RopeTail { get; set; }
        //public Nullable<int> NotificationId { get; set; }
        //public Nullable<System.DateTime> CreatedDate { get; set; }
        //public string CreatedBy { get; set; }
        //public Nullable<System.DateTime> ModifiedDate { get; set; }
        //public string ModifiedBy { get; set; }
        //public Nullable<bool> IsActive { get; set; }
        //public string IncidentActlion { get; set; }
        //public Nullable<System.DateTime> DamageDate { get; set; }
        //public int VesselID { get; set; }
        //public Nullable<int> WinchId { get; set; }
    }


}