using MenuLayer;
using Newtonsoft.Json;
using Reports;
using Shipment49Web.Areas.MSPS.Models;
using Shipment49Web.Common;
using Shipment49Web.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Linq.Dynamic;
using System.Text.RegularExpressions;
using System.Web.Mvc;

namespace Shipment49Web.Controllers
{
    [Authorize]
    [UserAuthenticationFilter]
    [ErrorClass]
    public class HomeController : BaseController
    {
        //private readonly IMenuRepository sc;

        //public HomeController(IMenuRepository repo)
        public HomeController()
        {
            //if (string.IsNullOrEmpty(UserRole.username))
            //{
            //    UserRole.username = string.Join("", Roles.GetRolesForUser());
            //}
            ////getmenulist();
            ////string roleNames = string.Join("", Roles.GetRolesForUser());
            //ViewBag.GetMenu = UserRole.username == "Admin" ? sc.Menus.ToList() : sc.Menus.Where(x => x.Role == "User").ToList();
            ////var data = sc.
            //ViewBag.GetSubMenu = UserRole.username == "Admin" ? sc.SubMenus.ToList() : sc.SubMenus.Where(x => x.Role == "User").ToList();


            using (ShipmentContaxt sc1 = new ShipmentContaxt())
            {
                DetailsViewModel model = new DetailsViewModel();

                var _smartmenu = sc1.SmartMenus.ToList().FirstOrDefault();
                if (_smartmenu != null)
                {
                    ViewBag.MenuContent = _smartmenu.SmartMenuContent;
                }
            }

        }


        //[PartialCache("OneMinuteCache")]
        // private List<Menu> getmenulist()
        // {

        //     string roleNames = string.Join("", Roles.GetRolesForUser());
        //     UserRole.GetMenu = roleNames == "Admin" ?  sc.Menus.ToList() :  sc.Menus.Where(x => x.Role == "User").ToList();


        //     return UserRole.GetMenu;
        // }

        //[PartialCache("OneMinuteCache")]
        public ActionResult Index(int page = 1, string sort = "LastImport", string sortdir = "DESC", string fleetname = "", string fleettype = "", string search = "")
        {

            using (ShipmentContaxt sc1 = new ShipmentContaxt())
            {
                DetailsViewModel model = new DetailsViewModel();

                var _smartmenu = sc1.SmartMenus.ToList().FirstOrDefault();
                if (_smartmenu != null)
                {
                    model.SmartMenuContent = _smartmenu.SmartMenuContent;

                    ViewBag.MenuContent = _smartmenu.SmartMenuContent;
                }

                //return RedirectToAction("Dashboard", "home", new {  Area = "MSPS" });

                return RedirectToAction("Dashboard", "home");
                //return PartialView("_LeftSideBar", model);

            }


            //fleettype = fleettype.TrimEnd(',').Replace("multiselect-all","").TrimStart(',').Trim();
            //fleetname = fleetname.TrimEnd(',').Replace("multiselect-all", "").TrimStart(',').Trim();
            //search = search.TrimEnd(',').Replace("multiselect-all", "").TrimStart(',').Trim();


            //ViewBag.fleettypebm1 = fleettype;
            //ViewBag.fleetnamebm1 = fleetname;
            //ViewBag.vesselnamebm1 = search;




            //int pagesize = 10;
            //int totalrecords = 0;

            //if (page < 1) page = 1;
            //int skip = (page * pagesize) - pagesize;

            //var data = GetCertificatelist(search, fleetname, fleettype,sort, sortdir, skip, pagesize, out totalrecords);
            //ViewBag.TotalRows = totalrecords;
            //ViewBag.vname = search;

            //ViewBag.Messagegrid = data.Count() > 0 ? "" : ViewBag.Messagegrid = "Data Not Available For Selected Criteria! Please Select The Other Criteria.";





            //ViewBag.fleettypebm = sc.AutoCompletedFleetType;
            //ViewBag.fleetnamebm = sc.AutoCompletedfleetname;
            //ViewBag.vesselnamebm = sc.AutoCompletevessel;





            //return View( data);
        }

        public ActionResult Dashboard()
        {
            //ViewBag.Notifications = sc.Notifications.OrderByDescending(u => u.Nid).Take(10).ToList();
            //ViewBag.ImportLogs = sc.ImportLogs.OrderByDescending(u => u.Id).Take(10).ToList();

            using (Reports.MorringOfficeEntities context = new Reports.MorringOfficeEntities())
            {
                ViewBag.Notifications = context.View_Notifications.OrderByDescending(u => u.NotificationDate).Take(15).ToList();
                ViewBag.ImportLogs = context.ImportLogs.OrderByDescending(u => u.Id).Take(15).ToList();

                //List<AbrasionData> listZoneA = new List<AbrasionData>();

                //List<AbrasionChartData> listZoneAAbrasion = new List<AbrasionChartData>();
                //var abrasionA = context.spView_AbrasionZoneA();
                //foreach (var zoneA in abrasionA)
                //{
                //    AbrasionChartData data = new AbrasionChartData
                //    {
                //        label = zoneA.RatingID == 0 ? 0 : Convert.ToInt32(zoneA.RatingID),
                //        value = zoneA.TotalRopes == 0 ? 0 : Convert.ToInt32(zoneA.TotalRopes)
                //    };

                //    data.color = getColor(Convert.ToInt32(data.label));
                //    listZoneAAbrasion.Add(data);
                //}

                //listZoneA.Add(new AbrasionData() { key = "Zone A", values = listZoneAAbrasion });
                //ViewBag.ZoneA = JsonConvert.SerializeObject(listZoneA);

                //List<AbrasionData> listZoneB = new List<AbrasionData>();
                //List<AbrasionChartData> listZoneBAbrasion = new List<AbrasionChartData>();
                //var abrasionB = context.spView_AbrasionZoneB();
                //foreach (var zoneB in abrasionB)
                //{
                //    AbrasionChartData data = new AbrasionChartData
                //    {
                //        label = zoneB.RatingID == 0 ? 0 : Convert.ToInt32(zoneB.RatingID),
                //        value = zoneB.TotalRopes == 0 ? 0 : Convert.ToInt32(zoneB.TotalRopes)
                //    };

                //    data.color = getColor(Convert.ToInt32(data.label));
                //    listZoneBAbrasion.Add(data);
                //}

                //listZoneB.Add(new AbrasionData() { key = "Zone B", values = listZoneBAbrasion });
                //ViewBag.ZoneB = JsonConvert.SerializeObject(listZoneB);

                List<AbrasionData> listCombinedZones = new List<AbrasionData>();
                List<AbrasionChartData> listCombinedZonesAbrasion = new List<AbrasionChartData>();

                DataTable graphData = CommonMethods.ExecStoredProceedure("spAbrasionCombined");

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

                //DataSet Result = CommonMethods.ExecStoredProceedureWithDataSet("DashboardMultipleData");
                //List<AnomaliesChartData> listCombinedAnomalies = new List<AnomaliesChartData>();
                //int sr = 0;
                //for (int i = 0; i < Result.Tables.Count; i++)
                //{
                //    sr++;
                //    DataTable table = Result.Tables[i];
                //    AnomaliesChartData chartdata = new AnomaliesChartData()
                //    {
                //        label = sr,
                //        value1 = Result.Tables[i].Rows.Count,//table.Rows.Count,
                //        value2 = Result.Tables[i+1].Rows.Count
                //    };

                //    listCombinedAnomalies.Add(chartdata);
                //    i = i + 1;
                //}

                DateTime dateF = DateTime.Now.Date.AddYears(-1);
                DateTime dateTo = DateTime.Now.Date;
                DataTable dt = CommonMethods.ExecStoredProceedureBetweenDates("SP_VesselWiseMooringOP_Detail", dateF, dateTo);

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

                List<AnomaliesChartData> listCombinedAnomalies = CommonMethods.GetAnomaliesChartList(records);
                int a1 = listCombinedAnomalies.Max(x => x.value1);
                int b1 = listCombinedAnomalies.Max(x => x.value2);
                int geater = a1 > b1 == true ? a1 : b1;

                int mv = listCombinedAnomalies.Count > 0 ? geater : 0;
                int mv2 = mv;// > 0 ? Convert.ToInt32(mv / 20) * 20 + 20 : mv;
                var ChData = new GraphData() { chartId = mv2, data = RopeAnalysis.GetChartDataCom(listCombinedAnomalies, "No of Times reported", "No of Ports reported") };
                ViewBag.CombinedAnomalies = ChData;
                ViewBag.AnomaliesTitle = "Date from " + dateF.ToString("dd-MMM-yyyy")+ " To " + dateTo.ToString("dd-MMM-yyyy");
                // ViewBag.CombinedAnomalies= ropeAnalysis.ChartData.Add(new GraphData() { chartId = i, data = ropeAnalysis.GetChartData(ropeAnalysis.RopeAnalysis, i) });
                //var combined = context.spView_AbrasionZoneAlls();
                //foreach (var zoneData in combined)
                //{
                //    AbrasionChartData data = new AbrasionChartData
                //    {
                //        label = zoneData.RatingID == 0 ? 0 : Convert.ToInt32(zoneData.RatingID),
                //        value = zoneData.TotalRopes == 0 ? 0 : Convert.ToInt32(zoneData.TotalRopes)
                //    };

                //    data.color = getColor(Convert.ToInt32(data.label));
                //    listCombinedZonesAbrasion.Add(data);
                //}

                listCombinedZones.Add(new AbrasionData() { key = "All Zones", values = listCombinedZonesAbrasion });
                ViewBag.CombinedZones = JsonConvert.SerializeObject(listCombinedZones);

                //var inspectionOverdue = context.spRopeInspectionPercentage().ToList();
                //if (inspectionOverdue.Count > 0)
                //{
                //    decimal overdue = inspectionOverdue[0].Overdue == null ? 0 : Convert.ToInt32(inspectionOverdue[0].Overdue);
                //    decimal pending = inspectionOverdue[0].Pending == null ? 0 : Convert.ToInt32(inspectionOverdue[0].Pending);

                //    decimal InspectionOverdue = 0;
                //    if ((overdue > 0) && (pending > 0))
                //        InspectionOverdue = (overdue / pending) * 100;

                //    ViewBag.InspectionOverdue = InspectionOverdue;
                //}

                //&&&&&&&&&&&&&&&&&&&&&&&&&&&&&  New code will be replace above code line no 227 //&&&&&&&&&&&&&&&&&&&&&&&&&&&&& 

                List<InspectionList> results = CommonMethods.InspectionList_Details; //CommonMethods.GetInspectionInformation();

                decimal Overdue = results.Count(u => u.InspectionDueDate < DateTime.Today);
                decimal Pending = results.Count(u => u.InspectionDueDate >= DateTime.Today);

                if ((Overdue > 0) && (Pending > 0))
                    ViewBag.InspectionOverdue = (Overdue / results.Count) * 100;
                else
                    ViewBag.InspectionOverdue = 0;

                //&&&&//&&&&&&&&&&&&&&&&&&&&&&&&&&&&& //&&&&&&&&&&&&&&&&&&&&&&&&&&&&& //&&&&&&&&&&&&&&&&&&&&&&&&&&&&&  

                var discarded = context.spRopesDiscarded().ToList().Select(u => new { x = u.Month - 1, y = u.TotalRopes });
                ViewBag.RopesDiscarded = JsonConvert.SerializeObject(discarded);

                var tobediscarded = context.spRopesRequiringDiscard().ToList();
                if (tobediscarded.Count > 0)
                {
                    ViewBag.RopesRequiringDiscarded = tobediscarded[0].Value;
                }
                var tobediscardedTails = context.spTailsRequiringDiscard().ToList();
                if (tobediscardedTails.Count > 0)
                {
                    ViewBag.TailsRequiringDiscarded = tobediscardedTails[0].Value;
                }


                DataTable UpcomingRopeDiscard = CommonMethods.ExecStoredProceedure("SpUpcomingRopeDiscard");
                DataTable UpcomingTailDiscard = CommonMethods.ExecStoredProceedure("SpUpcomingTailDiscard");
                ViewBag.UpComingRopeDiscard = UpcomingRopeDiscard.Rows.Count;
                ViewBag.UpComingTailDiscard = UpcomingTailDiscard.Rows.Count;

                var satisfactoryRopes = context.spSatisfactoryRopes().ToList();
                if (satisfactoryRopes.Count > 0)
                {
                    ViewBag.SatisfactoryRopes = satisfactoryRopes[0].RopePercent == null ? 0: satisfactoryRopes[0].RopePercent >100  ? 100 : satisfactoryRopes[0].RopePercent;
                }


                DataSet End2EndResult = CommonMethods.ExecStoredProceedureWithDataSet("EndtoEndUpcomingNDue");
                ViewBag.End2EndUpComing = End2EndResult.Tables[0].Rows.Count;
                ViewBag.End2EndDue = End2EndResult.Tables[1].Rows.Count;


                CommonMethods.AllWinchRotationList = CommonMethods.FindWinchRotation();

                ViewBag.WRotationUpComing = CommonMethods.AllWinchRotationList.Where(x => x.StatusUpDue == "Upcoming").ToList().Count(); // ApprochingCount;
                ViewBag.WRotationDue = CommonMethods.AllWinchRotationList.Where(x => x.StatusUpDue == "Overdue").ToList().Count(); //ExceedCount;
            }

            if (Response.Cookies.Count > 0)
            {
                // Response.CacheControl = "no-store";
                foreach (string s in Response.Cookies.AllKeys)
                {
                    if ( s == "ASP.NET_SessionId" || s.ToLower() == "asp.net_sessionid")
                    {
                        Response.Cookies[s].Secure = true;
                       
                    }
                }
            }

            return View();
        }


        [HttpGet]
        public ActionResult VesselDashboard()
        {
            return View();
        }

        //private readonly Func<IMenuRepository, IEnumerable<NotificationDashboard>> GetCerti = c => c.getNotiDashbord.ToList();

        //[NonAction]
        //private List<NotificationDashboard> GetCertificatelist(string search, string fleetname, string fleettype, string sort, string sortdir, int skip, int pagesize, out int totalrecord)
        //{
        //    //var dd = sc.getNotiDashbord.ToList();
        //    string[] searchb = search.TrimEnd(',').Split(',');

        //    var clist = GetCerti.Invoke(sc);
        //    if (!string.IsNullOrEmpty(search))
        //    {
        //        clist = clist.Where(x => searchb.Contains(x.VesselName));
        //    }
        //    if (!string.IsNullOrEmpty(fleetname))
        //    {
        //        string[] fleetnames = fleetname.TrimEnd(',').Split(',');
        //        clist = clist.Where(x => fleetnames.Contains(x.FleetName));
        //    }
        //    if (!string.IsNullOrEmpty(fleettype))
        //    {
        //        string[] fleettypes = fleettype.TrimEnd(',').Split(',');
        //        clist = clist.Where(x => fleettypes.Contains(x.FleetType));
        //    }
        //    if (UserRole.username.ToLower() != "admin")
        //    {
        //        var vsname = sc.Users.Where(x => x.EmailId.Equals(UserRole.username1)).Select(x => x.VesselName).FirstOrDefault();
        //        string[] vesselname = vsname.TrimEnd(',').Split(',');
        //        clist = clist.Where(x => vesselname.Contains(x.VesselName));
        //    }




        //    totalrecord = clist.Count();
        //    clist = clist.OrderBy(sort + " " + sortdir);
        //    if (pagesize > 0)
        //    {
        //        clist = clist.Skip(skip).Take(pagesize);
        //    }


        //    //string bms11 = sw1.ElapsedMilliseconds.ToString();
        //    //sw1.Stop();

        //    return clist.ToList();

        //}

        //[PartialCache("OneMinuteCache")]
        //public async Task<JsonResult> AutoCompletevessel()
        //{
        //    var students = await sc.CreReports.Select(x=> new {x.Vessel_ID,x.VesselName }).Distinct().ToListAsync();

        //    return Json( students, JsonRequestBehavior.AllowGet);
        //}

        //[PartialCache("OneMinuteCache")]
        //public JsonResult AutoCompletedfname()
        //{
        //    //var students = await sc.CreReports.Select(x => new { x.Vessel_ID, x.FleetName }).Distinct().ToListAsync();

        //    var sportsList =  new List<SelectListItem>();
        //    sportsList.Add(new SelectListItem { Text = "Baseball", Value = "1" });
        //    sportsList.Add(new SelectListItem { Text = "Basketball", Value = "2" });
        //    sportsList.Add(new SelectListItem { Text = "Football", Value = "3" });
        //    sportsList.Add(new SelectListItem { Text = "Soccer", Value = "4" });

        //    return Json( sportsList, JsonRequestBehavior.AllowGet);
        //}
        //[PartialCache("OneMinuteCache")]
        //public async Task<JsonResult> AutoCompletedtname()
        //{
        //    var students = await sc.CreReports.Select(x => new { x.Vessel_ID, x.FleetType }).Distinct().ToListAsync();

        //    return Json(students, JsonRequestBehavior.AllowGet);
        //}

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                //sc.Dispose();
            }
            base.Dispose(disposing);
        }
    }

}