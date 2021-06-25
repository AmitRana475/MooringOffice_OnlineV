using MenuLayer;
using Shipment49Web.Areas.MSPS.Models;
using Shipment49Web.Common;
using Shipment49Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Shipment49Web.Areas.DynamicMenus.Controllers
{
    [Authorize]
    [UserAuthenticationFilter]
    [ErrorClass]
    public class SubSubMenusController : BaseController
    {
        // GET: DynamicMenus/SubSubMenus
        private readonly IMenuRepository sc;

        public SubSubMenusController()
        {
            //sc = repo;

            //if (UserRole.username == null)
            //{
            //    UserRole.username = string.Join("", Roles.GetRolesForUser());
            //}
            //ViewBag.GetMenu = UserRole.username == "Admin" ? sc.Menus.ToList() : sc.Menus.Where(x => x.Role == "User").ToList();
            //ViewBag.GetSubMenu = UserRole.username == "Admin" ? sc.SubMenus.ToList() : sc.SubMenus.Where(x => x.Role == "User").ToList();
        }
        public ActionResult Index()
        {
            using (ShipmentContaxt sc1 = new ShipmentContaxt())
            {
                MenuIndex model = new MenuIndex();
                var ilist = new ShipmentContaxt()
                     .MultipleResults("[dbo].[spDynamicMenus] 10,0,'SubSubMenu'")
                   .With<TotalCount>()
                   .With<SubSubMenusDetails>()
                   .Execute();
                model.subsubmenuListing = (List<SubSubMenusDetails>)ilist[1];
                List<TotalCount> totobj = (List<TotalCount>)ilist[0];
                model.Total = totobj.FirstOrDefault().Total;
                ViewBag.TotalCount = model.Total;
                var pager = new Pager(Convert.ToInt32(model.Total), 0);
                model.Pager = pager;
                return View(model);
            }
        }

        [HttpGet]
        public ActionResult Add()
        {
            using (ShipmentContaxt context = new ShipmentContaxt())
            {
                var Menulist = context.Menus.ToList();
                ViewBag.Menulist = Menulist.AsQueryable().Select(x => new SelectListItem { Text = x.MenuName, Value = x.MId.ToString() }).ToList();
                return View();
            }
        }


        public JsonResult GetSubMenu(int id)
        {
            ShipmentContaxt sc1 = new ShipmentContaxt();
            // List<SelectListItem> docs = new List<SelectListItem>();
            var SubMenuList = sc1.SubMenus.ToList().Where(m => m.MId == id).ToList();
            //var docsList = "";
            var docData = SubMenuList.Select(m => new SelectListItem()
            {
                Text = m.SubMenuName,
                Value = m.SubId.ToString(),
            });
            return Json(docData, JsonRequestBehavior.AllowGet);
        }
    }
}