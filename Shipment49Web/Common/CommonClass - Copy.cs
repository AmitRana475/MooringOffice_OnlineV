using MenuLayer;
using Reports;
using Shipment49Web.Areas.MooringLine.Models;
using Shipment49Web.Areas.MSPS.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Shipment49Web.Common
{
    public struct DateTimeSpan
    {
        public int Years { get; }
        public int Months { get; }
        public int Days { get; }
        public int Hours { get; }
        public int Minutes { get; }
        public int Seconds { get; }
        public int Milliseconds { get; }

        public DateTimeSpan(int years, int months, int days, int hours, int minutes, int seconds, int milliseconds)
        {
            Years = years;
            Months = months;
            Days = days;
            Hours = hours;
            Minutes = minutes;
            Seconds = seconds;
            Milliseconds = milliseconds;
        }

        enum Phase { Years, Months, Days, Done }

        public static DateTimeSpan CompareDates(DateTime date1, DateTime date2)
        {
            if (date2 < date1)
            {
                var sub = date1;
                date1 = date2;
                date2 = sub;
            }

            DateTime current = date1;
            int years = 0;
            int months = 0;
            int days = 0;

            Phase phase = Phase.Years;
            DateTimeSpan span = new DateTimeSpan();
            int officialDay = current.Day;

            while (phase != Phase.Done)
            {
                switch (phase)
                {
                    case Phase.Years:
                        if (current.AddYears(years + 1) > date2)
                        {
                            phase = Phase.Months;
                            current = current.AddYears(years);
                        }
                        else
                        {
                            years++;
                        }
                        break;
                    case Phase.Months:
                        if (current.AddMonths(months + 1) > date2)
                        {
                            phase = Phase.Days;
                            current = current.AddMonths(months);
                            if (current.Day < officialDay && officialDay <= DateTime.DaysInMonth(current.Year, current.Month))
                                current = current.AddDays(officialDay - current.Day);
                        }
                        else
                        {
                            months++;
                        }
                        break;
                    case Phase.Days:
                        if (current.AddDays(days + 1) > date2)
                        {
                            current = current.AddDays(days);
                            var timespan = date2 - current;
                            span = new DateTimeSpan(years, months, days, timespan.Hours, timespan.Minutes, timespan.Seconds, timespan.Milliseconds);
                            phase = Phase.Done;
                        }
                        else
                        {
                            days++;
                        }
                        break;
                }
            }

            return span;
        }
    }

    public class CommonClass
    {
        public static string VesselSessionID { get; set; }
        static SqlConnection con = ConnectionBulder.con;
        // public static string VesselID;


        public static List<MooringRopeInspections> MooringInspectionList(string year)
        {
            // VesselID = CommonClass.VesselSessionID;
            using (SqlCommand cmd = new SqlCommand("GetRopeInspection", con))
            {
                List<MooringRopeInspections> mropd = new List<MooringRopeInspections>();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Insyear", year);
                cmd.Parameters.AddWithValue("@RopeTail", 0);
                cmd.Parameters.AddWithValue("@VesselId", VesselSessionID);
                if (con.State == ConnectionState.Closed)
                    con.Open();
                using (SqlDataReader sdr = cmd.ExecuteReader())
                {
                    while (sdr.Read())
                    {
                        mropd.Add(new MooringRopeInspections
                        {
                            InspectionId = Convert.ToInt32(sdr["InspectionId"]),
                            InspectBy = sdr["InspectBy"].ToString(),
                            InspectDate = sdr["InspectDate"].ToString(),
                        });
                    }
                }
                con.Close();
                return mropd;
            }

        }

        public static void InspectNotification(int RopeID, string NotiMsg, int NotiAlertType)
        {
            int VesselID = Convert.ToInt32(VesselSessionID);
            using (MorringOfficeEntities context = new MorringOfficeEntities())
            {
                var result = context.tblNotifications.Where(x =>x.VesselId == VesselID && x.RopeId == RopeID & x.NotificationAlertType == NotiAlertType).FirstOrDefault();
                if (result == null)
                {
                    tblNotification noti = new tblNotification();
                    noti.Id = NextNotiId();
                    noti.VesselId = VesselID;
                    noti.Acknowledge = false;
                    noti.AckRecord = "Not yet acknowledged";
                    noti.Notification = NotiMsg;
                    noti.RopeId = RopeID;
                    noti.IsActive = true;
                    noti.NotificationDueDate = DateTime.Now.Date;
                    noti.CreatedDate = DateTime.Now;
                    noti.CreatedBy = "Admin";
                    // noti.NotificationAlertType = NotiAlertType;
                    noti.NotificationType = NotiAlertType;
                    context.tblNotifications.Add(noti);
                    context.SaveChanges();
                }
            }
        }
        public static void WinchrotationSetting_and_Notifications(int Ropeid, string OPLead)
        {
            try
            {


                string qry = @"select distinct a.WinchId,b.AssignedNumber,b.Location, b.lead,a.AssignedDate,R.RopeId as RopeID,R.CurrentLeadRunningHours,R.ManufacturerId,R.RopeTypeId,R.UniqueID,R.CertificateNumber,
	T.RopeType,M.Name from AssignRopeToWinch a inner join MooringWinchDetail b on a.WinchId=b.Id and a.VesselID=b.VesselID
and a.IsActive=1 and a.RopeTail=0 join MooringRopeDetail R on R.RopeId=a.RopeId and R.VesselID=a.VesselID
and R.RopeTail=0 and R.DeleteStatus=0  and R.OutofServiceDate is null
join tblCommon M on M.Id=R.ManufacturerId join MooringRopeType T on T.Id=R.RopeTypeId
where R.RopeId = "+ Ropeid + " and a.VesselID = " + VesselSessionID + "";
                using (SqlDataAdapter ssda = new SqlDataAdapter(qry, con))
                {
                    DataTable tbls = new DataTable();
                    ssda.Fill(tbls);
                    if (tbls.Rows.Count > 0)
                    {
                        //int ropetypeid = Convert.ToInt32(tbls.Rows[0]["RopeTypeId"]);
                        //int manuFid = Convert.ToInt32(tbls.Rows[0]["ManufacturerId"]);
                        //decimal CurrentLeadRunningHours = Convert.ToInt32(tbls.Rows[0]["ManufacturerId"]);
                        //string lead = tbls.Rows[0]["lead"].ToString();
                        //int winchid = Convert.ToInt32(tbls.Rows[0]["WinchId"]);
                        int ApprochingCount = 0; int ExceedCount = 0;
                        for (int i = 0; i < tbls.Rows.Count; i++)
                        {
                            int ManufacturerId = Convert.ToInt32(tbls.Rows[i]["ManufacturerId"]);
                            int RopeTypeId = Convert.ToInt32(tbls.Rows[i]["RopeTypeId"]);
                            int ropeid = Convert.ToInt32(tbls.Rows[i]["RopeID"]);
                            string TestLeadRunningHours = tbls.Rows[i]["CurrentLeadRunningHours"].ToString();
                            decimal CurrentLeadRunningHours = 0;
                            if (!string.IsNullOrEmpty(TestLeadRunningHours))
                            {
                                CurrentLeadRunningHours = Convert.ToDecimal(tbls.Rows[i]["CurrentLeadRunningHours"]);
                            }

                            //string Lead = tbls.Rows[i]["lead"].ToString();
                            //Lead = Lead.Replace(System.Environment.NewLine, "").Trim();
                            DateTime AssignedDate = Convert.ToDateTime(tbls.Rows[i]["AssignedDate"]);
                            string UniqueID = tbls.Rows[i]["UniqueID"].ToString();
                            string CertificateNum = tbls.Rows[i]["CertificateNumber"].ToString();
                            string WinchAssignedNumber = tbls.Rows[i]["AssignedNumber"].ToString();

                            using (SqlDataAdapter pp1 = new SqlDataAdapter("select * from tblWinchRotationSetting where VesselID = " + VesselSessionID + " and mooringropetype = " + RopeTypeId + " and ManufacturerType= " + ManufacturerId + " and LeadFrom='" + OPLead + "'", con))
                            {
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
                                    var WinchMonthdiff = DateDiffInMonths(AssignedDate, DateTime.Now.Date);//
                                    if (CurrentDate >= AssignedDateAppro && CurrentDate < AssignedDateExceed)
                                    {

                                        var winchrotation = "Winch Rotation is Approaching for Line " + CertificateNum + " - " + UniqueID + " currently on Winch " + WinchAssignedNumber + " lead  " + OPLead + " , Current " + WinchMonthdiff + " month / Rotation was due at " + maxmnthallowed + " month,  Please shift from " + OPLead + " to " + leadto + "";

                                        int NotiAlertType = (int)NotificationAlertType.Winch_Rotation_approching;
                                        InspectNotification(ropeid, winchrotation, NotiAlertType);
                                        MA++;

                                    }

                                    if (maxrunhrs > 0)
                                    {
                                        var maxrunhrs1 = maxrunhrs * 90 / 100;
                                        if (CurrentLeadRunningHours > maxrunhrs1)
                                        {

                                            var winchrotation = "Winch Rotation is Approaching for Line " + CertificateNum + " - " + UniqueID + " currently on Winch " + WinchAssignedNumber + " lead  " + OPLead + " , Current " + CurrentLeadRunningHours + " hrs / Rotation was due at " + maxrunhrs + " hrs,  Please shift from " + OPLead + " to " + leadto + "";

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
                                        var winchrotation = "Winch Rotation was Exceeded for Line " + CertificateNum + " - " + UniqueID + " currently on Winch " + WinchAssignedNumber + " lead  " + OPLead + " , Current " + WinchMonthdiff + " month / Rotation was due at " + maxmnthallowed + " month,  Please shift from " + OPLead + " to " + leadto + "";

                                        int NotiAlertType = (int)NotificationAlertType.Winch_Rotation_exceed;
                                        InspectNotification(ropeid, winchrotation, NotiAlertType);
                                        MA2++;
                                    }

                                    if (maxrunhrs > 0)
                                    {
                                        //var maxrunhrs1 = maxrunhrs * 90 / 100;
                                        if (CurrentLeadRunningHours > maxrunhrs)
                                        {
                                            var winchrotation = "Winch Rotation was Exceeded for Line " + CertificateNum + " - " + UniqueID + " currently on Winch " + WinchAssignedNumber + " lead  " + OPLead + " , Current " + CurrentLeadRunningHours + " hrs / Rotation was due at " + maxrunhrs + " hrs,  Please shift from " + OPLead + " to " + leadto + "";

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
                        }
                    }
                }

            }
            catch (Exception ex)
            {

               // sc.ErrorLog(ex);
            }
        }

        public static List<MooringRopeInspection> AddLineInsList()
        {
            // VesselID = CommonClass.VesselSessionID;
            using (SqlCommand cmd = new SqlCommand("RopeInspection", con))
            {
                List<MooringRopeInspection> mropd = new List<MooringRopeInspection>();
                cmd.CommandType = CommandType.StoredProcedure;
                // cmd.Parameters.AddWithValue("@Insyear", year);
                cmd.Parameters.AddWithValue("@RopeTail", 0);
                cmd.Parameters.AddWithValue("@VesselId", VesselSessionID);
                if (con.State == ConnectionState.Closed)
                    con.Open();
                using (SqlDataReader sdr = cmd.ExecuteReader())
                {
                    while (sdr.Read())
                    {
                        mropd.Add(new MooringRopeInspection
                        {
                            RopeId = Convert.ToInt32(sdr["RopeId"]),
                            WinchId = Convert.ToInt32((sdr["WinchId"] == DBNull.Value) ? 0 : sdr["WinchId"]),
                            CertificateNumber = Convert.ToString((sdr["CertificateNumber"] == DBNull.Value) ? "Not Assigned" : sdr["CertificateNumber"]),
                            AssignedNumber = Convert.ToString((sdr["AssignedNumber"] == DBNull.Value) ? "Not Assigned" : sdr["AssignedNumber"]),
                            Location = Convert.ToString((sdr["Location"] == DBNull.Value) ? "Not Assigned" : sdr["Location"]),
                            UniqueId = Convert.ToString((sdr["UniqueId"] == DBNull.Value) ? "Not Assigned" : sdr["UniqueId"]),
                            RopeType = Convert.ToString((sdr["RopeType"] == DBNull.Value) ? "Not Assigned" : sdr["RopeType"]),

                            ExternalRating_A = 0,
                            InternalRating_A = 0,
                            AverageRating_A = 0,
                            LengthOFAbrasion_A = 0.00m,
                            DistanceOutboard_A = 0.00m,
                            CutYarnCount_A = 0.00m,
                            LengthOFGlazing_A = 0.00m,

                            ExternalRating_B = 0,
                            InternalRating_B = 0,
                            AverageRating_B = 0,
                            LengthOFAbrasion_B = 0.00m,
                            DistanceOutboard_B = 0.00m,
                            CutYarnCount_B = 0.00m,
                            LengthOFGlazing_B = 0.00m,

                        });
                    }
                }
                con.Close();
                return mropd;
            }

        }
        public static List<MooringRopeDetail> MooringRopeDisposalListCommon()
        {
            // VesselID = CommonClass.VesselSessionID;
            using (SqlCommand cmd = new SqlCommand("GetAllRope", con))
            {
                List<MooringRopeDetail> mropd = new List<MooringRopeDetail>();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@RopeTail", 0);
                cmd.Parameters.AddWithValue("@VesselId", VesselSessionID);
                con.Open();
                using (SqlDataReader sdr = cmd.ExecuteReader())
                {
                    while (sdr.Read())
                    {
                        mropd.Add(new MooringRopeDetail
                        {
                            Id = Convert.ToInt32(sdr["Id"]),
                            CertificateNumber = sdr["CertificateNumber"].ToString()
                        });
                    }
                }
                con.Close();
                return mropd;
            }

        }

        public static List<string> GetYearsList()
        {
            List<string> years = new List<string>();
            int year = DateTime.Now.Year;
            for (int i = year - 7; i <= year + 7; i++)
            {
                years.Add(i.ToString());
            }
            return years;

        }


        public static List<MooringRopeDetail> MooringRopeDiscardListCommon()
        {
            // VesselID = CommonClass.VesselSessionID;
            using (SqlCommand cmd = new SqlCommand("GetActiveAssignRopeType1", con))
            {
                List<MooringRopeDetail> mropd = new List<MooringRopeDetail>();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@RopeTail", 0);
                cmd.Parameters.AddWithValue("@VesselId", VesselSessionID);
                con.Open();
                using (SqlDataReader sdr = cmd.ExecuteReader())
                {
                    while (sdr.Read())
                    {
                        mropd.Add(new MooringRopeDetail
                        {
                            Id = Convert.ToInt32(sdr["Id"]),
                            CertificateNumber = sdr["CertificateNumber"].ToString()
                        });
                    }
                }
                con.Close();
                return mropd;
            }

        }
        public static List<MooringRopeDetail> MooringRopeDetailListCommon()
        {
            //VesselID = CommonClass.VesselSessionID;
            using (SqlCommand cmd = new SqlCommand("RopeBinding", con))
            {
                List<MooringRopeDetail> mropd = new List<MooringRopeDetail>();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@RopeTail", 0);
                cmd.Parameters.AddWithValue("@VesselId", VesselSessionID);
                con.Open();
                using (SqlDataReader sdr = cmd.ExecuteReader())
                {
                    while (sdr.Read())
                    {
                        mropd.Add(new MooringRopeDetail
                        {
                            Id = Convert.ToInt32(sdr["Id"]),
                            CertificateNumber = sdr["CertificateNumber"].ToString()
                        });
                    }
                }
                con.Close();
                return mropd;
            }

        }


        public static List<MOperationBirthDetail> MooringOpListCommon()
        {
            //VesselID = CommonClass.VesselSessionID;
            using (SqlCommand cmd = new SqlCommand("BindMooringOpertaion", con))
            {
                List<MOperationBirthDetail> mropd = new List<MOperationBirthDetail>();
                cmd.CommandType = CommandType.StoredProcedure;
                // cmd.Parameters.AddWithValue("@RopeTail", 0);
                cmd.Parameters.AddWithValue("@VesselID", VesselSessionID);
                con.Open();
                using (SqlDataReader sdr = cmd.ExecuteReader())
                {
                    while (sdr.Read())
                    {
                        mropd.Add(new MOperationBirthDetail
                        {
                            OPId = Convert.ToInt32(sdr["OPId"]),
                            Operation = sdr["Operation"].ToString()
                        });
                    }
                }
                con.Close();
                return mropd;
            }

        }
        public static List<DamageR> DamageReasonList()
        {
            List<DamageR> dmgrsn = new List<DamageR>();
            // VesselID = CommonClass.VesselSessionID;
            using (SqlDataAdapter cmd = new SqlDataAdapter("select * from damagecroppingreason", con))
            {
                DataTable dt = new DataTable();
                cmd.Fill(dt);
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    dmgrsn.Add(new DamageR
                    {
                        Id = Convert.ToInt32(dt.Rows[i]["Id"]),
                        DamageReasonL = dt.Rows[i]["DamageReason"].ToString()
                    });
                }
                con.Close();
            }
            return dmgrsn;
        }

        public static List<DamageL> DamageLocatonList()
        {
            List<DamageL> dmgrsn = new List<DamageL>();
            //VesselID = CommonClass.VesselSessionID;
            using (SqlDataAdapter cmd = new SqlDataAdapter("select * from damagelocation", con))
            {
                DataTable dt = new DataTable();
                cmd.Fill(dt);
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    dmgrsn.Add(new DamageL
                    {
                        Id = Convert.ToInt32(dt.Rows[i]["Id"]),
                        DamageLocationL = dt.Rows[i]["DamageLocation"].ToString()
                    });
                }
                con.Close();
            }
            return dmgrsn;
        }


        public static List<DamageObserved> DamagObservedList()
        {
            List<DamageObserved> dmgobs = new List<DamageObserved>();
            // VesselID = CommonClass.VesselSessionID;
            using (SqlDataAdapter cmd = new SqlDataAdapter("select distinct Id,DamageObserved from DamageObserved", con))
            {
                DataTable dt = new DataTable();
                cmd.Fill(dt);
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    dmgobs.Add(new DamageObserved
                    {
                        Id = Convert.ToInt32(dt.Rows[i]["Id"]),
                        DamageObserved1 = dt.Rows[i]["DamageObserved"].ToString()
                    });
                }
                con.Close();
            }
            return dmgobs;
        }

        public static List<OutofServiceR> OutofServiceList()
        {
            List<OutofServiceR> dmgrsn = new List<OutofServiceR>();
            //VesselID = CommonClass.VesselSessionID;
            using (SqlDataAdapter cmd = new SqlDataAdapter("select * from OutofServiceReason", con))
            {
                DataTable dt = new DataTable();
                cmd.Fill(dt);
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    dmgrsn.Add(new OutofServiceR
                    {
                        Id = Convert.ToInt32(dt.Rows[i]["Id"]),
                        Reason = dt.Rows[i]["Reason"].ToString()
                    });
                }
                con.Close();
            }
            return dmgrsn;
        }

        public static List<MooringRopeDetail> MooringRopeDetailListEndtoEnd()
        {
            //VesselID = CommonClass.VesselSessionID;
            using (SqlCommand cmd = new SqlCommand("select a.id,a.UniqueID + ' - ' +  a.certificatenumber as certificatenumber, b.ropeid from MooringRopeDetail a inner join AssignRopeToWinch b  on a.id=b.ropeid  where a.DeleteStatus=0 and a.VesselId='" + VesselSessionID + "' and OutofServiceDate is null and b.RopeTail=0 and  b.IsActive=1 order by a.UniqueID asc", con))
            {
                List<MooringRopeDetail> mropd = new List<MooringRopeDetail>();
                cmd.CommandType = CommandType.Text;
                con.Open();
                using (SqlDataReader sdr = cmd.ExecuteReader())
                {
                    while (sdr.Read())
                    {
                        mropd.Add(new MooringRopeDetail
                        {
                            Id = Convert.ToInt32(sdr["Id"]),
                            CertificateNumber = sdr["CertificateNumber"].ToString()
                        });
                    }
                }
                con.Close();
                return mropd;
            }

        }
        public static List<MooringRopeDetail> MooringRopeDetailList()
        {
            //VesselID = CommonClass.VesselSessionID;
            using (SqlCommand cmd = new SqlCommand("select Id,UniqueId +' - ' + CertificateNumber as CertificateNumber from MooringRopeDetail where ropetail=0 and OutofServiceDate is null and DeleteStatus=0 and VesselId='" + VesselSessionID + "' and InstalledDate is not null order by UniqueID asc", con))
            {
                List<MooringRopeDetail> mropd = new List<MooringRopeDetail>();
                cmd.CommandType = CommandType.Text;
                con.Open();
                using (SqlDataReader sdr = cmd.ExecuteReader())
                {
                    while (sdr.Read())
                    {
                        mropd.Add(new MooringRopeDetail
                        {
                            Id = Convert.ToInt32(sdr["Id"]),
                            CertificateNumber = sdr["CertificateNumber"].ToString()
                        });
                    }
                }
                con.Close();
                return mropd;
            }

        }
        public static int NextNotiId()
        {
            int notiid = 0;
            try
            {
                using (SqlDataAdapter adp = new SqlDataAdapter("select MAX(Id)+1 from tblNotification where VesselId ='" + VesselSessionID + "'", con))
                {
                    DataTable dt = new DataTable();
                    adp.Fill(dt);
                    notiid = Convert.ToInt32(dt.Rows[0][0]);

                    return notiid;
                }
            }
            catch { return notiid; }

        }
        public static List<MooringWinchDetail> MooringWinchList()
        {
            //VesselID = CommonClass.VesselSessionID;
            using (SqlCommand cmd = new SqlCommand("select Id,AssignedNumber from MooringWinchDetail where IsActive=1 and VesselId='" + VesselSessionID + "'", con))
            {
                List<MooringWinchDetail> mropd = new List<MooringWinchDetail>();
                cmd.CommandType = CommandType.Text;
                con.Open();
                using (SqlDataReader sdr = cmd.ExecuteReader())
                {
                    while (sdr.Read())
                    {
                        mropd.Add(new MooringWinchDetail
                        {
                            Id = Convert.ToInt32(sdr["Id"]),
                            AssignedNumber = sdr["AssignedNumber"].ToString()
                        });
                    }
                }
                con.Close();
                return mropd;
            }

        }

        public static List<ChafeGuardCondition> chafeGuardConditions()
        {
            //VesselID = CommonClass.VesselSessionID;
            using (SqlCommand cmd = new SqlCommand("select * from ChafeGuardCondition", con))
            {
                List<ChafeGuardCondition> mropd = new List<ChafeGuardCondition>();
                //cmd.CommandType = CommandType.StoredProcedure;
                //cmd.Parameters.AddWithValue("@RopeTail", 0);
                //cmd.Parameters.AddWithValue("@VesselId", VesselID);
                con.Open();
                using (SqlDataReader sdr = cmd.ExecuteReader())
                {
                    while (sdr.Read())
                    {
                        mropd.Add(new ChafeGuardCondition
                        {
                            Id = Convert.ToInt32(sdr["Id"]),
                            ChafeGuard = sdr["ChafeGuard"].ToString()
                        });
                    }
                }
                con.Close();
                return mropd;
            }

        }

        public static int NextInspectionId(int vesselid)
        {
            int inspectionid = 0;
            try
            {
                SqlDataAdapter adp = new SqlDataAdapter("select max(inspectionid) from mooringropeinspection where VesselID='" + vesselid + "'", con);
                System.Data.DataTable dt = new System.Data.DataTable();
                adp.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    inspectionid = Convert.ToInt32(dt.Rows[0][0]) + 1;
                }
                else
                {
                    inspectionid = 1;
                }

                return inspectionid;

            }
            catch
            {
                inspectionid = 1;
                return inspectionid;
            }

        }

        public static List<MooringLooseEquipInspection> AddLEInspectionList(int type, int VesselID)
        {
            // VesselID = CommonClass.VesselSessionID;
            List<MooringLooseEquipInspection> ForLEInspsction = new List<MooringLooseEquipInspection>();
            using (ShipmentContaxt sc1 = new ShipmentContaxt())
            {
                if (type == 1)
                {
                    //JoiningShackle 
                    var JShackle = new ShipmentContaxt().MultipleResults("[dbo].[LooseEquipmentTypeDetails] 'JShackle','" + VesselID + "'")
                       .With<JoiningShackle>().Execute();

                    var JoiningShackleList = (List<JoiningShackle>)JShackle[0];

                    foreach (var item in JoiningShackleList)
                    {
                        ForLEInspsction.Add(new MooringLooseEquipInspection()
                        {
                            Number = item.CertificateNumber + " - " + item.UniqueID,
                            LooseEtbPK = item.Id,
                            LooseETypeId = type,
                        });


                    }
                }

                if (type == 4)
                {
                    //RopeStopper 
                    var RTail = new ShipmentContaxt()
                    .MultipleResults("[dbo].[LooseEquipmentTypeDetails] 'RTail','" + VesselID + "'").With<RopeTail>().Execute();
                    var RopeStopperList = (List<RopeTail>)RTail[0];

                    foreach (var item in RopeStopperList)
                    {
                        ForLEInspsction.Add(new MooringLooseEquipInspection()
                        {
                            Number = item.CertificateNumber + " - " + item.UniqueID,
                            LooseEtbPK = item.Id,
                            LooseETypeId = type,

                        });
                    }
                }

                if (type == 9)
                {
                    //TowingRope 
                    var TowingRope = new ShipmentContaxt()
                   .MultipleResults("[dbo].[LooseEquipmentTypeDetails] 'TowingRope','" + VesselID + "'").With<RopeTail>().Execute();
                    var TowingRopeList = (List<RopeTail>)TowingRope[0];

                    foreach (var item in TowingRopeList)
                    {
                        ForLEInspsction.Add(new MooringLooseEquipInspection()
                        {
                            Number = item.CertificateNumber + " - " + item.UniqueID,
                            LooseEtbPK = item.Id,
                            LooseETypeId = type,
                        });
                    }
                }
                if (type == 10)
                {
                    //SuezRope 
                    var SuezRope = new ShipmentContaxt()
                   .MultipleResults("[dbo].[LooseEquipmentTypeDetails] 'SuezRope','" + VesselID + "'").With<RopeTail>().Execute();
                    var SuezRopeRopeList = (List<RopeTail>)SuezRope[0];

                    foreach (var item in SuezRopeRopeList)
                    {
                        ForLEInspsction.Add(new MooringLooseEquipInspection()
                        {
                            Number = item.CertificateNumber + " - " + item.UniqueID,
                            LooseEtbPK = item.Id,
                            LooseETypeId = type,
                        });

                    }
                }

                if (type == 3)
                {
                    //MessengerRope
                    var MRope = new ShipmentContaxt()
                  .MultipleResults("[dbo].[LooseEquipmentTypeDetails] 'MRope','" + VesselID + "'").With<RopeTail>().Execute();
                    var MessengerRopeList = (List<RopeTail>)MRope[0];

                    foreach (var item in MessengerRopeList)
                    {
                        ForLEInspsction.Add(new MooringLooseEquipInspection()
                        {
                            Number = item.CertificateNumber + " - " + item.UniqueID,
                            LooseEtbPK = item.Id,
                            LooseETypeId = type,
                        });

                    }
                }

                if (type == 6)
                {
                    //FireWire
                    var FireWire = new ShipmentContaxt()
                  .MultipleResults("[dbo].[LooseEquipmentTypeDetails] 'FireWire','" + VesselID + "'").With<RopeTail>().Execute();
                    var FireWireList = (List<RopeTail>)FireWire[0];

                    foreach (var item in FireWireList)
                    {
                        ForLEInspsction.Add(new MooringLooseEquipInspection()
                        {
                            Number = item.CertificateNumber + " - " + item.UniqueID,
                            LooseEtbPK = item.Id,
                            LooseETypeId = type,
                        });
                    }
                }

                if (type == 5)
                {
                    //ChainStopper
                    var CStopper = new ShipmentContaxt()
                 .MultipleResults("[dbo].[LooseEquipmentTypeDetails] 'CStopper','" + VesselID + "'").With<ChainStopper>().Execute();
                    var ChainStopperList = (List<ChainStopper>)CStopper[0];

                    foreach (var item in ChainStopperList)
                    {
                        ForLEInspsction.Add(new MooringLooseEquipInspection()
                        {
                            Number = item.CertificateNumber + " - " + item.UniqueID,
                            LooseEtbPK = item.Id,
                            LooseETypeId = type,
                        });
                    }
                }

                if (type == 7)
                {
                    //ChafeGuard
                    var ChafeGuard = new ShipmentContaxt()
                 .MultipleResults("[dbo].[LooseEquipmentTypeDetails] 'ChafeGuard','" + VesselID + "'").With<ChafeGuard>().Execute();
                    var ChafeGuardList = (List<ChafeGuard>)ChafeGuard[0];

                    foreach (var item in ChafeGuardList)
                    {
                        ForLEInspsction.Add(new MooringLooseEquipInspection()
                        {
                            Number = item.CertificateNumber + " - " + item.UniqueID,
                            LooseEtbPK = item.Id,
                            LooseETypeId = type,
                        });
                    }
                }

                if (type == 8)
                {
                    //WinchBreakTestKit
                    var WinchBreakTestKit = new ShipmentContaxt()
                 .MultipleResults("[dbo].[LooseEquipmentTypeDetails] 'WinchBreakTestKit','" + VesselID + "'").With<WinchBreakTestKit>().Execute();
                    var WinchBreakTestKitList = (List<WinchBreakTestKit>)WinchBreakTestKit[0];

                    foreach (var item in WinchBreakTestKitList)
                    {
                        ForLEInspsction.Add(new MooringLooseEquipInspection()
                        {
                            Number = item.CertificateNumber + " - " + item.UniqueID,
                            LooseEtbPK = item.Id,
                            LooseETypeId = type,
                        });
                    }
                }

                return ForLEInspsction;
            }

        }

        public static DateTime? LEqInspectionDate(DateTime DateInstalled, int LEType)
        {
            try
            {

                SqlDataAdapter adp = new SqlDataAdapter("select * from tblLooseEquipInspectionSetting where EquipmentType=" + LEType + "", con);
                DataTable dt = new DataTable();
                adp.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    //decimal rating1 = Convert.ToDecimal(dt.Rows[0]["MaximumMonthsAllowed"]);
                    //int rat = Convert.ToInt32(rating1);

                    decimal datecheck = Convert.ToDecimal(dt.Rows[0]["InspectionFrequency"]);
                    //decimal perchk = Convert.ToDecimal(dt.Rows[0]["InspectionFrequency"]) * 30 / 100;
                    //perchk = perchk * 100;
                    //int near = Convert.ToInt32(perchk);
                    // DateTime inspectduedate = Convert.ToDateTime(joiningShackle.DateInstalled).AddDays(near);



                    int firstValue = 0; int secondValue = 0; DateTime nextMonth;
                    double value = Convert.ToDouble(datecheck);
                    string a = value.ToString();
                    string[] b = a.Split('.');
                    firstValue = int.Parse(b[0]);
                    if (b.Length == 2)
                    {
                        secondValue = int.Parse(b[1]);
                    }
                    int chekint = Convert.ToInt32(datecheck);
                    DateTime date = DateInstalled;
                    if (secondValue == 0)
                    {
                        nextMonth = date.AddMonths(chekint);
                    }
                    else
                    {
                        if (chekint != 0)
                        {
                            nextMonth = date.AddMonths(chekint).AddDays(-15);
                        }
                        else
                        {
                            nextMonth = date.AddDays(15);
                        }
                    }

                    //int chekint = Convert.ToInt32(datecheck);
                    //DateTime date = Convert.ToDateTime(joiningShackle.DateInstalled);
                    //DateTime nextMonth = date.AddMonths(chekint);
                    TimeSpan t = nextMonth - date;
                    double NrOfDays = t.TotalDays;
                    DateTime inspectduedate = DateInstalled.AddDays(NrOfDays);

                    DateTime crntdt = DateTime.Now;
                    if (inspectduedate <= crntdt)
                    {
                        inspectduedate = DateTime.Now;
                    }

                    return inspectduedate;
                    //DateTime inspectduedate = Convert.ToDateTime(joiningShackle.DateInstalled).AddMonths(rat);

                    //joiningShackle.InspectionDueDate = inspectduedate;
                }
                else
                {
                    return null;
                }

            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static int DateDiffInMonths(DateTime StartDate, DateTime Nowdate)
        {
            var dateSpan = DateTimeSpan.CompareDates(StartDate, Nowdate);
            int DifYear = dateSpan.Years < 0 ? 0 : dateSpan.Years;

            int mmmm = (DifYear * 12) + dateSpan.Months;

            return mmmm;
        }
        public static List<SelectListItem> GetPortNameList()
        {
            List<SelectListItem> jst = new List<SelectListItem>();
            using (SqlDataAdapter sda = new SqlDataAdapter("select distinct PortName from PortList", con))
            {
                DataTable tbl = new DataTable();
                sda.Fill(tbl);
                if (tbl.Rows.Count > 0)
                {
                   // jst.Add(new SelectListItem() { Text = "None Selected", Value = "None Selected" });
                    for (int i = 0; i < tbl.Rows.Count; i++)
                    {

                        jst.Add(new SelectListItem() { Text = tbl.Rows[i]["PortName"].ToString(), Value = tbl.Rows[i]["PortName"].ToString() });

                    }
                    //jst.Add(new SelectListItem() { Text = "Other", Value = "Other" });
                    
                }
            }
            return jst;
        }

        public static List<SelectListItem> GetBerthOptions(string ColmunName)
        {
            List<SelectListItem> jst = new List<SelectListItem>();
            using (SqlDataAdapter sda = new SqlDataAdapter("select distinct "+ ColmunName + " from BerthOptionList", con))
            {
                DataTable tbl = new DataTable();
                sda.Fill(tbl);
                if (tbl.Rows.Count > 0)
                {
                    //jst.Add(new SelectListItem() { Text = "None Selected", Value = "None Selected" });
                    for (int i = 0; i < tbl.Rows.Count; i++)
                    {

                        jst.Add(new SelectListItem() { Text = tbl.Rows[i][ColmunName].ToString(), Value = tbl.Rows[i][ColmunName].ToString() });

                    }

                }
            }
            return jst;
        }

     

        public static List<string> GetFacilityNameList(string PortName)
        {
            List<string> jst = new List<string>();
            using (SqlDataAdapter sda = new SqlDataAdapter("select distinct FacilityName from PortList where PortName='"+ PortName + "'", con))
            {
                DataTable tbl = new DataTable();
                sda.Fill(tbl);
                if (tbl.Rows.Count > 0)
                {
                    for (int i = 0; i < tbl.Rows.Count; i++)
                    {
                        //jst.Add(new SelectListItem() { Text = tbl.Rows[i]["FacilityName"].ToString(), Value = tbl.Rows[i]["FacilityName"].ToString() });
                        jst.Add(tbl.Rows[i]["FacilityName"].ToString());
                    }
                    jst.Add("Others");
                }
                //else
                //{
                //    jst.Add("Others");
                //}
                //jst.Add(new SelectListItem() { Text = "Other", Value = "Other" });
            }
            return jst;
        }

        public static List<SelectListItem> GetCroppingReasonsList()
        {
            List<SelectListItem> jst = new List<SelectListItem>();
            using (SqlDataAdapter sda = new SqlDataAdapter("select * from CroppingReasons", con))
            {
                DataTable tbl = new DataTable();
                sda.Fill(tbl);
                if (tbl.Rows.Count > 0)
                {
                    for (int i = 0; i < tbl.Rows.Count; i++)
                    {

                        jst.Add(new SelectListItem() { Text = tbl.Rows[i]["Reasons"].ToString(), Value = tbl.Rows[i]["Reasons"].ToString() });

                    }

                }
            }
            return jst;
        }

      

        public static List<SelectListItem> GetRopeUsedinOperation(int OperationId,int VesselID)
        {
            List<SelectListItem> jst = new List<SelectListItem>();
            using (SqlDataAdapter sda = new SqlDataAdapter("RopeinMopDamage", con))
            {  sda.SelectCommand.CommandType = CommandType.StoredProcedure;
                sda.SelectCommand.Parameters.AddWithValue("@OperationId", OperationId);
                sda.SelectCommand.Parameters.AddWithValue("@VesselID", VesselID);

                DataTable tbl = new DataTable();
                sda.Fill(tbl);
                if (tbl.Rows.Count > 0)
                {
                    for (int i = 0; i < tbl.Rows.Count; i++)
                    {

                        jst.Add(new SelectListItem() { Text = tbl.Rows[i]["AssignedNumber"].ToString(), Value = tbl.Rows[i]["Id"].ToString() });

                    }

                }
            }
            return jst;
        }

     

    }

   
}