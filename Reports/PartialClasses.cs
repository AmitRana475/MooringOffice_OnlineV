using MenuLayer;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using System.Web;

namespace Reports
{

    public partial class NotificationComment
    {
        public string DisplayDate { get; set; }
    }
    public partial class VesselDetail
    {
        public int VesselAge
        {
            get
            {
                int days = Convert.ToInt32((DateTime.Today - DateBuilt).Days) / 365;
                return days;
            }
        }
    }
    public partial class View_MooringRopeInspectionDetails
    {
        public string InspectDateString
        {
            get
            {
                if (InspectDate.HasValue)
                    return this.InspectDate.Value.ToString("dd-MMM-yyyy");
                else
                    return string.Empty;
            }
        }
    }

    public partial class SpUpcomingRopeDiscard_Result
    {
        public int id { get; set; }
        public string VesselName { get; set; }
        public int VesselID { get; set; }
        public Nullable<int> RopeId { get; set; }
        public Nullable<int> ManufacturerId { get; set; }
        public string CertificateNumber { get; set; }
        public string RopeType { get; set; }
        public string Name { get; set; }
        public Nullable<System.DateTime> InstalledDate { get; set; }
        public decimal CurrentRunningHours { get; set; }
        public string Duration { get; set; }
        public string Reason { get; set; }
        public decimal CostUsd { get; set; }
    }
    public partial class SpEndtoEndUpcomingNDue_Result
    {
        public string VesselName { get; set; }
        public string Name { get; set; }
        public string RopeType { get; set; }
        public string CertificateNumber { get; set; }
        public int VesselID { get; set; }
        public Nullable<int> EndToEndMonth { get; set; }
        public Nullable<System.DateTime> End2EndUpcoming { get; set; }
        public Nullable<System.DateTime> End2EndDue { get; set; }

    }

    public partial class SpWinchRotationUpcomingNDue_Result
    {
        public int VesselID { get; set; }
        public int WinchId { get; set; }
        public int RopeId { get; set; }
        public string VesselName { get; set; }
        public string ManufacturerName { get; set; }
        public string RopeType { get; set; }
        public string AssignedNumber { get; set; }

        public string Location { get; set; }
        public string lead { get; set; }
        public Nullable<System.DateTime> AssignedDate { get; set; }
        public decimal CurrentLeadRunningHours { get; set; }

        //NotMap
        public string StatusUpDue { get; set; }
        public Nullable<System.DateTime> WUpcoming { get; set; }
        public Nullable<System.DateTime> WOverDue { get; set; }

    }
    public class TailOption
    {
        public int TId { get; set; } //Ropetail id.
        public string Name { get; set; }  //Unique ID.
        public bool Selected { get; set; }


    }
    public class WinchCheckClass
    {
        public WinchCheckClass()
        {
            Tails = new List<TailOption>();
        }
        public int RowSr { get; set; }
        public int WinchsId { get; set; }

        public int RopeId { get; set; }
        public string WinchNo { get; set; }
        //public static bool Mark { get; set; } = RopeTailMark == true ? true : true;

        //public static bool RopeTailMark { get; set; } = Mark == true ? true : false;

        public string FDate_g { get; set; }
        public string FTime_g { get; set; }
        public string CDate_g { get; set; }
        public string CTime_g { get; set; }

        public bool Mark { get; set; }

        public bool RopeTailMark { get; set; }


        public string Lead { get; set; }
        public string LastCurrentLead { get; set; }
        public string Lead1 { get; set; }
        public string Location { get; set; }

        public string outboard1 { get; set; }  //OutboardEnd
        public string LastCurrentOutboardEnd { get; set; }
        public bool outboard { get; set; }

        public string VisibilityCheck { get; set; }

        public string IsEditable { get; set; }
        public string GridID { get; set; }

        public DateTime StartDatetime { get; set; }
        public DateTime EndDatetime { get; set; }
        public int SortingOrder { get; set; }
        public List<TailOption> Tails { get; set; }

    }

    public partial class SP_OperationWiseRopes
    {
        public int Id { get; set; }
        public Nullable<int> OperationID { get; set; }
        public string AssignedNumber { get; set; }
        public string Location { get; set; }
        public string CertificateNumber { get; set; }
        public string Lead { get; set; }
        public string Lead1 { get; set; }
        public Nullable<int> RopeTail { get; set; }

        public DateTime OPDateFrom { get; set; }
        public DateTime OPDateTo { get; set; }
        public decimal RunningHours { get; set; }
        public int VesselID { get; set; }


        public string FromDateTime { get; set; }
        public string ToDateTime { get; set; }

    }
    public partial class MOperationBirthDetail
    {

        public string Operation { get; set; }

        public List<SP_OperationWiseRopes> RopeUsedInOperation2 { get; set; }
        public List<SP_OperationWiseRopes> RopeTailsUsedInOperation2 { get; set; }
        public List<View_OperationWiseRopes> RopeUsedInOperation { get; set; }
        public List<View_OperationWiseRopes> RopeTailsUsedInOperation { get; set; }
        public string CastDateTimeString
        {
            get
            {
                if (CastDatetime.HasValue)
                    return this.CastDatetime.Value.ToString("dd-MMM-yyyy hh:mm:ss");
                else
                    return string.Empty;
            }
        }
        public string FastDateTimeString
        {
            get
            {
                if (FastDatetime.HasValue)
                    return this.FastDatetime.Value.ToString("dd-MMM-yyyy hh:mm:ss");
                else
                    return string.Empty;
            }
        }

        public MOperationBirthDetail()
        {
            RopeUsedInOperation2 = new List<SP_OperationWiseRopes>();
            RopeTailsUsedInOperation2 = new List<SP_OperationWiseRopes>();

            PortNameList = new List<SelectListItem>();
            FacilityNameList = new List<SelectListItem>();
            WinchList = new List<WinchCheckClass>();

            BerthTypeList = new List<SelectListItem>();
            MooringTypeList = new List<SelectListItem>();
            BerthSideList = new List<SelectListItem>();
            VesselConditionList = new List<SelectListItem>();
            ShipAccessList = new List<SelectListItem>();
            PositiveNegative = new List<SelectListItem>
            {
                new SelectListItem { Text = "Positive", Value = "True" },
                new SelectListItem { Text = "Negative", Value = "False" }
            };
            WindDirections = new List<SelectListItem>
            {
                new SelectListItem (),
                new SelectListItem { Text = "1", Value = "1" },
                new SelectListItem { Text = "2", Value = "2" },
                new SelectListItem { Text = "3", Value = "3" },
                new SelectListItem { Text = "4", Value = "4" },
                new SelectListItem { Text = "5", Value = "5" },
                new SelectListItem { Text = "6", Value = "6" },
                new SelectListItem { Text = "7", Value = "7" },

            };

            AllLeads = new List<SelectListItem>() { new SelectListItem { Text = "--Select--", Value = null } };
            using (MorringOfficeEntities context = new MorringOfficeEntities())
            {
                var leads = context.tblCommons.Where(x => x.Type == 6).OrderBy(p => p.Id).Distinct().ToList();
                foreach (var v in leads)
                    AllLeads.Add(new SelectListItem() { Text = v.Name, Value = v.Name.ToString() });
            }
            All_Lead1 = new List<SelectListItem>
            {
                new SelectListItem { Text = "--Select--", Value = null },
                new SelectListItem { Text = "Direct", Value = "Direct" },
                new SelectListItem { Text = "InDirect", Value = "InDirect" }
            };
            OutBoards = new List<SelectListItem>
            {
                 new SelectListItem { Text = "--Select--", Value = "--Select--" },
                new SelectListItem { Text = "A", Value = "A" },
                new SelectListItem { Text = "B", Value = "B" }
            };
        }

        public string FDate { get; set; }
        public string FTime { get; set; }
        public string CDate { get; set; }
        public string CTime { get; set; }
        public List<SelectListItem> PortNameList { get; set; }
        public List<SelectListItem> FacilityNameList { get; set; }
        public List<WinchCheckClass> WinchList { get; set; }

        public List<SelectListItem> BerthTypeList { get; set; }
        public List<SelectListItem> MooringTypeList { get; set; }
        public List<SelectListItem> BerthSideList { get; set; }
        public List<SelectListItem> VesselConditionList { get; set; }
        public List<SelectListItem> ShipAccessList { get; set; }
        public List<SelectListItem> PositiveNegative { get; set; }
        public List<SelectListItem> WindDirections { get; set; }

        public List<SelectListItem> AllLeads { get; set; }
        public List<SelectListItem> All_Lead1 { get; set; }
        public List<SelectListItem> OutBoards { get; set; }

        public string OtherPort { get; set; }
        public string OtherFacility { get; set; }
    }
    public partial class tblLooseEquipInspectionSetting
    {
        public tblLooseEquipInspectionSetting()
        {
            EquipmentTypeLists = new List<SelectListItem>();
        }
        public List<SelectListItem> EquipmentTypeLists { get; set; }
        public string EquipmentTypeName { get; set; }
    }
    public partial class tblRopeTailInspectionSetting
    {
        public tblRopeTailInspectionSetting()
        {
            MooringRopeTypeLists = new List<SelectListItem>();
            ManufacturerTypeLists = new List<SelectListItem>();
        }
        public List<SelectListItem> MooringRopeTypeLists { get; set; }
        public string MooringRopeTypeName { get; set; }
        public List<SelectListItem> ManufacturerTypeLists { get; set; }
        public string ManufacturerTypeName { get; set; }
    }

    public partial class tblWinchRotationSetting
    {
        public tblWinchRotationSetting()
        {
            FleetNameIDs = new List<int>();
            FleetTypeIDs = new List<int>();
            VesselIDs = new List<int>();
            MooringRopeTypeIDs = new List<int>();
            ManufacturerTypeIDs = new List<int>();

            FleetNameList = new List<SelectListItem>();
            FleetTypeList = new List<SelectListItem>();
            VesselList = new List<SelectListItem>();

            MooringRopeTypeLists = new List<SelectListItem>();
            ManufacturerTypeLists = new List<SelectListItem>();

            AllLeads = new List<SelectListItem>();

            using (MorringOfficeEntities context = new MorringOfficeEntities())
            {
                foreach (var v in context.tblCommons.Where(x => x.Type == 6).OrderBy(p => p.Id).Distinct().ToList())
                    AllLeads.Add(new SelectListItem() { Text = v.Name, Value = v.Name.ToString() });
            }
        }

        public List<SelectListItem> FleetNameList { get; set; }
        public List<int> FleetNameIDs { get; set; }

        public List<SelectListItem> FleetTypeList { get; set; }
        public List<int> FleetTypeIDs { get; set; }

        public List<SelectListItem> AllLeads { get; set; }


        public List<SelectListItem> VesselList { get; set; }
        public List<int> VesselIDs { get; set; }
        public string VesselName { get; set; }

        public List<SelectListItem> MooringRopeTypeLists { get; set; }
        public List<int> MooringRopeTypeIDs { get; set; }
        public string MooringRopeTypeName { get; set; }
        public List<SelectListItem> ManufacturerTypeLists { get; set; }
        public string ManufacturerTypeName { get; set; }
        public List<int> ManufacturerTypeIDs { get; set; }
    }

    public partial class tblRopeInspectionSetting
    {
        public tblRopeInspectionSetting()
        {
            MooringRopeTypeLists = new List<SelectListItem>();
            ManufacturerTypeLists = new List<SelectListItem>();
        }
        public List<SelectListItem> MooringRopeTypeLists { get; set; }
        public string MooringRopeTypeName { get; set; }
        public List<SelectListItem> ManufacturerTypeLists { get; set; }
        public string ManufacturerTypeName { get; set; }
    }
    [MetadataType(typeof(UserInfoMetaData))]
    public partial class UserInfo
    {
        Reports.MorringOfficeEntities context = new Reports.MorringOfficeEntities();

        public UserInfo()
        {
            FNameIDs = new List<int>();
            FTypeIDs = new List<int>();
            VesselIMOs = new List<int>();

            // type 7 in common table for Login >> Office or Vessel
            LoginTypes = context.tblCommons.Where(u => u.Type == 7).OrderBy(u => u.Name).ToList();
            VesselList = context.VesselDetails.OrderBy(u => u.VesselName).ToList();
            FleetNameList = context.tblCommons.Where(u => u.Type == (int)CommonType.FleetName).OrderBy(u => u.Name).ToList();
            FleetTypeList = context.tblCommons.Where(u => u.Type == (int)CommonType.FleetType).OrderBy(u => u.Name).ToList();

            UserRoles = new List<SelectListItem>
            {
                new SelectListItem { Text = "User", Value = "USER" },
                new SelectListItem { Text = "Admin", Value = "ADMIN" }
            };
        }

        public List<tblCommon> FleetNameList { get; set; }
        public List<int> FNameIDs { get; set; }
        public List<tblCommon> FleetTypeList { get; set; }
        public List<int> FTypeIDs { get; set; }
        public List<VesselDetail> VesselList { get; set; }
        public List<int> VesselIMOs { get; set; }
        public List<SelectListItem> UserRoles { get; set; }

        public List<tblCommon> LoginTypes { get; set; }
        public List<int> LTypeIDs { get; set; }
        public string ConfirmPassword { get; set; }
        public string EmailAddress { get; set; }
        public string Password { get; set; }
    }
    public class UserInfoMetaData
    {
        [Required(ErrorMessage = "This field is Required")]
        [Display(Name = "Confirm Password")]
        [System.ComponentModel.DataAnnotations.Compare("Password")]
        public string ConfirmPassword { get; set; }
    }



    [MetadataType(typeof(MooringInfoMetaData))]
    public partial class MooringRopeDetail
    {
        Reports.MorringOfficeEntities context = new Reports.MorringOfficeEntities();

        public MooringRopeDetail()
        {
            MooringRopeTypeLists = new List<SelectListItem>();
            ManufacturerTypeLists = new List<SelectListItem>();


            //MooringRopeDetail mropdetls = new MooringRopeDetail();

            foreach (var v in context.MooringRopeTypes.OrderBy(p => p.RopeType).ToList())
                MooringRopeTypeLists.Add(new SelectListItem() { Text = v.RopeType, Value = v.Id.ToString() });

            foreach (var v in context.tblCommons.Where(u => u.Type == 1).OrderBy(p => p.Name).ToList())
                ManufacturerTypeLists.Add(new SelectListItem() { Text = v.Name, Value = v.Id.ToString() });

            //RopeTaggings = mropdetls.RopeTaggings;

            // return View(mropdetls);

            IncidentReports = new List<SelectListItem>
                {
                 new SelectListItem { Text = "Yes", Value = "Yes" },
                    new SelectListItem { Text = "No", Value = "No" }

                };

            RopeTaggings = new List<SelectListItem>
                {
                    new SelectListItem { Text = "No", Value = "No" },
                    new SelectListItem { Text = "Yes", Value = "Yes" }
                };
        }

        public List<OutofServiceR> ReasonOutofServices { get; set; }
        public DateTime? InstalledDate1 { get; set; }
        public string IsRopeInstalled { get; set; }
        public List<MooringRopeDetail> MooringLineList { get; set; }
        public List<MooringRopeDetail> MooringLineList1 { get; set; }
        public List<SelectListItem> MooringRopeTypeLists { get; set; }
        public List<MooringRopeDetail> MooringLineDiscardLists { get; set; }
        public string MooringRopeTypeName { get; set; }
        public List<SelectListItem> ManufacturerTypeLists { get; set; }
        public string ManufacturerTypeName { get; set; }

        //public string IncidentReport { get; set; }
        public string DamageLocation { get; set; }
        public string DamageReason { get; set; }

        //public HttpPostedFileBaseModelBinder ImageFile { get; set; }
        public HttpPostedFileBase ImageFile { get; set; }

        public List<DamageObserved> DamageObservedLists { get; set; }

        public List<SelectListItem> IncidentReports { get; set; }
        public List<DamageR> DamageReasons { get; set; }
        public List<DamageL> DamageLocations { get; set; }

        public List<MOperationBirthDetail> MooringOperationsLists { get; set; }
        public List<SelectListItem> RopeTaggings { get; set; }

        public List<tblCommon> FleetNameList { get; set; }
        public List<int> FNameIDs { get; set; }
        public List<tblCommon> FleetTypeList { get; set; }
        public List<int> FTypeIDs { get; set; }
        public List<VesselDetail> VesselList { get; set; }
        public List<int> VesselIMOs { get; set; }
        public List<SelectListItem> UserRoles { get; set; }

        public List<tblCommon> LoginTypes { get; set; }
        public List<int> LTypeIDs { get; set; }

    }
    public class MooringInfoMetaData
    {
        //[Required(ErrorMessage = "This field is Required")]
        //[Display(Name = "Confirm Password")]
        //[System.ComponentModel.DataAnnotations.Compare("Password")]
        //public string ConfirmPassword { get; set; }
    }

    public partial class AssignRopeToWinch
    {
        Reports.MorringOfficeEntities context = new Reports.MorringOfficeEntities();

        public AssignRopeToWinch()
        {

        }
        public string AssignedNumber { get; set; }
        public string CertificateNumber { get; set; }
        public string UniqueId { get; set; }
        public string Status { get; set; }
        public string AssignedDate1 { get; set; }
        public DateTime? InstalledDate1 { get; set; }
        public string IsRopeInstalled { get; set; }
        public List<AssignRopeToWinch> AssignMooringLineList { get; set; }
        public List<AssignRopeToWinch> AssignMooringLineList1 { get; set; }
        public List<MooringRopeDetail> MooringLineLists { get; set; }
        public List<MooringWinchDetail> MooringWinchLists { get; set; }

    }

    public partial class RopeEndtoEnd2
    {
        Reports.MorringOfficeEntities context = new Reports.MorringOfficeEntities();

        public RopeEndtoEnd2()
        {

        }
        public bool OutboadEndinUse { get; set; }

        public string AssignedNumber { get; set; }
        public string CertificateNumber { get; set; }
        public string UniqueId { get; set; }
        public string Status { get; set; }


        public List<MooringRopeDetail> MooringLineLists { get; set; }
        public List<MooringWinchDetail> MooringWinchLists { get; set; }

    }


    public partial class RopeSplicingRecord
    {

       // Reports.MorringOfficeEntities context = new Reports.MorringOfficeEntities();

        public RopeSplicingRecord()
        {
            SplicingDoneBys = new List<SelectListItem>
                {
                    new SelectListItem { Text = "Shore Assistance", Value = "Shore Assistance" },
                    new SelectListItem { Text = "Onboard", Value = "Onboard" }
                };
            IsLineCroppeds = new List<SelectListItem>
                {
                    new SelectListItem { Text = "No", Value = "No" },
                    new SelectListItem { Text = "Yes", Value = "Yes" }
                };

            //ReasonofCroppings = new List<SelectListItem>
            //    {
            //        new SelectListItem { Text = "Cut Strands", Value = "Cut Strands" },
            //        new SelectListItem { Text = "Kinked", Value = "Kinked" },
            //         new SelectListItem { Text = "Abrasion", Value = "Abrasion" },
            //        new SelectListItem { Text = "Paint Damage", Value = "Paint Damage" },
            //          new SelectListItem { Text = "Deformation", Value = "Deformation" }
            //    };
        }
        public bool OutboadEndinUse { get; set; }

        public string AssignedNumber { get; set; }
        public string CertificateNumber { get; set; }
        public string UniqueId { get; set; }
        public string Status { get; set; }

        public List<CroppingR> ReasonofCroppings { get; set; }

        public string ReasonofCropping { get; set; }
        public decimal CroppingLength { get; set; }
        public string IsLineCropped { get; set; }
        public bool Outboard { get; set; }
        public List<SelectListItem> SplicingDoneBys { get; set; }
        public List<SelectListItem> IsLineCroppeds { get; set; }
        public List<MooringRopeDetail> MooringLineLists { get; set; }
        public List<MooringWinchDetail> MooringWinchLists { get; set; }
        public List<string> RsCropping { get; set; }
    }

    public partial class RopeCropping
    {

       
        public List<string> RsCropping { get; set; }
       
        public bool OutboadEndinUse { get; set; }

        public List<CroppingR> ReasonofCroppings { get; set; }

        public string AssignedNumber { get; set; }
        public string CertificateNumber { get; set; }
        public string UniqueId { get; set; }
        public string Status { get; set; }

        // public List<SelectListItem> ReasonofCroppings { get; set; }


        public decimal CroppingLength { get; set; }
        public string IsLineCropped { get; set; }
        public bool Outboard { get; set; }

        public List<MooringRopeDetail> MooringLineLists { get; set; }
        public List<MooringWinchDetail> MooringWinchLists { get; set; }

    }

    public partial class RopeDamageRecord
    {

        Reports.MorringOfficeEntities context = new Reports.MorringOfficeEntities();

        public RopeDamageRecord()
        {

            IncidentReports = new List<SelectListItem>
                {
                 new SelectListItem { Text = "Yes", Value = "Yes" },
                    new SelectListItem { Text = "No", Value = "No" }

                };
        }
        public bool OutboadEndinUse { get; set; }

        public string AssignedNumber { get; set; }
        public string CertificateNumber { get; set; }
        public string UniqueId { get; set; }
        public string Status { get; set; }

        public List<SelectListItem> ReasonofCroppings { get; set; }


        public decimal CroppingLength { get; set; }
        public string IsLineCropped { get; set; }
        public bool Outboard { get; set; }

        public List<SelectListItem> IncidentReports { get; set; }
        public List<SelectListItem> SubDates { get; set; }
        public List<DamageR> DamageReasons { get; set; }
        public List<DamageL> DamageLocations { get; set; }

        public List<DamageObserved> DamageObservedLists { get; set; }
        public List<MooringRopeDetail> MooringLineLists { get; set; }
        public List<MOperationBirthDetail> MooringOperationsLists { get; set; }
        public List<MooringWinchDetail> MooringWinchLists { get; set; }

    }

    public partial class RopeDisposal
    {

        Reports.MorringOfficeEntities context = new Reports.MorringOfficeEntities();

        public RopeDisposal()
        {
            PortNameList = new List<SelectListItem>();            
            FacilityNameList = new List<SelectListItem>() { new SelectListItem() { Text = "None Selected", Value = "None Selected" } };

        }

        public string AssignedNumber { get; set; }
        public string CertificateNumber { get; set; }
        public string UniqueId { get; set; }
        public string Status { get; set; }

        public List<SelectListItem> PortNameList { get; set; }
        public List<SelectListItem> FacilityNameList { get; set; }

        public List<MooringRopeDetail> MooringLineLists { get; set; }
        public List<MOperationBirthDetail> MooringOperationsLists { get; set; }
        public List<MooringWinchDetail> MooringWinchLists { get; set; }

    }

    public partial class MooringRopeInspection
    {
        Reports.MorringOfficeEntities context = new Reports.MorringOfficeEntities();
        public MooringRopeInspection()
        {
            CommonExtInt = new List<SelectListItem>
                {
                    new SelectListItem { Text = "1", Value = "1" },
                    new SelectListItem { Text = "2", Value = "2" },
                    new SelectListItem { Text = "3", Value = "3" },
                    new SelectListItem { Text = "4", Value = "4" },
                    new SelectListItem { Text = "5", Value = "5" },
                    new SelectListItem { Text = "6", Value = "6" },
                    new SelectListItem { Text = "7", Value = "7" },
                };
        }
        public string UniqueId { get; set; }
        public string Status { get; set; }
        // public List<string> YearList { get; set; }



        public string RopeType { get; set; }
        public string AssignedNumber { get; set; }
        public string CertificateNumber { get; set; }

        public string Location { get; set; }
        public string External { get; set; }
        public string External1 { get; set; }
        public string Internal { get; set; }
        public string Internal1 { get; set; }
        public string Photo11 { get; set; }
        public string Photo12 { get; set; }
        public List<SelectListItem> CommonExtInt { get; set; }
        public List<ChafeGuardCondition> ChafeGuard { get; set; }
        public int VesselId { get; set; }
        public List<MooringRopeInspection> AddInspectionList { get; set; }

    }

    public partial class MooringWinchDetail
    {
        public MooringWinchDetail()
        {
        }
        public List<MooringRopeDetail> LeadList { get; set; }

        public List<LeadListClass> Leads { get; set; }

    }

    public partial class ResidualLabTest
    {
        Reports.MorringOfficeEntities context = new Reports.MorringOfficeEntities();
        public ResidualLabTest()
        {

        }
        public List<MooringRopeDetail> MooringLineLists { get; set; }
        public List<MooringWinchDetail> MooringWinchLists { get; set; }

    }

    public class DamageR
    {
        public int Id { get; set; }
        public string DamageReasonL { get; set; }
    }
    public class DamageL
    {
        public int Id { get; set; }
        public string DamageLocationL { get; set; }
    }
    public class CroppingR
    {
        public int Id { get; set; }
        public string CroppingReason { get; set; }
    }
    public class OutofServiceR
    {
        public int Id { get; set; }
        public string Reason { get; set; }
    }

    public class ChafeGuardCondition
    {
        public int Id { get; set; }
        public string ChafeGuard { get; set; }
    }

    public class LeadListClass
    {
        public int Id { get; set; }
        public string Lead { get; set; }
    }


}