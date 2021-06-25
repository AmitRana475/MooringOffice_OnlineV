
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Dynamic;
using System.Net;
using System.Web.Mvc;
using VesselLayer;
using MenuLayer;
using System.Web.Security;
using Shipment49Web.Models;
using CertificationLayer;
using CrewReportLayer;
using NotificationLayer;
using System.Threading.Tasks;
using System;

namespace Shipment49Web.Areas.Setting.Controllers
{

    [Authorize]
    [ErrorClass]
    public class VesselController : Controller
    {
        //private ShipmentContaxt sc = new ShipmentContaxt();

        private readonly IMenuRepository sc;
       
        public VesselController(IMenuRepository repo)
        {
            sc = repo;

            if (UserRole.username == null)
            {
                UserRole.username = string.Join("", Roles.GetRolesForUser());
            }
            ViewBag.GetMenu = UserRole.username == "Admin" ? sc.Menus.ToList() : sc.Menus.Where(x => x.Role == "User").ToList();
        }

        //[Authorize(Roles = "Admin,User")]
        [HttpGet]
        public ActionResult Index(int page = 1, string sort = "VesselName", string sortdir = "asc", string search = "")
        {
            
            int pagesize = 10;
            int totalrecord = 0;
            if (page < 1) page = 1;
            int skip = (page * pagesize) - pagesize;
            var data = GetVessels(search, sort, sortdir, skip, pagesize, out totalrecord);
            ViewBag.TotalRows = totalrecord;
            ViewBag.search = search;
            ViewBag.Messagegrid = data.Count() > 0 ? "" : ViewBag.Messagegrid = "Data not available for selected criteria! please select the other criteria.";


            return View(data);
        }

        [NonAction]
        private List<Vessel> GetVessels(string search, string sort, string sortdir, int skip, int pagesize, out int totalrecord)
        {

            var v = (from a in sc.Vessels
                     where
                     a.VesselName.Contains(search) ||
                     a.Flag.Contains(search) ||
                     a.FleetName.Contains(search)||
                     a.FleetType.Contains(search)
                     select a
                    );

            totalrecord = v.Count();
            v = v.OrderBy(sort + " " + sortdir);
            if (pagesize > 0)
            {
                v = v.Skip(skip).Take(pagesize);
            }

            return v.ToList();
            
        }


        //[Authorize(Roles = "Admin,User")]
        //public ActionResult Index(int? page)
        //{
        //    const int pagesize = 7;
        //    //int pagenumber = (page ?? 1);

        //    var emps = sc.Vessels.ToList().ToPagedList(page ?? 1, pagesize);

        //    ViewBag.Messagegrid = emps.Count() > 0 ? "" : ViewBag.Messagegrid = "Data not available !";


        //    //List<Vessel> VesselList = sc.Vessels.ToList();


        //    return View(emps);
        //}

        [NonAction]
        private List<FleetNameClass> GetFleetNames(string search, string sort, string sortdir, int skip, int pagesize, out int totalrecord)
        {

            var v = (from a in sc.FleetNames
                     where
                     a.FleetName.Contains(search) ||
                     a.AddedBy.Contains(search)
                     select a
                    );

            totalrecord = v.Count();
            v = v.OrderBy(sort + " " + sortdir);
            if (pagesize > 0)
            {
                v = v.Skip(skip).Take(pagesize);
            }

            return v.ToList();

        }

        [NonAction]
        private List<FleetTypeClass> GetFleetTypes(string search, string sort, string sortdir, int skip, int pagesize, out int totalrecord)
        {

            var v = (from a in sc.FleetTypes
                     where
                     a.FleetType.Contains(search) ||
                     a.AddedBy.Contains(search)
                     select a
                    );

            totalrecord = v.Count();
            v = v.OrderBy(sort + " " + sortdir);
            if (pagesize > 0)
            {
                v = v.Skip(skip).Take(pagesize);
            }

            return v.ToList();

        }


        [HttpGet]
        [Authorize(Roles = "Admin,User")]
        public ActionResult Create()
        {

            return View();
        }

        [Authorize(Roles = "Admin,User")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "VesselID,VesselName,ImoNo,Flag,FleetName,FleetType")]Vessel vs )
        {
            using (ShipmentContaxt sc1 = new ShipmentContaxt())
            {
                vs.VesselID = vs.ImoNo;
                vs.VesselName = vs.VesselName.Trim();
                //if (sc.Vessels.Any(x => x.ImoNo == vs.ImoNo))
                //{
                //    ModelState.AddModelError("ImoNo", "IMO No already in use");
                //}
                if (ModelState.IsValid)
                {
                    sc1.Vessels.Add(vs);
                    sc1.SaveChanges();
                    return RedirectToAction("index");
                }
                else
                {
                    return View(vs);
                }
            }
        }

        [HttpGet]
        [Authorize(Roles = "Admin,User")]
        public ActionResult Edit(int id)
        {
            Vessel vs = sc.Vessels.Where(x => x.Id == id).FirstOrDefault();

            return View(vs);
        }
        [HttpPost]
        [Authorize(Roles = "Admin,User")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(Vessel vs)
        {

            using (ShipmentContaxt sc1 = new ShipmentContaxt())
            {
                sc1.Configuration.ProxyCreationEnabled = false;

                vs.VesselID = vs.ImoNo;
                if (ModelState.IsValid)
                {

                    // [Bind(Include = "VesselName,Flag,FleetName,FleetType")]

                    var userbb = new Vessel() { Id = vs.Id, VesselName = vs.VesselName.Trim(), Flag = vs.Flag.Trim(), FleetName = vs.FleetName, FleetType = vs.FleetType };
                    sc1.Vessels.Attach(userbb);
                    sc1.Entry(userbb).Property(x => x.VesselName).IsModified = true;
                    sc1.Entry(userbb).Property(x => x.Flag).IsModified = true;
                    sc1.Entry(userbb).Property(x => x.FleetName).IsModified = true;
                    sc1.Entry(userbb).Property(x => x.FleetType).IsModified = true;
                    await sc1.SaveChangesAsync();

                    //.................

                    var user = sc1.Certifications.Where(p => p.VesselId == vs.ImoNo).Select(x => x.Nid).ToArray();
                    foreach (var item in user)  // ShopItems is a posted list with values 
                    {

                        var userb = new CertificationClass() { Nid = item, VesselName = vs.VesselName, FleetName = vs.FleetName, FleetType = vs.FleetType };
                        sc1.Certifications.Attach(userb);
                        sc1.Entry(userb).Property(x => x.VesselName).IsModified = true;
                        sc1.Entry(userb).Property(x => x.FleetName).IsModified = true;
                        sc1.Entry(userb).Property(x => x.FleetType).IsModified = true;

                        await sc1.SaveChangesAsync();
                    }


                    var user1 = sc1.CertificateLists.Where(p => p.VesselId == vs.ImoNo).Select(x => x.Id).ToArray();
                    foreach (var item in user1)  // ShopItems is a posted list with values 
                    {

                        var userb = new CertificateList() { Id = item, VesselName = vs.VesselName, FleetName = vs.FleetName, FleetType = vs.FleetType };
                        sc1.CertificateLists.Attach(userb);
                        sc1.Entry(userb).Property(x => x.VesselName).IsModified = true;
                        sc1.Entry(userb).Property(x => x.FleetName).IsModified = true;
                        sc1.Entry(userb).Property(x => x.FleetType).IsModified = true;

                        await sc1.SaveChangesAsync();
                    }

                    var user2 = sc1.CreReports.Where(p => p.Vessel_ID == vs.ImoNo).Select(x => x.Wid).ToArray();
                    foreach (var item in user2)  // ShopItems is a posted list with values 
                    {

                        var userb = new CreReportClass() { Wid = item, VesselName = vs.VesselName, FleetName = vs.FleetName, FleetType = vs.FleetType };
                        sc1.CreReports.Attach(userb);
                        sc1.Entry(userb).Property(x => x.VesselName).IsModified = true;
                        sc1.Entry(userb).Property(x => x.FleetName).IsModified = true;
                        sc1.Entry(userb).Property(x => x.FleetType).IsModified = true;

                        await sc1.SaveChangesAsync();
                    }

                    var user3 = sc1.CrewDetails.Where(p => p.Vessel_ID == vs.ImoNo).Select(x => x.Id).ToArray();
                    foreach (var item in user3)  // ShopItems is a posted list with values 
                    {

                        var userb = new CrewDetailClass() { Id = item, VesselName = vs.VesselName, FleetName = vs.FleetName, FleetType = vs.FleetType };
                        sc1.CrewDetails.Attach(userb);
                        sc1.Entry(userb).Property(x => x.VesselName).IsModified = true;
                        sc1.Entry(userb).Property(x => x.FleetName).IsModified = true;
                        sc1.Entry(userb).Property(x => x.FleetType).IsModified = true;

                        await sc1.SaveChangesAsync();
                    }

                    var user4 = sc1.Notifications.Where(p => p.VesselId == vs.ImoNo).Select(x => x.Nid).ToArray();
                    foreach (var item in user4)  // ShopItems is a posted list with values 
                    {

                        var userb = new NotificationClass() { Nid = item, VesselName = vs.VesselName, FleetName = vs.FleetName, FleetType = vs.FleetType };
                        sc1.Notifications.Attach(userb);
                        sc1.Entry(userb).Property(x => x.VesselName).IsModified = true;
                        sc1.Entry(userb).Property(x => x.FleetName).IsModified = true;
                        sc1.Entry(userb).Property(x => x.FleetType).IsModified = true;

                        await sc1.SaveChangesAsync();
                    }



                    //var user5 = sc1.Users.Select(x => new { x.UserId, x.VesselID }).ToArray();

                    //foreach (var item in user5)  // ShopItems is a posted list with values 
                    //{
                    //    if (item.VesselID != null)
                    //    {
                    //        string[] bms = item.VesselID.Split(',').ToArray();
                    //        string vesselname1 = "";
                    //        foreach (string bb in bms)
                    //        {
                    //            var ab = (bb.TrimStart('"'));

                    //            vesselname1 += sc1.Vessels.Where(x => x.VesselID.ToString() == ab).Select(p => p.VesselName).FirstOrDefault() + ",";
                    //        }

                    //        var userb = new UserClass() { UserId=item.UserId, VesselName = vesselname1 };
                    //        sc1.Users.Attach(userb);
                    //        sc1.Entry(userb).Property(x => x.VesselName).IsModified = true;
                    //        await sc1.SaveChangesAsync();


                    //    }
                    //}



                    return RedirectToAction("index");
                }
                else
                {
                    return View(vs);
                }
            }
        }
       
        [Authorize(Roles = "Admin,User")]
        [HttpGet]
        public ActionResult Detail(int? id)
        {
            using (ShipmentContaxt sc1 = new ShipmentContaxt())
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Vessel vs = sc1.Vessels.Find(id);
                if (vs == null)
                {
                    return HttpNotFound();
                }
                return View(vs);
            }
        }


        public JsonResult checkimo(int? initialProduct, int? imono)
        {
            if (initialProduct == imono)
            {

                return Json(true, JsonRequestBehavior.AllowGet);
            }
           
         return Json(sc.Vessels.All(c => c.ImoNo != imono), JsonRequestBehavior.AllowGet);
           
        }


        public JsonResult checkfnameVesselCreate( string fleetname)
        {
            //if (initialProductVessel.Trim() == fleetname.Trim())
            //{

            //    return Json(true, JsonRequestBehavior.AllowGet);
            //}

            var bbb = sc.FleetNames.Where(c => c.FleetName.Equals(fleetname)).FirstOrDefault() == null ? false : true;

            return Json(bbb,  JsonRequestBehavior.AllowGet);

        }

        public JsonResult checkftypeVesselCreate(string fleettype)
        {
           

            var bbb = sc.FleetTypes.Where(c => c.FleetType.Equals(fleettype)).FirstOrDefault() == null ? false : true;

            return Json(bbb, JsonRequestBehavior.AllowGet);

        }

        public JsonResult checkfname(string initialProductF, string fleetname)
        {
            if (initialProductF.Trim() == fleetname.Trim())
            {

                return Json(true, JsonRequestBehavior.AllowGet);
            }

            return Json(sc.FleetNames.All(c => c.FleetName.Trim() != fleetname.Trim()), JsonRequestBehavior.AllowGet);

        }
        public JsonResult checkftype(string initialProductT, string fleettype)
        {
            if (initialProductT.Trim() == fleettype.Trim())
            {

                return Json(true, JsonRequestBehavior.AllowGet);
            }

            return Json(sc.FleetTypes.All(c => c.FleetType.Trim() != fleettype.Trim()), JsonRequestBehavior.AllowGet);

        }

        [Authorize(Roles = "Admin,User")]
        public async Task<ActionResult> Delete(int? id)
        {
            using (ShipmentContaxt sc1 = new ShipmentContaxt())
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                else
                {
                    Vessel vs = await sc1.Vessels.Where(x => x.Id == id).FirstOrDefaultAsync();
                   
                    //................................
                    var user = sc1.CreReports.Where(p => p.Vessel_ID == vs.ImoNo).Select(x => x.Wid).ToArray();
                    foreach (var item in user)  // ShopItems is a posted list with values 
                    {
                        CreReportClass userb = await sc1.CreReports.Where(x => x.Wid == item).FirstOrDefaultAsync();
                        sc1.CreReports.Remove(userb);
                        await sc1.SaveChangesAsync();
                    }


                    var user1 = sc1.Certifications.Where(p => p.VesselId == vs.ImoNo).Select(x => x.Nid).ToArray();
                    foreach (var item in user1)  // ShopItems is a posted list with values 
                    {
                        CertificationClass userb = await sc1.Certifications.Where(x => x.Nid == item).FirstOrDefaultAsync();
                        sc1.Certifications.Remove(userb);
                        await sc1.SaveChangesAsync();

                        var userbm1 = sc1.Comments.Where(p => p.Nid == item).Select(x => x.Cid).ToArray();

                        foreach (var item1 in userbm1)
                        {
                            CommentCirtificate userb11 = await sc1.Comments.Where(x => x.Cid == item1).FirstOrDefaultAsync();
                            sc1.Comments.Remove(userb11);
                            await sc1.SaveChangesAsync();
                        }

                        var userbm2 = sc1.Comments1.Where(p => p.Nid == item).Select(x => x.Cid).ToArray();
                        foreach (var item2 in userbm2)
                        {
                            CommentClass userb22 = await sc1.Comments1.Where(x => x.Cid == item2).FirstOrDefaultAsync();
                            sc1.Comments1.Remove(userb22);
                            await sc1.SaveChangesAsync();
                        }
                    }


                    var user2 = sc1.CertificateLists.Where(p => p.VesselId == vs.ImoNo).Select(x => x.Id).ToArray();
                    foreach (var item in user2)  // ShopItems is a posted list with values 
                    {

                        CertificateList userb = await sc1.CertificateLists.Where(x => x.Id == item).FirstOrDefaultAsync();
                        sc1.CertificateLists.Remove(userb);
                        await sc1.SaveChangesAsync();
                    }

                  

                    var user3 = sc1.CrewDetails.Where(p => p.Vessel_ID == vs.ImoNo).Select(x => x.Id).ToArray();
                    foreach (var item in user3)  // ShopItems is a posted list with values 
                    {
                        CrewDetailClass userb = await sc1.CrewDetails.Where(x => x.Id == item).FirstOrDefaultAsync();
                        sc1.CrewDetails.Remove(userb);
                        await sc1.SaveChangesAsync();
                    }

                    var user4 = sc1.Notifications.Where(p => p.VesselId == vs.ImoNo).Select(x => x.Nid).ToArray();
                    foreach (var item in user4)  // ShopItems is a posted list with values 
                    {

                        NotificationClass userb =  await sc1.Notifications.Where(x => x.Nid == item).FirstOrDefaultAsync();
                        sc1.Notifications.Remove(userb);
                        await sc1.SaveChangesAsync();
                    }

                    


                    //........................
                    sc1.Vessels.Remove(vs);
                    await sc1.SaveChangesAsync();
                }

                return RedirectToAction("index");
            }
        }


        [HttpGet]
        [Authorize(Roles = "Admin,User")]
        public ActionResult FleetName(string fleetname, int page = 1,  string sort = "FleetName", string sortdir = "asc", string search = "")
        {
            ViewBag.Messagebms = fleetname;

            int pagesize = 10;
            int totalrecord = 0;
            if (page < 1) page = 1;
            int skip = (page * pagesize) - pagesize;
            var data = GetFleetNames(search, sort, sortdir, skip, pagesize, out totalrecord);
            ViewBag.TotalRows = totalrecord;
            ViewBag.search = search;
            ViewBag.Messagegrid = data.Count() > 0 ? "" : ViewBag.Messagegrid = "Data not available for selected criteria! please select the other criteria.";

            return View(data);
        }


        [HttpGet]
        [Authorize(Roles = "Admin,User")]
        public ActionResult FleetType(string fleettype, int page = 1, string sort = "FleetType", string sortdir = "asc", string search = "")
        {
            ViewBag.Messagebms = fleettype;

            int pagesize = 10;
            int totalrecord = 0;
            if (page < 1) page = 1;
            int skip = (page * pagesize) - pagesize;
            var data = GetFleetTypes(search, sort, sortdir, skip, pagesize, out totalrecord);
            ViewBag.TotalRows = totalrecord;
            ViewBag.search = search;
            ViewBag.Messagegrid = data.Count() > 0 ? "" : ViewBag.Messagegrid = "Data not available for selected criteria! please select the other criteria.";

            return View(data);
        }

        [HttpGet]
        [Authorize(Roles = "Admin,User")]
        public ActionResult CreateFleetName()
        {

           

            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin,User")]
        public ActionResult CreateFleetName(FleetNameClass fnc)
        {

            using (ShipmentContaxt sc1 = new ShipmentContaxt())
            {
                fnc.AddedBy = UserRole.username;
                fnc.FleetName = fnc.FleetName.Trim();

                if (ModelState.IsValid)
                {
                    sc1.FleetNames.Add(fnc);
                    sc1.SaveChanges();
                    return RedirectToAction("fleetname");
                }
                else
                {
                    return View(fnc);
                }
            }
            
        }

        [HttpGet]
        [Authorize(Roles = "Admin,User")]
        public ActionResult CreateFleetType()
        {

            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin,User")]
        public ActionResult CreateFleetType(FleetTypeClass ftc)
        {

            using (ShipmentContaxt sc1 = new ShipmentContaxt())
            {
                ftc.AddedBy = UserRole.username;
                ftc.FleetType = ftc.FleetType.Trim();
                if (ModelState.IsValid)
                {
                    sc1.FleetTypes.Add(ftc);
                    sc1.SaveChanges();
                    return RedirectToAction("fleettype");
                }
                else
                {
                    return View(ftc);
                }
            }
        }


        [HttpGet]
        [Authorize(Roles = "Admin,User")]
        public ActionResult EditFleetName(int id)
        {
            FleetNameClass vs = sc.FleetNames.Where(x => x.Fid == id).FirstOrDefault();

           
            return View(vs);
            
        }

        [HttpPost]
        [Authorize(Roles = "Admin,User")]
        public ActionResult EditFleetName(FleetNameClass fnc)
        {
            using (ShipmentContaxt sc1 = new ShipmentContaxt())
            {
                fnc.AddedBy = UserRole.username;
                fnc.FleetName = fnc.FleetName.Trim();
                if (ModelState.IsValid)
                {
                    sc1.Entry(fnc).State = EntityState.Modified;
                    sc1.SaveChanges();
                    return RedirectToAction("fleetname");
                }
                else
                {
                    return View(fnc);
                }
            }
        }

        [HttpGet]
        [Authorize(Roles = "Admin,User")]
        public ActionResult EditFleetType(int id)
        {
            FleetTypeClass vs = sc.FleetTypes.Where(x => x.Tid == id).FirstOrDefault();

            return View(vs);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,User")]
        public ActionResult EditFleetType(FleetTypeClass fnc)
        {
            using (ShipmentContaxt sc1 = new ShipmentContaxt())
            {
                fnc.AddedBy = UserRole.username;
                fnc.FleetType = fnc.FleetType.Trim();
                if (ModelState.IsValid)
                {
                    sc1.Entry(fnc).State = EntityState.Modified;
                    sc1.SaveChanges();
                    return RedirectToAction("fleettype");
                }
                else
                {
                    return View(fnc);
                }
            }
        }

        [Authorize(Roles = "Admin,User")]
        public async Task<ActionResult> DeleteFleetName(int? id)
        {
            using (ShipmentContaxt sc1 = new ShipmentContaxt())
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                else
                {
                    FleetNameClass vs = sc1.FleetNames.Where(x => x.Fid == id).FirstOrDefault();

                    //................................
                    var user = await sc1.CreReports.Where(p => p.FleetName == vs.FleetName).FirstOrDefaultAsync();

                    var user1 = await sc1.Certifications.Where(p => p.FleetName == vs.FleetName).FirstOrDefaultAsync();

                    var user2 = await sc1.CertificateLists.Where(p => p.FleetName == vs.FleetName).FirstOrDefaultAsync();

                    var user3 = await sc1.CrewDetails.Where(p => p.FleetName == vs.FleetName).FirstOrDefaultAsync();

                    var user4 = await sc1.Notifications.Where(p => p.FleetName == vs.FleetName).FirstOrDefaultAsync();


                    //..........................
                    if (user == null && user1 == null && user2 == null && user3 == null && user4 == null)
                    {
                        sc1.FleetNames.Remove(vs);
                        await sc1.SaveChangesAsync();
                    }
                    
                }

                return RedirectToAction("fleetname");
            }
        }

        [Authorize(Roles = "Admin,User")]
        public async Task<ActionResult> DeleteFleetType(int? id)
        {
            using (ShipmentContaxt sc1 = new ShipmentContaxt())
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                else
                {
                    FleetTypeClass vs = await sc1.FleetTypes.Where(x => x.Tid == id).FirstOrDefaultAsync();

                    //................................
                    var user = await sc1.CreReports.Where(p => p.FleetType == vs.FleetType).FirstOrDefaultAsync();

                    var user1 = await sc1.Certifications.Where(p => p.FleetType == vs.FleetType).FirstOrDefaultAsync();

                    var user2 = await sc1.CertificateLists.Where(p => p.FleetType == vs.FleetType).FirstOrDefaultAsync();

                    var user3 = await sc1.CrewDetails.Where(p => p.FleetType == vs.FleetType).FirstOrDefaultAsync();

                    var user4 = await sc1.Notifications.Where(p => p.FleetType == vs.FleetType).FirstOrDefaultAsync();

                    if (user == null && user1 == null && user2 == null && user3 == null && user4 == null)
                    {
                        //...........................
                        sc1.FleetTypes.Remove(vs);
                        await sc1.SaveChangesAsync();
                    }
                   
                }

                return RedirectToAction("fleettype");
            }
        }

        public ActionResult AutoCompletevs(string term)
        {
            List<string> students = sc.Vessels.Where(s => s.VesselName.ToLower().Contains(term.ToLower()))
                .Select(x => x.VesselName).Distinct().ToList();


            return Json(students, JsonRequestBehavior.AllowGet);

        }

        public ActionResult AutoCompleteFN(string term)
        {
            List<string> students = sc.FleetNames.Where(s => s.FleetName.ToLower().Contains(term.ToLower()))
                .Select(x => x.FleetName).Distinct().ToList();


            return Json(students, JsonRequestBehavior.AllowGet);

        }
        public ActionResult AutoCompleteFT(string term)
        {
            List<string> students = sc.FleetTypes.Where(s => s.FleetType.ToLower().Contains(term.ToLower()))
                .Select(x => x.FleetType).Distinct().ToList();


            return Json(students, JsonRequestBehavior.AllowGet);

        }


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