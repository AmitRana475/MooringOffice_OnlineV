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
using static Shipment49Web.Common.CommonClass;

namespace Shipment49Web.Areas.MooringLine
{
    [Authorize]
    [UserAuthenticationFilter]
    [ErrorClass]
    public class LineDamageController : BaseController
    {
        MorringOfficeEntities context = new MorringOfficeEntities();
        SqlConnection con = ConnectionBulder.con;
        //CommonClass cls = new CommonClass();
        RopeDamageRecord assR = new RopeDamageRecord();
        DamageR ss = new DamageR();
        public static int? winchid = 0;
        public static bool outboardEndinuse = true;
       // public static string VesselID;
        // GET: MooringLine/LineDamage

       // public static int VesselID { get; set; }
        public LineDamageController()
        {
           // VesselID = Convert.ToInt32(CommonClass.VesselSessionID);
        }
        public ActionResult Index()
        {
           int VesselID = Convert.ToInt32(Session["VesselSessionID"].ToString());
            using (ShipmentContaxt sc1 = new ShipmentContaxt())
            {
                RopeDamage model = new RopeDamage();
                var ilist = new ShipmentContaxt()
                     .MultipleResults("[dbo].[GetDamageRope]  0,'" + VesselID + "'")
                   .With<RopeDamage>()
                   .Execute();
                model.RopeDamageList = (List<RopeDamage>)ilist[0];
                return View(model);
            }
        }

        public ActionResult addlinedamage()
        {
            int VesselID = Convert.ToInt32(Session["VesselSessionID"].ToString());
            assR.MooringLineLists = CommonClass.MooringRopeDetailListCommon(0,VesselID);
            assR.DamageReasons = CommonClass.DamageReasonList();
            assR.DamageLocations = CommonClass.DamageLocatonList();
            assR.DamageObservedLists = CommonClass.DamagObservedList();
            assR.MooringOperationsLists = CommonClass.MooringOpListCommon(VesselID);
            return View(assR);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [PreventSpam("addlinedamage", 3, 1)]
        public ActionResult addlinedamage(RopeDamageRecord rpdamage)
        {
            int VesselID = Convert.ToInt32(Session["VesselSessionID"].ToString());
            int vesselid = Convert.ToInt32(VesselID);

            var IdPK1 = ((from asn in context.RopeDamageRecords.Where(x => x.VesselID == vesselid) select (int?)asn.Id).Max() ?? 0) + 1;
           
            RopeDamageRecord rpcrp = new RopeDamageRecord();
            int notiid1 = CommonClass.NextNotiId(vesselid);
            rpcrp.RopeId = rpdamage.RopeId;
          
            rpcrp.Id = IdPK1;
            rpcrp.CreatedDate = DateTime.Now;
            rpcrp.CreatedBy = "Admin";
            rpcrp.IsActive = true;
            rpcrp.DamageDate = rpdamage.DamageDate;
            rpcrp.WinchId = winchid;
            if (rpdamage.DamageObserved == "Mooring Operation")
            {
                rpcrp.MOPId = rpdamage.MOPId;
            }
            else
            {
                rpcrp.MOPId = null;
            }
            if (rpcrp.MOPId != null)
            {
                var dateck = context.MOperationBirthDetails.Where(x => x.OPId == rpcrp.MOPId && x.IsActive == true && x.VesselID == vesselid).FirstOrDefault();

                var dt1 = dateck.FastDatetime;
                var dt2 = dateck.CastDatetime;

                //DateTime ss = Convert.ToDateTime( dt1.ToString("yyyy-mm-dd"));

                if (rpdamage.DamageDate >= dt1 && rpdamage.DamageDate <= dt2)
                {
                    //string ss = "";
                }
                else
                {
                    TempData["Error"] = "Please insert damage date between Mooring Opertaion fastdate and castdate !";

                    rpdamage.MooringLineLists = CommonClass.MooringRopeDetailListCommon(0,vesselid);
                    rpdamage.DamageReasons = CommonClass.DamageReasonList();
                    rpdamage.DamageLocations = CommonClass.DamageLocatonList();
                    rpdamage.DamageObservedLists = CommonClass.DamagObservedList();
                    rpdamage.MooringOperationsLists = CommonClass.MooringOpListCommon(vesselid);
                    return View(rpdamage);
                }

            }

            rpcrp.DamageLocation = rpdamage.DamageLocation;
            rpcrp.DamageReason = rpdamage.DamageReason;
            rpcrp.DamageObserved = rpdamage.DamageObserved;
            rpcrp.IncidentReport = rpdamage.IncidentReport;
            rpcrp.RopeTail = 0;

            rpcrp.VesselID = vesselid;
            rpcrp.NotificationId = notiid1;

            context.RopeDamageRecords.Add(rpcrp);
            context.SaveChanges();


            // ========= Notification Info ================
            //string ExceptionMsg = "";
            var mrRope = context.MooringRopeDetails.Where(x => x.DeleteStatus == false & x.RopeId == rpcrp.RopeId && x.VesselID == vesselid).FirstOrDefault();
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

                notification = "Damaged - Line " + ropename + " on winch " + WinchName + " located at " + WinchLocation + "";
            }
            else
            {
                notification = "Damaged - Line " + ropename + "";
            }
            CommonClass.SaveNotification(vesselid, mrRope.RopeId, notification, (int)NotificationAlertType.RopeDamage);

            TempData["Success"] = "Record successfully saved !";
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

        public ActionResult BindingSubDates(int OpID)
        {
            int VesselID = Convert.ToInt32(Session["VesselSessionID"].ToString());
            var MoorRecod = context.MOperationBirthDetails.Where(x => x.OPId == OpID && x.VesselID == VesselID && x.IsActive == true).FirstOrDefault();
            List<SelectListItem> jst = new List<SelectListItem>();
            DateTime fd = Convert.ToDateTime(MoorRecod.FastDatetime);  // Convert.ToDateTime(to);
            DateTime cd = Convert.ToDateTime(MoorRecod.CastDatetime); //Convert.ToDateTime(Frm);
            var Day_Diff = (int)(cd - fd).TotalDays;

            //subDates.Clear();
            //OnPropertyChanged(new PropertyChangedEventArgs("SubDates"));
            for (int i = 0; i < Day_Diff + 1; i++)
            {
                // DateTime d1 = fd.AddDays(i);
                // var dd = d1.AddDays(i);

                //subDates.Add(fd.AddDays(i).ToShortDateString());
                jst.Add(new SelectListItem() { Text = fd.AddDays(i).ToShortDateString(), Value = fd.AddDays(i).ToShortDateString() });
            }

            //return Json(db.Departments.Select(x => new
            //{
            //    DepartmentID = x.DepartmentID,
            //    DepartmentName = x.DepartmentName
            //}).ToList(), JsonRequestBehavior.AllowGet);
            //FastDate = subDates.FirstOrDefault();

            return Json( jst.ToList(),JsonRequestBehavior.AllowGet);
        }
        public ActionResult Delete(int Id)
        {
            try
            {
                int VesselID = Convert.ToInt32(Session["VesselSessionID"].ToString());
                int vesselid = Convert.ToInt32(VesselID);
                RopeDamageRecord findcrs = context.RopeDamageRecords.Where(x => x.Id == Id && x.VesselID==vesselid ).FirstOrDefault();

                
                SqlDataAdapter adp = new SqlDataAdapter("update RopeDamageRecord set IsActive='False' where Id =" + Id + " and RopeTail = 0 and VesselID='" + vesselid + "' ", con);
                DataTable dt = new DataTable();
                adp.Fill(dt);

                int NotificationType = (int)NotificationAlertType.RopeDamage;
                CommonClass.DeleteNotificationsRopeID(VesselID, findcrs.RopeId, NotificationType);

                try
                {
                    //SqlDataAdapter adp1 = new SqlDataAdapter("update RopeCropping set IsActive='False' where SplicedId =" + Id + " and RopeTail = 0 and VesselID='" + vesselid + "' ", con);
                    //DataTable dt1 = new DataTable();
                    //adp1.Fill(dt1);


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