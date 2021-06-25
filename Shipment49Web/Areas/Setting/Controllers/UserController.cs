using PagedList;
using Shipment49Web.Common;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Dynamic;
using System.Net;
using System.Web.Mvc;
using UserLayer;
using MenuLayer;
using System.Web.Security;
using Shipment49Web.Models;
using System.Web.Routing;

namespace Shipment49Web.Areas.Setting.Controllers
{
    [Authorize]
    [ErrorClass]
    public class UserController : Controller
    {

        //private ShipmentContaxt sc = new ShipmentContaxt();

        private readonly IMenuRepository sc;
        public UserController(IMenuRepository repo)
        {
            sc = repo;

            if (UserRole.username == null)
            {
                UserRole.username = string.Join("", Roles.GetRolesForUser());
            }
            ViewBag.GetMenu = UserRole.username == "Admin" ? sc.Menus.ToList() : sc.Menus.Where(x => x.Role == "User").ToList();
        }


        //[Authorize(Roles = "Admin,User")]
        public ActionResult Index(int page = 1, string sort = "FullName", string sortdir = "asc", string search="")
        {
            int pagesize = 10;
            int totalrecord = 0;
            if (page < 1) page = 1;
            int skip = (page * pagesize) - pagesize;
            var data = Getusers(search, sort, sortdir, skip, pagesize, out totalrecord);
            ViewBag.TotalRows = totalrecord;
            ViewBag.search = search;

            ViewBag.Messagegrid = data.Count() > 0 ? "" : ViewBag.Messagegrid = "Data not available for selected criteria! please select the other criteria.";

            return View(data);
        }


        [NonAction]
        public List<UserClass> Getusers(string search, string sort, string sortdir, int skip, int pagesize, out int totalrecord)
        {
            var user = (from a in sc.Users
                        where
                        a.FullName.Contains(search) ||
                        a.EmailId.Contains(search) ||
                        a.Designation.Contains(search)
                        select a);

            totalrecord = user.Count();
            user = user.OrderBy(sort + " " + sortdir);
            if (pagesize > 0)
            {
                user = user.Skip(skip).Take(pagesize);
            }
            return user.ToList();
        }

        


        [Authorize(Roles = "Admin,User")]
        public ActionResult Create()
        {
            ModelState.Clear();

            ViewBag.vesselnamebm = sc.AutoCompletevessel;

            return View();
        }

        [Authorize(Roles = "Admin,User")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(UserClass user, string vesselname )
        {
            using (ShipmentContaxt sc1 = new ShipmentContaxt())
            {
                sc1.Configuration.ProxyCreationEnabled = false;

                ViewBag.vesselnamebm = sc.AutoCompletevessel;



                string vesselid = "";
                string vesselname1 = "";
                string[] bms = vesselname.TrimEnd(',').Split(',').ToArray();


                foreach (string bb in bms)
                {
                    vesselid += sc.Vessels.Where(x => x.VesselName.ToString() == bb).Select(p => p.VesselID).Distinct().FirstOrDefault() + ",";
                    vesselname1 += bb + ",";
                }

                user.VesselName = vesselname1.TrimEnd(',');
                user.VesselID = vesselid.TrimEnd(',');
                user.Role = "User";
                if (string.IsNullOrEmpty(vesselname))
                {
                    ModelState.AddModelError("VesselName", "VesselName is required");
                }

                if (ModelState.IsValid)
                {
                    sc1.Users.Add(user);
                    sc1.SaveChanges();
                    return RedirectToAction("index");
                }
                else
                {
                    return View(user);
                }
            }
        }

        [HttpGet]
        [Authorize(Roles = "Admin,User")]
        public ActionResult Edit(int? id)
        {
            UserClass uc = sc.Users.Where(x => x.UserId == id).FirstOrDefault();
            uc.ConfirmPassowrd = uc.Password;

            ViewBag.vesselnamebm1 = uc.VesselName;

            ViewBag.vesselnamebm = sc.AutoCompletevessel;

          

            return View(uc);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,User")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit( UserClass us, string vesselname)
        {
            using (ShipmentContaxt sc1 = new ShipmentContaxt())
            {
                sc1.Configuration.ProxyCreationEnabled = false;

                ViewBag.vesselnamebm = sc.AutoCompletevessel;

                string vesselid = "";
                string vesselname1 = "";
                string[] bms = vesselname.TrimEnd(',').Split(',').ToArray();


                foreach (string bb in bms)
                {
                    vesselid += sc.Vessels.Where(x => x.VesselName.ToString() == bb).Select(p => p.VesselID).Distinct().FirstOrDefault() + ",";
                    vesselname1 += bb + ",";
                }

                us.VesselName = vesselname1.TrimEnd(',');
                us.VesselID = vesselid.TrimEnd(',');
                us.Role = "User";
                if (string.IsNullOrEmpty(vesselname))
                {
                    ModelState.AddModelError("VesselName", "VesselName is required");
                }

                if (ModelState.IsValid)
                {
                    sc1.Entry(us).State = EntityState.Modified;
                    sc1.SaveChanges();

                    return RedirectToAction("index");
                   

                }
                else
                {
                    return View(us);
                }
            }
        }
        [Authorize(Roles = "Admin,User")]
        public ActionResult Detail(int? id)
        {
            using (ShipmentContaxt sc1 = new ShipmentContaxt())
            {
                sc1.Configuration.ProxyCreationEnabled = false;

                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                UserClass user = sc1.Users.Find(id);
                if (user == null)
                {
                    return HttpNotFound();
                }
                return View(user);
            }
        }

        [Authorize(Roles = "Admin,User")]
        public ActionResult Delete(int? id)
        {
            using (ShipmentContaxt sc1 = new ShipmentContaxt())
            {
                sc1.Configuration.ProxyCreationEnabled = false;

                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

                }
                else
                {
                    UserClass uc = sc1.Users.Where(x => x.UserId == id).FirstOrDefault();
                    sc1.Users.Remove(uc);
                    sc1.SaveChanges();
                }
                return RedirectToAction("index");
            }
        }
        public JsonResult checkuser(string initialProductCode, string emailid)
        {
            if (emailid == initialProductCode)
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            return Json(sc.Users.All(u => u.EmailId != emailid), JsonRequestBehavior.AllowGet);
        }

        //[PartialCache("OneMinuteCache")]
        public JsonResult Autocompletedropdown()
        {
            List<VesselLayer.Vessel> students = sc.Vessels.Distinct().ToList();

            return Json(students, JsonRequestBehavior.AllowGet);

        }

        //[HttpPost]
        //[OutputCache(Duration =20)]
        //public JsonResult Autocompletebm(string prefix)
        //{

        //    var result = (from r in sc1.Vessels
        //                  where r.VesselName.ToLower().Contains(prefix.ToLower())
        //                  select new
        //                  {
        //                      vesselname = r.VesselName
        //                  }).Distinct();

        //    if (result.Count() == 0)
        //    {

        //        ModelState.AddModelError("VesselName", "This Vessel not in database");
        //    }


        //    //List<string> vessel = sc1.Vessels.Where(s => s.VesselName.ToLower().Contains(prefix.ToLower())).Select(x => x.VesselName.ToLower()).ToList();

        //    return new JsonResult { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        //    //return Json(result, JsonRequestBehavior.AllowGet);
        //}


        //public JsonResult SkillBinding()
        //{

        //    List<VesselLayer.Vessel> skillb = sc1.Vessels.ToList();
        //    return Json(skillb, JsonRequestBehavior.AllowGet);
        //}
        //public List<SelectListItem> SkillBinding1()
        //{

        //    List<SelectListItem> ls = new List<SelectListItem>();

        //    List<Location> emps = _context.Locations.ToList();

        //    foreach (var temp in emps)
        //    {
        //        ls.Add(new SelectListItem() { Text = temp.City, Value = temp.LocationId.ToString() });
        //    }
        //    return ls;
        //}


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