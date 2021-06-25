using MenuLayer;
using Microsoft.AspNet.Identity;
using MSMPmodule;
using Shipment49Web.Areas.MSPS.Models;
using Shipment49Web.Common;
using Shipment49Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Shipment49Web.Areas.MSPS.Controllers
{
    [Authorize]
    [UserAuthenticationFilter]
    [ErrorClass]
    public class introductionController : BaseController
    {
        // GET: MSPS/introduction
        //private readonly IMenuRepository sc;
        public introductionController()
        {
            //sc = repo;

            //if (UserRole.username == null)
            //{
            //    UserRole.username = string.Join("", Roles.GetRolesForUser());
            //}
            //ViewBag.GetMenu = UserRole.username == "Admin" ? sc.Menus.ToList() : sc.Menus.Where(x => x.Role == "User").ToList();
            //ViewBag.GetSubMenu = UserRole.username == "Admin" ? sc.SubMenus.ToList() : sc.SubMenus.Where(x => x.Role == "User").ToList();
        }
        public ActionResult Index()
        {
            using (ShipmentContaxt sc1 = new ShipmentContaxt())
            {
                DocumentIndex model = new DocumentIndex();
                var ilist = new ShipmentContaxt()
                     .MultipleResults("[dbo].[spRevisionDetails] 10,0")
                   .With<TotalCount>()
                   .With<RevisionDetails>()
                   .Execute();
                model.docListing = (List<RevisionDetails>)ilist[1];
                List<TotalCount> totobj = (List<TotalCount>)ilist[0];
                model.Total = totobj.FirstOrDefault().Total;
                ViewBag.TotalCount = model.Total;
                var pager = new Pager(Convert.ToInt32(model.Total), 0);
                model.Pager = pager;
                return View(model);
            }
        }
        public ActionResult GetRevision(Int32 page)
        {
            DocumentIndex model = new DocumentIndex();
            var ilist = new ShipmentContaxt()
                 .MultipleResults("[dbo].[spRevisionDetails] 10" + "," + (page - 1) + "")
               .With<TotalCount>()
               .With<RevisionDetails>()
               .Execute();
            model.docListing = (List<RevisionDetails>)ilist[1];
            List<TotalCount> totobj = (List<TotalCount>)ilist[0];
            model.Total = totobj.FirstOrDefault().Total;
            ViewBag.TotalCount = model.Total;
            var pager = new Pager(Convert.ToInt32(model.Total), 0);
            model.Pager = pager;
            //return PartialView(model);
            return PartialView("~/Areas/MSPS/Views/Shared/RevisionDetails.cshtml", model);
        }


        [HttpPost]
        [PreventSpam("Document", 3, 1)]
        public ActionResult Document(Document txt)
        {
            using (ShipmentContaxt sc1 = new ShipmentContaxt())
            {
                if (ModelState.IsValid)
                {
                    txt.CreatedDate = DateTime.Now;
                    sc1.Documents.Add(txt);


                    sc1.SaveChanges();
                    return RedirectToAction("index");
                }
                else
                {
                    return View(txt);
                }
            }
        }
        [HttpGet]
        public ActionResult Document()
        {
            return View();


        }

        public class Pagetitles
        {
            public string Pagetitle { get; set; }
        }

        // Json Call to get Document  
        public JsonResult GetDocument(int id)
        {
            ShipmentContaxt sc1 = new ShipmentContaxt();
            //List<SelectListItem> docs = new List<SelectListItem>();
            //var docsList = sc1.DocsPagges.ToList().Where(m => m.DocsID == id).ToList();

            var docsList = "";
            var docData = docsList.Select(m => new SelectListItem()
            {
                //Text = m.PageTitle,
                //Value = m.PageTitle.ToString(),
            });
            return Json(docData, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetContent(int id, string textval)
        {
            ShipmentContaxt sc1 = new ShipmentContaxt();
            //List<SelectListItem> docs = new List<SelectListItem>();
            //var docscontent = sc1.DocsPagges.ToList().Where(m => m.DocsID == id && m.PageTitle==textval).Select(x=> x.Content).FirstOrDefault();

            var docscontent = "";
            //var docData = docsList.Select(m => new SelectListItem()
            //{
            //    Text = m.Content,
            //    Value = m.DocsID.ToString(),
            //});
            return Json(docscontent, JsonRequestBehavior.AllowGet);
        }

        // Get State from DB by country ID  
        //public IList<Pagetitles> Getstate(int CountryId)
        //{
        //    ShipmentContaxt sc1 = new ShipmentContaxt();
        //    var ss = sc1.DocsPagges.ToList().Where(m => m.DocsID == CountryId).ToList();

        //    return ss.ToString();
        //}

        [HttpGet]
        public ActionResult Edit(Int32? id)
        {
            using (ShipmentContaxt context = new ShipmentContaxt())
            {
                var Doumentlist = context.Documents.ToList();
                ViewBag.Doclist = Doumentlist.AsQueryable().Select(x => new SelectListItem { Text = x.DocumentName, Value = x.DocumentID.ToString() }).ToList();
                ViewBag.SelectedidDoc = Doumentlist.AsQueryable().Where(x => x.DocumentID == id).Select(x => x.DocumentName).FirstOrDefault();

                return View();
            }
        }


        [HttpPost]
        [PreventSpam("EditIntro", 3, 1)]
        public ActionResult Edit(TempDocsPageModel txt)
        {
            using (ShipmentContaxt sc1 = new ShipmentContaxt())
            {
                if (ModelState.IsValid)
                {


                    //var docid = txt.tempdoc.DocsId;

                    var Doumentlist = sc1.Documents.ToList();

                    //var docname = Doumentlist.Where(x => x.DocumentID == docid).Select(x => x.DocumentName).FirstOrDefault();

                    //txt.tempdoc.DocsId = docid;
                    // txt.tempdoc.PageTitle = txt.tempdoc.PageTitle;
                    txt.tempdoc.Content = txt.tempdoc.Content;
                    string username = User.Identity.GetUserName();

                    txt.tempdoc.CreatedBy = username;
                    txt.tempdoc.CreatedDate = DateTime.Now;
                    txt.tempdoc.ApprovedDate = DateTime.Now;

                    sc1.TempDocsPages.Add(txt.tempdoc);
                    sc1.SaveChanges();
                    return RedirectToAction("index");
                }
                else
                {
                    return View(txt);
                }
            }
        }

        [HttpGet]
        public ActionResult Add()
        {
            using (ShipmentContaxt context = new ShipmentContaxt())
            {
                var Doumentlist = context.Documents.ToList();
                ViewBag.Doclist = Doumentlist.AsQueryable().Select(x => new SelectListItem { Text = x.DocumentName, Value = x.DocumentID.ToString() }).ToList();
                return View();
            }

        }

        [HttpPost]
        [PreventSpam("AddIntro", 3, 1)]
        public ActionResult Add(DocEditorModel txt)
        {

            using (ShipmentContaxt sc1 = new ShipmentContaxt())
            {
                if (ModelState.IsValid)
                {
                    //var rr = txt.Revisions.RNumber;
                    //var rr = Convert.ToDecimal(0.1);
                    txt.Revisions.RNumber = 1;
                    txt.Revisions.ReviseDate = DateTime.Now;
                    txt.Revisions.CreateBy = "Admin";
                    txt.Revisions.ApprovedBy = "Admin";

                    //var docid = txt.Revisions.DocumentID;

                    var Doumentlist = sc1.Documents.ToList();

                    // var docname = Doumentlist.Where(x => x.DocumentID== docid).Select(x => x.DocumentName).FirstOrDefault();

                    //txt.Revisions.DocumentID = docid;
                    //txt.DocumentPages.DocsID = docid;
                    // txt.Revisions.DocsName = docname;
                    var Revisionstbl = txt.Revisions;

                    //var checkdupli = sc1.Revisions.ToList().Where(x => x.DocumentID == docid).FirstOrDefault();

                    //if (checkdupli == null)
                    //{
                    //    sc1.Revisions.Add(txt.Revisions);
                    //}

                    sc1.DocsPagges.Add(txt.DocumentPages);
                    sc1.SaveChanges();
                    return RedirectToAction("index");
                }
                else
                {
                    return View(txt);
                }
            }


        }


    }
}