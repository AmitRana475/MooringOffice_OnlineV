using CrewReportLayer;
using MenuLayer;
using Microsoft.AspNet.Identity;
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
    public class WorkscheduleController : Controller
    {
        private readonly IMenuRepository sc;
        public WorkscheduleController(IMenuRepository repo)
        {
            sc = repo;

            if (UserRole.username == null)
            {
                UserRole.username = string.Join("", Roles.GetRolesForUser());
            }
            
            ViewBag.GetMenu = UserRole.username == "Admin" ? sc.Menus.ToList() : sc.Menus.Where(x => x.Role == "User").ToList();
        }
        public ActionResult Index(string datefrom, string dateto, string search)
        {
            DateTime dt = DateTime.Now;
            ViewBag.vname = search == null ? string.Empty : search;
            string selectmonth = datefrom == null ? dt.ToString("MMMM") : datefrom;
            string selectyear = dateto == null ? dt.ToString("yyyy") : dateto;
            ViewBag.month = new SelectList(monthsname(), "text", "text", selectmonth);
            ViewBag.year = new SelectList(yearname(), "text", "text", selectyear);

            string usermame = User.Identity.GetUserName();
            if (usermame != "Admin")
            {
                var Vesselallot = sc.Users.Where(x => x.EmailId == usermame).FirstOrDefault();
                string[] vl = Vesselallot.VesselName.ToLower().TrimEnd(',').Split(',');

                if (!string.IsNullOrEmpty(search))
                {
                    var chk = vl.Contains(search.ToLower());
                    bmsuploaddat.vesselname = chk == false ? null : search;
                }
            }
            else
            {
                bmsuploaddat.vesselname = search;
            }
            bmsuploaddat.datefrom = DateTime.ParseExact(selectmonth, "MMMM", CultureInfo.InvariantCulture).Month;
            bmsuploaddat.dateto = Convert.ToInt32(selectyear);

           


            ViewBag.Messagegrid = "Data Not Available For Selected Criteria!  Please Select The Vessel, Month and Year to Check Report.";

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

        public async Task<ActionResult> AutoCompletecrew(string term)
        {
            if (UserRole.username == "Admin")
            {
                List<string> students = await sc.Vessels.Where(s => s.VesselName.ToLower().Contains(term.ToLower()))
                .Select(x => x.VesselName).Distinct().ToListAsync();
                return Json(students, JsonRequestBehavior.AllowGet);
            }
            else
            {
                string usermame = User.Identity.GetUserName();

                var Vesselallot = sc.Users.Where(x => x.EmailId == usermame).FirstOrDefault();
                string[] vl = Vesselallot.VesselName.TrimEnd(',').Split(',');
                                
 
                List<string> students = await sc.Vessels.Where(s => s.VesselName.ToLower().Contains(term.ToLower()))
                 .Select(x => x.VesselName).Distinct().ToListAsync();

                var adb = students.Intersect(vl).ToList();
                return Json(adb, JsonRequestBehavior.AllowGet);
            }
            

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