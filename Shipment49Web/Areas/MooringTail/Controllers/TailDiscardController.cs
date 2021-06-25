using MenuLayer;
using Reports;
using Shipment49Web.Areas.MooringTail.Models;
using Shipment49Web.Areas.MSPS.Models;
using Shipment49Web.Common;
using Shipment49Web.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;

namespace Shipment49Web.Areas.MooringTail.Controllers
{
    [Authorize]
    [UserAuthenticationFilter]
    [ErrorClass]
    public class TailDiscardController : BaseController
    {
        MorringOfficeEntities context = new MorringOfficeEntities();
        SqlConnection con = ConnectionBulder.con;
       // CommonClass cls = new CommonClass();
        MooringRopeDetail assR = new MooringRopeDetail();
        DamageR ss = new DamageR();
        public static int? winchid = 0;
        public static bool outboardEndinuse = true;
        //public static string VesselID;
        // GET: MooringLine/LineDiscard
       // public static int VesselID { get; set; }
        public TailDiscardController()
        {
           
        }
        public ActionResult Index()
        {
           int VesselID = Convert.ToInt32(Session["VesselSessionID"].ToString());
            using (ShipmentContaxt sc1 = new ShipmentContaxt())
            {
                MooringRopeDetails model = new MooringRopeDetails();
                var ilist = new ShipmentContaxt()
                     .MultipleResults("[dbo].[GetRopeDiscardList]  1,'" + VesselID + "'")
                   .With<MooringRopeDetails>()
                   .Execute();
                model.MooringLineDiscardList = (List<MooringRopeDetails>)ilist[0];
                return View(model);
            }
        }

        public ActionResult addlinediscard()
        {
            int VesselID = Convert.ToInt32(Session["VesselSessionID"].ToString());
            assR.ReasonOutofServices = CommonClass.OutofServiceList();
            assR.DamageObservedLists = CommonClass.DamagObservedList();
            assR.DamageReasons = CommonClass.DamageReasonList();
            assR.DamageLocations = CommonClass.DamageLocatonList();
            assR.MooringOperationsLists = CommonClass.MooringOpListCommon(VesselID);
            assR.MooringLineDiscardLists = CommonClass.MooringRopeDiscardListCommon(1,VesselID);
            return View(assR);
        }
        [HttpPost]
        [PreventSpam("addTaildiscard", 3, 1)]
        public ActionResult addlinediscard(MooringRopeDetail rpdiscard)
        {
            int VesselID = Convert.ToInt32(Session["VesselSessionID"].ToString());
            int vesselid = Convert.ToInt32(VesselID);

            SqlDataAdapter adp = new SqlDataAdapter("update MooringRopeDetail set DamageObserved='" + rpdiscard.DamageObserved + "',MooringOperationID='" + rpdiscard.MooringOperationID + "',OtherReason='" + rpdiscard.OtherReason + "',ReasonOutofService='" + rpdiscard.ReasonOutofService + "',OutofServiceDate='" + rpdiscard.OutofServiceDate + "' where RopeId=" + rpdiscard.RopeId + " and VesselID=" + vesselid + "", con);
            DataTable dt = new DataTable();
            adp.Fill(dt);

            SqlDataAdapter adp1 = new SqlDataAdapter("update AssignRopeToWinch set IsActive='False' where RopeId=" + rpdiscard.RopeId + " and VesselID=" + vesselid + " and IsActive='True'", con);
            DataTable dt1 = new DataTable();
            adp1.Fill(dt1);


            try
            {
                var IdPK1 = ((from asn in context.RopeDamageRecords.Where(x => x.VesselID == vesselid) select (int?)asn.Id).Max() ?? 0) + 1;

                RopeDamageRecord rpcrp = new RopeDamageRecord();
                int notiid1 = CommonClass.NextNotiId(vesselid);
                rpcrp.RopeId = rpdiscard.RopeId;

                rpcrp.Id = IdPK1;
                rpcrp.CreatedDate = DateTime.Now;
                rpcrp.CreatedBy = "Admin";
                rpcrp.IsActive = true;
                rpcrp.DamageDate = rpdiscard.OutofServiceDate;
                rpcrp.WinchId = winchid;
                if (rpcrp.DamageObserved == "Mooring Operation")
                {
                    rpcrp.MOPId = rpdiscard.MooringOperationID;
                }
                else
                {
                    rpcrp.MOPId = null;
                }
                rpcrp.DamageLocation = rpdiscard.DamageLocation;
                rpcrp.DamageReason = rpdiscard.DamageReason;
                rpcrp.DamageObserved = rpdiscard.DamageObserved;
                rpcrp.IncidentReport = rpdiscard.IncidentReport;
                rpcrp.RopeTail = 1;

                rpcrp.VesselID = vesselid;
                rpcrp.NotificationId = notiid1;

                context.RopeDamageRecords.Add(rpcrp);
                context.SaveChanges();


                // ========= Notification Info ================
                string ExceptionMsg = "";
                var mrRope = context.MooringRopeDetails.Where(x => x.DeleteStatus == false & x.RopeId == rpdiscard.RopeId && x.VesselID == vesselid).FirstOrDefault();
                // var winchid = context.AssignRopeToWinches.Where(x => x.RopeId == rpsplicing.RopeId && x.VesselID == vesselid && x.IsActive == true).Select(x => x.WinchId).SingleOrDefault();
                var ropename = mrRope.CertificateNumber;
                var uniqueid = mrRope.UniqueID;
                var winchDetail = context.MooringWinchDetails.Where(x => x.Id == winchid && x.VesselID == vesselid && x.IsActive == true).FirstOrDefault();
                string WinchName = ""; string WinchLocation = " (Not Assigned)";
                if (winchDetail != null)
                {
                    WinchName = winchDetail.AssignedNumber;
                    WinchLocation = winchDetail.Location;
                }

                // ========= Notification Info ================

                var notification = "";
                if (WinchName != null && WinchName != "" && WinchName != "Not Assigned")
                {
                    notification = "Out of Service / discarded - RopeTail " + ropename + " on winch " + WinchName + " located at " + WinchLocation + "";
                }
                else
                {
                    notification = "Out of Service / discarded - RopeTail " + ropename + "";
                }
                CommonClass.SaveNotification(vesselid, mrRope.RopeId, notification, (int)NotificationAlertType.OutofService_discarded_Tail);


                using (SqlDataAdapter adpdis = new SqlDataAdapter("select NotificationType from tblNotification where RopeId=" + rpdiscard.RopeId + "", con))
                {
                    DataTable dtdis = new DataTable();
                    adpdis.Fill(dtdis);
                    for (int i = 0; i < dtdis.Rows.Count; i++)
                    {
                        int gg = Convert.ToInt32(dtdis.Rows[i][0]);

                        int[] RopesNTails = { 5, 6, 7, 8, 9, 95, 10, 11, 12 };
                        if (RopesNTails.Contains(gg) == true)
                        {
                            using (SqlDataAdapter adp17 = new SqlDataAdapter("update tblNotification set IsActive='false' where NotificationType=" + gg + "", con))
                            {
                                DataTable dt17 = new DataTable();
                                adp17.Fill(dt17);
                            }
                        }
                    }
                }
            }
            catch { }


            TempData["Success"] = "Record successfully saved !";
            return RedirectToAction("Index");
        }

        public JsonResult alldatecheck(string dtvalue, int RopeId)
        {
            int VesselID = Convert.ToInt32(Session["VesselSessionID"].ToString());
            var data = context.MooringRopeDetails.Where(x => x.Id == RopeId && x.DeleteStatus == false && x.VesselID == VesselID).Select(x => x.InstalledDate).SingleOrDefault();
            var data1 = context.AssignRopeToWinches.Where(x => x.RopeId == RopeId && x.IsDelete == false && x.VesselID == VesselID).Select(x => x.AssignedDate).Take(1).SingleOrDefault();
            var data2 = context.RopeCroppings.Where(x => x.RopeId == RopeId && x.IsActive == true && x.VesselID == VesselID).Select(x => x.CroppedDate).Take(1).SingleOrDefault();
            var data3 = context.RopeSplicingRecords.Where(x => x.RopeId == RopeId && x.IsActive == true && x.VesselID == VesselID).Select(x => x.SplicingDoneDate).Take(1).SingleOrDefault();
            //var data4 = context.RopeEndtoEnd2.Where(x => x.RopeId == RopeId && x.IsActive == true && x.VesselID == VesselID).Select(x => x.EndtoEndDoneDate).Take(1).SingleOrDefault();
            var data5 = context.RopeDamageRecords.Where(x => x.RopeId == RopeId && x.IsActive == true && x.VesselID == VesselID).Select(x => x.DamageDate).Take(1).SingleOrDefault();
            var data6 = context.MOUsedWinchTbls.Where(x => x.RopeId == RopeId && x.VesselID == VesselID).Select(x => x.OPDateFrom).Max();
            var data7 = context.MooringRopeInspections.Where(x => x.RopeId == RopeId && x.IsActive == true && x.VesselID == VesselID).Select(x => x.InspectDate).Take(1).SingleOrDefault();


            var insdt = data;
            var insdt1 = data1;
            var insdt2 = data2;
            var insdt3 = data3;
            //var insdt4 = data4;
            var insdt5 = data5;
            var insdt6 = data6;
            var insdt7 = data7;
            string msg = "";

            var assigndate = dtvalue;
            //int checkdt = 0;

            if (Convert.ToDateTime(assigndate) < insdt)
            {
                msg = "Discarded Date can not less than Line installed date!";
                return Json(new { Result = true, instdate = "", Message = msg }, JsonRequestBehavior.AllowGet);
            }
            if (Convert.ToDateTime(assigndate) < insdt1)
            {
                msg = "Discarded Date can not less than Line assigned date!";
                return Json(new { Result = true, instdate = "", Message = msg }, JsonRequestBehavior.AllowGet);
            }
            if (Convert.ToDateTime(assigndate) < insdt2)
            {
                msg = "Discarded Date can not less than Line cropped date!";
                return Json(new { Result = true, instdate = "", Message = msg }, JsonRequestBehavior.AllowGet);
            }
            if (Convert.ToDateTime(assigndate) < insdt3)
            {
                msg = "Discarded Date can not less than Line spliced date!";
                return Json(new { Result = true, instdate = "", Message = msg }, JsonRequestBehavior.AllowGet);
            }
            //if (Convert.ToDateTime(assigndate) < insdt4)
            //{
            //    msg = "Discarded Date can not less than Line endtoend done date!";
            //    return Json(new { Result = true, instdate = "", Message = msg }, JsonRequestBehavior.AllowGet);
            //}
            if (Convert.ToDateTime(assigndate) < insdt5)
            {
                msg = "Discarded Date can not less than Line damage date!";
                return Json(new { Result = true, instdate = "", Message = msg }, JsonRequestBehavior.AllowGet);
            }
            if (Convert.ToDateTime(assigndate) < insdt6)
            {
                msg = "Discarded Date can not less than Line Mooring operation date!";
                return Json(new { Result = true, instdate = "", Message = msg }, JsonRequestBehavior.AllowGet);
            }
            if (Convert.ToDateTime(assigndate) < insdt7)
            {
                msg = "Discarded Date can not less than Line Mooring inspection date!";
                return Json(new { Result = true, instdate = "", Message = msg }, JsonRequestBehavior.AllowGet);
            }




            return Json(new { Result = true, instdate = "", Message = "" }, JsonRequestBehavior.AllowGet);
        }


        public JsonResult GetWinchlocation(int Id)
        {

            int VesselID = Convert.ToInt32(Session["VesselSessionID"].ToString());
            int vesselid = Convert.ToInt32(VesselID);
         
            var data = context.AssignRopeToWinches.Where(x => x.RopeId == Id && x.IsActive == true && x.VesselID == vesselid).FirstOrDefault();
            if (data != null)
            {
                int? winchidd = data.WinchId;
                winchid = winchidd;
                string outboard = data.Outboard == true ? "A" : "B";
                if (outboard == "A")
                {
                    outboardEndinuse = false;
                }
                if (outboard == "B")
                {
                    outboardEndinuse = true;
                }


                string msgdiv = "This rope is currently assigned to a Winch and is in active list. Please go to Assign to Winch option under Mooring Rope menu and 'Shift to Inactive' list.";

                string location = context.MooringWinchDetails.Where(x => x.Id == data.WinchId && x.IsActive == true && x.VesselID == vesselid).Select(x => x.Location).SingleOrDefault();
                    string outboard1 = data.Outboard == true ? "B" : "A";
                    string asswinch = context.MooringWinchDetails.Where(s => s.Id == data.WinchId && s.IsActive == true && s.VesselID == vesselid).Select(x => x.AssignedNumber).FirstOrDefault();
          

                int opId = context.MOUsedWinchTbls.Where(x => x.RopeId == Id && x.VesselID == vesselid).Select(item => item.OperationID).Distinct().Count();
                string rnghrs = context.MooringRopeDetails.Where(x => x.Id == Id && x.DeleteStatus == false && x.VesselID == vesselid).Select(x => x.CurrentRunningHours).SingleOrDefault().ToString();


                string noofOp = opId.ToString();

                return Json(new { Result = true, outboard = outboard, asswinch = asswinch, location = location, outboard1 = outboard1, noofOp= noofOp, rnghrs= rnghrs,msgdiv = msgdiv }, JsonRequestBehavior.AllowGet);
            }
            else
            {
               
                int opId = context.MOUsedWinchTbls.Where(x => x.RopeId == Id && x.VesselID == vesselid).Select(item => item.OperationID).Distinct().Count();
                string rnghrs = context.MooringRopeDetails.Where(x => x.Id == Id && x.DeleteStatus == false && x.VesselID == vesselid).Select(x => x.CurrentRunningHours).SingleOrDefault().ToString();
                string noofOp = opId.ToString();
                winchid = 0;
                return Json(new { Result = true, outboard = "", asswinch = "", location = "", outboard1 = "", noofOp = noofOp, rnghrs = rnghrs,msgdiv="" }, JsonRequestBehavior.AllowGet);
            }

            //return Json(new { Result = true, outboard = "", asswinch = "", location = "", outboard1 = "", noofOp = "", rnghrs = "" }, JsonRequestBehavior.AllowGet);
        }

        
        public ActionResult Delete(int Id)
        {
            try
            {
                int VesselID = Convert.ToInt32(Session["VesselSessionID"].ToString());
                int vesselid = Convert.ToInt32(VesselID);
                MooringRopeDetail findcrs = context.MooringRopeDetails.Where(x => x.RopeId == Id && x.VesselID == vesselid).FirstOrDefault();


                SqlDataAdapter adp = new SqlDataAdapter("update RopeDamageRecord set IsActive='False' where RopeId =" + Id + " and RopeTail = 1 and VesselID='" + vesselid + "' ", con);
                DataTable dt = new DataTable();
                adp.Fill(dt);

                try
                {
                    SqlDataAdapter adp5 = new SqlDataAdapter("select * from RopeDisposal where IsActive=1 and RopeId=" + findcrs.RopeId + " and VesselId= '" + vesselid + "'", con);
                    DataTable dt5 = new DataTable();
                    adp5.Fill(dt5);
                    if (dt5.Rows.Count > 0)
                    {
                        // MessageBox.Show("You can not delete this record ! Firstly remove from Line Disposal. ", "Mooring Line", MessageBoxButton.OK, MessageBoxImage.Information);
                        TempData["Error"] = "You can not delete this record ! Firstly remove from Tail Disposal";
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        SqlDataAdapter adp15 = new SqlDataAdapter("update mooringropedetail set outofservicedate= null,ModifiedDate='" + DateTime.Now + "' where RopeId =" + findcrs.RopeId + " and VesselID='" + vesselid + "' ", con);
                        DataTable dt15 = new DataTable();
                        adp15.Fill(dt15);

                        int NotificationType = (int)NotificationAlertType.OutofService_discarded_Rope;
                        CommonClass.DeleteNotificationsRopeID(VesselID, findcrs.RopeId, NotificationType);
                    }


                    //var notiid = findcrs.NotificationId;
                    //var notidelete = context.Notifications.Where(x => x.Nid == notiid).FirstOrDefault();
                    //context.Entry(notidelete).State = EntityState.Deleted;
                    //context.SaveChanges();
                }
                catch { }

                TempData["Success"] = "Record successfully deleted !";
            }
            catch { }
            return RedirectToAction("Index");
        }
    }
}