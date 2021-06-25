using MenuLayer;
using Shipment49Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Shipment49Web.Areas.Rules.Controllers
{
    [Authorize]
    [ErrorClass]
    public class ruleController : Controller
    {
        private readonly IMenuRepository sc;
        public ruleController(IMenuRepository repo)
        {
            sc = repo;
            if (UserRole.username == null)
            {
                UserRole.username = string.Join("", Roles.GetRolesForUser());
            }

            ViewBag.GetMenu = UserRole.username == "Admin" ? sc.Menus.ToList() : sc.Menus.Where(x => x.Role == "User").ToList();
        }
        // GET: Rules/rule
        public ActionResult Index()
        {
            return View();
        }
    }
}