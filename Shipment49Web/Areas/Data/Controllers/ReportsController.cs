using ClosedXML.Excel;
using MenuLayer;
using Newtonsoft.Json;
using Reports;
using Shipment49Web.Common;
using Shipment49Web.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web.Mvc;
using static Shipment49Web.Common.RopeAnalysis;

namespace Shipment49Web.Areas.Data.Controllers
{
    [Authorize]
    [UserAuthenticationFilter]
    [ErrorClass]
    public class ReportsController : BaseController
    {
        FilterModel filterModel;
        ReportAnalysisFilterModel analysisFilterModel;

        MorringOfficeEntities context = new MorringOfficeEntities();
        SqlConnection con = ConnectionBulder.con;
        //public ReportsController(IMenuRepository repo)
        public ReportsController()
        {
            //sc = repo;

            //if (UserRole.username == null)
            //{
            //    UserRole.username = string.Join("", Roles.GetRolesForUser());
            //}

            //ViewBag.GetMenu = UserRole.username == "Admin" ? sc.Menus.ToList() : sc.Menus.Where(x => x.Role == "User").ToList();
            //ViewBag.GetSubMenu = UserRole.username == "Admin" ? sc.SubMenus.ToList() : sc.SubMenus.Where(x => x.Role == "User").ToList();

            using (ShipmentContaxt sc1 = new ShipmentContaxt())
            {
                var _smartmenu = sc1.SmartMenus.ToList().FirstOrDefault();
                if (_smartmenu != null)
                {
                    ViewBag.MenuContent = _smartmenu.SmartMenuContent;
                }
            }

            //filterModel = new FilterModel();
            //filterModel.FleetNames = base.PermittedFleetNames;
            //filterModel.FleetTypes = base.PermittedFleetTypes;

            //{
            //    FleetNames = context.tblFleetNames.ToList(),
            //    FleetTypes = context.tblFleetTypes.ToList(),
            //    Vessels = context.VesselDetails.ToList(),
            //    TradePlatforms = context.tblCommons.Where(u => u.Type == (int)CommonType.TradePlatform).ToList()
            //};

            //filterModel.FleetNames.ForEach(u => filterModel.FleetNameIDs.Add(u.Fid));
            //filterModel.FleetTypes.ForEach(u => filterModel.FleetTypeIDs.Add(u.Tid));
            //filterModel.TradePlatforms.ForEach(u => filterModel.TradeIDs.Add(u.Id));
            //filterModel.Vessels.ForEach(u => filterModel.VesselIDs.Add(u.ImoNo));

        }

        public void InitializeFilterModel()
        {
            filterModel = new FilterModel
            {
                FleetNames = PermittedFleetNames,
                FleetTypes = PermittedFleetTypes
            };
        }

        public ActionResult Index()
        {
            InitializeFilterModel();
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

        public static List<View_RopeDetails> RopeDetailsDownload { get; set; }
        public static List<View_RopeDetails> RopeTailDetailsDownload { get; set; }
        // public static List<View_MooringRopeDetails> RopeTailDetailsDownload { get; set; }

        public ActionResult Downloads()
        {

            bool val = false;
            if (RopeDetailsDownload != null)
            {
                DataTable RopeDetails = CommonMethods.LINQResultToDataTable(RopeDetailsDownload.Select(x => new { x.VesselName, x.CertificateNumber, x.RopeType, x.Manufacturer, CurrentRunningHours = x.CurrentRunningHours.ToString(), LastInspectDate = x.InspectDate.ToString(), Status = x.OutofServiceDate == null ? "In Service" : "Out of Service", Cost = string.Format("{0:F2}", x.CostUsd == null ? 0 : x.CostUsd) }));
                RopeDetails.TableName = "Line-Details";
                using (XLWorkbook wb = new XLWorkbook())
                {
                    var protectedsheet = wb.Worksheets.Add(RopeDetails);

                    var projection = protectedsheet.Protect("49WEB$TREET#");
                    projection.InsertColumns = true;
                    projection.InsertRows = true;

                    wb.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    wb.Style.Font.Bold = true;

                    DateTime today = DateTime.Today;
                    //string vsname = searchTerm.Replace(" ", "");
                    //string HeaderName = "Work-Ship_Export_" + vsname + "_" + today.ToString("dd-MMM-yyyy");

                    Response.Clear();
                    Response.BufferOutput = true;
                    Response.Charset = "";
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    Response.AddHeader("content-disposition", "attachment;filename=DMX7_Line-List_" + DateTime.Now.ToString("ddMMyyyy") + ".xlsx");

                    using (MemoryStream MyMemoryStream = new MemoryStream())
                    {
                        wb.SaveAs(MyMemoryStream);
                        MyMemoryStream.WriteTo(Response.OutputStream);
                        Response.End();
                    }

                    // Response.Clear();

                    Thread.Sleep(300);
                }
                val = true;
            }
            //return Json(new { Result = val }, JsonRequestBehavior.AllowGet);
            return new EmptyResult();
        }
        public ActionResult DownloadsTail()
        {

            bool val = false;
            if (RopeTailDetailsDownload != null)
            {
                DataTable RopeDetails = CommonMethods.LINQResultToDataTable(RopeTailDetailsDownload.Select(x => new { x.VesselName, x.CertificateNumber, x.RopeType, x.Manufacturer, CurrentRunningHours = x.CurrentRunningHours.ToString(), LastInspectDate = x.InspectDate.ToString(), Status = x.OutofServiceDate == null ? "In Service" : "Out of Service", Cost = string.Format("{0:F2}", x.CostUsd == null ? 0 : x.CostUsd) }));
                RopeDetails.TableName = "Line-Details";
                using (XLWorkbook wb = new XLWorkbook())
                {
                    var protectedsheet = wb.Worksheets.Add(RopeDetails);

                    var projection = protectedsheet.Protect("49WEB$TREET#");
                    projection.InsertColumns = true;
                    projection.InsertRows = true;

                    wb.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    wb.Style.Font.Bold = true;

                    DateTime today = DateTime.Today;
                    //string vsname = searchTerm.Replace(" ", "");
                    //string HeaderName = "Work-Ship_Export_" + vsname + "_" + today.ToString("dd-MMM-yyyy");

                    Response.Clear();
                    Response.BufferOutput = true;
                    Response.Charset = "";
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    Response.AddHeader("content-disposition", "attachment;filename=DMX7_Rope-Tail-List_" + DateTime.Now.ToString("ddMMyyyy") + ".xlsx");

                    using (MemoryStream MyMemoryStream = new MemoryStream())
                    {
                        wb.SaveAs(MyMemoryStream);
                        MyMemoryStream.WriteTo(Response.OutputStream);
                        Response.End();
                    }

                    // Response.Clear();

                    Thread.Sleep(300);
                }
                val = true;
            }
            //return Json(new { Result = val }, JsonRequestBehavior.AllowGet);
            return new EmptyResult();
        }

        public ActionResult DownloadRopeSummary(int rope, int tail, int vessel)
        {
            RopeSummaryReport ropeSummary = new RopeSummaryReport();

            int VesselID = Convert.ToInt32(vessel);

            string shipIMO = vessel.ToString();
            var vesselData = context.VesselDetails.Where(p => p.ImoNo == VesselID).Select(x => new { x.Id, x.VesselName, x.ImoNo, x.Flag, x.FleetName, x.FleetType }).ToList();

            if (vesselData.ToList().Count > 0)
            {
                shipIMO = vesselData.First().ImoNo.ToString();
                VesselID = vesselData.First().ImoNo;
            }

            DataTable vesselDetails = CommonMethods.LINQResultToDataTable(vesselData);
            vesselDetails.TableName = "VesselDetail";


            DataTable ropesplicedTable = new DataTable();
            DataTable endtoendTable = new DataTable();
            DataTable WinchRotation = new DataTable();
            DataTable ropedisposalTable = new DataTable();
            DataTable ropedamageTable = new DataTable();
            DataTable ropecroppingTable = new DataTable();
            DataTable ropedetailsTable = new DataTable();
            DataTable ropeinspectionTable = new DataTable();

            var ropespliced = context.vRopesSpliceds.Where(p => p.RopeId == rope && p.VesselID == vessel && p.RopeTail == tail).ToList();

            if (ropespliced.Count != 0)
            {
                ropesplicedTable = CommonMethods.LINQResultToDataTable(ropespliced);
                ropesplicedTable.TableName = "Spliced";
            }

            var endtoend = context.vRopesEndToEnds.Where(p => p.RopeId == rope && p.VesselID == vessel && p.RopeTail == tail).ToList();
            if (endtoend.Count != 0)
            {
                endtoendTable = CommonMethods.LINQResultToDataTable(endtoend);
                endtoendTable.TableName = "LineEndtoEnd";
            }

            var WRotation = CommonMethods.AllWinchRotationList.Where(p => p.RopeId == rope && p.VesselID == vessel).ToList();
            if (WRotation.Count != 0)
            {
                WinchRotation = CommonMethods.LINQResultToDataTable(WRotation);
                WinchRotation.TableName = "WinchRotation";
            }

            var ropedisposal = context.vRopesDisposeds.Where(p => p.RopeId == rope && p.VesselID == vessel && p.RopeTail == tail).ToList();
            if (ropedisposal.Count != 0)
            {
                ropedisposalTable = CommonMethods.LINQResultToDataTable(ropedisposal);
                ropedisposalTable.TableName = "Disposal";
            }

            // var ropedetails = context.vRopesDetails.Where(p => p.RopeId == rope && p.VesselID == vessel && p.RopeTail == tail).ToList();
            var ropedamage = context.vRopesDamageds.Where(p => p.RopeId == rope && p.VesselID == vessel && p.RopeTail == tail).ToList();
            if (ropedamage.Count != 0)
            {
                ropedamageTable = CommonMethods.LINQResultToDataTable(ropedamage);
                ropedamageTable.TableName = "Damaged";
            }

            var ropecropping = context.vRopesCroppeds.Where(p => p.RopeId == rope && p.VesselID == vessel && p.RopeTail == tail).ToList();
            if (ropecropping.Count != 0)
            {
                ropecroppingTable = CommonMethods.LINQResultToDataTable(ropecropping);
                ropecroppingTable.TableName = "Cropped";
            }

            var ropedetails = context.MooringRopeDetails.Where(p => p.RopeId == rope && p.VesselID == vessel && p.RopeTail == tail && p.OutofServiceDate != null).ToList();
            if (ropedetails.Count != 0)
            {
                ropedetailsTable = CommonMethods.LINQResultToDataTable(ropedetails);
                ropedetailsTable.TableName = "Line-Details";
            }

            var ropeinspection = context.MooringRopeInspections.OrderByDescending(u => u.InspectDate).Where(p => p.RopeId == rope && p.VesselID == vessel && p.RopeTail == tail).ToList();
            if (ropeinspection.Count != 0)
            {
                ropeinspectionTable = CommonMethods.LINQResultToDataTable(ropeinspection);
                ropeinspectionTable.TableName = "Line-Inspection";
            }

            using (XLWorkbook wb = new XLWorkbook())
            {
                var protectedsheet = wb.Worksheets.Add(vesselDetails);


                if (ropedetailsTable.Rows.Count > 0)
                    protectedsheet = wb.Worksheets.Add(ropedetailsTable);

                if (ropeinspectionTable.Rows.Count > 0)
                    protectedsheet = wb.Worksheets.Add(ropeinspectionTable);


                if (endtoendTable.Rows.Count > 0)
                    protectedsheet = wb.Worksheets.Add(endtoendTable);


                if (WinchRotation.Rows.Count > 0)
                    protectedsheet = wb.Worksheets.Add(WinchRotation);



                if (ropesplicedTable.Rows.Count > 0)
                    protectedsheet = wb.Worksheets.Add(ropesplicedTable);

                if (ropecroppingTable.Rows.Count > 0)
                    protectedsheet = wb.Worksheets.Add(ropecroppingTable);

                if (ropedamageTable.Rows.Count > 0)
                    protectedsheet = wb.Worksheets.Add(ropedamageTable);


                if (ropedisposalTable.Rows.Count > 0)
                    protectedsheet = wb.Worksheets.Add(ropedisposalTable);



                //if (RopeTypes.Rows.Count > 0)
                //    protectedsheet = wb.Worksheets.Add(RopeTypes);

                //if (revisionTable.Rows.Count > 0)
                //    protectedsheet = wb.Worksheets.Add(revisionTable);

                //if (docsPages.Rows.Count > 0)
                //    protectedsheet = wb.Worksheets.Add(docsPages);

                //if (notificationCommentSettings.Rows.Count > 0)
                //    protectedsheet = wb.Worksheets.Add(notificationCommentSettings);

                //if (smartMenu.Rows.Count > 0)
                //    protectedsheet = wb.Worksheets.Add(smartMenu);

                var projection = protectedsheet.Protect("49WEB$TREET#");
                projection.InsertColumns = true;
                projection.InsertRows = true;

                wb.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                wb.Style.Font.Bold = true;

                DateTime today = DateTime.Today;
                //string vsname = searchTerm.Replace(" ", "");
                //string HeaderName = "Work-Ship_Export_" + vsname + "_" + today.ToString("dd-MMM-yyyy");

                Response.Clear();
                Response.BufferOutput = true;
                Response.Charset = "";
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment;filename=DMX7_Export_" + vessel.ToString().Replace(" ", "") + ".xlsx");

                using (MemoryStream MyMemoryStream = new MemoryStream())
                {
                    wb.SaveAs(MyMemoryStream);
                    MyMemoryStream.WriteTo(Response.OutputStream);
                    Response.End();
                }

                Response.Clear();

                Thread.Sleep(300);
            }

            return new EmptyResult();
            //return View(ropeSummary);
        }

        public static List<DamagedObserved> DamagedObservedsList = new List<DamagedObserved>();
        private List<DamagedObserved> GetDamagedObserveds(DataTable dt)
        {
            List<DamagedObserved> list = new List<DamagedObserved>();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                var Damaged = new DamagedObserved()
                {
                    VesselID = (int)dt.Rows[i]["VesselID"],
                    VesselName = Convert.ToString(dt.Rows[i]["VesselName"]),
                    FleetNameID = (int)dt.Rows[i]["FleetNameID"],
                    FleetTypeID = (int)dt.Rows[i]["FleetTypeID"],
                    TradeAreaID = (int)dt.Rows[i]["TradeAreaID"],
                    RopeTail = (int)dt.Rows[i]["RopeTail"],
                    RopeId = (int)dt.Rows[i]["RopeId"],
                    ManufacturerId = (int)dt.Rows[i]["ManufacturerId"],
                    RopeTypeId = (int)dt.Rows[i]["RopeTypeId"],
                    CertificateNumber = Convert.ToString(dt.Rows[i]["CertificateNumber"]),

                    ManufacturerName = Convert.ToString(dt.Rows[i]["Name"]),
                    RopeType = Convert.ToString(dt.Rows[i]["RopeType"]),
                    DamageObserved = Convert.ToString(dt.Rows[i]["DamageObserved"]),
                    DamageReason = Convert.ToString(dt.Rows[i]["DamageReason"]),
                    IncidentActlion = Convert.ToString(dt.Rows[i]["IncidentActlion"]),
                    DamageDate = Convert.ToDateTime(dt.Rows[i]["DamageDate"]),
                };

                string servicedate = dt.Rows[i]["OutofServiceDate"].ToString();
                if (!string.IsNullOrEmpty(servicedate))
                    Damaged.OutofServiceDate = Convert.ToDateTime(dt.Rows[i]["OutofServiceDate"]);

                //string servicedate = dt.Rows[i]["DamageDate"].ToString();
                //if (!string.IsNullOrEmpty(servicedate))
                //    Damaged.OutofServiceDate = Convert.ToDateTime(dt.Rows[i]["OutofServiceDate"]);

                list.Add(Damaged);
            }
            return list;
        }
        public ActionResult DamageObservedDetail(int? pn)
        {
            ResidualLabFilter labFilter = new ResidualLabFilter
            {
                FleetNameList = base.PermittedFleetNames,
                FleetTypeList = base.PermittedFleetTypes,

                DateInstalledFrom = DateTime.Today.AddYears(-1),
                DateInstalledTo = DateTime.Today,

                TestDateFrom = DateTime.Today.AddYears(-1),
                TestDateTo = DateTime.Today,
                AgeRangeFrom = 0,
                AgeRangeTo = 50
            };

            DataTable data = CommonMethods.ExecStoredProceedureBetweenDates("SP_DamageObserved_Detail", labFilter.TestDateFrom, labFilter.TestDateTo);
            // var recordsFound = context.View_RopesDamagedCroppedSplicedUsed.OrderBy(u => u.VesselName).ThenBy(u => u.Manufacturer).ThenBy(u => u.RopeType).AsQueryable();
            if (pn == null)
                DamagedObservedsList = GetDamagedObserveds(data);  //context.View_RopesDamagedCroppedSplicedUsed2.OrderBy(u => u.VesselName).AsQueryable();

            int currPage = pn == null ? 1 : Convert.ToInt32(pn);

            TempData["CurrentPage"] = currPage;
            TempData["TotalRecords"] = DamagedObservedsList.Count();

            labFilter.DamagedObservedList = DamagedObservedsList.Skip((CommonMethods.PAGESIZE * currPage) - CommonMethods.PAGESIZE).Take(CommonMethods.PAGESIZE).ToList();

            //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%% Barchart Rope-Type wise %%%%%%%%%%%%%%%%%%%%%%%%%%%%%
            var RopeTypeArray = context.MooringRopeTypes.Select(x => x.RopeType).ToArray();
            List<AnomaliesChartData> listCombinedAnomalies = new List<AnomaliesChartData>();
            string Action = "";
            string Lables = "";
            foreach (var item in RopeTypeArray)
            {
                Lables = Lables + item + ",";

                Action = "Rope Type Wise";
                AnomaliesChartData chartdata = new AnomaliesChartData()
                {
                    labelName = item.ToString(),
                    value1 = DamagedObservedsList.Where(x => x.RopeType == item.ToString() && x.RopeTail == 0).Count(),//table.Rows.Count,
                    value2 = DamagedObservedsList.Where(x => x.RopeType == item.ToString() && x.RopeTail == 1).Count()
                };

                listCombinedAnomalies.Add(chartdata);

            }


            int a1 = listCombinedAnomalies.Max(x => x.value1);
            int b1 = listCombinedAnomalies.Max(x => x.value2);
            int geater = a1 > b1 == true ? a1 : b1;

            int mv = listCombinedAnomalies.Count > 0 ? geater : 0;
            int mv2 = mv > 0 ? Convert.ToInt32(mv / 20) * 20 + 40 : mv;


            var ChData = new GraphData() { chartId = mv2, data = RopeAnalysis.GetChartDataCom(listCombinedAnomalies, "Lines", "Tails") };
            ViewBag.RopeTailsDiscardedCharts_RTW = ChData;
            ViewBag.ChartLables_RTW = Lables;
            ViewBag.TitleHead_RTW = Action + " Damage Observed as of Date " + DateTime.Now.ToString("dd-MMM-yyyy");
            //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%% Barchart Rope-Type wise %%%%%%%%%%%%%%%%%%%%%%%%%%%%%

            
            List<int> piedata = new List<int>();
            var Labling = new PieChart();
            foreach (var item in Labling.labels)
            {
                int val = DamagedObservedsList.Where(x => x.DamageReason == item).Count();
                piedata.Add(val);
                
            }
           
            Labling.data = piedata;
            Labling.label = "Reason of Damage Observed as of Date " + DateTime.Now.ToString("dd-MMM-yyyy");
            string getpiedata =  JsonConvert.SerializeObject(Labling);
            var ChData2 = new GraphData() { chartId = 1, data = getpiedata };
                       
            ViewBag.PieChartData = ChData2;
            return View(labFilter);
        }

        [HttpPost]
        public ActionResult DamageObservedDetail(ResidualLabFilter Filterlist)
        {
            int pn = 1;

            Filterlist.FleetNameList = base.PermittedFleetNames;
            Filterlist.FleetTypeList = base.PermittedFleetTypes;
            Filterlist.VesselList = base.PermittedVessels;
            DataTable data = CommonMethods.ExecStoredProceedureBetweenDates("SP_DamageObserved_Detail", Filterlist.TestDateFrom, Filterlist.TestDateTo);

            // var recordsFound = context.View_RopesDamagedCroppedSplicedUsed.OrderBy(u => u.VesselName).ThenBy(u => u.Manufacturer).ThenBy(u => u.RopeType).AsQueryable();
            DamagedObservedsList = GetDamagedObserveds(data);

            if (Filterlist.StatusName == "DISCARDED")
                DamagedObservedsList = DamagedObservedsList.Where(x => x.OutofServiceDate != null).ToList();
            else if (Filterlist.StatusName == "INSERVICE")
                DamagedObservedsList = DamagedObservedsList.Where(x => x.OutofServiceDate == null).ToList();

            if (Filterlist.VesselIDs?.Count > 0)
            {
                DamagedObservedsList = DamagedObservedsList.Where(p => Filterlist.VesselIDs.Contains(p.VesselID)).ToList();

            }

            if (Filterlist.ManufacturerIDs?.Count > 0)
            {
                DamagedObservedsList = DamagedObservedsList.Where(p => Filterlist.ManufacturerIDs.Contains(p.ManufacturerId)).ToList();

            }

            if (Filterlist.RopeTypeIDs?.Count > 0)
            {
                DamagedObservedsList = DamagedObservedsList.Where(p => Filterlist.RopeTypeIDs.Contains(p.RopeTypeId)).ToList();

            }

            //if (Filterlist.TradeIDs?.Count > 0)
            //{
            //    vesselInfo = vesselInfo.Where(p => Filterlist.TradeIDs.Contains(p.TradeAreaID)).AsQueryable();

            //}

            int currPage = pn == null ? 1 : Convert.ToInt32(pn);

            TempData["CurrentPage"] = currPage;
            TempData["TotalRecords"] = DamagedObservedsList.Count();

            Filterlist.DamagedObservedList = DamagedObservedsList.Skip((CommonMethods.PAGESIZE * currPage) - CommonMethods.PAGESIZE).Take(CommonMethods.PAGESIZE).ToList();

            //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%% Barchart Rope-Type wise %%%%%%%%%%%%%%%%%%%%%%%%%%%%%
            var RopeTypeArray = context.MooringRopeTypes.Select(x => x.RopeType).ToArray();
            List<AnomaliesChartData> listCombinedAnomalies = new List<AnomaliesChartData>();
            string Action = "";
            string Lables = "";
            foreach (var item in RopeTypeArray)
            {
                Lables = Lables + item + ",";

                Action = "Rope Type Wise";
                AnomaliesChartData chartdata = new AnomaliesChartData()
                {
                    labelName = item.ToString(),
                    value1 = DamagedObservedsList.Where(x => x.RopeType == item.ToString() && x.RopeTail == 0).Count(),//table.Rows.Count,
                    value2 = DamagedObservedsList.Where(x => x.RopeType == item.ToString() && x.RopeTail == 1).Count()
                };

                listCombinedAnomalies.Add(chartdata);

            }

            int a1 = listCombinedAnomalies.Max(x => x.value1);
            int b1 = listCombinedAnomalies.Max(x => x.value2);
            int geater = a1 > b1 == true ? a1 : b1;

            int mv = listCombinedAnomalies.Count > 0 ? geater : 0;
            int mv2 = mv > 0 ? Convert.ToInt32(mv / 20) * 20 + 40 : mv;

            var ChData = new GraphData() { chartId = mv2, data = RopeAnalysis.GetChartDataCom(listCombinedAnomalies, "Lines", "Tails") };
            ViewBag.RopeTailsDiscardedCharts_RTW = ChData;
            ViewBag.ChartLables_RTW = Lables;
            ViewBag.TitleHead_RTW = Action + " Damage Observed as of Date " + DateTime.Now.ToString("dd-MMM-yyyy");
            //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%% Barchart Rope-Type wise %%%%%%%%%%%%%%%%%%%%%%%%%%%%%

            List<int> piedata = new List<int>();
            var Labling = new PieChart();
            foreach (var item in Labling.labels)
            {
                int val = DamagedObservedsList.Where(x => x.DamageReason == item).Count();
                piedata.Add(val);

            }

            Labling.data = piedata;
            Labling.label = "Reason of Damage Observed as of Date " + DateTime.Now.ToString("dd-MMM-yyyy");
            string getpiedata = JsonConvert.SerializeObject(Labling);
            var ChData2 = new GraphData() { chartId = 1, data = getpiedata };

            ViewBag.PieChartData = ChData2;

            return View(Filterlist);
        }

        public ActionResult RopeDetail(int? vid, int? cp)
        {
            RopeDetailsReport ropeDetailsReport = new RopeDetailsReport
            {
                //DateFrom = DateTime.Today.AddYears(-5),
                //DateUpto = DateTime.Today,
                RunningHoursFrom = 0,
                RunningHoursTo = 100000,
            };

            //var ropeDetails = context.View_RopeDetails.Where(p => p.RopeTail == 0 && p.ReceivedDate >= ropeDetailsReport.DateFrom &&
            //    p.ReceivedDate <= ropeDetailsReport.DateUpto).OrderByDescending(u => u.ReceivedDate).AsQueryable();

            var ropeDetails = context.View_RopeDetails.Where(p => p.RopeTail == 0);

            ropeDetailsReport.VesselIDs = (List<int>)TempData["SelectedVessels"];
            if (ropeDetailsReport.VesselIDs?.Count > 0)
            {
                ropeDetails = ropeDetails.Where(p => ropeDetailsReport.VesselIDs.Contains(p.VesselID)).AsQueryable();
                TempData["SelectedVessels"] = ropeDetailsReport.VesselIDs;
            }

            if (vid > 0)
            {
                if (ropeDetailsReport.VesselIDs == null)
                    ropeDetailsReport.VesselIDs = new List<int>();

                TempData["VesselID"] = vid;
                ropeDetailsReport.VesselIDs.Add(Convert.ToInt32(vid));
                ropeDetails = ropeDetails.Where(p => ropeDetailsReport.VesselIDs.Contains(p.VesselID)).AsQueryable();
            }



            ropeDetailsReport.ManufacturerIDs = (List<int>)TempData["SelectedManufacturers"];
            if (ropeDetailsReport.ManufacturerIDs?.Count > 0)
            {
                ropeDetails = ropeDetails.Where(p => ropeDetailsReport.ManufacturerIDs.Contains(p.ManufacturerId ?? 0)).AsQueryable();
                TempData["SelectedManufacturers"] = ropeDetailsReport.ManufacturerIDs;
            }

            ropeDetailsReport.RopeTypeIDs = (List<int>)TempData["SelectedRopeTypes"];
            if (ropeDetailsReport.RopeTypeIDs?.Count > 0)
            {
                ropeDetails = ropeDetails.Where(p => ropeDetailsReport.RopeTypeIDs.Contains(p.RopeTypeId ?? 0)).AsQueryable();
                TempData["SelectedRopeTypes"] = ropeDetailsReport.RopeTypeIDs;
            }

            if (ropeDetailsReport.StatusIDs != null)
            {
                if (ropeDetailsReport.StatusIDs.Contains(1))
                    ropeDetails = ropeDetails.Where(p => p.OutofServiceDate != null).AsQueryable();
                else if (ropeDetailsReport.StatusIDs.Contains(2))
                    ropeDetails = ropeDetails.Where(p => p.OutofServiceDate == null).AsQueryable();
            }

            //ropeDetailsReport.InspectionRatingIDs = (List<int>)TempData["SelectedInspectionRatings"];
            //if (ropeDetailsReport.InspectionRatingIDs?.Count > 0)
            //{
            //    ropeDetails = ropeDetails.Where(p => ropeDetailsReport.InspectionRatingIDs.Contains(p.AverageRating_A ?? 0) || ropeDetailsReport.InspectionRatingIDs.Contains(p.AverageRating_B ?? 0)).AsQueryable();
            //    TempData["SelectedInspectionRatings"] = ropeDetailsReport.InspectionRatingIDs;
            //}
            ropeDetails = ropeDetails.Where(p => p.CurrentRunningHours >= ropeDetailsReport.RunningHoursFrom).AsQueryable();
            ropeDetails = ropeDetails.Where(p => p.CurrentRunningHours <= ropeDetailsReport.RunningHoursTo).AsQueryable();

            TempData["RunningHoursFrom"] = ropeDetailsReport.RunningHoursFrom;
            TempData["RunningHoursTo"] = ropeDetailsReport.RunningHoursTo;

            RopeDetailsDownload = ropeDetails.OrderBy(u => new { u.OutofServiceDate, u.RopeType }).ToList();
            int currPage = cp == null ? 1 : Convert.ToInt32(cp);

            TempData["CurrentPage"] = currPage;
            TempData["TotalRecords"] = ropeDetails.Count();

            ropeDetailsReport.ListRopeDetails = ropeDetails.OrderBy(u => new { u.OutofServiceDate, u.RopeType }).
                 Skip((CommonMethods.PAGESIZE * currPage) - CommonMethods.PAGESIZE).Take(CommonMethods.PAGESIZE).ToList();

            ropeDetailsReport.Vessels = PermittedVessels; // context.VesselDetails.OrderBy(u => u.VesselName).ToList();

            return View(ropeDetailsReport);
        }

        [HttpPost]
        public ActionResult RopeDetail(RopeDetailsReport ropeDetailsReport)
        {
            var ropeDetails = context.View_RopeDetails.Where(p => p.RopeTail == 0);

            if (ropeDetailsReport.DateFrom != null)
                ropeDetails = ropeDetails.Where(p => p.ReceivedDate >= ropeDetailsReport.DateFrom);

            if (ropeDetailsReport.DateUpto != null)
                ropeDetails = ropeDetails.Where(p => p.ReceivedDate <= ropeDetailsReport.DateUpto);

            if (ropeDetailsReport.StatusIDs != null)
            {
                if (ropeDetailsReport.StatusIDs.Contains(1))
                    ropeDetails = ropeDetails.Where(p => p.OutofServiceDate != null).AsQueryable();
                else if (ropeDetailsReport.StatusIDs.Contains(2))
                    ropeDetails = ropeDetails.Where(p => p.OutofServiceDate == null).AsQueryable();
            }

            if (ropeDetailsReport.VesselIDs?.Count > 0)
            {
                ropeDetails = ropeDetails.Where(p => ropeDetailsReport.VesselIDs.Contains(p.VesselID)).AsQueryable();
                TempData["SelectedVessels"] = ropeDetailsReport.VesselIDs;
            }

            if (ropeDetailsReport.ManufacturerIDs?.Count > 0)
            {
                ropeDetails = ropeDetails.Where(p => ropeDetailsReport.ManufacturerIDs.Contains(p.ManufacturerId ?? 0)).AsQueryable();
                TempData["SelectedManufacturers"] = ropeDetailsReport.ManufacturerIDs;
            }

            if (ropeDetailsReport.RopeTypeIDs?.Count > 0)
            {
                ropeDetails = ropeDetails.Where(p => ropeDetailsReport.RopeTypeIDs.Contains(p.RopeTypeId ?? 0)).AsQueryable();
                TempData["SelectedRopeTypes"] = ropeDetailsReport.RopeTypeIDs;
            }

            //if (ropeDetailsReport.InspectionRatingIDs?.Count > 0)
            //{
            //    ropeDetails = ropeDetails.Where(p => ropeDetailsReport.InspectionRatingIDs.Contains(p.AverageRating_A ?? 0) || ropeDetailsReport.InspectionRatingIDs.Contains(p.AverageRating_B ?? 0)).AsQueryable();
            //    TempData["SelectedInspectionRatings"] = ropeDetailsReport.InspectionRatingIDs;
            //}

            //ropeDetails = ropeDetails.Where(p => p.CurrentRunningHours >= ropeDetailsReport.RunningHoursFrom).AsQueryable();
            //ropeDetails = ropeDetails.Where(p => p.CurrentRunningHours <= ropeDetailsReport.RunningHoursTo).AsQueryable();

            ropeDetails = ropeDetails.Where(p => p.CurrentRunningHours >= ropeDetailsReport.RunningHoursFrom).AsQueryable();
            ropeDetails = ropeDetails.Where(p => p.CurrentRunningHours <= ropeDetailsReport.RunningHoursTo).AsQueryable();

            TempData["RunningHoursFrom"] = ropeDetailsReport.RunningHoursFrom;
            TempData["RunningHoursTo"] = ropeDetailsReport.RunningHoursTo;
            RopeDetailsDownload = ropeDetails.OrderBy(u => new { u.OutofServiceDate, u.RopeType }).ToList();
            int currPage = 1;
            TempData["CurrentPage"] = currPage;
            TempData["TotalRecords"] = ropeDetails.Count();
            TempData["VesselID"] = ropeDetailsReport.VesselIDs;

            ropeDetailsReport.ListRopeDetails = ropeDetails.OrderBy(u => new { u.OutofServiceDate, u.RopeType }).
                 Skip((CommonMethods.PAGESIZE * currPage) - CommonMethods.PAGESIZE).Take(CommonMethods.PAGESIZE).ToList();

            ropeDetailsReport.FleetNames = PermittedFleetNames;
            ropeDetailsReport.FleetTypes = PermittedFleetTypes;

            ropeDetailsReport.Vessels = PermittedVessels; // context.VesselDetails.Where(p => ropeDetailsReport.FleetTypeIDs.Contains(p.FleetTypeID) && ropeDetailsReport.FleetNameIDs.Contains(p.FleetNameID) && ropeDetailsReport.TradeIDs.Contains(p.TradeAreaID)).ToList();

            return View(ropeDetailsReport);
        }

        public static int PagePost { get; set; } = 1;
        public ActionResult RopeTailDetail(int? vid, int? cp)
        {
            RopeDetailsReport ropeDetailsReport = new RopeDetailsReport
            {
                //DateFrom = DateTime.Today.AddYears(-5),
                //DateUpto = DateTime.Today,
                RunningHoursFrom = 0,
                RunningHoursTo = 100000,
                FleetNames = PermittedFleetNames,
                FleetTypes = PermittedFleetTypes,
            };

            if (vid != null)
                PagePost = 1;
            //var ropeDetails = context.View_MooringRopeDetails.Where(p => p.RopeTail == 1 && p.ReceivedDate >= ropeDetailsReport.DateFrom && p.ReceivedDate <= ropeDetailsReport.DateUpto).
            //    OrderByDescending(u => u.ReceivedDate).AsQueryable();  

            // var ropeDetails = context.View_MooringRopeDetails.Where(p => p.RopeTail == 1).OrderByDescending(u => u.ReceivedDate).AsQueryable();
            var ropeDetails = context.View_RopeDetails.Where(p => p.RopeTail == 1).OrderByDescending(u => u.ReceivedDate).AsQueryable();

            if (vid != null && PagePost == 1)
            {
                if (ropeDetailsReport.VesselIDs == null)
                    ropeDetailsReport.VesselIDs = new List<int>();

                TempData["VesselID"] = vid;
                ropeDetailsReport.VesselIDs.Add(Convert.ToInt32(vid));
                ropeDetails = ropeDetails.Where(p => ropeDetailsReport.VesselIDs.Contains(p.VesselID));


                ropeDetailsReport.ManufacturerIDs = (List<int>)TempData["SelectedManufacturers"];
                if (ropeDetailsReport.ManufacturerIDs?.Count > 0)
                {
                    ropeDetails = ropeDetails.Where(p => ropeDetailsReport.ManufacturerIDs.Contains(p.ManufacturerId ?? 0)).AsQueryable();
                }

                ropeDetailsReport.RopeTypeIDs = (List<int>)TempData["SelectedRopeTypes"];
                if (ropeDetailsReport.RopeTypeIDs?.Count > 0)
                {
                    ropeDetails = ropeDetails.Where(p => ropeDetailsReport.RopeTypeIDs.Contains(p.RopeTypeId ?? 0)).AsQueryable();
                }

                //ropeDetailsReport.InspectionRatingIDs = (List<int>)TempData["SelectedInspectionRatings"];
                //if (ropeDetailsReport.InspectionRatingIDs?.Count > 0)
                //{
                //    ropeDetails = ropeDetails.Where(p => ropeDetailsReport.InspectionRatingIDs.Contains(p.AverageRating_A ?? 0) || ropeDetailsReport.InspectionRatingIDs.Contains(p.AverageRating_B ?? 0)).AsQueryable();
                //}

                // ropeDetailsReport.RunningHoursFrom = TempData["RunningHoursFrom"] == null ? 0 : (int)TempData["RunningHoursFrom"];
                // ropeDetailsReport.RunningHoursTo = TempData["RunningHoursTo"] == null ? 100000 : (int)TempData["RunningHoursTo"];

                ropeDetails = ropeDetails.Where(p => p.CurrentRunningHours >= ropeDetailsReport.RunningHoursFrom).AsQueryable();
                ropeDetails = ropeDetails.Where(p => p.CurrentRunningHours <= ropeDetailsReport.RunningHoursTo).AsQueryable();

                ropeDetailsReport.FleetTypeIDs = TempData["SelectedFleetTypes"] == null ? new List<int>() : (List<int>)TempData["SelectedFleetTypes"];
                ropeDetailsReport.FleetNameIDs = TempData["SelectedFleetNames"] == null ? new List<int>() : (List<int>)TempData["SelectedFleetNames"];
                ropeDetailsReport.TradeIDs = TempData["SelectedTrades"] == null ? new List<int>() : (List<int>)TempData["SelectedTrades"];

                //ropeDetailsReport.VesselIDs = (List<int>)TempData["SelectedVessels"];

                ropeDetailsReport.Vessels = PermittedVessels; // context.VesselDetails.Where(p => ropeDetailsReport.FleetTypeIDs.Contains(p.FleetTypeID) && ropeDetailsReport.FleetNameIDs.Contains(p.FleetNameID) && ropeDetailsReport.TradeIDs.Contains(p.TradeAreaID)).ToList();
                RopeTailDetailsDownload = ropeDetails.OrderBy(u => u.RopeType).ToList();
            }
            else
            {
                PagePost = 2;
            }

            //int currPage = 1;
            //TempData["CurrentPage"] = currPage;
            //TempData["TotalRecords"] = ropeDetails.Count();
            //TempData["VesselID"] = ropeDetailsReport.VesselIDs;

            //ropeDetailsReport.ListMooringRopeDetails = ropeDetails.Skip((CommonMethods.PAGESIZE * currPage) - CommonMethods.PAGESIZE).Take(CommonMethods.PAGESIZE).ToList();
            //RopeTailDetailsDownload = ropeDetails.OrderBy(u => u.RopeType).ToList();
            int currPage = cp == null ? 1 : Convert.ToInt32(cp);

            TempData["CurrentPage"] = currPage;
            TempData["TotalRecords"] = RopeTailDetailsDownload.Count();

            ropeDetailsReport.ListRopeDetails = RopeTailDetailsDownload.OrderBy(u => u.RopeType).
                 Skip((CommonMethods.PAGESIZE * currPage) - CommonMethods.PAGESIZE).Take(CommonMethods.PAGESIZE).ToList();


            return View(ropeDetailsReport);
        }

        [HttpPost]
        public ActionResult RopeTailDetail(RopeDetailsReport ropeDetailsReport)//MooringRopeDetailsReport ropeDetailsReport)
        {
            //var ropeDetails = context.View_MooringRopeDetails.Where(p => p.RopeTail == 1 && p.ReceivedDate >= ropeDetailsReport.DateFrom &&
            //    p.ReceivedDate <= ropeDetailsReport.DateUpto).OrderByDescending(u => u.ReceivedDate).AsQueryable();
            PagePost = 2;
            var ropeDetails = context.View_RopeDetails.Where(p => p.RopeTail == 1).OrderByDescending(u => u.ReceivedDate).AsQueryable();

            if (ropeDetailsReport.DateFrom != null)
                ropeDetails = ropeDetails.Where(p => p.ReceivedDate >= ropeDetailsReport.DateFrom);

            if (ropeDetailsReport.DateFrom != null)
                ropeDetails = ropeDetails.Where(p => p.ReceivedDate <= ropeDetailsReport.DateUpto);

            if (ropeDetailsReport.VesselIDs?.Count > 0)
            {
                ropeDetails = ropeDetails.Where(p => ropeDetailsReport.VesselIDs.Contains(p.VesselID)).AsQueryable();
                TempData["SelectedVessels"] = ropeDetailsReport.VesselIDs;
            }

            if (ropeDetailsReport.StatusIDs != null)
            {
                if (ropeDetailsReport.StatusIDs.Contains(1))
                    ropeDetails = ropeDetails.Where(p => p.OutofServiceDate != null).AsQueryable();
                if (ropeDetailsReport.StatusIDs.Contains(2))
                    ropeDetails = ropeDetails.Where(p => p.OutofServiceDate == null).AsQueryable();
            }

            if (ropeDetailsReport.ManufacturerIDs?.Count > 0)
            {
                ropeDetails = ropeDetails.Where(p => ropeDetailsReport.ManufacturerIDs.Contains(p.ManufacturerId ?? 0)).AsQueryable();
                TempData["SelectedManufacturers"] = ropeDetailsReport.ManufacturerIDs;
            }

            if (ropeDetailsReport.RopeTypeIDs?.Count > 0)
            {
                ropeDetails = ropeDetails.Where(p => ropeDetailsReport.RopeTypeIDs.Contains(p.RopeTypeId ?? 0)).AsQueryable();
                TempData["SelectedRopeTypes"] = ropeDetailsReport.RopeTypeIDs;
            }

            //if (ropeDetailsReport.InspectionRatingIDs?.Count > 0)
            //{
            //    ropeDetails = ropeDetails.Where(p => ropeDetailsReport.InspectionRatingIDs.Contains(p.AverageRating_A ?? 0) || ropeDetailsReport.InspectionRatingIDs.Contains(p.AverageRating_B ?? 0)).AsQueryable();
            //    TempData["SelectedInspectionRatings"] = ropeDetailsReport.InspectionRatingIDs;
            //}

            ropeDetails = ropeDetails.Where(p => p.CurrentRunningHours >= ropeDetailsReport.RunningHoursFrom).AsQueryable();
            ropeDetails = ropeDetails.Where(p => p.CurrentRunningHours <= ropeDetailsReport.RunningHoursTo).AsQueryable();

            // TempData["RunningHoursFrom"] = ropeDetailsReport.RunningHoursFrom;
            // TempData["RunningHoursTo"] = ropeDetailsReport.RunningHoursTo;
            RopeTailDetailsDownload = ropeDetails.OrderBy(u => u.RopeType).ToList();
            int currPage = 1;
            TempData["CurrentPage"] = currPage;
            TempData["TotalRecords"] = ropeDetails.Count();
            TempData["VesselID"] = ropeDetailsReport.VesselIDs;

            // ropeDetailsReport.ListMooringRopeDetails = ropeDetails.Skip((CommonMethods.PAGESIZE * currPage) - CommonMethods.PAGESIZE).Take(CommonMethods.PAGESIZE).ToList();
            ropeDetailsReport.ListRopeDetails = ropeDetails.OrderBy(u => u.RopeType).
                 Skip((CommonMethods.PAGESIZE * currPage) - CommonMethods.PAGESIZE).Take(CommonMethods.PAGESIZE).ToList();

            ropeDetailsReport.FleetNames = PermittedFleetNames;
            ropeDetailsReport.FleetTypes = PermittedFleetTypes;

            ropeDetailsReport.Vessels = PermittedVessels; // context.VesselDetails.Where(p => ropeDetailsReport.FleetTypeIDs.Contains(p.FleetTypeID) && ropeDetailsReport.FleetNameIDs.Contains(p.FleetNameID) && ropeDetailsReport.TradeIDs.Contains(p.TradeAreaID)).ToList();

            TempData["SelectedFleetTypes"] = ropeDetailsReport.FleetTypeIDs;
            TempData["SelectedFleetNames"] = ropeDetailsReport.FleetNameIDs;
            TempData["SelectedTrades"] = ropeDetailsReport.TradeIDs;
            TempData["SelectedVessels"] = ropeDetailsReport.VesselIDs;

            return View(ropeDetailsReport);
        }

        public ActionResult AssignRopeToWinch(int vid, int? cp)
        {
            InitializeFilterModel();

            //filterModel.AgeRangeFrom = 0;
            //filterModel.AgeRangeTo = 50;
            //filterModel.DateFrom = DateTime.Today.AddYears(-1);
            //filterModel.DateUpto = DateTime.Today;
            ////Request.QueryString["vid"] = vid.ToString();
            //filterModel.FleetNames = PermittedFleetNames;
            //filterModel.FleetTypes = PermittedFleetTypes;
            //filterModel.Vessels = PermittedVessels;
            DataSet DTS = CommonMethods.ExecStoredProceedureWithDataSet("SP_AssignRopeToWinchList", vid);
            DataTable tbl = DTS.Tables[0];
            DataTable Tails = DTS.Tables[1];
            List<WinchAssignedRopesTails> AssigedTail = new List<WinchAssignedRopesTails>();
            if (Tails.Rows.Count > 0)
            {
                for (int i = 0; i < Tails.Rows.Count; i++)
                {

                    AssigedTail.Add(new WinchAssignedRopesTails()
                    {
                        VesselID = Convert.ToInt32(Tails.Rows[i]["VesselID"]),
                        VesselName = Tails.Rows[i]["VesselName"].ToString(),
                        AssignedDate = Convert.ToDateTime(Tails.Rows[i]["AssignedDate"]),
                        Outboard = Tails.Rows[i]["Outboard"].ToString(),
                        UniqueID = Tails.Rows[i]["UniqueId"].ToString(),
                        assignedWinchNumber = Tails.Rows[i]["assignednumber"].ToString(),
                        AssignedLocation = Tails.Rows[i]["AssignedLocation"].ToString(),
                        MBL_Max_BHF = Tails.Rows[i]["MBL"].ToString(),
                        Lead = tbl.Rows[i]["Lead"].ToString(),
                        RopeId = Convert.ToInt32(Tails.Rows[i]["RopeId"]),
                        WinchId = Convert.ToInt32(Tails.Rows[i]["WinchId"]),
                        RopeCertificateNumber = Tails.Rows[i]["certificatenumber"].ToString(),
                        RopeTail = Convert.ToBoolean(Tails.Rows[i]["RopeTail"])

                    });
                }
            }



            List<WinchAssignedRopesTails> AssignRopeToWinchList = new List<WinchAssignedRopesTails>();
            if (tbl.Rows.Count > 0)
            {
                for (int i = 0; i < tbl.Rows.Count; i++)
                {

                    AssignRopeToWinchList.Add(new WinchAssignedRopesTails()
                    {
                        VesselID = Convert.ToInt32(tbl.Rows[i]["VesselID"]),
                        VesselName = tbl.Rows[i]["VesselName"].ToString(),
                        AssignedDate = Convert.ToDateTime(tbl.Rows[i]["AssignedDate"]),
                        Outboard = tbl.Rows[i]["Outboard"].ToString(),
                        UniqueID = tbl.Rows[i]["UniqueId"].ToString(),
                        assignedWinchNumber = tbl.Rows[i]["assignednumber"].ToString(),
                        AssignedLocation = tbl.Rows[i]["AssignedLocation"].ToString(),
                        MBL_Max_BHF = tbl.Rows[i]["MBL"].ToString(),
                        Lead = tbl.Rows[i]["Lead"].ToString(),
                        RopeId = Convert.ToInt32(tbl.Rows[i]["RopeId"]),
                        WinchId = Convert.ToInt32(tbl.Rows[i]["WinchId"]),
                        RopeCertificateNumber = tbl.Rows[i]["certificatenumber"].ToString(),
                        RopeTail = Convert.ToBoolean(tbl.Rows[i]["RopeTail"]),
                        TailList = AssigedTail.Where(x => x.WinchId == Convert.ToInt32(tbl.Rows[i]["WinchId"])).Select(s => new AssignedTailsDetail() { RopeId = s.RopeId, WinchId = s.WinchId, TailCertificateNumber = s.RopeCertificateNumber, UniqueID = s.UniqueID, VesselID = s.VesselID }).ToList(),

                    });
                }
            }


            filterModel.ListAssignRopeToWinch = AssignRopeToWinchList;

            //filterModel.VesselIDs = (List<int>)TempData["SelectedVessels"];
            //if (filterModel.VesselIDs?.Count > 0)
            //{

            //    AssignRopeToWinchList = AssignRopeToWinchList.Where(p => filterModel.VesselIDs.Contains(p.VesselID)).ToList();
            //}
            //else
            //{
            //    List<int> vids = new List<int>() { vid };
            //    filterModel.VesselIDs = vids;
            //    AssignRopeToWinchList = AssignRopeToWinchList.Where(p => filterModel.VesselIDs.Contains(p.VesselID)).ToList();
            //    //AssignRopeToWinchList = AssignRopeToWinchList.Where(p => p.VesselID == vid).ToList();
            //}



            //int currPage = cp == null ? 1 : Convert.ToInt32(cp);

            //TempData["CurrentPage"] = currPage;
            //TempData["TotalRecords"] = AssignRopeToWinchList.Count();

            //filterModel.ListAssignRopeToWinch = AssignRopeToWinchList.Skip((CommonMethods.PAGESIZE * currPage) - CommonMethods.PAGESIZE).Take(CommonMethods.PAGESIZE).ToList();
            //TempData["SelectedVessels"] = filterModel.VesselIDs;
            return View(filterModel);
        }

        //[HttpPost]
        //public ActionResult AssignRopeToWinch(int? cp, FilterModel model)
        //{


        //    model.FleetNames = PermittedFleetNames;
        //    model.FleetTypes = PermittedFleetTypes;
        //    model.Vessels = PermittedVessels;
        //    DataTable tbl = CommonMethods.ExecStoredProceedureBetweenDates("SP_AssignRopeToWinchList", Convert.ToDateTime(model.DateFrom), Convert.ToDateTime(model.DateUpto));
        //    List<SP_AssignRopeToWinchList_Result> AssignRopeToWinchList = new List<SP_AssignRopeToWinchList_Result>();
        //    if (tbl.Rows.Count > 0)
        //    {
        //        for (int i = 0; i < tbl.Rows.Count; i++)
        //        {

        //            AssignRopeToWinchList.Add(new SP_AssignRopeToWinchList_Result()
        //            {
        //                VesselID = Convert.ToInt32(tbl.Rows[i]["VesselID"]),
        //                VesselName = tbl.Rows[i]["VesselName"].ToString(),
        //                AssignedDate = Convert.ToDateTime(tbl.Rows[i]["AssignedDate"]),
        //                Outboard = tbl.Rows[i]["Outboard"].ToString(),
        //                UniqueId = tbl.Rows[i]["UniqueId"].ToString(),
        //                Assignednumber = tbl.Rows[i]["assignednumber"].ToString(),
        //                Certificatenumber = tbl.Rows[i]["certificatenumber"].ToString(),
        //                AssignedLocation = tbl.Rows[i]["AssignedLocation"].ToString(),
        //                Status = tbl.Rows[i]["Status"].ToString()

        //            });
        //        }
        //    }

        //    model.VesselIDs = (List<int>)TempData["SelectedVessels"];
        //    if (model.VesselIDs?.Count > 0)
        //    {
        //        AssignRopeToWinchList = AssignRopeToWinchList.Where(p => model.VesselIDs.Contains(p.VesselID)).ToList();
        //    }

        //    int currPage = cp == null ? 1 : Convert.ToInt32(cp);

        //    TempData["CurrentPage"] = currPage;
        //    TempData["TotalRecords"] = AssignRopeToWinchList.Count();

        //    model.ListAssignRopeToWinch = AssignRopeToWinchList.Skip((CommonMethods.PAGESIZE * currPage) - CommonMethods.PAGESIZE).Take(CommonMethods.PAGESIZE).ToList();
        //    TempData["SelectedVessels"] = model.VesselIDs;

        //    return View(model);
        //}

        public ActionResult LooseEquip()
        {
            return View();
        }

        public ActionResult Vessels(int? id)
        {
            var vesselInfo = PermittedVessels.AsQueryable(); // context.VesselDetails.OrderBy(u => u.VesselName).AsQueryable();

            InitializeFilterModel();

            filterModel.AgeRangeFrom = 0;
            filterModel.AgeRangeTo = 50;
            filterModel.DateFrom = DateTime.Today.AddYears(-1);
            filterModel.DateUpto = DateTime.Today;

            filterModel.VesselIDs = (List<int>)TempData["SelectedVessels"];
            if (filterModel.VesselIDs?.Count > 0)
            {
                vesselInfo = vesselInfo.Where(p => filterModel.VesselIDs.Contains(p.ImoNo)).AsQueryable();
            }

            filterModel.FleetNameIDs = (List<int>)TempData["SelectedFleetNames"];
            if (filterModel.FleetNameIDs?.Count > 0)
            {
                vesselInfo = vesselInfo.Where(p => filterModel.FleetNameIDs.Contains(p.FleetNameID)).AsQueryable();
            }

            filterModel.FleetTypeIDs = (List<int>)TempData["SelectedFleetTypes"];
            if (filterModel.FleetTypeIDs?.Count > 0)
            {
                vesselInfo = vesselInfo.Where(p => filterModel.FleetTypeIDs.Contains(p.FleetTypeID)).AsQueryable();
            }

            filterModel.TradeIDs = (List<int>)TempData["SelectedTrades"];
            if (filterModel.TradeIDs?.Count > 0)
            {
                vesselInfo = vesselInfo.Where(p => filterModel.TradeIDs.Contains(p.TradeAreaID)).AsQueryable();
            }

            int currPage = id == null ? 1 : Convert.ToInt32(id);

            TempData["CurrentPage"] = currPage;
            TempData["TotalRecords"] = vesselInfo.Count();

            filterModel.VesselList = vesselInfo.OrderBy(p => p.VesselName).Skip((CommonMethods.PAGESIZE * currPage) - CommonMethods.PAGESIZE).Take(CommonMethods.PAGESIZE).ToList();

            return View(filterModel);
        }

        [HttpPost]
        public ActionResult Vessels(FilterModel model)
        {
            var vessels = PermittedVessels; // context.VesselDetails.OrderBy(u => u.VesselName).AsQueryable();
            model.Vessels = vessels.ToList();

            DateTime builtFrom = DateTime.Today.AddYears(-model.AgeRangeFrom);
            DateTime builtUpto = DateTime.Today.AddYears(-model.AgeRangeTo);

            var vesselInfo = vessels.Where(p => p.DateBuilt <= builtFrom && p.DateBuilt >= builtUpto).AsQueryable();

            if (model.VesselIDs?.Count > 0)
            {
                vesselInfo = vesselInfo.Where(p => model.VesselIDs.Contains(p.ImoNo)).AsQueryable();
                TempData["SelectedVessels"] = model.VesselIDs;
            }

            if (model.FleetNameIDs?.Count > 0)
            {
                vesselInfo = vesselInfo.Where(p => model.FleetNameIDs.Contains(p.FleetNameID)).AsQueryable();
                TempData["SelectedFleetNames"] = model.FleetNameIDs;
            }

            if (model.FleetTypeIDs?.Count > 0)
            {
                vesselInfo = vesselInfo.Where(p => model.FleetTypeIDs.Contains(p.FleetTypeID)).AsQueryable();
                TempData["SelectedFleetTypes"] = model.FleetTypeIDs;
            }

            if (model.TradeIDs?.Count > 0)
            {
                vesselInfo = vesselInfo.Where(p => model.TradeIDs.Contains(p.TradeAreaID)).AsQueryable();
                TempData["SelectedTrades"] = model.TradeIDs;
            }

            int currPage = TempData["CurrentPage"] == null ? 1 : Convert.ToInt32(TempData["CurrentPage"]);

            TempData["TotalRecords"] = vesselInfo.Count();

            model.VesselList = vesselInfo.Skip((CommonMethods.PAGESIZE * currPage) - CommonMethods.PAGESIZE).Take(CommonMethods.PAGESIZE).ToList();

            //vesselInfo.Skip((currPage - 1) *  pageSize).Take(pageSize);

            return View(model);
        }

        #region Inspection_Reports

        public ActionResult LooseInspReport(int? vid, int? cp)
        {
            LooseEquipDetailReport detailReport = new LooseEquipDetailReport
            {
                LooseEquipments = context.LooseETypes.ToList(),
                Vessels = PermittedVessels, // context.VesselDetails.OrderBy(u => u.VesselName).ToList();
                //DateFrom = DateTime.Today.AddYears(-1),
                //DateUpto = DateTime.Today
            };

            //var inspections = context.View_ReportLooseEquipmentInspection.Where
            //    (p => p.InspectDate >= detailReport.DateFrom && p.InspectDate <= detailReport.DateUpto).OrderByDescending(u => u.InspectDate).AsQueryable();

            var inspections = context.View_ReportLooseEquipmentInspection.
                OrderByDescending(u => u.InspectDate).AsQueryable();

            if (vid > 0)
            {
                detailReport.VesselIDs.Add(Convert.ToInt32(vid));
                inspections = inspections.Where(p => detailReport.VesselIDs.Contains(p.VesselID));
                detailReport.LooseEquipments = detailReport.LooseEquipments.ToList();
            }

            int currPage = cp == null ? 1 : Convert.ToInt32(cp);

            TempData["CurrentPage"] = currPage;
            TempData["TotalRecords"] = inspections.Count();

            detailReport.ListLooseEquipmentInspection = inspections.ToList();

            return View(detailReport);
        }

        [HttpPost]
        public ActionResult LooseInspReport(LooseEquipDetailReport detailReport)
        {
            detailReport.LooseEquipments = context.LooseETypes.ToList();
            detailReport.FleetNames = PermittedFleetNames;
            detailReport.FleetTypes = PermittedFleetTypes;
            detailReport.Vessels = PermittedVessels;
            //detailReport.LooseEquipments = detailReport.LooseEquipments.ToList();

            //var inspections = context.View_ReportLooseEquipmentInspection.Where(
            //    p => p.InspectDate >= detailReport.DateFrom && p.InspectDate <= detailReport.DateUpto).OrderByDescending(u => u.InspectDate).AsQueryable();

            var inspections = context.View_ReportLooseEquipmentInspection.AsQueryable();

            if (detailReport.DateFrom != null)
                inspections = inspections.Where(p => p.InspectDate >= detailReport.DateFrom);

            if (detailReport.DateUpto != null)
                inspections = inspections.Where(p => p.InspectDate <= detailReport.DateUpto);

            if (detailReport.VesselIDs.Count > 0)
                inspections = inspections.Where(p => detailReport.VesselIDs.Contains(p.VesselID));

            if (detailReport.LooseEquipmentIDs.Count > 0)
                inspections = inspections.Where(p => detailReport.LooseEquipmentIDs.Contains(p.LooseETypeId ?? 0));

            int currPage = 1;
            TempData["CurrentPage"] = currPage;
            TempData["TotalRecords"] = inspections.Count();

            detailReport.ListLooseEquipmentInspection = inspections.OrderByDescending(p => p.InspectDate).
                Skip((CommonMethods.PAGESIZE * currPage) - CommonMethods.PAGESIZE).
                Take(CommonMethods.PAGESIZE).ToList();

            // detailReport.Vessels = context.VesselDetails.Where(p => detailReport.FleetTypeIDs.Contains(p.FleetTypeID) && detailReport.FleetNameIDs.Contains(p.FleetNameID) && detailReport.TradeIDs.Contains(p.TradeAreaID)).ToList();
            // detailReport.LooseEquipments = context.LooseETypes.ToList();
            // detailReport.Vessels = PermittedVessels;
            return View(detailReport);
        }

        public ActionResult RopeInspReport(int? vid, int? cp)
        {
            RopeInspectionReport inspectionReport = new RopeInspectionReport
            {
                //DateFrom = DateTime.Today.AddYears(-1),
                //DateUpto = DateTime.Today,
                RunningHoursFrom = 1,
                RunningHoursTo = 100000
            };

            var inspections = context.View_MooringRopeDetails.Where(p => p.RopeTail == 0).AsQueryable();

            if (vid > 0)
            {
                inspectionReport.VesselIDs.Add(Convert.ToInt32(vid));
                inspections = inspections.Where(p => inspectionReport.VesselIDs.Contains(p.VesselID));
            }

            var insFound = inspections.GroupBy(p => new { p.InspectBy, p.InspectDate, p.InspectionID, p.RopeTail }).
                Select(u => new RopeInspectionReportList { InspectBy = u.Key.InspectBy, InspectDate = u.Key.InspectDate, InspectionID = u.Key.InspectionID, RopeTail = u.Key.RopeTail });

            int currPage = cp == null ? 1 : Convert.ToInt32(cp);

            TempData["CurrentPage"] = currPage;
            TempData["TotalRecords"] = insFound.Count();

            inspectionReport.RopeInspectionList = insFound.Where(x => x.InspectionID != null).OrderByDescending(u => u.InspectDate).ToList();
            inspectionReport.Vessels = PermittedVessels; // context.VesselDetails.OrderBy(u => u.VesselName).ToList();

            return View(inspectionReport);
        }

        [HttpPost]
        public ActionResult RopeInspReport(RopeInspectionReport inspectionReport)
        {
            //var inspections = context.View_MooringRopeDetails.Where(p => inspectionReport.VesselIDs.Contains(p.VesselID) &&
            //  p.InstalledDate >= inspectionReport.DateFrom && p.InstalledDate <= inspectionReport.DateUpto).AsEnumerable();

            var inspections = context.View_MooringRopeDetails.Where(p => inspectionReport.VesselIDs.Contains(p.VesselID)).AsEnumerable();

            if (inspectionReport.DateFrom != null)
                inspections = inspections.Where(p => p.InstalledDate >= inspectionReport.DateFrom);

            if (inspectionReport.DateUpto != null)
                inspections = inspections.Where(p => p.InstalledDate <= inspectionReport.DateUpto);

            inspectionReport.FleetNames = PermittedFleetNames;
            inspectionReport.FleetTypes = PermittedFleetTypes;

            inspectionReport.Vessels = context.VesselDetails.
                Where(p => inspectionReport.FleetTypeIDs.Contains(p.FleetTypeID) &&
                inspectionReport.FleetNameIDs.Contains(p.FleetNameID) &&
                inspectionReport.TradeIDs.Contains(p.TradeAreaID)).ToList();

            if (inspectionReport.ManufacturerIDs?.Count > 0)
            {
                inspections = inspections.Where(p => inspectionReport.ManufacturerIDs.Contains(p.ManufacturerId ?? 0)).AsQueryable();
                TempData["SelectedManufacturers"] = inspectionReport.ManufacturerIDs;
            }

            if (inspectionReport.RopeTypeIDs?.Count > 0)
            {
                inspections = inspections.Where(p => inspectionReport.RopeTypeIDs.Contains(p.RopeTypeId ?? 0)).AsQueryable();
                TempData["SelectedRopeTypes"] = inspectionReport.RopeTypeIDs;
            }

            if (inspectionReport.InspectionRatingIDs?.Count > 0)
            {
                inspections = inspections.Where(p => inspectionReport.InspectionRatingIDs.Contains(p.AverageRating_A ?? 0) ||
                inspectionReport.InspectionRatingIDs.Contains(p.AverageRating_B ?? 0)).AsQueryable();

                TempData["SelectedInspectionRatings"] = inspectionReport.InspectionRatingIDs;
            }

            inspections = inspections.Where(p => p.CurrentRunningHours >= inspectionReport.RunningHoursFrom).AsQueryable();
            inspections = inspections.Where(p => p.CurrentRunningHours <= inspectionReport.RunningHoursTo).AsQueryable();

            var insFound = inspections.GroupBy(p => new { p.InspectBy, p.InspectDate, p.InspectionID, p.RopeTail }).
                Select(u => new RopeInspectionReportList { InspectBy = u.Key.InspectBy, InspectDate = u.Key.InspectDate, InspectionID = u.Key.InspectionID, RopeTail = u.Key.RopeTail }).
                OrderBy(u => u.InspectDate);

            int currPage = 1;
            TempData["CurrentPage"] = currPage;
            TempData["TotalRecords"] = insFound.Count();

            inspectionReport.RopeInspectionList = insFound.Where(p => p.RopeTail == 0).
                Skip((CommonMethods.PAGESIZE * currPage) - CommonMethods.PAGESIZE).
                Take(CommonMethods.PAGESIZE).OrderByDescending(p => p.InspectDate).ToList();

            return View(inspectionReport);
        }

        public ActionResult RopeTailInspReport(int? vid, int? cp)
        {
            RopeInspectionReport inspectionReport = new RopeInspectionReport
            {
                //DateFrom = DateTime.Today.AddYears(-1),
                //DateUpto = DateTime.Today,
                RunningHoursFrom = 1,
                RunningHoursTo = 100000
            };

            //var inspections = context.View_MooringRopeDetails.Where(p => p.InspectDate >= inspectionReport.DateFrom && p.InspectDate <= inspectionReport.DateUpto).AsQueryable();

            var inspections = context.View_MooringRopeDetails.AsQueryable();

            if (inspectionReport.DateFrom != null)
                inspections = inspections.Where(p => p.InspectDate >= inspectionReport.DateFrom);

            if (inspectionReport.DateUpto != null)
                inspections = inspections.Where(p => p.InspectDate <= inspectionReport.DateUpto);

            if (vid > 0)
            {
                inspectionReport.VesselIDs.Add(Convert.ToInt32(vid));
                inspections = inspections.Where(p => inspectionReport.VesselIDs.Contains(p.VesselID));
            }

            var insFound = inspections.GroupBy(p => new { p.InspectBy, p.InspectDate, p.InspectionID, p.RopeTail }).
                Select(u => new RopeInspectionReportList { InspectBy = u.Key.InspectBy, InspectDate = u.Key.InspectDate, InspectionID = u.Key.InspectionID, RopeTail = u.Key.RopeTail });

            int currPage = cp == null ? 1 : Convert.ToInt32(cp);

            TempData["CurrentPage"] = currPage;
            TempData["TotalRecords"] = insFound.Count();

            inspectionReport.RopeTailInspectionList = insFound.Where(p => p.RopeTail == 1 && p.InspectionID != null).OrderByDescending(u => u.InspectDate).ToList();
            inspectionReport.Vessels = PermittedVessels; // context.VesselDetails.OrderBy(u => u.VesselName).ToList();

            return View(inspectionReport);
        }

        [HttpPost]
        public ActionResult RopeTailInspReport(RopeInspectionReport inspectionReport)
        {
            //var inspections = context.View_MooringRopeDetails.Where(p => inspectionReport.VesselIDs.Contains(p.VesselID) &&
            //  p.InstalledDate >= inspectionReport.DateFrom && p.InstalledDate <= inspectionReport.DateUpto).AsEnumerable();

            var inspections = context.View_MooringRopeDetails.Where(p => inspectionReport.VesselIDs.Contains(p.VesselID)).AsQueryable();

            if (inspectionReport.DateFrom != null)
                inspections = inspections.Where(p => p.InstalledDate >= inspectionReport.DateFrom);

            if (inspectionReport.DateUpto != null)
                inspections = inspections.Where(p => p.InstalledDate <= inspectionReport.DateUpto);

            if (inspectionReport.ManufacturerIDs?.Count > 0)
                inspections = inspections.Where(p => inspectionReport.ManufacturerIDs.Contains(p.ManufacturerId ?? 0)).AsQueryable();

            if (inspectionReport.RopeTypeIDs?.Count > 0)
                inspections = inspections.Where(p => inspectionReport.RopeTypeIDs.Contains(p.RopeTypeId ?? 0)).AsQueryable();

            if (inspectionReport.InspectionRatingIDs?.Count > 0)
                inspections = inspections.Where(p => inspectionReport.InspectionRatingIDs.Contains(p.AverageRating_A ?? 0) ||
                inspectionReport.InspectionRatingIDs.Contains(p.AverageRating_B ?? 0)).AsQueryable();

            inspections = inspections.Where(p => p.CurrentRunningHours >= inspectionReport.RunningHoursFrom).AsQueryable();
            inspections = inspections.Where(p => p.CurrentRunningHours <= inspectionReport.RunningHoursTo).AsQueryable();

            var insFound = inspections.GroupBy(p => new { p.InspectBy, p.InspectDate, p.InspectionID, p.RopeTail }).
                Select(u => new RopeInspectionReportList { InspectBy = u.Key.InspectBy, InspectDate = u.Key.InspectDate, InspectionID = u.Key.InspectionID, RopeTail = u.Key.RopeTail }).
                OrderBy(u => u.InspectDate);

            int currPage = 1;
            TempData["CurrentPage"] = currPage;
            TempData["TotalRecords"] = insFound.Count();

            inspectionReport.RopeTailInspectionList = insFound.Where(p => p.RopeTail == 1).
                OrderByDescending(u => u.InspectDate).
                Skip((CommonMethods.PAGESIZE * currPage) - CommonMethods.PAGESIZE).
                Take(CommonMethods.PAGESIZE).ToList();

            inspectionReport.FleetNames = PermittedFleetNames;
            inspectionReport.FleetTypes = PermittedFleetTypes;

            inspectionReport.Vessels = context.VesselDetails.
                Where(p => inspectionReport.FleetTypeIDs.Contains(p.FleetTypeID) && inspectionReport.FleetNameIDs.Contains(p.FleetNameID) && inspectionReport.TradeIDs.Contains(p.TradeAreaID)).ToList();

            return View(inspectionReport);
        }

        //public JsonResult ViewInspDetails(int vesselid, int ropetail, int ropeid)
        //{
        //    filterModel.ViewInspectionDetails = context.spInspectionDetail(vesselid, ropetail, ropeid).ToList();
        //    return Json(new { Result = true, Data = filterModel.ViewInspectionDetails }, JsonRequestBehavior.AllowGet);
        //}

        public JsonResult ViewInspDetails(int? inspectionId, int? ropeTail, int? vesselId)
        {
            var inspectionDetails = context.View_MooringRopeInspectionDetails.
                Where(p => p.InspectionId == inspectionId && p.RopeTail == ropeTail && p.VesselID == vesselId).
                ToList();

            return Json(new { Result = true, Data = inspectionDetails }, JsonRequestBehavior.AllowGet);
        }

        #endregion


        public ActionResult LooseEquDetail(int vid, int? cp)
        {
            LooseEquDetailsList detailReport = new LooseEquDetailsList();

            var Data = CommonMethods.ExecStoredProceedureWithDataSet("SPLooseEquDetails", vid);

            detailReport.JoiningShackle = TableToLEqList(Data.Tables[0], "JoiningShackle");
            detailReport.ChainStopper = TableToLEqList(Data.Tables[1], "ChainStopper");
            detailReport.ChafeGuard = TableToLEqList(Data.Tables[2], "ChafeGuard");
            detailReport.WinchBreakTestKit = TableToLEqList(Data.Tables[3], "WinchBreakTestKit");
            detailReport.RopeStopper = TableToLEqList(Data.Tables[4], "RopeTail");
            detailReport.MessengerRope = TableToLEqList(Data.Tables[5], "RopeTail");
            detailReport.FireWire = TableToLEqList(Data.Tables[6], "RopeTail");
            detailReport.TowingRope = TableToLEqList(Data.Tables[7], "RopeTail");
            detailReport.SuezRope = TableToLEqList(Data.Tables[8], "RopeTail");



            //int currPage = cp == null ? 1 : Convert.ToInt32(cp);

            //TempData["CurrentPage"] = currPage;
            //TempData["TotalRecords"] = inspections.Count();

            //detailReport.ListLooseEquipmentInspection = inspections.ToList();

            return View(detailReport);
        }


        private List<LooseEquDetailClass> TableToLEqList(DataTable tbl, string zone)
        {
            List<LooseEquDetailClass> list = new List<LooseEquDetailClass>();
            if (tbl.Rows.Count > 0 && zone == "JoiningShackle")
            {
                for (int i = 0; i < tbl.Rows.Count; i++)
                {
                    var Detail = new LooseEquDetailClass()
                    {
                        UniqueID = tbl.Rows[i]["UniqueID"].ToString(),
                        LooseETypeId = Convert.ToInt32(tbl.Rows[i]["LooseETypeId"]),
                        IdentificationNumber = tbl.Rows[i]["IdentificationNumber"].ToString(),
                        ManufactureName = tbl.Rows[i]["ManufactureName"].ToString(),
                        CertificateNumber = tbl.Rows[i]["CertificateNumber"].ToString(),
                        Type = tbl.Rows[i]["Type"].ToString(),
                        MBL = Convert.ToDecimal(tbl.Rows[i]["MBL"]),
                        //ReceivedDate = Convert.ToDateTime(tbl.Rows[i]["DateReceived"]),
                        //InstalledDate = Convert.ToDateTime(tbl.Rows[i]["DateInstalled"]),
                        //InspectionDueDate = Convert.ToDateTime(tbl.Rows[i]["InspectionDueDate"]),
                        VesselName = tbl.Rows[i]["VesselName"].ToString(),
                    };
                    if (!string.IsNullOrEmpty(tbl.Rows[i]["DateReceived"].ToString()))
                        Detail.ReceivedDate = Convert.ToDateTime(tbl.Rows[i]["DateReceived"]);
                    if (!string.IsNullOrEmpty(tbl.Rows[i]["DateInstalled"].ToString()))
                        Detail.InstalledDate = Convert.ToDateTime(tbl.Rows[i]["DateInstalled"]);
                    if (!string.IsNullOrEmpty(tbl.Rows[i]["InspectionDueDate"].ToString()))
                        Detail.InspectionDueDate = Convert.ToDateTime(tbl.Rows[i]["InspectionDueDate"]);
                    list.Add(Detail);
                }
            }
            if (tbl.Rows.Count > 0 && zone == "ChainStopper")
            {
                for (int i = 0; i < tbl.Rows.Count; i++)
                {
                    var Detail = new LooseEquDetailClass()
                    {
                        UniqueID = tbl.Rows[i]["UniqueID"].ToString(),
                        LooseETypeId = Convert.ToInt32(tbl.Rows[i]["LooseETypeId"]),
                        //IdentificationNumber = tbl.Rows[i]["IdentificationNumber"].ToString(),
                        ManufactureName = tbl.Rows[i]["ManufactureName"].ToString(),
                        CertificateNumber = tbl.Rows[i]["CertificateNumber"].ToString(),
                        Length = Convert.ToDecimal(tbl.Rows[i]["Length"]),
                        MBL = Convert.ToDecimal(tbl.Rows[i]["MBL"]),
                        ReceivedDate = Convert.ToDateTime(tbl.Rows[i]["DateReceived"]),
                        InstalledDate = Convert.ToDateTime(tbl.Rows[i]["DateInstalled"]),
                        InspectionDueDate = Convert.ToDateTime(tbl.Rows[i]["InspectionDueDate"]),
                        VesselName = tbl.Rows[i]["VesselName"].ToString(),
                    };

                    list.Add(Detail);
                }
            }

            if (tbl.Rows.Count > 0 && (zone == "ChafeGuard" || zone == "WinchBreakTestKit"))
            {
                for (int i = 0; i < tbl.Rows.Count; i++)
                {
                    var Detail = new LooseEquDetailClass()
                    {
                        UniqueID = tbl.Rows[i]["UniqueID"].ToString(),
                        //LooseETypeId = Convert.ToInt32(tbl.Rows[i]["LooseETypeId"]),
                        //IdentificationNumber = tbl.Rows[i]["IdentificationNumber"].ToString(),
                        ManufactureName = tbl.Rows[i]["ManufacturerName"].ToString(),
                        //CertificateNumber = tbl.Rows[i]["CertificateNumber"].ToString(),
                        //Type = tbl.Rows[i]["Type"].ToString(),
                        //MBL = Convert.ToDecimal(tbl.Rows[i]["MBL"]),
                        //ReceivedDate = Convert.ToDateTime(tbl.Rows[i]["ReceivedDate"]),
                        //InstalledDate = Convert.ToDateTime(tbl.Rows[i]["InstalledDate"]),
                        //InspectionDueDate = Convert.ToDateTime(tbl.Rows[i]["InspectionDueDate"]),
                        VesselName = tbl.Rows[i]["VesselName"].ToString(),
                    };
                    if (!string.IsNullOrEmpty(tbl.Rows[i]["ReceivedDate"].ToString()))
                        Detail.ReceivedDate = Convert.ToDateTime(tbl.Rows[i]["ReceivedDate"]);
                    if (!string.IsNullOrEmpty(tbl.Rows[i]["InstalledDate"].ToString()))
                        Detail.InstalledDate = Convert.ToDateTime(tbl.Rows[i]["InstalledDate"]);
                    if (!string.IsNullOrEmpty(tbl.Rows[i]["InspectionDueDate"].ToString()))
                        Detail.InspectionDueDate = Convert.ToDateTime(tbl.Rows[i]["InspectionDueDate"]);

                    list.Add(Detail);
                }
            }

            if (tbl.Rows.Count > 0 && (zone == "RopeTail"))
            {
                for (int i = 0; i < tbl.Rows.Count; i++)
                {
                    var Detail = new LooseEquDetailClass()
                    {
                        UniqueID = tbl.Rows[i]["UniqueID"].ToString(),
                        LooseETypeId = Convert.ToInt32(tbl.Rows[i]["LooseETypeId"]),
                        ManufactureName = tbl.Rows[i]["ManufactureName"].ToString(),
                        CertificateNumber = tbl.Rows[i]["CertificateNumber"].ToString(),
                        RopeConstruction = tbl.Rows[i]["RopeConstruction"].ToString(),
                        Diameter = string.IsNullOrEmpty(tbl.Rows[i]["Diameter"].ToString()) == true ? 0 : Convert.ToDecimal(tbl.Rows[i]["Diameter"]),
                        MBL = string.IsNullOrEmpty(tbl.Rows[i]["MBL"].ToString()) == true ? 0 : Convert.ToDecimal(tbl.Rows[i]["MBL"]),
                        LDBF = string.IsNullOrEmpty(tbl.Rows[i]["LDBF"].ToString()) == true ? 0 : Convert.ToDecimal(tbl.Rows[i]["LDBF"]),
                        WLL = string.IsNullOrEmpty(tbl.Rows[i]["WLL"].ToString()) == true ? 0 : Convert.ToDecimal(tbl.Rows[i]["WLL"]),
                        RopeTagging = tbl.Rows[i]["RopeTagging"].ToString(),
                        //ReceivedDate = string.IsNullOrEmpty(tbl.Rows[i]["ReceivedDate"].ToString()) == true ? null :  Convert.ToDateTime(tbl.Rows[i]["ReceivedDate"]),
                        //InstalledDate = Convert.ToDateTime(tbl.Rows[i]["InstalledDate"]),
                        //InspectionDueDate = Convert.ToDateTime(tbl.Rows[i]["InspectionDueDate"]),
                        VesselName = tbl.Rows[i]["VesselName"].ToString(),
                    };
                    if (!string.IsNullOrEmpty(tbl.Rows[i]["ReceivedDate"].ToString()))
                        Detail.ReceivedDate = Convert.ToDateTime(tbl.Rows[i]["ReceivedDate"]);
                    if (!string.IsNullOrEmpty(tbl.Rows[i]["InstalledDate"].ToString()))
                        Detail.InstalledDate = Convert.ToDateTime(tbl.Rows[i]["InstalledDate"]);
                    if (!string.IsNullOrEmpty(tbl.Rows[i]["InspectionDueDate"].ToString()))
                        Detail.InspectionDueDate = Convert.ToDateTime(tbl.Rows[i]["InspectionDueDate"]);

                    list.Add(Detail);
                }
            }
            return list;
        }


        public ActionResult RopeSummary(int rope, int tail, int vessel)
        {
            ViewBag.IsTail = tail;

            RopeSummaryReport ropeSummary = new RopeSummaryReport();

            ropeSummary.RopesSplicedList = context.vRopesSpliceds.Where(p => p.RopeId == rope && p.VesselID == vessel && p.RopeTail == tail).ToList();
            ropeSummary.RopesEndToEndList = context.vRopesEndToEnds.Where(p => p.RopeId == rope && p.VesselID == vessel && p.RopeTail == tail).ToList();
            ropeSummary.RopesDisposedList = context.vRopesDisposeds.Where(p => p.RopeId == rope && p.VesselID == vessel && p.RopeTail == tail).ToList();
            ropeSummary.RopesDetailList = context.vRopesDetails.Where(p => p.RopeId == rope && p.VesselID == vessel && p.RopeTail == tail).ToList();
            ropeSummary.RopesDamagedList = context.vRopesDamageds.Where(p => p.RopeId == rope && p.VesselID == vessel && p.RopeTail == tail).ToList();
            ropeSummary.RopesCroppedList = context.vRopesCroppeds.Where(p => p.RopeId == rope && p.VesselID == vessel && p.RopeTail == tail).ToList();
            ropeSummary.RopesDiscardedList = context.MooringRopeDetails.Where(p => p.RopeId == rope && p.VesselID == vessel && p.RopeTail == tail && p.OutofServiceDate != null).ToList();
            ropeSummary.RopesInspectedList = context.MooringRopeInspections.OrderByDescending(u => u.InspectDate).Where(p => p.RopeId == rope && p.VesselID == vessel && p.RopeTail == tail).ToList();
            //ropeSummary.WinchRotationDue_Detail = CommonMethods.AllWinchRotationList.Where(x => x.StatusUpDue == "Overdue").ToList();
            ropeSummary.WinchRotationDue_Detail = CommonMethods.AllWinchRotationList.Where(p => p.RopeId == rope && p.VesselID == vessel).ToList();

            ropeSummary.ResidualLabTestList = CommonMethods.ResdualData(rope, tail, vessel);


            //ropeSummary.ResidualLabTestList=

            string attchment = "";
            try
            {
                if (tail == 0)
                {
                    SqlDataAdapter adp = new SqlDataAdapter("select * from MooringRopeAttachment where RopeId=" + rope + " and VesselId='" + vessel + "' and RopeTail=" + tail + " and LineResidual='Line'", con);
                    DataTable dt = new DataTable();
                    adp.Fill(dt);
                    if (dt.Rows.Count > 0)
                    {
                        attchment = dt.Rows[0]["AttachmentPath"].ToString();
                    }
                    else
                    {
                        attchment = "No Attachment";
                    }
                }
                if (tail == 1)
                {
                    SqlDataAdapter adp = new SqlDataAdapter("select * from MooringRopeAttachment where RopeId=" + rope + " and VesselId='" + vessel + "' and RopeTail=" + tail + " and LineResidual='RopeTail'", con);
                    DataTable dt = new DataTable();
                    adp.Fill(dt);
                    if (dt.Rows.Count > 0)
                    {
                        attchment = dt.Rows[0]["AttachmentPath"].ToString();
                    }
                    else
                    {
                        attchment = "No Attachment";
                    }
                }
            }
            catch { }

            ropeSummary.RopeAttachment = attchment;

            return View(ropeSummary);
        }


        public ActionResult Damaged(int vessel, string status)
        {
            RopeDamageCropSplicFilter LineData = new RopeDamageCropSplicFilter() { DateFrom = DateTime.Now.AddMonths(-1).Date, DateTo = DateTime.Now.Date };
            LineData.vessel = vessel;
            LineData.StatusName = string.IsNullOrEmpty(status) == true ? "All" : status;
            LineData.DamagedList = context.spDamage1(status, vessel).ToList();

            return View(LineData);
        }

        [HttpPost]
        public ActionResult Damaged(int vessel, string status, RopeDamageCropSplicFilter FilterData)
        {
            // RopeDamageCropSplicFilter LineData = new RopeDamageCropSplicFilter() { DateFrom = DateTime.Now.AddMonths(-1).Date, DateTo = DateTime.Now.Date };
            FilterData.vessel = vessel;
            FilterData.StatusName = string.IsNullOrEmpty(status) == true ? "All" : status;
            var results = context.spDamage1(status, vessel).Where(x => x.DamageDate >= FilterData.DateFrom.Date & x.DamageDate <= FilterData.DateTo.Date).ToList();

            if (FilterData.ManufacturerIDs?.Count > 0)
                results = results.Where(p => FilterData.ManufacturerIDs.Contains(p.Id)).ToList();

            if (FilterData.RopeTypeIDs?.Count > 0)
                results = results.Where(p => FilterData.RopeTypeIDs.Contains(p.RTID)).ToList();

            FilterData.DamagedList = results;

            return View(FilterData);
        }
        public ActionResult Cropped(int vessel)
        {
            RopeDamageCropSplicFilter LineData = new RopeDamageCropSplicFilter() { DateFrom = DateTime.Now.AddMonths(-1).Date, DateTo = DateTime.Now.Date };
            LineData.vessel = vessel;
            LineData.CroppedList = context.spCropping1(vessel).ToList();

            return View(LineData);
        }
        [HttpPost]
        public ActionResult Cropped(int vessel, RopeDamageCropSplicFilter FilterData)
        {

            FilterData.vessel = vessel;
            var results = context.spCropping1(vessel).Where(x => x.CroppedDate >= FilterData.DateFrom.Date & x.CroppedDate <= FilterData.DateTo.Date).ToList();

            if (FilterData.ManufacturerIDs?.Count > 0)
                results = results.Where(p => FilterData.ManufacturerIDs.Contains(p.Id)).ToList();

            if (FilterData.RopeTypeIDs?.Count > 0)
                results = results.Where(p => FilterData.RopeTypeIDs.Contains(p.RTID)).ToList();

            FilterData.CroppedList = results;

            return View(FilterData);
        }
        public ActionResult Spliced(int vessel)
        {
            RopeDamageCropSplicFilter LineData = new RopeDamageCropSplicFilter() { DateFrom = DateTime.Now.AddMonths(-1).Date, DateTo = DateTime.Now.Date };
            LineData.vessel = vessel;
            LineData.SplicedList = context.spSplicing1(vessel).ToList();

            return View(LineData);
        }
        [HttpPost]
        public ActionResult Spliced(int vessel, RopeDamageCropSplicFilter FilterData)
        {
            // RopeDamageCropSplicFilter LineData = new RopeDamageCropSplicFilter() { DateFrom = DateTime.Now.AddMonths(-1).Date, DateTo = DateTime.Now.Date };
            FilterData.vessel = vessel;
            var results = context.spSplicing1(vessel).Where(x => x.SplicingDoneDate >= FilterData.DateFrom.Date & x.SplicingDoneDate <= FilterData.DateTo.Date).ToList();

            if (FilterData.ManufacturerIDs?.Count > 0)
                results = results.Where(p => FilterData.ManufacturerIDs.Contains(p.Id)).ToList();

            if (FilterData.RopeTypeIDs?.Count > 0)
                results = results.Where(p => FilterData.RopeTypeIDs.Contains(p.RTID)).ToList();

            FilterData.SplicedList = results;
            return View(FilterData);
        }

        #region Dashboard_Reports

        public ActionResult SatisfactoryRopes()
        {
            RopeSummaryReport ropeSummary = new RopeSummaryReport
            {
                SatisfactoryRopesList = context.View_SatisfactoryRopes_Details.OrderByDescending(p => p.VesselID).ThenByDescending(p => p.RopeId).ToList(),
                UnSatisfactoryRopesList = context.View_UnSatisfactoryRopes_Details.OrderByDescending(p => p.VesselID).ThenByDescending(p => p.RopeId).ToList()
            };

            return View(ropeSummary);
        }

        public ActionResult OverdueInspection(int? o, int? p, string tab)
        {
            TempData["Tabs"] = string.IsNullOrEmpty(tab) == true ? "tab1" : tab;


            List<InspectionList> results = CommonMethods.InspectionList_Details;
            int currPage_Overdue = o == null ? 1 : Convert.ToInt32(o);
            TempData["CurrentPage_Overdue"] = currPage_Overdue;

            // var overdue = context.View_RopesOverdueInspection_Details.OrderByDescending(u => u.InspectionDueDate);
            var overdue = results.Where(x => x.InspectionDueDate < DateTime.Today).ToList();
            TempData["TotalRecords_Overdue"] = overdue.Count();

            int currPage_Pending = p == null ? 1 : Convert.ToInt32(p);
            TempData["CurrentPage_Pending"] = currPage_Pending;

            //  var pending = context.View_RopesPendingInspection_Details.OrderByDescending(u => u.InspectionDueDate);
            var pending = results.Where(x => x.InspectionDueDate >= DateTime.Today).ToList();
            TempData["TotalRecords_Pending"] = pending.Count();

            RopeSummaryReport ropeSummary = new RopeSummaryReport
            {
                //var llis = overdue.OrderByDescending(x => x.InspectDate).Skip((CommonMethods.PAGESIZE * currPage_Overdue) - CommonMethods.PAGESIZE).Take(CommonMethods.PAGESIZE).ToList();
                //PendingInspectionList
                OverdueInspectionList = overdue.OrderByDescending(x => x.InspectionDueDate).Skip((CommonMethods.PAGESIZE * currPage_Overdue) - CommonMethods.PAGESIZE).Take(CommonMethods.PAGESIZE).ToList(),
                PendingInspectionList = pending.OrderByDescending(x => x.InspectionDueDate).Skip((CommonMethods.PAGESIZE * currPage_Pending) - CommonMethods.PAGESIZE).Take(CommonMethods.PAGESIZE).ToList()
            };

            return View(ropeSummary);
        }

        public ActionResult AbrasionRating()
        {
            var results = context.View_AbrasionDetails.ToList();
            return View(results);
        }

        public ActionResult RopesRequiringDiscard()
        {
            DataTable RopeDiscard = CommonMethods.ExecStoredProceedure("spRopesRequiringDiscard_Details");
            DataTable TailDiscard = CommonMethods.ExecStoredProceedure("spTailsRequiringDiscard_Details");

            RopeSummaryReport ropeDisSummary = new RopeSummaryReport
            {
                RopeDiscardDetails = GetUpcomingList(RopeDiscard, 0),
                TailDiscardDetails = GetUpcomingList(TailDiscard, 0)
            };

            //RopeSummaryReport ropeDisSummary = new RopeSummaryReport
            //{
            ////     DataTable UpcomingRopeDiscard = CommonMethods.ExecStoredProceedure("SpUpcomingRopeDiscard");
            ////DataTable UpcomingTailDiscard = CommonMethods.ExecStoredProceedure("SpUpcomingTailDiscard");
            //RopeDiscardDetails = context.spRopesRequiringDiscard_Details().ToList(),
            //    TailDiscardDetails = context.spTailsRequiringDiscard_Details().ToList()
            //};

            RopeTypeWiseBarChart(ropeDisSummary, true);
            ManufactureWiseBarChart(ropeDisSummary, true);
            VesselsBarChart(ropeDisSummary, true);

            return View(ropeDisSummary);
        }
        public void RopeTypeWiseBarChart(RopeSummaryReport ropeDisSummary, bool RequiredDiscard)
        {
            //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%% Barchart Rope-Type wise %%%%%%%%%%%%%%%%%%%%%%%%%%%%%
            var RopeTypeArray = context.MooringRopeTypes.Select(x => x.RopeType).ToArray();
            List<AnomaliesChartData> listCombinedAnomalies = new List<AnomaliesChartData>();
            string Action = "";
            string Lables = "";
            foreach (var item in RopeTypeArray)
            {
                Lables = Lables + item + ",";
                if (RequiredDiscard == true)
                {
                    Action = "Requiring";
                    AnomaliesChartData chartdata = new AnomaliesChartData()
                    {
                        labelName = item.ToString(),
                        value1 = ropeDisSummary.RopeDiscardDetails.Where(x => x.RopeType == item.ToString()).Count(),//table.Rows.Count,
                        value2 = ropeDisSummary.TailDiscardDetails.Where(x => x.RopeType == item.ToString()).Count()
                    };

                    listCombinedAnomalies.Add(chartdata);
                }
                else
                {
                    Action = "Upcoming";
                    AnomaliesChartData chartdata = new AnomaliesChartData()
                    {
                        labelName = item.ToString(),
                        value1 = ropeDisSummary.UpcomingRopeDiscard_Detail.Where(x => x.RopeType == item.ToString()).Count(),//table.Rows.Count,
                        value2 = ropeDisSummary.UpcomingTailDiscard_Detail.Where(x => x.RopeType == item.ToString()).Count()
                    };

                    listCombinedAnomalies.Add(chartdata);
                }
            }


            int a1 = listCombinedAnomalies.Max(x => x.value1);
            int b1 = listCombinedAnomalies.Max(x => x.value2);
            int geater = a1 > b1 == true ? a1 : b1;

            int mv = listCombinedAnomalies.Count > 0 ? geater : 0;
            int mv2 = mv > 0 ? Convert.ToInt32(mv / 20) * 20 + 40 : mv;


            var ChData = new GraphData() { chartId = mv2, data = RopeAnalysis.GetChartDataCom(listCombinedAnomalies, "Lines", "Tails") };
            ViewBag.RopeTailsDiscardedCharts_RTW = ChData;
            ViewBag.ChartLables_RTW = Lables;
            ViewBag.TitleHead_RTW = "Lines and Rope-Tails " + Action + " Discard (Rope Type Wise) as of Date " + DateTime.Now.ToString("dd-MMM-yyyy");
            //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%% Barchart Rope-Type wise %%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        }

        public void ManufactureWiseBarChart(RopeSummaryReport ropeDisSummary, bool RequiredDiscard)
        {
            //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%% Barchart Rope-Type wise %%%%%%%%%%%%%%%%%%%%%%%%%%%%%
            var ManufactureArray = context.tblCommons.Where(y => y.Type == 1).Select(x => x.Name).ToArray();
            List<AnomaliesChartData> listCombinedAnomalies = new List<AnomaliesChartData>();
            string Action = ""; string Lables = "";
            foreach (var item in ManufactureArray)
            {
                Lables = Lables + item + ",";
                if (RequiredDiscard == true)
                {
                    Action = "Requiring";
                    AnomaliesChartData chartdata = new AnomaliesChartData()
                    {
                        labelName = item.ToString(),
                        value1 = ropeDisSummary.RopeDiscardDetails.Where(x => x.Name == item.ToString()).Count(),//table.Rows.Count,
                        value2 = ropeDisSummary.TailDiscardDetails.Where(x => x.Name == item.ToString()).Count()
                    };

                    listCombinedAnomalies.Add(chartdata);
                }
                else
                {
                    Action = "Upcoming";
                    AnomaliesChartData chartdata = new AnomaliesChartData()
                    {
                        labelName = item.ToString(),
                        value1 = ropeDisSummary.UpcomingRopeDiscard_Detail.Where(x => x.Name == item.ToString()).Count(),//table.Rows.Count,
                        value2 = ropeDisSummary.UpcomingTailDiscard_Detail.Where(x => x.Name == item.ToString()).Count()
                    };

                    listCombinedAnomalies.Add(chartdata);
                }
            }


            int a1 = listCombinedAnomalies.Max(x => x.value1);
            int b1 = listCombinedAnomalies.Max(x => x.value2);
            int geater = a1 > b1 == true ? a1 : b1;

            int mv = listCombinedAnomalies.Count > 0 ? geater : 0;
            int mv2 = mv > 0 ? Convert.ToInt32(mv / 20) * 20 + 40 : mv;


            var ChData = new GraphData() { chartId = mv2, data = RopeAnalysis.GetChartDataCom(listCombinedAnomalies, "Lines", "Tails") };
            ViewBag.RopeTailsDiscardedCharts_MW = ChData;
            ViewBag.ChartLables_MW = Lables;
            ViewBag.TitleHead_MW = "Lines and Rope-Tails " + Action + " Discard (Manufacturer Wise) as of Date " + DateTime.Now.ToString("dd-MMM-yyyy");
            //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%% Barchart Rope-Type wise %%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        }

        public void VesselsBarChart(RopeSummaryReport ropeDisSummary, bool RequiredDiscard)
        {
            //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%% Barchart Rope-Type wise %%%%%%%%%%%%%%%%%%%%%%%%%%%%%
            var VesselsArray = context.VesselDetails.Select(x => x.VesselName).ToArray();
            List<AnomaliesChartData> listCombinedAnomalies = new List<AnomaliesChartData>();
            string Action = ""; string Lables = "";
            foreach (var item in VesselsArray)
            {
                Lables = Lables + item + ",";
                if (RequiredDiscard == true)
                {
                    Action = "Requiring";
                    AnomaliesChartData chartdata = new AnomaliesChartData()
                    {
                        labelName = item.ToString(),
                        value1 = ropeDisSummary.RopeDiscardDetails.Where(x => x.VesselName == item.ToString()).Count(),//table.Rows.Count,
                        value2 = ropeDisSummary.TailDiscardDetails.Where(x => x.VesselName == item.ToString()).Count()
                    };

                    listCombinedAnomalies.Add(chartdata);
                }
                else
                {
                    Action = "Upcoming";
                    AnomaliesChartData chartdata = new AnomaliesChartData()
                    {
                        labelName = item.ToString(),
                        value1 = ropeDisSummary.UpcomingRopeDiscard_Detail.Where(x => x.VesselName == item.ToString()).Count(),//table.Rows.Count,
                        value2 = ropeDisSummary.UpcomingTailDiscard_Detail.Where(x => x.VesselName == item.ToString()).Count()
                    };

                    listCombinedAnomalies.Add(chartdata);
                }
            }


            int a1 = listCombinedAnomalies.Max(x => x.value1);
            int b1 = listCombinedAnomalies.Max(x => x.value2);
            int geater = a1 > b1 == true ? a1 : b1;

            int mv = listCombinedAnomalies.Count > 0 ? geater : 0;
            int mv2 = mv > 0 ? Convert.ToInt32(mv / 20) * 20 + 40 : mv;


            var ChData = new GraphData() { chartId = mv2, data = RopeAnalysis.GetChartDataCom(listCombinedAnomalies, "Lines", "Tails") };
            ViewBag.RopeTailsDiscardedCharts_VW = ChData;
            ViewBag.ChartLables_VW = Lables;
            ViewBag.TitleHead_VW = "Lines and Rope-Tails " + Action + " Discard (Vessel Wise) as of Date " + DateTime.Now.ToString("dd-MMM-yyyy");
            //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%% Barchart Rope-Type wise %%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        }
        private void InitializeAnalysisFilterModel()
        {
            analysisFilterModel = new ReportAnalysisFilterModel
            {
                FleetNameList = base.PermittedFleetNames,
                FleetTypeList = base.PermittedFleetTypes,

                AgeRangeFrom = 0,
                AgeRangeTo = 50,

                WindSpeedRangeFrom = 0,
                WindSpeedRangeTo = 200,

                CurrentSpeedRangeFrom = 0,
                CurrentSpeedRangeTo = 50,

                AirTempRangeFrom = -50,
                AirTempRangeTo = 60,

                TidalRangeFrom = 0,
                TidalRangeTo = 30,

                //Squal_Gusts="Yes",
                //SurgingObserved = "Yes",
                //BerthExposesToSwell = "Yes",
                //TrafficPassingEffect = "Yes",
                //ShipFenderContact = "No",


            };
        }
        public ActionResult Anomalies(int? id)
        {
            InitializeAnalysisFilterModel();


            analysisFilterModel.DateDiscardedFrom = DateTime.Today.AddYears(-1);
            analysisFilterModel.DateDiscardedUpto = DateTime.Today;
            //analysisFilterModel.OperationResultList

            //var MooringOP_Detail = context.View_VesselWiseMooringOP_Detail.Where(p => p.FastDatetime >= analysisFilterModel.DateDiscardedFrom && p.FastDatetime <= analysisFilterModel.DateDiscardedUpto).ToList();

            //MooringOP_Detail = MooringOP_Detail.GroupBy(x=> new { x.VesselName,x.PortName,x.FacilityName, x .BirthName, x .BirthType, x .FastDatetime,x.RangOfTide , x.CurrentSpeed, x.WindSpeed, x.AirTemprature, x.AnySquall, x.SurgingObserved, x.Berth_exposed_SeaSwell, x.Any_Affect_Passing_Traffic, x.Ship_was_continuously_contact_with_fender,x.OPId, x.ImoNo }).
            //    Select(s=> new View_VesselWiseMooringOP_Detail
            //    {
            //        OPId=s.Key.OPId,
            //        ImoNo=s.Key.ImoNo,
            //       VesselName= s.Key.VesselName,
            //        PortName= s.Key.PortName,
            //        FacilityName= s.Key.FacilityName,
            //        BirthName = s.Key.BirthName, 
            //        BirthType = s.Key.BirthType,
            //        FastDatetime = s.Key.FastDatetime,
            //        RangOfTide = s.Key.RangOfTide,
            //        CurrentSpeed = s.Key.CurrentSpeed,
            //        WindSpeed = s.Key.WindSpeed,
            //        AirTemprature = s.Key.AirTemprature,
            //        AnySquall = s.Key.AnySquall,
            //        SurgingObserved = s.Key.SurgingObserved,
            //        Berth_exposed_SeaSwell = s.Key.Berth_exposed_SeaSwell,
            //        Any_Affect_Passing_Traffic= s.Key.Any_Affect_Passing_Traffic,
            //        Ship_was_continuously_contact_with_fender = s.Key.Ship_was_continuously_contact_with_fender


            //        }).Distinct().OrderBy(u => u.VesselName).ToList();

            // return await TaskEx.Run(() => { return FillData(sql, connectionName); });

            DataTable dt = CommonMethods.ExecStoredProceedureBetweenDates("SP_VesselWiseMooringOP_Detail", analysisFilterModel.DateDiscardedFrom, analysisFilterModel.DateDiscardedUpto);

            List<View_VesselWiseMooringOP_Detail2> records = new List<View_VesselWiseMooringOP_Detail2>();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                records.Add(new View_VesselWiseMooringOP_Detail2
                {
                    OPId = Convert.ToInt32(dt.Rows[i]["OPId"]),
                    ImoNo = Convert.ToInt32(dt.Rows[i]["ImoNo"]),
                    VesselName = dt.Rows[i]["VesselName"].ToString(),
                    PortName = dt.Rows[i]["PortName"].ToString(),
                    FacilityName = dt.Rows[i]["FacilityName"].ToString(),
                    BirthName = dt.Rows[i]["BirthName"].ToString(),
                    BirthType = dt.Rows[i]["BirthType"].ToString(),
                    FastDatetime = Convert.ToDateTime(dt.Rows[i]["FastDatetime"]),
                    RangOfTide = Convert.ToDecimal(dt.Rows[i]["RangOfTide"]),
                    CurrentSpeed = Convert.ToDecimal(dt.Rows[i]["CurrentSpeed"]),
                    WindSpeed = Convert.ToInt32(dt.Rows[i]["WindSpeed"]),
                    AirTemprature = Convert.ToInt32(dt.Rows[i]["AirTemprature"]),
                    AnySquall = dt.Rows[i]["AnySquall"].ToString(),
                    SurgingObserved = dt.Rows[i]["SurgingObserved"].ToString(),
                    Berth_exposed_SeaSwell = dt.Rows[i]["Berth_exposed_SeaSwell"].ToString(),
                    Any_Affect_Passing_Traffic = dt.Rows[i]["Any_Affect_Passing_Traffic"].ToString(),
                    Ship_was_continuously_contact_with_fender = dt.Rows[i]["Ship_was_continuously_contact_with_fender"].ToString(),
                    Any_Rope_Damaged = dt.Rows[i]["Any_Rope_Damaged"].ToString(),

                });
            }

            /*
            List<int> analysis = new List<int>()
            {
                records.Where(x=>x.RangOfTide>3.0m).Count(),
                 records.Where(x=>x.CurrentSpeed> 0.5m).Count(),
                  records.Where(x=>x.WindSpeed>20).Count(),
                   records.Where(x=>x.AirTemprature<10).Count() + records.Where(x=>x.AirTemprature>40).Count(),

                 records.Where(x=>x.Any_Rope_Damaged=="Yes").Count(),
                 records.Where(x=>x.AnySquall=="Yes").Count(),
                  records.Where(x=>x.SurgingObserved=="Yes").Count(),
                   records.Where(x=>x.Berth_exposed_SeaSwell=="Yes").Count(),
                    records.Where(x=>x.Any_Affect_Passing_Traffic=="Yes").Count(),
                     records.Where(x=>x.Ship_was_continuously_contact_with_fender=="No").Count(),

            };
            */
            //value2 = records.Where(x=>x.RangOfTide>3.0m).GroupBy(p => new {p.FacilityName, p.PortName }).Select(u => new View_VesselWiseMooringOP_Detail { FacilityName = u.Key.FacilityName, PortName = u.Key.PortName }).Count(),
            List<AnomaliesChartData> listCombinedAnomalies = CommonMethods.GetAnomaliesChartList(records);


            //int mv = analysis.Count > 0 ? analysis.Max() : 0;
            //int mv2 = mv > 0 ? Convert.ToInt32(mv / 20) * 20 + 40 : mv;
            //var ChData = new GraphData() { chartId = mv2, data = GetChartDataMOP(analysis, "No of Times reported") };
            //ViewBag.MooringOpGraph = ChData;


            int a1 = listCombinedAnomalies.Max(x => x.value1);
            int b1 = listCombinedAnomalies.Max(x => x.value2);
            int geater = a1 > b1 == true ? a1 : b1;

            int mv = listCombinedAnomalies.Count > 0 ? geater : 0;
            int mv2 = mv;// > 0 ? Convert.ToInt32(mv / 20) * 20 + 20 : mv;
            var ChData = new GraphData() { chartId = mv2, data = RopeAnalysis.GetChartDataCom(listCombinedAnomalies, "No of Times reported", "No of Ports reported") };
            ViewBag.CombinedAnomalies = ChData;
            ViewBag.AnomaliesTitle = "Date from " + analysisFilterModel.DateDiscardedFrom.ToString("dd-MMM-yyyy") + " To " + analysisFilterModel.DateDiscardedUpto.ToString("dd-MMM-yyyy");

            int currPage = id == null ? 1 : Convert.ToInt32(id);

            TempData["CurrentPage"] = currPage;
            TempData["TotalRecords"] = records.Count();

            analysisFilterModel.VesselWiseMooringOP_Details = records.Skip((CommonMethods.PAGESIZE * currPage) - CommonMethods.PAGESIZE).Take(CommonMethods.PAGESIZE).ToList();


            return View(analysisFilterModel);


        }

        [HttpPost]
        public ActionResult Anomalies(int? id, ReportAnalysisFilterModel analysisFilterModel)
        {
            //  InitializeAnalysisFilterModel();

            DataTable dt = CommonMethods.ExecStoredProceedureBetweenDates("SP_View_VesselWiseMooringOP_Detail", analysisFilterModel.DateDiscardedFrom, analysisFilterModel.DateDiscardedUpto);

            List<View_VesselWiseMooringOP_Detail2> vesselWiseExpAnalysis = new List<View_VesselWiseMooringOP_Detail2>();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                vesselWiseExpAnalysis.Add(new View_VesselWiseMooringOP_Detail2
                {
                    OPId = Convert.ToInt32(dt.Rows[i]["OPId"]),
                    ImoNo = Convert.ToInt32(dt.Rows[i]["ImoNo"]),
                    VesselName = dt.Rows[i]["VesselName"].ToString(),
                    PortName = dt.Rows[i]["PortName"].ToString(),
                    FacilityName = dt.Rows[i]["FacilityName"].ToString(),
                    BirthName = dt.Rows[i]["BirthName"].ToString(),
                    BirthType = dt.Rows[i]["BirthType"].ToString(),
                    FastDatetime = Convert.ToDateTime(dt.Rows[i]["FastDatetime"]),
                    RangOfTide = Convert.ToDecimal(dt.Rows[i]["RangOfTide"]),
                    CurrentSpeed = Convert.ToDecimal(dt.Rows[i]["CurrentSpeed"]),
                    WindSpeed = Convert.ToInt32(dt.Rows[i]["WindSpeed"]),
                    AirTemprature = Convert.ToInt32(dt.Rows[i]["AirTemprature"]),
                    AnySquall = dt.Rows[i]["AnySquall"].ToString(),
                    SurgingObserved = dt.Rows[i]["SurgingObserved"].ToString(),
                    Berth_exposed_SeaSwell = dt.Rows[i]["Berth_exposed_SeaSwell"].ToString(),
                    Any_Affect_Passing_Traffic = dt.Rows[i]["Any_Affect_Passing_Traffic"].ToString(),
                    Ship_was_continuously_contact_with_fender = dt.Rows[i]["Ship_was_continuously_contact_with_fender"].ToString(),
                    Any_Rope_Damaged = dt.Rows[i]["Any_Rope_Damaged"].ToString(),

                });
            }


         //   var vesselWiseExpAnalysis = context.View_VesselWiseMooringOP_Detail.
                //Where(p => p.FastDatetime >= analysisFilterModel.DateDiscardedFrom && p.FastDatetime <= analysisFilterModel.DateDiscardedUpto).
                //AsQueryable();

            if (analysisFilterModel.VesselIDs?.Count > 0)
                vesselWiseExpAnalysis = vesselWiseExpAnalysis.Where(p => analysisFilterModel.VesselIDs.Contains(p.ImoNo)).ToList();

            if (analysisFilterModel.FleetNameIDs?.Count > 0)
                vesselWiseExpAnalysis = vesselWiseExpAnalysis.Where(p => analysisFilterModel.FleetNameIDs.Contains(p.FleetNameID)).ToList();

            if (analysisFilterModel.FleetTypeIDs?.Count > 0)
                vesselWiseExpAnalysis = vesselWiseExpAnalysis.Where(p => analysisFilterModel.FleetTypeIDs.Contains(p.FleetTypeID)).ToList();

            if (analysisFilterModel.TradeIDs?.Count > 0)
                vesselWiseExpAnalysis = vesselWiseExpAnalysis.Where(p => analysisFilterModel.TradeIDs.Contains(p.TradeAreaID)).ToList();

            if (!string.IsNullOrEmpty(analysisFilterModel.PortNames))
                vesselWiseExpAnalysis = vesselWiseExpAnalysis.Where(p => analysisFilterModel.PortNames.Contains(p.PortName)).ToList();

            analysisFilterModel.PortFacilityNames = analysisFilterModel.PortFacilityNames == "None Selected" ? null : analysisFilterModel.PortFacilityNames;

            if (!string.IsNullOrEmpty(analysisFilterModel.PortFacilityNames))
                vesselWiseExpAnalysis = vesselWiseExpAnalysis.Where(p => analysisFilterModel.PortFacilityNames.Contains(p.FacilityName)).ToList();

            if (!string.IsNullOrEmpty(analysisFilterModel.BirthTypeNames))
                vesselWiseExpAnalysis = vesselWiseExpAnalysis.Where(p => p.BirthType == analysisFilterModel.BirthTypeNames).ToList();

            if (!string.IsNullOrEmpty(analysisFilterModel.MooringTypeNames))
                vesselWiseExpAnalysis = vesselWiseExpAnalysis.Where(p => p.MooringType == analysisFilterModel.MooringTypeNames).ToList();

            var kk = vesselWiseExpAnalysis.ToList();

            if (!analysisFilterModel.Squal_Gusts.ToUpper().Contains("ALL"))
                vesselWiseExpAnalysis = vesselWiseExpAnalysis.Where(p => p.AnySquall == analysisFilterModel.Squal_Gusts).ToList();

            if (!analysisFilterModel.BerthExposesToSwell.ToUpper().Contains("ALL"))
                vesselWiseExpAnalysis = vesselWiseExpAnalysis.Where(p => p.Berth_exposed_SeaSwell == analysisFilterModel.BerthExposesToSwell).ToList();

            if (!analysisFilterModel.SurgingObserved.ToUpper().Contains("ALL"))
                vesselWiseExpAnalysis = vesselWiseExpAnalysis.Where(p => p.SurgingObserved == analysisFilterModel.SurgingObserved).ToList();

            if (!analysisFilterModel.TrafficPassingEffect.ToUpper().Contains("ALL"))
                vesselWiseExpAnalysis = vesselWiseExpAnalysis.Where(p => p.Any_Affect_Passing_Traffic == analysisFilterModel.TrafficPassingEffect).ToList();

            if (!analysisFilterModel.ShipFenderContact.ToUpper().Contains("ALL"))
                vesselWiseExpAnalysis = vesselWiseExpAnalysis.Where(p => p.Ship_was_continuously_contact_with_fender == analysisFilterModel.ShipFenderContact).ToList();

            if (!analysisFilterModel.RopeDamagedAnytime.ToUpper().Contains("ALL"))
                vesselWiseExpAnalysis = vesselWiseExpAnalysis.Where(p => p.Any_Rope_Damaged == analysisFilterModel.RopeDamagedAnytime).ToList();

            var kk2 = vesselWiseExpAnalysis.ToList();

            DateTime builtFrom = DateTime.Today.AddYears(-analysisFilterModel.AgeRangeFrom);
            DateTime builtUpto = DateTime.Today.AddYears(-analysisFilterModel.AgeRangeTo);


            vesselWiseExpAnalysis = vesselWiseExpAnalysis.Where(p => p.DateBuilt <= builtFrom && p.DateBuilt >= builtUpto).ToList();
            //vesselWiseExpAnalysis = vesselWiseExpAnalysis.Where(p => p.DateBuilt >= builtUpto);
            var kk3 = vesselWiseExpAnalysis.ToList();


            vesselWiseExpAnalysis = vesselWiseExpAnalysis.Where(p => p.WindSpeed >= analysisFilterModel.WindSpeedRangeFrom && p.WindSpeed <= analysisFilterModel.WindSpeedRangeTo).ToList();
            // vesselWiseExpAnalysis = vesselWiseExpAnalysis.Where(p => p.WindSpeed <= analysisFilterModel.WindSpeedRangeTo);

            vesselWiseExpAnalysis = vesselWiseExpAnalysis.Where(p => p.CurrentSpeed >= analysisFilterModel.CurrentSpeedRangeFrom && p.CurrentSpeed <= analysisFilterModel.CurrentSpeedRangeTo).ToList();
            // vesselWiseExpAnalysis = vesselWiseExpAnalysis.Where(p => p.CurrentSpeed <= analysisFilterModel.CurrentSpeedRangeTo);

            vesselWiseExpAnalysis = vesselWiseExpAnalysis.Where(p => p.AirTemprature >= analysisFilterModel.AirTempRangeFrom && p.AirTemprature <= analysisFilterModel.AirTempRangeTo).ToList();
            // vesselWiseExpAnalysis = vesselWiseExpAnalysis.Where(p => p.AirTemprature <= analysisFilterModel.AirTempRangeTo);

            vesselWiseExpAnalysis = vesselWiseExpAnalysis.Where(p => p.RangOfTide >= analysisFilterModel.TidalRangeFrom && p.RangOfTide <= analysisFilterModel.TidalRangeTo).ToList();
            //vesselWiseExpAnalysis = vesselWiseExpAnalysis.Where(p => p.RangOfTide <= analysisFilterModel.TidalRangeTo);
            var kk4 = vesselWiseExpAnalysis.ToList();

            //var results = vesselWiseExpAnalysis.GroupBy(p => new { p.ImoNo, p.RopeType, p.RopeTypeId, p.ManufacturerId, p.Manufacturer, p.VesselName }).
            //    Select(u => new AnalysisResult
            //    {
            //        ImoNo = u.Key.ImoNo,
            //        RopeType = u.Key.RopeType,
            //        RopeTypeId = u.Key.RopeTypeId,
            //        ManufacturerId = u.Key.ManufacturerId,
            //        Manufacturer = u.Key.Manufacturer,
            //        VesselName = u.Key.VesselName,
            //        Cost = u.Average(p => p.Cost),
            //        RunningHours = (int?)u.Average(p => p.RunningHours),
            //        Avg_Months = u.Average(p => p.Avg_Months)
            //    });



            analysisFilterModel.FleetNameList = base.PermittedFleetNames;
            analysisFilterModel.FleetTypeList = base.PermittedFleetTypes;

            analysisFilterModel.VesselList = context.VesselDetails.Where(p => analysisFilterModel.FleetTypeIDs.Contains(p.FleetTypeID) && analysisFilterModel.FleetNameIDs.Contains(p.FleetNameID) && analysisFilterModel.TradeIDs.Contains(p.TradeAreaID)).ToList();

            //List<int> analysis = new List<int>()
            //{
            //     vesselWiseExpAnalysis.Where(x=>x.RangOfTide>3.0m).Count(),
            //     vesselWiseExpAnalysis.Where(x=>x.CurrentSpeed> 0.5m).Count(),
            //      vesselWiseExpAnalysis.Where(x=>x.WindSpeed>20).Count(),
            //       vesselWiseExpAnalysis.Where(x=>x.AirTemprature<10).Count() + vesselWiseExpAnalysis.Where(x=>x.AirTemprature>40).Count(),

            //    vesselWiseExpAnalysis.Where(x=>x.Any_Rope_Damaged=="Yes").Count(),
            //     vesselWiseExpAnalysis.Where(x=>x.AnySquall=="Yes").Count(),
            //      vesselWiseExpAnalysis.Where(x=>x.SurgingObserved=="Yes").Count(),
            //       vesselWiseExpAnalysis.Where(x=>x.Berth_exposed_SeaSwell=="Yes").Count(),
            //        vesselWiseExpAnalysis.Where(x=>x.Any_Affect_Passing_Traffic=="Yes").Count(),
            //         vesselWiseExpAnalysis.Where(x=>x.Ship_was_continuously_contact_with_fender=="No").Count(),

            //};



            //var chdata = GetChartDataMOP(analysis, 1);
            //int mv = analysis.Count > 0 ? analysis.Max() : 0;
            //int mv2 = mv > 0 ? Convert.ToInt32(mv / 20) * 20 + 40 : mv;
            //var ChData = new GraphData() { chartId = mv2, data = GetChartDataMOP(analysis, "No of Times reported") };
            //ViewBag.MooringOpGraph = ChData;

            List<AnomaliesChartData> listCombinedAnomalies = CommonMethods.GetAnomaliesChartList(vesselWiseExpAnalysis.ToList());


            //int mv = analysis.Count > 0 ? analysis.Max() : 0;
            //int mv2 = mv > 0 ? Convert.ToInt32(mv / 20) * 20 + 40 : mv;
            //var ChData = new GraphData() { chartId = mv2, data = GetChartDataMOP(analysis, "No of Times reported") };
            //ViewBag.MooringOpGraph = ChData;


            int a1 = listCombinedAnomalies.Max(x => x.value1);
            int b1 = listCombinedAnomalies.Max(x => x.value2);
            int geater = a1 > b1 == true ? a1 : b1;

            int mv = listCombinedAnomalies.Count > 0 ? geater : 0;
            int mv2 = mv;// > 0 ? Convert.ToInt32(mv / 20) * 20 + 20 : mv;
            var ChData = new GraphData() { chartId = mv2, data = RopeAnalysis.GetChartDataCom(listCombinedAnomalies, "No of Times reported", "No of Ports reported") };
            ViewBag.CombinedAnomalies = ChData;
            ViewBag.AnomaliesTitle = "Date from " + analysisFilterModel.DateDiscardedFrom.ToString("dd-MMM-yyyy") + " To " + analysisFilterModel.DateDiscardedUpto.ToString("dd-MMM-yyyy");


            int currPage = id == null ? 1 : Convert.ToInt32(id);

            TempData["CurrentPage"] = currPage;
            TempData["TotalRecords"] = vesselWiseExpAnalysis.Count();

            analysisFilterModel.VesselWiseMooringOP_Details = vesselWiseExpAnalysis.OrderByDescending(x => x.FastDatetime).Skip((CommonMethods.PAGESIZE * currPage) - CommonMethods.PAGESIZE).Take(CommonMethods.PAGESIZE).ToList();

            // vesselWiseExpAnalysis.Skip((CommonMethods.PAGESIZE * currPage) - CommonMethods.PAGESIZE).Take(CommonMethods.PAGESIZE).ToList();


            return View(analysisFilterModel);


        }

        public static string GetChartDataMOP(List<int> analysisData, string Lable1)
        {
            List<ChartDetails> lstGraphData_1 = new List<ChartDetails>
            {
                GetAnomaliesGraphs(analysisData,true, Lable1, System.Drawing.Color.Navy),
                //GetAnomaliesGraphs(analysisData,false, rating,  System.Drawing.Color.LightGreen)
            };

            return JsonConvert.SerializeObject(lstGraphData_1);
        }

        public ActionResult UpComingDiscard()
        {
            DataTable UpcomingRopeDiscard = CommonMethods.ExecStoredProceedure("SpUpcomingRopeDiscard");
            DataTable UpcomingTailDiscard = CommonMethods.ExecStoredProceedure("SpUpcomingTailDiscard");

            RopeSummaryReport ropeDisSummary = new RopeSummaryReport
            {
                UpcomingRopeDiscard_Detail = GetUpcomingList(UpcomingRopeDiscard, 1),
                UpcomingTailDiscard_Detail = GetUpcomingList(UpcomingTailDiscard, 1)
            };


            /*
            var RopeTypeArray = context.MooringRopeTypes.Select(x => x.RopeType).ToArray();
            List<AnomaliesChartData> listCombinedAnomalies = new List<AnomaliesChartData>();
            string Lables = "";
            foreach (var item in RopeTypeArray)
            {
                Lables = Lables + item + ",";
                AnomaliesChartData chartdata = new AnomaliesChartData()
                {
                    labelName = item.ToString(),
                    value1 = ropeDisSummary.UpcomingRopeDiscard_Detail.Where(x => x.RopeType == item.ToString()).Count(),//table.Rows.Count,
                    value2 = ropeDisSummary.UpcomingTailDiscard_Detail.Where(x => x.RopeType == item.ToString()).Count()
                };

                listCombinedAnomalies.Add(chartdata);
            }

            int a1 = listCombinedAnomalies.Max(x => x.value1);
            int b1 = listCombinedAnomalies.Max(x => x.value2);
            int geater = a1 > b1 == true ? a1 : b1;

            int mv = listCombinedAnomalies.Count > 0 ? geater : 0;
            int mv2 = mv > 0 ? Convert.ToInt32(mv / 20) * 20 + 40 : mv;
            //var ChData = new GraphData() { chartId = mv2, data = GetChartDataMOP(analysis, "Vessels") };

            var ChData = new GraphData() { chartId = mv2, data = RopeAnalysis.GetChartDataCom(listCombinedAnomalies, "Lines", "Tails") };
            ViewBag.RopeTailsDiscardedCharts = ChData;
            ViewBag.ChartLables = Lables.TrimEnd(',');
            */

            RopeTypeWiseBarChart(ropeDisSummary, false);
            ManufactureWiseBarChart(ropeDisSummary, false);
            VesselsBarChart(ropeDisSummary, false);

            return View(ropeDisSummary);
        }

        public ActionResult End2EndDetail()
        {
            DataSet End2EndResult = CommonMethods.ExecStoredProceedureWithDataSet("EndtoEndUpcomingNDue");
            RopeSummaryReport ropeDisSummary = new RopeSummaryReport
            {
                End2EndUpcoming_Detail = GetEnd2EndList(End2EndResult.Tables[0]),
                End2EndDue_Detail = GetEnd2EndList(End2EndResult.Tables[1])
            };

            //var result = numbers.GroupBy(n => n).Select(c => new { Key = c.Key, total = c.Count() });

            //  var Countupcoming = ropeDisSummary.End2EndUpcoming_Detail.GroupBy(n => n.VesselName).Select(c => new { Key = c.Key, total = c.Count() });
            var CountOverDue = ropeDisSummary.End2EndDue_Detail.GroupBy(n => n.VesselName).Select(c => new { Key = c.Key, total = c.Count() });
            List<int> analysis = new List<int>(); string Lables = "";
            foreach (var item in CountOverDue)
            {
                Lables = Lables + item.Key + ",";
                analysis.Add(item.total);
            }

            int mv = analysis.Count > 0 ? analysis.Max() : 0;
            int mv2 = mv > 0 ? Convert.ToInt32(mv / 20) * 20 + 40 : mv;
            var ChData = new GraphData() { chartId = mv2, data = GetChartDataMOP(analysis, "Vessels") };
            ViewBag.End2EndWinch = ChData;
            ViewBag.ChartLables = Lables.TrimEnd(',');
            return View(ropeDisSummary);
        }

        public ActionResult WinchRotationDetail()
        {
            RopeSummaryReport ropeDisSummary = new RopeSummaryReport();


            // CommonMethods.AllWinchRotationList = CommonMethods.FindWinchRotation();

            ropeDisSummary.WinchRotationUpcoming_Detail = CommonMethods.AllWinchRotationList.Where(x => x.StatusUpDue == "Upcoming").ToList(); // ApprochingCount;
            ropeDisSummary.WinchRotationDue_Detail = CommonMethods.AllWinchRotationList.Where(x => x.StatusUpDue == "Overdue").ToList(); //ExceedCount;
            var CountOverDue = ropeDisSummary.WinchRotationDue_Detail.GroupBy(n => n.VesselName).Select(c => new { Key = c.Key, total = c.Count() });
            List<int> analysis = new List<int>(); string Lables = "";
            foreach (var item in CountOverDue)
            {
                Lables = Lables + item.Key + ",";
                analysis.Add(item.total);
            }

            int mv = analysis.Count > 0 ? analysis.Max() : 0;
            int mv2 = mv > 0 ? Convert.ToInt32(mv / 20) * 20 + 40 : mv;
            var ChData = new GraphData() { chartId = mv2, data = GetChartDataMOP(analysis, "Vessels") };
            ViewBag.End2EndWinch = ChData;
            ViewBag.ChartLables = Lables.TrimEnd(',');


            return View(ropeDisSummary);


        }


        private List<SpEndtoEndUpcomingNDue_Result> GetEnd2EndList(DataTable table)
        {
            List<SpEndtoEndUpcomingNDue_Result> SpEnd2End_list = new List<SpEndtoEndUpcomingNDue_Result>();
            foreach (DataRow dataRow in table.Rows)
            {
                Reports.SpEndtoEndUpcomingNDue_Result data = new Reports.SpEndtoEndUpcomingNDue_Result
                {

                    VesselName = dataRow["VesselName"].ToString(),
                    Name = dataRow["Name"].ToString(),
                    RopeType = dataRow["RopeType"].ToString(),
                    CertificateNumber = dataRow["CertificateNumber"].ToString(),
                    VesselID = Convert.ToInt32(dataRow["VesselID"]),
                    EndToEndMonth = Convert.ToInt32(dataRow["EndToEndMonth"]),

                    // v.VesselName, c.Name,d.RopeType,b.CertificateNumber, a.VesselID  , ss.EndToEndMonth,End2EndUpcoming,End2EndDue
                };

                if (!string.IsNullOrEmpty(dataRow["End2EndDue"].ToString()))
                    data.End2EndDue = Convert.ToDateTime(dataRow["End2EndDue"]);
                SpEnd2End_list.Add(data);
            }

            return SpEnd2End_list;
        }
        private List<SpUpcomingRopeDiscard_Result> GetUpcomingList(DataTable table, int UpComing)
        {
            List<SpUpcomingRopeDiscard_Result> SpUpcomingDiscard_list = new List<SpUpcomingRopeDiscard_Result>();
            foreach (DataRow dataRow in table.Rows)
            {
                Reports.SpUpcomingRopeDiscard_Result data = new Reports.SpUpcomingRopeDiscard_Result
                {
                    // id = Convert.ToInt32(dataRow["id"]),
                    RopeId = Convert.ToInt32(dataRow["RopeId"]),
                    VesselName = dataRow["VesselName"].ToString(),
                    VesselID = Convert.ToInt32(dataRow["VesselID"]),
                    // ManufacturerId = Convert.ToInt32(dataRow["ManufacturerId"]),
                    CertificateNumber = dataRow["CertificateNumber"].ToString(),
                    RopeType = dataRow["RopeType"].ToString(),
                    Name = dataRow["Name"].ToString()



                };

                if (UpComing == 0)
                {
                    data.CurrentRunningHours = string.IsNullOrEmpty(dataRow["CurrentRunningHours"].ToString()) == true ? 0 : Convert.ToDecimal(dataRow["CurrentRunningHours"]);
                    data.Duration = dataRow["Duration"].ToString();
                    data.Reason = dataRow["Reason"].ToString();
                    data.CostUsd = string.IsNullOrEmpty(dataRow["CostUsd"].ToString()) == true ? 0 : Convert.ToDecimal(dataRow["CostUsd"]);
                }

                if (!string.IsNullOrEmpty(dataRow["InstalledDate"].ToString()))
                    data.InstalledDate = Convert.ToDateTime(dataRow["InstalledDate"]);
                SpUpcomingDiscard_list.Add(data);
            }

            return SpUpcomingDiscard_list;
        }

        public ActionResult RopesDiscarded()
        {
            DateTime outofservice = DateTime.Today;
            outofservice = outofservice.AddMonths(-3);

            bool? deleteStatus = false;

            var results = context.View_MooringRopeDetails.Where(p => p.OutofServiceDate != null && p.OutofServiceDate >= outofservice && p.DeleteStatus == deleteStatus).ToList();

            return View(results);
        }

        #endregion

        public JsonResult AddCost(int ropeid, decimal cost)
        {
            using (MorringOfficeEntities officeEntities = new MorringOfficeEntities())
            {
                var ropeDetail = officeEntities.MooringRopeDetails.FirstOrDefault(u => u.Id == ropeid);

                if (ropeDetail != null)
                {
                    ropeDetail.CostUsd = cost;
                    officeEntities.SaveChanges();
                }

                return Json(new { Result = true }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult AbrasionDetails(int id, int? cp)
        {
            var abrasionDetails = new AbrasionDetails();
            // Anomalies
            abrasionDetails.VesselIDs = TempData.Peek("VesselIDs") == null ? new List<int>() : (List<int>)TempData.Peek("VesselIDs");
            abrasionDetails.ManufacturerIDs = TempData.Peek("ManufacturerIDs") == null ? new List<int>() : (List<int>)TempData.Peek("ManufacturerIDs");
            abrasionDetails.RopeTypeIDs = TempData.Peek("RopeTypeIDs") == null ? new List<int>() : (List<int>)TempData.Peek("RopeTypeIDs");
            abrasionDetails.InspectionRatingIDs = TempData.Peek("InspectionRatingIDs") == null ? new List<int>() : (List<int>)TempData.Peek("InspectionRatingIDs");
            abrasionDetails.RunningHoursFrom = TempData.Peek("RunningHoursFrom") == null ? 0 : (long)TempData.Peek("RunningHoursFrom");
            abrasionDetails.RunningHoursTo = TempData.Peek("RunningHoursTo") == null ? 1000000 : (long)TempData.Peek("RunningHoursTo");

            abrasionDetails.AbrasionDetailsResult = CommonMethods.GetAbrasionDetails(abrasionDetails.VesselIDs, abrasionDetails.ManufacturerIDs,
                abrasionDetails.RopeTypeIDs, abrasionDetails.InspectionRatingIDs, abrasionDetails.RunningHoursFrom, abrasionDetails.RunningHoursTo);

            DataTable graphData = abrasionDetails.AbrasionDetailsResult.Tables[1];

            DataView dataView = graphData.AsDataView();
            dataView.RowFilter = "Rating = " + id;
            TempData["Ratings"] = id;
            var records = GetAbrasionList(dataView.ToTable());
            int currPage = cp == null ? 1 : Convert.ToInt32(cp);

            TempData["CurrentPage"] = currPage;
            TempData["TotalRecords"] = records.Count();


            var llis = records.OrderByDescending(x => x.InspectDate).Skip((CommonMethods.PAGESIZE * currPage) - CommonMethods.PAGESIZE).Take(CommonMethods.PAGESIZE).ToList();


            //return RedirectToAction("AbrasionDetails", abrasionData.AbrasionDetailsList);
            return View(llis);
        }

        private List<Abrasion_DetailsList> GetAbrasionList(DataTable dtp)
        {
            List<Abrasion_DetailsList> list = new List<Abrasion_DetailsList>();
            for (int i = 0; i < dtp.Rows.Count; i++)
            {
                list.Add(new Abrasion_DetailsList()
                {
                    RopeId = Convert.ToInt32(dtp.Rows[i]["RopeId"]),
                    VesselID = Convert.ToInt32(dtp.Rows[i]["ImoNo"]),
                    Rating = Convert.ToInt32(dtp.Rows[i]["Rating"]),
                    VesselName = dtp.Rows[i]["VesselName"].ToString(),
                    AssignedNumber = dtp.Rows[i]["AssignedNumber"].ToString(),
                    InspectBy = dtp.Rows[i]["InspectBy"].ToString(),
                    InspectDate = Convert.ToDateTime(dtp.Rows[i]["InspectDate"]),
                    Image1 = dtp.Rows[i]["Image1"].ToString(),
                    Image2 = dtp.Rows[i]["Image2"].ToString(),
                    RopeCertificateNumber = dtp.Rows[i]["CertificateNumber"].ToString(),
                });
            }
            return list;
        }

        public ActionResult AbrasionReport()
        {
            var abrasionDetails = new AbrasionDetails
            {
                FleetNames = context.tblCommons.Where(u => u.Type == (int)CommonType.FleetName).ToList(),
                FleetTypes = context.tblCommons.Where(u => u.Type == (int)CommonType.FleetType).ToList(),
                TradePlatforms = context.tblCommons.Where(u => u.Type == (int)CommonType.TradePlatform).ToList(),
                RunningHoursFrom = 0,
                RunningHoursTo = 100000,
                SelectedManufacturers = string.Empty,
                SelectedRopeTypes = string.Empty,
                SelectedVessels = string.Empty
            };

            abrasionDetails.AbrasionDetailsResult = CommonMethods.GetAbrasionDetails(abrasionDetails.VesselIDs, abrasionDetails.ManufacturerIDs,
                abrasionDetails.RopeTypeIDs, abrasionDetails.InspectionRatingIDs, abrasionDetails.RunningHoursFrom, abrasionDetails.RunningHoursTo);

            List<AbrasionData> listCombinedZones = new List<AbrasionData>();
            List<AbrasionChartData> listCombinedZonesAbrasion = new List<AbrasionChartData>();

            DataTable graphData = abrasionDetails.AbrasionDetailsResult.Tables[0];

            foreach (DataRow dataRow in graphData.Rows)
            {
                AbrasionChartData data = new AbrasionChartData
                {
                    value = Convert.ToInt32(dataRow[0]),
                    label = Convert.ToInt32(dataRow[1])
                };

                data.color = CommonMethods.GetColor(Convert.ToInt32(data.label));
                listCombinedZonesAbrasion.Add(data);
            }

            listCombinedZones.Add(new AbrasionData() { key = "All Zones", values = listCombinedZonesAbrasion });
            ViewBag.CombinedZones = JsonConvert.SerializeObject(listCombinedZones);

            return View(abrasionDetails);
        }

        [HttpPost]
        public ActionResult AbrasionReport(AbrasionDetails abrasionDetails)
        {
            abrasionDetails.FleetNames = context.tblCommons.Where(u => u.Type == (int)CommonType.FleetName).ToList();
            abrasionDetails.FleetTypes = context.tblCommons.Where(u => u.Type == (int)CommonType.FleetType).ToList();
            abrasionDetails.TradePlatforms = context.tblCommons.Where(u => u.Type == (int)CommonType.TradePlatform).ToList();

            abrasionDetails.AbrasionDetailsResult = CommonMethods.GetAbrasionDetails(abrasionDetails.VesselIDs, abrasionDetails.ManufacturerIDs,
                abrasionDetails.RopeTypeIDs, abrasionDetails.InspectionRatingIDs, abrasionDetails.RunningHoursFrom, abrasionDetails.RunningHoursTo);

            List<AbrasionData> listCombinedZones = new List<AbrasionData>();
            List<AbrasionChartData> listCombinedZonesAbrasion = new List<AbrasionChartData>();

            DataTable graphData = abrasionDetails.AbrasionDetailsResult.Tables[0];

            foreach (DataRow dataRow in graphData.Rows)
            {
                AbrasionChartData data = new AbrasionChartData
                {
                    value = Convert.ToInt32(dataRow[0]),
                    label = Convert.ToInt32(dataRow[1])
                };

                data.color = CommonMethods.GetColor(Convert.ToInt32(data.label));
                listCombinedZonesAbrasion.Add(data);
            }

            listCombinedZones.Add(new AbrasionData() { key = "All Zones", values = listCombinedZonesAbrasion });
            ViewBag.CombinedZones = JsonConvert.SerializeObject(listCombinedZones);

            TempData["VesselIDs"] = abrasionDetails.VesselIDs;
            TempData["ManufacturerIDs"] = abrasionDetails.ManufacturerIDs;
            TempData["RopeTypeIDs"] = abrasionDetails.RopeTypeIDs;
            TempData["InspectionRatingIDs"] = abrasionDetails.InspectionRatingIDs;
            TempData["RunningHoursFrom"] = abrasionDetails.RunningHoursFrom;
            TempData["RunningHoursTo"] = abrasionDetails.RunningHoursTo;

            return View(abrasionDetails);
        }


        public JsonResult GetOperationDetail(int opid, int imo)
        {
            // string start = Request.QueryString["vid"].ToString();
            // string vsid = TempData["VsId"].ToString();

            int vesselid = imo;

            var filterModel = new FilterModel
            {
                FleetNames = base.PermittedFleetNames,
                FleetTypes = base.PermittedFleetTypes,
                MooringOperationDetails = context.MOperationBirthDetails.FirstOrDefault(p => p.OPId == opid && p.VesselID == vesselid)
            };

            filterModel.MooringOperationDetails.RopeUsedInOperation = new List<View_OperationWiseRopes>();
            filterModel.MooringOperationDetails.RopeTailsUsedInOperation = new List<View_OperationWiseRopes>();

            var operations = context.View_OperationWiseRopes.Where(p => p.OperationID == opid && p.VesselID == vesselid).ToList();

            filterModel.MooringOperationDetails.RopeUsedInOperation = operations.Where(p => p.RopeTail == 0).ToList();
            filterModel.MooringOperationDetails.RopeTailsUsedInOperation = operations.Where(p => p.RopeTail == 1).ToList();
            // TempData["VsId"] = vsid;
            return Json(new { Result = true, Data = filterModel.MooringOperationDetails }, JsonRequestBehavior.AllowGet);
        }

    }


    class PieChart
    {
        public PieChart()
        {
            data = new List<int>();
            backgroundColor = new List<System.Drawing.Color>() { System.Drawing.Color.Orange, System.Drawing.Color.Green, System.Drawing.Color.LightBlue, System.Drawing.Color.LightGreen, System.Drawing.Color.LightSalmon, System.Drawing.Color.Yellow, System.Drawing.Color.Pink, System.Drawing.Color.OrangeRed, System.Drawing.Color.BlueViolet };
            labels = new List<string>() { "Mooring arrangement design", "During tightening", "During lowering", "Entagled in Jetty", "Entangled onboard", "Surging", "High Wind", "Completed max duration allowed", "Unknown" };
        }
        public int borderWidth { get; set; }
        public string label { get; set; }
        public List<string> labels { get; set; }
        public List<System.Drawing.Color> backgroundColor { get; set; }
        public List<int> data { get; set; }

       // List<string> ReasonLable = new List<string>() { "Mooring arrangement design", "During tightening", "During lowering", "Entagled in Jetty", "Entangled onboard", "Surging", "High Wind", "Completed max duration allowed", "Unknown" };
       // List<System.Drawing.Color> colour = new List<System.Drawing.Color>() { System.Drawing.Color.Orange, System.Drawing.Color.Green, System.Drawing.Color.LightBlue, System.Drawing.Color.LightGreen, System.Drawing.Color.LightSalmon, System.Drawing.Color.Yellow, System.Drawing.Color.Pink, System.Drawing.Color.OrangeRed, System.Drawing.Color.Red };


    }

}