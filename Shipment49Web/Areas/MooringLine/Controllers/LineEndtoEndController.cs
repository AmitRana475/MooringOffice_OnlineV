using MenuLayer;
using Reports;
using Shipment49Web.Areas.MooringLine.Models;
using Shipment49Web.Areas.MSPS.Models;
using Shipment49Web.Common;
using Shipment49Web.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;

namespace Shipment49Web.Areas.MooringLine.Controllers
{
    [Authorize]
    [UserAuthenticationFilter]
    [ErrorClass]
    public class LineEndtoEndController : BaseController
    {
        MorringOfficeEntities context = new MorringOfficeEntities();
        SqlConnection con = ConnectionBulder.con;
        //CommonClass cls = new CommonClass();
        RopeEndtoEnd2 assR = new RopeEndtoEnd2();
        public static int? winchid = 0;
        public static bool outboardEndinuse = true;

        // GET: MooringLine/AssignLine
        //public static string VesselID;
        // GET: MooringLine/LineEndtoEnd

        //public static int VesselID { get; set; }
        public LineEndtoEndController()
        {
           // VesselID = Convert.ToInt32(CommonClass.VesselSessionID);
        }
        public ActionResult Index()
        {
           int VesselID = Convert.ToInt32(Session["VesselSessionID"].ToString());
            using (ShipmentContaxt sc1 = new ShipmentContaxt())
            {
                RopeEndtoEnd model = new RopeEndtoEnd();
                var ilist = new ShipmentContaxt()
                     .MultipleResults("[dbo].[GetRopeEndtoEnd2] 'Listing', 0,'" + VesselID + "'")
                   .With<RopeEndtoEnd>()
                   .Execute();
                model.RopeEndtoEndList = (List<RopeEndtoEnd>)ilist[0];
                return View(model);
            }
        }

        public ActionResult addlineendtoend()
        {
            int VesselID = Convert.ToInt32(Session["VesselSessionID"].ToString());
            assR.MooringLineLists = CommonClass.MooringRopeDetailListEndtoEnd(VesselID);
            return View(assR);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [PreventSpam("addlineendtoend",3, 1)]
        public ActionResult addlineendtoend(RopeEndtoEnd2 rpendtoend)
        {
            int VesselID = Convert.ToInt32(Session["VesselSessionID"].ToString());
            int vesselid = Convert.ToInt32(VesselID);

            var IdPK = ((from asn in context.RopeEndtoEnd2.Where(x=> x.VesselID==vesselid) select (int?)asn.Id).Max() ?? 0) + 1;
            var result = context.AssignRopeToWinches.SingleOrDefault(b => b.RopeId == rpendtoend.RopeId && b.IsActive == true && b.VesselID== vesselid);
            if (result != null)
            {
                //if (outboardEndinuse == false)
                    if (outboardEndinuse == false)
                    {
                    result.Outboard = false;
                    rpendtoend.CurrentOutboadEndinUse = false;
                }

                if (outboardEndinuse == true)
                {
                    result.Outboard = true;
                    rpendtoend.CurrentOutboadEndinUse = true;
                }

                result.ModifiedBy = "Admin";
                result.ModifiedDate = DateTime.Now;
                context.SaveChanges();
            }


            rpendtoend.Id = IdPK;
            rpendtoend.VesselID = vesselid;
            rpendtoend.WinchId = winchid;
            rpendtoend.RopeId = rpendtoend.RopeId;
            rpendtoend.CreatedDate = DateTime.Now;
            rpendtoend.CreatedBy = "Admin";
            rpendtoend.IsActive = true;


            context.RopeEndtoEnd2.Add(rpendtoend);
            context.SaveChanges();

            TempData["Success"] = "Record successfully saved !";
            return RedirectToAction("Index");
        }
        public JsonResult GetWinchlocation(int Id)
        {
            int VesselID = Convert.ToInt32(Session["VesselSessionID"].ToString());

            int vesselid = Convert.ToInt32(VesselID);
            var data = context.AssignRopeToWinches.Where(x => x.RopeId == Id && x.IsActive == true && x.VesselID== vesselid).FirstOrDefault();
            if (data != null)
            {
                int? winchidd = data.WinchId;
                winchid = winchidd;
                string outboard = data.Outboard == true ? "A" : "B";
                if(outboard =="A")
                {
                    outboardEndinuse = false;
                }
                if (outboard == "B")
                {
                    outboardEndinuse = true;
                }

                string location = context.MooringWinchDetails.Where(x => x.Id == data.WinchId && x.IsActive == true && x.VesselID == vesselid).Select(x => x.Location).SingleOrDefault();
                string outboard1 = data.Outboard == true ? "B" : "A";
                string asswinch = context.MooringWinchDetails.Where(s => s.Id == data.WinchId && s.IsActive == true && s.VesselID == vesselid).Select(x => x.AssignedNumber).FirstOrDefault();
                return Json(new { Result = true, outboard = outboard, asswinch= asswinch, location = location, outboard1 = outboard1, }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                winchid = 0;
            }

            return Json(new { Result = true, outboard = "", asswinch = "", location = "", outboard1 = "", }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Installdatecheck(string dtvalue, int RopeId)
        {
            int VesselID = Convert.ToInt32(Session["VesselSessionID"].ToString());
            var data = context.MooringRopeDetails.Where(x => x.Id == RopeId && x.DeleteStatus == false && x.VesselID == VesselID).Select(x => x.InstalledDate).SingleOrDefault();

            var installeddate = data;

            var assigndate = dtvalue;

            if (Convert.ToDateTime(assigndate) < installeddate)
            {
                string msg = "EndtoEnd Done date can not less than Line Installed date!";

                string instdt1 = installeddate.ToString();
                DateTime dateAndTime = Convert.ToDateTime(instdt1.ToString());
                string instdt = dateAndTime.ToString("yyyy-MM-dd");


                return Json(new { Result = true, instdate = instdt, Message = msg }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var data1 = context.RopeEndtoEnd2.Where(x => x.RopeId == RopeId && x.IsActive == true && x.VesselID == VesselID).Select(x => x.EndtoEndDoneDate).SingleOrDefault();

                var installeddate1 = data1;

                var assigndate1 = dtvalue;


                if (Convert.ToDateTime(assigndate1) < installeddate1)
                {
                    string msg = "EndtoEnd Done date can not less than of this Line past end to end date!";

                    string instdt1 = installeddate1.ToString();
                    DateTime dateAndTime = Convert.ToDateTime(instdt1.ToString());
                    string instdt = dateAndTime.ToString("yyyy-MM-dd");


                    return Json(new { Result = true, instdate = instdt, Message = msg }, JsonRequestBehavior.AllowGet);
                }
            }



            return Json(new { Result = true, instdate = "", Message = "" }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Delete(int Id)
        {
            try
            {
                int VesselID = Convert.ToInt32(Session["VesselSessionID"].ToString());
                int vesselid = Convert.ToInt32(VesselID);
                SqlDataAdapter adp = new SqlDataAdapter("update RopeEndtoEnd2 set IsActive='False' where Id =" + Id + " and VesselID='" + vesselid + "' ", con);
                DataTable dt = new DataTable();
                adp.Fill(dt);

                TempData["Success"] = "Record successfully deleted !";
            }
            catch { }
            return RedirectToAction("Index");
        }
    }
}