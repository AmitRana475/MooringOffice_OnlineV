using ClosedXML.Excel;
using MenuLayer;
using Newtonsoft.Json;
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
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Shipment49Web.Areas.LooseEquipment.Controllers
{
    [Authorize]
    [UserAuthenticationFilter]
    [ErrorClass]
    public class LEInspectionController : BaseController
    {
        // GET: LooseEquipment/LEInspection
        SqlConnection con = ConnectionBulder.con;
        MorringOfficeEntities context = new MorringOfficeEntities();
      
        public LEInspectionController()
        {
            //VesselID = Convert.ToInt32(CommonClass.VesselSessionID);
        }
        public ActionResult Index()
        {
            int VesselID = Convert.ToInt32(Session["VesselSessionID"].ToString());
            MooringLooseEquipInspection list = new MooringLooseEquipInspection();
            list.Year = DateTime.Now.Year.ToString();
            list.YearList = CommonClass.GetYearsList();
            using (ShipmentContaxt sc1 = new ShipmentContaxt())
            {

                var LEInspectionRecord = new ShipmentContaxt()
                     .MultipleResults("[dbo].[GetLooseEquipInspectionList] '" + list.Year + "','" + VesselID + "'")
                   .With<MooringLooseEquipInspection>().Execute();

                list.GetLooseEquipInspectionList = (List<MooringLooseEquipInspection>)LEInspectionRecord[0];
            }
            return View(list);
        }
        [HttpPost]
        public ActionResult Index(string Year)
        {
            var VesselID =  Session["VesselSessionID"].ToString();
            MooringLooseEquipInspection list = new MooringLooseEquipInspection();
            list.Year = Year;
            list.YearList = CommonClass.GetYearsList();
            using (ShipmentContaxt sc1 = new ShipmentContaxt())
            {

                var LEInspectionRecord = new ShipmentContaxt()
                     .MultipleResults("[dbo].[GetLooseEquipInspectionList] '" + list.Year + "','" + VesselID + "'")
                   .With<MooringLooseEquipInspection>().Execute();

                list.GetLooseEquipInspectionList = (List<MooringLooseEquipInspection>)LEInspectionRecord[0];
            }
            return View(list);
        }

        public string GetLooseEquipmentType(int Ltp)
        {
            return context.LooseETypes.Where(t => t.Id == Ltp).Select(s => s.LooseEquipmentType).SingleOrDefault();

        }
        public ActionResult AddLeInspection(int LET)
        {
            int VesselID = Convert.ToInt32(Session["VesselSessionID"].ToString());
            MooringLooseEquipInspection insp = new MooringLooseEquipInspection() { LooseETypeId = LET };
            //insp.LooseConditions = 
            insp.AddLeInspectionList = CommonClass.AddLEInspectionList(LET, VesselID);
            ViewBag.LooseEqType = GetLooseEquipmentType(LET);
            Session["LooseEqTypeID"] = LET;
            return View(insp);
        }
       
        [HttpPost]
        [PreventSpam("AddLeInspection", 1, 1)]
        public ActionResult AddLeInspection(MooringLooseEquipInspection LEIN)
        {
            return View(LEIN);
        }

        [PreventSpam("InsertInspection", 1, 1)]
        public ActionResult InsertInspection(string inspections)
        {
            int VesselID = Convert.ToInt32(Session["VesselSessionID"].ToString());
            var Result = inspections;
            string json = Result.ToString();
            string dd = json;
            int vesselid = Convert.ToInt32(VesselID);
            int nxtinspctid = CommonClass.NextInspectionId(vesselid);
           

            var Json2 = JsonConvert.DeserializeObject<List<MooringLooseEquipInspection>>(inspections);

            foreach (var rootObject in Json2)
            {
                var IdPK = ((from asn in context.MooringLooseEquipInspections.Where(x => x.VesselID == vesselid) select (int?)asn.Id).Max() ?? 0) + 1;
                ViewBag.LooseEqType = GetLooseEquipmentType(Convert.ToInt32(rootObject.LooseETypeId));
                //==== Image save in folder
                if (!string.IsNullOrEmpty(rootObject.Photo1))
                {
                    string xt = rootObject.Photo1.Replace("data:image/png;base64,", "");
                    // Convert Base64 String to byte[]
                    byte[] imageBytes = Convert.FromBase64String(xt);
                    MemoryStream ms = new MemoryStream(imageBytes, 0, imageBytes.Length);

                    string[] str1;
                    str1 = new string[4] { "jpg", "jpeg", "png", "GIF" };
                    string FileExtension = rootObject.Image1.Substring(rootObject.Image1.LastIndexOf('.') + 1).ToLower();

                    if (str1.Contains(FileExtension))
                    {
                        int compare = imageBytes.Length;
                        if (compare > 2000000)
                        {
                            TempData["Error"] = "Image size not to be greater than 2MB";
                            return RedirectToAction(Url.Action("Index", "LEInspection"));
                        }
                        // Convert byte[] to Image
                        ms.Write(imageBytes, 0, imageBytes.Length);
                        System.Drawing.Image image = System.Drawing.Image.FromStream(ms, true);
                        image.Save(Server.MapPath("~/images/inspectionimages/" + rootObject.Image1 + ""), System.Drawing.Imaging.ImageFormat.Jpeg);
                    }
                    else
                    {
                        TempData["Error"] = "Invalid File Format !";
                        return RedirectToAction(Url.Action("Index", "LEInspection"));
                    }
                }

                if (!string.IsNullOrEmpty(rootObject.Photo2))
                {
                    string xt1 = rootObject.Photo2.Replace("data:image/png;base64,", "");
                    // Convert Base64 String to byte[]
                    byte[] imageBytes1 = Convert.FromBase64String(xt1);
                    MemoryStream ms1 = new MemoryStream(imageBytes1, 0, imageBytes1.Length);
                    string[] str1;
                    str1 = new string[4] { "jpg", "jpeg", "png", "GIF" };
                    string FileExtension = rootObject.Image2.Substring(rootObject.Image2.LastIndexOf('.') + 1).ToLower();

                    if (str1.Contains(FileExtension))
                    {
                        int compare = imageBytes1.Length;
                        if (compare > 2000000)
                        {
                            TempData["Error"] = "Image size not to be greater than 2MB";
                            return RedirectToAction(Url.Action("Index", "LEInspection"));
                        }
                        // Convert byte[] to Image
                        ms1.Write(imageBytes1, 0, imageBytes1.Length);
                        System.Drawing.Image image1 = System.Drawing.Image.FromStream(ms1, true);
                        image1.Save(Server.MapPath("~/images/inspectionimages/" + rootObject.Image2 + ""), System.Drawing.Imaging.ImageFormat.Jpeg);
                    }
                    else
                    {
                        TempData["Error"] = "Invalid File Format !";
                        return RedirectToAction(Url.Action("Index", "LEInspection"));
                    }
                }

                int NotiID = CommonClass.NextInspectionId(VesselID);

                if (rootObject.Condition == "Not Acceptable")
                {
                    //if (MI1 == null && MI2 == null)
                    //{
                    //    MainViewModelWorkHours.CommonValue = true;
                    //    System.Windows.MessageBox.Show("Photo Can not be blank for  number '" + item.Number + "' ", "LooseEq Inspection", MessageBoxButton.OK, MessageBoxImage.Information);
                    //    return;
                    //}
                    //else
                    //{


                    var looseEname = context.LooseETypes.SingleOrDefault(x => x.Id == rootObject.LooseETypeId).LooseEquipmentType;
                    var notification = "Loose Equipment condition Not Acceptable of " + looseEname + " on CertificateNumber - '" + rootObject.Number + "'";

                    //var notification = "";
                    tblNotification noti = new tblNotification();
                    noti.Id = NotiID;
                    noti.VesselId = VesselID;
                    noti.Acknowledge = false;
                    noti.AckRecord = "Not yet acknowledged";
                    noti.Notification = notification;
                    noti.NotificationType = 1;
                    noti.RopeId = 0;
                    noti.IsActive = true;
                    // noti.NotificationDueDate = notidueMonth;
                    //noti.NotificationAlertType = NotificationAlertType.
                    noti.CreatedDate = DateTime.Now;
                    noti.CreatedBy = "Admin";
                    context.tblNotifications.Add(noti);
                    context.SaveChanges();
                    //}
                }


                //==== Image save in folder
                rootObject.Id = IdPK;
                rootObject.VesselID = VesselID;
                rootObject.CreatedDate = DateTime.Now.Date;
                rootObject.IsActive = true;
                rootObject.NotificationId = NotiID;
                rootObject.Photo1 = null;
                rootObject.Photo2 = null;
                context.MooringLooseEquipInspections.Add(rootObject);
                context.SaveChanges();

                UpdateNextInspectionDue(rootObject.InspectDate, Convert.ToInt32(rootObject.LooseETypeId), Convert.ToInt32(rootObject.LooseEtbPK));

            }

            return Json(Url.Action("Index", "LEInspection"));
            // return RedirectToAction("Index");
        }

        public void UpdateNextInspectionDue(DateTime? InspectDate, int LTp, int id)
        {
            int VesselID = Convert.ToInt32(Session["VesselSessionID"].ToString());
            if (LTp == 1)
            {

                DateTime? GetInspectionDueDate = null;
                if (InspectDate != null)
                {
                    GetInspectionDueDate = CommonClass.LEqInspectionDate(Convert.ToDateTime(InspectDate), LTp);

                }
                string qry = "update JoiningShackle  set InspectionDueDate=@InspectionDueDate where  Id=@Id and VesselID=@VesselID";
                using (SqlDataAdapter adp1 = new SqlDataAdapter(qry, con))
                {

                    adp1.SelectCommand.Parameters.AddWithValue("@InspectionDueDate", GetInspectionDueDate);
                    adp1.SelectCommand.Parameters.AddWithValue("@Id", id);
                    adp1.SelectCommand.Parameters.AddWithValue("@VesselID", VesselID);
                    DataTable dt1 = new DataTable();
                    adp1.Fill(dt1);
                }

            }
            else if (LTp == 5)
            {
                //var data = context.ChainStoppers.Where(x => x.VesselID == VesselID && x.Id == id).FirstOrDefault();
                //if (data != null)
                //{
                DateTime? GetInspectionDueDate = null;
                if (InspectDate != null)
                {
                    GetInspectionDueDate = CommonClass.LEqInspectionDate(Convert.ToDateTime(InspectDate), LTp);

                }

                string qry = "update ChainStopper   set InspectionDueDate=@InspectionDueDate where  Id=@Id and VesselID=@VesselID";
                using (SqlDataAdapter adp1 = new SqlDataAdapter(qry, con))
                {
                    adp1.SelectCommand.Parameters.AddWithValue("@InspectionDueDate", GetInspectionDueDate);
                    adp1.SelectCommand.Parameters.AddWithValue("@Id", id);
                    adp1.SelectCommand.Parameters.AddWithValue("@VesselID", VesselID);
                    DataTable dt1 = new DataTable();
                    adp1.Fill(dt1);
                }


            }
            else if (LTp == 7)
            {
                //var data = context.ChafeGuards.Where(x => x.VesselID == VesselID && x.Id == id).FirstOrDefault();
                //if (data != null)

                DateTime? GetInspectionDueDate = null;
                if (InspectDate != null)
                {
                    GetInspectionDueDate = CommonClass.LEqInspectionDate(Convert.ToDateTime(InspectDate), LTp);

                }

                string qry = "update ChafeGuard   set InspectionDueDate=@InspectionDueDate where  Id=@Id and VesselID=@VesselID";
                using (SqlDataAdapter adp1 = new SqlDataAdapter(qry, con))
                {
                    adp1.SelectCommand.Parameters.AddWithValue("@InspectionDueDate", GetInspectionDueDate);
                    adp1.SelectCommand.Parameters.AddWithValue("@Id", id);
                    adp1.SelectCommand.Parameters.AddWithValue("@VesselID", VesselID);
                    DataTable dt1 = new DataTable();
                    adp1.Fill(dt1);
                }

            }
            else if (LTp == 8)
            {
                //var data = context.WinchBreakTestKits.Where(x => x.VesselID == VesselID && x.Id == id).FirstOrDefault();
                //if (data != null)

                DateTime? GetInspectionDueDate = null;
                if (InspectDate != null)
                {
                    GetInspectionDueDate = CommonClass.LEqInspectionDate(Convert.ToDateTime(InspectDate), LTp);

                }

                string qry = "update WinchBreakTestKit   set InspectionDueDate=@InspectionDueDate where  Id=@Id and VesselID=@VesselID";
                using (SqlDataAdapter adp1 = new SqlDataAdapter(qry, con))
                {
                    adp1.SelectCommand.Parameters.AddWithValue("@InspectionDueDate", GetInspectionDueDate);
                    adp1.SelectCommand.Parameters.AddWithValue("@Id", id);
                    adp1.SelectCommand.Parameters.AddWithValue("@VesselID", VesselID);
                    DataTable dt1 = new DataTable();
                    adp1.Fill(dt1);
                }

            }
            else //if (LTp == 4)
            {
                //var data = context.RopeTails.Where(x => x.VesselID == VesselID && x.Id == id && x.LooseETypeId == LTp).FirstOrDefault();
                //if (data != null)

                DateTime? GetInspectionDueDate = null;
                if (InspectDate != null)
                {
                    GetInspectionDueDate = CommonClass.LEqInspectionDate(Convert.ToDateTime(InspectDate), LTp);

                }

                string qry = "update RopeTail set InspectionDueDate=@InspectionDueDate where  Id=@Id and VesselID=@VesselID and LooseETypeId=@LooseETypeId";
                using (SqlDataAdapter adp1 = new SqlDataAdapter(qry, con))
                {

                    adp1.SelectCommand.Parameters.AddWithValue("@InspectionDueDate", GetInspectionDueDate);
                    adp1.SelectCommand.Parameters.AddWithValue("@Id", id);
                    adp1.SelectCommand.Parameters.AddWithValue("@VesselID", VesselID);
                    adp1.SelectCommand.Parameters.AddWithValue("@LooseETypeId", LTp);
                    DataTable dt1 = new DataTable();
                    adp1.Fill(dt1);
                }

            }
        }

        
        public ActionResult delete(int id, int NotiID)
        {
            try
            {
                int vesselid = Convert.ToInt32(Session["VesselSessionID"].ToString());
                using (SqlDataAdapter adp1 = new SqlDataAdapter("update MooringLooseEquipInspection set IsActive=0 where  Id=" + id + " and VesselID= " + vesselid + "", con))
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



        public ActionResult DownloadExcelSheet()
        {
            int VesselID = Convert.ToInt32(Session["VesselSessionID"].ToString());

            int loosetypeid =Convert.ToInt32( Session["LooseEqTypeID"]);

            string tblname = context.LooseETypes.Where(x => x.Id == loosetypeid).Select(x => x.LooseEquipmentType).SingleOrDefault();

            //DataSet dataSet = new DataSet();
            DataSet dataSet = null;
            dataSet = new DataSet("General");
            dataSet.Locale = System.Threading.Thread.CurrentThread.CurrentCulture;

            string qry = "LooseEquipInspection";
            SqlDataAdapter sda = new SqlDataAdapter(qry, con);
            sda.SelectCommand.CommandType = CommandType.StoredProcedure;
            sda.SelectCommand.Parameters.AddWithValue("@Id", loosetypeid);
            sda.SelectCommand.Parameters.AddWithValue("@table_name", tblname);
            sda.SelectCommand.Parameters.AddWithValue("@VesselId", VesselID);
            System.Data.DataTable datatbl = new System.Data.DataTable();

            sda.Fill(datatbl);

         

            datatbl.Columns.Remove("id");
            datatbl.Columns.Remove("RNumber");
            datatbl.Columns.Remove("looseetypeid");


            datatbl.Columns.Add("Condition", typeof(string));
            datatbl.Columns.Add("Remarks", typeof(string));



            dataSet.Tables.Add(datatbl);

          

            // string fileName = "DigiMoor_X7_InspectionSheet_" + DateTime.Now.ToString("dd-MMM-yyyy")+".xlsx";
            string fileName = "DigiMoor_X7_LooseEInspectionSheet";
            using (XLWorkbook wb = new XLWorkbook())
            {
                foreach (DataTable dt in dataSet.Tables)
                {
                    //Add DataTable as Worksheet.
                    wb.Worksheets.Add(dt);
                }

                //Export the Excel file.

                DateTime today = DateTime.Today;
                string HeaderName = fileName + "_" + today.ToString("dd-MMM-yyyy");

                Response.Clear();
                Response.Buffer = true;
                Response.Charset = "";
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                //Response.AddHeader("content-disposition", "attachment;filename=DataSet.xlsx");
                Response.AddHeader("content-disposition", "attachment;filename= " + HeaderName + ".xlsx");
                using (MemoryStream MyMemoryStream = new MemoryStream())
                {
                    wb.SaveAs(MyMemoryStream);
                    MyMemoryStream.WriteTo(Response.OutputStream);
                    Response.Flush();
                    Response.End();
                }
            }


            return RedirectToAction("Index");
        }
    }
}