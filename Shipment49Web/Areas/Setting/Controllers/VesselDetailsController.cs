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
using UserLayer;
using Microsoft.AspNet.Identity;

namespace Shipment49Web.Areas.Setting.Controllers
{

    [Authorize]
    [ErrorClass]
    public class VesselDetailsController : Controller
    {
        //private ShipmentContaxt sc = new ShipmentContaxt();

        private readonly IMenuRepository sc;

        public VesselDetailsController(IMenuRepository repo)
        {
            sc = repo;

            if (UserRole.username == null)
            {
                UserRole.username = string.Join("", Roles.GetRolesForUser());
            }
            ViewBag.GetMenu = UserRole.username == "Admin" ? sc.Menus.ToList() : sc.Menus.Where(x => x.Role == "User").ToList();

            //ViewBag.GetMenu = UserRole.username == "Admin" ? sc.Menus.ToList() : sc.Menus.Where(x => x.Role == "User").ToList();
            ViewBag.GetSubMenu = UserRole.username == "Admin" ? sc.SubMenus.ToList() : sc.SubMenus.Where(x => x.Role == "User").ToList();

        }

        [Authorize(Roles = "Admin,User")]
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
            ViewBag.Messagegrid = data.Count() > 0 ? "" : ViewBag.Messagegrid = "Data Not Available For Selected Criteria! Please Select The Other Criteria.";


            return View(data);
        }

        [NonAction]
        private List<Vessel> GetVessels(string search, string sort, string sortdir, int skip, int pagesize, out int totalrecord)
        {
            if (UserRole.username == "Admin")
            {
                var v = (from a in sc.Vessels
                         where
                         a.VesselName.Contains(search) ||
                         a.Flag.Contains(search) ||
                         a.FleetName.Contains(search) ||
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
            else
            {
                string usermame = User.Identity.GetUserName();
                var Tuserlist = sc.Users.Where(x => x.EmailId == usermame).FirstOrDefault();
                string[] asignvessel = Tuserlist.VesselName.TrimEnd(',').Split(',');

                var vesslist = sc.Vessels.Where(x => asignvessel.Contains(x.VesselName)).ToList();


                var v = (from a in vesslist
                         where
                         a.VesselName.Contains(search) ||
                         a.Flag.Contains(search) ||
                         a.FleetName.Contains(search) ||
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
        public ActionResult Create([Bind(Include = "VesselName,ImoNo,Flag,FleetName,FleetType")]Vessel vs)
        {
            using (ShipmentContaxt sc1 = new ShipmentContaxt())
            {
                vs.ImoNo = vs.ImoNo;
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

                    var user5 = sc1.ImportLogs.Where(p => p.VesselName == vs.VesselName).Select(x => x.Id).ToArray();
                    foreach (var item in user5)  // ShopItems is a posted list with values 
                    {

                        var userb = new ImportLogClass() { Id = item, VesselName = vs.VesselName };
                        sc1.ImportLogs.Attach(userb);
                        sc1.Entry(userb).Property(x => x.VesselName).IsModified = true;

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


        public JsonResult checkimo(int? Id, int? imono)
        {
            var vesselInfo = sc.Vessels.FirstOrDefault(p => p.ImoNo == imono);

            if (vesselInfo.Id == Id)
                return Json(true, JsonRequestBehavior.AllowGet);
            else
                return Json(false, JsonRequestBehavior.AllowGet);

            //if (Id == imono)
            //{
            //    return Json(true, JsonRequestBehavior.AllowGet);
            //}
            //return Json(sc.Vessels.All(c => c.ImoNo != imono), JsonRequestBehavior.AllowGet);
        }


        public JsonResult checkfnameVesselCreate(string fleetname)
        {
            //if (initialProductVessel.Trim() == fleetname.Trim())
            //{

            //    return Json(true, JsonRequestBehavior.AllowGet);
            //}

            var bbb = sc.FleetNames.Where(c => c.FleetName.Equals(fleetname)).FirstOrDefault() == null ? false : true;

            return Json(bbb, JsonRequestBehavior.AllowGet);

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

                        NotificationClass userb = await sc1.Notifications.Where(x => x.Nid == item).FirstOrDefaultAsync();
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