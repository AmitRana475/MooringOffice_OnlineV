using MenuLayer;
using Shipment49Web.Common;
using Shipment49Web.Models;
using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using VesselLayer;

namespace Shipment49Web.Areas.CommonTypes.Controllers
{
    [Authorize]
    [UserAuthenticationFilter]
    [ErrorClass]
    public class VesselInfoController : BaseController
    {
        public VesselInfoController()
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

        // GET: CommonTypes/Vessel
        public ActionResult Index()
        {
            using (ShipmentContaxt sc1 = new ShipmentContaxt())
            {
                var list = sc1.Vessels.ToList();
                return View(list);
            }
        }

        private void SetViewBag()
        {
            using (ShipmentContaxt sc1 = new ShipmentContaxt())
            {
                //ViewBag.FleetNames = base.PermittedFleetNames; // sc1.MasterCommons.Where(p => p.Type == CommonType.FleetName).ToList();
                //ViewBag.FleetTypes = base.PermittedFleetTypes; // sc1.MasterCommons.Where(p => p.Type == CommonType.FleetType).ToList();

                ViewBag.FleetNames = sc1.MasterCommons.Where(p => p.Type == CommonType.FleetName).ToList();
                ViewBag.FleetTypes = sc1.MasterCommons.Where(p => p.Type == CommonType.FleetType).ToList();
                ViewBag.TradePlatforms = sc1.MasterCommons.Where(p => p.Type == CommonType.TradePlatform).ToList();
            }
        }

        // GET: CommonTypes/Vessel/Create
        public ActionResult Create()
        {
            SetViewBag();

            Vessel vessel = new Vessel();
            vessel.DateBuilt = DateTime.Today;

            return View(vessel);
        }

        // POST: CommonTypes/Vessel/Create
        [HttpPost]
        [PreventSpam("CreateVesselInfo", 3, 1)]
        public ActionResult Create(Vessel model)
        {
            if (ModelState.IsValid)
            {
                using (ShipmentContaxt sc1 = new ShipmentContaxt())
                {
                    //---Add
                    if (model.Id == 0)
                    {
                        model.FleetName = sc1.MasterCommons.First(e => e.Id == model.FleetNameID).Name;
                        model.FleetType = sc1.MasterCommons.First(e => e.Id == model.FleetTypeID).Name;
                        model.TradeArea = sc1.MasterCommons.First(e => e.Id == model.TradeAreaID).Name;

                        sc1.Vessels.Add(model);
                    }
                    //---Update
                    else
                    {
                        var obj = sc1.Vessels.FirstOrDefault(e => e.Id == model.Id);
                        if (obj != null)
                        {
                            obj.VesselName = model.VesselName;
                            obj.ImoNo = model.ImoNo;
                            obj.Flag = model.Flag;
                            obj.MinimumRopes = model.MinimumRopes;
                            obj.MinimumRopeTails = model.MinimumRopeTails;
                            obj.DateBuilt = model.DateBuilt;
                            obj.FleetNameID = model.FleetNameID;
                            obj.FleetTypeID = model.FleetTypeID;
                            obj.TradeAreaID = model.TradeAreaID;
                            obj.FleetName = sc1.MasterCommons.First(e => e.Id == model.FleetNameID).Name;
                            obj.FleetType = sc1.MasterCommons.First(e => e.Id == model.FleetTypeID).Name;
                            obj.TradeArea = sc1.MasterCommons.First(e => e.Id == model.TradeAreaID).Name;

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
                    return RedirectToAction("Index");
                }
            }
            SetViewBag();
            return View(model);
        }

        // GET: CommonTypes/Vessel/Edit/5
        public ActionResult Edit(int id)
        {
            SetViewBag();
            using (ShipmentContaxt sc1 = new ShipmentContaxt())
            {
                var obj = sc1.Vessels.FirstOrDefault(e => e.Id == id); //base.PermittedVessels.FirstOrDefault(e => e.Id == id);
                return View("Create", obj);
            }
        }


        // GET: CommonTypes/Vessel/Delete/5
        public ActionResult Delete(int id)
        {
            using (ShipmentContaxt sc1 = new ShipmentContaxt())
            {
                var obj = sc1.Vessels.FirstOrDefault(e => e.Id == id); //base.PermittedVessels.FirstOrDefault(e => e.Id == id);
                return View(obj);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [PreventSpam("DeleteVesselInfo", 3, 1)]

        public ActionResult DeleteType(int id)
        {
            using (ShipmentContaxt sc1 = new ShipmentContaxt())
            {
                var obj = sc1.Vessels.FirstOrDefault(e => e.Id == id);
                if (obj != null)
                {
                    sc1.Vessels.Remove(obj);
                    sc1.Entry(obj).State = EntityState.Deleted;
                    sc1.SaveChanges();
                    return RedirectToAction("Index");
                }

                ModelState.Clear();
                ModelState.AddModelError("", "Something went wrong!");
                return View();
            }
        }

        public JsonResult checkimo(int? Id, int? imono)
        {
            using (ShipmentContaxt sc = new ShipmentContaxt())
            {
                var vesselInfo = sc.Vessels.FirstOrDefault(p => p.ImoNo == imono);

                if (vesselInfo == null)
                    return Json(true, JsonRequestBehavior.AllowGet);
                else if (vesselInfo.Id == Id)
                    return Json(true, JsonRequestBehavior.AllowGet);
                else
                    return Json(false, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult checkfnameVesselCreate(string fleetname)
        {
            using (ShipmentContaxt sc = new ShipmentContaxt())
            {
                var bbb = sc.FleetNames.Where(c => c.FleetName.Equals(fleetname)).FirstOrDefault() == null ? false : true;
                return Json(bbb, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult checkftypeVesselCreate(string fleettype)
        {
            using (ShipmentContaxt sc = new ShipmentContaxt())
            {
                var bbb = sc.FleetTypes.Where(c => c.FleetType.Equals(fleettype)).FirstOrDefault() == null ? false : true;
                return Json(bbb, JsonRequestBehavior.AllowGet);
            }
        }
    }
}