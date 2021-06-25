using MenuLayer;
using Reports;
using Shipment49Web.Areas.LooseEquipment.Models;
using Shipment49Web.Areas.MooringOperation.Models;
using Shipment49Web.Areas.MSPS.Models;
using Shipment49Web.Common;
using Shipment49Web.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Shipment49Web.Areas.MooringOperation.Controllers
{
    [Authorize]
    [UserAuthenticationFilter]
    [ErrorClass]
    public class OperationController : BaseController
    {
        // GET: MooringOperation/Operation
        SqlConnection con = ConnectionBulder.con;
        MorringOfficeEntities context = new MorringOfficeEntities();
        // public static int? winchid = 0;
        public static bool outboardEndinuse = true;
        // public static int VesselID { get; set; }
        public OperationController()
        {

            CommonClass.TopeMenuID = "Menu4";
        }
        public async Task<ActionResult> Index(int? cp)
        {
            int VesselID = Convert.ToInt32(Session["VesselSessionID"].ToString());
            OperationListClass obj = new OperationListClass();
            obj.PortNameList = CommonClass.GetPortNameList();
            // obj.PortNameList = CommonClass.GetPortNameList();
            obj.DateTo = DateTime.Now.Date;
            obj.DateFrom = DateTime.Now.Date.AddYears(-1);
            // var records = await context.MOperationBirthDetails.Where(x => x.VesselID == VesselID && x.IsActive == true && x.FastDatetime >= obj.DateFrom && x.FastDatetime <= obj.DateTo).OrderByDescending(x => x.OPId).ToListAsync();
            var records = await context.MOperationBirthDetails.Where(x => x.VesselID == VesselID && x.IsActive == true).OrderByDescending(x => x.OPId).ToListAsync();
            if (cp == null)
            {
                records.ForEach(x =>
                {
                    int FindDamages = context.RopeDamageRecords.Where(b => b.VesselID == VesselID && b.MOPId == x.OPId && b.IsActive == true).Count();
                    if (FindDamages == 0)
                    {
                        Any_Rope_Damage("No", x.OPId, VesselID);
                        x.Any_Rope_Damaged = "No";
                    }
                });
            };
            int currPage = cp == null ? 1 : Convert.ToInt32(cp);

            TempData["CurrentPage"] = currPage;
            TempData["TotalRecords"] = records.Count();

            obj.OperationList = records.Skip((CommonMethods.PAGESIZE * currPage) - CommonMethods.PAGESIZE).Take(CommonMethods.PAGESIZE).ToList();


            return View(obj);
        }


        [HttpPost]
        public async Task<ActionResult> Index(int? cp, OperationListClass opr)
        {
            int VesselID = Convert.ToInt32(Session["VesselSessionID"].ToString());
            opr.PortNameList = CommonClass.GetPortNameList();
            List<MOperationBirthDetail> records = new List<MOperationBirthDetail>();
            opr.FacilityName = opr.FacilityName;
            if (!string.IsNullOrEmpty(opr.PortName))
            {
                records = await context.MOperationBirthDetails.Where(x => x.PortName == opr.PortName && x.VesselID == VesselID && x.IsActive == true && x.FastDatetime >= opr.DateFrom && x.FastDatetime <= opr.DateTo).OrderByDescending(x => x.OPId).ToListAsync();
                if (!string.IsNullOrEmpty(opr.FacilityName))
                {
                    records = records.Where(x => x.FacilityName == opr.FacilityName).ToList();
                }
                records = records.Where(x => x.FastDatetime >= opr.DateFrom && x.FastDatetime <= opr.DateTo).OrderByDescending(x => x.OPId).ToList();
            }
            else
            {
                records = await context.MOperationBirthDetails.Where(x => x.VesselID == VesselID && x.IsActive == true && x.FastDatetime >= opr.DateFrom && x.FastDatetime <= opr.DateTo).OrderByDescending(x => x.OPId).ToListAsync();
            }

            int currPage = cp == null ? 1 : Convert.ToInt32(cp);

            TempData["CurrentPage"] = currPage;
            TempData["TotalRecords"] = records.Count();

            opr.OperationList = records.Skip((CommonMethods.PAGESIZE * currPage) - CommonMethods.PAGESIZE).Take(CommonMethods.PAGESIZE).ToList();


            return View(opr);
        }


        public ActionResult AddOperation()
        {
            int VesselID = Convert.ToInt32(Session["VesselSessionID"].ToString());
            MOperationBirthDetail obj = new MOperationBirthDetail();
            obj.PortNameList = CommonClass.GetPortNameList();
            obj.BerthTypeList = CommonClass.GetBerthOptions("BerthType");
            obj.MooringTypeList = CommonClass.GetBerthOptions("MooringType");
            obj.BerthSideList = CommonClass.GetBerthOptions("BerthSide");
            obj.VesselConditionList = CommonClass.GetBerthOptions("VesselCondition");
            obj.ShipAccessList = CommonClass.GetBerthOptions("ShipAccess");
            obj.WinchList = BindWinchList(VesselID);
            return View(obj);
        }
        [HttpPost]
        [PreventSpam("AddOperation", 3, 1)]
        public async Task<ActionResult> AddOperation(MOperationBirthDetail OPRecord) // , List<WinchCheckClass> WinchList)
        {
            int VesselID = Convert.ToInt32(Session["VesselSessionID"].ToString());
            int DorpSelection = 0; string Msg1 = "", Msg3 = "", Msg2 = "", Msg4 = "";
            var OperationRopes = OPRecord.WinchList.Where(x => x.Mark == true).ToList();

            string AllFastDatetime = OPRecord.FDate + " " + OPRecord.FTime;
            string AllCastDatetime = OPRecord.CDate + " " + OPRecord.CTime;
            OPRecord.FastDatetime = Convert.ToDateTime(AllFastDatetime);
            OPRecord.CastDatetime = Convert.ToDateTime(AllCastDatetime);

            foreach (var item in OperationRopes)
            {
                // if (string.IsNullOrEmpty(item.outboard1)

                item.outboard1 = string.IsNullOrEmpty(item.outboard1) == true ? item.LastCurrentOutboardEnd : item.outboard1;
                item.Lead = string.IsNullOrEmpty(item.Lead) == true ? item.LastCurrentLead : item.Lead;

                if (item.outboard1 == "--Select--")
                {
                    DorpSelection = 1;
                    Msg1 += "[Row No. " + item.RowSr + " select Outboard End], ";
                }
                if (item.Lead == "--Select--")
                {
                    DorpSelection = 1;
                    Msg2 += "[Row No. " + item.RowSr + " select Lead], ";
                }
                if (item.Lead1 == "--Select--")
                {
                    DorpSelection = 1;
                    Msg3 += "[Row No. " + item.RowSr + " select Lead Type], ";
                }

                var RowFastDatetime = item.FDate_g + " " + item.FTime_g;
                var RowCastDatetime = item.CDate_g + " " + item.CTime_g;
                if (RowFastDatetime != " " && RowCastDatetime != " ")
                {
                    DateTime RowFastDatetime1 = Convert.ToDateTime(RowFastDatetime);
                    DateTime RowCastDatetime1 = Convert.ToDateTime(RowCastDatetime);

                    if (RowFastDatetime1 >= OPRecord.FastDatetime && OPRecord.CastDatetime <= RowCastDatetime1)
                    {

                    }
                    else
                    {
                        DorpSelection = 1;
                        Msg4 += "[Row No. " + item.RowSr + " Cast off date & time cannot be lesser than All fast date & time], ";
                    }
                }

            }

            if (DorpSelection > 0)
            {

                OPRecord.PortNameList = CommonClass.GetPortNameList();
                OPRecord.BerthTypeList = CommonClass.GetBerthOptions("BerthType");
                OPRecord.MooringTypeList = CommonClass.GetBerthOptions("MooringType");
                OPRecord.BerthSideList = CommonClass.GetBerthOptions("BerthSide");
                OPRecord.VesselConditionList = CommonClass.GetBerthOptions("VesselCondition");
                OPRecord.ShipAccessList = CommonClass.GetBerthOptions("ShipAccess");
                OPRecord.WinchList = BindWinchList(VesselID);


                TempData["Error"] = Msg1 + Environment.NewLine + Msg2 + Environment.NewLine + Msg3 + Environment.NewLine + Msg4;
                return View(OPRecord);
            }
            else
            {

                var IdPK_OperationID = ((from asn in context.MOperationBirthDetails.Where(x => x.VesselID == VesselID) select (int?)asn.OPId).Max() ?? 0) + 1;

                if (OPRecord.PortName == "Others")
                {
                    OPRecord.PortName = OPRecord.OtherPort;
                }
                //string AllFastDatetime = OPRecord.FDate + " " + OPRecord.FTime;
                //string AllCastDatetime = OPRecord.CDate + " " + OPRecord.CTime;
                //OPRecord.FastDatetime = Convert.ToDateTime(AllFastDatetime);
                //OPRecord.CastDatetime = Convert.ToDateTime(AllCastDatetime);
                OPRecord.OPId = IdPK_OperationID;
                OPRecord.VesselID = VesselID;
                OPRecord.IsActive = true;
                context.MOperationBirthDetails.Add(OPRecord);
                context.SaveChanges();

                // Grid 0 Selected Winches >

                try
                {
                    var ropelist = await context.MooringRopeDetails.Where(x => x.IsActive == true && x.OutofServiceDate == null && x.VesselID == VesselID).ToListAsync();
                    // var assignRope = await context.AssignRopeToWinches.Where(x => x.IsActive == true && x.RopeTail == 0 && x.VesselID == VesselID).ToListAsync();
                    // var rpinspsetting = await context.tblRopeInspectionSettings.Where(x=> x.v).ToListAsync();
                    // var winchlist = context.MooringWinch.Where(x => x.IsActive == true).ToListAsync();

                    int ropetypeid = 0;

                    foreach (var item in OperationRopes)
                    {
                        item.GridID = "Grid0";
                        string RopeFastDatetime = item.FDate_g + " " + item.FTime_g;
                        string RopeCastDatetime = item.CDate_g + " " + item.CTime_g;

                        DateTime Cast = Convert.ToDateTime(RopeCastDatetime);
                        DateTime Fast = Convert.ToDateTime(RopeFastDatetime);
                        var diff = Cast.Subtract(Fast);
                        //int hours = Convert.ToInt32(diff.TotalHours);
                        decimal Linehours = Convert.ToDecimal(diff.TotalHours);

                        Linehours = Math.Round(Linehours, 2);

                        // var ropeid = item.RopeId;

                        ropetypeid = Convert.ToInt32(ropelist.Where(x => x.RopeId == item.RopeId && x.VesselID == VesselID).Select(x => x.RopeTypeId).SingleOrDefault());
                        // do save code


                        //var maxNotiid = sc.Notifications.Select(x => x.Id).Max();
                        //int notiid = maxNotiid + 1;
                        int notiid = CommonClass.NextNotiId(VesselID);

                        var IdPK_MOU = ((from asn in context.MOUsedWinchTbls.Where(x => x.VesselID == VesselID) select (int?)asn.Id).Max() ?? 0) + 1;
                        MOUsedWinchTbl used = new MOUsedWinchTbl();
                        used.Id = IdPK_MOU;
                        used.VesselID = VesselID;
                        used.OperationID = IdPK_OperationID;
                        used.GridID = item.GridID;
                        used.OPDateFrom = Fast;
                        used.OPDateTo = Cast;
                        used.WinchId = item.WinchsId;
                        used.RopeId = item.RopeId;
                        used.RopeTail = 0;
                        used.RunningHours = Linehours > 0 ? Linehours : 0;
                        used.NotificationId = notiid;
                        used.Lead = item.Lead;
                        used.Lead1 = item.Lead1;

                        var mytails = item.Tails;

                        if (item.outboard1 == "A")
                        {
                            used.Outboard = true;
                        }
                        if (item.outboard1 == "B")
                        {
                            used.Outboard = false;
                        }


                        used.Lead = used.Lead.Replace(System.Environment.NewLine, "").Trim();
                        context.MOUsedWinchTbls.Add(used);
                        await context.SaveChangesAsync();


                        // winchrotaion_Notification(ropeid, used.Lead);

                        if (Linehours > 0)
                        {
                            decimal? SumofCurrentRuningHours = 0;
                            try
                            {
                                //var StartCounter = ropelist.Where(x => x.RopeId == item.RopeId && x.VesselID == VesselID && x.RopeTail == 0).Select(x => x.StartCounterHours).SingleOrDefault();
                                //StartCounter = StartCounter == null ? 0 : StartCounter;
                                var rninghrs = ropelist.Where(x => x.RopeId == item.RopeId && x.VesselID == VesselID && x.RopeTail == 0).Select(x => x.CurrentRunningHours).SingleOrDefault();
                                rninghrs = rninghrs == null ? 0 : rninghrs;
                                //  Linehours = Convert.ToDecimal(rninghrs) + Linehours;

                                SumofCurrentRuningHours = rninghrs + Linehours;
                            }
                            catch { }

                            var result = ropelist.SingleOrDefault(x => x.RopeId == item.RopeId && x.VesselID == VesselID && x.RopeTail == 0);
                            if (result != null)
                            {
                                result.MooringOperationID = IdPK_OperationID;
                                result.CurrentLeadRunningHours = UpdateCurrentLeadRunningHours(item.RopeId, item.WinchsId, item.Lead);
                                result.CurrentRunningHours = SumofCurrentRuningHours;
                                result.ModifiedBy = "Admin";
                                result.ModifiedDate = DateTime.Now;
                                await context.SaveChangesAsync();
                            }

                            // CommonClass.WinchrotationSetting_and_Notifications(item.RopeId, item.Lead);
                            // NotificationRopeDiscard(hours, OperationID, ropeid);
                        }

                        var OperationTails = item.Tails.Where(x => x.Selected == true).ToList();
                        foreach (var tail in OperationTails)
                        { // For Tails Loop

                            decimal Tailshours = Convert.ToDecimal(diff.TotalHours);

                            int notiid4T = CommonClass.NextNotiId(VesselID);

                            var IdPK_MOU_Tail = ((from asn in context.MOUsedWinchTbls.Where(x => x.VesselID == VesselID) select (int?)asn.Id).Max() ?? 0) + 1;
                            MOUsedWinchTbl usedTail = new MOUsedWinchTbl();
                            usedTail.Id = IdPK_MOU_Tail;
                            usedTail.VesselID = VesselID;
                            usedTail.OperationID = IdPK_OperationID;
                            usedTail.GridID = item.GridID;
                            usedTail.OPDateFrom = Fast;
                            usedTail.OPDateTo = Cast;
                            usedTail.WinchId = item.WinchsId;
                            usedTail.RopeId = tail.TId;
                            usedTail.RopeTail = 1;
                            usedTail.RunningHours = Tailshours > 0 ? Tailshours : 0;
                            usedTail.NotificationId = notiid4T;
                            usedTail.Lead = item.Lead;
                            usedTail.Lead1 = item.Lead1;
                            usedTail.Outboard = null;
                            // var mytails = item.Tails;

                            usedTail.Lead = usedTail.Lead.Replace(System.Environment.NewLine, "").Trim();
                            context.MOUsedWinchTbls.Add(usedTail);
                            await context.SaveChangesAsync();

                            if (Tailshours > 0)
                            {

                                try
                                {
                                    var rninghrs = ropelist.Where(x => x.RopeId == tail.TId && x.VesselID == VesselID && x.RopeTail == 1).Select(x => x.CurrentRunningHours).SingleOrDefault();
                                    // hours = Convert.ToInt32(rninghrs) + hours;
                                    Tailshours = Convert.ToDecimal(rninghrs) + Tailshours;
                                }
                                catch { }

                                var result = ropelist.SingleOrDefault(x => x.RopeId == tail.TId && x.VesselID == VesselID && x.RopeTail == 1);
                                if (result != null)
                                {
                                    // result.CurrentLeadRunningHours = UpdateCurrentLeadRunningHours(tail.TId, item.Lead);
                                    result.MooringOperationID = IdPK_OperationID;
                                    result.CurrentRunningHours = Tailshours;
                                    result.ModifiedBy = "Admin";
                                    result.ModifiedDate = DateTime.Now;
                                    await context.SaveChangesAsync();
                                }



                            }

                        }

                    }// Rope Loop End

                }
                catch (Exception ex)
                {


                }
                CommonClass.WinchrotationSetting_and_Notifications(VesselID);
                TempData["Success"] = "Record saved successfully ";
                if (OPRecord.Any_Rope_Damaged == "Yes")
                {
                    // return RedirectToAction("AddDamage?OpId="+ IdPK_OperationID + "");

                    return RedirectToAction("AddDamage", "Operation", new { OpId = IdPK_OperationID });
                }
                else
                {

                    return RedirectToAction("Index");
                }
            }
            //MOperationRopeTail2 9778296
        }

        public decimal UpdateCurrentLeadRunningHours(int Ropeid, int WinchId, string Lead)
        {
            int VesselID = Convert.ToInt32(Session["VesselSessionID"].ToString());
            string CurrentLead = "";
            using (SqlDataAdapter pp1 = new SqlDataAdapter("select Lead from MooringWinchDetail where VesselID=" + VesselID + " and id=" + WinchId + " and IsActive=1", con))
            {
                using (DataTable dd1 = new DataTable())
                {
                    pp1.Fill(dd1);
                    if (dd1.Rows.Count > 0)
                    {
                        CurrentLead = dd1.Rows[0]["Lead"].ToString();
                    }
                }
            }

            decimal TotalRunningHours_ofLead = 0;
            using (SqlDataAdapter pp11 = new SqlDataAdapter("select SUM(runninghours) as TotalRunningHours_ofLead from mousedwinchtbl where RopeId=@RopeId and Lead=@Lead and VesselID=@VesselID", con))
            {
                pp11.SelectCommand.CommandType = CommandType.Text;
                pp11.SelectCommand.Parameters.AddWithValue("@RopeId", Ropeid);
                pp11.SelectCommand.Parameters.AddWithValue("@Lead", CurrentLead);
                pp11.SelectCommand.Parameters.AddWithValue("@VesselID", VesselID);
                DataTable dd11 = new DataTable();
                pp11.Fill(dd11);
                if (dd11.Rows.Count > 0)
                {
                    string values = dd11.Rows[0][0].ToString();
                    TotalRunningHours_ofLead = string.IsNullOrEmpty(values) == true ? 0 : Convert.ToDecimal(values);
                }

            }

            return TotalRunningHours_ofLead;

        }
        private void winchrotaion_Notification(int ropeid, string OPLead)
        {
            try
            {
                using (SqlDataAdapter adp = new SqlDataAdapter("select ropetypeid,ManufacturerId from MooringRopeDetail where RopeTail=0 and DeleteStatus=0 and  id=" + ropeid + "", con))
                {
                    DataTable dt = new DataTable();
                    adp.Fill(dt);

                    if (dt.Rows.Count > 0)
                    {
                        int ropetypeid = Convert.ToInt32(dt.Rows[0]["ropetypeid"]);
                        int manuFid = Convert.ToInt32(dt.Rows[0]["ManufacturerId"]);

                        string lead = ""; int winchid = 0;

                        SqlDataAdapter pp = new SqlDataAdapter("select b.lead,a.WinchId from assignropetowinch a inner join MooringWinchDetail b on a.WinchId=b.id where a.ropeid=" + ropeid + " and a.IsActive=1", con);
                        DataTable dd = new DataTable();
                        pp.Fill(dd);
                        if (dd.Rows.Count > 0)
                        {
                            winchid = Convert.ToInt32(dd.Rows[0]["WinchId"]);
                            lead = dd.Rows[0]["Lead"].ToString();
                            lead = lead.Replace(System.Environment.NewLine, "").Trim();
                        }



                        SqlDataAdapter pp1 = new SqlDataAdapter("select * from tblWinchRotationSetting where mooringropetype = " + ropetypeid + " and ManufacturerType= " + manuFid + " and LeadFrom='" + lead + "'", con);
                        DataTable dd1 = new DataTable();
                        pp1.Fill(dd1);
                        if (dd1.Rows.Count > 0)
                        {
                            int maxrunhrs = Convert.ToInt32(dd1.Rows[0]["MaximumRunningHours"]);
                            int maxmnthallowed = Convert.ToInt32(dd1.Rows[0]["MaximumMonthsAllowed"]);

                            string leadto = dd1.Rows[0]["LeadTo"].ToString();



                            SqlDataAdapter pp11 = new SqlDataAdapter("select SUM(runninghours) as RunningHours from mousedwinchtbl where RopeId=" + ropeid + " and Lead='" + lead + "'", con);
                            //SqlDataAdapter pp11 = new SqlDataAdapter("select SUM(runninghours) as RunningHours from mousedwinchtbl where RopeId=" + ropeid + " and Lead='" + OPLead + "'", sc.con);
                            DataTable dd11 = new DataTable();
                            pp11.Fill(dd11);
                            if (dd11.Rows.Count > 0)
                            {
                                decimal ttlrnghrs = Convert.ToDecimal(dd11.Rows[0]["RunningHours"] == DBNull.Value ? 0 : dd11.Rows[0]["RunningHours"]);

                                //if (ttlrnghrs != 0)
                                //{
                                SqlDataAdapter pp112 = new SqlDataAdapter("select SUM(runninghours) as RunningHours from WinchRotation where RopeId=" + ropeid + " and Lead='" + lead + "'", con);
                                DataTable dd112 = new DataTable();
                                pp112.Fill(dd112);
                                decimal ttlrnghrs1 = Convert.ToDecimal(dd112.Rows[0]["RunningHours"] == DBNull.Value ? 0 : dd112.Rows[0]["RunningHours"]);

                                if (ttlrnghrs > ttlrnghrs1)
                                {
                                    ttlrnghrs = ttlrnghrs - ttlrnghrs1;


                                    try
                                    {
                                        SqlDataAdapter adpt = new SqlDataAdapter("update MooringRopeDetail set CurrentLeadRunningHours=" + ttlrnghrs + " where ID=" + ropeid + "", con);
                                        DataTable ddt = new DataTable();
                                        adpt.Fill(ddt);
                                    }
                                    catch { }

                                    /*

                                    maxrunhrs1 = maxrunhrs;

                                    maxrunhrs = maxrunhrs * 90 / 100;


                                    if (ttlrnghrs > maxrunhrs1)
                                    {

                                        var cerno = sc.MooringWinchRope.Where(x => x.Id == ropeid).Select(x => x.CertificateNumber + " - " + x.UniqueID).SingleOrDefault();
                                        var winch = sc.MooringWinch.Where(x => x.Id == winchid && x.IsActive == true).Select(x => x.AssignedNumber).SingleOrDefault();


                                        var winchrotation = "Winch Rotation Exceeded for Line " + cerno + " currently on Winch " + winch + " lead  " + lead + " , Current " + ttlrnghrs + " hrs / Rotation was due at " + maxrunhrs1 + " hrs,  Please shift from " + lead + " to " + leadto + "";

                                        int NotiAlertType = (int)NotificationAlertType.Winch_Rotation_exceed;
                                        InspectNotification(ropeid, winchrotation, NotiAlertType);
                                    }
                                    else
                                    {

                                        if (maxrunhrs <= ttlrnghrs)
                                        {
                                            var cerno = sc.MooringWinchRope.Where(x => x.Id == ropeid).Select(x => x.CertificateNumber + " - " + x.UniqueID).SingleOrDefault();
                                            var winch = sc.MooringWinch.Where(x => x.Id == winchid && x.IsActive == true).Select(x => x.AssignedNumber).SingleOrDefault();


                                            var winchrotation = "Winch Rotation Approaching for Line " + cerno + " currently on Winch " + winch + " lead  " + lead + " , Current " + ttlrnghrs + " hrs / Rotation was due at " + maxrunhrs1 + " hrs,  Please shift from " + lead + " to " + leadto + "";


                                            int NotiAlertType = (int)NotificationAlertType.Winch_Rotation_approching;
                                            InspectNotification(ropeid, winchrotation, NotiAlertType);
                                        }
                                    }

                                    */
                                }
                                //}
                            }



                        }

                    }



                }
            }
            catch (Exception ex)
            {
                // sc.ErrorLog(ex);
            }
        }
        public void WinchRotationNotification(MooringRopeDetail Rope, string Lead)
        {
            /*

             try
             {

                   var assig = context.AssignRopeToWinches.Where(x => x.VesselID == VesselID && x.IsActive == true && x.RopeId == Rope.RopeId).FirstOrDefault();
                  DateTime AssignedDate = Convert.ToDateTime(assig.AssignedDate);
                 SqlDataAdapter pp1 = new SqlDataAdapter("select * from tblWinchRotationSetting where VesselID="+VesselID+" and mooringropetype = " + Rope.RopeTypeId + " and ManufacturerType= " + Rope.ManufacturerId + " and LeadFrom='" + Lead + "'", con);
                 DataTable WRSetting = new DataTable();
                 pp1.Fill(WRSetting);
                 if (WRSetting.Rows.Count > 0)
                 {
                     //int maxrunhrs = Convert.ToInt32(dd1.Rows[0]["MaximumRunningHours"]);
                     //int maxmnthallowed = Convert.ToInt32(dd1.Rows[0]["MaximumMonthsAllowed"]);
                     string leadFrom = WRSetting.Rows[0]["LeadFrom"].ToString();
                     string leadto = WRSetting.Rows[0]["LeadTo"].ToString();

                     int maxrunhrs = Convert.ToInt32(WRSetting.Rows[0]["MaximumRunningHours"]);
                     int maxmnthallowed = Convert.ToInt32(WRSetting.Rows[0]["MaximumMonthsAllowed"]);

                     var AssignedDateAppro = AssignedDate.AddMonths(maxmnthallowed - 2);
                     var AssignedDateExceed = AssignedDate.AddMonths(maxmnthallowed);
                     var CurrentDate = DateTime.Now.Date;//.AddMonths(maxmnthallowed);



                     //Approching Count Start
                     #region
                     int RB = 0; int MA = 0;
                     var WinchMonthdiff = CommonClass.DateDiffInMonths(AssignedDate, DateTime.Now.Date);//
                     if (CurrentDate >= AssignedDateAppro && CurrentDate < AssignedDateExceed)
                     {

                         var winchrotation = "Winch Rotation is Approaching for Line " + Rope.CertificateNumber + " - " + Rope.UniqueID + " currently on Winch " + assig.WinchAssignedNumber + " lead  " + Lead + " , Current " + WinchMonthdiff + " month / Rotation was due at " + maxmnthallowed + " month,  Please shift from " + Lead + " to " + leadto + "";

                         int NotiAlertType = (int)NotificationAlertType.Winch_Rotation_approching;
                         InspectNotification(ropeid, winchrotation, NotiAlertType);
                         MA++;

                     }

                     if (maxrunhrs > 0)
                     {
                         var maxrunhrs1 = maxrunhrs * 90 / 100;
                         if (CurrentLeadRunningHours > maxrunhrs1)
                         {

                             var winchrotation = "Winch Rotation is Approaching for Line " + CertificateNum + " - " + UniqueID + " currently on Winch " + WinchAssignedNumber + " lead  " + Lead + " , Current " + CurrentLeadRunningHours + " hrs / Rotation was due at " + maxrunhrs + " hrs,  Please shift from " + Lead + " to " + leadto + "";

                             int NotiAlertType = (int)NotificationAlertType.Winch_Rotation_approching;
                             InspectNotification(ropeid, winchrotation, NotiAlertType);
                             RB++;
                         }
                     }

                     if (RB + MA > 0)
                     {
                         ApprochingCount++;
                     }

                     //Approching Count End
                     #endregion
                     //*********************************************************

                     //Exceeded Count Start
                     #region
                     int RB2 = 0; int MA2 = 0;
                     if (CurrentDate >= AssignedDateExceed)
                     {
                         var winchrotation = "Winch Rotation was Exceeded for Line " + CertificateNum + " - " + UniqueID + " currently on Winch " + WinchAssignedNumber + " lead  " + Lead + " , Current " + WinchMonthdiff + " month / Rotation was due at " + maxmnthallowed + " month,  Please shift from " + Lead + " to " + leadto + "";

                         int NotiAlertType = (int)NotificationAlertType.Winch_Rotation_exceed;
                         InspectNotification(ropeid, winchrotation, NotiAlertType);
                         MA2++;
                     }

                     if (maxrunhrs > 0)
                     {
                         //var maxrunhrs1 = maxrunhrs * 90 / 100;
                         if (CurrentLeadRunningHours > maxrunhrs)
                         {
                             var winchrotation = "Winch Rotation was Exceeded for Line " + CertificateNum + " - " + UniqueID + " currently on Winch " + WinchAssignedNumber + " lead  " + Lead + " , Current " + CurrentLeadRunningHours + " hrs / Rotation was due at " + maxrunhrs + " hrs,  Please shift from " + Lead + " to " + leadto + "";

                             int NotiAlertType = (int)NotificationAlertType.Winch_Rotation_exceed;
                             InspectNotification(ropeid, winchrotation, NotiAlertType);
                             RB2++;
                         }
                     }

                     if (RB2 + MA2 > 0)
                     {
                         ExceedCount++;
                     }

                     //Exceeded Count End
                     #endregion


                 }
             }
             catch (Exception)
             {

                 throw;
             }

             */
        }

        public ActionResult ViewOperation(int OpId)
        {
            int VesselID = Convert.ToInt32(Session["VesselSessionID"].ToString());
            var Moperation = context.MOperationBirthDetails.Where(x => x.VesselID == VesselID && x.IsActive == true && x.OPId == OpId).FirstOrDefault();

            Moperation.RopeUsedInOperation = new List<View_OperationWiseRopes>();
            Moperation.RopeTailsUsedInOperation = new List<View_OperationWiseRopes>();
            var ilist = new ShipmentContaxt()
                    .MultipleResults("[dbo].[SP_OperationWiseRopes] " + OpId + "," + VesselID + "")
                  .With<SP_OperationWiseRopes>()
                  // .With<SP_OperationWiseRopes>()
                  .Execute();
            // model.AssignMooringLineList = (List<AssignLinetoWinch>)ilist[0];
            // var operations = context.View_OperationWiseRopes.Where(p => p.OperationID == OpId && p.VesselID == VesselID).ToList();
            var operations = (List<SP_OperationWiseRopes>)ilist[0];
            operations.ForEach(x =>
            {
                x.AssignedNumber = string.IsNullOrEmpty(x.AssignedNumber) == true ? "Unassigned" : x.AssignedNumber;
                x.Location = string.IsNullOrEmpty(x.Location) == true ? "Unassigned" : x.Location;
                x.FromDateTime = x.OPDateFrom.ToString("yyyy-MM-dd HH:mm");
                x.ToDateTime = x.OPDateTo.ToString("yyyy-MM-dd HH:mm");
            });

            Moperation.RopeUsedInOperation2 = operations.Where(p => p.RopeTail == 0).ToList();
            Moperation.RopeTailsUsedInOperation2 = operations.Where(p => p.RopeTail == 1).ToList();

            return View(Moperation);

            // return View();  Unassigned
        }

        public JsonResult ViewDamageRecords(int opid, int imo)
        {
            var ListDamagedRopes = context.View_RopeDamages.Where(p => p.MOPId == opid && p.VesselID == imo).ToList();
            ListDamagedRopes.ForEach(x =>
            {
                x.AssignedNumber = string.IsNullOrEmpty(x.AssignedNumber) == true ? "Unassigned" : x.AssignedNumber;
                x.Location = string.IsNullOrEmpty(x.Location) == true ? "Unassigned" : x.Location;
            });
            return Json(new { Result = true, Data = ListDamagedRopes }, JsonRequestBehavior.AllowGet);
        }

        private void updaterunnighrs(int operationid, int VesselID)
        {
            try
            {
                SqlDataAdapter adp = new SqlDataAdapter("select * from  MOUsedWinchTbl where OperationID=" + operationid + " and VesselID=" + VesselID + "", con);
                DataTable dt = new DataTable();
                adp.Fill(dt);

                decimal CurrentRunningHours = 0; decimal CurrentLeadRunningHours = 0;
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    int ropeid = Convert.ToInt32(dt.Rows[i]["RopeId"]);
                    decimal rnghrs = Convert.ToDecimal(dt.Rows[i]["RunningHours"]);


                    using (SqlDataAdapter adp1 = new SqlDataAdapter("select CurrentRunningHours,CurrentLeadRunningHours from  MooringRopeDetail where VesselID=" + VesselID + " and RopeId = " + ropeid + "", con))
                    {
                        DataTable dt1 = new DataTable();
                        adp1.Fill(dt1);
                        if (dt1.Rows.Count > 0)
                        {
                            string SSCurrentRunningHours = dt1.Rows[0]["CurrentRunningHours"].ToString();
                            if (!string.IsNullOrEmpty(SSCurrentRunningHours))
                            {
                                CurrentRunningHours = Convert.ToDecimal(SSCurrentRunningHours) - rnghrs;

                                CurrentRunningHours = CurrentRunningHours < 0 ? 0 : CurrentRunningHours;
                            }

                            string SSCurrentLeadRunningHours = dt1.Rows[0]["CurrentLeadRunningHours"].ToString();
                            if (!string.IsNullOrEmpty(SSCurrentLeadRunningHours))
                            {
                                CurrentLeadRunningHours = Convert.ToDecimal(SSCurrentLeadRunningHours) - rnghrs;
                                CurrentLeadRunningHours = CurrentLeadRunningHours < 0 ? 0 : CurrentLeadRunningHours;
                            }


                            using (SqlDataAdapter adp11 = new SqlDataAdapter("update MooringRopeDetail set CurrentRunningHours=" + CurrentRunningHours + " , CurrentLeadRunningHours = " + CurrentLeadRunningHours + "  where VesselID=" + VesselID + " and RopeId = " + ropeid + "", con))
                            {
                                DataTable dt11 = new DataTable();
                                adp11.Fill(dt11);
                            }

                        }
                    }


                }

            }
            catch { }
        }

        private void VerifyRunningHours(int vesselID)
        {
            try
            {
                //SqlDataAdapter adp = new SqlDataAdapter("select * from  MOUsedWinchTbl where OperationID=" + operationid + " and VesselID=" + VesselID + "", con);
                //DataTable dt = new DataTable();
                //adp.Fill(dt);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public ActionResult Delete(int OpId, int VesselId)
        {
            string[] MoorOpTables = { "RopeSplicingRecord", "RopeDamageRecord", "RopeCropping", "RopeEndtoEnd2", "MooringRopeDetail" };
            var findrank = context.MOperationBirthDetails.Where(x => x.VesselID == VesselId && x.OPId == OpId && x.IsActive == true).FirstOrDefault();
            if (findrank != null)
            {


                int counter = 0; string Msg = "";
                foreach (var item in MoorOpTables)
                {
                    if (item != "MooringRopeDetail")
                    {
                        string sqlqry = "select * from " + item + " where  VesselID=" + VesselId + " and MOpid = " + OpId + " and IsActive=1 ";
                        using (SqlDataAdapter sda = new SqlDataAdapter(sqlqry, con))
                        {
                            DataTable dtp = new DataTable();
                            sda.Fill(dtp);
                            if (dtp.Rows.Count > 0)
                            {
                                counter++;
                                string Mvalue = "";
                                if (item == "RopeSplicingRecord")
                                {
                                    Mvalue = "Splicing";
                                }
                                if (item == "RopeDamageRecord")
                                {
                                    Mvalue = "Damaged";
                                }
                                if (item == "RopeCropping")
                                {
                                    Mvalue = "Cropping";
                                }
                                if (item == "RopeEndtoEnd2")
                                {
                                    Mvalue = "End to End";
                                }
                                Msg = Msg + Mvalue + ",";
                            }

                        }
                    }
                    else
                    {
                        // Check for Discarded records
                        string sqlqry = "select * from MooringRopeDetail where VesselID=" + VesselId + " and  MooringOperationID = " + OpId + " and IsActive=1 and DeleteStatus=0 and OutofServiceDate is not null";
                        using (SqlDataAdapter sda = new SqlDataAdapter(sqlqry, con))
                        {
                            DataTable dtp = new DataTable();
                            sda.Fill(dtp);
                            if (dtp.Rows.Count > 0)
                            {
                                counter++;
                                string Mvalue = "";
                                if (item == "MooringRopeDetail")
                                {
                                    Mvalue = "Mooring Line Detail";
                                }
                                Msg = Msg + Mvalue + ",";
                            }

                        }

                    }
                }

                if (counter == 0)
                {

                    using (SqlDataAdapter adp2 = new SqlDataAdapter("delete from tblNotification where  VesselID=" + VesselId + " and MOP_Id=" + OpId + "", con))
                    {
                        using (DataTable dt2 = new DataTable())
                        {
                            adp2.Fill(dt2);
                        }
                    }


                    // var result11 = context.MOperationBirthDetails.SingleOrDefault(b => b.VesselID==VesselId && b.OPId == OpId && b.IsActive == true);
                    if (findrank != null)
                    {
                        updaterunnighrs(OpId, VesselId);

                        findrank.IsActive = false;
                        context.SaveChanges();

                        using (SqlDataAdapter adp = new SqlDataAdapter("delete from  MOUsedWinchTbl where  VesselID=" + VesselId + " and OperationID=" + OpId + "", con))
                        {
                            using (DataTable dt = new DataTable())
                            {
                                adp.Fill(dt);
                            }
                        }
                    }

                    var result1 = context.RopeSplicingRecords.SingleOrDefault(b => b.VesselID == VesselId && b.MOpid == OpId && b.RopeTail == 0 && b.IsActive == true);
                    if (result1 != null)
                    {

                        result1.IsActive = false;
                        result1.ModifiedBy = "Admin";

                        result1.ModifiedDate = DateTime.Now;
                        context.SaveChanges();
                    }

                    var result2 = context.RopeCroppings.SingleOrDefault(b => b.VesselID == VesselId && b.MOpid == OpId && b.RopeTail == 0 && b.IsActive == true);
                    if (result2 != null)
                    {

                        result2.IsActive = false;
                        result2.ModifiedBy = "Admin";

                        result2.ModifiedDate = DateTime.Now;
                        context.SaveChanges();
                    }

                    var result21 = context.RopeDamageRecords.SingleOrDefault(b => b.VesselID == VesselId && b.MOPId == OpId && b.RopeTail == 0 && b.IsActive == true);
                    if (result21 != null)
                    {

                        result21.IsActive = false;
                        result21.ModifiedBy = "Admin";

                        result21.ModifiedDate = DateTime.Now;
                        context.SaveChanges();
                    }


                    //var lostdata = new ObservableCollection<MOperationBirthDetail>(sc.MOperationBirthDetailTbl.ToList());
                    //MooringOPRListingViewModel cc = new MooringOPRListingViewModel(lostdata);

                    //GetMooringOperationBirthD();

                    //NextCommand = new NextPageCommandMOperation(this);
                    //PreviousCommand = new PreviousPageCommandMOperation(this);
                    //FirstCommand = new FirstPageCommandMOperation(this);
                    //LastCommand = new LastPageCommandMOperation(this);

                    // MessageBox.Show("Record deleted successfully ", "Mooring Operation", MessageBoxButton.OK, MessageBoxImage.Information);
                    TempData["Success"] = "Record deleted successfully ";

                }
                else
                {
                    TempData["Error"] = "There was a rope / rope tail  are reported in " + Msg.TrimEnd(',') + " of this operation, you cannot delete this operation before deleting the " + Msg.TrimEnd(',') + " records";
                }
            }
            else
            {
                TempData["Error"] = "Record is not found ";
            }

            return RedirectToAction("Index", "Operation", new { Area = "MooringOperation" });
        }


        [PreventSpam("DeleteDamage", 3, 1)]
        public ActionResult DeleteDamage(int Id, int MOPId)
        {
            int VesselID = Convert.ToInt32(Session["VesselSessionID"].ToString());
            var result21 = context.RopeDamageRecords.SingleOrDefault(b => b.VesselID == VesselID && b.MOPId == MOPId && b.Id == Id && b.IsActive == true);
            if (result21 != null)
            {
                if (result21.IncidentActlion == "Spliced,Cropped")
                {
                    var result1 = context.RopeSplicingRecords.SingleOrDefault(b => b.VesselID == VesselID && b.MOpid == MOPId && b.RopeId == result21.RopeId && b.IsActive == true);
                    if (result1 != null)
                    {
                        int NotificationType = (int)NotificationAlertType.RopeSplicing;
                        CommonClass.DeleteNotificationsRopeID(VesselID, result1.RopeId, NotificationType);

                        result1.IsActive = false;
                        result1.ModifiedBy = "Admin";

                        result1.ModifiedDate = DateTime.Now;
                        context.SaveChanges();


                    }

                    var result2 = context.RopeCroppings.SingleOrDefault(b => b.VesselID == VesselID && b.MOpid == MOPId && b.RopeId == result21.RopeId && b.IsActive == true);
                    if (result2 != null)
                    {
                        int NotificationType = (int)NotificationAlertType.Over_Cropping;
                        CommonClass.DeleteNotificationsRopeID(VesselID, result2.RopeId, NotificationType);

                        result2.IsActive = false;
                        result2.ModifiedBy = "Admin";

                        result2.ModifiedDate = DateTime.Now;
                        context.SaveChanges();
                    }
                }
                if (result21.IncidentActlion == "Spliced")
                {
                    var result1 = context.RopeSplicingRecords.SingleOrDefault(b => b.VesselID == VesselID && b.MOpid == MOPId && b.RopeId == result21.RopeId && b.IsActive == true);
                    if (result1 != null)
                    {
                        int NotificationType = (int)NotificationAlertType.RopeSplicing;
                        CommonClass.DeleteNotificationsRopeID(VesselID, result1.RopeId, NotificationType);

                        result1.IsActive = false;
                        result1.ModifiedBy = "Admin";

                        result1.ModifiedDate = DateTime.Now;
                        context.SaveChanges();
                    }
                }

                if (result21.IncidentActlion == "Cropped")
                {
                    var result2 = context.RopeCroppings.SingleOrDefault(b => b.VesselID == VesselID && b.MOpid == MOPId && b.RopeId == result21.RopeId && b.IsActive == true);
                    if (result2 != null)
                    {
                        int NotificationType = (int)NotificationAlertType.Over_Cropping;
                        CommonClass.DeleteNotificationsRopeID(VesselID, result2.RopeId, NotificationType);

                        result2.IsActive = false;
                        result2.ModifiedBy = "Admin";

                        result2.ModifiedDate = DateTime.Now;
                        context.SaveChanges();
                    }
                }

                if (result21.IncidentActlion == "Discarded")
                {
                    var MRopeDetail = context.MooringRopeDetails.SingleOrDefault(b => b.VesselID == VesselID && b.RopeId == result21.RopeId && b.IsActive == true);
                    if (MRopeDetail != null)
                    {
                        int NotificationType = (int)NotificationAlertType.OutofService_discarded_Rope;
                        CommonClass.DeleteNotificationsRopeID(VesselID, result21.RopeId, NotificationType);

                        MRopeDetail.OutofServiceDate = null;
                        MRopeDetail.DamageObserved = null;
                        // MRopeDetail.MooringOperationID = moorwinchrope.MOPId;
                        MRopeDetail.ReasonOutofService = null;
                        MRopeDetail.OtherReason = null;
                        MRopeDetail.ModifiedBy = "Admin";
                        //MRopeDetail.WinchId = winchid;
                        MRopeDetail.ModifiedDate = DateTime.Now;
                        context.Entry(MRopeDetail).State = EntityState.Modified;
                        context.SaveChanges();
                    }

                }

                if (result21.IncidentActlion == "End-to-end")
                {
                    var MRopeDetail = context.RopeEndtoEnd2.SingleOrDefault(b => b.VesselID == VesselID && b.MOpid == MOPId && b.RopeId == result21.RopeId && b.IsActive == true);
                    if (MRopeDetail != null)
                    {


                        MRopeDetail.IsActive = false;
                        MRopeDetail.ModifiedBy = "Admin";
                        //MRopeDetail.WinchId = winchid;
                        MRopeDetail.ModifiedDate = DateTime.Now;
                        context.Entry(MRopeDetail).State = EntityState.Modified;
                        context.SaveChanges();
                    }

                }


                //=============
                int NotificationTypedm = (int)NotificationAlertType.RopeDamage;
                CommonClass.DeleteNotificationsRopeID(VesselID, result21.RopeId, NotificationTypedm);
                result21.IsActive = false;
                result21.ModifiedBy = "Admin";

                result21.ModifiedDate = DateTime.Now;
                context.SaveChanges();

                int FindDamages = context.RopeDamageRecords.Where(b => b.VesselID == VesselID && b.MOPId == MOPId && b.IsActive == true).Count();
                if (FindDamages == 0)
                {
                    Any_Rope_Damage("No", MOPId, VesselID);
                }

                TempData["Success"] = "Damage Record deleted successfully ";
            }
            else
            {
                TempData["Error"] = "Record is not found ";

            }

            return RedirectToAction("Index", "Operation", new { Area = "MooringOperation" });
        }

        public ActionResult AddDamage(int OpId)
        {
            int VesselID = Convert.ToInt32(Session["VesselSessionID"].ToString());
            MODamageRopeClass Mobj = new MODamageRopeClass();
            Mobj.MOPId = OpId;
            Mobj.RopeListUsingOp = CommonClass.GetRopeUsedinOperation(OpId, VesselID);
            Mobj.DamageReasons = CommonClass.DamageReasonList();
            Mobj.DamageLocations = CommonClass.DamageLocatonList();
            Mobj.CroppingReasonList = CommonClass.GetCroppingReasonsList();
            Mobj.DamageObservedLists = CommonClass.DamagObservedList();
            Mobj.OutofServiceReasonList = CommonClass.OutofServiceList();
            Mobj.SubDates = BindingSubDates(OpId);
            Mobj.DamageObserved = "Mooring Operation";

            return View(Mobj);
        }
        [HttpPost]
        [PreventSpam("AddDamage", 3, 1)]
        public ActionResult AddDamage(MODamageRopeClass moorwinchrope)
        {
            int VesselID = Convert.ToInt32(Session["VesselSessionID"].ToString());
            //
            var winchid = context.AssignRopeToWinches.Where(x => x.VesselID == VesselID && x.RopeId == moorwinchrope.RopeId && x.IsActive == true).Select(x => x.WinchId).SingleOrDefault();
            var IdPK_Damage = ((from asn in context.RopeDamageRecords.Where(x => x.VesselID == VesselID) select (int?)asn.Id).Max() ?? 0) + 1;


            // For Notification
            var WinchDetail = context.MooringWinchDetails.Where(x => x.VesselID == VesselID && x.Id == winchid && x.IsActive == true).SingleOrDefault();
            var MRopeDetail = context.MooringRopeDetails.Where(x => x.RopeId == moorwinchrope.RopeId && x.VesselID == VesselID && x.OutofServiceDate == null && x.IsActive == true && x.DeleteStatus == false).SingleOrDefault();
            // End

            moorwinchrope.ActionAfterDamage = moorwinchrope.IncidentAction;
            try
            {
                var notification = "";
                //if (WinchName != null && WinchName != "" && WinchName != "Not Assigned")
                if (WinchDetail != null)
                {
                    notification = "Damaged - Line " + MRopeDetail.CertificateNumber + " - " + MRopeDetail.UniqueID + " on winch " + WinchDetail.AssignedNumber + " located at " + WinchDetail.Location + "";
                }
                else
                {
                    notification = "Damaged - Line " + MRopeDetail.CertificateNumber + " - " + MRopeDetail.UniqueID + "";
                }
                // var notification = "Damaged - Rope " + ropename + " on winch " + WinchName + " located at " + WinchLocation + "";
                var notiid = CommonClass.NextNotiId(VesselID);
                tblNotification noti = new tblNotification();
                noti.Id = notiid;
                noti.VesselId = VesselID;
                noti.MOP_Id = moorwinchrope.MOPId;
                noti.Acknowledge = false;
                noti.AckRecord = "Not yet acknowledged";
                noti.Notification = notification;
                noti.NotificationType = (int)NotificationAlertType.RopeDamage;
                //noti.NotificationDueDate = DBNull.Value;
                noti.CreatedDate = DateTime.Now;
                noti.CreatedBy = "Admin";
                noti.IsActive = true;
                noti.RopeId = moorwinchrope.RopeId;
                context.tblNotifications.Add(noti);
                context.SaveChanges();


                RopeDamageRecord rdm = new RopeDamageRecord();
                rdm.Id = IdPK_Damage;
                rdm.VesselID = VesselID;
                rdm.RopeId = moorwinchrope.RopeId;
                rdm.MOPId = moorwinchrope.MOPId;
                rdm.DamageLocation = moorwinchrope.DamageLocation;
                rdm.DamageObserved = moorwinchrope.DamageObserved;
                rdm.DamageReason = moorwinchrope.DamageReason;
                rdm.DamageDate = DateTime.Now;
                rdm.IncidentReport = moorwinchrope.IncidentReport;
                rdm.IncidentActlion = moorwinchrope.IncidentAction;
                rdm.WinchId = winchid;
                rdm.CreatedDate = DateTime.Now;
                rdm.CreatedBy = "Admin";
                rdm.IsActive = true;
                rdm.RopeTail = 0;
                rdm.NotificationId = notiid;
                if (moorwinchrope.ActionAfterDamage == "Spliced")
                {
                    if (moorwinchrope.IsCropped == "Yes")
                    {
                        rdm.IncidentActlion = "Spliced,Cropped";
                    }
                }
                context.RopeDamageRecords.Add(rdm);
                context.SaveChanges();
            }
            catch { }

            Any_Rope_Damage("Yes", moorwinchrope.MOPId, VesselID);

            TempData["Success"] = "Record saved successfully ";
            // var damageid = context.RopeDamage.Select(x => x.Id).Max();






            if (moorwinchrope.ActionAfterDamage == "Spliced")
            {
                var IdPK_Splice = ((from asn in context.RopeSplicingRecords.Where(x => x.VesselID == VesselID) select (int?)asn.Id).Max() ?? 0) + 1;

                try
                {
                    var notification = "";
                    if (WinchDetail != null)
                    {
                        notification = "Spliced - Line " + MRopeDetail.CertificateNumber + " - " + MRopeDetail.UniqueID + " on winch " + WinchDetail.AssignedNumber + " located at " + WinchDetail.Location + "";
                    }
                    else
                    {
                        notification = "Spliced - Line " + MRopeDetail.CertificateNumber + " - " + MRopeDetail.UniqueID + "";
                    }

                   var notispid = CommonClass.NextNotiId(VesselID);
                    tblNotification noti = new tblNotification();
                    noti.Id = notispid;
                    noti.VesselId = VesselID;
                    noti.MOP_Id = moorwinchrope.MOPId;
                    noti.Acknowledge = false;
                    noti.AckRecord = "Not yet acknowledged";
                    noti.Notification = notification;
                    noti.NotificationType = (int)NotificationAlertType.RopeSplicing;
                    //noti.NotificationDueDate = DBNull.Value;
                    noti.CreatedDate = DateTime.Now;
                    noti.CreatedBy = "Admin";
                    noti.IsActive = true;
                    noti.RopeId = moorwinchrope.RopeId;
                    context.tblNotifications.Add(noti);
                    context.SaveChanges();

                    RopeSplicingRecord rpspl = new RopeSplicingRecord();
                    rpspl.Id = IdPK_Splice;
                    rpspl.VesselID = VesselID;
                    rpspl.RopeId = moorwinchrope.RopeId;
                    rpspl.SplicingDoneDate = moorwinchrope.SplicedDate;
                    rpspl.SplicingDoneBy = moorwinchrope.SplicingDoneBy;
                    rpspl.SplicingMethod = moorwinchrope.SplicedMethod;
                    rpspl.IsLineCropped = moorwinchrope.IsCropped;
                    rpspl.WinchId = winchid;

                    rpspl.CreatedDate = DateTime.Now;
                    rpspl.CreatedBy = "Admin";
                    rpspl.IsActive = true;
                    rpspl.RopeTail = 0;
                    rpspl.MOpid = moorwinchrope.MOPId;
                    rpspl.DamageId = IdPK_Damage;
                    rpspl.NotificationId = notispid;
                    context.RopeSplicingRecords.Add(rpspl);
                    context.SaveChanges();


                    if (rpspl.IsLineCropped == "Yes")
                    {
                        // var maxSplicedId = sc.RopeSplicing.DefaultIfEmpty().Max(r => r == null ? 1 : r.Id);
                        //var maxSplicedId = sc.RopeSplicing.Select(x => x.Id).Max();

                        var IdPK_Crop = ((from asn in context.RopeCroppings.Where(x => x.VesselID == VesselID) select (int?)asn.Id).Max() ?? 0) + 1;


                        foreach (var item in moorwinchrope.ReasonofCropping)
                            moorwinchrope.RsCropping = moorwinchrope.RsCropping + item + ",";

                        var notiCrop = CommonClass.NextNotiId(VesselID);
                        try
                        {
                            var percent = (MRopeDetail.Length * 10) / 100;
                            var crplength = context.RopeCroppings.Where(x => x.RopeId == moorwinchrope.RopeId && x.VesselID == VesselID).Select(x => x.LengthofCroppedRope).Sum();

                            if (crplength >= percent)
                            {
                                var notificationC = "";
                                // if (WinchName != null && WinchName != "" && WinchName != "Not Assigned")
                                if (WinchDetail != null)
                                {
                                    notificationC = "Cropped more than 10% - Line " + MRopeDetail.CertificateNumber + " - " + MRopeDetail.UniqueID + " on winch " + WinchDetail.AssignedNumber + " located at " + WinchDetail.Location + "";
                                }
                                else
                                {
                                    notificationC = "Cropped more than 10% - Line " + MRopeDetail.CertificateNumber + " - " + MRopeDetail.UniqueID + "";
                                }

                                tblNotification Cropnoti = new tblNotification();

                                Cropnoti.Id = notiCrop;
                                Cropnoti.VesselId = VesselID;
                                Cropnoti.MOP_Id = moorwinchrope.MOPId;
                                Cropnoti.Acknowledge = false;
                                Cropnoti.AckRecord = "Not yet acknowledged";
                                Cropnoti.Notification = notificationC;
                                Cropnoti.NotificationType = (int)NotificationAlertType.Over_Cropping;
                                //noti.NotificationDueDate = DBNull.Value;
                                Cropnoti.CreatedDate = DateTime.Now;
                                Cropnoti.CreatedBy = "Admin";
                                Cropnoti.IsActive = true;
                                Cropnoti.RopeId = moorwinchrope.RopeId;
                                //noti.NotificationAlertType = (int)NotificationAlertType.Over_Cropping;
                                context.tblNotifications.Add(Cropnoti);
                                context.SaveChanges();
                            }

                        }
                        catch { }

                        RopeCropping rpcrp = new RopeCropping();
                        rpcrp.Id = IdPK_Crop;
                        rpcrp.VesselID = VesselID;
                        rpcrp.RopeId = moorwinchrope.RopeId;
                        rpcrp.CroppedOutboardEnd = moorwinchrope.CroppedOutboardEnd;
                        rpcrp.LengthofCroppedRope = moorwinchrope.LengthofCroppedRope;

                        rpcrp.CroppedOutboardEnd = moorwinchrope.CroppedOutboardEnd;

                        rpcrp.CroppedDate = moorwinchrope.SplicedDate;
                        rpcrp.ReasonofCropping = moorwinchrope.RsCropping.TrimEnd(',');
                        rpcrp.WinchId = winchid;
                        rpcrp.CreatedDate = DateTime.Now;
                        rpcrp.CreatedBy = "Admin";
                        rpcrp.IsActive = true;
                        rpcrp.NotificationId = notiCrop;
                        rpcrp.RopeTail = 0;
                        rpcrp.SplicedId = IdPK_Splice;
                        rpcrp.MOpid = moorwinchrope.MOPId;
                        rpcrp.DamageId = IdPK_Damage;
                        context.RopeCroppings.Add(rpcrp);
                        context.SaveChanges();



                    }


                }
                catch { }




            }



            if (moorwinchrope.ActionAfterDamage == "Cropped")
            {

                var IdPK_Crop = ((from asn in context.RopeCroppings.Where(x => x.VesselID == VesselID) select (int?)asn.Id).Max() ?? 0) + 1;

                foreach (var item in moorwinchrope.ReasonofCropping)
                    moorwinchrope.RsCropping = moorwinchrope.RsCropping + item + ",";

                var notiCrop = CommonClass.NextNotiId(VesselID);
                try
                {
                    var percent = (MRopeDetail.Length * 10) / 100;
                    var crplength = context.RopeCroppings.Where(x => x.RopeId == moorwinchrope.RopeId && x.VesselID == VesselID).Select(x => x.LengthofCroppedRope).Sum();
                   
                    if (crplength >= percent)
                    {
                        var notification = "";
                        // if (WinchName != null && WinchName != "" && WinchName != "Not Assigned")
                        if (WinchDetail != null)
                        {
                            notification = "Cropped more than 10% - Line " + MRopeDetail.CertificateNumber + " - " + MRopeDetail.UniqueID + " on winch " + WinchDetail.AssignedNumber + " located at " + WinchDetail.Location + "";
                        }
                        else
                        {
                            notification = "Cropped more than 10% - Line " + MRopeDetail.CertificateNumber + " - " + MRopeDetail.UniqueID + "";
                        }

                        tblNotification noti = new tblNotification();
                       
                        noti.Id = notiCrop;
                        noti.VesselId = VesselID;
                        noti.MOP_Id = moorwinchrope.MOPId;
                        noti.Acknowledge = false;
                        noti.AckRecord = "Not yet acknowledged";
                        noti.Notification = notification;
                        noti.NotificationType = (int)NotificationAlertType.Over_Cropping;
                        //noti.NotificationDueDate = DBNull.Value;
                        noti.CreatedDate = DateTime.Now;
                        noti.CreatedBy = "Admin";
                        noti.IsActive = true;
                        noti.RopeId = moorwinchrope.RopeId;
                        //noti.NotificationAlertType = (int)NotificationAlertType.Over_Cropping;
                        context.tblNotifications.Add(noti);
                        context.SaveChanges();
                    }

                }
                catch { }

                RopeCropping rpcrp = new RopeCropping();
                rpcrp.Id = IdPK_Crop;
                rpcrp.VesselID = VesselID;
                rpcrp.RopeId = moorwinchrope.RopeId;
                rpcrp.CroppedOutboardEnd = moorwinchrope.CroppedOutboardEnd;
                rpcrp.LengthofCroppedRope = moorwinchrope.LengthofCroppedRope;
                rpcrp.CroppedOutboardEnd = moorwinchrope.CroppedOutboardEnd;
                rpcrp.CroppedDate = moorwinchrope.CroppedDate;
                rpcrp.ReasonofCropping = moorwinchrope.RsCropping.TrimEnd(',');
                rpcrp.WinchId = winchid;
                rpcrp.CreatedDate = DateTime.Now;
                rpcrp.CreatedBy = "Admin";
                rpcrp.IsActive = true;
                rpcrp.NotificationId = notiCrop;
                rpcrp.RopeTail = 0;
                //rpcrp.SplicedId = IdPK_Splice;
                rpcrp.MOpid = moorwinchrope.MOPId;
                rpcrp.DamageId = IdPK_Damage;
                context.RopeCroppings.Add(rpcrp);
                context.SaveChanges();
    


            }


            if (moorwinchrope.ActionAfterDamage == "Discarded")
            {
                // var result = sc.MooringWinchRope.SingleOrDefault(b => b.Id == moorwinchrope.RopeId);
                if (MRopeDetail != null)
                {
                    MRopeDetail.OutofServiceDate = moorwinchrope.DiscaredDate;
                    MRopeDetail.DamageObserved = moorwinchrope.DamageObserved1;
                    MRopeDetail.MooringOperationID = moorwinchrope.MOPId;
                    MRopeDetail.ReasonOutofService = moorwinchrope.ReasonOutofService;
                    MRopeDetail.OtherReason = moorwinchrope.otherReason;
                    MRopeDetail.ModifiedBy = "Admin";
                    //MRopeDetail.WinchId = winchid;
                    MRopeDetail.ModifiedDate = DateTime.Now;
                    context.SaveChanges();


                    try
                    {
                       
                        var notification = "";
                        //if (WinchName != null && WinchName != "" && WinchName != "Not Assigned")
                        if (WinchDetail != null)
                        {
                            notification = "Out of Service / discarded - Line " + MRopeDetail.CertificateNumber + " - " + MRopeDetail.UniqueID + " on winch " + WinchDetail.AssignedNumber + " located at " + WinchDetail.Location + "";
                        }
                        else
                        {
                            notification = "Out of Service / discarded - Line " + MRopeDetail.CertificateNumber + " - " + MRopeDetail.UniqueID + "";
                        }

                        //var notification = "Out of Service / discarded - Rope " + ropename + " on winch " + WinchName + " located at " + WinchLocation + "";
                        tblNotification noti = new tblNotification();
                        noti.Id = CommonClass.NextNotiId(VesselID); 
                        noti.VesselId = VesselID;
                        noti.MOP_Id = moorwinchrope.MOPId;
                        noti.Acknowledge = false;
                        noti.AckRecord = "Not yet acknowledged";
                        noti.Notification = notification;
                        noti.NotificationType = 1;
                        //noti.NotificationDueDate = DBNull.Value;
                        noti.CreatedDate = DateTime.Now;
                        noti.CreatedBy = "Admin";
                        noti.NotificationType = (int)NotificationAlertType.OutofService_discarded_Rope;
                        noti.IsActive = true;
                        noti.RopeId = moorwinchrope.RopeId;
                        context.tblNotifications.Add(noti);
                        context.SaveChanges();
                    }
                    catch { }
                }


            }


            if (moorwinchrope.ActionAfterDamage == "End-to-end")
            {
                if (moorwinchrope.outboard1 != null)
                {

                    if (moorwinchrope.outboard1 == "A")
                    {
                        outboardEndinuse = true;
                        moorwinchrope.CurrentOutboadEndinUse = outboardEndinuse;
                    }
                    if (moorwinchrope.outboard1 == "B")
                    {
                        outboardEndinuse = false;
                        moorwinchrope.CurrentOutboadEndinUse = outboardEndinuse;
                    }

                    var IdPK_End2End = ((from asn in context.RopeEndtoEnd2.Where(x => x.VesselID == VesselID) select (int?)asn.Id).Max() ?? 0) + 1;
                    RopeEndtoEnd2 rpcrp = new RopeEndtoEnd2();
                    rpcrp.Id = IdPK_End2End;
                    rpcrp.VesselID = VesselID;
                    rpcrp.MOpid = moorwinchrope.MOPId;
                    rpcrp.RopeId = moorwinchrope.RopeId;
                    rpcrp.EndtoEndDoneDate = moorwinchrope.EndtoEndDoneDate;
                    rpcrp.CurrentOutboadEndinUse = moorwinchrope.CurrentOutboadEndinUse;

                    rpcrp.CreatedDate = DateTime.Now;
                    rpcrp.CreatedBy = "Admin";
                    rpcrp.IsActive = true;
                    rpcrp.WinchId = winchid;
                    //rpcrp.RopeTail = 0;
                    rpcrp.MOpid = moorwinchrope.MOPId;
                    rpcrp.DamageId = IdPK_Damage;
                    context.RopeEndtoEnd2.Add(rpcrp);
                    context.SaveChanges();



                }

            }


            if (moorwinchrope.ActionAfterDamage == "End-to-end" && moorwinchrope.outboard1 == null)
            {
                moorwinchrope.RopeListUsingOp = CommonClass.GetRopeUsedinOperation(moorwinchrope.MOPId, VesselID);
                moorwinchrope.DamageReasons = CommonClass.DamageReasonList();
                moorwinchrope.DamageLocations = CommonClass.DamageLocatonList();
                moorwinchrope.CroppingReasonList = CommonClass.GetCroppingReasonsList();
                moorwinchrope.DamageObservedLists = CommonClass.DamagObservedList();
                moorwinchrope.OutofServiceReasonList = CommonClass.OutofServiceList();
                moorwinchrope.SubDates = BindingSubDates(moorwinchrope.MOPId);
                moorwinchrope.DamageObserved = "Mooring Operation";

                TempData["Error"] = "Unassigned Rope can not be End to End!";
                return View(moorwinchrope);
            }
            else
            {
                //
                TempData["Success"] = "Record saved successfully ";
                return RedirectToAction("Index");
            }



            // return View(moorwinchrope);
        }

        public void Any_Rope_Damage(string IsDamage, int MopId, int VesselID)
        {
            string qry = "update MOperationBirthDetail set Any_Rope_Damaged='" + IsDamage + "' where VesselID=" + VesselID + " and OpId=" + MopId + "";
            using (SqlDataAdapter adp = new SqlDataAdapter(qry, con))
            {
                using (DataTable dt = new DataTable())
                {
                    adp.Fill(dt);
                }
            }
        }

        public JsonResult CheckCroppingLength(int crpval, int RopeId)
        {


            int VesselID = Convert.ToInt32(Session["VesselSessionID"].ToString());

            string msg = "";
            SqlDataAdapter adp = new SqlDataAdapter("select Length from MooringRopeDetail where RopeId=" + RopeId + " and VesselId ='" + VesselID + "'", con);
            DataTable dt = new DataTable();
            adp.Fill(dt);

            decimal ss1 = Convert.ToDecimal(dt.Rows[0][0]);


            decimal check = Convert.ToDecimal(crpval);

            if (check > ss1)
            {
                msg = "You cannot crop the line beyond its total length of " + ss1 + " mtrs !";
                // MessageBox.Show("You cannot crop the line beyond its total length of " + ss1 + " mtrs !", "Line Splicing", MessageBoxButton.OK, MessageBoxImage.Warning);

            }

            //int check = Convert.ToInt32(txtCroppedLength.Text);
            if (check > 999)
            {
                msg = "You can not Enter Max 3 Digit";
                // MessageBox.Show("You can not Enter Max 3 Digit", "Character Length", MessageBoxButton.OK, MessageBoxImage.Warning);

            }

            SqlDataAdapter adp1 = new SqlDataAdapter("select SUM(lengthofcroppedRope) as lengthofcroppedRope from RopeCropping where RopeId= " + RopeId + " and VesselId ='" + VesselID + "'", con);
            DataTable dt1 = new DataTable();
            adp1.Fill(dt1);

            if (dt1.Rows.Count > 0 && dt1.Rows[0][0] != DBNull.Value)
            {
                decimal totalsum = Convert.ToDecimal(dt1.Rows[0][0]);

                decimal sumtotal = Convert.ToDecimal(dt1.Rows[0][0]) + check;

                if (sumtotal > ss1)
                {
                    msg = "You cannot crop the line beyond its total length of " + ss1 + " mtrs !";
                    // MessageBox.Show("You cannot crop the line beyond its total length of " + ss1 + " mtrs !", "Line Splicing", MessageBoxButton.OK, MessageBoxImage.Warning);

                }
            }



            return Json(new { Result = true, Message = msg }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult BindFacilityName(string PortName)
        {

            // var facilityList = context.PortLists.Where(p => p.PortName.Equals(PortName)).Select(u => u.FacilityName).ToList();
            var facilityList = CommonClass.GetFacilityNameList(PortName);
            //if (facilityList.Count == 0)
            //    facilityList.Add("Other");

            return Json(new { Result = true, Data = facilityList }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetWinchlocation(int Id)
        {
            int vesselid = Convert.ToInt32(Session["VesselSessionID"].ToString());


            var data = context.AssignRopeToWinches.Where(x => x.RopeId == Id && x.IsActive == true && x.VesselID == vesselid).FirstOrDefault();
            if (data != null)
            {
                int? winchidd = data.WinchId;
                // winchid = winchidd;
                string outboard = data.Outboard == true ? "A" : "B";
                if (outboard == "A")
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
                return Json(new { Result = true, outboard = outboard, asswinch = asswinch, location = location, outboard1 = outboard1, }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                // winchid = 0;
            }

            return Json(new { Result = true, outboard = "", asswinch = "", location = "", outboard1 = "", }, JsonRequestBehavior.AllowGet);
        }

        public List<SelectListItem> BindingSubDates(int OpID)
        {
            int VesselID = Convert.ToInt32(Session["VesselSessionID"].ToString());
            var MoorRecod = context.MOperationBirthDetails.Where(x => x.OPId == OpID && x.VesselID == VesselID && x.IsActive == true).FirstOrDefault();
            List<SelectListItem> jst = new List<SelectListItem>();
            DateTime fd = Convert.ToDateTime(MoorRecod.FastDatetime);  // Convert.ToDateTime(to);
            DateTime cd = Convert.ToDateTime(MoorRecod.CastDatetime); //Convert.ToDateTime(Frm);
            var Day_Diff = (int)(cd - fd).TotalDays;

            //subDates.Clear();
            //OnPropertyChanged(new PropertyChangedEventArgs("SubDates"));
            for (int i = 0; i < Day_Diff + 1; i++)
            {
                // DateTime d1 = fd.AddDays(i);
                // var dd = d1.AddDays(i);

                //subDates.Add(fd.AddDays(i).ToShortDateString());
                jst.Add(new SelectListItem() { Text = fd.AddDays(i).ToShortDateString(), Value = fd.AddDays(i).ToShortDateString() });
            }
            //FastDate = subDates.FirstOrDefault();

            return jst;
        }

        public List<WinchCheckClass> BindWinchList(int Vesselid)
        {
            List<WinchCheckClass> LoadWinchlist = new List<WinchCheckClass>();
            try
            {
                string qry = "MOperationRopeTail2";
                using (SqlDataAdapter sda = new SqlDataAdapter(qry, con))
                {
                    sda.SelectCommand.CommandType = CommandType.StoredProcedure;
                    sda.SelectCommand.Parameters.AddWithValue("@VesselID", Vesselid);
                    DataTable datatbl = new DataTable();
                    sda.Fill(datatbl);
                    bool MarkAll = true;

                    for (int i = 0; i < datatbl.Rows.Count; i++)
                    {
                        var CurrentOutboardEndinUse = GetOutboardEndToEnd(Convert.ToInt32(datatbl.Rows[i]["RopeId"]), Vesselid);
                        LoadWinchlist.Add(new WinchCheckClass()
                        {
                            RowSr = Convert.ToInt32(datatbl.Rows[i]["RowID"]),
                            WinchsId = Convert.ToInt32(datatbl.Rows[i]["winchid"]),
                            RopeId = Convert.ToInt32(datatbl.Rows[i]["RopeId"]),
                            Location = Convert.ToString(datatbl.Rows[i]["location"] == DBNull.Value ? "" : datatbl.Rows[i]["location"]).ToString(),
                            WinchNo = datatbl.Rows[i]["AssignedNumber"].ToString(),
                            Mark = MarkAll,
                            RopeTailMark = false, //MarkAll,
                            VisibilityCheck = datatbl.Rows[i]["VisibilityCheck"].ToString(),
                            IsEditable = datatbl.Rows[i]["IsEditable"].ToString(),
                            // outboard1 = Convert.ToString(datatbl.Rows[i]["outboard1"] == DBNull.Value ? "" : datatbl.Rows[i]["outboard1"]),
                            outboard1 = CurrentOutboardEndinUse == null ? datatbl.Rows[i]["outboard1"].ToString() : CurrentOutboardEndinUse,
                            LastCurrentOutboardEnd = CurrentOutboardEndinUse == null ? datatbl.Rows[i]["outboard1"].ToString() : CurrentOutboardEndinUse,
                            //VisibilityCheck = "Hidden",
                            // Lead = "Headline",
                            Lead = string.IsNullOrEmpty(datatbl.Rows[i]["Lead"].ToString()) == true ? "--Select--" : datatbl.Rows[i]["Lead"].ToString(),
                            LastCurrentLead = string.IsNullOrEmpty(datatbl.Rows[i]["Lead"].ToString()) == true ? "--Select--" : datatbl.Rows[i]["Lead"].ToString(),
                            Lead1 = "Direct",
                            GridID = "Grid0",
                            // SortingOrder = datatbl.Rows[i]["SortingOrder"] ==  null? 999999: Convert.ToInt32(datatbl.Rows[i]["SortingOrder"]), // static id 
                            Tails = GetTailsOnWinches(Vesselid, Convert.ToInt32(datatbl.Rows[i]["winchid"]), MarkAll)

                        });
                    }

                }



            }
            catch (Exception ex) { }

            return LoadWinchlist.OrderBy(x => x.SortingOrder).ToList();
        }

        private List<TailOption> GetTailsOnWinches(int Vesselid, int winchid, bool CheckTailAll)
        {
            List<TailOption> Tails = new List<TailOption>();
            try
            {
                string sqry = @"select a.RopeId,a.Outboard,b.UniqueID,b.CertificateNumber,c.RopeType from AssignRopeToWinch a join MooringRopeDetail b on a.RopeId=b.RopeId and a.VesselID=b.VesselID 
join MooringRopeType c on c.Id = b.RopeTypeId where a.RopeTail = 1 and a.IsActive = 1 and a.VesselID = @VesselID and a.WinchId = @WinchId";
                using (SqlDataAdapter sda = new SqlDataAdapter(sqry, con))
                {
                    sda.SelectCommand.Parameters.AddWithValue("@VesselID", Vesselid);
                    sda.SelectCommand.Parameters.AddWithValue("@WinchId", winchid);
                    DataTable dtp = new DataTable();
                    sda.Fill(dtp);
                    if (dtp.Rows.Count > 0)
                    {
                        for (int i = 0; i < dtp.Rows.Count; i++)
                        {
                            var tail = new TailOption()
                            {
                                TId = Convert.ToInt32(dtp.Rows[i]["RopeId"]),
                                Name = dtp.Rows[i]["UniqueID"].ToString() + " (" + dtp.Rows[i]["RopeType"].ToString() + ")",
                                Selected = CheckTailAll
                            };
                            Tails.Add(tail);
                        }
                    }
                }

            }
            catch (Exception ex)
            {

                throw ex;
            }

            return Tails;
        }

        private string GetOutboardEndToEnd(int Ropeid, int VesselID)
        {
            // A=1 , B=0;
            string Outoard = null;
            string qry = @"select id,CurrentOutboadEndinUse from RopeEndtoEnd2 where VesselID=@VesselID and RopeId=@Ropeid and id=(select Max(Id) from RopeEndtoEnd2 where VesselID=@VesselID and RopeId=@Ropeid )";
            using (SqlDataAdapter sda = new SqlDataAdapter(qry, con))
            {
                sda.SelectCommand.CommandType = CommandType.Text;
                sda.SelectCommand.Parameters.AddWithValue("@VesselID", VesselID);
                sda.SelectCommand.Parameters.AddWithValue("@Ropeid", Ropeid);
                DataTable datatbl = new DataTable();
                sda.Fill(datatbl);
                if (datatbl.Rows.Count > 0)
                {
                    bool board = Convert.ToBoolean(datatbl.Rows[0]["CurrentOutboadEndinUse"]);
                    Outoard = board == true ? "A" : "B";
                }

            }
            return Outoard;
        }
    }
}