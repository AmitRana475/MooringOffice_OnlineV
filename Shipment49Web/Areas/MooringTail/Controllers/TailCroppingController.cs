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
    public class TailCroppingController : BaseController
    {
        MorringOfficeEntities context = new MorringOfficeEntities();
        SqlConnection con = ConnectionBulder.con;
        //CommonClass cls = new CommonClass();
        RopeCropping assR = new RopeCropping();
        public static int? winchid = 0;
        public static bool outboardEndinuse = true;
        //public static string VesselID;
        // GET: MooringLine/LineCropping

       // public static int VesselID { get; set; }
        public TailCroppingController()
        {
            //VesselID = Convert.ToInt32(CommonClass.VesselSessionID);
        }
        public ActionResult Index()
        {
           int VesselID = Convert.ToInt32(Session["VesselSessionID"].ToString());
            using (ShipmentContaxt sc1 = new ShipmentContaxt())
            {
                RopeCroppingClass model = new RopeCroppingClass();
                var ilist = new ShipmentContaxt()
                     .MultipleResults("[dbo].[GetRopeCropping]  1,'" + VesselID + "'")
                   .With<RopeCroppingClass>()
                   .Execute();
                model.RopeCroppingList = (List<RopeCroppingClass>)ilist[0];
                return View(model);
            }
        }

        public ActionResult addlinecropping()
        {
            int VesselID = Convert.ToInt32(Session["VesselSessionID"].ToString());
            assR.ReasonofCroppings = CommonClass.CroppingReasonList();
            //assR.ReasonofCroppings = CommonClass.GetCroppingReasonsList();
            assR.MooringLineLists = CommonClass.MooringRopeDetailListCommon(1,VesselID);
            return View(assR);
        }

        [HttpPost]
        [PreventSpam("addTailcropping", 3, 1)]
        public ActionResult addlinecropping(RopeCropping rpscrpping)
        {
            //assR.MooringLineLists = cls.MooringRopeDetailListCommon();
            //return View(assR);
            int VesselID = Convert.ToInt32(Session["VesselSessionID"].ToString());
            foreach (var vid in rpscrpping.RsCropping)
                rpscrpping.ReasonofCropping = vid + "," + rpscrpping.ReasonofCropping;

            int vesselid = Convert.ToInt32(VesselID);

            var IdPK1 = ((from asn in context.RopeCroppings.Where(x => x.VesselID==vesselid) select (int?)asn.Id).Max() ?? 0) + 1;
            var maxSplicedId = context.RopeSplicingRecords.Where(x => x.VesselID == vesselid).DefaultIfEmpty().Max(r => r == null ? 1 : r.Id);
            //var maxSplicedId = sc.RopeSplicing.Select(x => x.Id).Max();
            RopeCropping rpcrp = new RopeCropping();
            int notiid1 = CommonClass.NextNotiId(vesselid);
            rpcrp.RopeId = rpscrpping.RopeId;
            if (rpscrpping.Outboard == true)
            {
                rpcrp.CroppedOutboardEnd = "A";
            }
            if (rpscrpping.Outboard == false)
            {
                rpcrp.CroppedOutboardEnd = "B";
            }
            rpcrp.Id = IdPK1;
            rpcrp.CreatedDate = DateTime.Now;
            rpcrp.CreatedBy = "Admin";
            rpcrp.IsActive = true;
            rpcrp.CroppedDate = rpscrpping.CroppedDate;
            rpcrp.WinchId = winchid;
            rpcrp.LengthofCroppedRope = rpscrpping.LengthofCroppedRope;
            rpcrp.ReasonofCropping = rpscrpping.ReasonofCropping.TrimEnd(',');
            rpcrp.RopeTail = 1;

            rpcrp.VesselID = vesselid;
            rpcrp.NotificationId = notiid1;

            context.RopeCroppings.Add(rpcrp);
            context.SaveChanges();

            string ExceptionMsg = "";
            try
            {
                var mrRope = context.MooringRopeDetails.Where(x => x.DeleteStatus == false & x.RopeId == rpscrpping.RopeId && x.VesselID == vesselid).FirstOrDefault();
                var winchid = context.AssignRopeToWinches.Where(x => x.RopeId == rpscrpping.RopeId && x.VesselID == vesselid && x.IsActive == true).Select(x => x.WinchId).SingleOrDefault();
                //var length = mrRope.Where(x => x.Id == rpspc.RopeId).Select(x => x.Length).SingleOrDefault();
                var length = mrRope.Length;
                var percent = (length * 10) / 100;
                var crplength = context.RopeCroppings.Where(x => x.RopeId == rpscrpping.RopeId && x.VesselID == vesselid && x.IsActive == true).Select(x => x.LengthofCroppedRope).Sum();


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



                if (crplength >= percent)
                {
                    var notification = "";
                    if (!string.IsNullOrEmpty(WinchName))
                    {

                        notification = "Cropped more than 10% - RopeTail " + ropename + " - " + uniqueid + " on winch " + WinchName + " located at " + WinchLocation + "";
                    }
                    else
                    {
                        notification = "Cropped more than 10% - RopeTail " + ropename + " - " + uniqueid + "";
                    }
                    CommonClass.SaveNotification(vesselid, mrRope.RopeId, notification, (int)NotificationAlertType.Over_Cropping);

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
                string ttlcrpdrope = "";
                string location = context.MooringWinchDetails.Where(x => x.Id == data.WinchId && x.IsActive == true && x.VesselID == vesselid).Select(x => x.Location).SingleOrDefault();
                string outboard1 = data.Outboard == true ? "B" : "A";
                string asswinch = context.MooringWinchDetails.Where(s => s.Id == data.WinchId && s.IsActive == true && s.VesselID == vesselid).Select(x => x.AssignedNumber).FirstOrDefault();

                var crplength = context.RopeCroppings.Where(x => x.RopeId == Id && x.IsActive == true && x.VesselID == vesselid).Select(x => x.LengthofCroppedRope).Sum();

                if (crplength != null)
                {
                    ttlcrpdrope = crplength.ToString();
                }


                return Json(new { Result = true, outboard = outboard, asswinch = asswinch, location = location, outboard1 = outboard1, ttlcrpd = ttlcrpdrope }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                winchid = 0;
            }

            return Json(new { Result = true, outboard = "", asswinch = "", location = "", outboard1 = "", ttlcrpd = "" }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult Installdatecheck(string dtvalue, int RopeId)
        {
            int VesselID = Convert.ToInt32(Session["VesselSessionID"].ToString());
            var data = context.MooringRopeDetails.Where(x => x.Id == RopeId && x.DeleteStatus == false && x.VesselID == VesselID).Select(x => x.InstalledDate).SingleOrDefault();

            var installeddate = data;

            var assigndate = dtvalue;

            if (Convert.ToDateTime(assigndate) < installeddate)
            {
                string msg = "Cropping Date can not less than RopeTail Installed date!";

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
                msg = "You cannot crop the tail beyond its total length of " + ss1 + " mtrs  !";
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
                    msg = "You cannot crop the tail beyond its total length of " + ss1 + " mtrs  !";
                    //  MessageBox.Show("You cannot crop the line beyond its total length of " + ss1 + " mtrs  !", "Line Splicing", MessageBoxButton.OK, MessageBoxImage.Warning);

                }
            }



            return Json(new { Result = true, Message = msg }, JsonRequestBehavior.AllowGet);
        }

        //[PreventSpam("DeleteTailcropping", 3, 1)]
        public ActionResult Delete(int Id)
        {
            int VesselID = Convert.ToInt32(Session["VesselSessionID"].ToString());
            int vesselid = Convert.ToInt32(VesselID);
            try
            {
                RopeCropping findcrs = context.RopeCroppings.Where(x => x.Id == Id && x.VesselID == vesselid).FirstOrDefault();

                int NotificationType = (int)NotificationAlertType.Over_Cropping;
                CommonClass.DeleteNotificationsRopeID(VesselID, findcrs.RopeId, NotificationType);



                try
                {
                    SqlDataAdapter adp1 = new SqlDataAdapter("update RopeCropping set IsActive='False' where Id =" + Id + " and RopeTail = 1 and VesselID='" + vesselid + "' ", con);
                    DataTable dt1 = new DataTable();
                    adp1.Fill(dt1);


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