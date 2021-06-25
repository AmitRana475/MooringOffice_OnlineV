using MenuLayer;
using Microsoft.AspNet.Identity;
using Shipment49Web.Common;
using Shipment49Web.Models;
using Shipment49Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace Shipment49Web.Areas.Notifications.Controllers
{
    [Authorize]
    [UserAuthenticationFilter]
    [ErrorClass]
    public class NotificationInfoController : BaseController
    {
        public NotificationInfoController()
        {
            using (ShipmentContaxt sc1 = new ShipmentContaxt())
            {
                var _smartmenu = sc1.SmartMenus.ToList().FirstOrDefault();
                if (_smartmenu != null)
                {
                    ViewBag.MenuContent = _smartmenu.SmartMenuContent;
                }
            }
        }

        //private void SetViewBag()
        //{
        //    using (ShipmentContaxt sc1 = new ShipmentContaxt())
        //    {
        //        ViewBag.FleetNames = sc1.MasterCommons.Where(e => e.Type == CommonType.FleetName).ToList();
        //        ViewBag.FleetTypes = sc1.MasterCommons.Where(e => e.Type == CommonType.FleetType).ToList();
        //        ViewBag.TradePlatforms = sc1.MasterCommons.Where(e => e.Type == CommonType.TradePlatform).ToList();
        //        ViewBag.VesselInfos = sc1.Vessels.ToList();
        //    }
        //}

        public ActionResult Index(int? id)
        {
            //SetViewBag();

            using (Reports.MorringOfficeEntities context = new Reports.MorringOfficeEntities())
            {
                NotificationSearchViewModel notificationSearchViewModel = new NotificationSearchViewModel();

                notificationSearchViewModel.DateFrom = DateTime.Today.AddMonths(-1);
                notificationSearchViewModel.DateTo = DateTime.Today;

                notificationSearchViewModel.VesselList = PermittedVessels;
                notificationSearchViewModel.FleetNameList = PermittedFleetNames; // context.tblCommons.Where(u => u.Type == (int)CommonType.FleetName).ToList();
                notificationSearchViewModel.FleetTypeList = PermittedFleetTypes; // context.tblCommons.Where(u => u.Type == (int)CommonType.FleetType).ToList();
                notificationSearchViewModel.TradePlatformList = context.tblCommons.Where(u => u.Type == (int)CommonType.TradePlatform).ToList();

                var acknowledged = context.View_Notifications.Where(p => p.Acknowledge == false).AsQueryable();

                var unacknowledged = context.View_Notifications.
                    Where(p => p.Acknowledge == true && p.NotificationDate >= notificationSearchViewModel.DateFrom && p.NotificationDate <= notificationSearchViewModel.DateTo).
                    OrderByDescending(u => u.NotificationDate).AsQueryable();

                var recordsFound = acknowledged.Concat(unacknowledged).OrderBy(p => p.Acknowledge);

                int currPage = id == null ? 1 : Convert.ToInt32(id);

                TempData["CurrentPage"] = currPage;
                TempData["TotalRecords"] = recordsFound.Count();

                notificationSearchViewModel.NotificationsList = recordsFound.Skip((CommonMethods.PAGESIZE * currPage) - CommonMethods.PAGESIZE).Take(CommonMethods.PAGESIZE).ToList();

                return View(notificationSearchViewModel);
            }
        }

        [HttpPost]
        public ActionResult Index(NotificationSearchViewModel searchViewModel)
        {
            //SetViewBag();

            using (Reports.MorringOfficeEntities context = new Reports.MorringOfficeEntities())
            {
                NotificationSearchViewModel notificationSearchViewModel = new NotificationSearchViewModel
                {
                    VesselList = PermittedVessels, // context.VesselDetails.ToList(),
                    FleetNameList = PermittedFleetNames, // context.tblCommons.Where(u => u.Type == (int)CommonType.FleetName).ToList(),
                    FleetTypeList = PermittedFleetTypes, // context.tblCommons.Where(u => u.Type == (int)CommonType.FleetType).ToList(),
                    TradePlatformList = context.tblCommons.Where(u => u.Type == (int)CommonType.TradePlatform).ToList(),
                };

                var notificationInfo = context.View_Notifications.Where(p => p.NotificationDate >= searchViewModel.DateFrom && p.NotificationDate <= searchViewModel.DateTo).
                    OrderByDescending(u => u.NotificationDate).AsQueryable();

                if (searchViewModel.VesselIDs != null)
                    if (searchViewModel.VesselIDs.Count > 0)
                        notificationInfo = notificationInfo.Where(p => searchViewModel.VesselIDs.Contains(p.VesselId)).AsQueryable();

                if (searchViewModel.FleetIDs != null)
                    if (searchViewModel.FleetIDs.Count > 0)
                        notificationInfo = notificationInfo.Where(p => searchViewModel.FleetIDs.Contains(p.FleetNameID)).AsQueryable();

                if (searchViewModel.FleetTypeIDs != null)
                    if (searchViewModel.FleetTypeIDs.Count > 0)
                        notificationInfo = notificationInfo.Where(p => searchViewModel.FleetTypeIDs.Contains(p.FleetTypeID)).AsQueryable();

                if (searchViewModel.TradeIDs != null)
                    if (searchViewModel.TradeIDs.Count > 0)
                        notificationInfo = notificationInfo.Where(p => searchViewModel.TradeIDs.Contains(p.TradeAreaID)).AsQueryable();

                var results = notificationInfo.ToList();

                int currPage = TempData["CurrentPage"] == null ? 1 : Convert.ToInt32(TempData["CurrentPage"]);

                //TempData["CurrentPage"] = currPage;
                TempData["TotalRecords"] = results.Count();

                var recordsFound = results.Skip((CommonMethods.PAGESIZE * currPage) - CommonMethods.PAGESIZE).Take(CommonMethods.PAGESIZE).AsQueryable();

                notificationSearchViewModel.NotificationsList = recordsFound.ToList();

                return View(notificationSearchViewModel);
            }
        }

        //[HttpPost]
        //public ActionResult Search(NotificationSearchViewModel model)
        //{
        //    SetViewBag();
        //    using (ShipmentContaxt sc1 = new ShipmentContaxt())
        //    {
        //        var list = (from nt in sc1.NotificationInfos
        //                    join vt in sc1.Vessels on nt.VesselId equals vt.Id
        //                    join fn in sc1.MasterCommons on vt.FleetNameID equals fn.Id
        //                    join ft in sc1.MasterCommons on vt.FleetTypeID equals ft.Id
        //                    join td in sc1.MasterCommons on vt.TradeAreaID equals td.Id
        //                    where vt.Id == model.VesselInfo
        //                    || fn.Id == model.FleetName
        //                    || ft.Id == model.FleetType
        //                    || td.Id == model.TradePlatform
        //                    //|| (!string.IsNullOrEmpty(model.DateFrom)) ? Convert.ToDateTime(nt.CreatedDate).Date >= Convert.ToDateTime(model.DateFrom).Date : true
        //                    //|| (!string.IsNullOrEmpty(model.DateTo)) ? Convert.ToDateTime(nt.CreatedDate).Date <= Convert.ToDateTime(model.DateTo).Date : true
        //                    select nt).ToList();
        //        ViewBag.List = list;
        //        return View("Index");
        //    }
        //}

        [HttpPost]
        public ActionResult SetAcknowledge(int id)
        {
            using (Reports.MorringOfficeEntities entities = new Reports.MorringOfficeEntities())
            {
                var obj = entities.tblNotifications.FirstOrDefault(e => e.Id == id);
                if (obj != null)
                {
                    obj.AcknDateTime = DateTime.Now;
                    obj.AcknBy = LoggedInUserFullName;
                    entities.SaveChanges();
                    return Json(true);
                }
                return Json(false);
            }
        }

        public JsonResult Acknowledgement(string ids)
        {
            using (Reports.MorringOfficeEntities officeEntities = new Reports.MorringOfficeEntities())
            {
                bool acknowledged = false;

                foreach (string val in ids.Split(','))
                {
                    string[] data = val.Split('_');
                    if (data.Length > 1)
                    {
                        int id = Convert.ToInt32(data[0]);
                        int vesselid = Convert.ToInt32(data[1]);

                        var notificationFound = officeEntities.tblNotifications.FirstOrDefault(u => u.Id == id & u.VesselId == vesselid);

                        if (notificationFound != null)
                        {
                            notificationFound.AcknDateTime = DateTime.Now;
                            notificationFound.AcknBy = LoggedInUserID;
                            acknowledged = true;
                        }
                    }
                }

                if (acknowledged)
                    officeEntities.SaveChanges();

                return Json(new { Result = true }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult ViewComments(int notificationid, int vesselid)
        {
            using (Reports.MorringOfficeEntities officeEntities = new Reports.MorringOfficeEntities())
            {
                var notificationComments = officeEntities.NotificationComments.
                    Where(u => u.NotificationId == notificationid & u.VesselID == vesselid).ToList();

                return Json(new { Result = true, Data = notificationComments }, JsonRequestBehavior.AllowGet);
            }
        }

        [PreventSpam("AddComments", 3, 1)]
        public JsonResult AddComments(int notificationid, int vesselid, string commentText)
        {
            using (Reports.MorringOfficeEntities officeEntities = new Reports.MorringOfficeEntities())
            {
                var notificationComments = officeEntities.NotificationComments.OrderByDescending(u => u.Id).FirstOrDefault();

                Reports.NotificationComment notComment = new Reports.NotificationComment();
                notComment.Id = notificationComments == null ? 1 : (notificationComments.Id + 1);
                notComment.CommentsType = 2;
                notComment.Comments = commentText;
                notComment.VesselID = vesselid;
                notComment.NotificationId = notificationid;
                notComment.CreatedBy = User.Identity.GetUserName();
                notComment.CreatedDate = DateTime.Now;
                notComment.IsActive = true;

                officeEntities.NotificationComments.Add(notComment);
                officeEntities.SaveChanges();

                return Json(new { Result = true, Data = notificationComments }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}