using MenuLayer;
using Reports;
using Shipment49Web.Areas.InsDisCriteria.Models;
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

namespace Shipment49Web.Areas.InsDisCriteria.Controllers
{
    [Authorize]
    [UserAuthenticationFilter]
    [ErrorClass]
    public class InspectDiscardCriteriaController : BaseController
    {
        SqlConnection con = ConnectionBulder.con;
        // GET: InsDisCriteria/InspectDiscardCriteria
       // public static int VesselID { get; set; }
        public InspectDiscardCriteriaController()
        {
            CommonClass.TopeMenuID = "Menu7";
            // VesselID = Convert.ToInt32(CommonClass.VesselSessionID);
        }
        public ActionResult Index(int? page)
        {
           int VesselID = Convert.ToInt32( Session["VesselSessionID"].ToString());
            using (ShipmentContaxt sc1 = new ShipmentContaxt())
            {
                InsDisCriteriaClass model = new InsDisCriteriaClass();
                var ilist = new ShipmentContaxt()
                     .MultipleResults("[dbo].[VeiwRopeInspectionSetting]")
                   .With<InsDisCriteriaClass>()
                    .With<InsDisCriteriaClass>()
                   .Execute();
                model.InsDisCriteriaList = (List<InsDisCriteriaClass>)ilist[0];
                //model.InsDisCriteriaList1 = (List<InsDisCriteriaClass>)ilist[1];

                var record = model.InsDisCriteriaList.Count();
                int currPage = page == null ? 1 : Convert.ToInt32(page);
                TempData["CurrentPage"] = currPage;
                TempData["TotalRecords"] = record;
                model.InsDisCriteriaList = model.InsDisCriteriaList.Skip((CommonMethods.PAGESIZE * currPage) - CommonMethods.PAGESIZE).Take(CommonMethods.PAGESIZE).ToList();


                //var record1 = model.InsDisCriteriaList1.Count();
                //int currPage1 = page == null ? 1 : Convert.ToInt32(page);
                //TempData["CurrentPage1"] = currPage1;
                //TempData["TotalRecords1"] = record1;
               // model.InsDisCriteriaList1 = model.InsDisCriteriaList1.Skip((CommonMethods.PAGESIZE * currPage) - CommonMethods.PAGESIZE).Take(CommonMethods.PAGESIZE).ToList();


                return View(model);
            }
        }


        public ActionResult taildiscardsetting(int? page)
        {
            int VesselID = Convert.ToInt32(Session["VesselSessionID"].ToString());
            using (ShipmentContaxt sc1 = new ShipmentContaxt())
            {
                InsDisCriteriaClass model = new InsDisCriteriaClass();
                var ilist = new ShipmentContaxt()
                     .MultipleResults("[dbo].[VeiwRopeInspectionSetting]")
                   .With<InsDisCriteriaClass>()
                    .With<InsDisCriteriaClass>()
                   .Execute();
               // model.InsDisCriteriaList = (List<InsDisCriteriaClass>)ilist[0];
                model.InsDisCriteriaList1 = (List<InsDisCriteriaClass>)ilist[1];

                var record = model.InsDisCriteriaList1.Count();
                int currPage = page == null ? 1 : Convert.ToInt32(page);
                TempData["CurrentPage"] = currPage;
                TempData["TotalRecords"] = record;
                model.InsDisCriteriaList1 = model.InsDisCriteriaList1.Skip((CommonMethods.PAGESIZE * currPage) - CommonMethods.PAGESIZE).Take(CommonMethods.PAGESIZE).ToList();


                //var record1 = model.InsDisCriteriaList1.Count();
                //int currPage1 = page == null ? 1 : Convert.ToInt32(page);
                //TempData["CurrentPage1"] = currPage1;
                //TempData["TotalRecords1"] = record1;
               // model.InsDisCriteriaList1 = model.InsDisCriteriaList1.Skip((CommonMethods.PAGESIZE * currPage) - CommonMethods.PAGESIZE).Take(CommonMethods.PAGESIZE).ToList();


                return View(model);
            }
        }


        public ActionResult winchrotationsetting(int? page)
        {
           int VesselID = Convert.ToInt32(Session["VesselSessionID"].ToString());
            // VesselID = CommonClass.VesselSessionID;
            using (ShipmentContaxt sc1 = new ShipmentContaxt())
            {
                WinchRotationClass model = new WinchRotationClass();
                var ilist = new ShipmentContaxt()
                     .MultipleResults("[dbo].[GetWinchRotationSetting] '" + VesselID + "'")
                   .With<WinchRotationClass>()                  
                   .Execute();
                model.WinchRotationList = (List<WinchRotationClass>)ilist[0];

                var record = model.WinchRotationList.Count();
                int currPage = page == null ? 1 : Convert.ToInt32(page);
                TempData["CurrentPage"] = currPage;
                TempData["TotalRecords"] = record;
                model.WinchRotationList = model.WinchRotationList.Skip((CommonMethods.PAGESIZE * currPage) - CommonMethods.PAGESIZE).Take(CommonMethods.PAGESIZE).ToList();


                return View(model);
            }
        }
        [HttpGet]
        public ActionResult LEdiscardsetting()
        {
            int type = 0;
            if (type == 0)
            {
                type = 1;
            }
            // VesselID = CommonClass.VesselSessionID;
            using (ShipmentContaxt sc1 = new ShipmentContaxt())
            {
                LEdiscardsettingClass model = new LEdiscardsettingClass();
                var ilist = new ShipmentContaxt()
                     .MultipleResults("[dbo].[GetLEdiscardSetting] '" + type + "'")
                   .With<LEdiscardsettingClass>()
                   .Execute();
                model.LEdiscardsettingList = (List<LEdiscardsettingClass>)ilist[0];

                return View(model);
            }
        }
[HttpPost]
        public JsonResult test5(int type)
        {
            
            if (type == 0)
            {
                type = 1;
            }
            // VesselID = CommonClass.VesselSessionID;
            using (ShipmentContaxt sc1 = new ShipmentContaxt())
            {
                LEdiscardsettingClass model = new LEdiscardsettingClass();
                var ilist = new ShipmentContaxt()
                     .MultipleResults("[dbo].[GetLEdiscardSetting] '" + type + "'")
                   .With<LEdiscardsettingClass>()
                   .Execute();
                model.LEdiscardsettingList = (List<LEdiscardsettingClass>)ilist[0];

                return Json(model);
            }
        }

        public JsonResult test(int type)
        {
           

            SqlDataAdapter adp = new SqlDataAdapter("GetLEdiscardSetting", con);
            adp.SelectCommand.CommandType = CommandType.StoredProcedure;
            adp.SelectCommand.Parameters.AddWithValue("@type", type);
            DataTable dt = new DataTable();
            adp.Fill(dt);

            List<LEdiscardsettingClass> stc = new List<LEdiscardsettingClass>();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                LEdiscardsettingClass search = new LEdiscardsettingClass();

                search.looseequipmenttype = dt.Rows[i]["looseequipmenttype"].ToString();
                search.InspectionFrequency = Convert.ToInt32(dt.Rows[i]["InspectionFrequency"]);
                search.MaximumMonthsAllowed = Convert.ToInt32(dt.Rows[i]["MaximumMonthsAllowed"]);
                


                stc.Add(search);
            }
            return Json(new { Result = true, Data = stc }, JsonRequestBehavior.AllowGet);
           


        }
    }
}