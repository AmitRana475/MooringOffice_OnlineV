using DocumentFormat.OpenXml.Drawing;
using MenuLayer;
using Reports;
using Shipment49Web.Common;
using Shipment49Web.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Shipment49Web.Areas.WinchRotationSettings.Controllers
{
    [Authorize]
    [UserAuthenticationFilter]
    [ErrorClass]
    public class WinchRotationController : Controller
    {
        int manufacturerID = (int)CommonType.RopeManufacturer;
        public WinchRotationController()
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
        public async Task<ActionResult> Index(int? cp)
        {
            using (MorringOfficeEntities entities = new MorringOfficeEntities())
            {

               


                var list = await entities.tblWinchRotationSettings.ToListAsync();
              var vesll= await entities.VesselDetails.ToListAsync();
                var ropeTypes = entities.MooringRopeTypes.OrderBy(p => p.RopeType).ToList();
                foreach (var v in list)
                {
                    if (ropeTypes.Count > 0)
                        v.MooringRopeTypeName = ropeTypes.First(u => u.Id == v.MooringRopeType).RopeType;

                    v.VesselName = vesll.First(s => s.ImoNo == v.VesselID).VesselName;
                }
                var manufacturerTypes = entities.tblCommons.Where(u => u.Type == manufacturerID).OrderBy(p => p.Name).ToList();
                foreach (var v in list)
                {
                    if (manufacturerTypes.Count > 0)
                        v.ManufacturerTypeName = manufacturerTypes.First(u => u.Id == v.ManufacturerType).Name;

                   
                }

                //======================================
                List<SelectListItem> FleetNameList = new List<SelectListItem>();
                List<SelectListItem> FleetTypeList = new List<SelectListItem>();

                //foreach (var v in entities.tblFleetNames.OrderBy(p => p.Fid).ToList())
                //    FleetNameList.Add(new SelectListItem() { Text = v.FleetName, Value = v.Fid.ToString() });

                //foreach (var v in entities.tblFleetTypes.OrderBy(p => p.Tid).ToList())
                //    FleetTypeList.Add(new SelectListItem() { Text = v.FleetType, Value = v.Tid.ToString() });

                foreach (var v in entities.tblCommons.Where(x => x.Type == 2).OrderBy(p => p.Id).ToList())
                    FleetNameList.Add(new SelectListItem() { Text = v.Name, Value = v.Id.ToString() });

                foreach (var v in entities.tblCommons.Where(x => x.Type == 3).OrderBy(p => p.Id).ToList())
                    FleetTypeList.Add(new SelectListItem() { Text = v.Name, Value = v.Id.ToString() });

                List<SelectListItem> VesselList = new List<SelectListItem>();
                foreach (var v in entities.VesselDetails.OrderBy(p => p.VesselName).ToList())
                    VesselList.Add(new SelectListItem() { Text = v.VesselName, Value = v.ImoNo.ToString() });

                List<SelectListItem> RopeType = new List<SelectListItem>();
                List<SelectListItem> ManufacturerType = new List<SelectListItem>();
                foreach (var v in entities.MooringRopeTypes.OrderBy(p => p.RopeType).ToList())
                    RopeType.Add(new SelectListItem() { Text = v.RopeType, Value = v.Id.ToString() });

                foreach (var v in entities.tblCommons.Where(u => u.Type == manufacturerID).OrderBy(p => p.Name).ToList())
                    ManufacturerType.Add(new SelectListItem() { Text = v.Name, Value = v.Id.ToString() });


                ViewBag.FleetNameListing = FleetNameList;
                ViewBag.FleetTypeListing = FleetTypeList;
                ViewBag.VesselListing = VesselList;
                ViewBag.RopeTypeListing = RopeType;
                ViewBag.ManufacturerTypeListing = ManufacturerType;

                // find frond end code =>  Reports > OverdueInspection 
                int currPage_Pending = cp == null ? 1 : Convert.ToInt32(cp);
                TempData["CurrentPage_Pending"] = currPage_Pending;
                TempData["TotalRecords_Pending"] = list.Count();

                var paginglsit = list.Skip((CommonMethods.PAGESIZE * currPage_Pending) - CommonMethods.PAGESIZE).Take(CommonMethods.PAGESIZE).ToList();

                return View(paginglsit);
            }
        }

        [HttpPost]
        public async Task<ActionResult>  Index(int? cp,List<int> VesselIDs, List<int> MooringRopeTypeIDs, List<int> ManufacturerTypeIDs)
        {
            using (MorringOfficeEntities entities = new MorringOfficeEntities())
            {
                var list = await entities.tblWinchRotationSettings.ToListAsync();
                var vesll = await entities.VesselDetails.ToListAsync();
                var ropeTypes = entities.MooringRopeTypes.OrderBy(p => p.RopeType).ToList();
                foreach (var v in list)
                {
                    if (ropeTypes.Count > 0)
                        v.MooringRopeTypeName = ropeTypes.First(u => u.Id == v.MooringRopeType).RopeType;

                    v.VesselName = vesll.First(s => s.ImoNo == v.VesselID).VesselName;
                }
                var manufacturerTypes = entities.tblCommons.Where(u => u.Type == manufacturerID).OrderBy(p => p.Name).ToList();
                foreach (var v in list)
                {
                    if (manufacturerTypes.Count > 0)
                        v.ManufacturerTypeName = manufacturerTypes.First(u => u.Id == v.ManufacturerType).Name;


                }

                //======================================
                List<SelectListItem> FleetNameList = new List<SelectListItem>();
                List<SelectListItem> FleetTypeList = new List<SelectListItem>();
                List<SelectListItem> VesselList = new List<SelectListItem>();
                foreach (var v in entities.tblCommons.Where(x => x.Type == 2).OrderBy(p => p.Id).ToList())
                    FleetNameList.Add(new SelectListItem() { Text = v.Name, Value = v.Id.ToString() });

                foreach (var v in entities.tblCommons.Where(x => x.Type == 3).OrderBy(p => p.Id).ToList())
                    FleetTypeList.Add(new SelectListItem() { Text = v.Name, Value = v.Id.ToString() });

               
                foreach (var v in entities.VesselDetails.OrderBy(p => p.VesselName).ToList())
                    VesselList.Add(new SelectListItem() { Text = v.VesselName, Value = v.ImoNo.ToString() });

                List<SelectListItem> RopeType = new List<SelectListItem>();
                List<SelectListItem> ManufacturerType = new List<SelectListItem>();
                foreach (var v in entities.MooringRopeTypes.OrderBy(p => p.RopeType).ToList())
                    RopeType.Add(new SelectListItem() { Text = v.RopeType, Value = v.Id.ToString() });

                foreach (var v in entities.tblCommons.Where(u => u.Type == manufacturerID).OrderBy(p => p.Name).ToList())
                    ManufacturerType.Add(new SelectListItem() { Text = v.Name, Value = v.Id.ToString() });

                // ViewBag.DropListing = VesselList;

                if (VesselIDs?.Count > 0)
                    list = list.Where(p => VesselIDs.Contains(p.VesselID ?? 0)).ToList();
                
                if (MooringRopeTypeIDs?.Count > 0)
                    list = list.Where(p => MooringRopeTypeIDs.Contains(p.MooringRopeType)).ToList();

                if (ManufacturerTypeIDs?.Count > 0)
                    list = list.Where(p => ManufacturerTypeIDs.Contains(p.ManufacturerType)).ToList();

                ViewBag.FleetNameListing = FleetNameList;
                ViewBag.FleetTypeListing = FleetTypeList;
                ViewBag.VesselListing = VesselList;
                ViewBag.RopeTypeListing = RopeType;
                ViewBag.ManufacturerTypeListing = ManufacturerType;

                int currPage_Pending = cp == null ? 1 : Convert.ToInt32(cp);
                TempData["CurrentPage_Pending"] = currPage_Pending;
                TempData["TotalRecords_Pending"] = list.Count();

                var paginglsit = list.Skip((CommonMethods.PAGESIZE * currPage_Pending) - CommonMethods.PAGESIZE).Take(CommonMethods.PAGESIZE).ToList();


                return View(paginglsit);
            }
        }

        public ActionResult Create()
        {
            using (MorringOfficeEntities entities = new MorringOfficeEntities())
            {
                tblWinchRotationSetting ropeInspectionSetting = new tblWinchRotationSetting();

                foreach (var v in entities.tblCommons.Where(x => x.Type == 2).OrderBy(p => p.Id).ToList())
                    ropeInspectionSetting.FleetNameList.Add(new SelectListItem() { Text = v.Name, Value = v.Id.ToString() });

                foreach (var v in entities.tblCommons.Where(x => x.Type == 3).OrderBy(p => p.Id).ToList())
                    ropeInspectionSetting.FleetTypeList.Add(new SelectListItem() { Text = v.Name, Value = v.Id.ToString() });
                               
                foreach (var v in entities.VesselDetails.OrderBy(p => p.ImoNo).ToList())
                    ropeInspectionSetting.VesselList.Add(new SelectListItem() { Text = v.VesselName, Value = v.ImoNo.ToString() });

                //===========================================================
                foreach (var v in entities.MooringRopeTypes.OrderBy(p => p.RopeType).ToList())
                    ropeInspectionSetting.MooringRopeTypeLists.Add(new SelectListItem() { Text = v.RopeType, Value = v.Id.ToString() });

                foreach (var v in entities.tblCommons.Where(u => u.Type == manufacturerID).OrderBy(p => p.Name).ToList())
                    ropeInspectionSetting.ManufacturerTypeLists.Add(new SelectListItem() { Text = v.Name, Value = v.Id.ToString() });

                return View(ropeInspectionSetting);
            }
        }

        private bool CheckValidation(tblWinchRotationSetting model)
        {
            using (MorringOfficeEntities entities = new MorringOfficeEntities())
            {
                var obj = entities.tblWinchRotationSettings.FirstOrDefault(e => e.MooringRopeType == model.MooringRopeType &&
                    e.Id != model.Id && e.ManufacturerType == model.ManufacturerType);

                return obj == null;
            }
        }


        public JsonResult GetVesselDetails(string fleetNames, string fleetTypes, string tradeAreas)
        {
            fleetNames = fleetNames.EndsWith(",") ? fleetNames.Substring(0, fleetNames.Length - 1) : fleetNames;
            fleetTypes = fleetTypes.EndsWith(",") ? fleetTypes.Substring(0, fleetTypes.Length - 1) : fleetTypes;
            tradeAreas = tradeAreas.EndsWith(",") ? tradeAreas.Substring(0, tradeAreas.Length - 1) : tradeAreas;

            List<int> fleets = new List<int>();
            int id = 0;

            if (!string.IsNullOrEmpty(fleetNames))
            {
                foreach (string s in fleetNames.Split(','))
                {
                    int.TryParse(s, out id);
                    fleets.Add(id);
                }
            }

            List<int> ftypes = new List<int>();

            if (!string.IsNullOrEmpty(fleetTypes))
            {
                foreach (string s in fleetTypes.Split(','))
                {
                    int.TryParse(s, out id);
                    ftypes.Add(id);
                }
            }

            List<int> areas = new List<int>();

            if (!string.IsNullOrEmpty(tradeAreas))
            {
                foreach (string s in tradeAreas.Split(','))
                {
                    int.TryParse(s, out id);
                    areas.Add(id);
                }
            }

            // var vessels = base.PermittedVessels; // context.VesselDetails.AsEnumerable();

            List<VesselDetail> vessels = new List<VesselDetail>();

            using (MorringOfficeEntities entities = new MorringOfficeEntities())
            {
                vessels = entities.VesselDetails.ToList();

                //tblWinchRotationSetting ropeInspectionSetting = new tblWinchRotationSetting();

                //foreach (var v in entities.MooringRopeTypes.OrderBy(p => p.RopeType).ToList())
                //    ropeInspectionSetting.MooringRopeTypeLists.Add(new SelectListItem() { Text = v.RopeType, Value = v.Id.ToString() });

                //foreach (var v in entities.tblCommons.Where(u => u.Type == manufacturerID).OrderBy(p => p.Name).ToList())
                //    ropeInspectionSetting.ManufacturerTypeLists.Add(new SelectListItem() { Text = v.Name, Value = v.Id.ToString() });

                //return View(ropeInspectionSetting);
            }

            if (fleets.Count > 0)
                vessels = vessels.Where(p => fleets.Contains(p.FleetNameID)).ToList();

            if (ftypes.Count > 0)
                vessels = vessels.Where(p => ftypes.Contains(p.FleetTypeID)).ToList();

            if (areas.Count > 0)
                vessels = vessels.Where(p => areas.Contains(p.TradeAreaID)).ToList();

            if ((fleets.Count == 0) && (ftypes.Count == 0) && (areas.Count == 0))
                vessels = new List<VesselDetail>();

            List<VesselModel> lstVessels = new List<VesselModel>();

            foreach (var v in vessels)
                lstVessels.Add(new VesselModel { VesselID = v.ImoNo, VesselName = v.VesselName });

            return Json(new { Result = true, Data = lstVessels }, JsonRequestBehavior.AllowGet);
        }

        private bool CheckValidationLead(tblWinchRotationSetting model)
        {
            using (MorringOfficeEntities entities = new MorringOfficeEntities())
            {
                var obj = entities.tblWinchRotationSettings.FirstOrDefault(e => e.MooringRopeType == model.MooringRopeType &&
                    e.Id != model.Id && e.ManufacturerType == model.ManufacturerType);

                return obj == null;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(tblWinchRotationSetting model)
        {
            if (ModelState.IsValid)
            {
                using (MorringOfficeEntities entities = new MorringOfficeEntities())
                {
                   

                    if (!CheckValidation(model))
                    {

                        ModelState.Clear();
                        ModelState.AddModelError("", "Please select different combination");

                        //foreach (var v in entities.tblFleetNames.OrderBy(p => p.Fid).ToList())
                        //    model.FleetNameList.Add(new SelectListItem() { Text = v.FleetName, Value = v.Fid.ToString() });

                        //foreach (var v in entities.tblFleetTypes.OrderBy(p => p.Tid).ToList())
                        //    model.FleetTypeList.Add(new SelectListItem() { Text = v.FleetType, Value = v.Tid.ToString() });

                        foreach (var v in entities.tblCommons.Where(x => x.Type == 2).OrderBy(p => p.Id).ToList())
                            model.FleetNameList.Add(new SelectListItem() { Text = v.Name, Value = v.Id.ToString() });

                        foreach (var v in entities.tblCommons.Where(x => x.Type == 3).OrderBy(p => p.Id).ToList())
                            model.FleetTypeList.Add(new SelectListItem() { Text = v.Name, Value = v.Id.ToString() });

                        foreach (var v in entities.VesselDetails.OrderBy(p => p.ImoNo).ToList())
                            model.VesselList.Add(new SelectListItem() { Text = v.VesselName, Value = v.ImoNo.ToString() });

                        foreach (var v in entities.MooringRopeTypes.OrderBy(p => p.RopeType).ToList())
                            model.MooringRopeTypeLists.Add(new SelectListItem() { Text = v.RopeType, Value = v.Id.ToString() });

                        foreach (var v in entities.tblCommons.Where(u => u.Type == manufacturerID).OrderBy(p => p.Name).ToList())
                            model.ManufacturerTypeLists.Add(new SelectListItem() { Text = v.Name, Value = v.Id.ToString() });


                        return View("Create", model);
                    }


                    foreach (var VesselID in model.VesselIDs)
                    {
                        foreach (var RopetypeID in model.MooringRopeTypeIDs)
                        {
                            foreach (var ManufacturerID in model.ManufacturerTypeIDs)
                            {

                                if (model.Id == 0)
                                {  //---Add
                                    if (model.LeadFrom == model.LeadTo)
                                    {
                                        ModelState.Clear();
                                        ModelState.AddModelError("", "Please select different combination of Lead");

                                        foreach (var v in entities.tblCommons.Where(x => x.Type == 2).OrderBy(p => p.Id).ToList())
                                            model.FleetNameList.Add(new SelectListItem() { Text = v.Name, Value = v.Id.ToString() });

                                        foreach (var v in entities.tblCommons.Where(x => x.Type == 3).OrderBy(p => p.Id).ToList())
                                            model.FleetTypeList.Add(new SelectListItem() { Text = v.Name, Value = v.Id.ToString() });

                                        foreach (var v in entities.VesselDetails.OrderBy(p => p.ImoNo).ToList())
                                            model.VesselList.Add(new SelectListItem() { Text = v.VesselName, Value = v.ImoNo.ToString() });

                                        foreach (var v in entities.MooringRopeTypes.OrderBy(p => p.RopeType).ToList())
                                            model.MooringRopeTypeLists.Add(new SelectListItem() { Text = v.RopeType, Value = v.Id.ToString() });

                                        foreach (var v in entities.tblCommons.Where(u => u.Type == manufacturerID).OrderBy(p => p.Name).ToList())
                                            model.ManufacturerTypeLists.Add(new SelectListItem() { Text = v.Name, Value = v.Id.ToString() });


                                        return View("Create", model);
                                        //break;
                                    }
                                    var obj = new Reports.tblWinchRotationSetting();
                                    obj.MooringRopeType = RopetypeID; //model.MooringRopeType;
                                    obj.ManufacturerType = ManufacturerID; // model.ManufacturerType;
                                    obj.MaximumRunningHours = model.MaximumRunningHours;
                                    obj.MaximumMonthsAllowed = model.MaximumMonthsAllowed;
                                    obj.LeadFrom = model.LeadFrom;
                                    obj.LeadTo = model.LeadTo;
                                    obj.VesselID = VesselID;
                                    entities.tblWinchRotationSettings.Add(obj);
                                }
                                else
                                {   //---Update
                                    if (model.LeadFrom == model.LeadTo)
                                    {
                                        ModelState.Clear();
                                        ModelState.AddModelError("", "Please select different combination of Lead");

                                        foreach (var v in entities.tblCommons.Where(x => x.Type == 2).OrderBy(p => p.Id).ToList())
                                            model.FleetNameList.Add(new SelectListItem() { Text = v.Name, Value = v.Id.ToString() });

                                        foreach (var v in entities.tblCommons.Where(x => x.Type == 3).OrderBy(p => p.Id).ToList())
                                            model.FleetTypeList.Add(new SelectListItem() { Text = v.Name, Value = v.Id.ToString() });

                                        foreach (var v in entities.VesselDetails.OrderBy(p => p.ImoNo).ToList())
                                            model.VesselList.Add(new SelectListItem() { Text = v.VesselName, Value = v.ImoNo.ToString() });

                                        foreach (var v in entities.MooringRopeTypes.OrderBy(p => p.RopeType).ToList())
                                            model.MooringRopeTypeLists.Add(new SelectListItem() { Text = v.RopeType, Value = v.Id.ToString() });

                                        foreach (var v in entities.tblCommons.Where(u => u.Type == manufacturerID).OrderBy(p => p.Name).ToList())
                                            model.ManufacturerTypeLists.Add(new SelectListItem() { Text = v.Name, Value = v.Id.ToString() });


                                        return View("Create", model);
                                    }

                                    var obj = entities.tblWinchRotationSettings.FirstOrDefault(e => e.Id == model.Id);
                                    if (obj != null)
                                    {
                                        obj.MooringRopeType = RopetypeID; //model.MooringRopeType;
                                        obj.ManufacturerType = ManufacturerID; // model.ManufacturerType;
                                        obj.MaximumRunningHours = model.MaximumRunningHours;
                                        obj.MaximumMonthsAllowed = model.MaximumMonthsAllowed;
                                        obj.LeadFrom = model.LeadFrom;
                                        obj.LeadTo = model.LeadTo;
                                        obj.VesselID = VesselID;
                                        entities.Entry(obj).State = EntityState.Modified;
                                    }
                                }

                            }
                        }
                    }
                    
                   

                    try
                    {
                        // entities.SaveChanges();
                        await entities.SaveChangesAsync();


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
                var ropeInspectionSetting = entities.tblWinchRotationSettings.FirstOrDefault(e => e.Id == id);

                //foreach (var v in entities.tblFleetNames.OrderBy(p => p.Fid).ToList())
                //    ropeInspectionSetting.FleetNameList.Add(new SelectListItem() { Text = v.FleetName, Value = v.Fid.ToString() });

                //foreach (var v in entities.tblFleetTypes.OrderBy(p => p.Tid).ToList())
                //    ropeInspectionSetting.FleetTypeList.Add(new SelectListItem() { Text = v.FleetType, Value = v.Tid.ToString() });

                foreach (var v in entities.tblCommons.Where(x => x.Type == 2).OrderBy(p => p.Id).ToList())
                    ropeInspectionSetting.FleetNameList.Add(new SelectListItem() { Text = v.Name, Value = v.Id.ToString() });

                foreach (var v in entities.tblCommons.Where(x => x.Type == 3).OrderBy(p => p.Id).ToList())
                    ropeInspectionSetting.FleetTypeList.Add(new SelectListItem() { Text = v.Name, Value = v.Id.ToString() });

                foreach (var v in entities.VesselDetails.OrderBy(p => p.ImoNo).ToList())
                    ropeInspectionSetting.VesselList.Add(new SelectListItem() { Text = v.VesselName, Value = v.ImoNo.ToString() });


                var Vid = ropeInspectionSetting.VesselID;
                if(Vid != null)
                ropeInspectionSetting.VesselIDs.Add((int)Vid);
                else
                { ropeInspectionSetting.VesselIDs.Add(0); }

               
                foreach (var v in entities.MooringRopeTypes.OrderBy(p => p.RopeType).ToList())
                    ropeInspectionSetting.MooringRopeTypeLists.Add(new SelectListItem() { Text = v.RopeType, Value = v.Id.ToString() });

                ropeInspectionSetting.MooringRopeTypeIDs.Add(ropeInspectionSetting.MooringRopeType);

                foreach (var v in entities.tblCommons.Where(u => u.Type == manufacturerID).OrderBy(p => p.Name).ToList())
                    ropeInspectionSetting.ManufacturerTypeLists.Add(new SelectListItem() { Text = v.Name, Value = v.Id.ToString() });

                return View("Create", ropeInspectionSetting);
            }
        }

        public ActionResult Delete(int id)
        {
            using (MorringOfficeEntities entities = new MorringOfficeEntities())
            {
                var obj = entities.tblWinchRotationSettings.FirstOrDefault(e => e.Id == id);
                obj.MooringRopeTypeName = entities.MooringRopeTypes.First(p => p.Id == obj.MooringRopeType).RopeType;
                obj.ManufacturerTypeName = entities.tblCommons.First(u => u.Id == obj.ManufacturerType).Name;
                return View(obj);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteType(int id)
        {
            using (MorringOfficeEntities entities = new MorringOfficeEntities())
            {
                var obj = entities.tblWinchRotationSettings.FirstOrDefault(e => e.Id == id);
                if (obj != null)
                {
                    entities.tblWinchRotationSettings.Remove(obj);
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