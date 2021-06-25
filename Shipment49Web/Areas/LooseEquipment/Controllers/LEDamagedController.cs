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
    public class LEDamagedController : BaseController
    {
        // GET: LooseEquipment/LEDamaged
        SqlConnection con = ConnectionBulder.con;
        MorringOfficeEntities context = new MorringOfficeEntities();
       // public static int VesselID { get; set; }
        public LEDamagedController()
        {
          //  VesselID = Convert.ToInt32(CommonClass.VesselSessionID);
        }
        public ActionResult Index()
        {
           int VesselID = Convert.ToInt32(Session["VesselSessionID"].ToString());
            
            LooseEDamageRecord list = new LooseEDamageRecord();
            using (ShipmentContaxt sc1 = new ShipmentContaxt())
            {

                var LEDamageRecord = new ShipmentContaxt()
                     .MultipleResults("[dbo].[GetDamageLooseE] '" + VesselID + "'")
                   .With<LooseEDamageRecord>().Execute();

                list.AllLEDamagedList = (List<LooseEDamageRecord>)LEDamageRecord[0];
            }
            return View(list);
        }

        public ActionResult DiscardIndex()
        {
            var VesselID = Session["VesselSessionID"].ToString();
            LooseEquipmentDetails AllEq = new LooseEquipmentDetails();
            using (ShipmentContaxt sc1 = new ShipmentContaxt())
            {
                //JoiningShackle 
                var JShackle = new ShipmentContaxt()
                     .MultipleResults("[dbo].[LooseEquipmentTypeDiscardDetails] 'JShackle','" + VesselID + "'")
                   .With<JoiningShackle>().Execute();

                AllEq.JoiningShackleList = (List<JoiningShackle>)JShackle[0];

                //RopeStopper 
                var RTail = new ShipmentContaxt()
                    .MultipleResults("[dbo].[LooseEquipmentTypeDiscardDetails] 'RTail','" + VesselID + "'").With<RopeTail>().Execute();
                AllEq.RopeStopperList = (List<RopeTail>)RTail[0];

                //TowingRope 
                var TowingRope = new ShipmentContaxt()
                   .MultipleResults("[dbo].[LooseEquipmentTypeDiscardDetails] 'TowingRope','" + VesselID + "'").With<RopeTail>().Execute();
                AllEq.TowingRopeList = (List<RopeTail>)TowingRope[0];

                //SuezRope 
                var SuezRope = new ShipmentContaxt()
                   .MultipleResults("[dbo].[LooseEquipmentTypeDiscardDetails] 'SuezRope','" + VesselID + "'").With<RopeTail>().Execute();
                AllEq.SuezRopeRopeList = (List<RopeTail>)SuezRope[0];

                //MessengerRope
                var MRope = new ShipmentContaxt()
                  .MultipleResults("[dbo].[LooseEquipmentTypeDiscardDetails] 'MRope','" + VesselID + "'").With<RopeTail>().Execute();
                AllEq.MessengerRopeList = (List<RopeTail>)MRope[0];

                //FireWire
                var FireWire = new ShipmentContaxt()
                  .MultipleResults("[dbo].[LooseEquipmentTypeDiscardDetails] 'FireWire','" + VesselID + "'").With<RopeTail>().Execute();
                AllEq.FireWireList = (List<RopeTail>)FireWire[0];

                //ChainStopper
                var CStopper = new ShipmentContaxt()
                 .MultipleResults("[dbo].[LooseEquipmentTypeDiscardDetails] 'CStopper','" + VesselID + "'").With<ChainStopper>().Execute();
                AllEq.ChainStopperList = (List<ChainStopper>)CStopper[0];

                //ChafeGuard
                var ChafeGuard = new ShipmentContaxt()
                 .MultipleResults("[dbo].[LooseEquipmentTypeDiscardDetails] 'ChafeGuard','" + VesselID + "'").With<ChafeGuard>().Execute();
                AllEq.ChafeGuardList = (List<ChafeGuard>)ChafeGuard[0];

                //WinchBreakTestKit
                var WinchBreakTestKit = new ShipmentContaxt()
                 .MultipleResults("[dbo].[LooseEquipmentTypeDiscardDetails] 'WinchBreakTestKit','" + VesselID + "'").With<WinchBreakTestKit>().Execute();
                AllEq.WinchBreakTestKitList = (List<WinchBreakTestKit>)WinchBreakTestKit[0];


                return View(AllEq);
            }

        }

        public ActionResult AddDiscard(int LET)
        {
            int VesselID = Convert.ToInt32(Session["VesselSessionID"].ToString());
            LooseEquipmentDiscard obj = new LooseEquipmentDiscard() { LooseETypeId = LET };
            ViewBag.LooseEquipmentType = context.LooseETypes.SingleOrDefault(x => x.Id == LET).LooseEquipmentType;
            obj.GetAllLE_Detail = GetLooseEquipments(LET);
            obj.ReasonOutofServices = CommonClass.OutofServiceList();
            obj.DamageObservedLists = CommonClass.DamagObservedList();
            obj.MooringOperationsLists = CommonClass.MooringOpListCommon(VesselID);
            return View(obj);
        }
        [HttpPost]
        [PreventSpam("AddDiscardLE", 3, 1)]
        public ActionResult AddDiscard(LooseEquipmentDiscard rpspc)
        {
            try
            {
                int VesselID = Convert.ToInt32(Session["VesselSessionID"].ToString());
                ViewBag.LooseEquipmentType = context.LooseETypes.SingleOrDefault(x => x.Id == rpspc.LooseETypeId).LooseEquipmentType;

                if (rpspc.LooseETypeId == 1)
                {
                    string qry = "update JoiningShackle  set OtherReason=@OtherReason,DamagedObserved=@DamagedObserved , OutofServiceDate=@OutofServiceDate, ReasonOutofService=@ReasonOutofService ,MOpId=@MOpId where  Id=@Id and VesselID=@VesselID";
                    using (SqlDataAdapter adp1 = new SqlDataAdapter(qry, con))
                    {
                        if (rpspc.OtherReason == null)
                        {
                            adp1.SelectCommand.Parameters.AddWithValue("@OtherReason", DBNull.Value);
                        }
                        else
                        {
                            adp1.SelectCommand.Parameters.AddWithValue("@OtherReason", rpspc.OtherReason);
                        }
                        if (rpspc.MooringOperationID == null)
                        {
                            adp1.SelectCommand.Parameters.AddWithValue("@MOpId", DBNull.Value);
                        }
                        else
                        {
                            adp1.SelectCommand.Parameters.AddWithValue("@MOpId", rpspc.MooringOperationID);
                        }
                        adp1.SelectCommand.Parameters.AddWithValue("@DamagedObserved", rpspc.DamageObserved);
                        adp1.SelectCommand.Parameters.AddWithValue("@OutofServiceDate", rpspc.OutofServiceDate);
                        adp1.SelectCommand.Parameters.AddWithValue("@ReasonOutofService", rpspc.ReasonOutofService);
                        adp1.SelectCommand.Parameters.AddWithValue("@Id", rpspc.LEQuipID);
                        adp1.SelectCommand.Parameters.AddWithValue("@VesselID", VesselID);
                        DataTable dt1 = new DataTable();
                        adp1.Fill(dt1);
                    }
                }
                else if (rpspc.LooseETypeId == 5)
                {
                    string qry = "update ChainStopper  set OtherReason=@OtherReason,DamagedObserved=@DamagedObserved , OutofServiceDate=@OutofServiceDate, ReasonOutofService=@ReasonOutofService ,MOpId=@MOpId where  Id=@Id and VesselID=@VesselID";
                    using (SqlDataAdapter adp1 = new SqlDataAdapter(qry, con))
                    {
                        if (rpspc.OtherReason == null)
                        {
                            adp1.SelectCommand.Parameters.AddWithValue("@OtherReason", DBNull.Value);
                        }
                        else
                        {
                            adp1.SelectCommand.Parameters.AddWithValue("@OtherReason", rpspc.OtherReason);
                        }
                        if (rpspc.MooringOperationID == null)
                        {
                            adp1.SelectCommand.Parameters.AddWithValue("@MOpId", DBNull.Value);
                        }
                        else
                        {
                            adp1.SelectCommand.Parameters.AddWithValue("@MOpId", rpspc.MooringOperationID);
                        }
                        adp1.SelectCommand.Parameters.AddWithValue("@DamagedObserved", rpspc.DamageObserved);
                        adp1.SelectCommand.Parameters.AddWithValue("@OutofServiceDate", rpspc.OutofServiceDate);
                        adp1.SelectCommand.Parameters.AddWithValue("@ReasonOutofService", rpspc.ReasonOutofService);
                        adp1.SelectCommand.Parameters.AddWithValue("@Id", rpspc.LEQuipID);
                        adp1.SelectCommand.Parameters.AddWithValue("@VesselID", VesselID);
                        DataTable dt1 = new DataTable();
                        adp1.Fill(dt1);
                    }
                }
                else if (rpspc.LooseETypeId == 7)
                {
                    string qry = "update ChafeGuard   set OtherReason=@OtherReason, OutofServiceDate=@OutofServiceDate, ReasonOutofService=@ReasonOutofService where  Id=@Id and VesselID=@VesselID";
                    using (SqlDataAdapter adp1 = new SqlDataAdapter(qry, con))
                    {
                        if (rpspc.OtherReason == null)
                        {
                            adp1.SelectCommand.Parameters.AddWithValue("@OtherReason", DBNull.Value);
                        }
                        else
                        {
                            adp1.SelectCommand.Parameters.AddWithValue("@OtherReason", rpspc.OtherReason);
                        }
                        //if (rpspc.MooringOperationID == null)
                        //{
                        //    adp1.SelectCommand.Parameters.AddWithValue("@MOpId", DBNull.Value);
                        //}
                        //else
                        //{
                        //    adp1.SelectCommand.Parameters.AddWithValue("@MOpId", rpspc.MooringOperationID);
                        //}
                        //adp1.SelectCommand.Parameters.AddWithValue("@DamagedObserved", rpspc.DamageObserved);
                        adp1.SelectCommand.Parameters.AddWithValue("@OutofServiceDate", rpspc.OutofServiceDate);
                        adp1.SelectCommand.Parameters.AddWithValue("@ReasonOutofService", rpspc.ReasonOutofService);
                        adp1.SelectCommand.Parameters.AddWithValue("@Id", rpspc.LEQuipID);
                        adp1.SelectCommand.Parameters.AddWithValue("@VesselID", VesselID);
                        DataTable dt1 = new DataTable();
                        adp1.Fill(dt1);
                    }
                }
                else if (rpspc.LooseETypeId == 8)
                {
                    string qry = "update WinchBreakTestKit  set OtherReason=@OtherReason, OutofServiceDate=@OutofServiceDate, ReasonOutofService=@ReasonOutofService  where  Id=@Id and VesselID=@VesselID";
                    using (SqlDataAdapter adp1 = new SqlDataAdapter(qry, con))
                    {
                        if (rpspc.OtherReason == null)
                        {
                            adp1.SelectCommand.Parameters.AddWithValue("@OtherReason", DBNull.Value);
                        }
                        else
                        {
                            adp1.SelectCommand.Parameters.AddWithValue("@OtherReason", rpspc.OtherReason);
                        }
                        //if (rpspc.MooringOperationID == null)
                        //{
                        //    adp1.SelectCommand.Parameters.AddWithValue("@MOpId", DBNull.Value);
                        //}
                        //else
                        //{
                        //    adp1.SelectCommand.Parameters.AddWithValue("@MOpId", rpspc.MooringOperationID);
                        //}
                        //adp1.SelectCommand.Parameters.AddWithValue("@DamagedObserved", rpspc.DamageObserved);
                        adp1.SelectCommand.Parameters.AddWithValue("@OutofServiceDate", rpspc.OutofServiceDate);
                        adp1.SelectCommand.Parameters.AddWithValue("@ReasonOutofService", rpspc.ReasonOutofService);
                        adp1.SelectCommand.Parameters.AddWithValue("@Id", rpspc.LEQuipID);
                        adp1.SelectCommand.Parameters.AddWithValue("@VesselID", VesselID);
                        DataTable dt1 = new DataTable();
                        adp1.Fill(dt1);
                    }
                }
                else //if (LTp == 4)
                {
                    string qry = "update RopeTail set OtherReason=@OtherReason,DamageObserved=@DamagedObserved , OutofServiceDate=@OutofServiceDate, ReasonOutofService=@ReasonOutofService ,MOpId=@MOpId where  Id=@Id and VesselID=@VesselID and LooseETypeId=@LooseETypeId";
                    using (SqlDataAdapter adp1 = new SqlDataAdapter(qry, con))
                    {
                        if (rpspc.OtherReason == null)
                        {
                            adp1.SelectCommand.Parameters.AddWithValue("@OtherReason", DBNull.Value);
                        }
                        else
                        {
                            adp1.SelectCommand.Parameters.AddWithValue("@OtherReason", rpspc.OtherReason);
                        }
                        if (rpspc.MooringOperationID == null)
                        {
                            adp1.SelectCommand.Parameters.AddWithValue("@MOpId", DBNull.Value);
                        }
                        else
                        {
                            adp1.SelectCommand.Parameters.AddWithValue("@MOpId", rpspc.MooringOperationID);
                        }
                        adp1.SelectCommand.Parameters.AddWithValue("@DamagedObserved", rpspc.DamageObserved);
                        adp1.SelectCommand.Parameters.AddWithValue("@OutofServiceDate", rpspc.OutofServiceDate);
                        adp1.SelectCommand.Parameters.AddWithValue("@ReasonOutofService", rpspc.ReasonOutofService);
                        adp1.SelectCommand.Parameters.AddWithValue("@Id", rpspc.LEQuipID);
                        adp1.SelectCommand.Parameters.AddWithValue("@VesselID", VesselID);
                        adp1.SelectCommand.Parameters.AddWithValue("@LooseETypeId", rpspc.LooseETypeId);
                        DataTable dt1 = new DataTable();
                        adp1.Fill(dt1);
                    }


                }

                LooseE_NotificationDiscard("Out of Service / discarded", Convert.ToInt32(rpspc.LooseETypeId), rpspc.CertificateNumber, rpspc.LEQuipID);


                if (rpspc.ReasonOutofService == "Damaged")
                {
                    LooseEDamageRecord LEDR = new LooseEDamageRecord() { LooseETypeId = rpspc.LooseETypeId };
                    // LEDR.LooseETypeId = LEDR.LooseETypeId;
                    var IdPK = ((from asn in context.LooseEDamageRecords.Where(x => x.VesselID == VesselID) select (int?)asn.Id).Max() ?? 0) + 1;


                    if (LEDR.DamageObserved == "Inspection")
                    {
                        LEDR.MOpId = null;
                    }
                    else
                    {
                        LEDR.MOpId = rpspc.MooringOperationID;
                    }


                    LEDR.Id = IdPK;
                    LEDR.CertificateNumber = rpspc.CertificateNumber;
                    LEDR.DamageObserved = rpspc.DamageObserved;
                    LEDR.DamageDate = rpspc.OutofServiceDate;
                    LEDR.VesselID = VesselID;
                    LEDR.CreatedDate = DateTime.Now;
                    LEDR.CreatedBy = "Admin";
                    LEDR.IsActive = true;
                    LEDR.NotificationId = CommonClass.NextNotiId(VesselID);

                    context.LooseEDamageRecords.Add(LEDR);
                    context.SaveChanges();
                    LooseE_NotificationDamage(Convert.ToInt32(LEDR.LooseETypeId), LEDR.CertificateNumber, Convert.ToInt32(LEDR.NotificationId));

                }


            }



            catch (Exception ex)
            {

            }
            return RedirectToAction("DiscardIndex");
        }

        public ActionResult AddLeDamage(int LET)
        {
            //var LEDamageRecord = new ShipmentContaxt().MultipleResults("[dbo].[GetDamageLooseE] '" + VesselID + "'")
            //         .With<LooseEDamageRecord>().Execute();
            int VesselID = Convert.ToInt32(Session["VesselSessionID"].ToString());
            LooseEDamageRecord led = new LooseEDamageRecord();

            led.LooseETypeId = LET;
            ViewBag.LooseEquipmentType = context.LooseETypes.SingleOrDefault(x => x.Id == LET).LooseEquipmentType;
            led.GetAllLE_Detail = GetLooseEquipments(LET);
            led.DamageReasons = CommonClass.DamageReasonList();
            led.DamageLocations = CommonClass.DamageLocatonList();
            led.DamageObservedLists = CommonClass.DamagObservedList();
            led.MooringOperationsLists = CommonClass.MooringOpListCommon(VesselID);

            return View(led);
        }

        [HttpPost]
        [PreventSpam("AddLeDamage", 3, 1)]
        public ActionResult AddLeDamage(LooseEDamageRecord LEDR)
        {
            int VesselID = Convert.ToInt32(Session["VesselSessionID"].ToString());
            LEDR.LooseETypeId = LEDR.LooseETypeId;
            var IdPK = ((from asn in context.LooseEDamageRecords.Where(x => x.VesselID == VesselID) select (int?)asn.Id).Max() ?? 0) + 1;


            if (LEDR.DamageObserved == "Inspection")
            {
                LEDR.MOpId = null;
            }


            LEDR.Id = IdPK;
            LEDR.VesselID = VesselID;
            LEDR.CreatedDate = DateTime.Now;
            LEDR.CreatedBy = "Admin";
            LEDR.IsActive = true;
            LEDR.NotificationId = CommonClass.NextNotiId(VesselID);

            context.LooseEDamageRecords.Add(LEDR);
            context.SaveChanges();
            LooseE_NotificationDamage(Convert.ToInt32(LEDR.LooseETypeId), LEDR.CertificateNumber, Convert.ToInt32(LEDR.NotificationId));
            return RedirectToAction("Index");
        }


        public ActionResult DisposeIndex()
        {
            var VesselID = Session["VesselSessionID"].ToString();
            LooseEDisposal list = new LooseEDisposal();
            using (ShipmentContaxt sc1 = new ShipmentContaxt())
            {

                var LooseEDisposals = new ShipmentContaxt()
                     .MultipleResults("[dbo].[GetDisposalLooseE] '" + VesselID + "'")
                   .With<LooseEDisposal>().Execute();

                list.AllDisposalList = (List<LooseEDisposal>)LooseEDisposals[0];
            }
            return View(list);


        }

        public ActionResult AddDisposal(int LET)
        {
            ViewBag.LooseEquipmentType = context.LooseETypes.SingleOrDefault(x => x.Id == LET).LooseEquipmentType;
            LooseEDisposal obj = new LooseEDisposal() { LooseETypeId = LET };
            obj.GetAllLE_DiscardedDetail = GetLooseEquipmentsDiscarded(LET);
            return View(obj);
        }
        [HttpPost]
        [PreventSpam("AddDisposalLE", 3, 1)]
        public ActionResult AddDisposal(LooseEDisposal LEDR)
        {
            int VesselID = Convert.ToInt32(Session["VesselSessionID"].ToString());
            LEDR.LooseETypeId = LEDR.LooseETypeId;
            ViewBag.LooseEquipmentType = context.LooseETypes.SingleOrDefault(x => x.Id == LEDR.LooseETypeId).LooseEquipmentType;
            var duplicate = context.LooseEDisposals.Where(x =>x.LooseECertiNo == LEDR.LooseECertiNo && x.VesselID == VesselID).Count();
            if (duplicate == 0)
            {
                var IdPK = ((from asn in context.LooseEDisposals.Where(x => x.VesselID == VesselID) select (int?)asn.Id).Max() ?? 0) + 1;

                if (!string.IsNullOrEmpty(LEDR.DiscardedDate1))
                    LEDR.DiscardedDate = Convert.ToDateTime(LEDR.DiscardedDate1);

                LEDR.Id = IdPK;
                LEDR.VesselID = VesselID;

                LEDR.CreatedDate = DateTime.Now;
                LEDR.CreatedBy = "Admin";
                LEDR.IsActive = true;
                //LEDR.NotificationId = CommonClass.NextNotiId();

                context.LooseEDisposals.Add(LEDR);
                context.SaveChanges();
                LooseE_NotificationDiscard("Disposal", Convert.ToInt32(LEDR.LooseETypeId), LEDR.LooseECertiNo,LEDR.LEQuipID);
                // LooseE_NotificationDamage(Convert.ToInt32(LEDR.LooseETypeId), LEDR.CertificateNumber, Convert.ToInt32(LEDR.NotificationId));
                TempData["Success"] = "Record deleted successfully ";
                return RedirectToAction("DisposeIndex");
            }
            else
            {
                LEDR.GetAllLE_DiscardedDetail = GetLooseEquipmentsDiscarded( Convert.ToInt32(LEDR.LooseETypeId));
                TempData["Error"] = "Record is already exist!";
                return View(LEDR);
            }
        }

        public JsonResult GetDiscardDate(int Id, int LET)
        {


            int VesselID = Convert.ToInt32(Session["VesselSessionID"].ToString());
            string discarddate;
            if (LET == 1)
            {
                // string qry = "update JoiningShackle  set OtherReason=@OtherReason,DamagedObserved=@DamagedObserved , OutofServiceDate=@OutofServiceDate, ReasonOutofService=@ReasonOutofService ,MOpId=@MOpId where  Id=@Id and VesselID=@VesselID";
                var data = context.JoiningShackles.Where(x => x.Id == Id && x.VesselID == VesselID).FirstOrDefault();
                discarddate = data.OutofServiceDate.ToString();
                if (!string.IsNullOrEmpty(discarddate))
                {
                    discarddate = Convert.ToDateTime(data.OutofServiceDate).ToString("yyyy-MM-dd");
                }
            }
            else if (LET == 5)
            {
                var data = context.ChainStoppers.Where(x => x.Id == Id && x.VesselID == VesselID).FirstOrDefault();
                discarddate = data.OutofServiceDate.ToString();
                if (!string.IsNullOrEmpty(discarddate))
                {
                    discarddate = Convert.ToDateTime(data.OutofServiceDate).ToString("yyyy-MM-dd");
                }
            }
            else if (LET == 7)
            {
                var data = context.ChafeGuards.Where(x => x.Id == Id && x.VesselID == VesselID).FirstOrDefault();
                discarddate = data.OutofServiceDate.ToString();
                if (!string.IsNullOrEmpty(discarddate))
                {
                    discarddate = Convert.ToDateTime(data.OutofServiceDate).ToString("yyyy-MM-dd");
                }
            }
            else if (LET == 8)
            {
                var data = context.WinchBreakTestKits.Where(x => x.Id == Id && x.VesselID == VesselID).FirstOrDefault();
                discarddate = data.OutofServiceDate.ToString();
                if (!string.IsNullOrEmpty(discarddate))
                {
                    discarddate = Convert.ToDateTime(data.OutofServiceDate).ToString("yyyy-MM-dd");
                }
            }
            else //if (LTp == 4)
            {
                // string qry = "update RopeTail set OtherReason=@OtherReason,DamageObserved=@DamagedObserved , OutofServiceDate=@OutofServiceDate, ReasonOutofService=@ReasonOutofService ,MOpId=@MOpId where  Id=@Id and VesselID=@VesselID and LooseETypeId=@LooseETypeId";
                var data = context.RopeTails.Where(x => x.Id == Id && x.LooseETypeId == LET && x.VesselID == VesselID).FirstOrDefault();
                discarddate = data.OutofServiceDate.ToString();
                if (!string.IsNullOrEmpty(discarddate))
                {
                    discarddate = Convert.ToDateTime(data.OutofServiceDate).ToString("yyyy-MM-dd");
                }

            }

            return Json(new { Result = true, outboard = discarddate }, JsonRequestBehavior.AllowGet);
        }

        
        public ActionResult delete(int id, int NotiID)
        {
            try
            {
                int vesselid = Convert.ToInt32(Session["VesselSessionID"].ToString());
                using (SqlDataAdapter adp1 = new SqlDataAdapter("update LooseEDamageRecord set IsActive=0 where  Id=" + id + " and VesselID= " + vesselid + "", con))
                {
                    DataTable dt1 = new DataTable();
                    adp1.Fill(dt1);
                }
                using (SqlDataAdapter adp1 = new SqlDataAdapter("delete from tblNotification where id=" + NotiID + " and VesselId=" + vesselid + "", con))
                {
                    DataTable dt1 = new DataTable();
                    adp1.Fill(dt1);
                }

            }
            catch (Exception ex)
            {
            }
            TempData["Success"] = "Record deleted successfully ";

            return RedirectToAction("index");
            // int ropeid = context.JoiningShackles.Where(x => x.Id == id && x.VesselID == vesselid).Select(x => x.RopeId).SingleOrDefault();

        }

       // [PreventSpam("DeleteDiscardLE", 3, 1)]
        public ActionResult DeleteDiscard(int id, int LTp)
        {
            int VesselID = Convert.ToInt32(Session["VesselSessionID"].ToString());
            if (LTp == 1)
            {
                string qry = "update JoiningShackle  set OtherReason=@OtherReason,DamagedObserved=@DamagedObserved , OutofServiceDate=@OutofServiceDate, ReasonOutofService=@ReasonOutofService ,MOpId=@MOpId where  Id=@Id and VesselID=@VesselID";
                using (SqlDataAdapter adp1 = new SqlDataAdapter(qry, con))
                {

                    adp1.SelectCommand.Parameters.AddWithValue("@OtherReason", DBNull.Value);

                    adp1.SelectCommand.Parameters.AddWithValue("@MOpId", DBNull.Value);

                    adp1.SelectCommand.Parameters.AddWithValue("@DamagedObserved", DBNull.Value);
                    adp1.SelectCommand.Parameters.AddWithValue("@OutofServiceDate", DBNull.Value);
                    adp1.SelectCommand.Parameters.AddWithValue("@ReasonOutofService", DBNull.Value);
                    adp1.SelectCommand.Parameters.AddWithValue("@Id", id);
                    adp1.SelectCommand.Parameters.AddWithValue("@VesselID", VesselID);
                    DataTable dt1 = new DataTable();
                    adp1.Fill(dt1);
                }
            }
            else if (LTp == 5)
            {
                string qry = "update ChainStopper  set OtherReason=@OtherReason,DamagedObserved=@DamagedObserved , OutofServiceDate=@OutofServiceDate, ReasonOutofService=@ReasonOutofService ,MOpId=@MOpId where  Id=@Id and VesselID=@VesselID";
                using (SqlDataAdapter adp1 = new SqlDataAdapter(qry, con))
                {

                    adp1.SelectCommand.Parameters.AddWithValue("@OtherReason", DBNull.Value);

                    adp1.SelectCommand.Parameters.AddWithValue("@MOpId", DBNull.Value);

                    adp1.SelectCommand.Parameters.AddWithValue("@DamagedObserved", DBNull.Value);
                    adp1.SelectCommand.Parameters.AddWithValue("@OutofServiceDate", DBNull.Value);
                    adp1.SelectCommand.Parameters.AddWithValue("@ReasonOutofService", DBNull.Value);
                    adp1.SelectCommand.Parameters.AddWithValue("@Id", id);
                    adp1.SelectCommand.Parameters.AddWithValue("@VesselID", VesselID);
                    DataTable dt1 = new DataTable();
                    adp1.Fill(dt1);
                }
            }
            else if (LTp == 7)
            {
                string qry = "update ChafeGuard   set OtherReason=@OtherReason, OutofServiceDate=@OutofServiceDate, ReasonOutofService=@ReasonOutofService where  Id=@Id and VesselID=@VesselID";
                using (SqlDataAdapter adp1 = new SqlDataAdapter(qry, con))
                {

                    adp1.SelectCommand.Parameters.AddWithValue("@OtherReason", DBNull.Value);

                    adp1.SelectCommand.Parameters.AddWithValue("@OutofServiceDate", DBNull.Value);
                    adp1.SelectCommand.Parameters.AddWithValue("@ReasonOutofService", DBNull.Value);
                    adp1.SelectCommand.Parameters.AddWithValue("@Id", id);
                    adp1.SelectCommand.Parameters.AddWithValue("@VesselID", VesselID);
                    DataTable dt1 = new DataTable();
                    adp1.Fill(dt1);
                }
            }
            else if (LTp == 8)
            {
                string qry = "update WinchBreakTestKit  set OtherReason=@OtherReason, OutofServiceDate=@OutofServiceDate, ReasonOutofService=@ReasonOutofService  where  Id=@Id and VesselID=@VesselID";
                using (SqlDataAdapter adp1 = new SqlDataAdapter(qry, con))
                {

                    adp1.SelectCommand.Parameters.AddWithValue("@OtherReason", DBNull.Value);

                    adp1.SelectCommand.Parameters.AddWithValue("@OutofServiceDate", DBNull.Value);
                    adp1.SelectCommand.Parameters.AddWithValue("@ReasonOutofService", DBNull.Value);
                    adp1.SelectCommand.Parameters.AddWithValue("@Id", id);
                    adp1.SelectCommand.Parameters.AddWithValue("@VesselID", VesselID);
                    DataTable dt1 = new DataTable();
                    adp1.Fill(dt1);
                }
            }
            else //if (LTp == 4)
            {
                string qry = "update RopeTail set OtherReason=@OtherReason,DamageObserved=@DamagedObserved , OutofServiceDate=@OutofServiceDate, ReasonOutofService=@ReasonOutofService ,MOpId=@MOpId where  Id=@Id and VesselID=@VesselID and LooseETypeId=@LooseETypeId";
                using (SqlDataAdapter adp1 = new SqlDataAdapter(qry, con))
                {

                    adp1.SelectCommand.Parameters.AddWithValue("@OtherReason", DBNull.Value);

                    adp1.SelectCommand.Parameters.AddWithValue("@MOpId", DBNull.Value);

                    adp1.SelectCommand.Parameters.AddWithValue("@DamagedObserved", DBNull.Value);
                    adp1.SelectCommand.Parameters.AddWithValue("@OutofServiceDate", DBNull.Value);
                    adp1.SelectCommand.Parameters.AddWithValue("@ReasonOutofService", DBNull.Value);
                    adp1.SelectCommand.Parameters.AddWithValue("@Id", id);
                    adp1.SelectCommand.Parameters.AddWithValue("@VesselID", VesselID);
                    adp1.SelectCommand.Parameters.AddWithValue("@LooseETypeId", LTp);
                    DataTable dt1 = new DataTable();
                    adp1.Fill(dt1);
                }


            }

            using (SqlDataAdapter adp1 = new SqlDataAdapter("delete from tblNotification where VesselId=" + VesselID + " and Notification like 'Out of Service%' and LooseEqType=" + LTp + " and LooseCertificateNum='"+ id + "' ", con))
            {
                DataTable dt1 = new DataTable();
                adp1.Fill(dt1);
            }

            return RedirectToAction("DiscardIndex");
        }

        [PreventSpam("DeleteDisposalLE", 3, 1)]
        public ActionResult DeleteDisposal(int id, int LET)
        {
            int vesselid = Convert.ToInt32(Session["VesselSessionID"].ToString());
            using (SqlDataAdapter adp1 = new SqlDataAdapter("update LooseEDisposal set IsActive=0 where  Id=" + id + " and LooseETypeId="+ LET + " and VesselID= " + vesselid + "", con))
            {
                DataTable dt1 = new DataTable();
                adp1.Fill(dt1);
            }

            using (SqlDataAdapter adp1 = new SqlDataAdapter("delete from tblNotification where VesselId="+ vesselid + " and Notification like 'Disposal%' and LooseEqType=" + LET + " and LooseCertificateNum='" + id + "' ", con))
            {
                DataTable dt1 = new DataTable();
                adp1.Fill(dt1);
            }
            return RedirectToAction("DisposeIndex");
        }

        public void LooseE_NotificationDamage(int looseETypeId, string CertificateNumber, int NotiID)
        {
            try
            {

                int VesselID = Convert.ToInt32(Session["VesselSessionID"].ToString());
                // var looseE = sc.LooseEDamageR.Where(x => x.Id == id && x.LooseETypeId == looseETypeId).FirstOrDefault();

                // var looseEname = sc.LooseETypes.Where(x => x.Id == looseE.LooseETypeId).Select(x => x.LooseEquipmentType).SingleOrDefault();
                var looseEname = context.LooseETypes.SingleOrDefault(x => x.Id == looseETypeId).LooseEquipmentType;
                var notification = "Damaged - Loose Equipment " + looseEname + " CertificateNo- " + CertificateNumber + "";

                tblNotification noti = new tblNotification();
                noti.Id = NotiID;
                noti.VesselId = VesselID;
                noti.Acknowledge = false;
                noti.AckRecord = "Not yet acknowledged";
                noti.Notification = notification;
                noti.NotificationType = (int)NotificationAlertType.All_LooseEquipmentDamaged;
                noti.RopeId = 0;
                noti.IsActive = true;
                //noti.NotificationDueDate = DBNull.Value;
                noti.CreatedDate = DateTime.Now;
                noti.CreatedBy = "Admin";
                noti.LooseCertificateNum = CertificateNumber;
                noti.LooseEqType = looseETypeId;
                context.tblNotifications.Add(noti);
                context.SaveChanges();

                // StaticHelper.AlarmFunction(1, true);

            }
            catch { }
        }

        public void LooseE_NotificationDiscard(string For, int looseETypeId, string CertificateNumber,int LooseEqpID)
        {
            try
            {
                int VesselID = Convert.ToInt32(Session["VesselSessionID"].ToString());
                if (looseETypeId == 1)
                {
                    var looseEname = context.LooseETypes.SingleOrDefault(x => x.Id == looseETypeId).LooseEquipmentType;

                    var notification = For + " - Loose Equipment " + looseEname + " CertificateNo. " + CertificateNumber + "";

                    tblNotification noti = new tblNotification();
                    noti.Id = CommonClass.NextNotiId(VesselID);
                    noti.VesselId = VesselID;
                    noti.Acknowledge = false;
                    noti.AckRecord = "Not yet acknowledged";
                    noti.Notification = notification;
                    noti.NotificationType = 1;
                    noti.RopeId = 0;
                    noti.IsActive = true;

                    //noti.NotificationDueDate = DBNull.Value;
                    noti.NotificationAlertType = (int)NotificationAlertType.JoiningShackle_LooseEquipmentDisCard;
                    noti.CreatedDate = DateTime.Now;
                    noti.CreatedBy = "Admin";
                    noti.LooseCertificateNum = LooseEqpID.ToString();
                    noti.LooseEqType = looseETypeId;
                    context.tblNotifications.Add(noti);
                    context.SaveChanges();

                    // StaticHelper.AlarmFunction(1, true);
                }
                else if (looseETypeId == 5)
                {

                    var looseEname = context.LooseETypes.SingleOrDefault(x => x.Id == looseETypeId).LooseEquipmentType;

                    var notification = For + " - Loose Equipment " + looseEname + " CertificateNo. " + CertificateNumber + "";

                    tblNotification noti = new tblNotification();
                    noti.Id = CommonClass.NextNotiId(VesselID);
                    noti.VesselId = VesselID;
                    noti.Acknowledge = false;
                    noti.AckRecord = "Not yet acknowledged";
                    noti.Notification = notification;
                    noti.NotificationType = 1;
                    noti.RopeId = 0;
                    noti.IsActive = true;
                    //noti.NotificationDueDate = DBNull.Value;
                    noti.NotificationAlertType = (int)NotificationAlertType.ChainStopper_LooseEquipmentDisCard;
                    noti.CreatedDate = DateTime.Now;
                    noti.CreatedBy = "Admin";
                    noti.LooseCertificateNum = LooseEqpID.ToString(); 
                    noti.LooseEqType = looseETypeId;
                    context.tblNotifications.Add(noti);
                    context.SaveChanges();


                }
                else if (looseETypeId == 7)
                {

                    var looseEname = context.LooseETypes.SingleOrDefault(x => x.Id == looseETypeId).LooseEquipmentType;

                    var notification = For + " - Loose Equipment " + looseEname + " CertificateNo. " + CertificateNumber + "";

                    tblNotification noti = new tblNotification();
                    noti.Id = CommonClass.NextNotiId(VesselID);
                    noti.VesselId = VesselID;
                    noti.Acknowledge = false;
                    noti.AckRecord = "Not yet acknowledged";
                    noti.Notification = notification;
                    noti.NotificationType = 1;
                    noti.RopeId = 0;
                    noti.IsActive = true;
                    //noti.NotificationDueDate = DBNull.Value;
                    noti.NotificationAlertType = (int)NotificationAlertType.ChainStopper_LooseEquipmentDisCard;
                    noti.CreatedDate = DateTime.Now;
                    noti.CreatedBy = "Admin";
                    noti.LooseCertificateNum = LooseEqpID.ToString();
                    noti.LooseEqType = looseETypeId;
                    context.tblNotifications.Add(noti);
                    context.SaveChanges();



                }
                else if (looseETypeId == 8)
                {


                    var looseEname = context.LooseETypes.SingleOrDefault(x => x.Id == looseETypeId).LooseEquipmentType;

                    var notification = For + " - Loose Equipment " + looseEname + " CertificateNo. " + CertificateNumber + "";

                    tblNotification noti = new tblNotification();
                    noti.Id = CommonClass.NextNotiId(VesselID);
                    noti.VesselId = VesselID;
                    noti.Acknowledge = false;
                    noti.AckRecord = "Not yet acknowledged";
                    noti.Notification = notification;
                    noti.NotificationType = 1;
                    noti.RopeId = 0;
                    noti.IsActive = true;
                    //noti.NotificationDueDate = DBNull.Value;
                    noti.NotificationAlertType = (int)NotificationAlertType.ChainStopper_LooseEquipmentDisCard;
                    noti.CreatedDate = DateTime.Now;
                    noti.CreatedBy = "Admin";
                    noti.LooseCertificateNum = LooseEqpID.ToString();
                    noti.LooseEqType = looseETypeId;
                    context.tblNotifications.Add(noti);
                    context.SaveChanges();



                }
                else
                {

                    var looseEname = context.LooseETypes.SingleOrDefault(x => x.Id == looseETypeId).LooseEquipmentType;

                    var notification = For + " - Loose Equipment " + looseEname + " CertificateNo. " + CertificateNumber + "";

                    tblNotification noti = new tblNotification();
                    noti.Id = CommonClass.NextNotiId(VesselID);
                    noti.VesselId = VesselID;
                    noti.Acknowledge = false;
                    noti.AckRecord = "Not yet acknowledged";
                    noti.Notification = notification;
                    noti.NotificationType = 1;
                    noti.RopeId = 0;
                    noti.IsActive = true;
                    //noti.NotificationDueDate = DBNull.Value;
                    noti.NotificationAlertType = (int)NotificationAlertType.RopeStopper_FireWire_Messanger_LooseEquipmentDisCard;
                    noti.CreatedDate = DateTime.Now;
                    noti.CreatedBy = "Admin";
                    noti.LooseCertificateNum = LooseEqpID.ToString();
                    noti.LooseEqType = looseETypeId;
                    context.tblNotifications.Add(noti);
                    context.SaveChanges();

                }
            }
            catch (Exception ex) { }
        }

        public List<SelectListItem> GetLooseEquipments(int type)
        {
            int VesselID = Convert.ToInt32(Session["VesselSessionID"].ToString());
            List<SelectListItem> LEDamagelist = new List<SelectListItem>();
            using (ShipmentContaxt sc1 = new ShipmentContaxt())
            {
                if (type == 1)
                {
                    //JoiningShackle 
                    var JShackle = new ShipmentContaxt().MultipleResults("[dbo].[LooseEquipmentTypeDetails] 'JShackle','" + VesselID + "'")
                       .With<JoiningShackle>().Execute();

                    var JoiningShackleList = (List<JoiningShackle>)JShackle[0];

                    foreach (var item in JoiningShackleList)
                    {
                        LEDamagelist.Add(new SelectListItem() { Text = item.CertificateNumber + " - " + item.UniqueID, Value = item.Id.ToString() });
                        //var show = item.CertificateNumber + " - " + item.UniqueID;
                        //LEDamagelist.Add(new SelectListItem() { Text = show, Value = show });

                    }
                }

                if (type == 4)
                {
                    //RopeStopper 
                    var RTail = new ShipmentContaxt()
                    .MultipleResults("[dbo].[LooseEquipmentTypeDetails] 'RTail','" + VesselID + "'").With<RopeTail>().Execute();
                    var RopeStopperList = (List<RopeTail>)RTail[0];

                    foreach (var item in RopeStopperList)
                    {
                        LEDamagelist.Add(new SelectListItem() { Text = item.CertificateNumber + " - " + item.UniqueID, Value = item.Id.ToString() });
                        //var show = item.CertificateNumber + " - " + item.UniqueID;
                        //LEDamagelist.Add(new SelectListItem() { Text = show, Value = show });
                    }
                }

                if (type == 9)
                {
                    //TowingRope 
                    var TowingRope = new ShipmentContaxt()
                   .MultipleResults("[dbo].[LooseEquipmentTypeDetails] 'TowingRope','" + VesselID + "'").With<RopeTail>().Execute();
                    var TowingRopeList = (List<RopeTail>)TowingRope[0];

                    foreach (var item in TowingRopeList)
                    {
                        LEDamagelist.Add(new SelectListItem() { Text = item.CertificateNumber + " - " + item.UniqueID, Value = item.Id.ToString() });
                        //var show = item.CertificateNumber + " - " + item.UniqueID;
                        //LEDamagelist.Add(new SelectListItem() { Text = show, Value = show });
                    }
                }
                if (type == 10)
                {
                    //SuezRope 
                    var SuezRope = new ShipmentContaxt()
                   .MultipleResults("[dbo].[LooseEquipmentTypeDetails] 'SuezRope','" + VesselID + "'").With<RopeTail>().Execute();
                    var SuezRopeRopeList = (List<RopeTail>)SuezRope[0];

                    foreach (var item in SuezRopeRopeList)
                    {
                        LEDamagelist.Add(new SelectListItem() { Text = item.CertificateNumber + " - " + item.UniqueID, Value = item.Id.ToString() });
                        //var show = item.CertificateNumber + " - " + item.UniqueID;
                        //LEDamagelist.Add(new SelectListItem() { Text = show, Value = show });

                    }
                }

                if (type == 3)
                {
                    //MessengerRope
                    var MRope = new ShipmentContaxt()
                  .MultipleResults("[dbo].[LooseEquipmentTypeDetails] 'MRope','" + VesselID + "'").With<RopeTail>().Execute();
                    var MessengerRopeList = (List<RopeTail>)MRope[0];

                    foreach (var item in MessengerRopeList)
                    {
                        LEDamagelist.Add(new SelectListItem() { Text = item.CertificateNumber + " - " + item.UniqueID, Value = item.Id.ToString() });
                        //var show = item.CertificateNumber + " - " + item.UniqueID;
                        //LEDamagelist.Add(new SelectListItem() { Text = show, Value = show });

                    }
                }

                if (type == 6)
                {
                    //FireWire
                    var FireWire = new ShipmentContaxt()
                  .MultipleResults("[dbo].[LooseEquipmentTypeDetails] 'FireWire','" + VesselID + "'").With<RopeTail>().Execute();
                    var FireWireList = (List<RopeTail>)FireWire[0];

                    foreach (var item in FireWireList)
                    {
                        LEDamagelist.Add(new SelectListItem() { Text = item.CertificateNumber + " - " + item.UniqueID, Value = item.Id.ToString() });
                        //var show = item.CertificateNumber + " - " + item.UniqueID;
                        //LEDamagelist.Add(new SelectListItem() { Text = show, Value = show });
                    }
                }

                if (type == 5)
                {
                    //ChainStopper
                    var CStopper = new ShipmentContaxt()
                 .MultipleResults("[dbo].[LooseEquipmentTypeDetails] 'CStopper','" + VesselID + "'").With<ChainStopper>().Execute();
                    var ChainStopperList = (List<ChainStopper>)CStopper[0];

                    foreach (var item in ChainStopperList)
                    {
                        LEDamagelist.Add(new SelectListItem() { Text = item.CertificateNumber + " - " + item.UniqueID, Value = item.Id.ToString() });

                        //var show = item.CertificateNumber + " - " + item.UniqueID;
                        //LEDamagelist.Add(new SelectListItem() { Text = show, Value = show });
                    }
                }

                if (type == 7)
                {
                    //ChafeGuard
                    var ChafeGuard = new ShipmentContaxt()
                 .MultipleResults("[dbo].[LooseEquipmentTypeDetails] 'ChafeGuard','" + VesselID + "'").With<ChafeGuard>().Execute();
                    var ChafeGuardList = (List<ChafeGuard>)ChafeGuard[0];

                    foreach (var item in ChafeGuardList)
                    {
                        LEDamagelist.Add(new SelectListItem() { Text = item.CertificateNumber + " - " + item.UniqueID, Value = item.Id.ToString() });
                        //var show = item.CertificateNumber + " - " + item.UniqueID;
                        //LEDamagelist.Add(new SelectListItem() { Text = show, Value = show });
                    }
                }

                if (type == 8)
                {
                    //WinchBreakTestKit
                    var WinchBreakTestKit = new ShipmentContaxt()
                 .MultipleResults("[dbo].[LooseEquipmentTypeDetails] 'WinchBreakTestKit','" + VesselID + "'").With<WinchBreakTestKit>().Execute();
                    var WinchBreakTestKitList = (List<WinchBreakTestKit>)WinchBreakTestKit[0];

                    foreach (var item in WinchBreakTestKitList)
                    {
                        LEDamagelist.Add(new SelectListItem() { Text = item.CertificateNumber + " - " + item.UniqueID, Value = item.Id.ToString() });
                        //var show = item.CertificateNumber + " - " + item.UniqueID;
                        //LEDamagelist.Add(new SelectListItem() { Text = show, Value = show });
                    }
                }

                return LEDamagelist;
            }
        }

        public List<SelectListItem> GetLooseEquipmentsDiscarded(int type)
        {
            int VesselID = Convert.ToInt32(Session["VesselSessionID"].ToString());
            List<SelectListItem> LEDamagelist = new List<SelectListItem>();
            using (ShipmentContaxt sc1 = new ShipmentContaxt())
            {
                if (type == 1)
                {
                    //JoiningShackle 
                    var JShackle = new ShipmentContaxt().MultipleResults("[dbo].[LooseEquipmentTypeDiscardDetails] 'JShackle','" + VesselID + "'")
                       .With<JoiningShackle>().Execute();

                    var JoiningShackleList = (List<JoiningShackle>)JShackle[0];

                    foreach (var item in JoiningShackleList)
                    {
                        LEDamagelist.Add(new SelectListItem() { Text = item.CertificateNumber + " - " + item.UniqueID, Value = item.Id.ToString() });
                        //var show = item.CertificateNumber + " - " + item.UniqueID;
                        //LEDamagelist.Add(new SelectListItem() { Text = show, Value = show });

                    }
                }

                if (type == 4)
                {
                    //RopeStopper 
                    var RTail = new ShipmentContaxt()
                    .MultipleResults("[dbo].[LooseEquipmentTypeDiscardDetails] 'RTail','" + VesselID + "'").With<RopeTail>().Execute();
                    var RopeStopperList = (List<RopeTail>)RTail[0];

                    foreach (var item in RopeStopperList)
                    {
                        LEDamagelist.Add(new SelectListItem() { Text = item.CertificateNumber + " - " + item.UniqueID, Value = item.Id.ToString() });
                        //var show = item.CertificateNumber + " - " + item.UniqueID;
                        //LEDamagelist.Add(new SelectListItem() { Text = show, Value = show });
                    }
                }

                if (type == 9)
                {
                    //TowingRope 
                    var TowingRope = new ShipmentContaxt()
                   .MultipleResults("[dbo].[LooseEquipmentTypeDiscardDetails] 'TowingRope','" + VesselID + "'").With<RopeTail>().Execute();
                    var TowingRopeList = (List<RopeTail>)TowingRope[0];

                    foreach (var item in TowingRopeList)
                    {
                        LEDamagelist.Add(new SelectListItem() { Text = item.CertificateNumber + " - " + item.UniqueID, Value = item.Id.ToString() });
                        //var show = item.CertificateNumber + " - " + item.UniqueID;
                        //LEDamagelist.Add(new SelectListItem() { Text = show, Value = show });
                    }
                }
                if (type == 10)
                {
                    //SuezRope 
                    var SuezRope = new ShipmentContaxt()
                   .MultipleResults("[dbo].[LooseEquipmentTypeDiscardDetails] 'SuezRope','" + VesselID + "'").With<RopeTail>().Execute();
                    var SuezRopeRopeList = (List<RopeTail>)SuezRope[0];

                    foreach (var item in SuezRopeRopeList)
                    {
                        LEDamagelist.Add(new SelectListItem() { Text = item.CertificateNumber + " - " + item.UniqueID, Value = item.Id.ToString() });
                        //var show = item.CertificateNumber + " - " + item.UniqueID;
                        //LEDamagelist.Add(new SelectListItem() { Text = show, Value = show });

                    }
                }

                if (type == 3)
                {
                    //MessengerRope
                    var MRope = new ShipmentContaxt()
                  .MultipleResults("[dbo].[LooseEquipmentTypeDiscardDetails] 'MRope','" + VesselID + "'").With<RopeTail>().Execute();
                    var MessengerRopeList = (List<RopeTail>)MRope[0];

                    foreach (var item in MessengerRopeList)
                    {
                        LEDamagelist.Add(new SelectListItem() { Text = item.CertificateNumber + " - " + item.UniqueID, Value = item.Id.ToString() });
                        //var show = item.CertificateNumber + " - " + item.UniqueID;
                        //LEDamagelist.Add(new SelectListItem() { Text = show, Value = show });

                    }
                }

                if (type == 6)
                {
                    //FireWire
                    var FireWire = new ShipmentContaxt()
                  .MultipleResults("[dbo].[LooseEquipmentTypeDiscardDetails] 'FireWire','" + VesselID + "'").With<RopeTail>().Execute();
                    var FireWireList = (List<RopeTail>)FireWire[0];

                    foreach (var item in FireWireList)
                    {
                        LEDamagelist.Add(new SelectListItem() { Text = item.CertificateNumber + " - " + item.UniqueID, Value = item.Id.ToString() });
                        //var show = item.CertificateNumber + " - " + item.UniqueID;
                        //LEDamagelist.Add(new SelectListItem() { Text = show, Value = show });
                    }
                }

                if (type == 5)
                {
                    //ChainStopper
                    var CStopper = new ShipmentContaxt()
                 .MultipleResults("[dbo].[LooseEquipmentTypeDiscardDetails] 'CStopper','" + VesselID + "'").With<ChainStopper>().Execute();
                    var ChainStopperList = (List<ChainStopper>)CStopper[0];

                    foreach (var item in ChainStopperList)
                    {
                        LEDamagelist.Add(new SelectListItem() { Text = item.CertificateNumber + " - " + item.UniqueID, Value = item.Id.ToString() });

                        //var show = item.CertificateNumber + " - " + item.UniqueID;
                        //LEDamagelist.Add(new SelectListItem() { Text = show, Value = show });
                    }
                }

                if (type == 7)
                {
                    //ChafeGuard
                    var ChafeGuard = new ShipmentContaxt()
                 .MultipleResults("[dbo].[LooseEquipmentTypeDiscardDetails] 'ChafeGuard','" + VesselID + "'").With<ChafeGuard>().Execute();
                    var ChafeGuardList = (List<ChafeGuard>)ChafeGuard[0];

                    foreach (var item in ChafeGuardList)
                    {
                        LEDamagelist.Add(new SelectListItem() { Text = item.CertificateNumber + " - " + item.UniqueID, Value = item.Id.ToString() });
                        //var show = item.CertificateNumber + " - " + item.UniqueID;
                        //LEDamagelist.Add(new SelectListItem() { Text = show, Value = show });
                    }
                }

                if (type == 8)
                {
                    //WinchBreakTestKit
                    var WinchBreakTestKit = new ShipmentContaxt()
                 .MultipleResults("[dbo].[LooseEquipmentTypeDiscardDetails] 'WinchBreakTestKit','" + VesselID + "'").With<WinchBreakTestKit>().Execute();
                    var WinchBreakTestKitList = (List<WinchBreakTestKit>)WinchBreakTestKit[0];

                    foreach (var item in WinchBreakTestKitList)
                    {
                        LEDamagelist.Add(new SelectListItem() { Text = item.CertificateNumber + " - " + item.UniqueID, Value = item.Id.ToString() });
                        //var show = item.CertificateNumber + " - " + item.UniqueID;
                        //LEDamagelist.Add(new SelectListItem() { Text = show, Value = show });
                    }
                }

                return LEDamagelist;
            }
        }
    }
}