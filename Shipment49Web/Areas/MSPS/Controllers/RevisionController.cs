using MenuLayer;
using Shipment49Web.Areas.MSPS.Models;
using Shipment49Web.Common;
using Shipment49Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace Shipment49Web.Areas.MSPS.Controllers
{
    [Authorize]
    [UserAuthenticationFilter]
    [ErrorClass]
    public class RevisionController : BaseController
    {
        // GET: MSPS/Revision

        //private readonly IMenuRepository sc;
        public RevisionController()
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
                RevisionIndex model = new RevisionIndex();
                var ilist = new ShipmentContaxt()
                     .MultipleResults("[dbo].[spUpdatedRevisionList] 10,0")
                   .With<TotalCount>()
                   .With<RevisionListing>()
                   //.With<RevisionView>()
                   .Execute();
                model.revisionListing = (List<RevisionListing>)ilist[1];
                //model.revisionViewListing = (List<RevisionView>)ilist[1];
                List<TotalCount> totobj = (List<TotalCount>)ilist[0];
                model.Total = totobj.FirstOrDefault().Total;
                ViewBag.TotalCount = model.Total;
                var pager = new Pager(Convert.ToInt32(model.Total), 0);
                model.Pager = pager;
                return View(model);
            }
        }

        public ActionResult RevisionDetails(Int32 id, string PageTitle)
        {
            using (ShipmentContaxt sc1 = new ShipmentContaxt())
            {
                RevisionIndex model = new RevisionIndex();
                var ilist = new ShipmentContaxt()
                     .MultipleResults("[dbo].[spViewRevisionDetails] " + id + ",'" + PageTitle + "'")
                     .With<RevisionView>()
                   .Execute();
                model.revisionViewListing = (List<RevisionView>)ilist[0];
                //model.revisionViewListing = (List<RevisionView>) ( model.revisionListing.Where(x => x.DocsId == id));

                var docid = model.revisionViewListing.Select(x => x.DocsId).SingleOrDefault();
                var pgtitle = model.revisionViewListing.Select(x => x.PageTitle).SingleOrDefault();
                var content = model.revisionViewListing.Select(x => x.Content).SingleOrDefault();

                //List<TotalCount> totobj = (List<TotalCount>)ilist[0];
                //model.Total = totobj.FirstOrDefault().Total;
                //ViewBag.TotalCount = model.Total;
                //var pager = new Pager(Convert.ToInt32(model.Total), 0);
                //model.Pager = pager;

                var docspagelisting = sc1.DocsPagges.ToList();
                //var anothercontent = docspagelisting.Where(x => x.DocsID == docid && x.PageTitle == pgtitle).Select(x => x.Content).FirstOrDefault();
                var anothercontent = docspagelisting.Select(x => x.Content).FirstOrDefault();
                string str = Regex.Replace(HttpUtility.HtmlDecode(content), "<.*?>", string.Empty);
                string str1 = Regex.Replace(HttpUtility.HtmlDecode(anothercontent), "<.*?>", string.Empty);
                model.Content = str;
                model.Content1 = str1;

                return PartialView("~/Areas/MSPS/Views/Shared/RivisionDetails.cshtml", model);
                // return PartialView(model);
            }
        }
    }
}