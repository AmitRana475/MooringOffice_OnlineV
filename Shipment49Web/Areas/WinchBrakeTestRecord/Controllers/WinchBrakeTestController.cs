using MenuLayer;
using Reports;
using Shipment49Web.Areas.WinchBrakeTestRecord.Models;
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

namespace Shipment49Web.Areas.WinchBrakeTestRecord.Controllers
{
    [Authorize]
    [UserAuthenticationFilter]
    [ErrorClass]
    public class WinchBrakeTestController : Controller
    {
        MorringOfficeEntities context = new MorringOfficeEntities();
        SqlConnection con = ConnectionBulder.con;
        //CommonClass cls = new CommonClass();
      
        // GET: WinchBrakeTestRecord/WinchBrakeTest
        public ActionResult Index(int? page)
        {
           string VesselID =  Session["VesselSessionID"].ToString();
            using (ShipmentContaxt sc1 = new ShipmentContaxt())
            {
                WbtRecordClass model = new WbtRecordClass();
                var ilist = new ShipmentContaxt()
                     .MultipleResults("[dbo].[GetWinchBrakeTestRecord] '" + VesselID + "'")
                   .With<WbtRecordClass>()
                   .Execute();
                model.WbtList = (List<WbtRecordClass>)ilist[0];

                var record = model.WbtList.Count();
                int currPage = page == null ? 1 : Convert.ToInt32(page);
                TempData["CurrentPage"] = currPage;
                TempData["TotalRecords"] = record;
                model.WbtList = model.WbtList.Skip((CommonMethods.PAGESIZE * currPage) - CommonMethods.PAGESIZE).Take(CommonMethods.PAGESIZE).ToList();

                return View(model);
            }
        }


        [HttpGet]
        public ActionResult addwbtattachment()
        {
            return View();
        }
        [HttpPost]
        [PreventSpam("addwbtattachment", 3, 1)]
        public ActionResult addwbtattachment(WbtRecordClass wbtRecord)
        {
            if (string.IsNullOrEmpty(wbtRecord.AttachmentDate))
            {
                TempData["Error"] = "Select Date of Attachment.";
                return View(wbtRecord);
            }
            else
            {


                Random randon = new Random();
                int num = randon.Next(10000);
                string FileName = Path.GetFileNameWithoutExtension(wbtRecord.ImageFile.FileName);
                //string FileExtension = Path.GetExtension(wbtRecord.ImageFile.FileName);        
                //FileName = DateTime.Now.ToString("yyyyMMdd") + "-" + FileName.Trim() + FileExtension;   

                string fileExtention = Path.GetExtension(wbtRecord.ImageFile.FileName);
                string fileName = Path.GetFileName(wbtRecord.ImageFile.FileName);
                var withoutextnsn = Path.GetFileNameWithoutExtension(fileName) + num;


                string[] str1 = { "jpg", "jpeg", "png", "pps", "ppt", "pptx", "xls", "xlsm", "xlsx", "doc", "docx", "pdf", "txt", "GIF" };
                // str1 = new string[26] { "jpg", "jpeg", "png", "pps", "ppt", "pptx", "xls", "xlsm", "xlsx", "doc", "docx", "pdf", "rtf", "txt", "GIF", "WAV", "MID", "MIDI", "WMA", "MP3", "OGG", "RMA", "AVI", "MP4", "DIVX", "WMV" };
                string FileExtension = wbtRecord.ImageFile.FileName.Substring(wbtRecord.ImageFile.FileName.LastIndexOf('.') + 1).ToLower();

                if (str1.Contains(FileExtension))
                {
                    int compare = wbtRecord.ImageFile.ContentLength;

                    if (compare > 5000000)
                    {
                        TempData["Error"] = "Attachment size not to be greater than 5MB";
                        return View(wbtRecord);
                    }

                    fileName = withoutextnsn + fileExtention;
                    string UploadPath = "~/images/AttachFiles/";
                    wbtRecord.AttachmentPath = UploadPath + fileName;
                    string origPath = Server.MapPath("~/images/AttachFiles/");
                    var originalFilePath = Path.Combine(origPath, fileName);

                    wbtRecord.ImageFile.SaveAs(originalFilePath);

                }
                else
                {
                    TempData["Error"] = "Invalid File Format";
                    return View(wbtRecord);

                }

                int VesselID = Convert.ToInt32(Session["VesselSessionID"].ToString());
                int IdPK = 0;
                using (SqlDataAdapter adp1 = new SqlDataAdapter("SELECT COALESCE(MAX(id), 0) + 1  FROM WinchBrakeTestAttachment where VesselID= '" + VesselID + "'", con))
                {

                    DataTable dt1 = new DataTable();
                    adp1.Fill(dt1);

                    IdPK = Convert.ToInt32(dt1.Rows[0][0]);
                }



                using (SqlDataAdapter adp = new SqlDataAdapter("INSERT INTO WinchBrakeTestAttachment (Id, AttachmentName, AttachmentPath,CreatedDate,VesselID) VALUES (" + IdPK + ", '" + wbtRecord.AttachmentName + "', '" + fileName + "','" + wbtRecord.AttachmentDate + "','" + VesselID + "')", con))
                {
                    DataTable dt = new DataTable();
                    adp.Fill(dt);

                    TempData["Success"] = "Record successfully saved !";
                }
                return RedirectToAction("Index");
            }

        }


        [PreventSpam("Deletewbtattachment", 3, 1)]
        public ActionResult Delete(int Id)
        {
            try
            {

                string VesselID = Session["VesselSessionID"].ToString();
                using (SqlDataAdapter adp = new SqlDataAdapter("delete from WinchBrakeTestAttachment where Id =" + Id + "  and VesselID='" + VesselID + "' ", con))
                {
                    DataTable dt = new DataTable();
                    adp.Fill(dt);
                }
                TempData["Success"] = "Record successfully deleted !";
            }
            catch { }
            return RedirectToAction("Index");
        }

        //DownloadAttachment
        public ActionResult DownloadAttachment(string fname)
        {
            if (!string.IsNullOrEmpty(fname))
            {
                string paths = Server.MapPath("~/images/AttachFiles/" + fname);
               if(System.IO.File.Exists(paths))
                {
                    //string fullName = Server.MapPath("~" + filePath);

                    //byte[] fileBytes = System.IO.GetFile(fullName);
                    //return File(
                    //    fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, filePath);

                    byte[] fileBytes = System.IO.File.ReadAllBytes(paths);
                    string fileName = fname;
                    return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);

                    //Response.Clear();
                    //Response.BufferOutput = true;
                    //Response.Charset = "";
                    //Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    //Response.AddHeader("content-disposition", "attachment;filename=DMX7_Export_" + vessel.ToString().Replace(" ", "") + ".xlsx");

                    //using (MemoryStream MyMemoryStream = new MemoryStream())
                    //{
                    //    wb.SaveAs(MyMemoryStream);
                    //    MyMemoryStream.WriteTo(Response.OutputStream);
                    //    Response.End();
                    //}

                    //Response.Clear();

                    //Thread.Sleep(300);
                }
                TempData["Error"] = "File not found !";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["Error"] = "File not found !";
                return new EmptyResult();
            }
           // return RedirectToAction("Index");
        }
    }
}