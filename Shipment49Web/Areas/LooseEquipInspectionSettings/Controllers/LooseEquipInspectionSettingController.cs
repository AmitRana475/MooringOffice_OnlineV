using Reports;
using Shipment49Web.Common;
using Shipment49Web.Models;
using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace Shipment49Web.Areas.LooseEquipInspectionSettings.Controllers
{
    [Authorize]
    [UserAuthenticationFilter]
    [ErrorClass]
    public class LooseEquipInspectionSettingController : BaseController
    {
        public LooseEquipInspectionSettingController()
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

        // GET: CommonTypes/Home
        public ActionResult Index()
        {
            using (MorringOfficeEntities entities = new MorringOfficeEntities())
            {
                var equipTypes = entities.LooseETypes.ToList();
                var list = entities.tblLooseEquipInspectionSettings.ToList();

                foreach (var v in list)
                {
                    if (equipTypes.Count > 0)
                        v.EquipmentTypeName = equipTypes.First(u => u.Id == v.EquipmentType).LooseEquipmentType;
                }
                return View(list);
            }
        }

        public ActionResult Create()
        {
            using (MorringOfficeEntities entities = new MorringOfficeEntities())
            {
                var looseEquipType = new tblLooseEquipInspectionSetting();

                foreach (var et in entities.LooseETypes.OrderBy(p => p.LooseEquipmentType).ToList())
                    looseEquipType.EquipmentTypeLists.Add(new SelectListItem() { Text = et.LooseEquipmentType, Value = et.Id.ToString() });

                return View(looseEquipType);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [PreventSpam("CreateLESettings", 3, 1)]
        public ActionResult Create(tblLooseEquipInspectionSetting model)
        {
            if (ModelState.IsValid)
            {
                using (MorringOfficeEntities entities = new MorringOfficeEntities())
                {
                    //---Add
                    if (model.Id == 0)
                    {
                        entities.tblLooseEquipInspectionSettings.Add(model);
                    }
                    //---Update
                    else
                    {
                        var obj = entities.tblLooseEquipInspectionSettings.FirstOrDefault(e => e.Id == model.Id);
                        if (obj != null)
                        {
                            obj.EquipmentType = model.EquipmentType;
                            obj.InspectionFrequency = model.InspectionFrequency;
                            obj.MaximumRunningHours = model.MaximumRunningHours;
                            obj.MaximumMonthsAllowed = model.MaximumMonthsAllowed;
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
                var looseEquipType = entities.tblLooseEquipInspectionSettings.FirstOrDefault(e => e.Id == id);

                foreach (var et in entities.LooseETypes.OrderBy(p => p.LooseEquipmentType).ToList())
                    looseEquipType.EquipmentTypeLists.Add(new SelectListItem() { Text = et.LooseEquipmentType, Value = et.Id.ToString() });

                return View("Create", looseEquipType);
            }
        }

        public ActionResult Delete(int id)
        {
            using (MorringOfficeEntities entities = new MorringOfficeEntities())
            {
                var obj = entities.tblLooseEquipInspectionSettings.FirstOrDefault(e => e.Id == id);
                obj.EquipmentTypeName = entities.LooseETypes.First(p => p.Id == obj.EquipmentType).LooseEquipmentType;
                return View(obj);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [PreventSpam("DeleteLEType", 3, 1)]
        public ActionResult DeleteType(int id)
        {
            using (MorringOfficeEntities entities = new MorringOfficeEntities())
            {
                var obj = entities.tblLooseEquipInspectionSettings.FirstOrDefault(e => e.Id == id);
                if (obj != null)
                {
                    entities.tblLooseEquipInspectionSettings.Remove(obj);
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