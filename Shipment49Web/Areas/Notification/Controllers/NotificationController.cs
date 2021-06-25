using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Web.Mvc;
using NotificationLayer;
using System;
using System.Web.Security;
using System.Web.Routing;
using System.Globalization;
using MenuLayer;
using System.Threading.Tasks;
using System.Data.Entity;
using Shipment49Web.Common;
using System.Diagnostics;
using Shipment49Web.Models;

namespace Shipment49Web.Areas.Notification.Controllers
{
    [Authorize]
    [ErrorClass]
    public class NotificationController : Controller
    {

        private readonly ShipmentContaxt sc1 = new ShipmentContaxt();

        private readonly IMenuRepository sc;
        public NotificationController(IMenuRepository repo)
        {
            sc = repo;
            //getmenulist();


            if (UserRole.username == null)
            {
                UserRole.username = string.Join("", Roles.GetRolesForUser());
            }


            ViewBag.GetMenu = UserRole.username == "Admin" ? sc.Menus.ToList() : sc.Menus.Where(x => x.Role == "User").ToList();

        }



        [Authorize(Roles = "Admin,User")]
        public async Task<ActionResult> Index(DateTime? datefrom, DateTime? dateto, string ids = "", string Acknowledge = "", int page = 1, string sort = "NcDate", string sortdir = "desc", string fleettype = "", string fleetname = "", string search = "", string rank = "", string Searchbtn="")
        {
            

            datefrom = Convert.ToDateTime(datefrom == null ? DateTime.Parse(DateTime.Now.AddMonths(-1).ToString()).ToString("MM/dd/yyyy", CultureInfo.InvariantCulture) : DateTime.Parse(datefrom.ToString()).ToString("MM/dd/yyyy", CultureInfo.InvariantCulture));
            dateto = Convert.ToDateTime(dateto == null ? DateTime.Parse(DateTime.Now.ToString()).ToString("MM/dd/yyyy", CultureInfo.InvariantCulture) : DateTime.Parse(dateto.ToString()).ToString("MM/dd/yyyy", CultureInfo.InvariantCulture));

            fleettype = fleettype.TrimEnd(',');
            fleetname = fleetname.TrimEnd(',');
            search = search.TrimEnd(',');
            rank = rank.TrimEnd(',');

            ViewBag.fleettypebm1 = fleettype;
            ViewBag.fleetnamebm1 = fleetname;
            ViewBag.vesselnamebm1 = search;
            ViewBag.rankbm1 = rank;


            if (!string.IsNullOrEmpty(Acknowledge)) // Function for Acknowledge...
            {
                if (!string.IsNullOrEmpty(ids))
                    AcknowledgeAll(ids);
            }



            //........................

            int pagesize = 10;
            int totalrecords = 0;

            if (page < 1) page = 1;
            int skip = (page * pagesize) - pagesize;
            //var data = GetNotification(datefrom, dateto,search, fleetname,fleettype, sort, sortdir, skip, pagesize, out totalrecords);

            string[] searchs = search.TrimEnd(',').Split(',');
            var nlist = sc.Notifications.Where(x => searchs.Contains(x.VesselName));


            /*
            if (search != "" && fleetname != "" && fleettype != "" && datefrom == null && dateto == null)
            {
                nlist = null;
                string[] fleetnames1 = fleetname.TrimEnd(',').Split(',');
                string[] fleettypes1 = fleettype.TrimEnd(',').Split(',');
                nlist = sc.Notifications.Where
                (x => fleettypes1.Contains(x.FleetType) && fleetnames1.Contains(x.FleetName) && searchs.Contains(x.VesselName));
            }
            else if (search == "" && fleetname != "" && fleettype != "" && datefrom != null && dateto != null)
            {
                nlist = null;
                string[] fleetnames1 = fleetname.TrimEnd(',').Split(',');
                string[] fleettypes1 = fleettype.TrimEnd(',').Split(',');
                nlist = sc.Notifications.Where
                (x => fleettypes1.Contains(x.FleetType) && fleetnames1.Contains(x.FleetName) && (x.NcDate >= datefrom && x.NcDate <= dateto));

                

            }
            else if (search == "" && fleetname != "" && fleettype == "" && datefrom != null && dateto != null)
            {
                nlist = null;
                string[] fleetnames1 = fleetname.TrimEnd(',').Split(',');
                nlist = sc.Notifications.Where
                (x => fleetnames1.Contains(x.FleetName) && (x.NcDate >= datefrom && x.NcDate <= dateto));

                

            }
            else if (search == "" && fleetname == "" && fleettype != "" && datefrom != null && dateto != null)
            {
                nlist = null;
                string[] fleettypes1 = fleettype.TrimEnd(',').Split(',');
                nlist = sc.Notifications.Where
                (x => fleettypes1.Contains(x.FleetType) && (x.NcDate >= datefrom && x.NcDate <= dateto));

                

            }

            else if (search != "" && fleetname != "" && fleettype != "" && datefrom != null && dateto != null)
            {
                nlist = null;
                string[] fleetnames1 = fleetname.TrimEnd(',').Split(',');
                string[] fleettypes1 = fleettype.TrimEnd(',').Split(',');
                nlist = sc.Notifications.Where
                (x => fleettypes1.Contains(x.FleetType) && fleetnames1.Contains(x.FleetName) && searchs.Contains(x.VesselName) && (x.NcDate >= datefrom && x.NcDate <= dateto));

                

            }
            else if (search != "" && fleetname != "" && fleettype == "" && datefrom != null && dateto != null)
            {
                nlist = null;
                string[] fleetnames1 = fleetname.TrimEnd(',').Split(',');
                nlist = sc.Notifications.Where
                (x => fleetnames1.Contains(x.FleetName) && searchs.Contains(x.VesselName) && (x.NcDate >= datefrom && x.NcDate <= dateto));

                

            }
            else if (search != "" && fleetname == "" && fleettype != "" && datefrom != null && dateto != null)
            {
                nlist = null;
                string[] fleettypes1 = fleettype.TrimEnd(',').Split(',');
                nlist = sc.Notifications.Where
                (x => fleettypes1.Contains(x.FleetType) && searchs.Contains(x.VesselName) && (x.NcDate >= datefrom && x.NcDate <= dateto));

                

            }
            else if (search != "" && fleetname == "" && fleettype == "" && datefrom != null && dateto != null)
            {
                nlist = null;
                nlist = sc.Notifications.Where
                (x => searchs.Contains(x.VesselName) && (x.NcDate >= datefrom && x.NcDate <= dateto));

                if (string.IsNullOrEmpty(Searchbtn))
                {
                    if (nlist.Count() == 0)
                    {
                        nlist = null;
                        datefrom = Convert.ToDateTime(DateTime.Parse(DateTime.Now.AddMonths(-3).ToString()).ToString("MM/dd/yyyy", CultureInfo.InvariantCulture));
                        nlist = sc.Notifications.Where
                       (x => searchs.Contains(x.VesselName) && (x.NcDate >= datefrom && x.NcDate <= dateto));
                    }
                    if (nlist.Count() == 0)
                    {
                        nlist = null;
                        datefrom = Convert.ToDateTime(DateTime.Parse(DateTime.Now.AddMonths(-6).ToString()).ToString("MM/dd/yyyy", CultureInfo.InvariantCulture));
                        nlist = sc.Notifications.Where
                       (x => searchs.Contains(x.VesselName) && (x.NcDate >= datefrom && x.NcDate <= dateto));
                    }
                    if (nlist.Count() == 0)
                    {
                        nlist = null;
                        datefrom = Convert.ToDateTime(DateTime.Parse(DateTime.Now.AddMonths(-12).ToString()).ToString("MM/dd/yyyy", CultureInfo.InvariantCulture));
                        nlist = sc.Notifications.Where
                       (x => searchs.Contains(x.VesselName) && (x.NcDate >= datefrom && x.NcDate <= dateto));
                    }
                }

            }
            else 
            {
                nlist = sc.Notifications.Where
                (x => x.NcDate >= datefrom && x.NcDate <= dateto);

                if (string.IsNullOrEmpty(Searchbtn))
                {
                    if (nlist.Count() == 0)
                    {
                        nlist = null;
                        datefrom = Convert.ToDateTime(DateTime.Parse(DateTime.Now.AddMonths(-3).ToString()).ToString("MM/dd/yyyy", CultureInfo.InvariantCulture));
                        nlist = sc.Notifications.Where
                       (x => x.NcDate >= datefrom && x.NcDate <= dateto);
                    }
                    if (nlist.Count() == 0)
                    {
                        nlist = null;
                        datefrom = Convert.ToDateTime(DateTime.Parse(DateTime.Now.AddMonths(-6).ToString()).ToString("MM/dd/yyyy", CultureInfo.InvariantCulture));
                        nlist = sc.Notifications.Where
                       (x => x.NcDate >= datefrom && x.NcDate <= dateto);
                    }
                    if (nlist.Count() == 0)
                    {
                        nlist = null;
                        datefrom = Convert.ToDateTime(DateTime.Parse(DateTime.Now.AddMonths(-12).ToString()).ToString("MM/dd/yyyy", CultureInfo.InvariantCulture));
                        nlist = sc.Notifications.Where
                       (x => x.NcDate >= datefrom && x.NcDate <= dateto);
                    }
                }
            }
            */

            //........New implementation for rank's filter.........

            if (fleettype != "" && fleetname != "" && search != "" && rank != "")
            {
                nlist = null;
                string[] fleetnames1 = fleetname.TrimEnd(',').Split(',');
                string[] fleettypes1 = fleettype.TrimEnd(',').Split(',');
                string[] vesselnames1 = search.TrimEnd(',').Split(',');
                string[] ranks1 = rank.TrimEnd(',').Split(',');

                nlist = sc.Notifications.Where
                (x => fleettypes1.Contains(x.FleetType) && fleetnames1.Contains(x.FleetName) && vesselnames1.Contains(x.VesselName) && ranks1.Contains(x.Rank) && (x.NcDate >= datefrom && x.NcDate <= dateto));

            }

            else if (fleettype != "" && fleetname == "" && search == "" && rank == "")
            {
                nlist = null;
                string[] fleettypes1 = fleettype.TrimEnd(',').Split(',');

                nlist = sc.Notifications.Where
                (x => fleettypes1.Contains(x.FleetType) && (x.NcDate >= datefrom && x.NcDate <= dateto));

            }
            else if (fleettype == "" && fleetname != "" && search == "" && rank == "")
            {

                nlist = null;
                string[] fleetnames1 = fleetname.TrimEnd(',').Split(',');

                nlist = sc.Notifications.Where
                (x => fleetnames1.Contains(x.FleetName) && (x.NcDate >= datefrom && x.NcDate <= dateto));

            }

            else if (fleettype == "" && fleetname == "" && search == "" && rank != "")
            {
                nlist = null;
                string[] ranks1 = rank.TrimEnd(',').Split(',');

                nlist = sc.Notifications.Where
                (x => ranks1.Contains(x.Rank) && (x.NcDate >= datefrom && x.NcDate <= dateto));

            }
            else if (fleettype != "" && fleetname != "" && search == "" && rank == "")
            {
                nlist = null;
                string[] fleetnames1 = fleetname.TrimEnd(',').Split(',');
                string[] fleettypes1 = fleettype.TrimEnd(',').Split(',');
               
                nlist = sc.Notifications.Where
                (x => fleettypes1.Contains(x.FleetType) && fleetnames1.Contains(x.FleetName)  && (x.NcDate >= datefrom && x.NcDate <= dateto));

            }
            else if (fleettype != "" && fleetname != "" && search != "" && rank == "")
            {

                nlist = null;
                string[] fleetnames1 = fleetname.TrimEnd(',').Split(',');
                string[] fleettypes1 = fleettype.TrimEnd(',').Split(',');
                string[] vesselnames1 = search.TrimEnd(',').Split(',');
               
                nlist = sc.Notifications.Where
                (x => fleettypes1.Contains(x.FleetType) && fleetnames1.Contains(x.FleetName) && vesselnames1.Contains(x.VesselName) && (x.NcDate >= datefrom && x.NcDate <= dateto));
            }

            else if (fleettype == "" && fleetname == "" && search != "" && rank != "")
            {
                nlist = null;
                string[] vesselnames1 = search.TrimEnd(',').Split(',');
                string[] ranks1 = rank.TrimEnd(',').Split(',');

                nlist = sc.Notifications.Where
                (x => vesselnames1.Contains(x.VesselName) && ranks1.Contains(x.Rank) && (x.NcDate >= datefrom && x.NcDate <= dateto));

            }
            else if (fleettype == "" && fleetname != "" && search == "" && rank != "")
            {
                nlist = null;
                string[] fleetnames1 = fleetname.TrimEnd(',').Split(',');
                string[] ranks1 = rank.TrimEnd(',').Split(',');

                nlist = sc.Notifications.Where
                (x => fleetnames1.Contains(x.FleetName) && ranks1.Contains(x.Rank) && (x.NcDate >= datefrom && x.NcDate <= dateto));

            }
            else if (fleettype == "" && fleetname != "" && search != "" && rank == "")
            {
                nlist = null;
                string[] fleetnames1 = fleetname.TrimEnd(',').Split(',');
                string[] vesselnames1 = search.TrimEnd(',').Split(',');

                nlist = sc.Notifications.Where
                (x => fleetnames1.Contains(x.FleetName) && vesselnames1.Contains(x.VesselName) && (x.NcDate >= datefrom && x.NcDate <= dateto));

            }
            else if (fleettype != "" && fleetname == "" && search != "" && rank == "")
            {
                nlist = null;
                string[] fleettypes1 = fleettype.TrimEnd(',').Split(',');
                string[] vesselnames1 = search.TrimEnd(',').Split(',');

                nlist = sc.Notifications.Where
                (x => fleettypes1.Contains(x.FleetType)  && vesselnames1.Contains(x.VesselName)  && (x.NcDate >= datefrom && x.NcDate <= dateto));

            }
            else if (fleettype != "" && fleetname == "" && search == "" && rank != "")
            {
                nlist = null;
                string[] fleettypes1 = fleettype.TrimEnd(',').Split(',');
                string[] ranks1 = rank.TrimEnd(',').Split(',');

                nlist = sc.Notifications.Where
                (x => fleettypes1.Contains(x.FleetType) && ranks1.Contains(x.Rank) && (x.NcDate >= datefrom && x.NcDate <= dateto));

            }
            else if (fleettype == "" && fleetname != "" && search != "" && rank != "")
            {
                nlist = null;
                string[] fleetnames1 = fleetname.TrimEnd(',').Split(',');
                string[] vesselnames1 = search.TrimEnd(',').Split(',');
                string[] ranks1 = rank.TrimEnd(',').Split(',');

                nlist = sc.Notifications.Where
                (x => fleetnames1.Contains(x.FleetName) && vesselnames1.Contains(x.VesselName) && ranks1.Contains(x.Rank) && (x.NcDate >= datefrom && x.NcDate <= dateto));

            }
            else if (fleettype != "" && fleetname != "" && search == "" && rank != "")
            {
                nlist = null;
                string[] fleetnames1 = fleetname.TrimEnd(',').Split(',');
                string[] fleettypes1 = fleettype.TrimEnd(',').Split(',');
                string[] ranks1 = rank.TrimEnd(',').Split(',');
                nlist = sc.Notifications.Where
                (x => fleettypes1.Contains(x.FleetType) && fleetnames1.Contains(x.FleetName) && ranks1.Contains(x.Rank) && (x.NcDate >= datefrom && x.NcDate <= dateto));

            }
            else if (fleettype == "" && fleetname == "" && search != "" && rank == "")
            {

                nlist = null;
                string[] vesselnames1 = search.TrimEnd(',').Split(',');
                nlist = sc.Notifications.Where
                (x => vesselnames1.Contains(x.VesselName) && (x.NcDate >= datefrom && x.NcDate <= dateto));

                if (string.IsNullOrEmpty(Searchbtn))
                {
                    if (nlist.Count() == 0)
                    {
                        nlist = null;
                        datefrom = Convert.ToDateTime(DateTime.Parse(DateTime.Now.AddMonths(-3).ToString()).ToString("MM/dd/yyyy", CultureInfo.InvariantCulture));
                        nlist = sc.Notifications.Where
                       (x => vesselnames1.Contains(x.VesselName) && (x.NcDate >= datefrom && x.NcDate <= dateto));
                    }
                    if (nlist.Count() == 0)
                    {
                        nlist = null;
                        datefrom = Convert.ToDateTime(DateTime.Parse(DateTime.Now.AddMonths(-6).ToString()).ToString("MM/dd/yyyy", CultureInfo.InvariantCulture));
                        nlist = sc.Notifications.Where
                       (x => vesselnames1.Contains(x.VesselName) && (x.NcDate >= datefrom && x.NcDate <= dateto));
                    }
                    if (nlist.Count() == 0)
                    {
                        nlist = null;
                        datefrom = Convert.ToDateTime(DateTime.Parse(DateTime.Now.AddMonths(-12).ToString()).ToString("MM/dd/yyyy", CultureInfo.InvariantCulture));
                        nlist = sc.Notifications.Where
                       (x => vesselnames1.Contains(x.VesselName) && (x.NcDate >= datefrom && x.NcDate <= dateto));
                    }
                }

            }
            else
            {
                nlist = sc.Notifications.Where
               (x => x.NcDate >= datefrom && x.NcDate <= dateto);

                if (string.IsNullOrEmpty(Searchbtn))
                {
                    if (nlist.Count() == 0)
                    {
                        nlist = null;
                        datefrom = Convert.ToDateTime(DateTime.Parse(DateTime.Now.AddMonths(-3).ToString()).ToString("MM/dd/yyyy", CultureInfo.InvariantCulture));
                        nlist = sc.Notifications.Where
                       (x => x.NcDate >= datefrom && x.NcDate <= dateto);
                    }
                    if (nlist.Count() == 0)
                    {
                        nlist = null;
                        datefrom = Convert.ToDateTime(DateTime.Parse(DateTime.Now.AddMonths(-6).ToString()).ToString("MM/dd/yyyy", CultureInfo.InvariantCulture));
                        nlist = sc.Notifications.Where
                       (x => x.NcDate >= datefrom && x.NcDate <= dateto);
                    }
                    if (nlist.Count() == 0)
                    {
                        nlist = null;
                        datefrom = Convert.ToDateTime(DateTime.Parse(DateTime.Now.AddMonths(-12).ToString()).ToString("MM/dd/yyyy", CultureInfo.InvariantCulture));
                        nlist = sc.Notifications.Where
                       (x => x.NcDate >= datefrom && x.NcDate <= dateto);
                    }
                }

            }


            //........End New implementation for rank's filter........




            //....................
            ViewBag.Ack = nlist.Where(x => x.AcknDate != null).Count();
            ViewBag.UnAck = nlist.Where(x => x.NonConfirmity != null).Count();
            //...................


            totalrecords = nlist.Count();

            string bmsasc = (sort + sortdir).ToLower();
            if (bmsasc == "ncdatedesc")
            {
                nlist = nlist.OrderBy(u => u.AcknDate);
            }
            else if (bmsasc == "ncdateasc")
            {
                nlist = nlist.OrderBy(u => u.NcDate).ThenBy(u => u.AcknDate);
            }
            else
            {
                nlist = nlist.OrderBy(sort + " " + sortdir);
            }


            //switch (bmsasc)
            //{
               
            //    case "ncdatedesc":
            //        nlist = nlist.OrderBy(u => u.AcknDate);
           
            //    default:
            //        nlist = nlist.OrderBy(u => u.NcDate).ThenBy(u => u.AcknDate);
            //        break;

            //}


           
          
            if (pagesize > 0)
            {
                nlist = nlist.Skip(skip).Take(pagesize);
            }

            

            ViewBag.TotalRows = totalrecords;
            TempData["page"] = page;
            BmsPageNumber.Pagenumber = page;
            if (page == 1 || search != "" || fleetname != "" || fleettype != "")
            {
                BmsPageNumber.FleetType = fleettype;
                BmsPageNumber.FleetName = fleetname;
                BmsPageNumber.VesselName = search;
                BmsPageNumber.datefrom = datefrom.ToString();
                BmsPageNumber.dateto = dateto.ToString();
            }
            ViewBag.vname = search;
            ViewBag.fleettype = BmsPageNumber.FleetType;
            ViewBag.fleetname = BmsPageNumber.FleetName;
               

            ViewBag.Messagegrid = nlist.Count() > 0 ? "" : ViewBag.Messagegrid = "Data not available for selected criteria! please select the other criteria.";


            //ViewBag.Ack = sc.Notifications.Where(x => x.AcknDate != null).Count();
            //ViewBag.UnAck = sc.Notifications.Where(x => x.NonConfirmity != null).Count();

          

            ViewBag.datefrom = DateTime.Parse(datefrom.ToString()).ToString("MM/dd/yyyy", CultureInfo.InvariantCulture);
            ViewBag.dateto = DateTime.Parse(dateto.ToString()).ToString("MM/dd/yyyy", CultureInfo.InvariantCulture);

            //..................
            ViewBag.fleettypebm = sc.AutoCompletedFleetType;
            ViewBag.fleetnamebm = sc.AutoCompletedfleetname;
            ViewBag.vesselnamebm = sc.AutoCompletevessel;
            ViewBag.rankebm = sc.AutoCompleterank;
            //......................



            return View(await nlist.ToListAsync());

        }
        /*
        [NonAction]
        private List<NotificationClass> GetNotification(DateTime? datefrom,DateTime? dateto, string search, string fleetname,string fleettype, string sort, string sortdir, int skip, int pagesize, out int totalrecord)
        {
            string[] searchs = search.TrimEnd(',').Split(',');
            var nlist = sc.Notifications.Where(x => searchs.Contains(x.VesselName));

            if (search !="" && fleetname !="" && fleettype !="" && datefrom == null && dateto == null)
            {
                string[] fleetnames = fleetname.TrimEnd(',').Split(',');
                string[] fleettypes = fleettype.TrimEnd(',').Split(',');
                nlist = sc.Notifications.Where
                (x => fleettypes.Contains(x.FleetType) && fleetnames.Contains(x.FleetName) && searchs.Contains(x.VesselName));
            }
            else if (search == "" && fleetname !="" && fleettype !="" && datefrom != null && dateto != null)
            {
                string[] fleetnames = fleetname.TrimEnd(',').Split(',');
                string[] fleettypes = fleettype.TrimEnd(',').Split(',');
                nlist = sc.Notifications.Where
                (x => fleettypes.Contains(x.FleetType) && fleetnames.Contains(x.FleetName) && (x.NcDate >= datefrom && x.NcDate <= dateto));
                
            }
            else if (search ==""  && fleetname != "" && fleettype == "" && datefrom != null && dateto != null)
            {
                string[] fleetnames = fleetname.TrimEnd(',').Split(',');
               
                nlist = sc.Notifications.Where
                (x => fleetnames.Contains(x.FleetName) && (x.NcDate >= datefrom && x.NcDate <= dateto));
            }
            else if (search == "" && fleetname == "" && fleettype != "" && datefrom != null && dateto != null)
            {
               
                string[] fleettypes = fleettype.TrimEnd(',').Split(',');
                nlist = sc.Notifications.Where
                (x => fleettypes.Contains(x.FleetType) && (x.NcDate >= datefrom && x.NcDate <= dateto));
            }

            else if (search != "" && fleetname != "" && fleettype != "" && datefrom != null && dateto != null)
            {
                string[] fleetnames = fleetname.TrimEnd(',').Split(',');
                string[] fleettypes = fleettype.TrimEnd(',').Split(',');
                nlist = sc.Notifications.Where
                (x => fleettypes.Contains(x.FleetType) && fleetnames.Contains(x.FleetName)&& searchs.Contains(x.VesselName)  && (x.NcDate >= datefrom && x.NcDate <= dateto));
            }
            else if (search != "" && fleetname != "" && fleettype == "" && datefrom != null && dateto != null)
            {
                string[] fleetnames = fleetname.TrimEnd(',').Split(',');
                
                nlist = sc.Notifications.Where
                (x =>  fleetnames.Contains(x.FleetName) && searchs.Contains(x.VesselName) && (x.NcDate >= datefrom && x.NcDate <= dateto));
            }
            else if (search != "" && fleetname == "" && fleettype != "" && datefrom != null && dateto != null)
            {
                
                string[] fleettypes = fleettype.TrimEnd(',').Split(',');
                nlist = sc.Notifications.Where
                (x => fleettypes.Contains(x.FleetType) && searchs.Contains(x.VesselName) && (x.NcDate >= datefrom && x.NcDate <= dateto));
            }
            else if (search != "" && fleetname == "" && fleettype == "" && datefrom != null && dateto != null)
            {
                
                nlist = sc.Notifications.Where
                (x => searchs.Contains(x.VesselName) && (x.NcDate >= datefrom && x.NcDate <= dateto));
            }
            else if (search == "" && fleetname == "" && fleettype == "" && datefrom != null && dateto != null)
            {
                nlist = sc.Notifications.Where
                (x => x.NcDate >= datefrom && x.NcDate <= dateto);
            }
           

            totalrecord = nlist.Count();
            nlist = nlist.OrderBy(sort + " " + sortdir);
            if (pagesize > 0)
            {
                nlist = nlist.Skip(skip).Take(pagesize);
            }
            return nlist.ToList();
        }
        */

        /*
        [NonAction]
        private List<CommentClass> GetComment( int? id, string search, string sort, string sortdir, int skip, int pagesize, out int totalrecord)
        {
            var nlist = sc.Comments1.Where
                (x => x.Nid == id);
            if (search != null)
            {
                nlist = sc.Comments1.Where
                (x => x.CommentBy.Contains(search) && x.Nid == id);
            }

            totalrecord = nlist.Count();
            nlist = nlist.OrderBy(sort + " " + sortdir);
            if (pagesize > 0)
            {
                nlist = nlist.Skip(skip).Take(pagesize);
            }
            return nlist.ToList();
        }
        */


        private  void AcknowledgeAll(string id)
        {
            using (ShipmentContaxt sc2 = new ShipmentContaxt())
            {
                sc2.Configuration.ProxyCreationEnabled = false;

                string roleNames = string.Join("", Roles.GetRolesForUser());
                string bms = "Acknowledged on " + DateTime.Now.ToString("dd-MMM-yyyy") + " By " + roleNames;

                string[] idack = id.TrimEnd(',').Split(',');

                foreach (var item in idack)
                {
                    int idi = int.Parse(item);
                    var bmcheck =  sc2.Notifications.Where(p => p.Nid.Equals(idi)).Select(x => x.Acknowledge).FirstOrDefault();
                    if (bmcheck == null)
                    {
                        var user = new NotificationClass() { Nid = int.Parse(item), Acknowledge = bms, AcknDate = DateTime.Now };

                        sc2.Notifications.Attach(user);
                        sc2.Entry(user).Property(x => x.Acknowledge).IsModified = true;
                        sc2.Entry(user).Property(x => x.AcknDate).IsModified = true;
                         sc2.SaveChanges();
                    }
                } 
                
            }

        }

        [Authorize(Roles = "Admin,User")]
        public async Task<ActionResult> Acknowledge(DateTime? datefrom, DateTime dateto, string search, string fleetname, string fleettype, int id)
        {
            using (ShipmentContaxt sc1 = new ShipmentContaxt())
            {
                sc1.Configuration.ProxyCreationEnabled = false;

                string roleNames = string.Join("", Roles.GetRolesForUser());
                string bms = "Acknowledged on " + DateTime.Now.ToString("dd-MMM-yyyy") + " By " + roleNames;


                var user = new NotificationClass() { Nid = id, Acknowledge = bms, AcknDate = DateTime.Now };

                sc1.Notifications.Attach(user);
                sc1.Entry(user).Property(x => x.Acknowledge).IsModified = true;
                sc1.Entry(user).Property(x => x.AcknDate).IsModified = true;
                await sc1.SaveChangesAsync();

                return RedirectToAction("index", new RouteValueDictionary(
       new { controller = "notification", area = "notification", datefrom, dateto, search, fleetname, fleettype, page = (int)TempData["page"] }));

            }
        }

        public async Task<ActionResult> comment(int? id, int page = 1, string sort = "CDate", string sortdir = "desc", string search = "")
        {
            int pagesize = 10;
            int totalrecords = 0;

            if (page < 1) page = 1;
            int skip = (page * pagesize) - pagesize;
            //var data = GetComment(id,search, sort, sortdir, skip, pagesize, out totalrecords);
            var nlist = sc.Comments1.Where
                (x => x.Nid == id);
            if (search != "")
            {
                nlist = sc.Comments1.Where
                (x => x.CommentBy.Contains(search) && x.Nid == id);
            }

            totalrecords = nlist.Count();
            nlist = nlist.OrderBy(sort + " " + sortdir);
            if (pagesize > 0)
            {
                nlist = nlist.Skip(skip).Take(pagesize);
            }


            ViewBag.TotalRows = totalrecords;
            ViewBag.Nid = id;

            ViewBag.Messagegrid = nlist.Count() > 0 ? "" : ViewBag.Messagegrid = "Data not available for selected criteria! please select the other criteria.";

            ViewBag.page = BmsPageNumber.Pagenumber;
            ViewBag.fleettype = BmsPageNumber.FleetType;
            ViewBag.fleetname = BmsPageNumber.FleetName;
            ViewBag.vname = BmsPageNumber.VesselName;
            ViewBag.datefrom = BmsPageNumber.datefrom;
            ViewBag.dateto = BmsPageNumber.dateto;


            return View(await nlist.ToListAsync());
        }

        [HttpGet]
        public ActionResult Create(CommentClass cm, int id)
        {
            string roleNames = string.Join("", Roles.GetRolesForUser());
            cm.Nid = id;
            cm.CommentBy = roleNames;

            //..................
            ViewBag.page = BmsPageNumber.Pagenumber;
            ViewBag.fleettype = BmsPageNumber.FleetType;
            ViewBag.fleetname = BmsPageNumber.FleetName;
            ViewBag.vname = BmsPageNumber.VesselName;
            ViewBag.datefrom = BmsPageNumber.datefrom;
            ViewBag.dateto = BmsPageNumber.dateto;
            //...............

            return View(cm);
        }
        [HttpPost]
        public async Task<ActionResult> Create(CommentClass cm)
        {
            using (ShipmentContaxt sc1 = new ShipmentContaxt())
            {

                sc1.Configuration.ProxyCreationEnabled = false;

                cm.CDate = DateTime.Now;

                if (ModelState.IsValid)
                {
                    sc1.Comments1.Add(cm);
                    await sc1.SaveChangesAsync();

                    return RedirectToAction("comment", new RouteValueDictionary(
        new { controller = "notification", area = "notification", Id = cm.Nid }));

                }
                else
                {
                    return View(cm);
                }
            }
        }



      

        //public async Task<ActionResult> AutoComplete4(string term)
        //{


        //    List<string> students = await sc.Vessels.Where(s => s.VesselName.ToLower().Contains(term.ToLower()))
        //        .Select(x => x.VesselName).Distinct().ToListAsync();

        //    return Json(students, JsonRequestBehavior.AllowGet);
        //}


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // sc.Dispose();

            }

            base.Dispose(disposing);
        }

    }
}