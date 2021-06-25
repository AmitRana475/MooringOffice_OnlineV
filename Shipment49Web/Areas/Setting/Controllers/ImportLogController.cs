using MenuLayer;
using Shipment49Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Web.Mvc;
using System.Web.Security;
using UserLayer;

namespace Shipment49Web.Areas.Setting.Controllers
{
    [Authorize]
    [ErrorClass]
    public class ImportLogController : Controller
    {
        private readonly IMenuRepository sc;
        public ImportLogController(IMenuRepository repo)
        {
            sc = repo;

            if (UserRole.username == null)
            {
                UserRole.username = string.Join("", Roles.GetRolesForUser());
            }
            ViewBag.GetMenu = UserRole.username == "Admin" ? sc.Menus.ToList() : sc.Menus.Where(x => x.Role == "User").ToList();
        }

        [HttpGet]
        public ActionResult Index(int page = 1, string sort = "DateImported", string sortdir = "desc", string search = "")
        {

            int pagesize = 10;
            int totalrecord = 0;
            if (page < 1) page = 1;
            int skip = (page * pagesize) - pagesize;
            var data = GetImportLogs(search, sort, sortdir, skip, pagesize, out totalrecord);
            ViewBag.TotalRows = totalrecord;
            ViewBag.search = search;
            ViewBag.Messagegrid = data.Count() > 0 ? "" : ViewBag.Messagegrid = "Data Not Available For Selected Criteria! Please Select The Other Criteria.";


            return View(data);
        }

        private readonly Func<IMenuRepository, List<ImportLogClass>> GetCerti = c => c.ImportLogs.ToList();

        [NonAction]
        private List<ImportLogClass> GetImportLogs(string search, string sort, string sortdir, int skip, int pagesize, out int totalrecord)
        {
            

            var v = (from a in GetCerti.Invoke(sc)
                     where
                     a.VesselName.Contains(search) ||
                     a.ImportedBy.Contains(search)
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
}