using MenuLayer;
using System.Web.Mvc;
using AnalysisLayer;
using Newtonsoft.Json;
using System.Linq;
using System;
using System.Globalization;
using System.Collections.Generic;
using Shipment49Web.Common;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Web.Security;
using Shipment49Web.Models;

namespace Shipment49Web.Areas.Analysis.Controllers
{
    [Authorize]
    [ErrorClass]
    public class AnalysisController : Controller
    {
        private readonly ShipmentContaxt sc1= new ShipmentContaxt();
        JsonSerializerSettings _jsonSetting = new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore };

        private readonly IMenuRepository sc;
        public AnalysisController(IMenuRepository repo)
        {
            sc = repo;

            if (UserRole.username == null)
            {
                UserRole.username = string.Join("", Roles.GetRolesForUser());
            }
            ViewBag.GetMenu = UserRole.username == "Admin" ? sc.Menus.ToList() : sc.Menus.Where(x => x.Role == "User").ToList();

        }

        public ActionResult Index(DateTime? datefrom, DateTime? dateto, string fleettype="",string fleetname="", string vesselname="", string rank="",string Searchbtn="")
        {
            UserRole.CheckAnalysis = false;

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


            //fleetname = fleetname == "[]" ? fleetname = string.Empty : fleetname == null ? fleetname = string.Empty : fleetname;
            //fleettype = fleettype == "[]" ? fleettype = string.Empty : fleettype == null ? fleettype = string.Empty : fleettype;
            //vesselname = vesselname == "[]" ? vesselname = string.Empty : vesselname == null ? vesselname = string.Empty : vesselname;
            //rank = rank == "[]" ? rank = string.Empty : rank == null ? rank = string.Empty : rank;

            //fleetname = fleetname.Replace("{", "").Replace("}", "").Replace("[", "").Replace("]", "").Replace("id", "").Replace(":", "").Replace(@"""", "").Replace("\"", "");
            //fleettype = fleettype.Replace("{", "").Replace("}", "").Replace("[", "").Replace("]", "").Replace("id", "").Replace(":", "").Replace(@"""", "").Replace("\"", "");
            //vesselname = vesselname.Replace("{", "").Replace("}", "").Replace("[", "").Replace("]", "").Replace("id", "").Replace(":", "").Replace(@"""", "").Replace("\"", "");
            //rank = rank.Replace("{", "").Replace("}", "").Replace("[", "").Replace("]", "").Replace("id", "").Replace(":", "").Replace(@"""", "").Replace("\"", ""); 


            ViewBag.vesselname = vesselname == "" ? "All Vessels" : vesselname;
            ViewBag.rank = rank == "" ? "All Ranks" : rank;
            //ViewBag.Data1 = datefrom;

            int noofday = Enumerable.Range(0, 0 + (Convert.ToDateTime(dateto) - Convert.ToDateTime(datefrom)).Days).Count();
            int days = DateTime.DaysInMonth(Convert.ToDateTime(datefrom).Year, Convert.ToDateTime(datefrom).Month);
            


          


            //int noofday1 = (int)(Convert.ToDateTime(TempData["dateto"]) - Convert.ToDateTime(TempData["datefrom"])).TotalDays;
            List<ChartData> data;

            if (fleettype != "" && fleetname == "" && vesselname == "" && rank == "")
            {
                string[] fleettypes = fleettype.TrimEnd(',').Split(',');
                data =  sc1.getChartDataFT(datefrom, dateto, fleettypes, noofday).ToList();
                

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
                data = sc1.getChartDataFNRK(datefrom, dateto, fleetnames,  ranks, noofday).ToList();

                

            }
            else if (fleettype == "" && fleetname != "" && vesselname != "" && rank == "")
            {
                string[] fleetnames = fleetname.TrimEnd(',').Split(',');
                string[] vesselnames = vesselname.TrimEnd(',').Split(',');
                data = sc1.getChartDataFNVN(datefrom, dateto,  fleetnames, vesselnames,  noofday).ToList();

               
            }
            else if (fleettype != "" && fleetname == "" && vesselname != "" && rank == "")
            {
                string[] fleettypes = fleettype.TrimEnd(',').Split(',');
                string[] vesselnames = vesselname.TrimEnd(',').Split(',');
                data = sc1.getChartDataFTVN(datefrom, dateto, fleettypes,  vesselnames,  noofday).ToList();

                

            }
            else if (fleettype != "" && fleetname == "" && vesselname == "" && rank != "")
            {
                string[] fleettypes = fleettype.TrimEnd(',').Split(',');
                string[] ranks = rank.TrimEnd(',').Split(',');
                data = sc1.getChartDataFTRK(datefrom, dateto, fleettypes,  ranks, noofday).ToList();

               


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

        /*
        [PartialCache("OneMinuteCache")]
        public async Task< JsonResult> Autocompleterank()
        {
            //using (ShipmentContaxt sc1 = new ShipmentContaxt())
            //{
                var bb = await sc.CreReports.Select(x => x.Position).Distinct().ToListAsync();
                
                List<RankClass> students = new List<RankClass>();
                int i = 1;
                foreach (var item in bb)
                {
                    RankClass rc = new RankClass();
                    rc.Id = i;
                    rc.Ranks = item;
                    students.Add(rc);
                    i++;
                }

                return Json(students, JsonRequestBehavior.AllowGet);
            //}
        }
        */


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