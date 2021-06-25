using MenuLayer;
using Reports;
using Shipment49Web.Common;
using Shipment49Web.Models;
using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace Shipment49Web.Areas.RopeInspectionSettings.Controllers
{
    [Authorize]
    [UserAuthenticationFilter]
    [ErrorClass]
    public class RopeInspectionController : BaseController
    {
        int manufacturerID = (int)CommonType.RopeManufacturer;

        public RopeInspectionController()
        {
            using (MorringOfficeEntities entities = new MorringOfficeEntities())
            {
                var _smartmenu = entities.tblSmartMenus.ToList().FirstOrDefault();
                if (_smartmenu != null)
                {
                    ViewBag.MenuContent = _smartmenu.SmartMenuContent;
                }
            }
        }

        public ActionResult Index()
        {
            using (MorringOfficeEntities entities = new MorringOfficeEntities())
            {
                var list = entities.tblRopeInspectionSettings.ToList();

                var ropeTypes = entities.MooringRopeTypes.OrderBy(p => p.RopeType).ToList();
                foreach (var v in list)
                {
                    if (ropeTypes.Count > 0)
                        v.MooringRopeTypeName = ropeTypes.First(u => u.Id == v.MooringRopeType).RopeType;
                }
                var manufacturerTypes = entities.tblCommons.Where(u => u.Type == manufacturerID).OrderBy(p => p.Name).ToList();
                foreach (var v in list)
                {
                    if (manufacturerTypes.Count > 0)
                        v.ManufacturerTypeName = manufacturerTypes.First(u => u.Id == v.ManufacturerType).Name;
                }
                return View(list);
            }
        }

        public ActionResult Create()
        {
            using (MorringOfficeEntities entities = new MorringOfficeEntities())
            {
                tblRopeInspectionSetting ropeInspectionSetting = new tblRopeInspectionSetting();

                foreach (var v in entities.MooringRopeTypes.OrderBy(p => p.RopeType).ToList())
                    ropeInspectionSetting.MooringRopeTypeLists.Add(new SelectListItem() { Text = v.RopeType, Value = v.Id.ToString() });

                foreach (var v in entities.tblCommons.Where(u => u.Type == manufacturerID).OrderBy(p => p.Name).ToList())
                    ropeInspectionSetting.ManufacturerTypeLists.Add(new SelectListItem() { Text = v.Name, Value = v.Id.ToString() });

                return View(ropeInspectionSetting);
            }
        }

        private bool CheckValidation(tblRopeInspectionSetting model)
        {
            using (MorringOfficeEntities entities = new MorringOfficeEntities())
            {
                var obj = entities.tblRopeInspectionSettings.FirstOrDefault(e => e.MooringRopeType == model.MooringRopeType &&
                    e.Id != model.Id && e.ManufacturerType == model.ManufacturerType);

                return obj == null;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [PreventSpam("CreateRopeInspSetting", 3, 1)]
        public ActionResult Create(tblRopeInspectionSetting model)
        {
            if (ModelState.IsValid)
            {
                using (MorringOfficeEntities entities = new MorringOfficeEntities())
                {
                    if (!CheckValidation(model))
                    {
                        ModelState.Clear();
                        ModelState.AddModelError("", "Please select different combination");
                        return View("Create", model);
                    }

                    //---Add
                    if (model.Id == 0)
                    {
                        entities.tblRopeInspectionSettings.Add(model);
                    }
                    //---Update
                    else
                    {
                        var obj = entities.tblRopeInspectionSettings.FirstOrDefault(e => e.Id == model.Id);
                        if (obj != null)
                        {
                            obj.MooringRopeType = model.MooringRopeType;
                            obj.ManufacturerType = model.ManufacturerType;
                            obj.MaximumRunningHours = model.MaximumRunningHours;
                            obj.MaximumMonthsAllowed = model.MaximumMonthsAllowed;
                            obj.EndToEndMonth = model.EndToEndMonth;
                            obj.RotationOnWinches = model.RotationOnWinches;
                            obj.Rating1 = model.Rating1;
                            obj.Rating2 = model.Rating2;
                            obj.Rating3 = model.Rating3;
                            obj.Rating4 = model.Rating4;
                            obj.Rating5 = model.Rating5;
                            obj.Rating6 = model.Rating6;
                            obj.Rating7 = model.Rating7;
                            entities.Entry(obj).State = EntityState.Modified;
                        }
                    }
                    try
                    {
                        entities.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }

                    return RedirectToAction("Index");
                }
            }
            return View(model);
        }

        public ActionResult Edit(int id)
        {
            using (MorringOfficeEntities entities = new MorringOfficeEntities())
            {
                var ropeInspectionSetting = entities.tblRopeInspectionSettings.FirstOrDefault(e => e.Id == id);

                foreach (var v in entities.MooringRopeTypes.OrderBy(p => p.RopeType).ToList())
                    ropeInspectionSetting.MooringRopeTypeLists.Add(new SelectListItem() { Text = v.RopeType, Value = v.Id.ToString() });

                foreach (var v in entities.tblCommons.Where(u => u.Type == manufacturerID).OrderBy(p => p.Name).ToList())
                    ropeInspectionSetting.ManufacturerTypeLists.Add(new SelectListItem() { Text = v.Name, Value = v.Id.ToString() });

                return View("Create", ropeInspectionSetting);
            }
        }

        public ActionResult Delete(int id)
        {
            using (MorringOfficeEntities entities = new MorringOfficeEntities())
            {
                var obj = entities.tblRopeInspectionSettings.FirstOrDefault(e => e.Id == id);
                obj.MooringRopeTypeName = entities.MooringRopeTypes.First(p => p.Id == obj.MooringRopeType).RopeType;
                obj.ManufacturerTypeName = entities.tblCommons.First(u => u.Id == obj.ManufacturerType).Name;
                return View(obj);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [PreventSpam("DeleteRopeInspSetting", 3, 1)]
        public ActionResult DeleteType(int id)
        {
            using (MorringOfficeEntities entities = new MorringOfficeEntities())
            {
                var obj = entities.tblRopeInspectionSettings.FirstOrDefault(e => e.Id == id);
                if (obj != null)
                {
                    entities.tblRopeInspectionSettings.Remove(obj);
                    entities.Entry(obj).State = EntityState.Deleted;
                    entities.SaveChanges();
                    return RedirectToAction("Index");
                }

                ModelState.Clear();
                ModelState.AddModelError("", "Something went wrong!");
                return View();
            }
        }

    }
}