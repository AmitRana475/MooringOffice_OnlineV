using MenuLayer;
using Microsoft.AspNet.Identity;
using MSMPmodule;
using Newtonsoft.Json;
using Shipment49Web.Areas.MSPS.Models;
using Shipment49Web.Common;
using Shipment49Web.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Mvc;

namespace Shipment49Web.Areas.MSPS.Controllers
{
    [Authorize]
    [UserAuthenticationFilter]
    [ErrorClass]
    public class ApproveController : BaseController
    {
        // GET: MSPS/Approve

        //private readonly IMenuRepository sc;
        public ApproveController()
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

        public ActionResult Index()
        {
            using (ShipmentContaxt sc1 = new ShipmentContaxt())
            {

                // var data = sc1.DocsPagges.ToList();
                // var tt = data.FirstOrDefault();
                //return View(tt);

                ApproveIndex model = new ApproveIndex();
                var ilist = new ShipmentContaxt()
                     .MultipleResults("[dbo].[spCheckContent] 10,0")
                   .With<TotalCount>()
                   .With<ContentDetails>()
                   .Execute();
                model.contentListing = (List<ContentDetails>)ilist[1];
                List<TotalCount> totobj = (List<TotalCount>)ilist[0];
                model.Total = totobj.FirstOrDefault().Total;
                ViewBag.TotalCount = model.Total;
                var pager = new Pager(Convert.ToInt32(model.Total), 0);
                model.Pager = pager;
                return View(model);
            }
        }

        [HttpGet]
        public ActionResult CheckContent(Int32? id)
        {
            using (ShipmentContaxt context = new ShipmentContaxt())
            {
                ContentDetails model = new ContentDetails();

                /////get change content path

                var Menutest1 = context.SmartMenus.ToList();// from table
                string allmenu = Menutest1.Select(x => x.SmartMenuContent).FirstOrDefault();

                var response = JsonConvert.DeserializeObject<List<RootObject>>(allmenu);
                //var response = JsonConvert.DeserializeObject<RootObject>(allmenu);

                //string path = " >> ";int check = 0;
                //foreach (var vObj in response)
                //{
                //    if (check == 1)
                //    {
                //        break;
                //    }
                //    path += vObj.text;
                //    var childerncheck = vObj.children;
                //    if (childerncheck != null)
                //    {
                //        foreach (var vObj1 in vObj.children)
                //        {
                //            //var 

                //            path += " >> " + vObj1.text;
                //            var childerncheck1 = vObj1.children;
                //            if (childerncheck1 != null)
                //            {
                //                foreach (var vObj2 in vObj1.children)
                //                {
                //                   var ids = vObj2.id;
                //                    if (ids == id)
                //                    {
                //                        path += " >> " + vObj2.text;
                //                        check = 1;
                //                        break;
                //                    }
                //                }
                //            }
                //            if(check==1)
                //            {
                //                break;
                //            }
                //        }
                //    }


                //}

                //string completepath = path;

                //ViewBag.ContentChangePath = completepath;
                /////get change content path
                var revtblelist = context.Revisions.Where(x => x.Id == id).FirstOrDefault();

                var Mid = revtblelist.MId;
                var ShipId = revtblelist.RevisionType;
                var revtype = revtblelist.RevisionType;

                var path = revtblelist.ContentPath;
                ViewBag.ContentChangePath = path;
                if (revtype == "General")
                {
                    ViewBag.Type = "General";
                }
                else
                {
                    ViewBag.Type = "IMO-No : " + revtype + "";
                }

                //var docspagelisting = context.DocsPagges.ToList().Where(x => x.Mid == Mid && x.Subid==revtblelist.SubId && x.SubSubid == revtblelist.SubSubId).FirstOrDefault();
                var docspagelisting = context.DocsPagges.Where(x => x.Mid == Mid && x.ShipId == ShipId).FirstOrDefault();

                var revlist = context.Revisions.ToList();
                //var secondlast ="";
                var secondlast = revlist.OrderBy(x => x.Id).Where(x => x.MId == Mid && x.RevisionType == ShipId && x.Status == "Approved").Reverse().FirstOrDefault();

                if (secondlast != null)
                {
                    var awaitcheck = revlist.Where(x => x.Id == id).Select(x => x.Status).FirstOrDefault();
                    if (awaitcheck != "Awaiting")
                    {
                        if (secondlast.Status == "Approved")
                        {
                            secondlast = context.Revisions.ToList().OrderBy(x => x.Id).Where(x => x.MId == Mid && x.RevisionType == ShipId && x.Status == "Approved").Reverse().Skip(1).FirstOrDefault();
                        }
                    }
                    else
                    {
                        secondlast = revlist.OrderBy(x => x.Id).Where(x => x.MId == Mid && x.RevisionType == ShipId && x.Status == "Approved").Reverse().FirstOrDefault();
                    }
                }
                //if (docspagelisting != null)
                //{
                //    var maincontent = docspagelisting.Content;
                //    model.Content1 = maincontent;
                //}
                if (secondlast != null)
                {
                    var maincontent = secondlast.Content;
                    model.Content1 = maincontent;
                }
                else
                {
                    model.Content1 = revtblelist.Content;
                }

                model.RevisionType = revtype;
                model.MId = Mid;
                model.ShipId = ShipId;
                model.Status = revtblelist.Status;
                //model.SubId = revtblelist.SubId;
                //model.SubSubId = revtblelist.SubSubId;
                model.Content = revtblelist.Content;
                model.RNumber = revtblelist.RNumber;

                return View(model);
            }
        }

        [ValidateInput(false)]
        [HttpPost]
        [PreventSpam("CheckContent", 3, 1)]
        //public ActionResult CheckContent(ContentDetails model, Int32 Id, Int32 MId, string RevisionType, string Content, string submitButton)
        public ActionResult CheckContent(ContentDetails model, Int32 Id, Int32 MId, string submitButton)
        {
            using (ShipmentContaxt context = new ShipmentContaxt())
            {
                //// var Doctblelist = context.DocsPagges.ToList().Where(x => x.Mid == MId && x.Subid == SubId && x.SubSubid == SubSubId).FirstOrDefault();
                //var Doctblelist = context.DocsPagges.ToList().Where(x => x.Mid == MId && x.ShipId == model.RevisionType).FirstOrDefault();
                //if (Doctblelist == null)
                //{
                //    DocsPages txt = new DocsPages();
                //    txt.Content = model.Content;
                //    txt.Mid = model.MId;
                //    // txt.Subid = model.SubId;
                //    //txt.SubSubid = model.SubSubId;
                //    if (model.RevisionType == "General")
                //    {
                //        txt.ShipId = model.RevisionType;
                //        txt.CreateBy = "Admin";
                //        txt.CreatedDate = DateTime.Now;
                //    }
                //    if (model.RevisionType != "General")
                //    {
                //        ShipSpecificContent txt1 = new ShipSpecificContent();
                //        txt1.Content = model.Content;
                //        txt1.MId = model.MId;
                //        txt1.ShipId = model.RevisionType;
                //        txt1.CreatedDate = DateTime.Now;
                //        context.ShipSpecificContents.Add(txt1);
                //    }

                //}
                //else
                //{
                //    Doctblelist.Content = model.Content;
                //    Doctblelist.ModifiedDate = DateTime.Now;
                //    string username = User.Identity.GetUserName();
                //    Doctblelist.ModifiedBy = username;
                //    context.Entry(Doctblelist).State = EntityState.Modified;
                //}

                //var Revtblelist = context.Revisions.ToList().Where(x => x.MId == MId && x.SubId == SubId && x.SubSubId == SubSubId).Reverse().Skip(1).Take(1).SingleOrDefault();
                // var Revtblelist = context.Revisions.OrderByDescending(x=> x.Id).ToList().Skip(1).FirstOrDefault();

                var Revtblelist = context.Revisions.AsEnumerable().LastOrDefault(x => x.RNumber != null);

                var rno = "";
                if (Revtblelist == null)
                {
                    rno = "0";
                }
                else
                {
                    var maxLength = context.Revisions.Select(x => x.RNumber).Max();
                    //rno = Revtblelist.RNumber.ToString();
                    rno = maxLength.ToString();
                }

                //var RNumber = "";
                //if (rno == null || rno == "")
                //{
                //    var rl = context.Revisions.ToList().Where(x => x.RNumber != null).LastOrDefault();
                //    var addrnumber1 = Convert.ToDecimal(1);
                //    RNumber = addrnumber1.ToString();
                //}
                //else
                //{
                //    decimal rnum = Convert.ToDecimal(rno);
                //    rnum += 1;
                //    RNumber = rnum.ToString();
                //}

                var modifyRevTble = context.Revisions.FirstOrDefault(x => x.Id == Id);
                //modifyRevTble.RNumber = Convert.ToDecimal(RNumber);
                if (submitButton == "Approve")
                {
                    var Doctblelist = context.DocsPagges.FirstOrDefault(x => x.Mid == MId && x.ShipId == model.RevisionType);
                    if (Doctblelist == null)
                    {

                        if (model.RevisionType == "General")
                        {
                            DocsPages txt = new DocsPages();
                            txt.Content = model.Content;
                            txt.Mid = model.MId;
                            txt.ShipId = model.RevisionType;
                            txt.CreateBy = "Admin";
                            txt.CreatedDate = DateTime.Today;
                            context.DocsPagges.Add(txt);
                        }
                        if (model.RevisionType != "General")
                        {
                            ShipSpecificContent txt1 = new ShipSpecificContent();
                            txt1.Content = model.Content;
                            txt1.MId = model.MId;
                            txt1.ShipId = model.RevisionType;
                            txt1.CreatedDate = DateTime.Today;
                            context.ShipSpecificContents.Add(txt1);
                        }
                    }
                    else
                    {
                        string updatedText = model.RevisionText.Trim();

                        updatedText = updatedText.Replace("<new-text>", "");
                        updatedText = updatedText.Replace("</new-text>", "");

                        updatedText = updatedText.Replace("<ins>", "<new-text>");
                        updatedText = updatedText.Replace("</ins>", "</new-text>");

                        Doctblelist.Content = Regex.Replace(updatedText, @"<del>(.|\n)*?</del>", string.Empty);
                        Doctblelist.ModifiedDate = DateTime.Today;
                        string username = User.Identity.GetUserName();
                        Doctblelist.ModifiedBy = username;
                        context.Entry(Doctblelist).State = EntityState.Modified;
                    }

                    //var smartMenu = context.SmartMenus.FirstOrDefault();

                    //if (smartMenu != null)
                    //{
                    //    try
                    //    {
                    //        var dynamicMenus = JsonConvert.DeserializeObject<List<RootObject>>(smartMenu.SmartMenuContent);

                    //        var menuDefinition = dynamicMenus.FirstOrDefault(p => p.id == model.MId);

                    //        if (menuDefinition == null)
                    //        {
                    //            var results = dynamicMenus.Where(p => p.children.Where(u => u.id == model.MId).Count() > 0);
                    //            if (results.Count() == 0)
                    //            {
                    //                var count = dynamicMenus.Where(p => p.children.Where(u => u.children.Where(a => a.id == model.MId).Count() > 0).Count() > 0);
                    //                if (count.Count() > 0)
                    //                {
                    //                    modifyRevTble.RPrefix = count.First().children.First().prefix;
                    //                }
                    //            }
                    //            else
                    //                modifyRevTble.RPrefix = results.First().children.First().prefix;
                    //        }
                    //        else
                    //            modifyRevTble.RPrefix = menuDefinition.prefix;
                    //    }
                    //    catch (Exception ex)
                    //    {
                    //    }                        
                    //}

                    //var latestRevision = context.Revisions.Where(u => u.MId == model.MId && u.RNumber != null).OrderByDescending(p => p.Id).FirstOrDefault();


                    var Menutest1 = context.SmartMenus.ToList();// from table
                    string allmenu = Menutest1.Select(x => x.SmartMenuContent).FirstOrDefault();

                    var response = JsonConvert.DeserializeObject<List<RootObject>>(allmenu);
                    string path = " >> "; int check = 0;
                    string prefix = string.Empty;

                    foreach (var vObj in response)
                    {
                        if (vObj.id == MId)
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
                                if (vObj1.id == MId)
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
                                        if (vObj2.id == MId)
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

                    modifyRevTble.RPrefix = prefix;
                    var latestRevision = context.Revisions.OrderByDescending(u => u.Id).FirstOrDefault(u => u.MId == model.MId && u.RNumber != null);
                    modifyRevTble.RNumber = latestRevision == null ? 1 : (latestRevision.RNumber + 1);

                    modifyRevTble.Status = "Approved";
                    modifyRevTble.ApprovedBy = User.Identity.GetUserName();
                    modifyRevTble.RevisionText = model.RevisionText.Trim();
                }

                if (submitButton == "Reject")
                {
                    modifyRevTble.Status = "Rejected";
                    modifyRevTble.RPrefix = string.Empty;
                    modifyRevTble.RNumber = null;
                }

                modifyRevTble.ApproveDate = DateTime.Today;
                context.Entry(modifyRevTble).State = EntityState.Modified;
                context.SaveChanges();

                //return RedirectToAction("index");
                return RedirectToAction("Dashboard", "Home", new { Area = "" });
                //return RedirectToAction("index", "DetailsView", new { area = "MSPS", @id = model.MId });
            }
        }

        public ActionResult test()
        {
            return View();
        }
    }
}