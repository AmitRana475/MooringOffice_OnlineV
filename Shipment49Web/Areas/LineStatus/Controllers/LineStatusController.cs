using MenuLayer;
using Reports;
using Shipment49Web.Areas.MooringLine.Models;
using Shipment49Web.Areas.LineStatus.Models;
using Shipment49Web.Areas.MSPS.Models;
using Shipment49Web.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Shipment49Web.Models;

namespace Shipment49Web.Areas.LineStatus.Controllers
{
    [Authorize]
    [UserAuthenticationFilter]
    [ErrorClass]
    public class LineStatusController : BaseController
    {
        MorringOfficeEntities context = new MorringOfficeEntities();
        SqlConnection con = ConnectionBulder.con;       
        public static string VesselID;
        // GET: LineStatus/LineStatus
        public ActionResult Index(int? page)
        {
            VesselID  = Session["VesselSessionID"].ToString();
            using (ShipmentContaxt sc1 = new ShipmentContaxt())
            {

                MooringRopeDetails model = new MooringRopeDetails();
                var ilist = new ShipmentContaxt()
                .MultipleResults("[dbo].[GetMooringRopeDetailList] 0,'" + VesselID + "','Line'")
                   .With<MooringRopeDetails>()
                   .With<MooringRopeDetails>()
                   .Execute();
                model.MooringLineList = (List<MooringRopeDetails>)ilist[0];
                model.MooringLineList1 = (List<MooringRopeDetails>)ilist[1];
                //List<TotalCount> totobj = (List<TotalCount>)ilist[1];
                //model.Total = totobj.FirstOrDefault().Total;
                //ViewBag.TotalCount = model.Total;
                //var pager = new Pager(Convert.ToInt32(model.Total), 0);
                //model.Pager = pager;


                var record = model.MooringLineList1.Count();
                //int currPage = TempData["CurrentPage"] == null ? 1 : Convert.ToInt32(TempData["CurrentPage"]);

                int currPage = page == null ? 1 : Convert.ToInt32(page);

                TempData["CurrentPage"] = currPage;
                TempData["TotalRecords"] = record;

                model.MooringLineList1 = model.MooringLineList1.Skip((CommonMethods.PAGESIZE * currPage) - CommonMethods.PAGESIZE).Take(CommonMethods.PAGESIZE).ToList();

                return View(model);
            }
        }

       
        public ActionResult viewdetails(int id)
        {
            VesselID = Session["VesselSessionID"].ToString();
            using (ShipmentContaxt sc1 = new ShipmentContaxt())
            {

                LineStatusClass model = new LineStatusClass();
                var ilist = new ShipmentContaxt()
                     .MultipleResults("[dbo].[GetRopeStatus] " + id + ",0,'" + VesselID + "'")
                   .With<LineStatusClass>()
                    .With<Mooringinspection>()
                     .With<RopeSplicing>()
                     .With<RopeCroppingClass>()
                      .With<RopeDamage>()
                       .With<RopeDiscardClass>()
                         .With<RopeDisposals>()
                           .With<RopeEndtoEnd>()
                            .With<WinchRotationClass>()
                   .Execute();
                model.MooringLineList = (List<LineStatusClass>)ilist[0];
                model.InspectionDetail = (List<Mooringinspection>)ilist[1];
                model.RopeSplicingList = (List<RopeSplicing>)ilist[2];
                model.RopeCroppingList = (List<RopeCroppingClass>)ilist[3];
                model.RopeDamageList = (List<RopeDamage>)ilist[4];
                model.MooringLineDiscardList = (List<RopeDiscardClass>)ilist[5];
                model.RopeDisposalList = (List<RopeDisposals>)ilist[6];
                model.RopeEndtoEndList = (List<RopeEndtoEnd>)ilist[7];
                model.WinchRotationList = (List<WinchRotationClass>)ilist[8];

                SqlDataAdapter adp = new SqlDataAdapter("RopeDetailform", con);
                adp.SelectCommand.CommandType = CommandType.StoredProcedure; ;
                adp.SelectCommand.Parameters.AddWithValue("@RopeId", id);
                adp.SelectCommand.Parameters.AddWithValue("@VesselId", VesselID);
                DataSet dt = new DataSet();
                adp.Fill(dt);
                if (dt.Tables[0].Rows.Count > 0)
                {
                    ViewBag.Winch = dt.Tables[0].Rows[0]["assignednumber"] == DBNull.Value ? "Not Assigned" : dt.Tables[0].Rows[0]["assignednumber"].ToString();
                    ViewBag.Location = dt.Tables[0].Rows[0]["location"] == DBNull.Value ? "Not Assigned" : dt.Tables[0].Rows[0]["location"].ToString();
                    ViewBag.TtlRnghrs = dt.Tables[0].Rows[0]["currentrunninghours"] == DBNull.Value ? "Not Assigned" : dt.Tables[0].Rows[0]["currentrunninghours"].ToString();
                    ViewBag.OutboardEnd = dt.Tables[0].Rows[0]["OutboardEnd"] == DBNull.Value ? "Not Assigned" : dt.Tables[0].Rows[0]["OutboardEnd"].ToString();
                }
                else
                {
                    ViewBag.Winch = "Not Assigned";
                    ViewBag.Location = "Not Assigned";
                    ViewBag.TtlRnghrs = "Not Assigned";
                    ViewBag.OutboardEnd = "Not Assigned";
                }
                if (dt.Tables[1].Rows.Count > 0)
                {
                  
                    ViewBag.TtlRnghrs = dt.Tables[1].Rows[0]["currentrunninghours"] == DBNull.Value ? "Not Assigned" : dt.Tables[1].Rows[0]["currentrunninghours"].ToString();
                    ViewBag.NxtInsDue = dt.Tables[1].Rows[0]["InspectionDueDate"] == DBNull.Value ? "Not Assigned" : dt.Tables[1].Rows[0]["InspectionDueDate"].ToString();
                }

                return View(model);
            }
        }


        public ActionResult tailinfo(int? page)
        {
            VesselID = Session["VesselSessionID"].ToString();
            using (ShipmentContaxt sc1 = new ShipmentContaxt())
            {

                MooringRopeDetails model = new MooringRopeDetails();
                var ilist = new ShipmentContaxt()
                       .MultipleResults("[dbo].[GetMooringRopeDetailList] 1,'" + VesselID + "','RopeTail'")
                   .With<MooringRopeDetails>()
                   .With<MooringRopeDetails>()
                   .Execute();
                model.MooringLineList = (List<MooringRopeDetails>)ilist[0];
                model.MooringLineList1 = (List<MooringRopeDetails>)ilist[1];


                var record = model.MooringLineList1.Count();
                int currPage = page == null ? 1 : Convert.ToInt32(page);
                TempData["CurrentPage"] = currPage;
                TempData["TotalRecords"] = record;
                model.MooringLineList1 = model.MooringLineList1.Skip((CommonMethods.PAGESIZE * currPage) - CommonMethods.PAGESIZE).Take(CommonMethods.PAGESIZE).ToList();



                return View(model);
            }
        }
        public ActionResult viewtaildetails(int id)
        {
            VesselID = Session["VesselSessionID"].ToString();
            using (ShipmentContaxt sc1 = new ShipmentContaxt())
            {

                LineStatusClass model = new LineStatusClass();
                var ilist = new ShipmentContaxt()
                     .MultipleResults("[dbo].[GetRopeStatus] " + id + ",1,'" + VesselID + "'")
                   .With<LineStatusClass>()
                    .With<Mooringinspection>()
                     .With<RopeSplicing>()
                     .With<RopeCroppingClass>()
                      .With<RopeDamage>()
                       .With<RopeDiscardClass>()
                         .With<RopeDisposals>()
                          
                   .Execute();
                model.MooringLineList = (List<LineStatusClass>)ilist[0];
                model.InspectionDetail = (List<Mooringinspection>)ilist[1];
                model.RopeSplicingList = (List<RopeSplicing>)ilist[2];
                model.RopeCroppingList = (List<RopeCroppingClass>)ilist[3];
                model.RopeDamageList = (List<RopeDamage>)ilist[4];
                model.MooringLineDiscardList = (List<RopeDiscardClass>)ilist[5];
                model.RopeDisposalList = (List<RopeDisposals>)ilist[6];
               

                SqlDataAdapter adp = new SqlDataAdapter("RopeDetailform", con);
                adp.SelectCommand.CommandType = CommandType.StoredProcedure; ;
                adp.SelectCommand.Parameters.AddWithValue("@RopeId", id);
                adp.SelectCommand.Parameters.AddWithValue("@VesselId", VesselID);
                DataSet dt = new DataSet();
                adp.Fill(dt);
                if (dt.Tables[0].Rows.Count > 0)
                {
                    ViewBag.Winch = dt.Tables[0].Rows[0]["assignednumber"] == DBNull.Value ? "Not Assigned" : dt.Tables[0].Rows[0]["assignednumber"].ToString();
                    ViewBag.Location = dt.Tables[0].Rows[0]["location"] == DBNull.Value ? "Not Assigned" : dt.Tables[0].Rows[0]["location"].ToString();
                    ViewBag.TtlRnghrs = dt.Tables[0].Rows[0]["currentrunninghours"] == DBNull.Value ? "Not Assigned" : dt.Tables[0].Rows[0]["currentrunninghours"].ToString();
                    ViewBag.OutboardEnd = dt.Tables[0].Rows[0]["OutboardEnd"] == DBNull.Value ? "Not Assigned" : dt.Tables[0].Rows[0]["OutboardEnd"].ToString();
                }
                else
                {
                    ViewBag.Winch = "Not Assigned";
                    ViewBag.Location = "Not Assigned";
                    ViewBag.TtlRnghrs = "Not Assigned";
                    ViewBag.OutboardEnd = "Not Assigned";
                }
                if (dt.Tables[1].Rows.Count > 0)
                {

                    ViewBag.TtlRnghrs = dt.Tables[1].Rows[0]["currentrunninghours"] == DBNull.Value ? "Not Assigned" : dt.Tables[1].Rows[0]["currentrunninghours"].ToString();
                    ViewBag.NxtInsDue = dt.Tables[1].Rows[0]["InspectionDueDate"] == DBNull.Value ? "Not Assigned" : dt.Tables[1].Rows[0]["InspectionDueDate"].ToString();
                }

                return View(model);
            }
        }
    }
}