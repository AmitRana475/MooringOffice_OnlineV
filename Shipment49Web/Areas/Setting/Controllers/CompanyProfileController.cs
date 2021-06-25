using System.Web.Mvc;
using CompanyLayer;
using MenuLayer;
using Shipment49Web.Models;
using System.Linq;
using System.Data.Entity;
using System.Net;
using System.Web.Security;

namespace Shipment49Web.Areas.Setting.Controllers
{
    [Authorize]
    [ErrorClass]
    public class CompanyProfileController : Controller
    {
        //private readonly ShipmentContaxt sc = new ShipmentContaxt();
        private readonly IMenuRepository sc;
        public CompanyProfileController(IMenuRepository repo)
        {
            sc = repo;

            if (UserRole.username == null)
            {
                UserRole.username = string.Join("", Roles.GetRolesForUser());
            }

            ViewBag.GetMenu = UserRole.username == "Admin" ? sc.Menus.ToList() : sc.Menus.Where(x => x.Role == "User").ToList();
        }

        [Authorize(Roles ="Admin,User")]
        public ActionResult Index()
        {  
            Company com = sc.Companys.SingleOrDefault();
            
            ViewBag.com1 = com;
            return View(com);
        }
        [Authorize(Roles ="Admin")]
        [HttpGet]
        //[ValidateAntiForgeryToken]
        public ActionResult Create()
        {
           

            return View();
        }


        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Company comp)
        {
            using (ShipmentContaxt sc1 = new ShipmentContaxt())
            {
                if (ModelState.IsValid)
                {

                    sc1.Companys.Add(comp);
                    sc1.SaveChanges();

                    return RedirectToAction("index");
                }
                else
                {
                    return View(comp);
                }
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult Edit(int? id)
        {
            using (ShipmentContaxt sc1 = new ShipmentContaxt())
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Company employee = sc1.Companys.Find(id);
                if (employee == null)
                {
                    return HttpNotFound();
                }
                return View(employee);
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "CompanyID,CompanyName,EstablishYear,TotalEmployee,contectNo,FaxNo,EmailID,WebSite,Address")] Company comp)
        {
            using (ShipmentContaxt sc1 = new ShipmentContaxt())
            {
                if (ModelState.IsValid)
                {

                    sc1.Entry(comp).State = EntityState.Modified;

                    sc1.SaveChanges();

                    return RedirectToAction("index");
                }
                return View(comp);
            }
        }


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