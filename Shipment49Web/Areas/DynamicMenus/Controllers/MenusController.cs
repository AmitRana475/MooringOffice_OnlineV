using MenuLayer;
using Newtonsoft.Json;
using Shipment49Web.Areas.MSPS.Models;
using Shipment49Web.Common;
using Shipment49Web.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace Shipment49Web.Areas.DynamicMenus.Controllers
{
    [Authorize]
    [UserAuthenticationFilter]
    [ErrorClass]
    public class MenusController : BaseController
    {
        // GET: DynamicMenus/Menus
        //private readonly IMenuRepository sc;
        public MenusController()
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

        [HttpGet]
        public ActionResult Index()
        {
            SmartMenusDetails model = new SmartMenusDetails();
            using (ShipmentContaxt sc1 = new ShipmentContaxt())
            {
                var smartmenulist = sc1.SmartMenus.ToList();

                var _smartmenu = smartmenulist.FirstOrDefault();
                if (_smartmenu != null)
                {
                    model.HtmlContent = _smartmenu.HtmlContent;
                    model.SmartMenuContent = _smartmenu.SmartMenuContent;

                    var Menutest1 = smartmenulist;// from table
                    string allmenu = Menutest1.Select(x => x.SmartMenuContent).FirstOrDefault();

                    var response = JsonConvert.DeserializeObject<List<RootObject>>(allmenu);

                    ViewBag.Menus = response;

                    int id = 0;

                    if (response != null)
                    {
                        foreach (var vObj in response)
                        {
                            id = vObj.id > id ? vObj.id : id;

                            var childerncheck = vObj.children;
                            if (childerncheck != null)
                            {
                                foreach (var vObj1 in vObj.children)
                                {
                                    //var 

                                    id = vObj1.id > id ? vObj1.id : id; ;
                                    var childerncheck1 = vObj1.children;
                                    if (childerncheck1 != null)
                                    {
                                        foreach (var vObj2 in vObj1.children)
                                        {
                                            id = vObj2.id > id ? vObj2.id : id;
                                        }
                                    }
                                }
                            }
                        }
                    }

                    model.MaxId = id + 1;
                }
            }

            ViewBag.MenuLinkText = EnumHelperNew.SelectListFor<EnumHelper.DynamicMenuText>();

            return View(model);
        }


        //[HttpPost]
        //public ActionResult Index(MSPS.Models.DynamicMenuModel txt)
        //{
        //    return View();
        //}
        public ActionResult Index_old()
        {

            using (ShipmentContaxt sc1 = new ShipmentContaxt())
            {
                MenuIndex model = new MenuIndex();
                var ilist = new ShipmentContaxt()
                     .MultipleResults("[dbo].[spDynamicMenus] 10,0,'Menu'")
                   .With<TotalCount>()
                   .With<MenusDetails>()
                   .Execute();
                model.menuListing = (List<MenusDetails>)ilist[1];
                List<TotalCount> totobj = (List<TotalCount>)ilist[0];
                model.Total = totobj.FirstOrDefault().Total;
                ViewBag.TotalCount = model.Total;
                var pager = new Pager(Convert.ToInt32(model.Total), 0);
                model.Pager = pager;
                return View(model);
            }
        }
        public ActionResult Add()
        {
            return View();
        }

        [HttpPost]
        [PreventSpam("AddMenus", 3, 1)]

        public ActionResult Add(DynamicMenuModel txt)
        {
            using (ShipmentContaxt sc1 = new ShipmentContaxt())
            {
                if (ModelState.IsValid)
                {
                    txt.Menuss.MenuName = txt.Menuss.MenuName;

                    sc1.Menus.Add(txt.Menuss);
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
        public ActionResult Edit(int? id)
        {
            using (ShipmentContaxt context = new ShipmentContaxt())
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                DynamicMenuModel model = new MSPS.Models.DynamicMenuModel
                {
                    Menuss = context.Menus.Find(id)
                };

                if (model == null)
                {
                    return HttpNotFound();
                }
                return View(model);
            }
        }
        [HttpPost]
        [PreventSpam("EditMenus", 3, 1)]
        public ActionResult Edit(DynamicMenuModel model, int id)
        {
            using (ShipmentContaxt context = new ShipmentContaxt())
            {
                var modifymenuTble = context.Menus.ToList().FirstOrDefault(x => x.MId == id);

                modifymenuTble.MenuName = model.Menuss.MenuName;

                context.Entry(modifymenuTble).State = EntityState.Modified;
                context.SaveChanges();
            }
            return View();
        }

        [ValidateInput(false)]
        public ActionResult AddData(string Name, string HtmlData, string DataToBeChange, string PrefixToBeChange, string LinkToBeChange)
        {
            using (ShipmentContaxt sc1 = new ShipmentContaxt())
            {
                DynamicMenuModel txt = new DynamicMenuModel();

                if (ModelState.IsValid)
                {
                    var menucheck = sc1.SmartMenus.ToList().Count;

                    if (menucheck == 0)
                    {
                        SmartMenu model = new SmartMenu
                        {
                            SmartMenuContent = Name,
                            UpdatedDate = DateTime.Now
                        };

                        ExportMenu(ref model);

                        sc1.SmartMenus.Add(model);
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(DataToBeChange))
                        {
                            foreach (string nvp in DataToBeChange.Trim('|').Split('|'))
                            {
                                if (!string.IsNullOrEmpty(nvp))
                                {
                                    string[] NameValuePair = nvp.Split(':');

                                    string oldValue = string.Format("data-text=\"{0}\"", NameValuePair[0].Trim());
                                    string newValue = string.Format("data-text=\"{0}\"", NameValuePair[1].Trim());

                                    if (HtmlData.Contains(oldValue))
                                        HtmlData = HtmlData.Replace(oldValue, newValue);

                                    oldValue = string.Format("data-name=\"{0}\"", NameValuePair[0].Trim());
                                    newValue = string.Format("data-name=\"{0}\"", NameValuePair[1].Trim());

                                    if (HtmlData.Contains(oldValue))
                                        HtmlData = HtmlData.Replace(oldValue, newValue);

                                    oldValue = string.Format("<div class=\"dd-handle\">{0}</div>", NameValuePair[0].Trim());
                                    newValue = string.Format("<div class=\"dd-handle\">{0}</div>", NameValuePair[1].Trim());

                                    if (HtmlData.Contains(oldValue))
                                        HtmlData = HtmlData.Replace(oldValue, newValue);
                                }
                            }
                        }

                        if (!string.IsNullOrEmpty(PrefixToBeChange))
                        {
                            foreach (string prefix in PrefixToBeChange.Trim('|').Split('|'))
                            {
                                if (!string.IsNullOrEmpty(prefix))
                                {
                                    string[] NameValuePair = prefix.Split(':');

                                    string oldValue = string.Format("data-prefix=\"{0}\"", NameValuePair[0].Trim());
                                    string newValue = string.Format("data-prefix=\"{0}\"", NameValuePair[1].Trim());

                                    if (HtmlData.Contains(oldValue))
                                        HtmlData = HtmlData.Replace(oldValue, newValue);
                                }
                            }
                        }

                        if (!string.IsNullOrEmpty(LinkToBeChange))
                        {
                            foreach (string link in LinkToBeChange.Trim('|').Split('|'))
                            {
                                if (!string.IsNullOrEmpty(link))
                                {
                                    string[] NameValuePair = link.Split(':');

                                    string oldValue = string.Format("data-href=\"{0}\"", NameValuePair[0].Trim());
                                    string newValue = string.Format("data-href=\"{0}\"", NameValuePair[1].Trim());

                                    if (HtmlData.Contains(oldValue))
                                        HtmlData = HtmlData.Replace(oldValue, newValue);
                                }
                            }
                        }

                        var menuContent = sc1.SmartMenus.ToList().FirstOrDefault();
                        menuContent.SmartMenuContent = Name;
                        menuContent.HtmlContent = HtmlData;
                        menuContent.UpdatedDate = DateTime.Now;

                        ExportMenu(ref menuContent);

                        sc1.Entry(menuContent).State = EntityState.Modified;

                        foreach (var v in JsonConvert.DeserializeObject<List<RootObject>>(Name))
                        {
                            var rev1 = sc1.Revisions.Where(p => p.MId == v.id).ToList();
                            foreach (var r in rev1)
                            {
                                r.RPrefix = v.prefix;
                                sc1.Entry(r).State = EntityState.Modified;
                            }

                            foreach (var v1 in v.children)
                            {
                                var rev2 = sc1.Revisions.Where(p => p.MId == v1.id).ToList();
                                foreach (var r in rev2)
                                {
                                    r.RPrefix = v1.prefix;
                                    sc1.Entry(r).State = EntityState.Modified;
                                }

                                foreach (var v2 in v1.children)
                                {
                                    var rev3 = sc1.Revisions.Where(p => p.MId == v2.id).ToList();
                                    foreach (var r in rev3)
                                    {
                                        r.RPrefix = v2.prefix;
                                        sc1.Entry(r).State = EntityState.Modified;
                                    }
                                }
                            }
                        }
                    }

                    sc1.SaveChanges();

                    return RedirectToAction("index");
                }
            }

            return View();
        }

        private void UpdatePrefix()
        {

        }


        private void ExportMenu(ref SmartMenu smartMenuData)
        {
            var menuList = JsonConvert.DeserializeObject<List<RootObject>>(smartMenuData.SmartMenuContent);

            // child == 2 or parent == 2
            var parentList = menuList.Where(p => p.type == 2 || (p.children.Count > 0)).ToList();

            for (int i = 0; i < parentList.Count; i++)
            {
                var c = parentList[i].children.Where(u => u.type == 3).ToList();

                for (int j = 0; j < c.Count; j++)
                {
                    var c1 = c[j].children.Where(u => u.type == 3).ToList();
                    for (int k = 0; k < c1.Count; k++)
                    {
                        c[j].children.Remove(c1[k]);
                    }

                    if (c[j].children.Count == 0)
                    {
                        parentList[i].children.Remove(c[j]);
                    }
                }

                if (parentList[i].children.Count() == 0 && parentList[i].type != 2)
                {
                    parentList.Remove(parentList[i]);
                    i--;
                }
            }

            smartMenuData.SmartMenuContentExport = JsonConvert.SerializeObject(parentList);
        }

        //private void ExportMenu(ref SmartMenu smartMenuData)
        //{
        //    RootObject exportedMenuJson = new RootObject();

        //    string htmlMenu = "<ul>";

        //    var response = JsonConvert.DeserializeObject<List<RootObject>>(smartMenuData.SmartMenuContent);

        //    // type = 1 means blank, 2 means text, 3 means hyperlink

        //    foreach (var vObj in response)
        //    {
        //        if (vObj.type != 3)
        //        {
        //            exportedMenuJson = vObj;
        //            if (vObj.children.Count == 0)
        //            {
        //                htmlMenu += string.Format(@"<li class='dd-item' data-id='{0}' data-text='{1}' data-prefix='{2}' data-type='{3}' data-href='{4}' data-slug='{5}' 
        //                data-new='{6}' data-deleted='{7}'><div class='dd-handle'>{8}</div><span class='button-delete btn btn-default btn-xs pull-right' data-owner-id='{9}'>
        //                <i class='fa fa-times-circle-o' aria-hidden='true'></i></span><span class='button-edit btn btn-default btn-xs pull-right' data-owner-id='{10}'>
        //                <i class='fa fa-pencil' aria-hidden='true'></i></span></li>", vObj.id, vObj.text, vObj.prefix, vObj.type, vObj.href, vObj.slug, vObj.@new,
        //                        vObj.deleted, vObj.text, vObj.id, vObj.id);
        //            }
        //            else
        //            {
        //                htmlMenu += string.Format(@"<li class='dd-item' data-id='{0}' data-text='{1}' data-prefix='{2}' data-type='{3}' data-href='' 
        //                data-slug='{4}' data-new='{5}' data-deleted='{6}'><div class='dd-handle'>{7}</div><ol class='dd-list'>", vObj.id, vObj.text,
        //                    vObj.prefix, vObj.type, vObj.slug, vObj.@new, vObj.deleted, vObj.text);
        //            }
        //        }

        //        var childerncheck = vObj.children;
        //        if (childerncheck != null)
        //        {
        //            foreach (var vObj1 in vObj.children)
        //            {
        //                Child childMenuJson = vObj1;
        //                if (vObj1.type != 3)
        //                {
        //                    if (vObj1.children.Count == 0)
        //                    {
        //                        htmlMenu += string.Format(@"<li class='dd-item' data-id='{0}' data-text='{1}' data-prefix='{2}' data-type='{3}' data-href='{4}' data-slug='{5}' 
        //                            data-new='{6}' data-deleted='{7}'><div class='dd-handle'>{8}</div><span class='button-delete btn btn-default btn-xs pull-right' data-owner-id='{9}'>
        //                            <i class='fa fa-times-circle-o' aria-hidden='true'></i></span><span class='button-edit btn btn-default btn-xs pull-right' data-owner-id='{10}'>
        //                            <i class='fa fa-pencil' aria-hidden='true'></i></span></li>", vObj1.id, vObj1.text, vObj1.prefix, vObj1.type, vObj1.href, vObj1.slug, vObj1.@new,
        //                            vObj1.deleted, vObj1.text, vObj1.id, vObj1.id);
        //                    }
        //                    else
        //                    {
        //                        htmlMenu += string.Format(@"<li class='dd-item' data-id='{0}' data-text='{1}' data-prefix='{2}' data-type='{3}' data-href='' 
        //                            data-slug='{4}' data-new='{5}' data-deleted='{6}'><div class='dd-handle'>{7}</div><ol class='dd-list'>", vObj1.id, vObj1.text,
        //                            vObj1.prefix, vObj1.type, vObj1.slug, vObj1.@new, vObj1.deleted, vObj1.text);
        //                    }
        //                }

        //                var childerncheck1 = vObj1.children;
        //                if (childerncheck1 != null)
        //                {
        //                    foreach (var vObj2 in vObj1.children)
        //                    {
        //                        childMenuJson.children.Add(vObj2);

        //                        if (vObj2.type != 3)
        //                        {
        //                            htmlMenu += string.Format(@"<li class='dd-item' data-id='{0}' data-text='{1}' data-prefix='{2}' data-type='{3}' data-href='{4}' data-slug='{5}' 
        //                                    data-new='{6}' data-deleted='{7}'><div class='dd-handle'>{8}</div><span class='button-delete btn btn-default btn-xs pull-right' data-owner-id='{9}'>
        //                                    <i class='fa fa-times-circle-o' aria-hidden='true'></i></span><span class='button-edit btn btn-default btn-xs pull-right' data-owner-id='{10}'>
        //                                    <i class='fa fa-pencil' aria-hidden='true'></i></span></li>", vObj1.id, vObj1.text, vObj1.prefix, vObj1.type, vObj1.href, vObj1.slug, vObj1.@new,
        //                                vObj1.deleted, vObj1.text, vObj1.id, vObj1.id);
        //                        }
        //                    }
        //                }

        //                exportedMenuJson.children.Add(childMenuJson);
        //            }
        //        }
        //    }

        //    smartMenuData.HtmlContentExport = htmlMenu + "</ul>";
        //    smartMenuData.SmartMenuContentExport = JsonConvert.SerializeObject(exportedMenuJson);
        //}

        //private void ExportMenu(ref SmartMenu smartMenuData)
        //{
        //    //string htmlMenu = "<ul>";
        //    //string parent = string.Empty;
        //    //string parentWithChildren = string.Empty;
        //    //string children = string.Empty;

        //    var menuList = JsonConvert.DeserializeObject<List<RootObject>>(smartMenuData.SmartMenuContent);

        //    // child == 2 or parent == 2
        //    var parentList = menuList.Where(p => p.type == 2 || (p.children.Count > 0)).ToList();

        //    //for (int i = 0; i < parentList.Count; i++)
        //    //{
        //    //    // Parent i.e. without Children
        //    //    parent = string.Format(@"<li class='dd-item' data-id='{0}' data-text='{1}' data-prefix='{2}' data-type='{3}' data-href='{4}' data-slug='{5}' 
        //    //            data-new='{6}' data-deleted='{7}'><div class='dd-handle'>{8}</div><span class='button-delete btn btn-default btn-xs pull-right' data-owner-id='{9}'>
        //    //            <i class='fa fa-times-circle-o' aria-hidden='true'></i></span><span class='button-edit btn btn-default btn-xs pull-right' data-owner-id='{10}'>
        //    //            <i class='fa fa-pencil' aria-hidden='true'></i></span></li>", parentList[i].id, parentList[i].text, parentList[i].prefix, parentList[i].type,
        //    //        parentList[i].href, parentList[i].slug, parentList[i].@new, parentList[i].deleted, parentList[i].text, parentList[i].id, parentList[i].id);

        //    //    // Parent i.e. with Children
        //    //    parentWithChildren = string.Format(@"<li class='dd-item' data-id='{0}' data-text='{1}' data-prefix='{2}' data-type='{3}' data-href='' 
        //    //            data-slug='{4}' data-new='{5}' data-deleted='{6}'><div class='dd-handle'>{7}</div><ol class='dd-list'>", parentList[i].id, parentList[i].text,
        //    //        parentList[i].prefix, parentList[i].type, parentList[i].slug, parentList[i].@new, parentList[i].deleted, parentList[i].text);

        //    //    parentList[i].children = parentList[i].children.ToList(); //.Where(p => p.type == 2 || (p.children.Count > 0)).ToList();

        //    //    var childrenBasedUpontype = parentList[i].children.Where(p => p.type != 3).Count();

        //    //    if (childrenBasedUpontype != 0)
        //    //    {
        //    //        if (parentList[i].children.Count == 0)
        //    //            htmlMenu += parent;
        //    //        else
        //    //            htmlMenu += parentWithChildren;

        //    //        for (int j = 0; j < parentList[i].children.Count; j++)
        //    //        {
        //    //            parentList[i].children = parentList[i].children.ToList(); //.Where(p => p.type == 2).ToList();

        //    //            if (parentList[i].children.Count > 0)
        //    //            {
        //    //                for (int k = 0; k < parentList[i].children.Count; k++)
        //    //                {
        //    //                    int menuType = parentList[i].children[k].type;

        //    //                    if (menuType != 3)
        //    //                    {
        //    //                        children = string.Format(@"<li class='dd-item' data-id='{0}' data-text='{1}' data-prefix='{2}' data-type='{3}' data-href='{4}' 
        //    //                    data-slug='{5}' data-new='{6}' data-deleted='{7}'><div class='dd-handle'>{8}</div><span class='button-delete btn btn-default 
        //    //                    btn-xs pull-right' data-owner-id='{9}'><i class='fa fa-times-circle-o' aria-hidden='true'></i></span><span class='button-edit btn 
        //    //                    btn-default btn-xs pull-right' data-owner-id='{10}'><i class='fa fa-pencil' aria-hidden='true'></i></span></li></ol></li>",
        //    //                        parentList[i].children[k].id, parentList[i].children[k].text, parentList[i].children[k].prefix, parentList[i].children[k].type,
        //    //                        parentList[i].children[k].href, parentList[i].children[k].slug, parentList[i].children[k].@new, parentList[i].children[k].deleted,
        //    //                        parentList[i].children[k].text, parentList[i].children[k].id, parentList[i].children[k].id);

        //    //                        htmlMenu += children;

        //    //                        List<Child2> child2 = parentList[i].children[k].children.ToList(); //.Where(p => p.type == 2).ToList();

        //    //                        foreach (var chi in child2)
        //    //                        {
        //    //                            children = string.Format(@"<li class='dd-item' data-id='{0}' data-text='{1}' data-prefix='{2}' data-type='{3}' data-href='{4}' 
        //    //                    data-slug='{5}' data-new='{6}' data-deleted='{7}'><div class='dd-handle'>{8}</div><span class='button-delete btn btn-default 
        //    //                    btn-xs pull-right' data-owner-id='{9}'><i class='fa fa-times-circle-o' aria-hidden='true'></i></span><span class='button-edit btn 
        //    //                    btn-default btn-xs pull-right' data-owner-id='{10}'><i class='fa fa-pencil' aria-hidden='true'></i></span></li></ol></li>",
        //    //                            chi.id, chi.text, chi.prefix, chi.type, chi.href, chi.slug, chi.@new, chi.deleted, chi.text, chi.id, chi.id);

        //    //                            htmlMenu += children;
        //    //                        }
        //    //                    }
        //    //                }
        //    //            }
        //    //        }
        //    //    }
        //    //}

        //    for (int i = 0; i < parentList.Count; i++)
        //    {
        //        var c = parentList[i].children.Where(u => u.type == 3).ToList();

        //        for (int j = 0; j < c.Count; j++)
        //        {
        //            var c1 = c[j].children.Where(u => u.type == 3).ToList();
        //            for (int k = 0; k < c1.Count; k++)
        //            {
        //                c[j].children.Remove(c1[k]);
        //            }

        //            if (c[j].children.Count == 0)
        //            {
        //                parentList[i].children.Remove(c[j]);
        //            }
        //        }

        //        if (parentList[i].children.Count() == 0 && parentList[i].type != 2)
        //        {
        //            parentList.Remove(parentList[i]);
        //            i--;
        //        }
        //    }

        //    //smartMenuData.HtmlContentExport = htmlMenu + "</ul>";
        //    smartMenuData.SmartMenuContentExport = JsonConvert.SerializeObject(parentList);
        //}
    }
}