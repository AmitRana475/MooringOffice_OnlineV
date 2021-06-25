using MenuLayer;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using Reports;
using Shipment49Web.Areas.MSPS.Models;
using Shipment49Web.Common;
using Shipment49Web.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
namespace Shipment49Web.Areas.MooringManual.Controllers
{
    [Authorize]
    [UserAuthenticationFilter]
    [ErrorClass]
    public class MSMPController : BaseController
    {
        MorringOfficeEntities context = new MorringOfficeEntities();
        SqlConnection con = ConnectionBulder.con;
        static int? Menuid = 0;
        // GET: MooringManual/MSMP

        public static int VesselID { get; set; }
        public MSMPController()
        {
            VesselID = Convert.ToInt32(CommonClass.VesselSessionID);
            CommonClass.TopeMenuID = "Menu2";
        }
        public ActionResult Index(int? id)
        {
          
            DetailsViewModel model = new DetailsViewModel();

            var menulist = context.tblSmartMenus.Select(x => x.SmartMenuContentExport).SingleOrDefault();
            ViewBag.MenuContent1 = menulist;

            if(id==null)
            {
                id = 1;
            }

            Menuid = id;

            //var Json = JsonConvert.DeserializeObject<List<MenuName>>(menulist);
            //// MenuName m = JsonConvert.DeserializeObject<MenuName>(menulist);

            //using (SqlDataAdapter adp5 = new SqlDataAdapter("select * from tblmenuname ", con))
            //{
            //    DataTable dt5 = new DataTable();
            //    adp5.Fill(dt5);

            //    List<tblMenu> studentList = new List<tblMenu>();
            //    for (int i = 0; i < dt5.Rows.Count; i++)
            //    {
            //        tblMenu student = new tblMenu();
            //        student.Id = Convert.ToInt32(dt5.Rows[i]["Id"]);
            //        student.Mid = Convert.ToInt32(dt5.Rows[i]["Mid"]);
            //        student.MenuName = dt5.Rows[i]["MenuName"].ToString();
            //        student.Type = Convert.ToInt32(dt5.Rows[i]["Type"]);
            //        studentList.Add(student);
            //    }

            //    //List<MenuName> fundList = dt5;
            //    ViewBag.MenuContent1 = studentList;
            //    //ViewBag.MenuContent1 = dt5;
            //}



            //using (ShipmentContaxt sc1 = new ShipmentContaxt())
            //{

            //    var _smartmenu = sc1.SmartMenus.ToList().FirstOrDefault();
            //    if (_smartmenu != null)
            //    {
            //        ViewBag.MenuContent1 = _smartmenu.SmartMenuContent;
            //    }
            //}


            //foreach (var rootObject in Json)
            //{
            //    var Mid = rootObject.id;
            //    var Mname = rootObject.text;
            //    var type = rootObject.type;
            //    List<MenuName> ss = rootObject.children;

            //    //if (ss == null)
            //    //{
            //        using (SqlDataAdapter adp = new SqlDataAdapter("INSERT INTO tblmenuname (Mid, MenuName, Type) VALUES (" + Mid + ", '" + Mname + "', " + type + ")", con))
            //        {
            //            DataTable dt = new DataTable();
            //            adp.Fill(dt);
            //        }
            //    //}
            //    if (ss != null)
            //    {
            //        foreach (var item in ss)
            //        {
            //            var Mid1 = item.id;
            //            var Mname1 = item.text;
            //            var type1 = item.type;
            //            List<MenuName> ss1 = item.children;

            //            //if (ss1 == null)
            //            //{

            //                using (SqlDataAdapter adp = new SqlDataAdapter("INSERT INTO tblmenuname (Mid, MenuName, Type) VALUES (" + Mid1 + ", '" + Mname1 + "', " + type1 + ")", con))
            //                {
            //                    DataTable dt = new DataTable();
            //                    adp.Fill(dt);
            //                }
            //            //}

            //            if (ss1 != null)
            //            {
            //                foreach (var item1 in ss1)
            //                {
            //                    var Mid11 = item1.id;
            //                    var Mname11 = item1.text;
            //                    var type11 = item1.type;


            //                    using (SqlDataAdapter adp = new SqlDataAdapter("INSERT INTO tblmenuname (Mid, MenuName, Type) VALUES (" + Mid11 + ", '" + Mname11 + "', " + type11 + ")", con))
            //                    {
            //                        DataTable dt = new DataTable();
            //                        adp.Fill(dt);
            //                    }
            //                }
            //            }

            //        }
            //    }
            //}

            //model.Content1 = context.ShipSpecificContents.ToList()
            //    .Where(m => m.MId == id && m.ShipId == VesselID)
            //    .OrderByDescending(p => p.Id)
            //    .Select(x => x.Content).FirstOrDefault();

            //model.Content1 = context.tblShipSpecificContents.Where(x => x.MId == Menuid && x.ShipId == VesselID.ToString()).Select(x => x.Content).SingleOrDefault();
            return View();
        }

        //[HttpPost]
        //public ActionResult Index(int? Mid)
        //{
        //    //var menulist = context.tblSmartMenus.Select(x => x.SmartMenuContentExport).SingleOrDefault();
        //    //ViewBag.MenuContent1 = menulist;
        //    Menuid = Mid;
        //    return View();
        //}

            [HttpGet]
        public ActionResult CheckMid(int id)
        {
            return View();
        }

        [HttpPost]
        [PreventSpam("_revisiondetails", 3, 1)]
        public ActionResult _revisiondetails( int? page)
        {
           

            int currPage = page == null ? 1 : Convert.ToInt32(page);

            TempData["CurrentPage"] = currPage;

            Reports.MorringOfficeEntities entities = new Reports.MorringOfficeEntities();

            var revisions = entities.Revisions.OrderByDescending(p => p.Id).Where(u => u.Mid == Menuid).AsEnumerable();

            TempData["TotalRecords"] = revisions.Count();
            revisions = revisions.Skip((CommonMethods.PAGESIZE * currPage) - CommonMethods.PAGESIZE).Take(CommonMethods.PAGESIZE).ToList();

            return PartialView("_revisionDetail", revisions);
        }



        public JsonResult Viewattachments()
        {
            using (Reports.MorringOfficeEntities officeEntities = new Reports.MorringOfficeEntities())
            {
                var shipspecattach = officeEntities.tblShipSpecificAttachments.
                    Where(u => u.MId == Menuid & u.ShipId == VesselID.ToString()).ToList();

                return Json(new { Result = true, Data = shipspecattach }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetSpecificContent()
        {
            ShipmentContaxt sc1 = new ShipmentContaxt();
            //List<SelectListItem> docs = new List<SelectListItem>();
            var docscontent = sc1.ShipSpecificContents.ToList()
                .Where(m => m.MId == Menuid && m.ShipId == VesselID.ToString())
                .OrderByDescending(p => p.Id)
                .Select(x => x.Content).FirstOrDefault();

          
            return Json(docscontent, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetGdata()
        {
            ShipmentContaxt sc1 = new ShipmentContaxt();
            
            var docscontent = sc1.DocsPagges.ToList()
                .Where(m => m.Mid == Menuid )
                .OrderByDescending(p => p.Id)
                .Select(x => x.Content).FirstOrDefault();


            return Json(docscontent, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [PreventSpam("addattachmentMSMP", 3, 1)]
        public ActionResult addattachment(DetailsViewModel model, FormCollection collection)
        {
            string IDs = collection["ShipIds"];
            //string[] selectedShips = IDs.Split(',');    
            //string[] selectedShips = VesselID.ToString();

            using (ShipmentContaxt context = new ShipmentContaxt())
            {
                //foreach (string shipID in selectedShips)
                //{
                    for (int count = 0; count < Request.Files.Count; count++)
                    {
                        var uploadedFile = Request.Files[0];
                        string attachmentName = DateTime.Now.Ticks + Path.GetExtension(uploadedFile.FileName);


                        string[] str1;
                        str1 = new string[13] { "jpg", "png", "pps", "ppt", "pptx", "xls", "xlsm", "xlsx", "doc", "docx", "pdf", "rtf", "txt" };
                        string FileExtension = uploadedFile.FileName.Substring(uploadedFile.FileName.LastIndexOf('.') + 1).ToLower();

                        if (str1.Contains(FileExtension))
                        {
                            int compare = uploadedFile.ContentLength;

                            if (compare > 5000000)
                            {
                                TempData["fileformat"] = "Attachment size not to be greater than 5MB";
                                return RedirectToAction("index", "MSMP", new { area = "MooringManual", @id = Menuid });
                            }

                            if (uploadedFile != null && uploadedFile.ContentLength > 0)
                            {
                                string filePath = Server.MapPath(string.Format("~/images/AttachFiles/filepath/{0}/", VesselID));

                                if (!Directory.Exists(filePath))
                                    Directory.CreateDirectory(filePath);

                                var path = Path.Combine(filePath, attachmentName);
                                uploadedFile.SaveAs(path);
                            }
                        }
                        else
                        {
                            TempData["fileformat"] = "Invalid File Format";

                            return RedirectToAction("index", "MSMP", new { area = "MooringManual", @id = Menuid });

                        }

                        ShipSpecificAttachment attachment = new ShipSpecificAttachment();

                        attachment.Attachment = attachmentName;
                        attachment.AttachmentName = model.shipmodel.AttachmentName;
                        attachment.MId = Menuid ?? default(int);
                        attachment.ShipId = VesselID.ToString();
                        attachment.CreatedDate = attachment.ModifiedDate = DateTime.Today;
                        attachment.CreateBy = attachment.ModifiedBy = User.Identity.GetUserName();

                        context.ShipSpecificAttachments.Add(attachment);
                    }
               // }

                context.SaveChanges();
            }

            return RedirectToAction("index", "MSMP", new { area = "MooringManual", @id = Menuid });
        }

    }
}

public class MenuName
{
    public int deleted { get; set; }
  
    public int slug { get; set; }
    public string href { get; set; }
    public int type { get; set; }
    public string prefix { get; set; }
    public string text { get; set; }
    public int id { get; set; }

    public List<MenuName> children { get; set; }


}

public class tblMenu
{
    public int Id { get; set; }
    public int Mid { get; set; }
    public string MenuName { get; set; }
    public int Type { get; set; }
   

}