using MenuLayer;
using Reports;
using Shipment49Web.Areas.LooseEquipment.Models;
using Shipment49Web.Areas.MSPS.Models;
using Shipment49Web.Common;
using Shipment49Web.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Shipment49Web.Areas.LooseEquipment.Controllers
{
    [Authorize]
    [UserAuthenticationFilter]
    [ErrorClass]
    public class LEDetailsController : BaseController
    {
        // GET: LooseEquipment/LEDetails 
        SqlConnection con = ConnectionBulder.con;
        MorringOfficeEntities context = new MorringOfficeEntities();
       
       
        public ActionResult Index()
        {
           int VesselID = Convert.ToInt32(Session["VesselSessionID"].ToString());
            LooseEquipmentDetails AllEq = new LooseEquipmentDetails();
            using (ShipmentContaxt sc1 = new ShipmentContaxt())
            {
                //JoiningShackle 
                var JShackle = new ShipmentContaxt()
                     .MultipleResults("[dbo].[LooseEquipmentTypeDetails] 'JShackle','" + VesselID + "'")
                   .With<JoiningShackle>().Execute();

                AllEq.JoiningShackleList = (List<JoiningShackle>)JShackle[0];

                //RopeStopper 
                var RTail = new ShipmentContaxt()
                    .MultipleResults("[dbo].[LooseEquipmentTypeDetails] 'RTail','" + VesselID + "'").With<RopeTail>().Execute();
                AllEq.RopeStopperList = (List<RopeTail>)RTail[0];

                //TowingRope 
                var TowingRope = new ShipmentContaxt()
                   .MultipleResults("[dbo].[LooseEquipmentTypeDetails] 'TowingRope','" + VesselID + "'").With<RopeTail>().Execute();
                AllEq.TowingRopeList = (List<RopeTail>)TowingRope[0];

                //SuezRope 
                var SuezRope = new ShipmentContaxt()
                   .MultipleResults("[dbo].[LooseEquipmentTypeDetails] 'SuezRope','" + VesselID + "'").With<RopeTail>().Execute();
                AllEq.SuezRopeRopeList = (List<RopeTail>)SuezRope[0];

                //MessengerRope
                var MRope = new ShipmentContaxt()
                  .MultipleResults("[dbo].[LooseEquipmentTypeDetails] 'MRope','" + VesselID + "'").With<RopeTail>().Execute();
                AllEq.MessengerRopeList = (List<RopeTail>)MRope[0];

                //FireWire
                var FireWire = new ShipmentContaxt()
                  .MultipleResults("[dbo].[LooseEquipmentTypeDetails] 'FireWire','" + VesselID + "'").With<RopeTail>().Execute();
                AllEq.FireWireList = (List<RopeTail>)FireWire[0];
               
                //ChainStopper
                var CStopper = new ShipmentContaxt()
                 .MultipleResults("[dbo].[LooseEquipmentTypeDetails] 'CStopper','" + VesselID + "'").With<ChainStopper>().Execute();
                AllEq.ChainStopperList = (List<ChainStopper>)CStopper[0];

                //ChafeGuard
                var ChafeGuard = new ShipmentContaxt()
                 .MultipleResults("[dbo].[LooseEquipmentTypeDetails] 'ChafeGuard','" + VesselID + "'").With<ChafeGuard>().Execute();
                AllEq.ChafeGuardList = (List<ChafeGuard>)ChafeGuard[0];

                //WinchBreakTestKit
                var WinchBreakTestKit = new ShipmentContaxt()
                 .MultipleResults("[dbo].[LooseEquipmentTypeDetails] 'WinchBreakTestKit','" + VesselID + "'").With<WinchBreakTestKit>().Execute();
                AllEq.WinchBreakTestKitList = (List<WinchBreakTestKit>)WinchBreakTestKit[0];

               
                return View(AllEq);
            }

        }

        public string GetLooseEquipmentType(int Ltp)
        {
            return context.LooseETypes.Where(t => t.Id == Ltp).Select(s => s.LooseEquipmentType).SingleOrDefault();

        }
        public ActionResult LERopeTail(int LTp)
        {
            RopeTail svropetail = new RopeTail();
            svropetail.LooseETypeId = LTp;
            ViewBag.LooseEqType = GetLooseEquipmentType(LTp);
            ViewBag.LooseEqTypeID = LTp;
            return View(svropetail);
            //return View();
        }

        [HttpPost]
        [PreventSpam("LERopeTail", 3, 1)]
        public ActionResult LERopeTail(RopeTail svropetail)
        {
            //Add
            int VesselID = Convert.ToInt32(Session["VesselSessionID"].ToString());
            if (svropetail.Id == 0)
            {
                var duplicatecheck = context.RopeTails.Where(x => x.VesselID == VesselID && x.UniqueID == svropetail.UniqueID && x.LooseETypeId == svropetail.LooseETypeId).FirstOrDefault();
                if (duplicatecheck == null)
                {

                    var IdPK = ((from asn in context.RopeTails.Where(x => x.VesselID == VesselID) select (int?)asn.Id).Max() ?? 0) + 1;


                    svropetail.Id = IdPK;
                    svropetail.VesselID = VesselID;
                    // svropetail.LooseETypeId = 4;
                    svropetail.RopeConstruction = svropetail.RopeConstruction;


                    //svropetail.DiaMeter = textinfo.ToTitleCase(svropetail.DiaMeter.ToLower());

                    //svropetail.Diameter = Convert.ToDecimal(svropetail.DiaMeter1);
                    //svropetail.Length = Convert.ToDecimal(svropetail.Length);
                    //svropetail.MBL = Convert.ToDecimal(svropetail.MBL);
                    //svropetail.LDBF = Convert.ToDecimal(svropetail.LDBF);
                    //svropetail.WLL = Convert.ToDecimal(svropetail.WLL);

                    svropetail.ManufactureName = svropetail.ManufactureName;
                    svropetail.CertificateNumber = svropetail.CertificateNumber;
                    svropetail.UniqueID = svropetail.UniqueID;
                    svropetail.ReceivedDate = Convert.ToDateTime(svropetail.ReceivedDate);


                    if (svropetail.IsRopeInstalled == "No")
                    {
                        svropetail.InstalledDate = null;
                        if (svropetail.ReceivedDate != null)
                        {
                            svropetail.InspectionDueDate = CommonClass.LEqInspectionDate(Convert.ToDateTime(svropetail.ReceivedDate), Convert.ToInt32(svropetail.LooseETypeId));

                        }

                    }
                    else
                    {
                        if (svropetail.InstalledDate != null)
                        {
                            svropetail.InspectionDueDate = CommonClass.LEqInspectionDate(Convert.ToDateTime(svropetail.InstalledDate), Convert.ToInt32(svropetail.LooseETypeId));

                        }

                    }


                    svropetail.DeleteStatus = false;
                    svropetail.RopeTagging = svropetail.RopeTagging;
                    //svropetail.OutofServiceDate = Convert.ToDateTime(svropetail.InstalledDate);
                    svropetail.OutofServiceDate = null;
                    //svropetail.ReasonOutofService = textinfo.ToTitleCase(svropetail.ReasonOutofService.ToLower());

                    if (svropetail.OtherReason != null)
                    {
                        svropetail.OtherReason = svropetail.OtherReason;
                    }
                    if (svropetail.DamageObserved != null)
                    {
                        svropetail.DamageObserved = svropetail.DamageObserved;
                    }

                    svropetail.CreatedDate = DateTime.Now;
                    svropetail.CreatedBy = "Admin";
                    svropetail.IsActive = true;





                    context.RopeTails.Add(svropetail);
                    context.SaveChanges();


                }

                else
                {
                    TempData["Error"] = "UniqueID already exists !";
                    svropetail.UniqueID = null;
                    return View(svropetail);
                }
            }
            else
            {
                var duplicatecheck = context.RopeTails.Where(x => x.VesselID == VesselID && x.UniqueID == svropetail.UniqueID && x.LooseETypeId == svropetail.LooseETypeId).FirstOrDefault();

                if (duplicatecheck != null)
                {
                    context.Entry(duplicatecheck).State = EntityState.Detached;
                }

                if (svropetail.IsRopeInstalled == "No")
                {
                    svropetail.InstalledDate = null;
                    if (svropetail.ReceivedDate != null)
                    {
                        svropetail.InspectionDueDate = CommonClass.LEqInspectionDate(Convert.ToDateTime(svropetail.ReceivedDate), Convert.ToInt32(duplicatecheck.LooseETypeId));

                    }
                }
                else
                {
                    // duplicatecheck.InstalledDate = svropetail.InstalledDate;
                    if (svropetail.InstalledDate != null)
                    {
                        svropetail.InspectionDueDate = CommonClass.LEqInspectionDate(Convert.ToDateTime(svropetail.InstalledDate), Convert.ToInt32(duplicatecheck.LooseETypeId));

                    }

                }

                var UpdatedRopedetails = new RopeTail()
                {
                    VesselID = duplicatecheck.VesselID,
                    Id = duplicatecheck.Id,
                    LooseETypeId = duplicatecheck.LooseETypeId,
                    Length = svropetail.Length,
                    Diameter = svropetail.Diameter,
                    WLL = svropetail.WLL,
                    LDBF = svropetail.LDBF,
                    MBL = svropetail.MBL,
                    ReasonOutofService = svropetail.ReasonOutofService,
                    //OutofServiceDate = rptl.OutofServiceDate,
                    DamageObserved = svropetail.DamageObserved,
                    InstalledDate = svropetail.InstalledDate,
                    InspectionDueDate = svropetail.InspectionDueDate,
                    MOpId = svropetail.MOpId,
                    ModifiedDate = DateTime.Now,
                    IsActive = true,
                    DeleteStatus = false,
                    RopeConstruction = svropetail.RopeConstruction,
                    RopeTagging = svropetail.RopeTagging,
                    ReceivedDate = svropetail.ReceivedDate,
                    CertificateNumber = svropetail.CertificateNumber,
                    ManufactureName = svropetail.ManufactureName,
                    Remarks = svropetail.Remarks,
                    UniqueID = duplicatecheck.UniqueID,
                    CreatedBy = "Admin",
                    CreatedDate = DateTime.Now,
                };

                context.Entry(UpdatedRopedetails).State = EntityState.Modified;
                context.SaveChanges();


            }

            TempData["Success"] = "Record successfully saved !";
            return RedirectToAction("Index");
        }

        public ActionResult createjs()
        {
            var jskl = new JoiningShackle() { LooseETypeId = 1 };
            return View(jskl);
        }

        [HttpPost]
        [PreventSpam("createjs", 3, 1)]
        public ActionResult createjs(JoiningShackle joiningShackle)
        {

            int VesselID = Convert.ToInt32(Session["VesselSessionID"].ToString());

            if (ModelState.IsValid)
            {

                //---Add
                if (joiningShackle.Id == 0)
                {


                    var result = context.JoiningShackles.SingleOrDefault(b => b.UniqueID == joiningShackle.UniqueID && b.DeleteStatus == false && b.VesselID == VesselID);
                    if (result == null)
                    {
                        var IdPK = ((from asn in context.JoiningShackles.Where(x => x.VesselID == VesselID) select (int?)asn.Id).Max() ?? 0) + 1;

                        joiningShackle.Id = IdPK;
                        joiningShackle.VesselID = VesselID;
                        joiningShackle.LooseETypeId = 1;
                        joiningShackle.IdentificationNumber = joiningShackle.UniqueID;


                        //joiningShackle.MBL = Convert.ToDecimal(joiningShackle.MBL);
                        //joiningShackle.Type = joiningShackle.Type;
                        //joiningShackle.CertificateNumber = joiningShackle.CertificateNumber;
                        //joiningShackle.UniqueID = joiningShackle.UniqueID;
                        //joiningShackle.DateReceived = Convert.ToDateTime(joiningShackle.DateReceived);

                        // joiningShackle.DateInstalled = Convert.ToDateTime(joiningShackle.DateInstalled);

                        if (joiningShackle.IsInstalled == "No")
                        {
                            joiningShackle.DateInstalled = null;
                            if (joiningShackle.DateReceived != null)
                            {
                                joiningShackle.InspectionDueDate = CommonClass.LEqInspectionDate(Convert.ToDateTime(joiningShackle.DateReceived), Convert.ToInt32(joiningShackle.LooseETypeId));

                            }
                        }
                        else
                        {
                            if (joiningShackle.DateInstalled != null)
                            {
                                joiningShackle.InspectionDueDate = CommonClass.LEqInspectionDate(Convert.ToDateTime(joiningShackle.DateInstalled), Convert.ToInt32(joiningShackle.LooseETypeId));

                            }

                        }


                        joiningShackle.OutofServiceDate = null;
                        joiningShackle.CreatedDate = DateTime.Now;
                        joiningShackle.CreatedBy = "Admin";
                        joiningShackle.IsActive = true;
                        joiningShackle.DeleteStatus = false;

                        context.JoiningShackles.Add(joiningShackle);
                        context.SaveChanges();
                    }
                    else
                    {
                        TempData["Error"] = "UniqueID already exists !";
                        joiningShackle.UniqueID = null;
                        return View(joiningShackle);
                    }
                }
                else
                {// Update


                    var result = context.JoiningShackles.SingleOrDefault(b => b.UniqueID == joiningShackle.UniqueID && b.DeleteStatus == false && b.VesselID == VesselID);

                    try
                    {

                        //joiningShackle.Id = IdPK;
                        // joiningShackle.VesselID = vesselid;
                        //result.LooseETypeId = 1;
                        result.IdentificationNumber = joiningShackle.UniqueID;
                        result.ManufactureName = joiningShackle.ManufactureName;
                        result.MBL = joiningShackle.MBL;
                        result.Type = joiningShackle.Type;
                        result.CertificateNumber = joiningShackle.CertificateNumber;
                        result.Remarks = joiningShackle.Remarks;

                        if (joiningShackle.IsInstalled == "No")
                        {
                            result.DateInstalled = null;
                            if (joiningShackle.DateReceived != null)
                            {
                                result.InspectionDueDate = CommonClass.LEqInspectionDate(Convert.ToDateTime(joiningShackle.DateReceived), Convert.ToInt32(result.LooseETypeId));

                            }
                        }
                        else
                        {
                            result.DateInstalled = joiningShackle.DateInstalled;
                            if (result.DateInstalled != null)
                            {
                                result.InspectionDueDate = CommonClass.LEqInspectionDate(Convert.ToDateTime(joiningShackle.DateInstalled), Convert.ToInt32(result.LooseETypeId));

                            }
                        }






                        result.OutofServiceDate = null;
                        result.ModifiedDate = DateTime.Now;
                        result.ModifiedBy = "Admin";
                        result.IsActive = true;
                        result.DeleteStatus = false;



                    }
                    catch (Exception ex) { }

                    context.Entry(result).State = EntityState.Modified;
                    context.SaveChanges();
                }

                TempData["Success"] = "Record successfully saved !";
                return RedirectToAction("Index");
            }
            else
             return View(joiningShackle);
        }

        public ActionResult ChainStopperadd()
        {
            var jskl = new ChainStopper() { LooseETypeId = 5 };
            return View(jskl);
        }

        [HttpPost]
        [PreventSpam("ChainStopperadd", 3, 1)]
        public ActionResult ChainStopperadd(ChainStopper svchainst)
        {
            //---Add
            int VesselID = Convert.ToInt32(Session["VesselSessionID"].ToString());
            if (svchainst.Id == 0)
            {


                var result = context.ChainStoppers.SingleOrDefault(b => b.UniqueID == svchainst.UniqueID && b.DeleteStatus == false && b.VesselID == VesselID);
                if (result == null)
                {
                    var IdPK = ((from asn in context.ChainStoppers.Where(x => x.VesselID == VesselID) select (int?)asn.Id).Max() ?? 0) + 1;

                    svchainst.Id = IdPK;
                    svchainst.VesselID = VesselID;
                    svchainst.LooseETypeId = 5;
                    svchainst.Length = Convert.ToDecimal(svchainst.Length);
                    svchainst.MBL = Convert.ToDecimal(svchainst.MBL);
                    svchainst.ManufactureName = svchainst.ManufactureName;
                    svchainst.CertificateNumber = svchainst.CertificateNumber;
                    svchainst.UniqueID = svchainst.UniqueID;
                    svchainst.DateReceived = Convert.ToDateTime(svchainst.DateReceived);
                    //svchainst.DateInstalled = Convert.ToDateTime(svchainst.DateInstalled);


                    if (svchainst.IsRopeInstalled == "No")
                    {
                        svchainst.DateInstalled = null;
                        if (svchainst.DateReceived != null)
                        {
                            svchainst.InspectionDueDate = CommonClass.LEqInspectionDate(Convert.ToDateTime(svchainst.DateReceived), Convert.ToInt32(svchainst.LooseETypeId));

                        }
                    }
                    else
                    {
                        if (svchainst.DateInstalled != null)
                        {
                            svchainst.InspectionDueDate = CommonClass.LEqInspectionDate(Convert.ToDateTime(svchainst.DateInstalled), Convert.ToInt32(svchainst.LooseETypeId));

                        }

                    }


                    svchainst.OutofServiceDate = null;
                    svchainst.CreatedDate = DateTime.Now;
                    svchainst.CreatedBy = "Admin";
                    svchainst.IsActive = true;
                    svchainst.DeleteStatus = false;



                    context.ChainStoppers.Add(svchainst);
                    context.SaveChanges();


                }

                else
                {
                    TempData["Error"] = "UniqueID already exists !";
                    svchainst.UniqueID = null;
                    return View(svchainst);
                }
            }
            else
            {
                // update

                var result = context.ChainStoppers.SingleOrDefault(b => b.UniqueID == svchainst.UniqueID && b.DeleteStatus == false && b.VesselID == VesselID);

                //result.Id = IdPK;
                //result.VesselID = VesselID;
                //result.LooseETypeId = 5;
                //result.UniqueID = svchainst.UniqueID;
                result.Length = Convert.ToDecimal(svchainst.Length);
                result.MBL = Convert.ToDecimal(svchainst.MBL);
                result.ManufactureName = svchainst.ManufactureName;
                result.CertificateNumber = svchainst.CertificateNumber;
                result.DateReceived = Convert.ToDateTime(svchainst.DateReceived);
                result.Remarks = svchainst.Remarks;

                if (svchainst.IsRopeInstalled == "No")
                {
                    result.DateInstalled = null;
                    if (svchainst.DateReceived != null)
                    {
                        result.InspectionDueDate = CommonClass.LEqInspectionDate(Convert.ToDateTime(svchainst.DateReceived), Convert.ToInt32(result.LooseETypeId));

                    }
                }
                else
                {
                    result.DateInstalled = svchainst.DateInstalled;
                    if (result.DateInstalled != null)
                    {
                        result.InspectionDueDate = CommonClass.LEqInspectionDate(Convert.ToDateTime(svchainst.DateInstalled), Convert.ToInt32(result.LooseETypeId));

                    }
                }

                result.OutofServiceDate = null;
                result.ModifiedDate = DateTime.Now;
                result.ModifiedBy = "Admin";
                result.IsActive = true;
                result.DeleteStatus = false;



                context.Entry(result).State = EntityState.Modified;
                context.SaveChanges();
            }

            TempData["Success"] = "Record successfully saved !";
            return RedirectToAction("Index");
        }

        public ActionResult ChafeGuardadd()
        {
            var jskl = new ChafeGuard() { LooseETypeId = 7 };
            return View(jskl);
        }

        [HttpPost]
        [PreventSpam("ChafeGuardadd", 3, 1)]
        public ActionResult ChafeGuardadd(ChafeGuard svchainst)
        {
            int VesselID = Convert.ToInt32(Session["VesselSessionID"].ToString());
            var result = context.ChafeGuards.SingleOrDefault(b => b.UniqueID == svchainst.UniqueID && b.DeleteStatus == false && b.VesselID == VesselID);
            if (result == null)
            {
                var IdPK = ((from asn in context.ChafeGuards.Where(x => x.VesselID == VesselID) select (int?)asn.Id).Max() ?? 0) + 1;

                svchainst.Id = IdPK;
                svchainst.VesselID = VesselID;
                svchainst.LooseETypeId = 7;
                svchainst.ManufacturerName = svchainst.ManufacturerName;
                svchainst.CertificateNumber = svchainst.CertificateNumber;
                svchainst.UniqueID = svchainst.UniqueID;
                svchainst.ReceivedDate = Convert.ToDateTime(svchainst.ReceivedDate);
               
                if (svchainst.IsRopeInstalled == "No")
                {
                    svchainst.InstalledDate = null;
                    if (svchainst.ReceivedDate != null)
                    {
                        svchainst.InspectionDueDate = CommonClass.LEqInspectionDate(Convert.ToDateTime(svchainst.ReceivedDate), Convert.ToInt32(svchainst.LooseETypeId));

                    }
                }
                else
                {
                    if (svchainst.InstalledDate != null)
                    {
                        svchainst.InspectionDueDate = CommonClass.LEqInspectionDate(Convert.ToDateTime(svchainst.InstalledDate), Convert.ToInt32(svchainst.LooseETypeId));

                    }

                }


                svchainst.OutofServiceDate = null;
                svchainst.CreatedDate = DateTime.Now;
                svchainst.IsActive = true;
                svchainst.DeleteStatus = false;



                context.ChafeGuards.Add(svchainst);
                context.SaveChanges();


            }
            else
            {
                // update

                               
                result.ManufacturerName = svchainst.ManufacturerName;
                result.CertificateNumber = svchainst.CertificateNumber;
                result.Remarks = svchainst.Remarks;
                result.ReceivedDate = Convert.ToDateTime(svchainst.ReceivedDate);


                if (svchainst.IsRopeInstalled == "No")
                {
                    result.InstalledDate = null;
                    if (svchainst.ReceivedDate != null)
                    {
                        result.InspectionDueDate = CommonClass.LEqInspectionDate(Convert.ToDateTime(svchainst.ReceivedDate), Convert.ToInt32(svchainst.LooseETypeId));

                    }
                }
                else
                {
                    result.InstalledDate = svchainst.InstalledDate;
                    if (result.InstalledDate != null)
                    {
                        result.InspectionDueDate = CommonClass.LEqInspectionDate(Convert.ToDateTime(svchainst.InstalledDate), Convert.ToInt32(result.LooseETypeId));

                    }
                }

                result.OutofServiceDate = null;
                result.IsActive = true;
                result.DeleteStatus = false;



                context.Entry(result).State = EntityState.Modified;
                context.SaveChanges();
            }

            TempData["Success"] = "Record successfully saved !";
            return RedirectToAction("Index");

          
        }

        public ActionResult WBTKadd()
        {
            var jskl = new WinchBreakTestKit() { LooseETypeId = 8 };
            return View(jskl);
        }

        [HttpPost]
        [PreventSpam("WBTKadd", 3, 1)]
        public ActionResult WBTKadd(WinchBreakTestKit WBTK)
        { //---Add
            int VesselID = Convert.ToInt32(Session["VesselSessionID"].ToString());
            if (WBTK.Id == 0)
            {

                var result = context.WinchBreakTestKits.SingleOrDefault(b => b.UniqueID == WBTK.UniqueID && b.DeleteStatus == false && b.VesselID == VesselID);
                if (result == null)
                {
                    var IdPK = ((from asn in context.WinchBreakTestKits.Where(x => x.VesselID == VesselID) select (int?)asn.Id).Max() ?? 0) + 1;

                    WBTK.Id = IdPK;
                    WBTK.VesselID = VesselID;
                    WBTK.LooseETypeId = 8;
                    //WBTK.UniqueID = svchainst.UniqueID;
                    WBTK.ReceivedDate = Convert.ToDateTime(WBTK.ReceivedDate);
                    WBTK.Remarks = WBTK.Remarks;

                    if (WBTK.IsRopeInstalled == "No")
                    {
                        WBTK.InstalledDate = null;
                       
                        if (WBTK.ReceivedDate != null)
                        {
                            WBTK.InspectionDueDate = CommonClass.LEqInspectionDate(Convert.ToDateTime(WBTK.ReceivedDate), Convert.ToInt32(WBTK.LooseETypeId));

                        }
                    }
                    else
                    {
                        if (WBTK.InstalledDate != null)
                        {
                            WBTK.InspectionDueDate = CommonClass.LEqInspectionDate(Convert.ToDateTime(WBTK.InstalledDate), Convert.ToInt32(WBTK.LooseETypeId));

                        }

                    }


                    WBTK.OutofServiceDate = null;
                    WBTK.CreatedDate = DateTime.Now;
                    WBTK.IsActive = true;
                    WBTK.DeleteStatus = false;



                    context.WinchBreakTestKits.Add(WBTK);
                    context.SaveChanges();


                }

                else
                {
                    TempData["Error"] = "UniqueID already exists !";
                    WBTK.UniqueID = null;
                    return View(WBTK);
                }
            }
            else
            {
                // update

                var result = context.WinchBreakTestKits.SingleOrDefault(b => b.UniqueID == WBTK.UniqueID && b.DeleteStatus == false && b.VesselID == VesselID);

                result.ManufacturerName = WBTK.ManufacturerName;
                result.CertificateNumber = WBTK.CertificateNumber;
                result.ReceivedDate = Convert.ToDateTime(WBTK.ReceivedDate);
                result.Remarks = WBTK.Remarks;

                if (WBTK.IsRopeInstalled == "No")
                {
                    result.InstalledDate = null;
                    if (WBTK.ReceivedDate != null)
                    {
                        result.InspectionDueDate = CommonClass.LEqInspectionDate(Convert.ToDateTime(WBTK.ReceivedDate), Convert.ToInt32(WBTK.LooseETypeId));

                    }
                }
                else
                {
                    result.InstalledDate = WBTK.InstalledDate;
                    if (result.InstalledDate != null)
                    {
                        result.InspectionDueDate = CommonClass.LEqInspectionDate(Convert.ToDateTime(WBTK.InstalledDate), Convert.ToInt32(WBTK.LooseETypeId));

                    }
                }

                result.OutofServiceDate = null;
                result.IsActive = true;
                result.DeleteStatus = false;

                context.Entry(result).State = EntityState.Modified;
                context.SaveChanges();
            }

            TempData["Success"] = "Record successfully saved !";
            return RedirectToAction("Index");
        }

       
        public ActionResult Edit(int id, int LTp)
        {
            int VesselID = Convert.ToInt32(Session["VesselSessionID"].ToString());
            if (LTp == 1)
            {
                using (MorringOfficeEntities entities = new MorringOfficeEntities())
                {
                   // int vesselid = Convert.ToInt32(CommonClass.VesselSessionID);
                    var joiningShackle = entities.JoiningShackles.FirstOrDefault(e => e.Id == id && e.VesselID == VesselID);

                    if (joiningShackle.DateInstalled != null)
                    {
                        joiningShackle.IsInstalled = "Yes";

                    }
                    else
                    {
                        joiningShackle.IsInstalled = "No";
                    }

                    return View("createjs", joiningShackle);
                }
            }
            else if (LTp == 5)
            {
                using (MorringOfficeEntities entities = new MorringOfficeEntities())
                {
                   // int vesselid = Convert.ToInt32(CommonClass.VesselSessionID);
                    var ChainStopperss = entities.ChainStoppers.Where(e => e.Id == id && e.VesselID == VesselID).FirstOrDefault();
                    ChainStopperss.LooseETypeId = 5;
                    if (ChainStopperss.DateInstalled != null)
                    {
                        ChainStopperss.IsRopeInstalled = "Yes";

                    }
                    else
                    {
                        ChainStopperss.IsRopeInstalled = "No";
                    }

                    return View("ChainStopperadd", ChainStopperss);
                }
            }
            else if (LTp == 7)
            {
                using (MorringOfficeEntities entities = new MorringOfficeEntities())
                {
                   // int vesselid = Convert.ToInt32(CommonClass.VesselSessionID);
                    var ChafeGuard = entities.ChafeGuards.FirstOrDefault(e => e.Id == id && e.VesselID == VesselID);

                    if (ChafeGuard.InstalledDate != null)
                    {
                        ChafeGuard.IsRopeInstalled = "Yes";

                    }
                    else
                    {
                        ChafeGuard.IsRopeInstalled = "No";
                    }

                   
                    return View("ChafeGuardadd", ChafeGuard);
                }
            }
            else if (LTp == 8)
            {
                using (MorringOfficeEntities entities = new MorringOfficeEntities())
                {
                  
                    var WinchBreakKit = entities.WinchBreakTestKits.FirstOrDefault(e => e.Id == id && e.VesselID == VesselID);

                    if (WinchBreakKit.InstalledDate != null)
                    {
                        WinchBreakKit.IsRopeInstalled = "Yes";

                    }
                    else
                    {
                        WinchBreakKit.IsRopeInstalled = "No";
                    }

                    return View("WBTKadd", WinchBreakKit);
                }
            }
            else
            {
                RopeTail RopeTailss;
                using (MorringOfficeEntities entities = new MorringOfficeEntities())
                {
                    // int vesselid = Convert.ToInt32(CommonClass.VesselSessionID);
                    RopeTailss = entities.RopeTails.FirstOrDefault(e => e.Id == id && e.LooseETypeId == LTp && e.VesselID == VesselID);

                    if (RopeTailss.InstalledDate != null)
                    {
                        RopeTailss.IsRopeInstalled = "Yes";

                    }
                    else
                    {
                        RopeTailss.IsRopeInstalled = "No";
                    }



                }
                ViewBag.LooseEqType = GetLooseEquipmentType(Convert.ToInt32(RopeTailss.LooseETypeId));
                return View("LERopeTail", RopeTailss);
              
            }
        }

       
        public ActionResult delete(int id, int LTp)
        {

            // int vesselid = Convert.ToInt32(CommonClass.VesselSessionID);
            // int ropeid = context.JoiningShackles.Where(x => x.Id == id && x.VesselID == vesselid).Select(x => x.RopeId).SingleOrDefault();
            int VesselID = Convert.ToInt32(Session["VesselSessionID"].ToString());
            try
            {
                if (LTp == 1)
                {
                    using (SqlDataAdapter adp1 = new SqlDataAdapter("update JoiningShackle set IsActive=0, DeleteStatus=1 where  Id=" + id + " and VesselID= " + VesselID + "", con))
                    {
                        DataTable dt1 = new DataTable();
                        adp1.Fill(dt1);
                    }
                }
                else if (LTp == 5)
                {
                    using (SqlDataAdapter adp1 = new SqlDataAdapter("update ChainStopper set IsActive=0, DeleteStatus=1 where  Id=" + id + " and VesselID= " + VesselID + "", con))
                    {
                        DataTable dt1 = new DataTable();
                        adp1.Fill(dt1);
                    }
                }
                else if (LTp == 7)
                {
                    using (SqlDataAdapter adp1 = new SqlDataAdapter("update ChafeGuard set IsActive=0, DeleteStatus=1 where  Id=" + id + " and VesselID= " + VesselID + "", con))
                    {
                        DataTable dt1 = new DataTable();
                        adp1.Fill(dt1);
                    }
                }
                else if (LTp == 8)
                {
                    using (SqlDataAdapter adp1 = new SqlDataAdapter("update WinchBreakTestKit set IsActive=0, DeleteStatus=1 where  Id=" + id + " and VesselID= " + VesselID + "", con))
                    {
                        DataTable dt1 = new DataTable();
                        adp1.Fill(dt1);
                    }
                }
                else //if (LTp == 4)
                {
                    using (SqlDataAdapter adp1 = new SqlDataAdapter("update RopeTail set IsActive=0, DeleteStatus=1 where LooseETypeId= " + LTp + " and Id=" + id + " and VesselID= " + VesselID + "", con))
                    {
                        DataTable dt1 = new DataTable();
                        adp1.Fill(dt1);
                    }
                }

                //****** Delete LE Notification
                using (SqlDataAdapter adp1 = new SqlDataAdapter("Delete from tblNotification where VesselId=" + VesselID + " and LooseCertificateNum=" + id + " and LooseEqType=" + LTp + "", con))
                {
                    using (DataTable dt1 = new DataTable())
                    {
                        adp1.Fill(dt1);
                    }
                }
                //****** Delete LE Notification
            }
            catch (Exception ex)
            {
            }
            TempData["Success"] = "Record deleted successfully ";


            return RedirectToAction("Index");
        }
    }
}