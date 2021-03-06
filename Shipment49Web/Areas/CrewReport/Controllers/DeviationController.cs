using CrewReportLayer;
using MenuLayer;
using Shipment49Web.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Shipment49Web.Areas.CrewReport.Controllers
{
    [Authorize]
    [ErrorClass]
    public class DeviationController : Controller
    {
        private readonly IMenuRepository sc;
        public DeviationController(IMenuRepository repo)
        {
            sc = repo;

            if (UserRole.username == null)
            {
                UserRole.username = string.Join("", Roles.GetRolesForUser());
            }
            ViewBag.GetMenu = UserRole.username == "Admin" ? sc.Menus.ToList() : sc.Menus.Where(x => x.Role == "User").ToList();
        }

        public ActionResult Index(string datefrom, string dateto, string search, string username)
        {
            DateTime dt = DateTime.Now;
            bmsuploaddat.search = search;
            ViewBag.vname = search == null ? string.Empty : search;
            ViewBag.username = username == null ? string.Empty : username;
            string selectmonth = datefrom == null ? dt.ToString("MMMM") : datefrom;
            string selectyear = dateto == null ? dt.ToString("yyyy") : dateto;
            ViewBag.month = new SelectList(monthsname(), "text", "text", selectmonth);
            ViewBag.year = new SelectList(yearname(), "text", "text", selectyear);
            bmsuploaddat.vesselname = search;
            bmsuploaddat.fullname = username;
            bmsuploaddat.datefrom = DateTime.ParseExact(selectmonth, "MMMM", CultureInfo.InvariantCulture).Month;
            bmsuploaddat.dateto = Convert.ToInt32(selectyear);

            ViewBag.Messagegrid = "Data not available for selected criteria! please select the other criteria.";

            return View();
        }


        private IEnumerable<SelectListItem> yearname()
        {
            int year = DateTime.Now.Year;
            List<SelectListItem> month = new List<SelectListItem>();
            for (int i = year - 7; i <= year + 7; i++)
            {
                month.Add(new SelectListItem() { Text = i.ToString() });
            }
            return month;
        }
        private IEnumerable<SelectListItem> monthsname()
        {
            List<SelectListItem> month = new List<SelectListItem>();
            for (int i = 0; i <= 12; i++)
            {
                month.Add(new SelectListItem() { Text = CultureInfo.CurrentCulture.DateTimeFormat.MonthNames[i] });
            }
            return month;
        }


        [HttpGet]
        public ActionResult AutoCompletecrewbm(string id)
        {
            if (id != null)
            {
                bmsuploaddat.search = id;
            }

            return Json(id, JsonRequestBehavior.AllowGet);
        }

        //[HttpPost]
        public async Task<ActionResult> AutoCompletecrew1(string term)
        {
            string search1 = bmsuploaddat.search;

            List<string> students = await sc.CreReports.Where(s => s.FullName.ToLower().Contains(term.ToLower()) && s.VesselName.ToLower().Contains(search1.ToLower()))
                .Select(x => x.FullName).Distinct().ToListAsync();

            return Json(students, JsonRequestBehavior.AllowGet);

        }

        public async Task<ActionResult> AutoCompletecrew(string term)
        {
            List<string> students = await sc.Vessels.Where(s => s.VesselName.ToLower().Contains(term.ToLower()))
                .Select(x => x.VesselName).Distinct().ToListAsync();

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