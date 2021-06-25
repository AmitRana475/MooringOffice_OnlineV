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
    public class DeviationRiskController : BaseController
    {
        private readonly ShipmentContaxt sc1 = new ShipmentContaxt();
        JsonSerializerSettings _jsonSetting = new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore };
        private readonly IMenuRepository sc;

        public DeviationRiskController()
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


            ViewBag.vesselname = vesselname == "" ? "All Vessels" : vesselname;
            ViewBag.rank = rank == "" ? "All Ranks" : rank;



            ViewBag.fleettypebm1 = fleettype;
            ViewBag.fleetnamebm1 = fleetname;
            ViewBag.vesselnamebm1 = vesselname;
            ViewBag.rankbm1 = rank;
            ViewBag.status1 = status;


            //int noofday = Enumerable.Range(0, 0 + (Convert.ToDateTime(dateto) - Convert.ToDateTime(datefrom)).Days).Count();
            //int days = DateTime.DaysInMonth(Convert.ToDateTime(datefrom).Year, Convert.ToDateTime(datefrom).Month);

            string[] vesselnames = vesselname.TrimEnd(',').Split(',');
            string[] Rank = rank.TrimEnd(',').Split(',');
            string[] FleteType = fleettype.TrimEnd(',').Split(',');
            string[] FleeteName = fleetname.TrimEnd(',').Split(',');
            // List<ChartData> data = new List<ChartData>();
            List<ChartData> data;


            data = sc1.getChartDataVNRK_Bar(datefrom, dateto, vesselnames, Rank, FleteType, FleeteName).ToList();
            int ncs = 0;
            foreach (var item in data)
            {
                ncs += item.Deviation;
            }
            ViewBag.TotalNc = ncs;
            ViewBag.Datacount = data.Count > 0 ? "Deviation Risk Analysis" : "No data availble, please select another range";
            ViewBag.DataPoints = JsonConvert.SerializeObject(data, _jsonSetting);


            //ViewBag.datefrom1 = noofday > days ? DateTime.Parse(datefrom.ToString()).ToString("MMM/yyyy", CultureInfo.InvariantCulture) : DateTime.Parse(datefrom.ToString()).ToString("MM/dd/yyyy", CultureInfo.InvariantCulture);
            //ViewBag.dateto1 = noofday > days ? DateTime.Parse(dateto.ToString()).ToString("MMM/yyyy", CultureInfo.InvariantCulture) : DateTime.Parse(dateto.ToString()).ToString("MM/dd/yyyy", CultureInfo.InvariantCulture);

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
                ChartData nots = new AnalysisLayer.ChartData();
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

        private List<ChartData> BarGraphData(List<ChartData> data)
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