using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Dynamic;
using System.Web.Mvc;
using MenuLayer;
using System.Web.Security;
using System.Threading.Tasks;
using System.Data.Entity;
using Shipment49Web.Common;
using Shipment49Web.Models;
using CertificationLayer;

namespace Shipment49Web.Areas.Certificate.Controllers
{
    [Authorize]
    [ErrorClass]
    public class CertificateListController : Controller
    {

        //private ShipmentContaxt sc = new ShipmentContaxt();
        private readonly IMenuRepository sc;

        public CertificateListController(IMenuRepository repo)
        {
            sc = repo;

            if (UserRole.username == null)
            {
                UserRole.username = string.Join("", Roles.GetRolesForUser());
            }
            ViewBag.GetMenu = UserRole.username == "Admin" ? sc.Menus.ToList() : sc.Menus.Where(x => x.Role == "User").ToList();
        }

        [PartialCache("OneMinuteCache")]
        private List<Menu> getmenulist()
        {

            string roleNames = string.Join("", Roles.GetRolesForUser());
            UserRole.GetMenu = roleNames == "Admin" ? sc.Menus.ToList() : sc.Menus.Where(x => x.Role == "User").ToList();

            return UserRole.GetMenu;
        }

        private readonly Func<IMenuRepository, IEnumerable<CertificateList>> GetCerti = c => c.CertificateLists.ToList();
        //[Authorize(Roles = "Admin,User")]
        [HttpGet]
        public ActionResult Index(DateTime? datefrom, DateTime? dateto, int page = 1, string sort = "ImportDate", string sortdir = "desc", string search = "", string fleetname = "", string fleettype = "",string Searchbtn="")
        {
            datefrom = Convert.ToDateTime(datefrom == null ? DateTime.Parse(DateTime.Now.AddMonths(-1).ToString()).ToString("MM/dd/yyyy", CultureInfo.InvariantCulture) : DateTime.Parse(datefrom.ToString()).ToString("MM/dd/yyyy", CultureInfo.InvariantCulture));
            dateto = Convert.ToDateTime(dateto == null ? DateTime.Parse(DateTime.Now.ToString()).ToString("MM/dd/yyyy", CultureInfo.InvariantCulture) : DateTime.Parse(dateto.ToString()).ToString("MM/dd/yyyy", CultureInfo.InvariantCulture));
            

            fleettype = fleettype.TrimEnd(',').Replace("multiselect-all", "").TrimStart(',').Trim();
            fleetname = fleetname.TrimEnd(',').Replace("multiselect-all", "").TrimStart(',').Trim();
            search = search.TrimEnd(',').Replace("multiselect-all", "").TrimStart(',').Trim();


            ViewBag.fleettypebm1 = fleettype;
            ViewBag.fleetnamebm1 = fleetname;
            ViewBag.vesselnamebm1 = search;

            int pagesize = 10;
            int totalrecords = 0;
            if (page < 1) page = 1;
            int skip = (page * pagesize) - pagesize;
            //var data = GetCertificatelist(datefrom, dateto, search, fleetname,fleettype, sort, sortdir, skip, pagesize, out totalrecords);

          


            IEnumerable<CertificateList> clist = null;

            if (string.IsNullOrEmpty(Searchbtn))
            {
                string[] searchs = search.TrimEnd(',').Split(',');
                clist = GetCerti.Invoke(sc);
                clist = clist.Where(x => (x.ImportDate >= datefrom && x.ImportDate <= dateto));
                if (clist.Count() == 0)
                {
                    clist = null;
                    datefrom = Convert.ToDateTime(DateTime.Parse(DateTime.Now.AddMonths(-3).ToString()).ToString("MM/dd/yyyy", CultureInfo.InvariantCulture));
                    clist = GetCerti.Invoke(sc).Where(x => (x.ImportDate >= datefrom && x.ImportDate <= dateto));

                }
                if (clist.Count() == 0)
                {
                    clist = null;
                    datefrom = Convert.ToDateTime(DateTime.Parse(DateTime.Now.AddMonths(-6).ToString()).ToString("MM/dd/yyyy", CultureInfo.InvariantCulture));
                    clist = GetCerti.Invoke(sc).Where(x => (x.ImportDate >= datefrom && x.ImportDate <= dateto));

                }
                if (clist.Count() == 0)
                {
                    clist = null;
                    datefrom = Convert.ToDateTime(DateTime.Parse(DateTime.Now.AddMonths(-12).ToString()).ToString("MM/dd/yyyy", CultureInfo.InvariantCulture));
                    clist = GetCerti.Invoke(sc).Where(x => (x.ImportDate >= datefrom && x.ImportDate <= dateto));

                }

                if (!string.IsNullOrEmpty(search))
                    clist = clist.Where(x => searchs.Contains(x.VesselName));


                if (UserRole.username.ToLower() != "admin")
                {
                    var vsname = sc.Users.Where(x => x.EmailId.Equals(UserRole.username1)).Select(x => x.VesselName).FirstOrDefault();
                    string[] vesselname1 = vsname.TrimEnd(',').Split(',');
                    clist = clist.Where(x => vesselname1.Contains(x.VesselName));
                }

            }
            else
            {
                clist = GetCerti.Invoke(sc);
                if (!string.IsNullOrEmpty(fleettype))
                {
                    string[] fleettypes = fleettype.TrimEnd(',').Split(',');
                    clist = clist.Where(x => fleettypes.Contains(x.FleetType) && (x.ImportDate >= datefrom && x.ImportDate <= dateto));
                }
                if (!string.IsNullOrEmpty(fleetname))
                {
                    string[] fleetnames = fleetname.TrimEnd(',').Split(',');
                    clist = clist.Where(x => fleetnames.Contains(x.FleetName) && (x.ImportDate >= datefrom && x.ImportDate <= dateto));
                }
                if (!string.IsNullOrEmpty(search))
                {
                    string[] searchs = search.TrimEnd(',').Split(',');
                    clist = clist.Where(x => searchs.Contains(x.VesselName) && (x.ImportDate >= datefrom && x.ImportDate <= dateto));
                }

                if (UserRole.username.ToLower() != "admin")
                {
                    var vsname = sc.Users.Where(x => x.EmailId.Equals(UserRole.username1)).Select(x => x.VesselName).FirstOrDefault();
                    string[] vesselname1 = vsname.TrimEnd(',').Split(',');
                    clist = clist.Where(x => vesselname1.Contains(x.VesselName)).ToList();
                }

            }

            


            totalrecords = clist.Count();
            clist = clist.OrderBy(sort + " " + sortdir);
            if (pagesize > 0)
            {
                clist = clist.Skip(skip).Take(pagesize);

            }



            ViewBag.TotalRows = totalrecords;
            ViewBag.vname = search;

            ViewBag.Messagegrid = clist.Count() > 0 ? "" : ViewBag.Messagegrid = "Data Not Available For Selected Criteria! Please Select The Other Criteria.";

            ViewBag.datefrom = DateTime.Parse(datefrom.ToString()).ToString("MM/dd/yyyy", CultureInfo.InvariantCulture);
            ViewBag.dateto = DateTime.Parse(dateto.ToString()).ToString("MM/dd/yyyy", CultureInfo.InvariantCulture);

            //..................
            ViewBag.fleettypebm = sc.AutoCompletedFleetType;
            ViewBag.fleetnamebm = sc.AutoCompletedfleetname;
            ViewBag.vesselnamebm = sc.AutoCompletevessel;
            //......................


            return View(clist.ToList());
        }


        

        /*
        [NonAction]
        private List<CertificateList> GetCertificatelist(DateTime? datefrom, DateTime? dateto, string search,string fleetname,string fleettype, string sort, string sortdir, int skip, int pagesize, out int totalrecord)
        {
            //var clist = sc.CertificateLists.Where(x => x.VesselName.Contains(search) || x.FleetName.Contains(search) || x.FleetType.Contains(search) || x.ImportDate >= datefrom && x.ImportDate <= dateto);

            //if (search != "" && datefrom == null && dateto == null)
            //{
            //    clist = sc.CertificateLists.Where(x => x.VesselName.Contains(search)||x.FleetName.Contains(search)|| x.FleetType.Contains(search));
            //}
            //else if (search == "" && datefrom != null && dateto != null)
            //{
            //    clist = sc.CertificateLists.Where
            //    (x => x.ImportDate >= datefrom && x.ImportDate <= dateto);
            //}
            //else if (search != "" && datefrom != null && dateto != null)
            //{
            //    clist = (from a in sc.CertificateLists
            //             where
            //             a.VesselName.Contains(search)|| 
            //             a.FleetName.Contains(search) ||
            //             a.FleetType.Contains(search)

            //             select a
            //        );

            //    clist = clist.Where(x => (x.ImportDate >= datefrom && x.ImportDate <= dateto));


            //    //clist = sc.CertificateLists.Where
            //    //(x => x.FleetName.Contains(search)
            //    // && (x.ImportDate >= datefrom && x.ImportDate <= dateto));
            //}


            string[] searchs = search.TrimEnd(',').Split(',');
            var clist = sc.CertificateLists.Where(x => searchs.Contains(x.VesselName));

            if (search != "" && fleetname != "" && fleettype != "" && datefrom == null && dateto == null)
            {
                string[] fleetnames = fleetname.TrimEnd(',').Split(',');
                string[] fleettypes = fleettype.TrimEnd(',').Split(',');
                clist = sc.CertificateLists.Where
                (x => fleettypes.Contains(x.FleetType) && fleetnames.Contains(x.FleetName) && searchs.Contains(x.VesselName));
            }
            else if (search == "" && fleetname != "" && fleettype != "" && datefrom != null && dateto != null)
            {
                string[] fleetnames = fleetname.TrimEnd(',').Split(',');
                string[] fleettypes = fleettype.TrimEnd(',').Split(',');
                clist = sc.CertificateLists.Where
                (x => fleettypes.Contains(x.FleetType) && fleetnames.Contains(x.FleetName) && (x.ImportDate >= datefrom && x.ImportDate <= dateto));

            }
            else if (search == "" && fleetname != "" && fleettype == "" && datefrom != null && dateto != null)
            {
                string[] fleetnames = fleetname.TrimEnd(',').Split(',');

                clist = sc.CertificateLists.Where
                (x => fleetnames.Contains(x.FleetName) && (x.ImportDate >= datefrom && x.ImportDate <= dateto));
            }
            else if (search == "" && fleetname == "" && fleettype != "" && datefrom != null && dateto != null)
            {

                string[] fleettypes = fleettype.TrimEnd(',').Split(',');
                clist = sc.CertificateLists.Where
                (x => fleettypes.Contains(x.FleetType) && (x.ImportDate >= datefrom && x.ImportDate <= dateto));
            }

            else if (search != "" && fleetname != "" && fleettype != "" && datefrom != null && dateto != null)
            {
                string[] fleetnames = fleetname.TrimEnd(',').Split(',');
                string[] fleettypes = fleettype.TrimEnd(',').Split(',');
                clist = sc.CertificateLists.Where
                (x => fleettypes.Contains(x.FleetType) && fleetnames.Contains(x.FleetName) && searchs.Contains(x.VesselName) && (x.ImportDate >= datefrom && x.ImportDate <= dateto));
            }
            else if (search != "" && fleetname != "" && fleettype == "" && datefrom != null && dateto != null)
            {
                string[] fleetnames = fleetname.TrimEnd(',').Split(',');

                clist = sc.CertificateLists.Where
                (x => fleetnames.Contains(x.FleetName) && searchs.Contains(x.VesselName) && (x.ImportDate >= datefrom && x.ImportDate <= dateto));
            }
            else if (search != "" && fleetname == "" && fleettype != "" && datefrom != null && dateto != null)
            {

                string[] fleettypes = fleettype.TrimEnd(',').Split(',');
                clist = sc.CertificateLists.Where
                (x => fleettypes.Contains(x.FleetType) && searchs.Contains(x.VesselName) && (x.ImportDate >= datefrom && x.ImportDate <= dateto));
            }
            else if (search != "" && fleetname == "" && fleettype == "" && datefrom != null && dateto != null)
            {

                clist = sc.CertificateLists.Where
                (x => searchs.Contains(x.VesselName) && (x.ImportDate >= datefrom && x.ImportDate <= dateto));
            }
            else if (search == "" && fleetname == "" && fleettype == "" && datefrom != null && dateto != null)
            {
                clist = sc.CertificateLists.Where
                (x => x.ImportDate >= datefrom && x.ImportDate <= dateto);
            }


            totalrecord = clist.Count();
            clist = clist.OrderBy(sort + " " + sortdir);
            if (pagesize > 0)
            {
                clist = clist.Skip(skip).Take(pagesize);

            }

            return clist.ToList();

            
        }
        */


        public async Task<ActionResult> AutoCompletecel(string term)
        {
            //using (VesselLayer.ShipmentContaxt sc1 = new VesselLayer.ShipmentContaxt())
            //{

            List<string> students = await sc.Vessels.Where(s => s.VesselName.ToLower().Contains(term.ToLower()))
                .Select(x => x.VesselName).Distinct().ToListAsync();

            //var students = (from a in sc.Vessels
            //                         where
            //         a.VesselName.Contains(term.ToLower()) ||
            //         a.FleetName.Contains(term.ToLower()) ||
            //         a.FleetType.Contains(term.ToLower())

            //         select  new {a.VesselName,a.FleetName,a.FleetType }
            //       );


            return Json(students, JsonRequestBehavior.AllowGet);
            //}
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