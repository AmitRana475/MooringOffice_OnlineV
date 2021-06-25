using MenuLayer;
using Shipment49Web.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Dynamic;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using VesselLayer;

namespace Shipment49Web.Areas.Setting.Controllers
{
    [Authorize]
    [ErrorClass]
    public class FleetTypeController : Controller
    {

        private readonly IMenuRepository sc;

        public FleetTypeController(IMenuRepository repo)
        {
            sc = repo;

            if (UserRole.username == null)
            {
                UserRole.username = string.Join("", Roles.GetRolesForUser());
            }
            ViewBag.GetMenu = UserRole.username == "Admin" ? sc.Menus.ToList() : sc.Menus.Where(x => x.Role == "User").ToList();
        }
        [HttpGet]
        [Authorize(Roles = "Admin,User")]
        public ActionResult Index(int page = 1, string sort = "FleetType", string sortdir = "asc", string search = "")
        {


            int pagesize = 10;
            int totalrecord = 0;
            if (page < 1) page = 1;
            int skip = (page * pagesize) - pagesize;
            var data = GetFleetTypes(search, sort, sortdir, skip, pagesize, out totalrecord);
            ViewBag.TotalRows = totalrecord;
            ViewBag.search = search;
            ViewBag.Messagegrid = data.Count() > 0 ? "" : ViewBag.Messagegrid = "Data Not Available For Selected Criteria! Please Select The Other Criteria.";

            return View(data);
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

        [HttpPost]
        [Authorize(Roles = "Admin,User")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(FleetTypeClass ftc)
        {


            ftc.AddedBy = UserRole.username;
            //ftc.FleetType = ftc.FleetType.Trim();
            if (ModelState.IsValid)
            {
                using (ShipmentContaxt sc1 = new ShipmentContaxt())
                {
                    sc1.FleetTypes.Add(ftc);
                    await sc1.SaveChangesAsync();
                }
                ftc = new FleetTypeClass();
                return RedirectToAction("index");
            }
            else
            {
                return View(ftc);
            }

        }



        [HttpGet]
        [Authorize(Roles = "Admin,User")]
        public ActionResult Edit(int id)
        {
            FleetTypeClass vs = sc.FleetTypes.Where(x => x.Tid == id).FirstOrDefault();

            return View(vs);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,User")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(FleetTypeClass fnc)
        {
            using (ShipmentContaxt sc1 = new ShipmentContaxt())
            {
                fnc.AddedBy = UserRole.username;
                //fnc.FleetType = fnc.FleetType.Trim();
                if (ModelState.IsValid)
                {
                    sc1.Entry(fnc).State = EntityState.Modified;
                    sc1.SaveChanges();

                    fnc = new FleetTypeClass();
                    return RedirectToAction("index");
                }
                else
                {
                    return View(fnc);
                }
            }
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
                    FleetTypeClass vs = await sc1.FleetTypes.Where(x => x.Tid == id).FirstOrDefaultAsync();

                    //................................
                    var user = await sc.CreReports.Where(p => p.FleetType == vs.FleetType).FirstOrDefaultAsync();

                    var user1 = await sc.Certifications.Where(p => p.FleetType == vs.FleetType).FirstOrDefaultAsync();

                    var user2 = await sc.CertificateLists.Where(p => p.FleetType == vs.FleetType).FirstOrDefaultAsync();

                    var user3 = await sc.CrewDetails.Where(p => p.FleetType == vs.FleetType).FirstOrDefaultAsync();

                    var user4 = await sc.Notifications.Where(p => p.FleetType == vs.FleetType).FirstOrDefaultAsync();

                    if (user == null && user1 == null && user2 == null && user3 == null && user4 == null)
                    {
                        //...........................
                        sc1.FleetTypes.Remove(vs);
                        await sc1.SaveChangesAsync();
                    }

                }

                return RedirectToAction("index");
            }
        }
        

    }
}