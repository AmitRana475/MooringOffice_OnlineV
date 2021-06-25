using MenuLayer;
using Reports;
using Shipment49Web.Areas.ResidualLabTests.Models;
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
using System.IO;
using System.Web;

namespace Shipment49Web.Areas.ResidualLabTests.Controllers
{
    [Authorize]
    [UserAuthenticationFilter]
    [ErrorClass]
    public class ReslabTestController : BaseController
    {
        MorringOfficeEntities context = new MorringOfficeEntities();
        SqlConnection con = ConnectionBulder.con;
        //CommonClass cls = new CommonClass();
        //ResidualLabTests assR = new ResidualLabTest();
        ResidualLabTest assR = new ResidualLabTest();
        DamageR ss = new DamageR();
        public static int? winchid = 0;
        public static string instdt = "";
       
        // GET: ResidualLabTest/ReslabTest
        public ActionResult Index(int? page)
        {
           string VesselID = Session["VesselSessionID"].ToString();
            using (ShipmentContaxt sc1 = new ShipmentContaxt())
            {
                ResidualLabTestClass model = new ResidualLabTestClass();
                var ilist = new ShipmentContaxt()
                     .MultipleResults("[dbo].[ResidualList]  0,'" + VesselID + "','ResidualLine'")
                   .With<ResidualLabTestClass>()
                   .Execute();
                model.ResidualLabTestList = (List<ResidualLabTestClass>)ilist[0];

                var record = model.ResidualLabTestList.Count();
                int currPage = page == null ? 1 : Convert.ToInt32(page);
                TempData["CurrentPage"] = currPage;
                TempData["TotalRecords"] = record;
                model.ResidualLabTestList = model.ResidualLabTestList.Skip((CommonMethods.PAGESIZE * currPage) - CommonMethods.PAGESIZE).Take(CommonMethods.PAGESIZE).ToList();


                return View(model);
            }
        }

        public ActionResult tailreslablist(int? page)
        {
          string  VesselID = Session["VesselSessionID"].ToString();
            using (ShipmentContaxt sc1 = new ShipmentContaxt())
            {
                ResidualLabTestClass model = new ResidualLabTestClass();
                var ilist = new ShipmentContaxt()
                     .MultipleResults("[dbo].[ResidualList]  1,'" + VesselID + "','ResidualRopeTail'")
                   .With<ResidualLabTestClass>()
                   .Execute();
                model.ResidualLabTestList = (List<ResidualLabTestClass>)ilist[0];

                var record = model.ResidualLabTestList.Count();
                int currPage = page == null ? 1 : Convert.ToInt32(page);
                TempData["CurrentPage"] = currPage;
                TempData["TotalRecords"] = record;
                model.ResidualLabTestList = model.ResidualLabTestList.Skip((CommonMethods.PAGESIZE * currPage) - CommonMethods.PAGESIZE).Take(CommonMethods.PAGESIZE).ToList();

                return View(model);
            }
        }

        public ActionResult addreslabtest()
        {
            int VesselID = Convert.ToInt32(Session["VesselSessionID"].ToString());
            assR.MooringLineLists = CommonClass.MooringRopeDetailListResidual(0, VesselID);
            return View(assR);
        }
        [HttpPost]
        [PreventSpam("addreslabtest", 3, 1)]
        public ActionResult addreslabtest(ResidualLabTest rslbtest, HttpPostedFileBase file)
        {
            int VesselID = Convert.ToInt32(Session["VesselSessionID"].ToString());
            var IdPK = ((from asn in context.ResidualLabTests.Where(x => x.VesselID == VesselID) select (int?)asn.Id).Max() ?? 0) + 1;

            rslbtest.LabTestDate = Convert.ToDateTime(rslbtest.LabTestDate);
            rslbtest.TestResults = Convert.ToDecimal(rslbtest.TestResults);

            rslbtest.RopeTypeId = rslbtest.RopeTypeId;
            rslbtest.ManufacturerId = rslbtest.ManufacturerId;
            if (rslbtest.Remarks == null)
            {
                rslbtest.Remarks = null;
            }
            else
            {
                rslbtest.Remarks = rslbtest.Remarks;
            }
            rslbtest.CreatedDate = DateTime.Now;
            rslbtest.IsActive = true;
            rslbtest.RopeTail = 0;
            rslbtest.VesselID = VesselID;
            rslbtest.Id = IdPK;
            context.ResidualLabTests.Add(rslbtest);


            if (file != null)
            {

                Random randon = new Random();
                int num = randon.Next(10000);
                string FileName = Path.GetFileNameWithoutExtension(file.FileName);
                //string FileExtension = Path.GetExtension(wbtRecord.ImageFile.FileName);        
                //FileName = DateTime.Now.ToString("yyyyMMdd") + "-" + FileName.Trim() + FileExtension;   

                string fileExtention = Path.GetExtension(file.FileName);
                string fileName = Path.GetFileName(file.FileName);
                var withoutextnsn = Path.GetFileNameWithoutExtension(fileName) + num;

                string[] str1;
                str1 = new string[26] { "jpg", "jpeg", "png", "pps", "ppt", "pptx", "xls", "xlsm", "xlsx", "doc", "docx", "pdf", "rtf", "txt", "gif", "wav", "mid", "midi", "wma", "mp3", "mp4", "ogg", "rma", "avi", "divx", "wmv" };
                string FileExtension = file.FileName.Substring(file.FileName.LastIndexOf('.') + 1).ToLower();

                if (str1.Contains(FileExtension))
                {

                    // string size = FormatSize(trngattach.ImageFile.ContentLength);
                    //size = size.Remove(size.Length - 2);
                    //int sizecheck = trngattach.ImageFile.ContentLength;

                    int compare = file.ContentLength;

                    if (compare > 5000000)
                    {
                        TempData["Error"] = "Attachment size not to be greater than 5MB";
                        return RedirectToAction("Index");
                    }

                    fileName = withoutextnsn + fileExtention;
                    string UploadPath = "~/images/AttachFiles/";

                    string pth = fileName;
                    //trngattach.AttachmentPath = UploadPath + fileName;
                    string origPath = Server.MapPath("~/images/AttachFiles/");
                    var originalFilePath = Path.Combine(origPath, fileName);

                    file.SaveAs(originalFilePath);

                    var tuple = CommonClass.Getmaxid(0, 0, 0, VesselID);

                    int idPk = tuple.Item1;
                    string ss = Convert.ToString(rslbtest.RopeId);

                    int rpid = Convert.ToInt32(ss);
                    int idpk1 = tuple.Item3;
                    CommonClass.InsertRopeAttachment(idPk, pth, rpid, 0, "ResidualLine",idpk1, VesselID);
                }
            }


            context.SaveChanges();

            //var lostdata = new ObservableCollection<ResidualLabTestClass>(sc.ResidualLabTestTbl.ToList());
            //MooringWinchRopeViewModel vm = new MooringWinchRopeViewModel(lostdata);



            var minvalue = context.ResidualLabTests.Where(x => x.RopeId == rslbtest.RopeId && x.VesselID == VesselID).Select(x => x.TestResults).Min();

            if (minvalue == null)
            {
                minvalue = rslbtest.TestResults;
            }

            if (rslbtest.TestResults <= 75)
            {
                if (minvalue <= 75)
                {
                    minvalue = minvalue;
                }
                else
                {
                    minvalue = rslbtest.TestResults;
                }

                var certificat = context.MooringRopeDetails.Where(x => x.VesselID == VesselID && x.RopeId == rslbtest.RopeId).FirstOrDefault();
                var NotiMsg = certificat + " Line residual strength Current - " + minvalue + "% / Min. Required 75% for CertificateNo- " + certificat.CertificateNumber + " - " + certificat.UniqueID;
                //var NotiMsg = "Line residual strength below minimum allowable (" + minvalue + " - / Required - 75%)";

                //InspectNotification(Ropeid, Max_running_hours_Approaching, NotiAlertType);

                int NotiAlertType = (int)NotificationAlertType.RopeResidual_StrengthCheck;
                var result = context.tblNotifications.Where(x => x.VesselId == VesselID & x.RopeId == rslbtest.RopeId & x.NotificationType == NotiAlertType).FirstOrDefault();

                if (result != null)
                {
                    context.tblNotifications.Remove(result);
                    context.SaveChanges();

                }

                var IdPK1 = ((from asn in context.tblNotifications.Where(x => x.VesselId == VesselID) select (int?)asn.Id).Max() ?? 0) + 1;

                tblNotification noti = new tblNotification();
                noti.Acknowledge = false;
                noti.AckRecord = "This notification cannot be acknowledged, kindly discard this item";
                noti.Notification = NotiMsg;
                noti.RopeId = rslbtest.RopeId;
                noti.IsActive = true;
                noti.NotificationDueDate = DateTime.Now.Date;
                noti.CreatedDate = DateTime.Now;
                noti.CreatedBy = "Admin";
                // noti.NotificationAlertType = 1;
                noti.NotificationType = NotiAlertType;
                noti.Id = IdPK1;
                noti.VesselId = VesselID;
                context.tblNotifications.Add(noti);
                context.SaveChanges();


            }

            //}

           // context.SaveChanges();


            TempData["Success"] = "Record successfully saved !";
            return RedirectToAction("Index");
        }


        public ActionResult addtailreslabtest()
        {
            int VesselID = Convert.ToInt32(Session["VesselSessionID"].ToString());
            assR.MooringLineLists = CommonClass.MooringRopeDetailListResidual(1, VesselID);
            return View(assR);
        }
        [HttpPost]
        [PreventSpam("addtailreslabtest", 3, 1)]
        public ActionResult addtailreslabtest(ResidualLabTest rslbtest, HttpPostedFileBase file)
        {
            int vesselid = Convert.ToInt32(Session["VesselSessionID"].ToString());
           
            var IdPK = ((from asn in context.ResidualLabTests.Where(x => x.VesselID == vesselid) select (int?)asn.Id).Max() ?? 0) + 1;

            rslbtest.LabTestDate = Convert.ToDateTime(rslbtest.LabTestDate);
            rslbtest.TestResults = Convert.ToDecimal(rslbtest.TestResults);

            rslbtest.RopeTypeId = rslbtest.RopeTypeId;
            rslbtest.ManufacturerId = rslbtest.ManufacturerId;
            if (rslbtest.Remarks == null)
            {
                rslbtest.Remarks = null;
            }
            else
            {
                rslbtest.Remarks = rslbtest.Remarks;
            }
            rslbtest.CreatedDate = DateTime.Now;
            rslbtest.IsActive = true;
            rslbtest.RopeTail = 1;
            rslbtest.VesselID = vesselid;
            rslbtest.Id = IdPK;
            context.ResidualLabTests.Add(rslbtest);


            if (file != null)
            {

                Random randon = new Random();
                int num = randon.Next(10000);
                string FileName = Path.GetFileNameWithoutExtension(file.FileName);
                //string FileExtension = Path.GetExtension(wbtRecord.ImageFile.FileName);        
                //FileName = DateTime.Now.ToString("yyyyMMdd") + "-" + FileName.Trim() + FileExtension;   

                string fileExtention = Path.GetExtension(file.FileName);
                string fileName = Path.GetFileName(file.FileName);
                var withoutextnsn = Path.GetFileNameWithoutExtension(fileName) + num;

                string[] str1;
                str1 = new string[26] { "jpg", "jpeg", "png", "pps", "ppt", "pptx", "xls", "xlsm", "xlsx", "doc", "docx", "pdf", "rtf", "txt", "gif", "wav", "mid", "midi", "wma", "mp3", "mp4", "ogg", "rma", "avi", "divx", "wmv" };
                string FileExtension = file.FileName.Substring(file.FileName.LastIndexOf('.') + 1).ToLower();

                if (str1.Contains(FileExtension))
                {

                    // string size = FormatSize(trngattach.ImageFile.ContentLength);
                    //size = size.Remove(size.Length - 2);
                    //int sizecheck = trngattach.ImageFile.ContentLength;

                    int compare = file.ContentLength;

                    if (compare > 5000000)
                    {
                        TempData["Error"] = "Attachment size not to be greater than 5MB";
                        return RedirectToAction("Index");
                    }

                    fileName = withoutextnsn + fileExtention;
                    string UploadPath = "~/images/AttachFiles/";

                    string pth = fileName;
                    //trngattach.AttachmentPath = UploadPath + fileName;
                    string origPath = Server.MapPath("~/images/AttachFiles/");
                    var originalFilePath = Path.Combine(origPath, fileName);

                    file.SaveAs(originalFilePath);

                    var tuple = CommonClass.Getmaxid(0, 0, 0, vesselid);

                    int idPk = tuple.Item1;
                    string ss = Convert.ToString(rslbtest.RopeId);

                    int rpid = Convert.ToInt32(ss);
                    int idpk1 = tuple.Item3;
                    CommonClass.InsertRopeAttachment(idPk, pth, rpid, 0, "ResidualRopeTail", idpk1, vesselid);
                    //CommonClass.InsertRopeAttachment(idPk, pth, rpid, 1, "ResidualRopeTail", vesselid);
                }
            }

            //var lostdata = new ObservableCollection<ResidualLabTestClass>(sc.ResidualLabTestTbl.ToList());
            //MooringWinchRopeViewModel vm = new MooringWinchRopeViewModel(lostdata);



            var minvalue = context.ResidualLabTests.Where(x => x.RopeId == rslbtest.RopeId).Select(x => x.TestResults).Min();

            if (minvalue == null)
            {
                minvalue = rslbtest.TestResults;
            }

            if (rslbtest.TestResults <= 75)
            {
                if (minvalue <= 75)
                {
                    minvalue = minvalue;
                }
                else
                {
                    minvalue = rslbtest.TestResults;
                }

                var certificat = context.MooringRopeDetails.Where(x => x.VesselID == vesselid && x.RopeId == rslbtest.RopeId).FirstOrDefault();

                var NotiMsg = "Tail residual strength Current - " + minvalue + "% / Min. Required 75% for CertificateNo- " + certificat.CertificateNumber + " - " + certificat.UniqueID;
                int NotiAlertType = (int)NotificationAlertType.RopeResidual_StrengthCheck;
                var result = context.tblNotifications.Where(x => x.VesselId == vesselid && x.RopeId == rslbtest.RopeId & x.NotificationType == NotiAlertType).FirstOrDefault();

                if (result != null)
                {
                    context.tblNotifications.Remove(result);
                    context.SaveChanges();

                }


                var IdPK1 = ((from asn in context.tblNotifications.Where(x => x.VesselId == vesselid) select (int?)asn.Id).Max() ?? 0) + 1;


                tblNotification noti = new tblNotification();
                noti.Acknowledge = false;
                noti.AckRecord = "This notification cannot be acknowledged, kindly discard this item";
                noti.Notification = NotiMsg;
                noti.RopeId = rslbtest.RopeId;
                noti.IsActive = true;
                noti.NotificationDueDate = DateTime.Now.Date;
                noti.CreatedDate = DateTime.Now;
                noti.CreatedBy = "Admin";
                noti.NotificationAlertType = 1;
                noti.NotificationType = (int)NotificationAlertType.RopeResidual_StrengthCheck;
                noti.Id = IdPK1;
                noti.VesselId = vesselid;
                context.tblNotifications.Add(noti);
                context.SaveChanges();


            }

            //}


            context.SaveChanges();



            TempData["Success"] = "Record successfully saved !";
            return RedirectToAction("tailreslablist");
        }
        public JsonResult GetWinchlocation(int Id, int Ropetail)
        {
            int VesselID = Convert.ToInt32(Session["VesselSessionID"].ToString());
            int ropetypeid = 0; int manuFid = 0; decimal crntRhrs = 0; string minserchk = "";
            string RopeType = ""; string ManuFname = ""; string location = "";
           
            SqlDataAdapter adp = new SqlDataAdapter("ResidualTestD", con);
            adp.SelectCommand.CommandType = CommandType.StoredProcedure;
            adp.SelectCommand.Parameters.AddWithValue("@RopeId", Id);
            adp.SelectCommand.Parameters.AddWithValue("@RopeTail", Ropetail);
            adp.SelectCommand.Parameters.AddWithValue("@VesselId", VesselID);
            DataSet dt = new DataSet();
            adp.Fill(dt);
            if (dt.Tables[0].Rows.Count > 0)
            {


                ropetypeid = Convert.ToInt32(dt.Tables[0].Rows[0]["ropetypeid"]);
                manuFid = Convert.ToInt32(dt.Tables[0].Rows[0]["manufacturerid"]);
                //DateTime ss = Convert.ToDateTime(dt.Tables[0].Rows[0]["InstalledDate"] == DBNull.Value ? null : dt.Tables[0].Rows[0]["InstalledDate"]);

                string ss = dt.Tables[0].Rows[0]["InstalledDate"] == DBNull.Value ? null : dt.Tables[0].Rows[0]["InstalledDate"].ToString();
                crntRhrs = Convert.ToDecimal(dt.Tables[0].Rows[0]["CurrentRunningHours"] == DBNull.Value ? 0 : dt.Tables[0].Rows[0]["CurrentRunningHours"]);
                RopeType = dt.Tables[0].Rows[0]["ropetype"].ToString();
                ManuFname = dt.Tables[0].Rows[0]["name"].ToString();

                //txtropetype.Text = RopeType;
                //txtmanfname.Text = ManuFname;
                if (ss == null)
                {
                    instdt = DateTime.Now.ToString();
                }
                else
                {

                    instdt = ss.ToString();
                }

                string dtes = DateTime.Now.ToString();

                minserchk = diffchek(dtes);



            }
            if (dt.Tables[1].Rows.Count > 0)
            {
                location = dt.Tables[1].Rows[0]["location"].ToString();

            }

            return Json(new { Result = true, linetype = RopeType, mname = ManuFname, locations = location, CrntRHrs = crntRhrs, rptypeid = ropetypeid, manufid = manuFid, minserchks = minserchk }, JsonRequestBehavior.AllowGet);

            //return Json(new { Result = true, linetype = "", mname = "", location = "", crntRhrs = "",rptypeid=0,manufid=0 }, JsonRequestBehavior.AllowGet);
        }


        public JsonResult TestDate(int RopeId, string ResLabTDate, int Ropetail)
        {
            string msg = ""; string minserchk = "";
            int VesselID = Convert.ToInt32(Session["VesselSessionID"].ToString());
            var data = context.MooringRopeDetails.Where(x => x.RopeId == RopeId && x.DeleteStatus == false && x.RopeTail == Ropetail && x.VesselID == VesselID).Select(x => x.ReceivedDate).SingleOrDefault();

            var installeddate = data;

            var assigndate = ResLabTDate;

            if (Convert.ToDateTime(assigndate) < installeddate)
            {
                msg = "Lab Test Date can not less than Rope Received date ! ";

                //MessageBox.Show("Lab Test Date can not less than Rope Received date!", "Residual Lab Test", MessageBoxButton.OK, MessageBoxImage.Error);

                // dpInsDate.Text = installeddate.ToString();
            }
            else
            {
                minserchk = diffchek(ResLabTDate);
            }

            return Json(new { Result = true, Message = msg, minserchks = minserchk }, JsonRequestBehavior.AllowGet);

            //return Json(new { Result = true, linetype = "", mname = "", location = "", crntRhrs = "",rptypeid=0,manufid=0 }, JsonRequestBehavior.AllowGet);
        }

       // [PreventSpam("DeleteLinereslabtest", 3, 1)]
        public ActionResult Delete(int Id, int? RopeId)
        {
            try
            {

                int VesselID = Convert.ToInt32(Session["VesselSessionID"].ToString());
                int ntype = (int)NotificationAlertType.RopeResidual_StrengthCheck;
                CommonClass.DeleteNotificationsRopeID(VesselID, RopeId, ntype);

                SqlDataAdapter adp = new SqlDataAdapter("update ResidualLabTest set IsActive='False' where Id =" + Id + " and RopeTail = 0 and VesselID='" + VesselID + "' ", con);
                DataTable dt = new DataTable();
                adp.Fill(dt);

                TempData["Success"] = "Record successfully deleted !";
            }
            catch { }
            return RedirectToAction("Index");
        }

       // [PreventSpam("DeleteTailreslabtest", 3, 1)]
        public ActionResult Deletetail(int Id, int? RopeId)
        {
            try
            {

                int VesselID = Convert.ToInt32(Session["VesselSessionID"].ToString());
                int ntype = (int)NotificationAlertType.RopeResidual_StrengthCheck;
                CommonClass.DeleteNotificationsRopeID(VesselID, RopeId, ntype);

                using (SqlDataAdapter adp = new SqlDataAdapter("update ResidualLabTest set IsActive='False' where Id =" + Id + " and RopeTail = 1 and VesselID='" + VesselID + "' ", con))
                {
                    DataTable dt = new DataTable();
                    adp.Fill(dt);
                }
                TempData["Success"] = "Record successfully deleted !";
            }
            catch { }
           
            return RedirectToAction("tailreslablist");
        }

        private string diffchek(string dates)
        {
            string dtcheck = "";
            try
            {
                //DateTime ss = Convert.ToDateTime(dpInsDate.Text);
                DateTime ss = Convert.ToDateTime(dates);
                DateTime ss1 = Convert.ToDateTime(instdt);
                string chk = ss1.ToString();
                if (chk == "01/01/0001 00:00:00")
                {
                    dtcheck = "0";

                }
                else if (ss1 > ss)
                {
                    dtcheck = "0";

                }
                else
                {
                    const double DaysPerMonth = 36524.0 / 1200.0;
                    double days = (ss - ss1).TotalDays;
                    double ddtt = days / DaysPerMonth;

                    Double d1 = Convert.ToDouble(ddtt);
                    Double dc1 = Math.Round((Double)d1, 1);

                    var ssss = (ss - ss1).TotalDays;


                    //decimal monthsApart = 12 * (2012 - 2020) + 4 - 5;
                    //decimal dgg = Math.Abs(monthsApart);

                    decimal diff = Convert.ToDecimal(ssss / 30);

                    //txttestdate.Text = diff.ToString();

                    Double d = Convert.ToDouble(diff);
                    Double dc = Math.Round((Double)d, 1);
                    dtcheck = dc1.ToString();
                    return dtcheck;

                }

            }
            catch (Exception ex)
            {

            }
            return dtcheck;
        }
    }
}