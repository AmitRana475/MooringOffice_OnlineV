using MenuLayer;
using Shipment49Web.Areas.MSPS.Models;
using Shipment49Web.Common;
using Shipment49Web.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace Shipment49Web.Areas.DynamicMenus.Controllers
{
    [Authorize]
    [UserAuthenticationFilter]
    [ErrorClass]
    public class SubMenusController : BaseController
    {
        // GET: DynamicMenus/SubMenus
        private readonly IMenuRepository sc;

        public SubMenusController()
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
                     .MultipleResults("[dbo].[spDynamicMenus] 10,0,'SubMenu'")
                   .With<TotalCount>()
                   .With<SubMenusDetails>()
                   .Execute();
                model.submenuListing = (List<SubMenusDetails>)ilist[1];
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

        [HttpPost]
        public ActionResult Add(MSPS.Models.DynamicMenuModel txt)
        {
            using (ShipmentContaxt sc1 = new ShipmentContaxt())
            {
                if (ModelState.IsValid)
                {
                    txt.SubMenus.MId = txt.SubMenus.MId;
                    txt.SubMenus.SubMenuName = txt.SubMenus.SubMenuName;

                    sc1.SubMenus.Add(txt.SubMenus);
                    sc1.SaveChanges();
                    return RedirectToAction("index");
                }
                else
                {
                    return View(txt);
                }
            }
        }

        [HttpGet]
        public ActionResult Edit(Int32? id)
        {
            using (ShipmentContaxt context = new ShipmentContaxt())
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                MSPS.Models.DynamicMenuModel model = new MSPS.Models.DynamicMenuModel();
                model.SubMenus = context.SubMenus.Find(id);
                var Menulist = context.Menus.ToList();
                ViewBag.Menulist = Menulist.AsQueryable().Select(x => new SelectListItem { Text = x.MenuName, Value = x.MId.ToString() }).ToList();
                if (model == null)
                {
                    return HttpNotFound();
                }
                return View(model);
            }
        }
        [HttpPost]
        public ActionResult Edit(MSPS.Models.DynamicMenuModel model, Int32 id)
        {
            using (ShipmentContaxt context = new ShipmentContaxt())
            {
                var modifysubmenuTble = context.SubMenus.ToList().Where(x => x.SubId == id).FirstOrDefault();

                modifysubmenuTble.MId = model.SubMenus.MId;

                modifysubmenuTble.SubMenuName = model.SubMenus.SubMenuName;

                context.Entry(modifysubmenuTble).State = EntityState.Modified;
                context.SaveChanges();
            }
            return RedirectToAction("Index");
        }

    }
}