using AnalysisLayer;
using MenuLayer;
using Shipment49Web.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Dynamic;
using System.Web.Mvc;
using System.Web.Security;

namespace Shipment49Web.Areas.Analysis.Controllers
{
    [Authorize]
    [ErrorClass]
    public class GraphicviewController : Controller
    {
        private readonly ShipmentContaxt sc1 = new ShipmentContaxt();
        private readonly IMenuRepository sc;
        public GraphicviewController(IMenuRepository repo)
        {

            sc = repo;
            if (UserRole.username == null)
            {
                UserRole.username = string.Join("", Roles.GetRolesForUser());
            }
            ViewBag.GetMenu = UserRole.username == "Admin" ? sc.Menus.ToList() : sc.Menus.Where(x => x.Role == "User").ToList();
        }

        public  ActionResult Index(DateTime? datefrom, DateTime? dateto, int page = 1, string sort = "VesselName", string sortdir = "asc", string fleettype="", string fleetname="", string vesselname="", string rank="",string Searchbtn="")
        {
            UserRole.CheckAnalysis = true;

            datefrom = Convert.ToDateTime(datefrom == null ? DateTime.Parse(DateTime.Now.AddMonths(-1).ToString()).ToString("MM/dd/yyyy", CultureInfo.InvariantCulture) : DateTime.Parse(datefrom.ToString()).ToString("MM/dd/yyyy", CultureInfo.InvariantCulture));
            dateto = Convert.ToDateTime(dateto == null ? DateTime.Parse(DateTime.Now.ToString()).ToString("MM/dd/yyyy", CultureInfo.InvariantCulture) : DateTime.Parse(dateto.ToString()).ToString("MM/dd/yyyy", CultureInfo.InvariantCulture));

           

            fleettype = fleettype.TrimEnd(',');
            fleetname = fleetname.TrimEnd(',');
            vesselname = vesselname.TrimEnd(',');
            rank = rank.TrimEnd(',');


            ViewBag.fleettypebm1 = fleettype;
            ViewBag.fleetnamebm1 = fleetname;
            ViewBag.vesselnamebm1 = vesselname;
            ViewBag.rankbm1 = rank;


            ViewBag.vesselname = vesselname == "" ? "All Vessels" : vesselname;
            ViewBag.rank = rank == "" ? "All Ranks" : rank;
         

            int noofday = Enumerable.Range(0, 0 + (Convert.ToDateTime(dateto) - Convert.ToDateTime(datefrom)).Days).Count();




           


            List<ChartData> data;

            if (fleettype != "" && fleetname == "" && vesselname == "" && rank == "")
            {
                string[] fleettypes = fleettype.TrimEnd(',').Split(',');
                data = sc1.getChartDataFT(datefrom, dateto, fleettypes, noofday).ToList();


               

            }
            else if (fleettype == "" && fleetname != "" && vesselname == "" && rank == "")
            {

                string[] fleetnames = fleetname.TrimEnd(',').Split(',');
                data = sc1.getChartDataFN(datefrom, dateto, fleetnames, noofday).ToList();

               

            }
           
            else if (fleettype == "" && fleetname == "" && vesselname == "" && rank != "")
            {
                string[] ranks = rank.TrimEnd(',').Split(',');
                data = sc1.getChartDataRK(datefrom, dateto, ranks, noofday).ToList();

               
            }
            else if (fleettype != "" && fleetname != "" && vesselname == "" && rank == "")
            {
                string[] fleettypes = fleettype.TrimEnd(',').Split(',');
                string[] fleetnames = fleetname.TrimEnd(',').Split(',');
                data = sc1.getChartDataFTFN(datefrom, dateto, fleettypes, fleetnames, noofday).ToList();

               

            }
            else if (fleettype != "" && fleetname != "" && vesselname != "" && rank == "")
            {
                string[] fleettypes = fleettype.TrimEnd(',').Split(',');
                string[] fleetnames = fleetname.TrimEnd(',').Split(',');
                string[] vesselnames = vesselname.TrimEnd(',').Split(',');
                data = sc1.getChartDataFTFNVN(datefrom, dateto, fleettypes, fleetnames, vesselnames, noofday).ToList();

                

            }
            else if (fleettype != "" && fleetname != "" && vesselname != "" && rank != "")
            {
                string[] fleettypes = fleettype.TrimEnd(',').Split(',');
                string[] fleetnames = fleetname.TrimEnd(',').Split(',');
                string[] vesselnames = vesselname.TrimEnd(',').Split(',');
                string[] ranks = rank.TrimEnd(',').Split(',');
                data = sc1.getChartDataFTFNVNRK(datefrom, dateto, fleettypes, fleetnames, vesselnames, ranks, noofday).ToList();

               

            }
            else if (fleettype == "" && fleetname == "" && vesselname != "" && rank != "")
            {
                string[] ranks = rank.TrimEnd(',').Split(',');
                string[] vesselnames = vesselname.TrimEnd(',').Split(',');
                data = sc1.getChartDataVNRK(datefrom, dateto, vesselnames, ranks, noofday).ToList();

                

            }
            else if (fleettype == "" && fleetname != "" && vesselname == "" && rank != "")
            {

                string[] fleetnames = fleetname.TrimEnd(',').Split(',');
                string[] ranks = rank.TrimEnd(',').Split(',');
                data = sc1.getChartDataFNRK(datefrom, dateto, fleetnames, ranks, noofday).ToList();

                

            }
            else if (fleettype == "" && fleetname != "" && vesselname != "" && rank == "")
            {
                string[] fleetnames = fleetname.TrimEnd(',').Split(',');
                string[] vesselnames = vesselname.TrimEnd(',').Split(',');
                data = sc1.getChartDataFNVN(datefrom, dateto, fleetnames, vesselnames, noofday).ToList();

               
            }
            else if (fleettype != "" && fleetname == "" && vesselname != "" && rank == "")
            {
                string[] fleettypes = fleettype.TrimEnd(',').Split(',');
                string[] vesselnames = vesselname.TrimEnd(',').Split(',');
                data = sc1.getChartDataFTVN(datefrom, dateto, fleettypes, vesselnames, noofday).ToList();

                

            }
            else if (fleettype != "" && fleetname == "" && vesselname == "" && rank != "")
            {
                string[] fleettypes = fleettype.TrimEnd(',').Split(',');
                string[] ranks = rank.TrimEnd(',').Split(',');
                data = sc1.getChartDataFTRK(datefrom, dateto, fleettypes, ranks, noofday).ToList();

               


            }
            else if (fleettype == "" && fleetname != "" && vesselname != "" && rank != "")
            {
                string[] fleetnames = fleetname.TrimEnd(',').Split(',');
                string[] vesselnames = vesselname.TrimEnd(',').Split(',');
                string[] ranks = rank.TrimEnd(',').Split(',');
                data = sc1.getChartDataFNVNRK(datefrom, dateto, fleetnames, vesselnames, ranks, noofday).ToList();

                

            }
            else if (fleettype != "" && fleetname != "" && vesselname == "" && rank != "")
            {
                string[] fleettypes = fleettype.TrimEnd(',').Split(',');
                string[] fleetnames = fleetname.TrimEnd(',').Split(',');
                string[] ranks = rank.TrimEnd(',').Split(',');
                data = sc1.getChartDataFTFNRK(datefrom, dateto, fleettypes, fleetnames, ranks, noofday).ToList();

                

            }
            else if (fleettype == "" && fleetname == "" && vesselname != "" && rank == "")
            {

                string[] vesselnames = vesselname.TrimEnd(',').Split(',');
                data = sc1.getChartDataVN(datefrom, dateto, vesselnames, noofday).ToList();

                if (string.IsNullOrEmpty(Searchbtn))
                {
                    if (data.Count() == 0)
                    {
                        data = null;
                        datefrom = Convert.ToDateTime(DateTime.Parse(DateTime.Now.AddMonths(-3).ToString()).ToString("MM/dd/yyyy", CultureInfo.InvariantCulture));
                        noofday = Enumerable.Range(0, 0 + (Convert.ToDateTime(dateto) - Convert.ToDateTime(datefrom)).Days).Count();
                        data = sc1.getChartDataVN(datefrom, dateto, vesselnames, noofday).ToList();

                    }
                    if (data.Count() == 0)
                    {
                        data = null;
                        datefrom = Convert.ToDateTime(DateTime.Parse(DateTime.Now.AddMonths(-6).ToString()).ToString("MM/dd/yyyy", CultureInfo.InvariantCulture));
                        noofday = Enumerable.Range(0, 0 + (Convert.ToDateTime(dateto) - Convert.ToDateTime(datefrom)).Days).Count();
                        data = sc1.getChartDataVN(datefrom, dateto, vesselnames, noofday).ToList();

                    }
                    if (data.Count() == 0)
                    {
                        data = null;
                        datefrom = Convert.ToDateTime(DateTime.Parse(DateTime.Now.AddMonths(-12).ToString()).ToString("MM/dd/yyyy", CultureInfo.InvariantCulture));
                        noofday = Enumerable.Range(0, 0 + (Convert.ToDateTime(dateto) - Convert.ToDateTime(datefrom)).Days).Count();
                        data = sc1.getChartDataVN(datefrom, dateto, vesselnames, noofday).ToList();

                    }
                }

            }
            else
            {
                data = sc1.getChartData(datefrom, dateto, noofday).ToList();

                if (string.IsNullOrEmpty(Searchbtn))
                {
                    if (data.Count() == 0)
                    {
                        data = null;
                        datefrom = Convert.ToDateTime(DateTime.Parse(DateTime.Now.AddMonths(-3).ToString()).ToString("MM/dd/yyyy", CultureInfo.InvariantCulture));
                        noofday = Enumerable.Range(0, 0 + (Convert.ToDateTime(dateto) - Convert.ToDateTime(datefrom)).Days).Count();
                        data = sc1.getChartData(datefrom, dateto, noofday).ToList();

                    }
                    if (data.Count() == 0)
                    {
                        data = null;
                        datefrom = Convert.ToDateTime(DateTime.Parse(DateTime.Now.AddMonths(-6).ToString()).ToString("MM/dd/yyyy", CultureInfo.InvariantCulture));
                        noofday = Enumerable.Range(0, 0 + (Convert.ToDateTime(dateto) - Convert.ToDateTime(datefrom)).Days).Count();
                        data = sc1.getChartData(datefrom, dateto, noofday).ToList();

                    }
                    if (data.Count() == 0)
                    {
                        data = null;
                        datefrom = Convert.ToDateTime(DateTime.Parse(DateTime.Now.AddMonths(-12).ToString()).ToString("MM/dd/yyyy", CultureInfo.InvariantCulture));
                        noofday = Enumerable.Range(0, 0 + (Convert.ToDateTime(dateto) - Convert.ToDateTime(datefrom)).Days).Count();
                        data = sc1.getChartData(datefrom, dateto, noofday).ToList();

                    }
                }

            }





            ViewBag.Datacount = data.Count > 0 ? "Graphical Trend View" : "No data availble please select another range";

            //var test = (from name in data
            //            select
            //            name);

           

            IEnumerable<ChartData> test1 = data;

            int pagesize = 10;
            int totalrecords = 0;

            if (page < 1) page = 1;
            int skip = (page * pagesize) - pagesize;

            totalrecords = data.Count();
            test1 = data.OrderBy(sort + " " + sortdir);
            if (pagesize > 0)
            {
                test1 = data.Skip(skip).Take(pagesize);
            }

            ViewBag.TotalRows = totalrecords;

            //return RedirectToAction("index", new RouteValueDictionary(
            //new { controller = "notification", area = "notification" }));

            ViewBag.datefrom = DateTime.Parse(datefrom.ToString()).ToString("MM/dd/yyyy", CultureInfo.InvariantCulture);
            ViewBag.dateto = DateTime.Parse(dateto.ToString()).ToString("MM/dd/yyyy", CultureInfo.InvariantCulture);

            //..................
            ViewBag.fleettypebm = sc.AutoCompletedFleetType;
            ViewBag.fleetnamebm = sc.AutoCompletedfleetname;
            ViewBag.vesselnamebm = sc.AutoCompletevessel;
            ViewBag.rankebm = sc.AutoCompleterank;
            //......................


            return View(test1.ToList());

        }
    }
}