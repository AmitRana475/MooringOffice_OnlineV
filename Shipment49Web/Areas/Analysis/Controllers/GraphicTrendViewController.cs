using AnalysisLayer;
using MenuLayer;
using Newtonsoft.Json;
using Shipment49Web.Common;
using Shipment49Web.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;

namespace Shipment49Web.Areas.Analysis.Controllers
{
    [Authorize]
    [UserAuthenticationFilter]
    [ErrorClass]
    public class GraphicTrendViewController : BaseController
    {
        private readonly ShipmentContaxt sc1 = new ShipmentContaxt();
        JsonSerializerSettings _jsonSetting = new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore };

        private readonly IMenuRepository sc;
        public GraphicTrendViewController()
        {
            //sc = repo;

            //if (UserRole.username == null)
            //{
            //    UserRole.username = string.Join("", Roles.GetRolesForUser());
            //}
            //ViewBag.GetMenu = UserRole.username == "Admin" ? sc.Menus.ToList() : sc.Menus.Where(x => x.Role == "User").ToList();

        }

        public ActionResult Index(DateTime? datefrom, DateTime? dateto, string fleettype = "", string fleetname = "", string vesselname = "", string rank = "", string status = "", string Searchbtn = "")
        {
            UserRole.CheckAnalysis = false;

            datefrom = Convert.ToDateTime(datefrom == null ? DateTime.Parse(DateTime.Now.AddMonths(-1).ToString()).ToString("MM/dd/yyyy", CultureInfo.InvariantCulture) : DateTime.Parse(datefrom.ToString()).ToString("MM/dd/yyyy", CultureInfo.InvariantCulture));
            dateto = Convert.ToDateTime(dateto == null ? DateTime.Parse(DateTime.Now.ToString()).ToString("MM/dd/yyyy", CultureInfo.InvariantCulture) : DateTime.Parse(dateto.ToString()).ToString("MM/dd/yyyy", CultureInfo.InvariantCulture));



            fleettype = fleettype.TrimEnd(',').Replace("multiselect-all", "").TrimStart(',').Trim();
            fleetname = fleetname.TrimEnd(',').Replace("multiselect-all", "").TrimStart(',').Trim();
            vesselname = vesselname.TrimEnd(',').Replace("multiselect-all", "").TrimStart(',').Trim();
            rank = rank.TrimEnd(',').Replace("multiselect-all", "").TrimStart(',').Trim();
            status = status.TrimEnd(',').Replace("multiselect-all", "").TrimStart(',').Trim();


            ViewBag.fleettypebm1 = fleettype;
            ViewBag.fleetnamebm1 = fleetname;
            ViewBag.vesselnamebm1 = vesselname;
            ViewBag.rankbm1 = rank;
            ViewBag.status1 = status;




            ViewBag.vesselname = vesselname == "" ? "All Vessels" : vesselname;
            ViewBag.rank = rank == "" ? "All Ranks" : rank;
            //ViewBag.Data1 = datefrom;

            int noofday = Enumerable.Range(0, 0 + (Convert.ToDateTime(dateto) - Convert.ToDateTime(datefrom)).Days).Count();
            int days = DateTime.DaysInMonth(Convert.ToDateTime(datefrom).Year, Convert.ToDateTime(datefrom).Month);


            //int noofday1 = (int)(Convert.ToDateTime(TempData["dateto"]) - Convert.ToDateTime(TempData["datefrom"])).TotalDays;
            List<ChartData> data;


            if (string.IsNullOrEmpty(Searchbtn))
            {
                string[] vesselnames = vesselname.TrimEnd(',').Split(',');
                data = sc1.getChartData(datefrom, dateto, noofday).ToList();
                if (data.Count() == 0)
                {
                    data = null;
                    datefrom = Convert.ToDateTime(DateTime.Parse(DateTime.Now.AddMonths(-3).ToString()).ToString("MM/dd/yyyy", CultureInfo.InvariantCulture));
                    noofday = Enumerable.Range(0, 0 + (Convert.ToDateTime(dateto) - Convert.ToDateTime(datefrom)).Days).Count();
                    data = sc1.getChartData(datefrom, dateto, noofday).ToList();

                    if (UserRole.username.ToLower() != "admin")
                    {
                        var vsname = sc.Users.Where(x => x.EmailId.Equals(UserRole.username1)).Select(x => x.VesselName).FirstOrDefault();
                        string[] vesselname1 = vsname.TrimEnd(',').Split(',');
                        data = data.Where(x => vesselname1.Contains(x.VesselName)).ToList();
                    }
                    if (!string.IsNullOrEmpty(vesselname))
                        data = data.Where(x => vesselnames.Contains(x.VesselName)).ToList();
                }
                if (data.Count() == 0)
                {
                    data = null;
                    datefrom = Convert.ToDateTime(DateTime.Parse(DateTime.Now.AddMonths(-6).ToString()).ToString("MM/dd/yyyy", CultureInfo.InvariantCulture));
                    noofday = Enumerable.Range(0, 0 + (Convert.ToDateTime(dateto) - Convert.ToDateTime(datefrom)).Days).Count();
                    data = sc1.getChartData(datefrom, dateto, noofday).ToList();

                    if (UserRole.username.ToLower() != "admin")
                    {
                        var vsname = sc.Users.Where(x => x.EmailId.Equals(UserRole.username1)).Select(x => x.VesselName).FirstOrDefault();
                        string[] vesselname1 = vsname.TrimEnd(',').Split(',');
                        data = data.Where(x => vesselname1.Contains(x.VesselName)).ToList();
                    }
                    if (!string.IsNullOrEmpty(vesselname))
                        data = data.Where(x => vesselnames.Contains(x.VesselName)).ToList();
                }
                if (data.Count() == 0)
                {
                    data = null;
                    datefrom = Convert.ToDateTime(DateTime.Parse(DateTime.Now.AddMonths(-12).ToString()).ToString("MM/dd/yyyy", CultureInfo.InvariantCulture));
                    noofday = Enumerable.Range(0, 0 + (Convert.ToDateTime(dateto) - Convert.ToDateTime(datefrom)).Days).Count();
                    data = sc1.getChartData(datefrom, dateto, noofday).ToList();

                    if (UserRole.username.ToLower() != "admin")
                    {
                        var vsname = sc.Users.Where(x => x.EmailId.Equals(UserRole.username1)).Select(x => x.VesselName).FirstOrDefault();
                        string[] vesselname1 = vsname.TrimEnd(',').Split(',');
                        data = data.Where(x => vesselname1.Contains(x.VesselName)).ToList();
                    }
                    if (!string.IsNullOrEmpty(vesselname))
                        data = data.Where(x => vesselnames.Contains(x.VesselName)).ToList();
                }



                data = GraphData(data);


            }
            else
            {
                data = sc1.getChartData(datefrom, dateto, noofday).ToList();

                if (!string.IsNullOrEmpty(fleettype))
                {
                    string[] fleettypes1 = fleettype.TrimEnd(',').Split(',');
                    data = data.Where(x => fleettypes1.Contains(x.FleetType)).ToList();
                }
                if (!string.IsNullOrEmpty(fleetname))
                {
                    string[] fleetnames1 = fleetname.TrimEnd(',').Split(',');
                    data = data.Where(x => fleetnames1.Contains(x.FleetName)).ToList();
                }
                if (!string.IsNullOrEmpty(vesselname))
                {
                    string[] vesselnames1 = vesselname.TrimEnd(',').Split(',');
                    data = data.Where(x => vesselnames1.Contains(x.VesselName)).ToList();
                }
                if (!string.IsNullOrEmpty(rank))
                {
                    string[] ranks1 = rank.TrimEnd(',').Split(',');
                    data = data.Where(x => ranks1.Contains(x.Rank)).ToList();
                }

                if (UserRole.username.ToLower() != "admin")
                {
                    var vsname = sc.Users.Where(x => x.EmailId.Equals(UserRole.username1)).Select(x => x.VesselName).FirstOrDefault();
                    string[] vesselname1 = vsname.TrimEnd(',').Split(',');
                    data = data.Where(x => vesselname1.Contains(x.VesselName)).ToList();
                }

                data = GraphData(data);

            }


            var bb = status.Split(',');
            string a = string.Empty;
            //var results = Array.FindAll(bb, s => s.Equals("Work Hours"));
            //bool exists = bb.Contains("Work Hours");

            if (bb.Contains("Work Hours"))
                a = "1";
            if (bb.Contains("Rest Hours"))
                a = "2";
            if (bb.Contains("Deviation"))
                a = "3";
            if (bb.Contains("Work Hours") && bb.Contains("Rest Hours"))
                a = "4";
            if (bb.Contains("Work Hours") && bb.Contains("Deviation"))
                a = "5";
            if (bb.Contains("Rest Hours") && bb.Contains("Deviation"))
                a = "6";
            if (bb.Contains("Work Hours") && bb.Contains("Rest Hours") && bb.Contains("Deviation"))
                a = "7";
            else if (status == string.Empty)
                a = "7";

            ViewBag.status2 = a;

            ViewBag.Datacount = data.Count > 0 ? "Graphical Trend View" : "No data availble, please select another range";
            ViewBag.DataPoints = JsonConvert.SerializeObject(data, _jsonSetting);


            ViewBag.datefrom1 = noofday > days ? DateTime.Parse(datefrom.ToString()).ToString("MMM/yyyy", CultureInfo.InvariantCulture) : DateTime.Parse(datefrom.ToString()).ToString("MM/dd/yyyy", CultureInfo.InvariantCulture);
            ViewBag.dateto1 = noofday > days ? DateTime.Parse(dateto.ToString()).ToString("MMM/yyyy", CultureInfo.InvariantCulture) : DateTime.Parse(dateto.ToString()).ToString("MM/dd/yyyy", CultureInfo.InvariantCulture);

            ViewBag.datefrom = DateTime.Parse(datefrom.ToString()).ToString("MM/dd/yyyy", CultureInfo.InvariantCulture);
            ViewBag.dateto = DateTime.Parse(dateto.ToString()).ToString("MM/dd/yyyy", CultureInfo.InvariantCulture);

            //..................
            ViewBag.fleettypebm = sc.AutoCompletedFleetType;
            ViewBag.fleetnamebm = sc.AutoCompletedfleetname;
            ViewBag.vesselnamebm = sc.AutoCompletevessel;
            ViewBag.rankebm = sc.AutoCompleterank;
            //......................


            return View();

        }

        private List<ChartData> GraphData(List<ChartData> data)
        {
            List<ChartData> notilist = new List<ChartData>();

            var emps = data.GroupBy(s => s.Months).ToList();
            foreach (var m in emps)
            {
                ChartData nots = new ChartData();
                nots.VesselName = data.Where(x => x.Months.Equals(m.Key)).FirstOrDefault().VesselName;
                nots.FleetType = data.Where(x => x.Months.Equals(m.Key)).FirstOrDefault().FleetType;
                nots.FleetName = data.Where(x => x.Months.Equals(m.Key)).FirstOrDefault().FleetName;
                nots.Deviation = data.Where(x => x.Months.Equals(m.Key)).ToList().Sum(x => x.Deviation);
                nots.Rank = data.Where(x => x.Months.Equals(m.Key)).FirstOrDefault().Rank;
                var workSum = (from emp in data.Where(x => x.Months.Equals(m.Key))
                               select emp.Work);
                nots.Work = workSum.Sum();
                var RestSum = (from emp in data.Where(x => x.Months.Equals(m.Key))
                               select emp.Rest).Sum();
                nots.Rest = RestSum;

                nots.Months = m.Key.ToString();


                notilist.Add(nots);

            }


            return notilist;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                sc1.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}