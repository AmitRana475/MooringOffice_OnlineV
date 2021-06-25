using MenuLayer;
using Reports;
using Shipment49Web.Areas.MooringLine.Models;
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

namespace Shipment49Web.Areas.MooringLine.Controllers
{
    [Authorize]
    [UserAuthenticationFilter]
    [ErrorClass]
    public class LineDisposalController : BaseController
    {
        MorringOfficeEntities context = new MorringOfficeEntities();
        SqlConnection con = ConnectionBulder.con;
        //CommonClass cls = new CommonClass();
        RopeDisposal assR = new RopeDisposal();
        DamageR ss = new DamageR();
        public static int? winchid = 0;
        public static bool outboardEndinuse = true;
       // public static string VesselID;
        // GET: MooringLine/LineDisposal

        //public static int VesselID { get; set; }
        public LineDisposalController()
        {
           // VesselID = Convert.ToInt32(CommonClass.VesselSessionID);
        }
        public ActionResult Index()
        {

           int VesselID = Convert.ToInt32(Session["VesselSessionID"].ToString());
            using (ShipmentContaxt sc1 = new ShipmentContaxt())
            {
                RopeDisposals model = new RopeDisposals();
                var ilist = new ShipmentContaxt()
                     .MultipleResults("[dbo].[GetRopeDisposal]  0,'" + VesselID + "'")
                   .With<RopeDisposals>()
                   .Execute();
                model.RopeDisposalList = (List<RopeDisposals>)ilist[0];
                return View(model);
            }
        }

        public ActionResult addlinedisposal()
        {
            int VesselID = Convert.ToInt32(Session["VesselSessionID"].ToString());
            //assR.ReasonOutofServices = cls.OutofServiceList();
            //assR.DamageObservedLists = cls.DamagObservedList();
            //assR.MooringOperationsLists = cls.MooringOpListCommon();
            assR.MooringLineLists = CommonClass.MooringRopeDisposalListCommon(0,VesselID);
            assR.PortNameList = CommonClass.GetPortNameList();
            return View(assR);
        }
        [HttpPost]
        [PreventSpam("addlinedisposal", 3, 1)]
        public ActionResult addlinedisposal(RopeDisposal rpdisposal)
        {
            int VesselID = Convert.ToInt32(Session["VesselSessionID"].ToString());
            int vesselid = Convert.ToInt32(VesselID);

            var findassno = context.RopeDisposals.Where(x => x.RopeId == rpdisposal.RopeId && x.VesselID== vesselid && x.IsActive==true).FirstOrDefault();

            if (findassno != null)
            {
                TempData["Error"] = "Record already exist !";
                //rpdisposal.MooringLineLists = CommonClass.MooringRopeDisposalListCommon(0, VesselID);
                //rpdisposal.PortNameList = CommonClass.GetPortNameList();
            }
            else
            {

                var IdPK1 = ((from asn in context.RopeDisposals.Where(x => x.VesselID == vesselid) select (int?)asn.Id).Max() ?? 0) + 1;
                rpdisposal.DisposalPortName = rpdisposal.DisposalPortName;
                rpdisposal.ReceptionFacilityName = rpdisposal.ReceptionFacilityName;

                rpdisposal.DisposalDate = rpdisposal.DisposalDate;
                rpdisposal.CreatedDate = DateTime.Now;
                rpdisposal.CreatedBy = "Admin";
                rpdisposal.IsActive = true;
                rpdisposal.RopeTail = 0;
                rpdisposal.Id = IdPK1;
                rpdisposal.VesselID = vesselid;
                rpdisposal.WinchId = winchid;

                context.RopeDisposals.Add(rpdisposal);
                context.SaveChanges();
                TempData["Success"] = "Record successfully saved !";
            }
           
            return RedirectToAction("Index");
        }

        public JsonResult GetWinchlocation(int Id)
        {
            int VesselID = Convert.ToInt32(Session["VesselSessionID"].ToString());
            int vesselid = Convert.ToInt32(VesselID);

            var data = context.AssignRopeToWinches.Where(x => x.RopeId == Id && x.IsActive == true && x.VesselID == vesselid).FirstOrDefault();
            if (data != null)
            {
                int? winchidd = data.WinchId;
                winchid = winchidd;

                string msgdiv = "";string outofSDt = "";
                var outofserdt = context.MooringRopeDetails.Where(x => x.RopeId == Id && x.VesselID== vesselid && x.DeleteStatus == false).Select(x => x.OutofServiceDate).SingleOrDefault();
                if (outofserdt == null)
                {
                     msgdiv = "Line not yet Out of Service, Please fill Line Discard Form before filling the Disposal Form.";
                }
                else
                {
                    outofSDt = outofserdt.ToString();
                }
                return Json(new { Result = true, outofserdt= outofSDt, msgdiv = msgdiv }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                string msgdiv = "";
                string outofSDt = "";
                var outofserdt = context.MooringRopeDetails.Where(x => x.RopeId == Id && x.VesselID == vesselid && x.DeleteStatus == false).Select(x => x.OutofServiceDate).SingleOrDefault();
                if (outofserdt == null)
                {

                    msgdiv = "Line not yet Out of Service, Please fill Line Discard Form before filling the Disposal Form.";
                }
                else
                {
                    DateTime frmdt = Convert.ToDateTime(outofserdt);
                    outofSDt = frmdt.ToString("yyyy-MM-dd");
                    
                }

                
                winchid = 0;
                return Json(new { Result = true, outofserdt = outofSDt, msgdiv = msgdiv }, JsonRequestBehavior.AllowGet);
            }

            //return Json(new { Result = true, outboard = "", asswinch = "", location = "", outboard1 = "", noofOp = "", rnghrs = "" }, JsonRequestBehavior.AllowGet);
        }


        public JsonResult discarddatecheck(string dtvalue, int RopeId)
        {
            int VesselID = Convert.ToInt32(Session["VesselSessionID"].ToString());
            var data = context.MooringRopeDetails.Where(x => x.Id == RopeId && x.DeleteStatus == false && x.VesselID == VesselID).Select(x => x.OutofServiceDate).SingleOrDefault();

            var outofserrdt = data;

            var assigndate = dtvalue;

            if (Convert.ToDateTime(assigndate) < outofserrdt)
            {
                string msg = "Disposal Date can not less than Line Discarded date!";

                string instdt1 = outofserrdt.ToString();
                DateTime dateAndTime = Convert.ToDateTime(instdt1.ToString());
                string instdt = dateAndTime.ToString("yyyy-MM-dd");


                return Json(new { Result = true, instdate = instdt, Message = msg }, JsonRequestBehavior.AllowGet);
            }



            return Json(new { Result = true, instdate = "", Message = "" }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Delete(int Id)
        {
            try
            {
                int VesselID = Convert.ToInt32(Session["VesselSessionID"].ToString());
                int vesselid = Convert.ToInt32(VesselID);
                SqlDataAdapter adp = new SqlDataAdapter("update RopeDisposal set IsActive='False' where Id =" + Id + " and RopeTail = 0 and VesselID='" + vesselid + "' ", con);
                DataTable dt = new DataTable();
                adp.Fill(dt);               

                TempData["Success"] = "Record successfully deleted !";
            }
            catch { }
            return RedirectToAction("Index");
        }
    }
}