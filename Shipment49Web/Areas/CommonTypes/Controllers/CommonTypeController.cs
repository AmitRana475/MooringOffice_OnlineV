using MenuLayer;
using Shipment49Web.Common;
using Shipment49Web.Models;
using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace Shipment49Web.Areas.CommonTypes.Controllers
{
    [Authorize]
    [UserAuthenticationFilter]
    [ErrorClass]
    public class CommonTypeController : BaseController
    {
        public CommonTypeController()
        {
            using (ShipmentContaxt sc1 = new ShipmentContaxt())
            {
                var _smartmenu = sc1.SmartMenus.ToList().FirstOrDefault();
                if (_smartmenu != null)
                {
                    ViewBag.MenuContent = _smartmenu.SmartMenuContent;
                }
            }
        }

        public ActionResult Index(CommonType type)
        {
            using (ShipmentContaxt sc1 = new ShipmentContaxt())
            {
                ViewBag.Type = type;
                var list = sc1.MasterCommons.Where(e => e.Type == type).ToList();
                return View(list);
            }
        }

        public ActionResult Create(CommonType type)
        {
            ViewBag.Type = type;
            var obj = new MasterCommon() { Type = type };
            return View(obj);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [PreventSpam("CreateCommonType", 3, 1)]
        public ActionResult Create(MasterCommon model)
        {
            if (ModelState.IsValid)
            {
                using (ShipmentContaxt sc1 = new ShipmentContaxt())
                {
                    //---Add
                    if (model.Id == 0)
                    {
                        sc1.MasterCommons.Add(model);
                    }
                    //---Update
                    else
                    {
                        var obj = sc1.MasterCommons.FirstOrDefault(e => e.Id == model.Id);
                        if (obj != null)
                        {
                            obj.Name = model.Name;
                            sc1.Entry(obj).State = EntityState.Modified;
                        }
                    }
                    try
                    {
                        sc1.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    return RedirectToAction("Index", new { type = model.Type });
                }
            }
            return View(model);
        }

        public ActionResult Edit(int id)
        {
            using (ShipmentContaxt sc1 = new ShipmentContaxt())
            {
                var obj = sc1.MasterCommons.FirstOrDefault(e => e.Id == id);
                ViewBag.Type = obj.Type;
                return View("Create", obj);
            }
        }


        public ActionResult Delete(int id)
        {
            using (ShipmentContaxt sc1 = new ShipmentContaxt())
            {
                var obj = sc1.MasterCommons.FirstOrDefault(e => e.Id == id);
                ViewBag.Type = obj.Type;
                return View(obj);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [PreventSpam("DeleteCommonType", 3, 1)]
        public ActionResult DeleteType(int id)
        {
            using (ShipmentContaxt sc1 = new ShipmentContaxt())
            {
                var obj = sc1.MasterCommons.FirstOrDefault(e => e.Id == id);
                if (obj != null)
                {
                    sc1.MasterCommons.Remove(obj);
                    //TODO:Conflict with rope inspection setting
                    sc1.Entry(obj).State = EntityState.Deleted;
                    sc1.SaveChanges();
                    return RedirectToAction("Index", new { type = obj.Type });
                }

                ModelState.Clear();
                ModelState.AddModelError("", "Something went wrong!");
                return View();
            }
        }

    }
}