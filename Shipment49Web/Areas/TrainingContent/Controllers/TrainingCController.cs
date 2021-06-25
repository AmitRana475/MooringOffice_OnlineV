using MenuLayer;
using Reports;
using Shipment49Web.Areas.TrainingContent.Models;
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
namespace Shipment49Web.Areas.TrainingContent.Controllers
{
    [Authorize]
    [UserAuthenticationFilter]
    [ErrorClass]
    public class TrainingCController : BaseController
    {
        MorringOfficeEntities context = new MorringOfficeEntities();
        SqlConnection con = ConnectionBulder.con;
       // CommonClass cls = new CommonClass();
      
        // GET: TrainingContent/TrainingC
        public TrainingCController()
        {
            CommonClass.TopeMenuID = "Menu3";
        }
        public ActionResult Index(int? page)
        {
            
          string  VesselID =  Session["VesselSessionID"].ToString(); 
            using (ShipmentContaxt sc1 = new ShipmentContaxt())
            {
                TrainingAttachmentClass model = new TrainingAttachmentClass();
                var ilist = new ShipmentContaxt()
                     .MultipleResults("[dbo].[GetTrainingAttachment] '" + VesselID + "'")
                   .With<TrainingAttachmentClass>()
                   .Execute();
                model.TrainingContentList = (List<TrainingAttachmentClass>)ilist[0];

                var record = model.TrainingContentList.Count();
                int currPage = page == null ? 1 : Convert.ToInt32(page);
                TempData["CurrentPage"] = currPage;
                TempData["TotalRecords"] = record;
                model.TrainingContentList = model.TrainingContentList.Skip((CommonMethods.PAGESIZE * currPage) - CommonMethods.PAGESIZE).Take(CommonMethods.PAGESIZE).ToList();


                return View(model);
            }
        }

        [HttpGet]
        public ActionResult addtrainingattachment()
        {
            return View();
        }
        [HttpPost]
        [PreventSpam("addtrainingattachment", 3, 1)]
        public ActionResult addtrainingattachment(TrainingAttachmentClass trngattach)
        {

            Random randon = new Random();
            int num = randon.Next(10000);
            string FileName = Path.GetFileNameWithoutExtension(trngattach.ImageFile.FileName);
            //string FileExtension = Path.GetExtension(wbtRecord.ImageFile.FileName);        
            //FileName = DateTime.Now.ToString("yyyyMMdd") + "-" + FileName.Trim() + FileExtension;   

            string fileExtention = Path.GetExtension(trngattach.ImageFile.FileName);
            string fileName = Path.GetFileName(trngattach.ImageFile.FileName);
            var withoutextnsn = Path.GetFileNameWithoutExtension(fileName) + num;

            string[] str1;
            str1 = new string[26] { "jpg", "jpeg", "png", "pps", "ppt", "pptx", "xls", "xlsm", "xlsx", "doc", "docx", "pdf", "rtf", "txt", "gif", "wav", "mid", "midi", "wma", "mp3", "mp4", "ogg", "rma", "avi", "divx", "wmv" };
            string FileExtension = trngattach.ImageFile.FileName.Substring(trngattach.ImageFile.FileName.LastIndexOf('.') + 1).ToLower();

            if (str1.Contains(FileExtension))
            {

                // string size = FormatSize(trngattach.ImageFile.ContentLength);
                //size = size.Remove(size.Length - 2);
                //int sizecheck = trngattach.ImageFile.ContentLength;

                int compare = trngattach.ImageFile.ContentLength;

                if (compare > 5000000)
                {
                    TempData["Error"] = "Attachment size not to be greater than 5MB";
                    return RedirectToAction("Index");
                }

                fileName = withoutextnsn + fileExtention;
                string UploadPath = "~/images/AttachFiles/";
                trngattach.AttachmentPath = UploadPath + fileName;
                string origPath = Server.MapPath("~/images/AttachFiles/");
                var originalFilePath = Path.Combine(origPath, fileName);

                trngattach.ImageFile.SaveAs(originalFilePath);

            }
            else
            {
                TempData["Error"] = "Invalid File Format";
                return RedirectToAction("Index");
            }

            int VesselID = Convert.ToInt32(Session["VesselSessionID"].ToString());
            int IdPK = CommonClass.MaxID(0, VesselID);
            CommonClass.InsertTrAtt(IdPK, trngattach.AttachmentName, fileName, VesselID);         
            TempData["Success"] = "Record successfully saved !";
            return RedirectToAction("Index");


        }
        static readonly string[] suffixes = { "Bytes", "KB", "MB", "GB", "TB", "PB" };
        public static string FormatSize(Int64 bytes)
        {
            int counter = 0;
            decimal number = (decimal)bytes;
            while (Math.Round(number / 1024) >= 1)
            {
                number = number / 1024;
                counter++;
            }
            return string.Format("{0:n1}{1}", number, suffixes[counter]);
        }
        public ActionResult Delete(int Id)
        {
            try
            {
                int VesselID = Convert.ToInt32(Session["VesselSessionID"].ToString());
                CommonClass.DeleteTrAtt(Id, VesselID);
                TempData["Success"] = "Record successfully deleted !";
            }
            catch { }
            return RedirectToAction("Index");
        }
    }
}