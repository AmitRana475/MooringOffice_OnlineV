using AnalysisLayer;
using ClosedXML.Excel;
using CrewReportLayer;
using MenuLayer;
using Newtonsoft.Json;
using Shipment49Web.Common;
using Shipment49Web.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Entity;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Dynamic;
using System.Threading;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace Shipment49Web.Areas.Analysis.Controllers
{
    [Authorize]
    [UserAuthenticationFilter]
    [ErrorClass]
    public class StatisticalTrendViewController : BaseController
    {
        private readonly IMenuRepository sc;

        private readonly ShipmentContaxt sc1 = new ShipmentContaxt();
        JsonSerializerSettings _jsonSetting = new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore };

        public StatisticalTrendViewController()
        {

            //sc = repo;
            //if (UserRole.username == null)
            //{
            //    UserRole.username = string.Join("", Roles.GetRolesForUser());
            //}
            //ViewBag.GetMenu = UserRole.username == "Admin" ? sc.Menus.ToList() : sc.Menus.Where(x => x.Role == "User").ToList();
        }

        public ActionResult Index(DateTime? datefrom, DateTime? dateto, int page = 1, string sort = "VesselName", string sortdir = "asc", string fleettype = "", string fleetname = "", string vesselname = "", string rank = "", string Searchbtn = "", string searchTerm = "")
        {
            //UserRole.CheckAnalysis = true;

            datefrom = Convert.ToDateTime(datefrom == null ? DateTime.Parse(DateTime.Now.AddMonths(-1).ToString()).ToString("MM/dd/yyyy", CultureInfo.InvariantCulture) : DateTime.Parse(datefrom.ToString()).ToString("MM/dd/yyyy", CultureInfo.InvariantCulture));
            dateto = Convert.ToDateTime(dateto == null ? DateTime.Parse(DateTime.Now.ToString()).ToString("MM/dd/yyyy", CultureInfo.InvariantCulture) : DateTime.Parse(dateto.ToString()).ToString("MM/dd/yyyy", CultureInfo.InvariantCulture));



            fleettype = fleettype.TrimEnd(',').Replace("multiselect-all", "").TrimStart(',').Trim();
            fleetname = fleetname.TrimEnd(',').Replace("multiselect-all", "").TrimStart(',').Trim();
            vesselname = vesselname.TrimEnd(',').Replace("multiselect-all", "").TrimStart(',').Trim();
            rank = rank.TrimEnd(',').Replace("multiselect-all", "").TrimStart(',').Trim();


            ViewBag.fleettypebm1 = fleettype;
            ViewBag.fleetnamebm1 = fleetname;
            ViewBag.vesselnamebm1 = vesselname;
            ViewBag.rankbm1 = rank;



            IEnumerable<CreReportClass> data1 = null;

            if (string.IsNullOrEmpty(Searchbtn))
            {
                string[] searchs = vesselname.TrimEnd(',').Split(',');
                IEnumerable<CreReportClass> data2 = GetCerti1.Invoke(sc);
                data1 = data2.Where(x => (x.Dates >= datefrom && x.Dates <= dateto));
                if (data1.Count() == 0)
                {
                    data1 = null;
                    datefrom = Convert.ToDateTime(DateTime.Parse(DateTime.Now.AddMonths(-3).ToString()).ToString("MM/dd/yyyy", CultureInfo.InvariantCulture));
                    data1 = data2.Where(x => (x.Dates >= datefrom && x.Dates <= dateto));

                    if (UserRole.username.ToLower() != "admin")
                    {
                        var vsname = sc.Users.Where(x => x.EmailId.Equals(UserRole.username1)).Select(x => x.VesselName).FirstOrDefault();
                        string[] vesselname1 = vsname.TrimEnd(',').Split(',');
                        data1 = data1.Where(x => vesselname1.Contains(x.VesselName));
                    }
                    if (!string.IsNullOrEmpty(vesselname))
                        data1 = data1.Where(x => searchs.Contains(x.VesselName));
                }
                if (data1.Count() == 0)
                {
                    data1 = null;
                    datefrom = Convert.ToDateTime(DateTime.Parse(DateTime.Now.AddMonths(-6).ToString()).ToString("MM/dd/yyyy", CultureInfo.InvariantCulture));
                    data1 = data2.Where(x => (x.Dates >= datefrom && x.Dates <= dateto));

                    if (UserRole.username.ToLower() != "admin")
                    {
                        var vsname = sc.Users.Where(x => x.EmailId.Equals(UserRole.username1)).Select(x => x.VesselName).FirstOrDefault();
                        string[] vesselname1 = vsname.TrimEnd(',').Split(',');
                        data1 = data1.Where(x => vesselname1.Contains(x.VesselName));
                    }
                    if (!string.IsNullOrEmpty(vesselname))
                        data1 = data1.Where(x => searchs.Contains(x.VesselName));
                }
                if (data1.Count() == 0)
                {
                    data1 = null;
                    datefrom = Convert.ToDateTime(DateTime.Parse(DateTime.Now.AddMonths(-12).ToString()).ToString("MM/dd/yyyy", CultureInfo.InvariantCulture));
                    data1 = data2.Where(x => (x.Dates >= datefrom && x.Dates <= dateto));

                    if (UserRole.username.ToLower() != "admin")
                    {
                        var vsname = sc.Users.Where(x => x.EmailId.Equals(UserRole.username1)).Select(x => x.VesselName).FirstOrDefault();
                        string[] vesselname1 = vsname.TrimEnd(',').Split(',');
                        data1 = data1.Where(x => vesselname1.Contains(x.VesselName));
                    }
                    if (!string.IsNullOrEmpty(vesselname))
                        data1 = data1.Where(x => searchs.Contains(x.VesselName));
                }






            }
            else
            {
                data1 = GetCerti1.Invoke(sc).Where(x => (x.Dates >= datefrom && x.Dates <= dateto));
                if (!string.IsNullOrEmpty(fleettype))
                {
                    string[] fleettypes = fleettype.TrimEnd(',').Split(',');
                    data1 = data1.Where(x => fleettypes.Contains(x.FleetType));
                }
                if (!string.IsNullOrEmpty(fleetname))
                {
                    string[] fleetnames = fleetname.TrimEnd(',').Split(',');
                    data1 = data1.Where(x => fleetnames.Contains(x.FleetName));
                }
                if (!string.IsNullOrEmpty(vesselname))
                {
                    string[] searchs = vesselname.TrimEnd(',').Split(',');
                    data1 = data1.Where(x => searchs.Contains(x.VesselName));
                }
                if (!string.IsNullOrEmpty(rank))
                {
                    string[] ranks1 = rank.TrimEnd(',').Split(',');
                    data1 = data1.Where(x => ranks1.Contains(x.Position));
                }

                if (UserRole.username.ToLower() != "admin")
                {
                    var vsname = sc.Users.Where(x => x.EmailId.Equals(UserRole.username1)).Select(x => x.VesselName).FirstOrDefault();
                    string[] vesselname1 = vsname.TrimEnd(',').Split(',');
                    data1 = data1.Where(x => vesselname1.Contains(x.VesselName));
                }

            }

            ViewBag.datefrom = DateTime.Parse(datefrom.ToString()).ToString("MM/dd/yyyy", CultureInfo.InvariantCulture);
            ViewBag.dateto = DateTime.Parse(dateto.ToString()).ToString("MM/dd/yyyy", CultureInfo.InvariantCulture);

            var data = sc.getGraphicalView(data1.ToList(), (DateTime)datefrom, (DateTime)dateto);


            ViewBag.Datacount = data.Count() > 0 ? "Graphical Trend View" : "No data availble please select another range";

            int pagesize = 10;
            int totalrecords = 0;

            if (page < 1) page = 1;
            int skip = (page * pagesize) - pagesize;

            totalrecords = data.Count();
            data = data.OrderBy(sort + " " + sortdir);
            if (pagesize > 0)
            {
                data = data.Skip(skip).Take(pagesize);
            }

            ViewBag.TotalRows = totalrecords;





            //..................
            ViewBag.fleettypebm = sc.AutoCompletedFleetType;
            ViewBag.fleetnamebm = sc.AutoCompletedfleetname;
            ViewBag.vesselnamebm = sc.AutoCompletevessel;
            ViewBag.rankebm = sc.AutoCompleterank;
            //......................


            if (!string.IsNullOrEmpty(searchTerm))
            {



                //............................

                //stored result into datatable  
                DataTable bb = LINQResultToDataTable(data.Select(x => new { x.VesselId, x.VesselName, x.Work, x.Rest, x.Deviation, x.From, x.To }));



                DataTable dt = bb;
                dt.TableName = "StatisticalView";


                using (XLWorkbook wb = new XLWorkbook())
                {

                    var protectedsheet = wb.Worksheets.Add(dt);
                    var projection = protectedsheet.Protect("bms123");
                    projection.InsertColumns = true;
                    projection.InsertRows = true;

                    wb.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    wb.Style.Font.Bold = true;


                    DateTime today = DateTime.Today;

                    string HeaderName = "StatisticalView_Export" + "_" + today.ToString("dd-MMM-yyyy");

                    Response.Clear();
                    Response.BufferOutput = true;
                    Response.Charset = "";
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    Response.AddHeader("content-disposition", "attachment;filename= " + HeaderName + ".xlsx");

                    using (MemoryStream MyMemoryStream = new MemoryStream())
                    {

                        wb.SaveAs(MyMemoryStream);

                        MyMemoryStream.WriteTo(Response.OutputStream);

                        Response.End();



                    }
                    Thread.Sleep(200);

                }


            }



            //return RedirectToAction("index", new RouteValueDictionary(
            //new { controller = "notification", area = "notification" }));



            return View(data.ToList());

        }

        private DataTable LINQResultToDataTable<T>(IEnumerable<T> Linqlist)
        {


            PropertyDescriptorCollection props = TypeDescriptor.GetProperties(typeof(T));

            DataTable table = new DataTable();
            for (int i = 0; i < props.Count; i++)
            {
                PropertyDescriptor prop = props[i];
                table.Columns.Add(prop.Name, prop.PropertyType);
            }
            object[] values = new object[props.Count];
            foreach (T item in Linqlist)
            {
                for (int i = 0; i < values.Length; i++)
                {
                    values[i] = props[i].GetValue(item);
                }
                table.Rows.Add(values);
            }

            return table;
        }

        private readonly Func<IMenuRepository, IEnumerable<CreReportClass>> GetCerti1 = c => c.CreReports.ToList();

        public ActionResult Detail(DateTime? datefrom, DateTime? dateto, string vesselname = "", string rank = "")
        {

            List<ChartData> data = new List<ChartData>();
            if (vesselname != "" && rank == "")
            {

                string[] vesselnames = vesselname.TrimEnd(',').Split(',');
                data = sc1.getChartDataVN(datefrom, dateto, vesselnames).ToList();

            }
            else
            {
                string[] ranks = rank.TrimEnd(',').Replace("multiselect-all", "").TrimStart(',').Trim().Split(',');
                string[] vesselnames = vesselname.TrimEnd(',').Replace("multiselect-all", "").TrimStart(',').Trim().Split(',');
                data = sc1.getChartDataVNRK(datefrom, dateto, vesselnames, ranks).ToList();

            }



            ViewBag.datefrom1 = DateTime.Parse(datefrom.ToString()).ToString("dd-MMM-yyyy");
            ViewBag.dateto1 = DateTime.Parse(dateto.ToString()).ToString("dd-MMM-yyyy");

            ViewBag.datefrom = DateTime.Parse(datefrom.ToString()).ToString("MM/dd/yyyy", CultureInfo.InvariantCulture);
            ViewBag.dateto = DateTime.Parse(dateto.ToString()).ToString("MM/dd/yyyy", CultureInfo.InvariantCulture);
            ViewBag.vesselname = vesselname;
            ViewBag.rank = rank == "" ? "All Ranks" : rank;

            ViewBag.Datacount1 = data.Count > 0 ? "Graphical View (Work-Hours) " : "No data availble, please select another range";
            ViewBag.Datacount2 = data.Count > 0 ? "Graphical View (Rest-Hours)" : "No data availble, please select another range";
            ViewBag.Datacount3 = data.Count > 0 ? "Graphical View (Deviation)" : "No data availble, please select another range";

            ViewBag.DataPoints = JsonConvert.SerializeObject(data, _jsonSetting);


            return View();
        }


        public ActionResult Statistical(DateTime? datefrom, DateTime? dateto, int page = 1, string sort = "Rank", string sortdir = "asc", string vesselname = "", string rank = "", string searchTerm = "")
        {

            if (datefrom == null)
            {
                datefrom = Convert.ToDateTime(TempData["datefrom1"]);
                dateto = Convert.ToDateTime(TempData["dateto1"]);
                vesselname = TempData["vesselname"].ToString();
                rank = TempData["rank"].ToString();
            }

            IEnumerable<ChartData> data = new List<ChartData>();
            if (vesselname != "" && rank == "")
            {

                string[] vesselnames = vesselname.TrimEnd(',').Split(',');
                data = sc1.getChartDataVN(datefrom, dateto, vesselnames).ToList();

            }
            else
            {
                string[] ranks = rank.TrimEnd(',').Replace("multiselect-all", "").TrimStart(',').Trim().Split(',');
                string[] vesselnames = vesselname.TrimEnd(',').Replace("multiselect-all", "").TrimStart(',').Trim().Split(',');
                data = sc1.getChartDataVNRK(datefrom, dateto, vesselnames, ranks).ToList();

            }

            //int pagesize = 10;
            int totalrecords = 0;

            if (page < 1) page = 1;
            //int skip = (page * pagesize) - pagesize;

            totalrecords = data.Count();
            data = data.OrderBy(sort + " " + sortdir);
            //if (pagesize > 0)
            //{
            // data = data.Skip(skip).Take(pagesize);
            //}

            ViewBag.TotalRows = totalrecords;


            TempData["datefrom1"] = DateTime.Parse(datefrom.ToString()).ToString("dd-MMM-yyyy");
            TempData["dateto1"] = DateTime.Parse(dateto.ToString()).ToString("dd-MMM-yyyy");
            TempData["vesselname"] = vesselname;
            TempData["rank"] = rank;

            ViewBag.datefrom1 = DateTime.Parse(datefrom.ToString()).ToString("dd-MMM-yyyy");
            ViewBag.dateto1 = DateTime.Parse(dateto.ToString()).ToString("dd-MMM-yyyy");

            ViewBag.vesselname = vesselname;

            ViewBag.Messagegrid = data.Count() > 0 ? "" : ViewBag.Messagegrid = "Data Not Available For Selected Criteria! Please Select The Other Criteria.";


            if (!string.IsNullOrEmpty(searchTerm))
            {
                //............................



                //stored result into datatable  
                DataTable bb = LINQResultToDataTable(data.Select(x => new { x.VesselName, x.Rank, x.Work, x.Rest, x.Deviation, From = ViewBag.datefrom1, To = ViewBag.dateto1 }));



                DataTable dt = bb;
                dt.TableName = "StatisticalTrendView";


                using (XLWorkbook wb = new XLWorkbook())
                {

                    var protectedsheet = wb.Worksheets.Add(dt);
                    var projection = protectedsheet.Protect("bms123");
                    projection.InsertColumns = true;
                    projection.InsertRows = true;

                    wb.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    wb.Style.Font.Bold = true;


                    DateTime today = DateTime.Today;

                    string HeaderName = "StatisticalTrendView_Export" + "_" + today.ToString("dd-MMM-yyyy");

                    Response.Clear();
                    Response.BufferOutput = true;
                    Response.Charset = "";
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    Response.AddHeader("content-disposition", "attachment;filename= " + HeaderName + ".xlsx");

                    using (MemoryStream MyMemoryStream = new MemoryStream())
                    {

                        wb.SaveAs(MyMemoryStream);

                        MyMemoryStream.WriteTo(Response.OutputStream);

                        Response.End();



                    }
                    Thread.Sleep(100);

                }


            }


            return View(data);
        }


    }
}