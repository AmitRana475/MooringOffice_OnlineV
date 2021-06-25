using MenuLayer;
using Shipment49Web.Common;
using Microsoft.AspNet.Identity;
using Shipment49Web.Models;
using Shipment49Web.ViewModels;
using Shipment49Web.Areas.NotificationInfos.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Shipment49Web.Areas.MSPS.Models;
using Newtonsoft.Json;
using System.Data.SqlClient;
using System.Data;

namespace Shipment49Web.Areas.Notifications.Controllers
{
    [Authorize]
    [UserAuthenticationFilter]
    [ErrorClass]
    public class ShipNotificationController : BaseController
    {
        // GET: NotificationInfos/ShipNotification
        SqlConnection con = ConnectionBulder.con;
       // public static int VesselID { get; set; }
        public static DateTime? SearchFrom { get; set; }
        public static DateTime? SearchTo { get; set; }
        //  public static ShipNotificationClass ModelNotification = new ShipNotificationClass();
        public ShipNotificationController()
        {
           
            CommonClass.TopeMenuID = "Menu1";

        }
        //static ShipNotificationController()
        //{
        //    // VesselID = Convert.ToInt32(Session["VesselSessionID"].ToString());
        //    // CommonClass sl = new CommonClass();
        //    NotificationAlert controller = new NotificationAlert();
        //    controller.GetAllNotification(VesselID);
        //    CommonClass.Below21RopesAtDeleteTime(VesselID);
        //    CommonClass.Below21TailsAtDeleteTime(VesselID);


        //}


        public ActionResult Index(int? page, string Search)
        {
           
          int  VesselID = Convert.ToInt32(Session["VesselSessionID"].ToString());
            NotificationAlert controller = new NotificationAlert();
            controller.GetAllNotification(VesselID);
            CommonClass.Below21RopesAtDeleteTime(VesselID);
            CommonClass.Below21TailsAtDeleteTime(VesselID);

            if (Response.Cookies.Count > 0)
            {
                // Response.CacheControl = "no-store";
                foreach (string s in Response.Cookies.AllKeys)
                {
                    if (s == "ASP.NET_SessionId" || s.ToLower() == "asp.net_sessionid")
                    {
                        Response.Cookies[s].Secure = true;
                    }
                    if (s == "VesselSessionID" || s.ToLower() == "vesselsessionid")
                    {
                        Response.Cookies[s].Secure = true;
                    }
                    if (s == "VesselNames" || s.ToLower() == "vesselnames")
                    {
                        Response.Cookies[s].Secure = true;
                    }
                }
            }

            if (Search == "Ak")
            {
                using (ShipmentContaxt sc1 = new ShipmentContaxt())
                {
                    //  .MultipleResults("[dbo].[NotificationsListingkk_Archive_All]  '" + VesselID + "','" + SearchFrom + "','" + SearchTo + "'")

                    ShipNotificationClass model = new ShipNotificationClass();
                    var ilist = new ShipmentContaxt()
                         .MultipleResults("[dbo].[NotificationsListingkk_Archive_All]  '" + VesselID + "'")
                       .With<ShipNotificationClass>()
                        .With<TotalCountComment>()
                         .With<TotalCountComment>()
                          .With<TotalCountComment>()
                       .Execute();
                    model.ShipnotificationList = (List<ShipNotificationClass>)ilist[0];

                    List<TotalCountComment> totobj = (List<TotalCountComment>)ilist[1];
                    model.totalComment = totobj.FirstOrDefault().totalComment;
                    ViewBag.totalComment = model.totalComment;

                    List<TotalCountComment> totobj1 = (List<TotalCountComment>)ilist[2];
                    model.totalNotA = totobj1.FirstOrDefault().totalNotA;
                    ViewBag.totalNotA = model.totalNotA;

                    List<TotalCountComment> totobj2 = (List<TotalCountComment>)ilist[3];
                    model.totalA = totobj2.FirstOrDefault().totalA;
                    ViewBag.totalA = model.totalA;




                    var record = model.ShipnotificationList.Count();
                    int currPage = page == null ? 1 : Convert.ToInt32(page);
                    TempData["CurrentPage"] = currPage;
                    TempData["TotalRecords"] = record;
                    model.ShipnotificationList = model.ShipnotificationList.Skip((CommonMethods.PAGESIZE * currPage) - CommonMethods.PAGESIZE).Take(CommonMethods.PAGESIZE).ToList();
                    ViewBag.ArchivedMsg = "Notifications - Acknowledged";
                    return View(model);

                }
            }
            else if (Search == "unread")
            {
                using (ShipmentContaxt sc1 = new ShipmentContaxt())
                {
                    //  .MultipleResults("[dbo].[NotificationsListingkk_Archive_All]  '" + VesselID + "','" + SearchFrom + "','" + SearchTo + "'")

                    ShipNotificationClass model = new ShipNotificationClass();
                    var ilist = new ShipmentContaxt()
                         .MultipleResults("[dbo].[NotificationsListingkk_unread]  '" + VesselID + "'")
                       .With<ShipNotificationClass>()
                        .With<TotalCountComment>()
                         .With<TotalCountComment>()
                          .With<TotalCountComment>()
                       .Execute();
                    model.ShipnotificationList = (List<ShipNotificationClass>)ilist[0];

                    List<TotalCountComment> totobj = (List<TotalCountComment>)ilist[1];
                    model.totalComment = totobj.FirstOrDefault().totalComment;
                    ViewBag.totalComment = model.totalComment;

                    List<TotalCountComment> totobj1 = (List<TotalCountComment>)ilist[2];
                    model.totalNotA = totobj1.FirstOrDefault().totalNotA;
                    ViewBag.totalNotA = model.totalNotA;

                    List<TotalCountComment> totobj2 = (List<TotalCountComment>)ilist[3];
                    model.totalA = totobj2.FirstOrDefault().totalA;
                    ViewBag.totalA = model.totalA;




                    var record = model.ShipnotificationList.Count();
                    int currPage = page == null ? 1 : Convert.ToInt32(page);
                    TempData["CurrentPage"] = currPage;
                    TempData["TotalRecords"] = record;
                    model.ShipnotificationList = model.ShipnotificationList.Skip((CommonMethods.PAGESIZE * currPage) - CommonMethods.PAGESIZE).Take(CommonMethods.PAGESIZE).ToList();
                    ViewBag.ArchivedMsg = "Notifications - Unread Office Comments";
                    return View(model);

                }
            }
            //if (string.IsNullOrEmpty(Search))
            else
            {
                using (ShipmentContaxt sc1 = new ShipmentContaxt())
                {
                    ShipNotificationClass model = new ShipNotificationClass();
                    var ilist = new ShipmentContaxt()
                         .MultipleResults("[dbo].[NotificationsListingkk]  '" + VesselID + "'")
                       .With<ShipNotificationClass>()
                        .With<TotalCountComment>()
                         .With<TotalCountComment>()
                          .With<TotalCountComment>()
                       .Execute();
                    model.ShipnotificationList = (List<ShipNotificationClass>)ilist[0];

                    List<TotalCountComment> totobj = (List<TotalCountComment>)ilist[1];
                    model.totalComment = totobj.FirstOrDefault().totalComment;
                    ViewBag.totalComment = model.totalComment;

                    List<TotalCountComment> totobj1 = (List<TotalCountComment>)ilist[2];
                    model.totalNotA = totobj1.FirstOrDefault().totalNotA;
                    ViewBag.totalNotA = model.totalNotA;

                    List<TotalCountComment> totobj2 = (List<TotalCountComment>)ilist[3];
                    model.totalA = totobj2.FirstOrDefault().totalA;
                    ViewBag.totalA = model.totalA;




                    var record = model.ShipnotificationList.Count();
                    int currPage = page == null ? 1 : Convert.ToInt32(page);
                    TempData["CurrentPage"] = currPage;
                    TempData["TotalRecords"] = record;
                    model.ShipnotificationList = model.ShipnotificationList.Skip((CommonMethods.PAGESIZE * currPage) - CommonMethods.PAGESIZE).Take(CommonMethods.PAGESIZE).ToList();

                    ViewBag.ArchivedMsg = "Notifications - To be Acknowledged";
                    return View(model);

                }
            }
           
        }

        public class TotalCountComment
        {
            public Int32? totalComment { get; set; }
            public Int32? totalNotA { get; set; }
            public Int32? totalA { get; set; }


        }
        [HttpPost]
        public ActionResult Index(ShipNotificationClass searchViewModel) // NotificationSearchViewModel
        {
            int VesselID = Convert.ToInt32(Session["VesselSessionID"].ToString());
            if (Response.Cookies.Count > 0)
            {
                // Response.CacheControl = "no-store";
                foreach (string s in Response.Cookies.AllKeys)
                {
                    if (s == "ASP.NET_SessionId" || s.ToLower() == "asp.net_sessionid")
                    {
                        Response.Cookies[s].Secure = true;
                    }
                }
            }
            int page = 1;
            using (ShipmentContaxt sc1 = new ShipmentContaxt())
            {
                ShipNotificationClass model = new ShipNotificationClass();
                var ilist = new ShipmentContaxt()
                     .MultipleResults("[dbo].[NotificationsListingkk_Archive]  '" + VesselID + "','" + searchViewModel.SearchFrom + "','" + searchViewModel.SearchTo + "'")
                   .With<ShipNotificationClass>()
                    .With<TotalCountComment>()
                     .With<TotalCountComment>()
                      .With<TotalCountComment>()
                   .Execute();
                model.ShipnotificationList = (List<ShipNotificationClass>)ilist[0];

                List<TotalCountComment> totobj = (List<TotalCountComment>)ilist[1];
                model.totalComment = totobj.FirstOrDefault().totalComment;
                ViewBag.totalComment = model.totalComment;

                List<TotalCountComment> totobj1 = (List<TotalCountComment>)ilist[2];
                model.totalNotA = totobj1.FirstOrDefault().totalNotA;
                ViewBag.totalNotA = model.totalNotA;

                List<TotalCountComment> totobj2 = (List<TotalCountComment>)ilist[3];
                model.totalA = totobj2.FirstOrDefault().totalA;
                ViewBag.totalA = model.totalA;




                var record = model.ShipnotificationList.Count();
                int currPage = page == null ? 1 : Convert.ToInt32(page);
                TempData["CurrentPage"] = currPage;
                TempData["TotalRecords"] = record;

                SearchFrom = searchViewModel.SearchFrom;
                SearchTo = searchViewModel.SearchTo;
                model.ShipnotificationList = model.ShipnotificationList.Skip((CommonMethods.PAGESIZE * currPage) - CommonMethods.PAGESIZE).Take(CommonMethods.PAGESIZE).ToList();
                ViewBag.ArchivedMsg = "Archives Notifications Between Dates " + Convert.ToDateTime(searchViewModel.SearchFrom).ToString("dd-MMM-yyyy") + " and " + Convert.ToDateTime(searchViewModel.SearchTo).ToString("dd-MMM-yyyy");
                return View(model);

            }

        }

        public JsonResult ViewComments(int notificationid)
        {
            int VesselID = Convert.ToInt32(Session["VesselSessionID"].ToString());
            using (Reports.MorringOfficeEntities officeEntities = new Reports.MorringOfficeEntities())
            {
                var notificationComments = officeEntities.NotificationComments.
                    Where(u => u.NotificationId == notificationid & u.CommentsType == 1 & u.VesselID == VesselID).ToList();

                notificationComments.ForEach(x =>
                {
                    var date = x.CreatedDate.ToString();
                    x.DisplayDate = string.IsNullOrEmpty(date) == true ? date : Convert.ToDateTime(date).ToString("yyyy-mm-dd HH:MM");
                });

                return Json(new { Result = true, Data = notificationComments }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult ViewCommentsOfc(int notificationid)
        {
            int VesselID = Convert.ToInt32(Session["VesselSessionID"].ToString());
            using (Reports.MorringOfficeEntities officeEntities = new Reports.MorringOfficeEntities())
            {
                var notificationComments = officeEntities.NotificationComments.
                    Where(u => u.NotificationId == notificationid & u.CommentsType == 2 & u.VesselID == VesselID).ToList();

                notificationComments.ForEach(x =>
                {
                    var date = x.CreatedDate.ToString();
                    x.DisplayDate = string.IsNullOrEmpty(date) == true ? date : Convert.ToDateTime(date).ToString("yyyy-mm-dd HH:MM");
                });

                //using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings[ConnectionName].ConnectionString))
                //{

                using (SqlDataAdapter pp = new SqlDataAdapter("update notificationcomment set IsRead=1 where CommentsType=2 and NotificationId="+ notificationid + " and VesselID="+ VesselID + "", con))
                {
                    using (DataTable dd = new DataTable())
                    {
                        pp.Fill(dd);
                    }
                }

                //}

                return Json(new { Result = true, Data = notificationComments }, JsonRequestBehavior.AllowGet);
            }
        }

        [PreventSpam("AddCommentsShip", 3, 1)]
        public JsonResult AddComments(int notificationid, string commentText)
        {
            int VesselID = Convert.ToInt32(Session["VesselSessionID"].ToString());
            using (Reports.MorringOfficeEntities officeEntities = new Reports.MorringOfficeEntities())
            {
                var notificationComments = officeEntities.NotificationComments.Where(u => u.VesselID == VesselID).OrderByDescending(u => u.Id).FirstOrDefault();

                Reports.NotificationComment notComment = new Reports.NotificationComment();
                notComment.Id = notificationComments == null ? 1 : (notificationComments.Id + 1);
                notComment.CommentsType = 1;
                notComment.Comments = commentText;
                notComment.VesselID = VesselID;
                notComment.NotificationId = notificationid;
                notComment.CreatedBy = User.Identity.GetUserName();
                notComment.CreatedDate = DateTime.Now;
                notComment.IsActive = true;
                notComment.IsRead = false;

                officeEntities.NotificationComments.Add(notComment);
                officeEntities.SaveChanges();


                var notificationComments1 = officeEntities.NotificationComments.
                    Where(u => u.NotificationId == notificationid & u.VesselID == VesselID).ToList();

                notificationComments1.ForEach(x =>
                {
                    var date = x.CreatedDate.ToString();
                    x.DisplayDate = string.IsNullOrEmpty(date) == true ? date : Convert.ToDateTime(date).ToString("yyyy-mm-dd HH:MM");
                });

                return Json(new { Result = true, Data = notificationComments1 }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult Acknowledge(string inspections)
        {
            //  MooringRopeInspection morIns = new MooringRopeInspection();
            var Result = inspections;
            string json = Result.ToString();
            string dd = json;
            //int vesselid = Convert.ToInt32(VesselID);
            //int nxtinspctid = CommonClass.NextInspectionId(vesselid);

            var Json2 = JsonConvert.DeserializeObject<List<ShipNotificationClass>>(inspections);
            string dt = DateTime.Now.ToString("dd-MMM-yyyy HHmm");
            Json2.ForEach(x => { x.AckRecord = "Acknowledged on " + dt + " Hrs"; x.Acknowledge = true; });

            string sqry = "update tblNotification set AckRecord=@AckRecord, Acknowledge = @Acknowledge where id=@id and NotificationType = @NotificationType and RopeId=@RopeId and VesselId=@VesselId";
            if (con.State == ConnectionState.Closed)
            {
                con.Open();
            }
            foreach (var rootObject in Json2)
            {
                int[] notAckid = { 6, 8, 10, 17, 18, 43 };
                //if (item.NotificationAlertType == 17 || item.NotificationAlertType == 18)
                if (notAckid.Contains(rootObject.NotificationType) == true)
                {
                }
                else
                {
                    SqlCommand cmd = new SqlCommand(sqry, con);
                    cmd.Parameters.AddWithValue("@AckRecord", rootObject.AckRecord);
                    cmd.Parameters.AddWithValue("@Acknowledge", rootObject.Acknowledge);
                    cmd.Parameters.AddWithValue("@id", rootObject.Id);
                    cmd.Parameters.AddWithValue("@NotificationType", rootObject.NotificationType);
                    cmd.Parameters.AddWithValue("@RopeId", rootObject.RopeId);
                    cmd.Parameters.AddWithValue("@VesselId", rootObject.VesselId);
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();
                }
            }

            return Json(Url.Action("Index", "ShipNotification"));
            // return RedirectToAction("Index");
        }
    }
}