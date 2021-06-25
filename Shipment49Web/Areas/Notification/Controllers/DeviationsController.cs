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
    public class DeviationsController : Controller
    {

        private readonly ShipmentContaxt sc1 = new ShipmentContaxt();

        private readonly IMenuRepository sc;
        public DeviationsController(IMenuRepository repo)
        {
            sc = repo;
            //getmenulist();


            if (UserRole.username == null)
            {
                UserRole.username = string.Join("", Roles.GetRolesForUser());
            }


            ViewBag.GetMenu = UserRole.username == "Admin" ? sc.Menus.ToList() : sc.Menus.Where(x => x.Role == "User").ToList();

        }

        private readonly Func<IMenuRepository, IEnumerable<NotificationClass>> GetNoti = c => c.Notifications.ToList();

        [Authorize(Roles = "Admin,User")]
        public ActionResult Index(DateTime? datefrom, DateTime? dateto, string ids = "", string Acknowledge = "", int page = 1, string sort = "NcDate", string sortdir = "desc", string fleettype = "", string fleetname = "", string search = "", string rank = "", string Searchbtn="")
        {

            var now = DateTime.Now;
            var first = new DateTime(now.Year, now.Month, 1);
            var last = first.AddMonths(1).AddDays(-1);

            datefrom = Convert.ToDateTime(datefrom == null ? DateTime.Parse(first.ToString()).ToString("MM/dd/yyyy", CultureInfo.InvariantCulture) : DateTime.Parse(datefrom.ToString()).ToString("MM/dd/yyyy", CultureInfo.InvariantCulture));
            dateto = Convert.ToDateTime(dateto == null ? DateTime.Parse(last.ToString()).ToString("MM/dd/yyyy", CultureInfo.InvariantCulture) : DateTime.Parse(dateto.ToString()).ToString("MM/dd/yyyy", CultureInfo.InvariantCulture));

            fleettype = fleettype.TrimEnd(',').Replace("multiselect-all", "").TrimStart(',').Trim();
            fleetname = fleetname.TrimEnd(',').Replace("multiselect-all", "").TrimStart(',').Trim();
            search = search.TrimEnd(',').Replace("multiselect-all", "").TrimStart(',').Trim();
            rank = rank.TrimEnd(',').Replace("multiselect-all", "").TrimStart(',').Trim();

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
            

            IEnumerable<NotificationClass> nlist = null;

            if (string.IsNullOrEmpty(Searchbtn))
            {

                nlist = GetTreeData(search, datefrom, dateto);
            }
            else
            {
                nlist = GetNoti.Invoke(sc).Where(x => (x.NcDate >= datefrom && x.NcDate <= dateto));
                if (!string.IsNullOrEmpty(fleettype))
                {
                    string[] fleettypes1 = fleettype.TrimEnd(',').Split(',');
                    nlist = nlist.Where(x => fleettypes1.Contains(x.FleetType) && (x.NcDate >= datefrom && x.NcDate <= dateto));
                }
                if (!string.IsNullOrEmpty(fleetname))
                {
                    string[] fleetnames1 = fleetname.TrimEnd(',').Split(',');
                    nlist = nlist.Where(x => fleetnames1.Contains(x.FleetName) && (x.NcDate >= datefrom && x.NcDate <= dateto));
                }
                if (!string.IsNullOrEmpty(search))
                {
                    string[] vesselnames1 = search.TrimEnd(',').Split(',');
                    nlist = nlist.Where(x => vesselnames1.Contains(x.VesselName) && (x.NcDate >= datefrom && x.NcDate <= dateto));
                }
                if (!string.IsNullOrEmpty(rank))
                {
                    string[] ranks1 = rank.TrimEnd(',').Split(',');
                    nlist = nlist.Where(x => ranks1.Contains(x.Rank) && (x.NcDate >= datefrom && x.NcDate <= dateto));
                }

            }

            if (UserRole.username.ToLower() != "admin")
            {
                var vsname = sc.Users.Where(x => x.EmailId.Equals(UserRole.username1)).Select(x => x.VesselName).FirstOrDefault();
                string[] vesselname1 = vsname.TrimEnd(',').Split(',');
                nlist = nlist.Where(x => vesselname1.Contains(x.VesselName)).ToList();
            }


            //....................
            ViewBag.Ack = nlist.Where(x => x.AcknDate != null).Count();
            ViewBag.UnAck = nlist.Where(x => x.NonConfirmity != null).Count();
            //...................


            totalrecords = nlist.Count();

            string bmsasc = (sort + sortdir).ToLower();
            if (bmsasc == "ncdatedesc")
            {
                nlist = nlist.OrderByDescending(u => u.NcDate).ThenByDescending(u => u.AcknDate);
            }
            else if (bmsasc == "ncdateasc")
            {
                nlist = nlist.OrderBy(u => u.NcDate).ThenBy(u => u.AcknDate);
            }
            else
            {
                nlist = nlist.OrderBy(sort + " " + sortdir);
            }


          


           
          
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
               

            ViewBag.Messagegrid = nlist.Count() > 0 ? "" : ViewBag.Messagegrid = "Data Not Available For Selected Criteria! Please Select The Other Criteria.";



          

            ViewBag.datefrom = DateTime.Parse(datefrom.ToString()).ToString("MM/dd/yyyy", CultureInfo.InvariantCulture);
            ViewBag.dateto = DateTime.Parse(dateto.ToString()).ToString("MM/dd/yyyy", CultureInfo.InvariantCulture);

            //..................
            ViewBag.fleettypebm = sc.AutoCompletedFleetType;
            ViewBag.fleetnamebm = sc.AutoCompletedfleetname;
            ViewBag.vesselnamebm = sc.AutoCompletevessel;
            ViewBag.rankebm = sc.AutoCompleterank;
            //......................



            return View(nlist.ToList());

        }

        private IQueryable<NotificationClass> GetTreeData(string vesselname,DateTime? datefrom,DateTime? dateto)
        {
            if (!string.IsNullOrEmpty(vesselname))
            {
                string[] vesselnames1 = vesselname.TrimEnd(',').Split(',');

                var locations = sc.Notifications.Where
              (x => vesselnames1.Contains(x.VesselName) && x.NonConfirmity != null && x.AcknDate == null);


                var meters = sc.Notifications.Where
                (x => vesselnames1.Contains(x.VesselName) && x.AcknDate != null && (x.NcDate >= datefrom && x.NcDate <= dateto));

                return locations.Concat(meters);
            }
            else
            {
                var locations = sc.Notifications.Where(x => x.NonConfirmity != null && x.AcknDate == null);


                var meters = sc.Notifications.Where
                (x =>  x.AcknDate != null && (x.NcDate >= datefrom && x.NcDate <= dateto));

                return locations.Concat(meters);
            }
          
           
        }



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
       new { controller = "deviations", area = "notification", datefrom, dateto, search, fleetname, fleettype, page = (int)TempData["page"] }));

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

            ViewBag.Messagegrid = nlist.Count() > 0 ? "" : ViewBag.Messagegrid = "Data Not Available For Selected Criteria! Please Select The Other Criteria.";

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
        new { controller = "deviations", area = "notification", Id = cm.Nid }));

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