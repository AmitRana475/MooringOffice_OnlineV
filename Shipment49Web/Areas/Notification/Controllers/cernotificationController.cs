using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Web.Mvc;
using CertificationLayer;
using System.Web.Security;
using System.Web.Routing;
using System.Globalization;
using MenuLayer;
using System.Threading.Tasks;
using System.Data.Entity;
using Shipment49Web.Common;
using Shipment49Web.Models;

namespace Shipment49Web.Areas.Notification.Controllers
{
    [Authorize]
    [ErrorClass]
    public class cernotificationController : Controller
    {
        //private readonly ShipmentContaxt sc = new ShipmentContaxt();

        private readonly IMenuRepository sc;
        public cernotificationController(IMenuRepository repo)
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
        public async Task<ActionResult> Index(DateTime? datefrom, DateTime? dateto, string ids = "", string Acknowledge = "", int page = 1, string sort = "DOI", string sortdir = "desc", string fleettype = "",string fleetname="", string search="", string Searchbtn="")
        {

            datefrom = Convert.ToDateTime(datefrom == null ? DateTime.Parse(DateTime.Now.AddMonths(-1).ToString()).ToString("MM/dd/yyyy", CultureInfo.InvariantCulture) : DateTime.Parse(datefrom.ToString()).ToString("MM/dd/yyyy", CultureInfo.InvariantCulture));
            dateto = Convert.ToDateTime(dateto == null ? DateTime.Parse(DateTime.Now.ToString()).ToString("MM/dd/yyyy", CultureInfo.InvariantCulture) : DateTime.Parse(dateto.ToString()).ToString("MM/dd/yyyy", CultureInfo.InvariantCulture));


            fleettype = fleettype.TrimEnd(',');
            fleetname = fleetname.TrimEnd(',');
            search = search.TrimEnd(',');
           

            ViewBag.fleettypebm1 = fleettype;
            ViewBag.fleetnamebm1 = fleetname;
            ViewBag.vesselnamebm1 = search;
           

            if (!string.IsNullOrEmpty(Acknowledge)) // Function for Acknowledge...
            {
                if (!string.IsNullOrEmpty(ids))
                    AcknowledgeAll(ids);
            }



            int pagesize = 10;
            int totalrecords = 0;
            if (page < 1) page = 1;
            int skip = (page * pagesize) - pagesize;
            //var data = GetCertification(datefrom, dateto, search, fleetname,fleettype, sort, sortdir, skip, pagesize, out totalrecords);

            string[] searchs = search.TrimEnd(',').Split(',');
            var clist = sc.Certifications.Where(x => searchs.Contains(x.VesselName));

            if (search != "" && fleetname != "" && fleettype != "" && datefrom == null && dateto == null)
            {
                clist = null;
                string[] fleetnames = fleetname.TrimEnd(',').Split(',');
                string[] fleettypes = fleettype.TrimEnd(',').Split(',');
                clist = sc.Certifications.Where
                (x => fleettypes.Contains(x.FleetType) && fleetnames.Contains(x.FleetName) && searchs.Contains(x.VesselName));
            }
            else if (search == "" && fleetname != "" && fleettype != "" && datefrom != null && dateto != null)
            {
                clist = null;
                string[] fleetnames = fleetname.TrimEnd(',').Split(',');
                string[] fleettypes = fleettype.TrimEnd(',').Split(',');
                clist = sc.Certifications.Where
                (x => fleettypes.Contains(x.FleetType) && fleetnames.Contains(x.FleetName) && (x.DOI >= datefrom && x.DOI <= dateto));

                



            }
            else if (search == "" && fleetname != "" && fleettype == "" && datefrom != null && dateto != null)
            {
                clist = null;
                string[] fleetnames = fleetname.TrimEnd(',').Split(',');
                clist = sc.Certifications.Where
                (x => fleetnames.Contains(x.FleetName) && (x.DOI >= datefrom && x.DOI <= dateto));

                

            }
            else if (search == "" && fleetname == "" && fleettype != "" && datefrom != null && dateto != null)
            {
                clist = null;
                string[] fleettypes = fleettype.TrimEnd(',').Split(',');
                clist = sc.Certifications.Where
                (x => fleettypes.Contains(x.FleetType) && (x.DOI >= datefrom && x.DOI <= dateto));

                

            }

            else if (search != "" && fleetname != "" && fleettype != "" && datefrom != null && dateto != null)
            {
                clist = null;
                string[] fleetnames = fleetname.TrimEnd(',').Split(',');
                string[] fleettypes = fleettype.TrimEnd(',').Split(',');
                clist = sc.Certifications.Where
                (x => fleettypes.Contains(x.FleetType) && fleetnames.Contains(x.FleetName) && searchs.Contains(x.VesselName) && (x.DOI >= datefrom && x.DOI <= dateto));

                

            }
            else if (search != "" && fleetname != "" && fleettype == "" && datefrom != null && dateto != null)
            {
                clist = null;
                string[] fleetnames = fleetname.TrimEnd(',').Split(',');
                clist = sc.Certifications.Where
                (x => fleetnames.Contains(x.FleetName) && searchs.Contains(x.VesselName) && (x.DOI >= datefrom && x.DOI <= dateto));

               


            }
            else if (search != "" && fleetname == "" && fleettype != "" && datefrom != null && dateto != null)
            {
                clist = null;
                string[] fleettypes = fleettype.TrimEnd(',').Split(',');
                clist = sc.Certifications.Where
                (x => fleettypes.Contains(x.FleetType) && searchs.Contains(x.VesselName) && (x.DOI >= datefrom && x.DOI <= dateto));


                

            }
            else if (search != "" && fleetname == "" && fleettype == "" && datefrom != null && dateto != null)
            {
                clist = null;
                clist = sc.Certifications.Where
                (x => searchs.Contains(x.VesselName) && (x.DOI >= datefrom && x.DOI <= dateto));

                //if (string.IsNullOrEmpty(Searchbtn))
                //{
                //    if (clist.Count() == 0)
                //    {
                //        clist = null;
                //        datefrom = Convert.ToDateTime(DateTime.Parse(DateTime.Now.AddMonths(-3).ToString()).ToString("MM/dd/yyyy", CultureInfo.InvariantCulture));
                //        clist = sc.Certifications.Where
                //        (x => searchs.Contains(x.VesselName) && (x.DOI >= datefrom && x.DOI <= dateto));
                //    }
                //    if (clist.Count() == 0)
                //    {
                //        clist = null;
                //        datefrom = Convert.ToDateTime(DateTime.Parse(DateTime.Now.AddMonths(-6).ToString()).ToString("MM/dd/yyyy", CultureInfo.InvariantCulture));
                //        clist = sc.Certifications.Where
                //        (x => searchs.Contains(x.VesselName) && (x.DOI >= datefrom && x.DOI <= dateto));
                //    }
                //    if (clist.Count() == 0)
                //    {
                //        clist = null;
                //        datefrom = Convert.ToDateTime(DateTime.Parse(DateTime.Now.AddMonths(-12).ToString()).ToString("MM/dd/yyyy", CultureInfo.InvariantCulture));
                //        clist = sc.Certifications.Where
                //       (x => searchs.Contains(x.VesselName) && (x.DOI >= datefrom && x.DOI <= dateto));
                //    }
                //}


            }
            else if (search == "" && fleetname == "" && fleettype == "" && datefrom != null && dateto != null)
            {
                clist = null;
                clist = sc.Certifications.Where
                (x => x.DOI >= datefrom && x.DOI <= dateto);

                if (string.IsNullOrEmpty(Searchbtn))
                {
                    if (clist.Count() == 0)
                    {
                        clist = null;
                        datefrom = Convert.ToDateTime(DateTime.Parse(DateTime.Now.AddMonths(-3).ToString()).ToString("MM/dd/yyyy", CultureInfo.InvariantCulture));
                        clist = sc.Certifications.Where
                         (x => x.DOI >= datefrom && x.DOI <= dateto);
                    }
                    if (clist.Count() == 0)
                    {
                        clist = null;
                        datefrom = Convert.ToDateTime(DateTime.Parse(DateTime.Now.AddMonths(-6).ToString()).ToString("MM/dd/yyyy", CultureInfo.InvariantCulture));
                        clist = sc.Certifications.Where
                        (x => x.DOI >= datefrom && x.DOI <= dateto);
                    }
                    if (clist.Count() == 0)
                    {
                        clist = null;
                        datefrom = Convert.ToDateTime(DateTime.Parse(DateTime.Now.AddMonths(-12).ToString()).ToString("MM/dd/yyyy", CultureInfo.InvariantCulture));
                        clist = sc.Certifications.Where
                         (x => x.DOI >= datefrom && x.DOI <= dateto);
                    }
                }

            }

            


            ViewBag.Ack = clist.Where(x => x.AcknDate != null).Count();
            ViewBag.UnAck = clist.Where(x => x.AlertFrequency != null).Count();

            totalrecords = clist.Count();

            string bmsasc = (sort + sortdir).ToLower();
            if (bmsasc == "doidesc")
            {
                clist = clist.OrderBy(u => u.AcknDate);
            }
            else if (bmsasc == "doiasc")
            {
                clist = clist.OrderBy(u => u.DOI).ThenBy(u => u.AcknDate);
            }
            else
            {
                clist = clist.OrderBy(sort + " " + sortdir);
            }


          

            if (pagesize > 0)
            {
                clist = clist.Skip(skip).Take(pagesize);

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

            
            ViewBag.Messagegrid = clist.Count() > 0 ? "" : ViewBag.Messagegrid = "Data not available for selected criteria! please select the other criteria.";

            ViewBag.datefrom = DateTime.Parse(datefrom.ToString()).ToString("MM/dd/yyyy", CultureInfo.InvariantCulture);
            ViewBag.dateto = DateTime.Parse(dateto.ToString()).ToString("MM/dd/yyyy", CultureInfo.InvariantCulture);


            //..................
            ViewBag.fleettypebm = sc.AutoCompletedFleetType;
            ViewBag.fleetnamebm = sc.AutoCompletedfleetname;
            ViewBag.vesselnamebm = sc.AutoCompletevessel;
         
            //......................


            return View(await clist.ToListAsync());
        }




        private void AcknowledgeAll(string id)
        {
            using (ShipmentContaxt sc2 = new ShipmentContaxt())
            {
                sc2.Configuration.ProxyCreationEnabled = false;

                string rolename = string.Join("", Roles.GetRolesForUser());
                string bms = "Acknowledged on " + DateTime.Now.ToString("dd-MMM-yyyy") + " By " + rolename;

                string[] idack = id.TrimEnd(',').Split(',');

                foreach (var item in idack)
                {
                    int idi = int.Parse(item);
                    var bmcheck = sc2.Certifications.Where(p => p.Nid.Equals(idi)).Select(x => x.Acknowledge).FirstOrDefault();
                    if (bmcheck == null)
                    {
                        var user = new CertificationClass() { Nid = int.Parse(item), Acknowledge = bms, AcknDate = DateTime.Now };

                        sc2.Certifications.Attach(user);
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
                string rolename = string.Join("", Roles.GetRolesForUser());
                string bms = "Acknowledged on " + DateTime.Now.ToString("dd-MMM-yyyy") + " By " + rolename;

                var user = new CertificationClass() { Nid = id, Acknowledge = bms, AcknDate = DateTime.Now };
                sc1.Certifications.Attach(user);
                sc1.Entry(user).Property(x => x.Acknowledge).IsModified = true;
                sc1.Entry(user).Property(x => x.AcknDate).IsModified = true;
                await sc1.SaveChangesAsync();

                return RedirectToAction("index", new RouteValueDictionary(
       new { controller = "cernotification", area = "notification",datefrom, dateto, search, fleetname, fleettype, page = (int)TempData["page"] }));

            }
        }

        public async Task<ActionResult> comment( int? id, int page = 1, string sort = "CDate", string sortdir = "desc", string search = "")
        {
            int pagesize = 10;
            int totalrecords = 0;

            if (page < 1) page = 1;
            int skip = (page * pagesize) - pagesize;
            //var data = Getcecomment(id, search, sort, sortdir, skip, pagesize, out totalrecords);
            var clist = sc.Comments.Where(x => x.Nid == id);
            if (search != null)
            {
                clist = sc.Comments.Where(x => x.CommentBy.Contains(search) && x.Nid == id);
            }
            totalrecords = clist.Count();
            clist = clist.OrderBy(sort + " " + sortdir);
            if (pagesize > 0)
            {
                clist = clist.Skip(skip).Take(pagesize);
            }

            ViewBag.TotalRows = totalrecords;
            ViewBag.Nid = id;

            ViewBag.Messagegrid = clist.Count() > 0 ? "" : ViewBag.Messagegrid = "Data not available for selected criteria! please select the other criteria.";

            ViewBag.page = BmsPageNumber.Pagenumber;
            ViewBag.fleettype = BmsPageNumber.FleetType;
            ViewBag.fleetname = BmsPageNumber.FleetName;
            ViewBag.vname = BmsPageNumber.VesselName;
            ViewBag.datefrom = BmsPageNumber.datefrom;
            ViewBag.dateto = BmsPageNumber.dateto;

            return View(await clist.ToListAsync());
        }

        /*
        [NonAction]
        private List<CertificationClass> GetCertification(DateTime? datefrom, DateTime? dateto, string search, string fleetname,string fleettype, string sort, string sortdir, int skip, int pagesize, out int totalrecord)
        {
            //var clist = sc.Certifications.Where(x => x.VesselName.Contains(search) || x.DOI >= datefrom && x.DOI <= dateto);

            //if (search != null && datefrom == null && dateto == null)
            //{
            //    clist = sc.Certifications.Where(x => x.VesselName.Contains(search));
            //}
            //else if (search == null && datefrom != null && dateto != null)
            //{
            //    clist = sc.Certifications.Where
            //    (x => x.DOI >= datefrom && x.DOI <= dateto);
            //}
            //else if (search != null && datefrom != null && dateto != null)
            //{
            //    clist = sc.Certifications.Where
            //    (x => x.VesselName.Contains(search)
            //     && (x.DOI >= datefrom && x.DOI <= dateto));
            //}

            string[] searchs = search.TrimEnd(',').Split(',');
            var clist = sc.Certifications.Where(x => searchs.Contains(x.VesselName));

            if (search != "" && fleetname != "" && fleettype != "" && datefrom == null && dateto == null)
            {
                string[] fleetnames = fleetname.TrimEnd(',').Split(',');
                string[] fleettypes = fleettype.TrimEnd(',').Split(',');
                clist = sc.Certifications.Where
                (x => fleettypes.Contains(x.FleetType) && fleetnames.Contains(x.FleetName) && searchs.Contains(x.VesselName));
            }
            else if (search == "" && fleetname != "" && fleettype != "" && datefrom != null && dateto != null)
            {
                string[] fleetnames = fleetname.TrimEnd(',').Split(',');
                string[] fleettypes = fleettype.TrimEnd(',').Split(',');
                clist = sc.Certifications.Where
                (x => fleettypes.Contains(x.FleetType) && fleetnames.Contains(x.FleetName) && (x.DOI >= datefrom && x.DOI <= dateto));

            }
            else if (search == "" && fleetname != "" && fleettype == "" && datefrom != null && dateto != null)
            {
                string[] fleetnames = fleetname.TrimEnd(',').Split(',');

                clist = sc.Certifications.Where
                (x => fleetnames.Contains(x.FleetName) && (x.DOI >= datefrom && x.DOI <= dateto));
            }
            else if (search == "" && fleetname == "" && fleettype != "" && datefrom != null && dateto != null)
            {

                string[] fleettypes = fleettype.TrimEnd(',').Split(',');
                clist = sc.Certifications.Where
                (x => fleettypes.Contains(x.FleetType) && (x.DOI >= datefrom && x.DOI <= dateto));
            }

            else if (search != "" && fleetname != "" && fleettype != "" && datefrom != null && dateto != null)
            {
                string[] fleetnames = fleetname.TrimEnd(',').Split(',');
                string[] fleettypes = fleettype.TrimEnd(',').Split(',');
                clist = sc.Certifications.Where
                (x => fleettypes.Contains(x.FleetType) && fleetnames.Contains(x.FleetName) && searchs.Contains(x.VesselName) && (x.DOI >= datefrom && x.DOI <= dateto));
            }
            else if (search != "" && fleetname != "" && fleettype == "" && datefrom != null && dateto != null)
            {
                string[] fleetnames = fleetname.TrimEnd(',').Split(',');

                clist = sc.Certifications.Where
                (x => fleetnames.Contains(x.FleetName) && searchs.Contains(x.VesselName) && (x.DOI >= datefrom && x.DOI <= dateto));
            }
            else if (search != "" && fleetname == "" && fleettype != "" && datefrom != null && dateto != null)
            {

                string[] fleettypes = fleettype.TrimEnd(',').Split(',');
                clist = sc.Certifications.Where
                (x => fleettypes.Contains(x.FleetType) && searchs.Contains(x.VesselName) && (x.DOI >= datefrom && x.DOI <= dateto));
            }
            else if (search != "" && fleetname == "" && fleettype == "" && datefrom != null && dateto != null)
            {

                clist = sc.Certifications.Where
                (x => searchs.Contains(x.VesselName) && (x.DOI >= datefrom && x.DOI <= dateto));
            }
            else if (search == "" && fleetname == "" && fleettype == "" && datefrom != null && dateto != null)
            {
                clist = sc.Certifications.Where
                (x => x.DOI >= datefrom && x.DOI <= dateto);
            }


            totalrecord = clist.Count();
            clist = clist.OrderBy(sort + " " + sortdir);
            if(pagesize>0)
            {
                clist = clist.Skip(skip).Take(pagesize);

            }

            return clist.ToList();
        }
        */


        /*
        [NonAction]
        private List<CommentCirtificate> Getcecomment(int? id, string search, string sort, string sortdir, int skip, int pagesize, out int totalrecord)
        {
            var clist = sc.Comments.Where(x => x.Nid == id);
            if (search != null)
            {
                clist = sc.Comments.Where(x => x.CommentBy.Contains(search) && x.Nid == id);
            }
            totalrecord = clist.Count();
            clist = clist.OrderBy(sort + " " + sortdir);
            if (pagesize > 0)
            {
                clist = clist.Skip(skip).Take(pagesize);
            }
            return  clist.ToList();
        }
        */
        [HttpGet]
        public ActionResult Create(CommentCirtificate cm, int id)
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
        public async Task<ActionResult> Create(CommentCirtificate cm)
        {
            using (ShipmentContaxt sc1 = new ShipmentContaxt())
            {
               
                cm.CDate = DateTime.Now;

                if (ModelState.IsValid)
                {
                    sc1.Comments.Add(cm);
                   await sc1.SaveChangesAsync();

                    return RedirectToAction("comment", new RouteValueDictionary(
        new { controller = "cernotification", area = "notification", Id = cm.Nid }));

                   
                }
                else
                {
                    return View(cm);
                }

            }
        }


      


        /*
        public async Task<ActionResult> AutoCompletece(string term)
        {
                List<string> students = await sc.Vessels.Where(s => s.VesselName.ToLower().Contains(term.ToLower()))
                    .Select(x => x.VesselName).Distinct().ToListAsync();

                return Json(students, JsonRequestBehavior.AllowGet);
           
        }
        */
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