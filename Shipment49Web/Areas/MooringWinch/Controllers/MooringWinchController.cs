using MenuLayer;
using Newtonsoft.Json;
using Reports;
using Shipment49Web.Areas.MooringWinch.Models;
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

namespace Shipment49Web.Areas.MooringWinch.Controllers
{
    [Authorize]
    [UserAuthenticationFilter]
    [ErrorClass]
    public class MooringWinchController : BaseController
    {
        MorringOfficeEntities context = new MorringOfficeEntities();
        SqlConnection con = ConnectionBulder.con;
        //CommonClass cls = new CommonClass();
        MooringWinchDetail assR = new MooringWinchDetail();
        public static int? winchid = 0;
        public static bool outboardEndinuse = true;
      
        // GET: MooringWinch/MooringWinch
        public ActionResult Index()
        {
           string VesselID = Session["VesselSessionID"].ToString();
            using (ShipmentContaxt sc1 = new ShipmentContaxt())
            {
                MooringWinchClass model = new MooringWinchClass();
                var ilist = new ShipmentContaxt()
                     .MultipleResults("[dbo].[GetMooringWinchDetail]  '" + VesselID + "'")
                   .With<MooringWinchClass>()
                   .Execute();
                model.MooringWinchList = (List<MooringWinchClass>)ilist[0];
                return View(model);
            }
        }

        public ActionResult addmooringwinch()
        {
            assR.Leads = CommonClass.LeadBind();
            return View(assR);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [PreventSpam("addmooringwinch", 3, 1)]
        public ActionResult addmooringwinch(MooringWinchDetail mwinch)
        {
            int vesselid = Convert.ToInt32(Session["VesselSessionID"].ToString());
           
            //Add
            if (mwinch.Id == 0)
            {

                if (mwinch.MBL == null)
                {
                    TempData["Error"] = "Max Brake Holding Force can not be Null !";
                    return RedirectToAction("Index");
                }

                var mrngwnchlist = context.MooringWinchDetails.ToList();
                var IdPK = ((from asn in mrngwnchlist.Where(x => x.VesselID == vesselid) select (int?)asn.Id).Max() ?? 0) + 1;
                var sortingorder = mrngwnchlist.Where(x => x.IsActive == true && x.VesselID == vesselid).DefaultIfEmpty().Max(r => r == null ? 1 : r.SortingOrder) + 1;

                var duplchk = mrngwnchlist.Where(x => x.IsActive == true && x.VesselID == vesselid && x.AssignedNumber == mwinch.AssignedNumber).FirstOrDefault();

                if (duplchk == null)
                {

                    mwinch.AssignedNumber = mwinch.AssignedNumber;
                    mwinch.Location = mwinch.Location;
                    mwinch.Lead = mwinch.Lead;
                    mwinch.Id = IdPK;
                    mwinch.MBL = mwinch.MBL;
                    mwinch.VesselID = vesselid;

                    mwinch.SortingOrder = sortingorder;
                    mwinch.CreatedDate = DateTime.Now;
                    mwinch.CreatedBy = "Admin";
                    mwinch.IsActive = true;


                    context.MooringWinchDetails.Add(mwinch);
                }
                else
                {
                    TempData["Error"] = "Assigned number already exists !";

                    mwinch.Leads = CommonClass.LeadBind();
                    return View(mwinch);

                }

            }
            //Update
            else
            {

                //var duplchk = context.MooringWinchDetails.Where(x => x.IsActive == true && x.VesselID == vesselid && x.AssignedNumber == mwinch.AssignedNumber && x.Id != mwinch.Id).FirstOrDefault();
                //if (duplchk == null)
                //{

                    var obj = context.MooringWinchDetails.FirstOrDefault(e => e.Id == mwinch.Id && e.VesselID == vesselid);
                    if (obj != null)
                    {

                        obj.AssignedNumber = mwinch.AssignedNumber;
                        obj.Location = mwinch.Location;
                        //obj.Lead = mwinch.Lead;
                        obj.MBL = mwinch.MBL;
                        obj.IsActive = true;

                        context.Entry(obj).State = EntityState.Modified;

                    }
                //}
                //else
                //{
                //    TempData["Error"] = "Assigned number already exists !";

                //    mwinch.Leads = CommonClass.LeadBind();
                //    return View(mwinch);
                //}
            }

            try
            {
                context.SaveChanges();
                TempData["Success"] = "Record successfully saved !";
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return RedirectToAction("Index");
        }

        public ActionResult Edit(int id)
        {
            int vesselid = Convert.ToInt32(Session["VesselSessionID"].ToString());
            using (MorringOfficeEntities entities = new MorringOfficeEntities())
            {
                //var mwinchdetail = entities.MooringWinchDetails.Where(x => x.VesselID == vesselid).FirstOrDefault(e => e.Id == id);
                var mooringwinchlist = entities.MooringWinchDetails.ToList();

                assR.AssignedNumber = mooringwinchlist.Where(x => x.Id == id && x.VesselID == vesselid).Select(x => x.AssignedNumber).Distinct().SingleOrDefault();
                assR.Location = mooringwinchlist.Where(x => x.Id == id && x.VesselID == vesselid).Select(x => x.Location).Distinct().SingleOrDefault();
                assR.MBL = mooringwinchlist.Where(x => x.Id == id && x.VesselID == vesselid).Select(x => x.MBL).Distinct().SingleOrDefault();
                assR.Lead = mooringwinchlist.Where(x => x.Id == id && x.VesselID == vesselid).Select(x => x.Lead).Distinct().SingleOrDefault();


                assR.Leads = CommonClass.LeadBind();



                return View("addmooringwinch", assR);
            }
        }

        [PreventSpam("Deletemooringwinch", 3, 1)]
        public ActionResult Delete(int Id)
        {
            try
            {
                int vesselid = Convert.ToInt32(Session["VesselSessionID"].ToString());
                SqlDataAdapter adp1 = new SqlDataAdapter("select IsDelete from AssignRopeToWinch where WinchId=" + Id + " and IsDelete=0  and VesselId ='" + vesselid + "'", con);
                DataTable dt1 = new DataTable();
                adp1.Fill(dt1);
                if (dt1.Rows.Count == 0)
                {
                    SqlDataAdapter adp = new SqlDataAdapter("update MooringWinchDetail set  IsActive='False' where Id =" + Id + " and VesselId ='" + vesselid + "'", con);
                    DataTable dt = new DataTable();
                    adp.Fill(dt);




                    //MessageBox.Show("Record deleted successfully ", "Delete MooringWinch", MessageBoxButton.OK, MessageBoxImage.Information);

                }
                else
                {
                    bool isdelete = Convert.ToBoolean(dt1.Rows[0][0]);

                    if (isdelete == true)
                    {
                        SqlDataAdapter adp = new SqlDataAdapter("update MooringWinchDetail set  IsActive='False' where Id =" + Id + " and VesselId ='" + vesselid + "'", con);
                        DataTable dt = new DataTable();
                        adp.Fill(dt);




                    }
                    else
                    {
                        TempData["Error"] = "This winch id cannot be deleted as it is already assigned to an active or past record of line / rope tail ! ";
                        return RedirectToAction("Index");
                        //MessageBox.Show("This winch id cannot be deleted as it is already assigned to an active or past record of line / rope tail ! ", "Delete MooringWinch", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }

                TempData["Success"] = "Record successfully deleted !";
            }
            catch { }
            return RedirectToAction("Index");
        }
        [HttpPost]
       
        public ActionResult InsertInOrder(string winches)
        {
            try
            {

                //using (SqlDataAdapter adp1 = new SqlDataAdapter("delete from Mooringwinchdetail where vesselId='" + VesselID + "'", con))
                //{
                //    DataTable dt1 = new DataTable();
                //    adp1.Fill(dt1);
                //}
                int VesselID = Convert.ToInt32(Session["VesselSessionID"].ToString());
                MooringWinchClass morIns = new MooringWinchClass();
                var Result = winches;
                string json = Result.ToString();
                string dd = json;
               
                //int nxtinspctid = CommonClass.NextInspectionId(vesselid);

                int count = 0;
                var Json = JsonConvert.DeserializeObject<List<MooringWinchClass>>(dd);
                foreach (var rootObject in Json)
                {
                    //var IdPK = ((from asn in context.MooringWinchDetails.Where(x => x.VesselID == vesselid) select (int?)asn.Id).Max() ?? 0) + 1;

                    count++;


                    //using (SqlDataAdapter adp = new SqlDataAdapter("InsertMooringWinchInOrder", con))
                    using (SqlDataAdapter adp = new SqlDataAdapter("update MooringWinchDetail set SortingOrder=" + count + " where AssignedNumber='" + rootObject.AssignedNumber + "' and VesselID='" + VesselID + "'", con))
                    {
                        //adp.SelectCommand.CommandType = CommandType.StoredProcedure;
                        //adp.SelectCommand.Parameters.AddWithValue("@Id", IdPK);
                        //adp.SelectCommand.Parameters.AddWithValue("@AssignedNumber", rootObject.AssignedNumber);
                        //adp.SelectCommand.Parameters.AddWithValue("@Location", rootObject.Location);
                        //adp.SelectCommand.Parameters.AddWithValue("@MBL", rootObject.MBL);
                        //adp.SelectCommand.Parameters.AddWithValue("@SortingOrder", IdPK);
                        //adp.SelectCommand.Parameters.AddWithValue("@Lead", rootObject.Lead);
                        //adp.SelectCommand.Parameters.AddWithValue("@VesselID", vesselid);
                        DataTable dt = new DataTable();
                        adp.Fill(dt);


                    }

                }
            }
            catch { }

            TempData["Success"] = "Record successfully saved !";
            // RedirectToAction("Index");

            return Json(Url.Action("Index", "MooringWinch"));
            //string[] Jsonn = Json.Select(x => x.ToString()).ToArray();



        }

    }
}