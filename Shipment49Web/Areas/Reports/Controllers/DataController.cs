using MenuLayer;
using Reports;
using Shipment49Web.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using System;
using System.IO;

namespace Shipment49Web.Areas.CrewReport.Controllers
{
    [Authorize]
    [ErrorClass]
    public class DataController : Controller
    {
        FilterModel filterModel;
        private readonly IMenuRepository sc;
        MorringOfficeEntities context = new MorringOfficeEntities();

        public DataController(IMenuRepository repo)
        {
            sc = repo;

            if (UserRole.username == null)
            {
                UserRole.username = string.Join("", Roles.GetRolesForUser());
            }

            ViewBag.GetMenu = UserRole.username == "Admin" ? sc.Menus.ToList() : sc.Menus.Where(x => x.Role == "User").ToList();
            ViewBag.GetSubMenu = UserRole.username == "Admin" ? sc.SubMenus.ToList() : sc.SubMenus.Where(x => x.Role == "User").ToList();

            using (ShipmentContaxt sc1 = new ShipmentContaxt())
            {
                var _smartmenu = sc1.SmartMenus.ToList().FirstOrDefault();
                if (_smartmenu != null)
                {
                    ViewBag.MenuContent = _smartmenu.SmartMenuContent;
                }
            }

            filterModel = new FilterModel
            {
                FleetNames = context.tblFleetNames.ToList(),
                FleetTypes = context.tblFleetTypes.ToList(),
                Vessels = context.VesselDetails.ToList(),
                TradePlatforms = context.tblCommons.Where(u => u.Type == (int)CommonType.TradePlatform).ToList()
            };
        }

        public ActionResult Index()
        {
            filterModel.ResultList = context.View_ReportInspectionStatus.ToList();
            return View(filterModel);
        }

        [HttpPost]
        public ActionResult Index(FormCollection collection)
        {
            return View();
        }

        public ActionResult Details()
        {
            var results = context.View_ReportInspectionStatusDetails.ToList();
            return View(results);
        }

        public ActionResult Vessels()
        {
            filterModel.DateFrom = DateTime.Today.AddYears(-1);
            filterModel.DateUpto = DateTime.Today;
            filterModel.Vessels = context.VesselDetails.ToList();
            return View(filterModel);
        }

        [HttpPost]
        public ActionResult Vessels(FilterModel model)
        {
            return View();
        }

        public ActionResult RopeInspReport(int id)
        {
            filterModel.DateFrom = DateTime.Today.AddYears(-1);
            filterModel.DateUpto = DateTime.Today;

            var inspections = context.MooringRopeInspections.Where(p => p.InspectDate >= filterModel.DateFrom && p.InspectDate <= filterModel.DateUpto);

            if (id > 0)
            {
                filterModel.VesselID = id;
                inspections = inspections.Where(p => p.VesselID == filterModel.VesselID);
            }

            filterModel.RopeInspectionList = inspections.Where(p => p.RopeTail == 0).ToList();
            filterModel.RopeTailInspectionList = inspections.Where(p => p.RopeTail == 1).ToList();

            return View(filterModel);
        }

        [HttpPost]
        public ActionResult RopeInspReport(FilterModel filter)
        {
            var inspections = context.MooringRopeInspections.Where(p => p.InspectDate >= filter.DateFrom && p.InspectDate <= filter.DateUpto);

            if (filter.VesselID > 0)
                inspections = inspections.Where(p => p.VesselID == filter.VesselID);

            filterModel.RopeInspectionList = inspections.Where(p => p.RopeTail == 0).ToList();
            filterModel.RopeTailInspectionList = inspections.Where(p => p.RopeTail == 1).ToList();

            return View(filterModel);
        }

        public JsonResult ViewInspDetails(int vesselid, int ropetail, int ropeid)
        {
            //filterModel.ViewInspectionDetails = context.ViewInspectionDetail(vesselid, ropetail, ropeid).ToList();
            //return Json(new { Result = true, Data = filterModel.ViewInspectionDetails }, JsonRequestBehavior.AllowGet);
            return new JsonResult();
        }

        public ActionResult LooseInspReport(int id)
        {
            filterModel.DateFrom = DateTime.Today.AddYears(-1);
            filterModel.DateUpto = DateTime.Today;

            var inspections = context.View_ReportLooseEquipmentInspection.Where(p => p.InspectDate >= filterModel.DateFrom && p.InspectDate <= filterModel.DateUpto);

            if (id > 0)
            {
                filterModel.VesselID = id;
                inspections = inspections.Where(p => p.VesselID == filterModel.VesselID);
            }

            filterModel.ListLooseEquipmentInspection = inspections.ToList();

            return View(filterModel);
        }

        [HttpPost]
        public ActionResult LooseInspReport(FilterModel filter)
        {
            var inspections = context.View_ReportLooseEquipmentInspection.Where(p => p.InspectDate >= filter.DateFrom && p.InspectDate <= filter.DateUpto);

            if (filter.VesselID > 0)
                inspections = inspections.Where(p => p.VesselID == filter.VesselID);

            filterModel.ListLooseEquipmentInspection = inspections.ToList();

            return View(filterModel);
        }


        public ActionResult OperationRecords(int id)
        {
            filterModel.DateFrom = DateTime.Today.AddYears(-1);
            filterModel.DateUpto = DateTime.Today;

            var oprecords = context.MOperationBirthDetails.AsQueryable();

            if (id > 0)
            {
                filterModel.VesselID = id;
                oprecords = oprecords.Where(p => p.VesselID == filterModel.VesselID);
            }

            filterModel.ListOperationRecords = oprecords.ToList();

            return View(filterModel);
        }

        [HttpPost]
        public ActionResult OperationRecords(FilterModel filter)
        {
            var oprecords = context.MOperationBirthDetails.AsQueryable();
            if (filter.VesselID > 0)
                oprecords = oprecords.Where(p => p.VesselID == filterModel.VesselID);

            filterModel.ListOperationRecords = oprecords.ToList();

            return View(filterModel);
        }

        public JsonResult ViewDamageRecords(int opid)
        {
            //filterModel.ListMooringDamageRecords = context.MooringDamageRecord(opid).ToList();
            //return Json(new { Result = true, Data = filterModel.ListMooringDamageRecords }, JsonRequestBehavior.AllowGet);
            return new JsonResult();
        }

        public JsonResult GetOperationDetails(int opid)
        {
            filterModel.MooringOperationDetails = context.MOperationBirthDetails.FirstOrDefault(p => p.OPId == opid);
            return Json(new { Result = true, Data = filterModel.MooringOperationDetails }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult MooringRopeDetail(int id)
        {
            filterModel.DateFrom = DateTime.Today.AddYears(-1);
            filterModel.DateUpto = DateTime.Today;

            var ropeDetails = context.View_MooringRopeDetails.Where(p => p.InstalledDate >= filterModel.DateFrom && p.InstalledDate <= filterModel.DateUpto).AsQueryable();

            if (id > 0)
            {
                filterModel.VesselID = id;
                ropeDetails = ropeDetails.Where(p => p.VesselID == filterModel.VesselID);
            }

            filterModel.ListMooringRopeDetails = ropeDetails.ToList();
            return View(filterModel);
        }

        [HttpPost]
        public ActionResult MooringRopeDetail(FormCollection collection)
        {
            return View();
        }

        public ActionResult RopeSummary(int rope, int tail, int vessel)
        {
            var ropeDetails = context.View_MooringRopeDetails.Where(p => p.RopeId == rope && p.RopeTail == tail).AsQueryable();

            if (vessel > 0)
            {
                filterModel.VesselID = vessel;
                ropeDetails = ropeDetails.Where(p => p.VesselID == filterModel.VesselID);
            }

            filterModel.ListMooringRopeDetails = ropeDetails.ToList();
            return View(filterModel);
        }

        public ActionResult AssignRopeToWinch(int? id, int? ropetailid)
        {
            //filterModel.ListAssignRopeToWinch = context.ViewAssignRopeToWinch(id, ropetailid).ToList();
            //return View(filterModel);
            return View();
        }

        [HttpPost]
        public ActionResult AssignRopeToWinch(FormCollection collection)
        {
            return View();
        }

        public ActionResult RopeSplicing(int? id, int? ropetailid)
        {
            //filterModel.ListRopeSplicing = context.ViewRopeSplicing(ropetailid, id).ToList();
            ////return View(filterModel);
            return View();
        }

        [HttpPost]
        public ActionResult RopeSplicing(FormCollection collection)
        {
            return View();
        }

        public ActionResult RopeCropping(int? id, int? ropetailid)
        {
            //filterModel.ListRopeCropping = context.ViewRopeCropping(ropetailid, id).ToList();
            //return View(filterModel);
            return View();
        }

        [HttpPost]
        public ActionResult RopeCropping(FormCollection collection)
        {
            return View();
        }

        public ActionResult DamageRope(int? id, int? ropetailid)
        {
            //filterModel.ListDamageRope = context.ViewDamageRope(ropetailid, id).ToList();
            //return View(filterModel);
            return View();
        }

        [HttpPost]
        public ActionResult DamageRope(FormCollection collection)
        {
            return View();
        }

        public ActionResult RopeDiscard(int? id, int? ropetailid)
        {
            //filterModel.ListRopeDiscardList = context.ViewRopeDiscardList(ropetailid, id).ToList();
            //return View(filterModel);
            return View();
        }

        [HttpPost]
        public ActionResult RopeDiscard(FormCollection collection)
        {
            return View();
        }
    }

    public class FilterModel
    {
        public FilterModel()
        {
            MooringOperationDetails = new MOperationBirthDetail();
            FleetNames = new List<tblFleetName>();
            FleetTypes = new List<tblFleetType>();
            TradePlatforms = new List<tblCommon>();
            Vessels = new List<VesselDetail>();
            ResultList = new List<View_ReportInspectionStatus>();
            ListMooringRopeDetails = new List<Reports.View_MooringRopeDetails>();
            ListAssignRopeToWinch = new List<ViewAssignRopeToWinch_Result>();
            ListRopeSplicing = new List<ViewRopeSplicing_Result>();
            ListRopeCropping = new List<ViewRopeCropping_Result>();
            ListDamageRope = new List<ViewDamageRope_Result>();
            ListRopeDiscardList = new List<ViewRopeDiscardList_Result>();
            RopeInspectionList = new List<Reports.MooringRopeInspection>();
            ViewInspectionDetails = new List<Reports.ViewInspectionDetail_Result>();
            ListLooseEquipmentInspection = new List<View_ReportLooseEquipmentInspection>();
            ListOperationRecords = new List<MOperationBirthDetail>();
            ListMooringDamageRecords = new List<MooringDamageRecord_Result>();
            RopeSummary = new List<View_MooringRopeDetails>();
        }

        public long FleetID { get; set; }
        public long FleetTypeID { get; set; }
        public long VesselID { get; set; }
        public long TradeID { get; set; }
        public long AgeRange { get; set; }
        public long RunningHours { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateUpto { get; set; }
        public List<tblFleetName> FleetNames { get; set; }
        public List<tblFleetType> FleetTypes { get; set; }
        public List<tblCommon> TradePlatforms { get; set; }
        public List<VesselDetail> Vessels { get; set; }
        public List<View_ReportInspectionStatus> ResultList { get; set; }
        public List<Reports.View_MooringRopeDetails> ListMooringRopeDetails { get; set; }
        public List<ViewAssignRopeToWinch_Result> ListAssignRopeToWinch { get; set; }
        public List<ViewRopeSplicing_Result> ListRopeSplicing { get; set; }
        public List<ViewRopeCropping_Result> ListRopeCropping { get; set; }
        public List<ViewDamageRope_Result> ListDamageRope { get; set; }
        public List<ViewRopeDiscardList_Result> ListRopeDiscardList { get; set; }
        public List<Reports.MooringRopeInspection> RopeInspectionList { get; set; }
        public List<Reports.MooringRopeInspection> RopeTailInspectionList { get; set; }
        public List<Reports.ViewInspectionDetail_Result> ViewInspectionDetails { get; set; }
        public List<View_ReportLooseEquipmentInspection> ListLooseEquipmentInspection { get; set; }
        public List<MOperationBirthDetail> ListOperationRecords { get; set; }
        public List<MooringDamageRecord_Result> ListMooringDamageRecords { get; set; }
        public MOperationBirthDetail MooringOperationDetails { get; set; }
        public List<View_MooringRopeDetails> RopeSummary { get; set; }
    }
}