using MenuLayer;
using Reports;
using Shipment49Web.Areas.MooringLine.Models;
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

namespace Shipment49Web.Areas.MooringLine.Controllers
{
    [Authorize]
    [UserAuthenticationFilter]
    [ErrorClass]
    public class LineSplicingController : BaseController
    {
        MorringOfficeEntities context = new MorringOfficeEntities();
        SqlConnection con = ConnectionBulder.con;
        //CommonClass cls = new CommonClass();
        RopeSplicingRecord assR = new RopeSplicingRecord();
        public static int? winchid = 0;
        public static bool outboardEndinuse = true;
        // public static string VesselID;
        // GET: MooringLine/LineSplicing

        //public static int VesselID { get; set; }
      
        public ActionResult Index()
        {
           int VesselID = Convert.ToInt32(Session["VesselSessionID"].ToString());
            using (ShipmentContaxt sc1 = new ShipmentContaxt())
            {
                RopeSplicing model = new RopeSplicing();
                var ilist = new ShipmentContaxt()
                     .MultipleResults("[dbo].[GetRopeSplicing]  0,'" + VesselID + "'")
                   .With<RopeSplicing>()
                   .Execute();
                model.RopeSplicingList = (List<RopeSplicing>)ilist[0];
                return View(model);
            }
        }

        public ActionResult addlinesplicing()
        {
            int VesselID = Convert.ToInt32(Session["VesselSessionID"].ToString());
            assR.ReasonofCroppings = CommonClass.CroppingReasonList();
            assR.MooringLineLists = CommonClass.MooringRopeDetailListCommon(0,VesselID);
            return View(assR);
        }

        [HttpPost]
        [PreventSpam("addlinesplicing", 3, 1)]
        public ActionResult addlinesplicing(RopeSplicingRecord rpsplicing)
        {
            //assR.MooringLineLists = cls.MooringRopeDetailListCommon();
            //return View(assR);

            int VesselID = Convert.ToInt32(Session["VesselSessionID"].ToString());

            int vesselid = Convert.ToInt32(VesselID);

            var IdPK = ((from asn in context.RopeSplicingRecords.Where(x => x.VesselID == vesselid) select (int?)asn.Id).Max() ?? 0) + 1;


            int notiid = CommonClass.NextNotiId(vesselid);


            rpsplicing.SplicingDoneBy = rpsplicing.SplicingDoneBy;
            rpsplicing.SplicingDoneDate = rpsplicing.SplicingDoneDate;
            rpsplicing.SplicingMethod = rpsplicing.SplicingMethod;
            rpsplicing.Id = IdPK;
            rpsplicing.VesselID = vesselid;
            rpsplicing.WinchId = winchid;
            rpsplicing.RopeTail = 0;
            rpsplicing.RopeId = rpsplicing.RopeId;
            rpsplicing.CreatedDate = DateTime.Now;
            rpsplicing.CreatedBy = "Admin";
            rpsplicing.IsActive = true;
            rpsplicing.NotificationId = notiid;


            // ========= Notification Info ================
            string ExceptionMsg = "";
            var mrRope = context.MooringRopeDetails.Where(x => x.DeleteStatus == false & x.RopeId == rpsplicing.RopeId && x.VesselID == vesselid).FirstOrDefault();
            // var winchid = context.AssignRopeToWinches.Where(x => x.RopeId == rpsplicing.RopeId && x.VesselID == vesselid && x.IsActive == true).Select(x => x.WinchId).SingleOrDefault();
            //var length = mrRope.Where(x => x.Id == rpspc.RopeId).Select(x => x.Length).SingleOrDefault();
            var length = mrRope.Length;
            var percent = (length * 10) / 100;
            var crplength = context.RopeCroppings.Where(x => x.RopeId == rpsplicing.RopeId && x.VesselID == vesselid && x.IsActive == true).Select(x => x.LengthofCroppedRope).Sum();


            var ropetypeid = mrRope.RopeTypeId;

            var manufacid = mrRope.ManufacturerId;

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

            if (rpsplicing.IsLineCropped == "Yes")
            {
                if (rpsplicing.RsCropping == null)
                {
                    TempData["Error"] = "Reason of cropping can not be null !";
                    rpsplicing.ReasonofCroppings = CommonClass.CroppingReasonList();
                    rpsplicing.MooringLineLists = CommonClass.MooringRopeDetailListCommon(0,vesselid);
                    return View(rpsplicing);
                }
                else if (rpsplicing.CroppingLength == 0)
                {
                    TempData["Error"] = "Cropping length can not be null !";
                    rpsplicing.ReasonofCroppings = CommonClass.CroppingReasonList();
                    rpsplicing.MooringLineLists = CommonClass.MooringRopeDetailListCommon(0,vesselid);
                    return View(rpsplicing);
                }
                else
                {
                    foreach (var vid in rpsplicing.RsCropping)
                        rpsplicing.ReasonofCropping = vid + "," + rpsplicing.ReasonofCropping;

                    var IdPK1 = ((from asn in context.RopeCroppings.Where(x => x.VesselID == vesselid) select (int?)asn.Id).Max() ?? 0) + 1;
                    var maxSplicedId = context.RopeSplicingRecords.Where(x => x.VesselID == vesselid).DefaultIfEmpty().Max(r => r == null ? 1 : r.Id);
                    //var maxSplicedId = sc.RopeSplicing.Select(x => x.Id).Max();
                    RopeCropping rpcrp = new RopeCropping();
                    int notiid1 = CommonClass.NextNotiId(VesselID);
                    rpcrp.RopeId = rpsplicing.RopeId;
                    if (rpsplicing.Outboard == true)
                    {
                        rpcrp.CroppedOutboardEnd = "A";
                    }
                    if (rpsplicing.Outboard == false)
                    {
                        rpcrp.CroppedOutboardEnd = "B";
                    }
                    rpcrp.Id = IdPK1;
                    rpcrp.CreatedDate = DateTime.Now;
                    rpcrp.CreatedBy = "Admin";
                    rpcrp.IsActive = true;
                    rpcrp.CroppedDate = rpsplicing.SplicingDoneDate;
                    rpcrp.WinchId = winchid;
                    rpcrp.LengthofCroppedRope = rpsplicing.CroppingLength;
                    rpcrp.ReasonofCropping = rpsplicing.ReasonofCropping.TrimEnd(',');
                    rpcrp.RopeTail = 0;
                    rpcrp.SplicedId = maxSplicedId;
                    rpcrp.NotificationId = notiid1;
                    rpcrp.VesselID = vesselid;

                    context.RopeCroppings.Add(rpcrp);
                    context.SaveChanges();

                   
                    try
                    {

                        if (crplength >= percent)
                        {
                            var notificationcrop = "";
                            if (!string.IsNullOrEmpty(WinchName))
                            {

                                notificationcrop = "Cropped more than 10% - Line " + ropename + " - " + uniqueid + " on winch " + WinchName + " located at " + WinchLocation + "";
                            }
                            else
                            {
                                notificationcrop = "Cropped more than 10% - Line " + ropename + " - " + uniqueid + "";
                            }
                            CommonClass.SaveNotification(vesselid, mrRope.RopeId, notificationcrop, (int)NotificationAlertType.Over_Cropping);

                        }
                    }
                    catch (Exception es)
                    {
                        ExceptionMsg = es.Message;
                    }

                    if (string.IsNullOrEmpty(ExceptionMsg))
                    {
                        TempData["Success"] = "Record successfully saved !";
                    }
                    else
                    {
                        TempData["Error"] = ExceptionMsg;
                    }

                }
            }


            context.RopeSplicingRecords.Add(rpsplicing);
            context.SaveChanges();


            var notification = "";
            if (WinchName != null && WinchName != "" && WinchName != "Not Assigned")
            {

                notification = "Spliced - Line " + ropename + " on winch " + WinchName + " located at " + WinchLocation + "";
            }
            else
            {
                notification = "Spliced - Line " + ropename + "";
            }
            CommonClass.SaveNotification(vesselid, mrRope.RopeId, notification, (int)NotificationAlertType.RopeSplicing);

            if (string.IsNullOrEmpty(ExceptionMsg))
            {
                TempData["Success"] = "Record successfully saved !";
            }
            else
            {
                TempData["Error"] = ExceptionMsg;
            }
            return RedirectToAction("Index");
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

                string location = context.MooringWinchDetails.Where(x => x.Id == data.WinchId && x.IsActive == true && x.VesselID == vesselid).Select(x => x.Location).SingleOrDefault();
                string outboard1 = data.Outboard == true ? "B" : "A";
                string asswinch = context.MooringWinchDetails.Where(s => s.Id == data.WinchId && s.IsActive == true && s.VesselID == vesselid).Select(x => x.AssignedNumber).FirstOrDefault();
                return Json(new { Result = true, outboard = outboard, asswinch = asswinch, location = location, outboard1 = outboard1, }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                winchid = 0;
            }

            return Json(new { Result = true, outboard = "", asswinch = "", location = "", outboard1 = "", }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult Installdatecheck(string dtvalue, int RopeId)
        {
            int VesselID = Convert.ToInt32(Session["VesselSessionID"].ToString());
            var data = context.MooringRopeDetails.Where(x => x.Id == RopeId && x.DeleteStatus == false && x.VesselID == VesselID).Select(x => x.InstalledDate).SingleOrDefault();

            var installeddate = data;

            var assigndate = dtvalue;

            if (Convert.ToDateTime(assigndate) < installeddate)
            {
                string msg = "Splicing Date can not less than Line Installed date!";

                string instdt1 = installeddate.ToString();
                DateTime dateAndTime = Convert.ToDateTime(instdt1.ToString());
                string instdt = dateAndTime.ToString("yyyy-MM-dd");


                return Json(new { Result = true, instdate = instdt, Message = msg }, JsonRequestBehavior.AllowGet);
            }



            return Json(new { Result = true, instdate = "", Message = "" }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult CheckCroppingLength(int crpval, int RopeId)
        {

            int VesselID = Convert.ToInt32(Session["VesselSessionID"].ToString());
            int vesselid = Convert.ToInt32(VesselID);

            string msg = "";
            SqlDataAdapter adp = new SqlDataAdapter("select Length from MooringRopeDetail where RopeId=" + RopeId + " and VesselId ='" + vesselid + "'", con);
            DataTable dt = new DataTable();
            adp.Fill(dt);

            decimal ss1 = Convert.ToDecimal(dt.Rows[0][0]);


            decimal check = Convert.ToDecimal(crpval);

            if (check > ss1)
            {
                msg = "You cannot crop the line beyond its total length of " + ss1 + " mtrs  !";
                // MessageBox.Show("You cannot crop the line beyond its total length of " + ss1 + " mtrs  !", "Line Splicing", MessageBoxButton.OK, MessageBoxImage.Warning);

            }

            //int check = Convert.ToInt32(txtCroppedLength.Text);
            if (check > 999)
            {
                msg = "You can not Enter Max 3 Digit";
                // MessageBox.Show("You can not Enter Max 3 Digit", "Character Length", MessageBoxButton.OK, MessageBoxImage.Warning);

            }

            SqlDataAdapter adp1 = new SqlDataAdapter("select SUM(lengthofcroppedRope) as lengthofcroppedRope from RopeCropping where  RopeId= " + RopeId + " and VesselId ='" + vesselid + "'", con);
            DataTable dt1 = new DataTable();
            adp1.Fill(dt1);

            if (dt1.Rows.Count > 0 && dt1.Rows[0][0] != DBNull.Value)
            {
                decimal totalsum = Convert.ToDecimal(dt1.Rows[0][0]);

                decimal sumtotal = Convert.ToDecimal(dt1.Rows[0][0]) + check;

                if (sumtotal > ss1)
                {
                    msg = "You cannot crop the line beyond its total length of " + ss1 + " mtrs  !";
                    //  MessageBox.Show("You cannot crop the line beyond its total length of " + ss1 + " mtrs  !", "Line Splicing", MessageBoxButton.OK, MessageBoxImage.Warning);

                }
            }



            return Json(new { Result = true, Message = msg }, JsonRequestBehavior.AllowGet);
        }

        
        public ActionResult Delete(int Id)
        {
            try
            {
                int VesselID = Convert.ToInt32(Session["VesselSessionID"].ToString());
                int vesselid = Convert.ToInt32(VesselID);
                RopeSplicingRecord findcrs = context.RopeSplicingRecords.Where(x => x.Id == Id && x.VesselID == vesselid).FirstOrDefault();


                SqlDataAdapter adp = new SqlDataAdapter("update RopeSplicingRecord set IsActive='False' where Id =" + Id + " and RopeTail = 0 and VesselID='" + vesselid + "' ", con);
                DataTable dt = new DataTable();
                adp.Fill(dt);

                int NotificationType = (int)NotificationAlertType.RopeSplicing;
                CommonClass.DeleteNotificationsRopeID(VesselID, findcrs.RopeId, NotificationType);

                

                try
                {
                    SqlDataAdapter adp1 = new SqlDataAdapter("update RopeCropping set IsActive='False' where SplicedId =" + Id + " and RopeTail = 0 and VesselID='" + vesselid + "' ", con);
                    DataTable dt1 = new DataTable();
                    adp1.Fill(dt1);



                   // RopeCropping findcrs1 = context.RopeCroppings.Where(x => x.SplicedId == Id && x.VesselID == vesselid).FirstOrDefault();

                    int NotificationType1 = (int)NotificationAlertType.Over_Cropping;
                    CommonClass.DeleteNotificationsRopeID(VesselID, findcrs.RopeId, NotificationType1);

                    //var notiid = findcrs.NotificationId;
                    //var notidelete = context.Notifications.Where(x => x.Nid == notiid).FirstOrDefault();
                    //context.Entry(notidelete).State = EntityState.Deleted;
                    //context.SaveChanges();
                }
                catch (Exception ex) { }

                TempData["Success"] = "Record successfully deleted !";
            }
            catch { }
            return RedirectToAction("Index");
        }
    }
}