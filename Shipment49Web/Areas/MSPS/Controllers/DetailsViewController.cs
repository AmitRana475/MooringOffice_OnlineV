using iTextSharp.text;
using iTextSharp.text.pdf;
using MenuLayer;
using Microsoft.AspNet.Identity;
using MSMPmodule;
using Newtonsoft.Json;
using Shipment49Web.Areas.MSPS.Models;
using Shipment49Web.Common;
using Shipment49Web.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static Shipment49Web.Areas.MSPS.Models.RevisionModel;

namespace Shipment49Web.Areas.MSPS.Controllers
{
    [Authorize]
    [UserAuthenticationFilter]
    [ErrorClass]
    public class DetailsViewController : BaseController
    {
        //private readonly IMenuRepository sc;
        public DetailsViewController()
        {
            //sc = repo;

            //if (UserRole.username == null)
            //{
            //    UserRole.username = string.Join("", Roles.GetRolesForUser());
            //}
            //ViewBag.GetMenu = UserRole.username == "Admin" ? sc.Menus.ToList() : sc.Menus.Where(x => x.Role == "User").ToList();
            //ViewBag.GetSubMenu = UserRole.username == "Admin" ? sc.SubMenus.ToList() : sc.SubMenus.Where(x => x.Role == "User").ToList();
            using (ShipmentContaxt sc1 = new ShipmentContaxt())
            {
                DetailsViewModel model = new DetailsViewModel();

                var _smartmenu = sc1.SmartMenus.ToList().FirstOrDefault();
                if (_smartmenu != null)
                {
                    ViewBag.MenuContent = _smartmenu.SmartMenuContent;
                }
            }
        }
        //public ActionResult Index(string subid,string subsubid,string mid)
        [HttpGet]
        public ActionResult Index(int id)
        {
            ShipmentContaxt sc1 = new ShipmentContaxt();
            DetailsViewModel model = new DetailsViewModel();

            //var docscontent = sc1.DocsPagges.ToList().Where(m => m.Subid == Convert.ToInt32(subid) && m.SubSubid == Convert.ToInt32(subsubid) && m.Mid==Convert.ToInt32(mid)).Select(x => x.Content).FirstOrDefault();
            //var docscontent = sc1.DocsPagges.ToList().Where(m => m.Mid == Convert.ToInt32(id)).Select(x => x.Content).FirstOrDefault();
            ////string str = Regex.Replace(HttpUtility.HtmlDecode(docscontent), "<.*?>", string.Empty);
            //model.Content = docscontent;

            model.Mid = id;

            //string viess = ViewBag.message;

            //var Ship = sc1.Vessels.ToList();
            //var shipid =  Ship.Select(x => x.ImoNo).First();

            //var shipid =  sc1.Vessels.FirstOrDefault().ImoNo;

            ViewBag.ShipList = sc1.Vessels.AsQueryable().Select(x => new SelectListItem { Text = x.VesselName, Value = x.ImoNo.ToString() }).ToList();

            //var shipsp = sc1.ShipSpecificContents.ToList().Where(m => m.MId == Convert.ToInt32(id) && m.ShipId == shipid.ToString()).Select(x => x.Content).FirstOrDefault();

            //var existrvno=sc1.MasterRevisions.ToList()
            //var s = string.Join(",", sc1.MasterRevisions.ToList().Select(p => p.RevisionsIncluded.Trim().ToString()));

            //string codes = s;
            //string[] codesarray = codes.TrimStart().Split(',');

            //model.RevList = sc1.Revisions.ToList().Where(x => !codesarray.ToList().Contains(x.RNumber.ToString())).Where(x => !codesarray.ToList().Contains(x.RNumber.ToString())).
            //    Select(x => new SelectListItem { Text = x.RNumber.ToString(), Value = x.Id.ToString() }).ToList();

            //model.RevList = sc1.Revisions.Where(x => x.MId == model.Mid && x.Status == "Approved").Select(x => new SelectListItem { Text = x.RNumber.ToString(), Value = x.Id.ToString() }).ToList();

            var revisions = sc1.Revisions.Where(x => x.MId == model.Mid && x.Status == "Approved").ToList();

            if (revisions.Count > 0)
            {
                model.RevList = revisions.Select(x => new SelectListItem { Text = x.RNumber.ToString(), Value = x.Id.ToString() }).ToList();
            }

            var docsPages = sc1.DocsPagges.FirstOrDefault(m => m.Mid == id);

            if (docsPages != null)
            {
                model.Content = docsPages.Content;
            }

            string proceedure = string.Format("[dbo].[spShipSpecAttachment] {0}", id);

            var ilist = new ShipmentContaxt().MultipleResults(proceedure).With<ShipSpecificModel>().Execute();

            //var ilist = new ShipmentContaxt()
            //     .MultipleResults("[dbo].[spShipSpecAttachment]")
            //   //.With<TotalCount>()
            //   .With<ShipSpecificModel>()
            //   .Execute();

            model.shipattListing = (List<ShipSpecificModel>)ilist[0];

            //List<TotalCount> totobj = (List<TotalCount>)ilist[0];
            //model.Total = totobj.FirstOrDefault().Total;
            //ViewBag.TotalCount = model.Total;
            //var pager = new Pager(Convert.ToInt32(model.Total), 0);
            //model.Pager = pager;
            //return View(model);

            //model.Content1 = shipsp;

            var shipInfo = sc1.Vessels.FirstOrDefault();

            if (shipInfo != null)
            {
                model.ShipId = shipInfo.ImoNo;
            }

            var _smartmenu = sc1.SmartMenus.FirstOrDefault();

            if (_smartmenu != null)
            {
                model.SmartMenuContent = _smartmenu.SmartMenuContent;

                string allmenu = sc1.SmartMenus.Select(x => x.SmartMenuContent).FirstOrDefault();
                var response = JsonConvert.DeserializeObject<List<RootObject>>(allmenu);

                int check = 0;

                foreach (var vObj in response)
                {
                    if (check == 1)
                    {
                        break;
                    }

                    if (vObj.id == model.Mid)
                    {
                        model.BreadCrumb = vObj.text;
                    }

                    var childerncheck = vObj.children;
                    if (childerncheck != null)
                    {
                        foreach (var vObj1 in vObj.children)
                        {
                            if (vObj1.id == model.Mid)
                            {
                                model.BreadCrumb = vObj.text + " >> " + vObj1.text;
                            }

                            var childerncheck1 = vObj1.children;
                            if (childerncheck1 != null)
                            {
                                foreach (var vObj2 in vObj1.children)
                                {
                                    if (vObj2.id == model.Mid)
                                    {
                                        model.BreadCrumb = vObj.text + " >> " + vObj1.text + " >> " + vObj2.text;
                                        check = 1;
                                        break;
                                    }
                                }
                            }
                            if (check == 1)
                            {
                                break;
                            }
                        }
                    }
                }
            }

            return View(model);
        }

        public ActionResult Download(int id)
        {
            //return View(PDF(id));
            byte[] fileContent = PDF(id);
            return new FileContentResult(fileContent, "application/octet-stream") { FileDownloadName = DateTime.Now.Ticks + ".pdf" };
        }

        public ActionResult DownloadShipSpecific(int id)
        {
            //return View(ShipSpecific(id));
            byte[] fileContent = ShipSpecific(id);
            return new FileContentResult(fileContent, "application/octet-stream") { FileDownloadName = DateTime.Now.Ticks + ".pdf" };
        }

        [HttpGet]
        public ActionResult Add(string mid, string shipid)
        {
            using (ShipmentContaxt context = new ShipmentContaxt())
            {
                DocsPagesModel model = new DocsPagesModel();
                var Revtblelist = context.Revisions.ToList().Where(x => x.MId == Convert.ToInt32(mid)).FirstOrDefault();
                //ViewBag.Doclist = Doumentlist.AsQueryable().Select(x => new SelectListItem { Text = x.DocumentName, Value = x.DocumentID.ToString() }).ToList();
                var status = "";
                if (Revtblelist != null)
                {
                    status = Revtblelist.Status;
                }
                else
                {
                    status = "approve";
                }
                if (status == "Awaiting")
                {
                    //ViewBag.message = "await";
                    TempData["await"] = "await";
                    //return Content("<script language='javascript' type='text/javascript'>alert('Thanks for Feedback!');</script>");
                    //return RedirectToAction("index", new { id = id });
                    return Redirect(string.Format("~/MSPS/DetailsView?id={0}", mid));
                    // return View(model);
                }
                else
                {

                    model.Mid = Convert.ToInt32(mid);
                    model.ShipId = Convert.ToInt32(shipid);
                    //model.Subid = Convert.ToInt32(subid);
                    //model.SubSubid = Convert.ToInt32(subsubid);
                    return View(model);
                }
            }

        }
        [HttpPost]
        [PreventSpam("Add", 3, 1)]
        public ActionResult Add(DocsPagesModel model)
        {
            using (ShipmentContaxt context = new ShipmentContaxt())
            {
                var latestRevision = context.Revisions.Where(u => u.MId == model.Mid).OrderByDescending(p => p.Id).FirstOrDefault();

                //TempDocsPage txt = new TempDocsPage();
                Revision txt = new Revision();
                //var doctble = context.DocsPagges.ToList().Where(x => x.Mid == model.Mid && x.Subid==model.Subid && x.SubSubid ==model.SubSubid).FirstOrDefault();

                //doctble.Content = model.Content;
                var ss = model.Content;

                txt.Content = ss;
                string username = User.Identity.GetUserName();

                //txt.RNumber = 1; // latestRevision == null ? 1 : (latestRevision.RNumber + 1);
                txt.MId = model.Mid;
                // txt.SubId = model.Subid;
                // txt.SubSubId = model.SubSubid;
                txt.CreateBy = username;
                txt.Status = "Awaiting";
                if (model.ShipId != 0)
                {
                    txt.RevisionType = model.ShipId.ToString();
                }
                else
                {
                    txt.RevisionType = "General";
                }
                txt.ReviseDate = DateTime.Today;

                /// get content path 
                /// 

                var Menutest1 = context.SmartMenus.ToList();// from table
                string allmenu = Menutest1.Select(x => x.SmartMenuContent).FirstOrDefault();
                var response = JsonConvert.DeserializeObject<List<RootObject>>(allmenu);

                string path = " >> ";
                int check = 0;
                string prefix = string.Empty;

                foreach (var vObj in response)
                {
                    if (vObj.id == model.Mid)
                    {
                        path = vObj.text;
                        prefix = vObj.prefix;
                        check = 1;
                        break;
                    }

                    if (check == 1)
                    {
                        break;
                    }

                    var childerncheck = vObj.children;
                    if (childerncheck != null)
                    {
                        foreach (var vObj1 in vObj.children)
                        {
                            if (vObj1.id == model.Mid)
                            {
                                path = vObj.text + " >> " + vObj1.text;
                                prefix = vObj1.prefix;
                                break;
                            }

                            var childerncheck1 = vObj1.children;
                            if (childerncheck1 != null)
                            {
                                foreach (var vObj2 in vObj1.children)
                                {
                                    if (vObj2.id == model.Mid)
                                    {
                                        path = vObj.text + " >> " + vObj1.text + " >> " + vObj2.text;
                                        check = 1;
                                        prefix = vObj2.prefix;
                                        break;
                                    }
                                }
                            }
                            if (check == 1)
                            {
                                break;
                            }
                        }
                    }
                }

                txt.ContentPath = path;
                //txt.RPrefix = prefix;
                context.Revisions.Add(txt);
                //context.Entry(doctble).State = EntityState.Modified;
                context.SaveChanges();
                return RedirectToAction("Dashboard", "Home", new { Area = "" });
            }
        }

        [HttpGet]
        public ActionResult Edit(string mid, string shipid)
        {
            using (ShipmentContaxt context = new ShipmentContaxt())
            {
                DocsPagesModel model = new DocsPagesModel();
                //var doctble = context.DocsPagges.ToList().Where(x => x.Mid == Convert.ToInt32(mid) && x.Subid == Convert.ToInt32(subid) && x.SubSubid == Convert.ToInt32(subsubid)).Select(x=> x.Content).FirstOrDefault();
                var doctble = "";
                if (shipid == null)
                {
                    doctble = context.DocsPagges.ToList().Where(x => x.Mid == Convert.ToInt32(mid)).Select(x => x.Content).FirstOrDefault();
                }
                if (shipid != null)
                {
                    doctble = context.ShipSpecificContents.ToList()
                        .Where(x => x.MId == Convert.ToInt32(mid) && x.ShipId == shipid)
                        .OrderByDescending(p => p.Id)
                        .Select(x => x.Content).FirstOrDefault();
                }

                model.Content = doctble;
                model.Mid = Convert.ToInt32(mid);
                //model.Subid = Convert.ToInt32(subid);
                //model.SubSubid = Convert.ToInt32(subsubid);
                return View(model);
            }
        }

        [HttpGet]
        public ActionResult ViewCK()
        {
            return View();
        }
        [HttpPost]
        public ActionResult ViewCK(int ddd)
        {
            return View();
        }

        [HttpPost]
        [PreventSpam("Edit", 3, 1)]
        public ActionResult Edit(DocsPagesModel model)
        {
            using (ShipmentContaxt context = new ShipmentContaxt())
            {
                //var Revtblelist = context.Revisions.ToList().Where(x => x.MId == model.Mid && x.SubId == model.Subid && x.SubSubId == model.SubSubid).FirstOrDefault();
                //var Revtblelist = context.Revisions.ToList().Where(x => x.MId == model.Mid).FirstOrDefault();

                //var Rnumber = Revtblelist == null ? 1 : Revtblelist.RNumber;

                //var addrnumber = Rnumber + Convert.ToDecimal(0.1);
                Revision txt = new Revision();
                txt.Content = model.Content;
                string username = User.Identity.GetUserName();

                var latestRevision = context.Revisions.Where(u => u.MId == model.Mid).OrderByDescending(p => p.Id).FirstOrDefault();
                txt.RNumber = null; // latestRevision == null ? 1 : (latestRevision.RNumber + 1);

                //txt.RNumber = null;
                txt.MId = model.Mid;
                // txt.SubId = model.Subid;
                // txt.SubSubId = model.SubSubid;

                if (model.ShipId == 0)
                {
                    txt.RevisionType = "General";
                }
                if (model.ShipId != 0)
                {
                    txt.RevisionType = model.ShipId.ToString();
                }

                txt.CreateBy = username;
                txt.Status = "Awaiting";
                txt.ReviseDate = DateTime.Today;


                /// get content path 
                /// 
                var Menutest1 = context.SmartMenus.ToList();// from table
                string allmenu = Menutest1.Select(x => x.SmartMenuContent).FirstOrDefault();

                var response = JsonConvert.DeserializeObject<List<RootObject>>(allmenu);
                string path = " >> "; int check = 0;
                string prefix = string.Empty;

                foreach (var vObj in response)
                {
                    if (vObj.id == model.Mid)
                    {
                        path = vObj.text;
                        prefix = vObj.prefix;
                        check = 1;
                        break;
                    }

                    if (check == 1)
                    {
                        break;
                    }

                    var childerncheck = vObj.children;
                    if (childerncheck != null)
                    {
                        foreach (var vObj1 in vObj.children)
                        {
                            if (vObj1.id == model.Mid)
                            {
                                path = vObj.text + " >> " + vObj1.text;
                                prefix = vObj1.prefix;
                                break;
                            }

                            var childerncheck1 = vObj1.children;
                            if (childerncheck1 != null)
                            {
                                foreach (var vObj2 in vObj1.children)
                                {
                                    if (vObj2.id == model.Mid)
                                    {
                                        path = vObj.text + " >> " + vObj1.text + " >> " + vObj2.text;
                                        check = 1;
                                        prefix = vObj2.prefix;
                                        break;
                                    }
                                }
                            }
                            if (check == 1)
                            {
                                break;
                            }
                        }
                    }
                }

                txt.ContentPath = path;
                //txt.RPrefix = prefix;
                context.Revisions.Add(txt);
                context.SaveChanges();

                return RedirectToAction("Dashboard", "Home", new { Area = "" });
            }
        }
        public JsonResult GetContent(int id, string textval)
        {
            ShipmentContaxt sc1 = new ShipmentContaxt();
            //List<SelectListItem> docs = new List<SelectListItem>();
            var docscontent = sc1.ShipSpecificContents.ToList()
                .Where(m => m.MId == id && m.ShipId == textval)
                .OrderByDescending(p => p.Id)
                .Select(x => x.Content).FirstOrDefault();

            //var docscontent = "";
            //var docData = docsList.Select(m => new SelectListItem()
            //{
            //    Text = m.Content,
            //    Value = m.DocsID.ToString(),
            //});
            return Json(docscontent, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAttachments(int mID, int shipID, int? page)
        {
            string proceedure = string.Format("[dbo].[spShipSpecificAttachment] {0}, {1}", shipID, mID);

            var ilist = new ShipmentContaxt().MultipleResults(proceedure).With<ShipSpecificModel>().Execute();

            var attachments = (List<ShipSpecificModel>)ilist[0];

            return Json(attachments, JsonRequestBehavior.AllowGet);
        }
        //[HttpPost]
        //public ActionResult addattachment(string Name, string City, string Address)
        //{
        //    return View();
        //}

        public JsonResult DeleteAttachment(int id)
        {
            using (Reports.MorringOfficeEntities morringOfficeContext = new Reports.MorringOfficeEntities())
            {
                // Delete Attachment
                var attachment = morringOfficeContext.tblShipSpecificAttachments.FirstOrDefault(p => p.Id == id);
                morringOfficeContext.tblShipSpecificAttachments.Remove(attachment);
                morringOfficeContext.SaveChanges();
                // Get All Attachments
                var attachmentList = morringOfficeContext.tblShipSpecificAttachments.Where(p => p.MId == attachment.MId).ToList();
                return Json(new { Result = true, Data = attachmentList }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [PreventSpam("addattachment_DetailsView", 3, 1)]
        public ActionResult addattachment(DetailsViewModel model, FormCollection collection)
        {
            string IDs = collection["ShipIds"];

            string[] selectedShips = IDs.Split(',');

            //ArrayList attachments = new ArrayList();

            //for (int count = 0; count < Request.Files.Count; count++)
            //{
            //    var uploadedFile = Request.Files[0];

            //    if (uploadedFile != null && uploadedFile.ContentLength > 0)
            //    {
            //        string attachmentName = DateTime.Now.Ticks + Path.GetExtension(uploadedFile.FileName);
            //        string filePath = Server.MapPath("~/images/AttachFiles/filepath/");

            //        var path = Path.Combine(filePath, attachmentName);
            //        uploadedFile.SaveAs(path);

            //        attachments.Add(attachmentName);
            //    }
            //}

            using (ShipmentContaxt context = new ShipmentContaxt())
            {
                foreach (string shipID in selectedShips)
                {
                    for (int count = 0; count < Request.Files.Count; count++)
                    {
                        var uploadedFile = Request.Files[0];
                        string attachmentName = DateTime.Now.Ticks + Path.GetExtension(uploadedFile.FileName);


                        string[] str1 = { "jpg", "jpeg", "png", "pps", "ppt", "pptx", "xls", "xlsm", "xlsx", "doc", "docx", "pdf", "txt", "GIF" };
                        string FileExtension = uploadedFile.FileName.Substring(uploadedFile.FileName.LastIndexOf('.') + 1).ToLower();

                        if (str1.Contains(FileExtension))
                        {
                            int compare = uploadedFile.ContentLength;

                            if (compare > 5000000)
                            {
                                TempData["fileformat"] = "Attachment size not to be greater than 5MB";
                                return RedirectToAction("index", "DetailsView", new { area = "MSPS", @id = model.Mid });
                            }

                            if (uploadedFile != null && uploadedFile.ContentLength > 0)
                            {
                                string filePath = Server.MapPath(string.Format("~/images/AttachFiles/filepath/{0}/", shipID));

                                if (!Directory.Exists(filePath))
                                    Directory.CreateDirectory(filePath);

                                var path = Path.Combine(filePath, attachmentName);
                                uploadedFile.SaveAs(path);
                            }
                        }
                        else
                        {
                            TempData["fileformat"] = "Invalid File Format";

                            return RedirectToAction("index", "DetailsView", new { area = "MSPS", @id = model.Mid });

                        }

                        ShipSpecificAttachment attachment = new ShipSpecificAttachment();

                        attachment.Attachment = attachmentName;
                        attachment.AttachmentName = model.shipmodel.AttachmentName;
                        attachment.MId = model.Mid;
                        attachment.ShipId = shipID;
                        attachment.CreatedDate = attachment.ModifiedDate = DateTime.Today;
                        attachment.CreateBy = attachment.ModifiedBy = User.Identity.GetUserName();

                        context.ShipSpecificAttachments.Add(attachment);
                    }
                }

                context.SaveChanges();
            }

            return RedirectToAction("index", "DetailsView", new { area = "MSPS", @id = model.Mid });
        }

        public ActionResult _LeftSideBar()
        {
            using (ShipmentContaxt sc1 = new ShipmentContaxt())
            {
                DetailsViewModel model = new DetailsViewModel();

                var _smartmenu = sc1.SmartMenus.ToList().FirstOrDefault();
                if (_smartmenu != null)
                {
                    model.SmartMenuContent = _smartmenu.SmartMenuContent;
                }
                return PartialView("_LeftSideBar", model);


            }
        }

        public ActionResult test()
        {
            ShipmentContaxt sc1 = new ShipmentContaxt();
            DetailsViewModel model = new DetailsViewModel();



            var _smartmenu = sc1.SmartMenus.ToList().FirstOrDefault();
            if (_smartmenu != null)
            {
                model.SmartMenuContent = _smartmenu.SmartMenuContent;
            }

            return View(model);
        }

        [HttpPost]
        //public ActionResult test(DetailsViewModel model)
        //{
        //    bool isSavedSuccessfully = false;

        //    foreach (string fileName in Request.Files)
        //    {
        //        HttpPostedFileBase file = Request.Files[fileName];

        //        You can Save the file content here

        //        isSavedSuccessfully = true;
        //    }

        //    return Json(new { Message = string.Empty });

        //}
        public ActionResult test(DetailsViewModel model)
        {
            bool isSavedSuccessfully = true;
            string fName = "";
            try
            {
                foreach (string fileName in Request.Files)
                {
                    HttpPostedFileBase file = Request.Files[fileName];
                    //Save file content goes here
                    fName = file.FileName;
                    if (file != null && file.ContentLength > 0)
                    {

                        var originalDirectory = new DirectoryInfo(string.Format("{0}images\\AttachFiles", Server.MapPath(@"\")));

                        string pathString = System.IO.Path.Combine(originalDirectory.ToString(), "filepath");

                        var fileName1 = Path.GetFileName(file.FileName);

                        bool isExists = System.IO.Directory.Exists(pathString);

                        if (!isExists)
                            System.IO.Directory.CreateDirectory(pathString);

                        var path = string.Format("{0}\\{1}", pathString, file.FileName);
                        file.SaveAs(path);

                    }

                }

            }
            catch (Exception ex)
            {
                isSavedSuccessfully = false;
            }


            if (isSavedSuccessfully)
            {
                return Json(new { Message = fName });
            }
            else
            {
                return Json(new { Message = "Error in saving file" });
            }
        }

        //[HttpGet]
        //public ActionResult MasterRevision()
        //{
        //    return View();
        //}

        [HttpPost]
        public ActionResult MasterRevision(Int32? page)
        {
            DetailsViewModel model = new DetailsViewModel();
            var ilist = new ShipmentContaxt()
              .MultipleResults("[dbo].[spMasterRevisionDetails] 'Detail',''")
            //.With<TotalCount>()
            .With<MasterRevisionModel>()
            .Execute();
            model.masterrivision = (List<MasterRevisionModel>)ilist[0];
            // ShipmentContaxt sc1 = new ShipmentContaxt();
            // var revlist = sc1.Revisions.ToList().Select(x => new SelectListItem { Text = x.RNumber.ToString(), Value = x.Id.ToString() }).ToList();
            //model.RevList = revlist;
            return PartialView("_MasterRevision", model);
        }

        [HttpPost]
        public ActionResult _revisiondetails(int? mid, int? page)
        {
            //ApproveIndex model = new ApproveIndex();
            //using (ShipmentContaxt sc1 = new ShipmentContaxt())
            //{
            //    string parameters = string.Format("[dbo].[spCheckContent_PageWise] 10,0,{0}", page);

            //    var ilist = new ShipmentContaxt()
            //         .MultipleResults(parameters)
            //       .With<TotalCount>()
            //       .With<ContentDetails>()
            //       .Execute();
            //    model.contentListing = (List<ContentDetails>)ilist[1];
            //    List<TotalCount> totobj = (List<TotalCount>)ilist[0];
            //    model.Total = totobj.FirstOrDefault().Total;
            //    ViewBag.TotalCount = model.Total;
            //    var pager = new Pager(Convert.ToInt32(model.Total), 0);
            //    model.Pager = pager;
            //    //return View(model);
            //}

            //return PartialView("_revisiondetails", model);

            //int currPage = TempData["CurrPage"] == null ? 1 : Convert.ToInt32(TempData.Peek("CurrPage"));

            int currPage = page == null ? 1 : Convert.ToInt32(page);

            TempData["CurrentPage"] = currPage;

            Reports.MorringOfficeEntities entities = new Reports.MorringOfficeEntities();

            var revisions = entities.Revisions.OrderByDescending(p => p.Id).Where(u => u.Mid == mid).AsEnumerable();

            TempData["TotalRecords"] = revisions.Count();
            revisions = revisions.Skip((CommonMethods.PAGESIZE * currPage) - CommonMethods.PAGESIZE).Take(CommonMethods.PAGESIZE).ToList();

            return PartialView("_revisiondetails", revisions);
        }

        [HttpPost]
        public ActionResult _masterrevisiondetails(string revincl)
        {
            ApproveIndex model = new ApproveIndex();
            using (ShipmentContaxt sc1 = new ShipmentContaxt())
            {
                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ShipmentContaxt"].ConnectionString);
                SqlDataAdapter adp = new SqlDataAdapter("select * from Revision where rnumber in (" + revincl + ")", con);
                DataTable dt = new DataTable();
                adp.Fill(dt);


                List<ContentDetails> Studentlist = new List<ContentDetails>();
                model.contentListing = CommonMethod.ConvertToList<ContentDetails>(dt);


                //List<object> lst = dt.AsEnumerable().ToList<ContentDetails>();
                //List<DataRow> list = dt.AsEnumerable().ToList();
                //var ilist = new ShipmentContaxt()
                //     .MultipleResults("select * from Revision where rnumber in ("+ revincl + ")")
                //   //.With<TotalCount>()
                //   .With<ContentDetails>()
                //   .Execute();
                //model.contentListing = (List<ContentDetails>)dt.Rows[0][0];
                //List<TotalCount> totobj = (List<TotalCount>)ilist[0];
                // model.Total = totobj.FirstOrDefault().Total;
                //ViewBag.TotalCount = model.Total;
                //var pager = new Pager(Convert.ToInt32(model.Total), 0);
                //model.Pager = pager;
                //return View(model);
            }
            return PartialView("_masterrevisiondetails", model);
        }

        public static class CommonMethod
        {
            public static List<T> ConvertToList<T>(DataTable dt)
            {
                var columnNames = dt.Columns.Cast<DataColumn>().Select(c => c.ColumnName.ToLower()).ToList();
                var properties = typeof(T).GetProperties();
                return dt.AsEnumerable().Select(row =>
                {
                    var objT = Activator.CreateInstance<T>();
                    foreach (var pro in properties)
                    {
                        if (columnNames.Contains(pro.Name.ToLower()))
                        {
                            try
                            {
                                pro.SetValue(objT, row[pro.Name]);
                            }
                            catch (Exception ex) { }
                        }
                    }
                    return objT;
                }).ToList();
            }
        }

        [HttpPost]
        [PreventSpam("_createrevision", 3, 1)]
        public ActionResult _createrevision(DetailsViewModel model)
        {
            List<string> items = new List<string>();
            foreach (var item in model.RevList)
            {
                if (item.Selected)
                {
                    string revno = item.Text;
                    items.Add(revno);
                }
            }
            string inclrvno = String.Join(",", items.ToArray());

            string rvno = model.masterrivisionmodel.MasterRevisionNo;
            ShipmentContaxt context = new ShipmentContaxt();
            MasterRevision txt = new MasterRevision();
            txt.MasterRevisionNo = rvno;
            txt.RevisionsIncluded = inclrvno;
            txt.CreatedDate = DateTime.Today;
            context.MasterRevisions.Add(txt);
            context.SaveChanges();
            //return View();
            return RedirectToAction("Dashboard", "Home", new { Area = "" });
        }

        public ActionResult testeditor()
        {
            return View();
        }

        public byte[] ShipSpecific(int id)
        {
            using (ShipmentContaxt sc1 = new ShipmentContaxt())
            {
                var docscontent = sc1.ShipSpecificContents.FirstOrDefault(m => m.MId == id);

                iTextSharp.text.Document document = new iTextSharp.text.Document(PageSize.A4.Rotate(), 10, 10, 10, 15);

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    BaseFont bfTimes = BaseFont.CreateFont(BaseFont.TIMES_ROMAN, BaseFont.CP1252, false);
                    PdfWriter writer = PdfWriter.GetInstance(document, memoryStream);

                    document.Open();

                    document.NewPage();

                    PdfPTable table = new PdfPTable(1)
                    {
                        HorizontalAlignment = 0,
                        TotalWidth = 820f,
                        LockedWidth = true,
                        SpacingAfter = 20f,
                        SpacingBefore = 20f
                    };

                    float[] widths = new float[1];

                    widths[0] = 820f;

                    table.SetWidths(widths);

                    PdfPTable headerTable = new PdfPTable(1)
                    {
                        HorizontalAlignment = Element.ALIGN_JUSTIFIED,
                        TotalWidth = 818f,
                        LockedWidth = true
                    };

                    float[] hWidths = new float[1];

                    hWidths[0] = 820f;

                    headerTable.SetWidths(hWidths);

                    table.AddCell(new PdfPCell(new Phrase(docscontent.Content)) { BorderColor = BaseColor.LIGHT_GRAY });

                    document.Add(table);

                    document.Close();

                    byte[] bytes = memoryStream.ToArray();

                    memoryStream.Close();

                    return bytes;
                }
            }
        }

        public byte[] PDF(int id)
        {
            using (ShipmentContaxt sc1 = new ShipmentContaxt())
            {
                var docscontent = sc1.DocsPagges.FirstOrDefault(m => m.Mid == id);

                iTextSharp.text.Document document = new iTextSharp.text.Document(PageSize.A4.Rotate(), 10, 10, 10, 15);

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    BaseFont bfTimes = BaseFont.CreateFont(BaseFont.TIMES_ROMAN, BaseFont.CP1252, false);
                    PdfWriter writer = PdfWriter.GetInstance(document, memoryStream);

                    document.Open();

                    document.NewPage();

                    PdfPTable table = new PdfPTable(1)
                    {
                        HorizontalAlignment = 0,
                        TotalWidth = 820f,
                        LockedWidth = true,
                        SpacingAfter = 20f,
                        SpacingBefore = 20f
                    };

                    float[] widths = new float[1];

                    widths[0] = 820f;

                    table.SetWidths(widths);

                    PdfPTable headerTable = new PdfPTable(1)
                    {
                        HorizontalAlignment = Element.ALIGN_JUSTIFIED,
                        TotalWidth = 818f,
                        LockedWidth = true
                    };

                    float[] hWidths = new float[1];

                    hWidths[0] = 820f;

                    headerTable.SetWidths(hWidths);

                    //Image logo = Image.GetInstance(reportHeaderInfo.Logo);
                    //Image rightImage = Image.GetInstance(reportHeaderInfo.Middle_RightImage);
                    //Image footerImage = Image.GetInstance(reportHeaderInfo.FooterImage);

                    ////===============================================
                    //headerTable.AddCell(new PdfPCell(logo, true)
                    //{
                    //    HorizontalAlignment = Element.ALIGN_LEFT,
                    //    Rowspan = 6,
                    //    Border = 0
                    //});

                    //headerTable.AddCell(new PdfPCell(new Phrase(reportHeaderInfo.Company, itextFontBold8))
                    //{
                    //    Border = 0
                    //});
                    //headerTable.AddCell(new PdfPCell(new Phrase(string.Format("Vessel Name: {0}", data.VesselName), itextFontBold8))
                    //{
                    //    Border = 0
                    //});
                    //headerTable.AddCell(new PdfPCell(rightImage, true)
                    //{
                    //    HorizontalAlignment = Element.ALIGN_RIGHT,
                    //    Rowspan = 7,
                    //    Border = 0
                    //});

                    //headerTable.AddCell(new PdfPCell(new Phrase(docscontent.Content))
                    //{
                    //    Border = 0
                    //});

                    table.AddCell(new PdfPCell(new Phrase(docscontent.Content)) { BorderColor = BaseColor.LIGHT_GRAY });

                    // Footer Image

                    //PdfPTable footerTable = new PdfPTable(1)
                    //{
                    //    HorizontalAlignment = Element.ALIGN_CENTER,
                    //    TotalWidth = 818f,
                    //    LockedWidth = true
                    //};

                    //footerTable.AddCell(new PdfPCell(footerImage, true)
                    //{
                    //    Border = 0
                    //});

                    //footerCell.AddElement(footerTable);

                    //table.AddCell(footerCell);


                    document.Add(table);

                    document.Close();

                    byte[] bytes = memoryStream.ToArray();

                    memoryStream.Close();

                    return bytes;
                }
            }
        }
    }
}