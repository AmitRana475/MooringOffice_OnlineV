using MenuLayer;
using Reports;
using Shipment49Web.Areas.MooringTail.Models;
using Shipment49Web.Areas.MSPS.Models;
using Shipment49Web.Common;
using Shipment49Web.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Shipment49Web.Areas.MooringTail.Controllers
{
    [Authorize]
    [UserAuthenticationFilter]
    [ErrorClass]
    public class AssignTailController : BaseController
    {
        MorringOfficeEntities context = new MorringOfficeEntities();
        SqlConnection con = ConnectionBulder.con;
       // CommonClass cls = new CommonClass();
        AssignRopeToWinch assR = new AssignRopeToWinch();
        // GET: MooringLine/AssignLine
       // public static string VesselID;
        //public static int VesselID { get; set; }
        public AssignTailController()
        {
           // VesselID = Convert.ToInt32(CommonClass.VesselSessionID);
        }
        public ActionResult Index()
        {
           int VesselID = Convert.ToInt32(Session["VesselSessionID"].ToString());
            using (ShipmentContaxt sc1 = new ShipmentContaxt())
            {

                AssignLinetoWinch model = new AssignLinetoWinch();
                var ilist = new ShipmentContaxt()
                     .MultipleResults("[dbo].[GetAssignRopeToWinch] 1,'" + VesselID + "'")
                   .With<AssignLinetoWinch>()
                   .With<AssignLinetoWinch>()
                   .Execute();
                model.AssignMooringLineList = (List<AssignLinetoWinch>)ilist[0];
                model.AssignMooringLineList1 = (List<AssignLinetoWinch>)ilist[1];
                //List<TotalCount> totobj = (List<TotalCount>)ilist[1];
                //model.Total = totobj.FirstOrDefault().Total;
                //ViewBag.TotalCount = model.Total;
                //var pager = new Pager(Convert.ToInt32(model.Total), 0);
                //model.Pager = pager;
                return View(model);
            }
        }

        public ActionResult addassignline()
        {
            int VesselID = Convert.ToInt32(Session["VesselSessionID"].ToString());
            assR.MooringLineLists = CommonClass.MooringRopeDetailList(1,VesselID);
            assR.MooringWinchLists = CommonClass.MooringWinchList(VesselID);
            return View(assR);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [PreventSpam("addassignTail", 3, 1)]
        public ActionResult addassignline(AssignRopeToWinch asswinchrope)
        {
            int VesselID = Convert.ToInt32(Session["VesselSessionID"].ToString());
            int vesselid = Convert.ToInt32(VesselID);
            try
            {
                var IdPK = ((from asn in context.AssignRopeToWinches.Where(x => x.VesselID == vesselid) select (int?)asn.Id).Max() ?? 0) + 1;

                if (asswinchrope.Outboard == true)
                {
                    asswinchrope.Outboard = true;
                }
                if (asswinchrope.Outboard == false)
                {
                    asswinchrope.Outboard = false;
                }
                asswinchrope.RopeId = asswinchrope.RopeId;
                asswinchrope.WinchId = asswinchrope.WinchId;
                //asswinchrope.AssignedDate = asswinchrope.AssignedDate;
                asswinchrope.CreatedDate = DateTime.Now;
                asswinchrope.CreatedBy = "Admin";
                asswinchrope.IsActive = true;
                asswinchrope.RopeTail = 1;
                asswinchrope.Id = IdPK;
                asswinchrope.VesselID = vesselid;
                asswinchrope.IsDelete = false;

                var duplcheck = context.AssignRopeToWinches.Where(x => x.RopeId == asswinchrope.RopeId && x.WinchId == asswinchrope.WinchId && x.IsActive == true && x.RopeTail == 1 && x.VesselID== vesselid).FirstOrDefault();
                if (duplcheck == null)
                {

                    var duplcheck1 = context.AssignRopeToWinches.SingleOrDefault(x => x.RopeId == asswinchrope.RopeId && x.IsActive == true && x.RopeTail == 1 && x.VesselID == vesselid);

                    //if (duplcheck1 != null)
                    //{
                    //    MessageBox.Show("This RopeTail is already assigned on another winch ! ", "Assign RopeTail To Winch", MessageBoxButton.OK, MessageBoxImage.Information);
                    //}
                    //else
                    //{
                    //var duplcheck2 = sc.AssignRopetoWinch.SingleOrDefault(x => x.WinchId == SRopeAss.Id && x.IsActive == true && x.RopeTail == 1);

                    if (duplcheck1 != null)
                    {
                        duplcheck1.IsActive = false;
                        context.SaveChanges();
                    }
                    //if (duplcheck2 != null)
                    //    {
                    //        duplcheck2.IsActive = false;
                    //        sc.SaveChanges();
                    //    }



                    context.AssignRopeToWinches.Add(asswinchrope);
                    context.SaveChanges();

                    TempData["Success"] = "Record successfully saved !";
                }
                else
                {
                    TempData["Error"] = "This RopeTail is already assigned !";
                    return View(asswinchrope);
                }






                //var IdPK = ((from asn in context.AssignRopeToWinches.Where(x => x.VesselID == vesselid) select (int?)asn.Id).Max() ?? 0) + 1;

                //if (asswinchrope.Outboard == true)
                //{
                //    asswinchrope.Outboard = true;
                //}
                //if (asswinchrope.Outboard == false)
                //{
                //    asswinchrope.Outboard = false;
                //}
                //asswinchrope.RopeId = asswinchrope.RopeId;
                //asswinchrope.WinchId = asswinchrope.WinchId;
                //asswinchrope.AssignedDate = asswinchrope.AssignedDate;
                //asswinchrope.CreatedDate = DateTime.Now;
                //asswinchrope.CreatedBy = "Admin";
                //asswinchrope.IsActive = true;
                //asswinchrope.RopeTail = 1;
                //asswinchrope.IsDelete = false;
                //asswinchrope.VesselID = vesselid;
                //asswinchrope.Id = IdPK;

                //var duplcheck = context.AssignRopeToWinches.Where(x => x.RopeId == asswinchrope.RopeId && x.WinchId == asswinchrope.WinchId && x.IsActive == true && x.RopeTail == 1 && x.VesselID==vesselid).FirstOrDefault();
                //if (duplcheck == null)
                //{
                //    try
                //    {
                //        //var winchidcheck1 = sc.AssignRopetoWinch.Where(x => x.RopeId == SRopeType.Id && x.IsActive == false).Select(x => x.WinchId).SingleOrDefault();
                //        var winchidcheck = context.AssignRopeToWinches.Where(x => x.RopeId == asswinchrope.RopeId && x.IsActive == true && x.VesselID == vesselid).Select(x => x.WinchId).SingleOrDefault();

                //        if (winchidcheck == 0)
                //        {
                //            //lead_check(SRopeType.Id, asswinchrope.AssignedDate, SRopeAss.Id);
                //            lead_check(asswinchrope.RopeId, asswinchrope.AssignedDate, 0,vesselid);
                //        }
                //        else
                //        {

                //            lead_check(asswinchrope.RopeId, asswinchrope.AssignedDate, winchidcheck,vesselid);
                //        }
                //    }
                //    catch { }


                //    var duplcheck1 = context.AssignRopeToWinches.SingleOrDefault(x => x.RopeId == asswinchrope.RopeId && x.IsActive == true && x.RopeTail == 1 && x.VesselID == vesselid);
                //    var duplcheck2 = context.AssignRopeToWinches.SingleOrDefault(x => x.WinchId == asswinchrope.WinchId && x.IsActive == true && x.RopeTail == 1 && x.VesselID == vesselid);

                //    if (duplcheck1 != null)
                //    {
                //        duplcheck1.IsActive = false;
                //        context.SaveChanges();
                //    }
                //    if (duplcheck2 != null)
                //    {
                //        duplcheck2.IsActive = false;
                //        context.SaveChanges();
                //    }

                //    context.AssignRopeToWinches.Add(asswinchrope);
                //    context.SaveChanges();

                //    TempData["Success"] = "Record successfully saved !";
                //}
                //else
                //{
                //    TempData["Error"] = "This Line is already assigned !";
                //    return View(asswinchrope);
                //}




            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error occured !";
                return View(asswinchrope);
            }
            return RedirectToAction("Index");
        }

        public JsonResult Getleadlocation(int Id)
        {
            int VesselID = Convert.ToInt32(Session["VesselSessionID"].ToString());
            int vesselid = Convert.ToInt32(VesselID);
            var winches = context.MooringWinchDetails.Where(x => x.Id == Id && x.VesselID== vesselid).FirstOrDefault();

            string lead = winches.Lead;
            string location = winches.Location;

            return Json(new { Result = true ,lead= lead, location=location }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult Installdatecheck(string dtvalue, int RopeId)
        {
            int VesselID = Convert.ToInt32(Session["VesselSessionID"].ToString());
            var data = context.MooringRopeDetails.Where(x => x.Id == RopeId && x.DeleteStatus == false && x.VesselID == VesselID).Select(x => x.InstalledDate).SingleOrDefault();

            var installeddate = data;

            var assigndate = dtvalue;

            if (Convert.ToDateTime(assigndate) < installeddate)
            {
                string msg = "Assign Date can not less than Line Installed date!";

                string instdt1 = installeddate.ToString();
                DateTime dateAndTime = Convert.ToDateTime(instdt1.ToString());
                string instdt = dateAndTime.ToString("yyyy-MM-dd");

                return Json(new { Result = true, instdate = instdt, Message = msg }, JsonRequestBehavior.AllowGet);
            }



            return Json(new { Result = true, instdate = "", Message = "" }, JsonRequestBehavior.AllowGet);
        }

        //[PreventSpam("DeleteAssignTail", 3, 1)]
        public ActionResult Delete(int Id)
        {
            try
            {
                int VesselID = Convert.ToInt32(Session["VesselSessionID"].ToString());
                int vesselid = Convert.ToInt32(VesselID);
                SqlDataAdapter adp = new SqlDataAdapter("update AssignRopeToWinch set isdelete='True', IsActive='False' where Id =" + Id + " and VesselID='" + vesselid + "' ", con);
                DataTable dt = new DataTable();
                adp.Fill(dt);
                TempData["Success"] = "Record successfully deleted !";
            }
            catch { }
            return RedirectToAction("Index");
        }
        public JsonResult Shifttoinactive(int Id)
        {
            try
            {
                int VesselID = Convert.ToInt32(Session["VesselSessionID"].ToString());
                int vesselid = Convert.ToInt32(VesselID);
                //int ropeid = context.AssignRopeToWinches.Where(x => x.Id == Id && x.VesselID == vesselid).Select(x => x.RopeId).SingleOrDefault();

                int ropeid = context.AssignRopeToWinches.Where(x => x.Id == Id && x.VesselID == vesselid).Select(x => x.RopeId.Value).SingleOrDefault();

                var result1 = context.AssignRopeToWinches.SingleOrDefault(b => b.RopeId == ropeid && b.RopeTail == 1 && b.IsActive == true && b.VesselID == vesselid);
                if (result1 != null)
                {

                    result1.IsActive = false;
                    result1.ModifiedBy = "Admin";
                    result1.ModifiedDate = DateTime.Now;
                    context.SaveChanges();
                    try
                    {
                        int? ropeid1 = result1.RopeId;
                        int? winchid = result1.WinchId;
                        DateTime? assigneddate = result1.AssignedDate;

                       CommonClass.lead_check(ropeid1, assigneddate, winchid,vesselid);
                    }
                    catch (Exception ex)
                    {
                       
                    }
                }             
            }
            catch (Exception ex)
            {
              
            }

            return Json(new { Result = true }, JsonRequestBehavior.AllowGet);
        }

       

    }
}