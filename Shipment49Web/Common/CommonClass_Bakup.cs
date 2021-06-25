using MenuLayer;
using Reports;
using Shipment49Web.Areas.MooringLine.Models;
using Shipment49Web.Areas.MooringTail.Models;
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
        public static UserInfo UserInformation { get; set; }

        public static string TopeMenuID { get; set; } = "Menu1";

        public static string GetIpAddress(HttpRequestBase Request)
        {
            // Request.UserHostAddress returns the IP address of the client.
            var userip = Request.UserHostAddress;
            if (Request.UserHostAddress != null)
            {
                return userip;
            }
            //In case the Request.UserHostAddress returns null then we need to read the ip address from the ServerVariables
            else
            {
                //Request.ServerVariables["HTTP_X_FORWARDED_FOR"] will have value if the client machine is using a proxy server 
                userip = Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                if (string.IsNullOrEmpty(userip))
                {
                    //Request.ServerVariables["REMOTE_ADDR"] contains the ip address of the client.
                    userip = Request.ServerVariables["REMOTE_ADDR"];
                }
            }
            return userip;
        }

        public static void LoggedIPAddressesAdd(string IPAddress, string MethodName, DateTime Created)
        {
           
            using (SqlDataAdapter pp = new SqlDataAdapter("INSERT INTO RateLimit VALUES(@IPAddress,@MethodName,@CreatedOn,@VesselId); ", con))
            {
                pp.SelectCommand.Parameters.AddWithValue("@IPAddress", IPAddress);
                pp.SelectCommand.Parameters.AddWithValue("@MethodName", MethodName);
                pp.SelectCommand.Parameters.AddWithValue("@CreatedOn", Created);
                pp.SelectCommand.Parameters.AddWithValue("@VesselId",123456);
                DataTable dd = new DataTable();
                pp.Fill(dd);
            }


        }

        public static int LoggedIPAddressesGet(string IPAddress, string MethodName, DateTime Created)
        {
            int count = 0;

            using (SqlDataAdapter pp = new SqlDataAdapter("select Count(*) from RateLimit where IPAddress=@IPAddress  and MethodName=@MethodName and CreatedOn > @CreatedOn ", con))
            {
                pp.SelectCommand.Parameters.AddWithValue("@IPAddress", IPAddress);
                pp.SelectCommand.Parameters.AddWithValue("@MethodName", MethodName);
                pp.SelectCommand.Parameters.AddWithValue("@CreatedOn", Created);
               // pp.SelectCommand.Parameters.AddWithValue("@VesselId", 123456);
                DataTable dd = new DataTable();
                pp.Fill(dd);
                count = Convert.ToInt32(dd.Rows[0][0]);


            }

            return count;
        }

        public static void LoggedIPAddressesRemove()
        {

            using (SqlDataAdapter pp = new SqlDataAdapter("delete from RateLimit where CreatedOn < DATEADD(dd, 0, DATEDIFF(dd, 0, GETDATE())); ", con))
            {
                DataTable dd = new DataTable();
                pp.Fill(dd);
            }


        }

        //SqlConnection con = ConnectionBulder.con;
        // public static string VesselID;


        static SqlConnection con = ConnectionBulder.con;
        public static void Below21RopesAtDeleteTime(int VesselID)
        {
            try
            {   // minimum required of 21 Ropes ---------------------------------
                string qry = @"select COUNT(*) as Total,b.MinimumRopes,b.MinimumRopeTails,a.VesselID from MooringRopeDetail a join VesselDetail b on a.VesselID=b.ImoNo  where 
a.OutofServiceDate is null and a.DeleteStatus=0 and a.RopeTail=0 and a.VesselID = " + VesselID + " Group by b.MinimumRopes,b.MinimumRopeTails,a.VesselID";
                SqlDataAdapter adp = new SqlDataAdapter(qry, con);
                DataTable dt = new DataTable();
                adp.Fill(dt);
                if (dt.Rows.Count >= 0)
                {
                    int cnt = Convert.ToInt32(dt.Rows[0]["Total"]);
                    int MinimumRopes = Convert.ToInt32(dt.Rows[0]["MinimumRopes"]);
                    int MinimumRopeTails = Convert.ToInt32(dt.Rows[0]["MinimumRopeTails"]);
                    if (cnt < MinimumRopes)
                    {
                        var notification = "Active lines below minimum required of " + MinimumRopes + " Lines including spare";

                        SqlDataAdapter act = new SqlDataAdapter("select COUNT(*) from tblNotification where VesselID = " + VesselID + " and Notification='Active lines below minimum required of " + MinimumRopes + " Lines including spare'", con);
                        DataTable dd = new DataTable();
                        act.Fill(dd);

                        int cntnoti = Convert.ToInt32(dd.Rows[0][0]);
                        if (cntnoti == 0)
                        {
                            using (MorringOfficeEntities context = new MorringOfficeEntities())
                            {
                                tblNotification noti = new tblNotification();
                                noti.Id = CommonClass.NextNotiId();
                                noti.VesselId = VesselID;
                                noti.Acknowledge = false;
                                noti.AckRecord = "Not yet acknowledged";
                                //noti.AckRecord = "Please insert minimum of {21} active ropes";
                                noti.Notification = notification;
                                noti.RopeId = 0;
                                noti.IsActive = true;
                                // noti.NotificationType = 1;
                                //noti.NotificationDueDate = notidueMonth;
                                noti.CreatedDate = DateTime.Now;
                                noti.CreatedBy = "Admin";
                                noti.NotificationType = (int)NotificationAlertType.Minimum21RopeCount;
                                context.tblNotifications.Add(noti);
                                context.SaveChanges();
                            }
                        }

                        act.Dispose();
                        dd.Dispose();
                    }
                    else
                    {
                        // delete Above Notification
                        int RLs = (int)NotificationAlertType.Minimum21RopeCount;
                        SqlDataAdapter act = new SqlDataAdapter("delete from tblNotification where VesselID = " + VesselID + " and NotificationType = " + RLs + "", con);
                        DataTable dd = new DataTable();
                        act.Fill(dd);
                        act.Dispose();
                        dd.Dispose();
                    }
                }

                dt.Dispose();
                adp.Dispose();


            }
            catch { }
        }

        public static void Below21TailsAtDeleteTime(int VesselID)
        {
            try
            {   // minimum required of 21 Ropes ---------------------------------
                string qry = @"select COUNT(*) as Total,b.MinimumRopes,b.MinimumRopeTails,a.VesselID from MooringRopeDetail a join VesselDetail b on a.VesselID=b.ImoNo  where 
a.OutofServiceDate is null and a.DeleteStatus=0 and a.RopeTail=1 and a.VesselID = " + VesselID + " Group by b.MinimumRopes,b.MinimumRopeTails,a.VesselID";
                SqlDataAdapter adp = new SqlDataAdapter(qry, con);
                DataTable dt = new DataTable();
                adp.Fill(dt);
                if (dt.Rows.Count >= 0)
                {
                    int cnt = Convert.ToInt32(dt.Rows[0]["Total"]);
                    int MinimumRopes = Convert.ToInt32(dt.Rows[0]["MinimumRopes"]);
                    int MinimumRopeTails = Convert.ToInt32(dt.Rows[0]["MinimumRopeTails"]);
                    if (cnt < MinimumRopes)
                    {
                        var notification = "Active rope tail below minimum required of " + MinimumRopeTails + " Rope tail including spare";

                        SqlDataAdapter act = new SqlDataAdapter("select COUNT(*) from tblNotification where VesselID = " + VesselID + " and Notification='Active rope tail below minimum required of " + MinimumRopeTails + " Rope tail including spare'", con);
                        DataTable dd = new DataTable();
                        act.Fill(dd);

                        int cntnoti = Convert.ToInt32(dd.Rows[0][0]);
                        if (cntnoti == 0)
                        {
                            using (MorringOfficeEntities context = new MorringOfficeEntities())
                            {
                                tblNotification noti = new tblNotification();
                                noti.Id = CommonClass.NextNotiId();
                                noti.VesselId = VesselID;
                                noti.Acknowledge = false;
                                noti.AckRecord = "Not yet acknowledged";
                                //noti.AckRecord = "Please insert minimum of {21} active ropes";
                                noti.Notification = notification;
                                noti.RopeId = 0;
                                noti.IsActive = true;
                                // noti.NotificationType = 1;
                                //noti.NotificationDueDate = notidueMonth;
                                noti.CreatedDate = DateTime.Now;
                                noti.CreatedBy = "Admin";
                                noti.NotificationType = (int)NotificationAlertType.Minimum21TailCount;
                                context.tblNotifications.Add(noti);
                                context.SaveChanges();
                            }
                        }

                        act.Dispose();
                        dd.Dispose();
                    }
                    else
                    {
                        // delete Above Notification
                        int RLs = (int)NotificationAlertType.Minimum21TailCount;
                        SqlDataAdapter act = new SqlDataAdapter("delete from tblNotification where VesselID = " + VesselID + " and NotificationType = " + RLs + "", con);
                        DataTable dd = new DataTable();
                        act.Fill(dd);
                        act.Dispose();
                        dd.Dispose();
                    }
                }

                dt.Dispose();
                adp.Dispose();


            }
            catch { }
        }

        public static List<MooringRopeInspections> MooringInspectionList(string year, int ropetail)
        {
            //VesselID = CommonClass.VesselSessionID;
            using (SqlCommand cmd = new SqlCommand("GetRopeInspection", con))
            {
                List<MooringRopeInspections> mropd = new List<MooringRopeInspections>();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Insyear", year);
                cmd.Parameters.AddWithValue("@RopeTail", ropetail);
                cmd.Parameters.AddWithValue("@VesselId", VesselSessionID);
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

        public static List<MooringTailInspections> MooringInspectionList1(string year, int ropetail)
        {
            //VesselID = CommonClass.VesselSessionID;
            using (SqlCommand cmd = new SqlCommand("GetRopeInspection", con))
            {
                List<MooringTailInspections> mropd = new List<MooringTailInspections>();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Insyear", year);
                cmd.Parameters.AddWithValue("@RopeTail", ropetail);
                cmd.Parameters.AddWithValue("@VesselId", VesselSessionID);
                con.Open();
                using (SqlDataReader sdr = cmd.ExecuteReader())
                {
                    while (sdr.Read())
                    {
                        mropd.Add(new MooringTailInspections
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

        public static List<MooringRopeInspection> AddLineInsList(int ropetail)
        {
            //VesselID = CommonClass.VesselSessionID;
            using (SqlCommand cmd = new SqlCommand("RopeInspection", con))
            {
                List<MooringRopeInspection> mropd = new List<MooringRopeInspection>();
                cmd.CommandType = CommandType.StoredProcedure;
                // cmd.Parameters.AddWithValue("@Insyear", year);
                cmd.Parameters.AddWithValue("@RopeTail", ropetail);
                cmd.Parameters.AddWithValue("@VesselId", VesselSessionID);
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
                            LengthOFAbrasion_A = 0,
                            DistanceOutboard_A = 0,
                            CutYarnCount_A = 0,
                            LengthOFGlazing_A = 0,

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

        public static List<MooringRopeInspection> EditLineInsList(int inspectionid)
        {
            //VesselID = CommonClass.VesselSessionID;
            using (SqlCommand cmd = new SqlCommand("EditRopeInspection", con))
            {
                List<MooringRopeInspection> mropd = new List<MooringRopeInspection>();
                cmd.CommandType = CommandType.StoredProcedure;
                // cmd.Parameters.AddWithValue("@Insyear", year);
                cmd.Parameters.AddWithValue("@inspectionid", inspectionid);
                cmd.Parameters.AddWithValue("@VesselId", VesselSessionID);
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

                            ExternalRating_A = Convert.ToInt32((sdr["ExternalRating_A"] == DBNull.Value) ? 1 : sdr["ExternalRating_A"]),
                            InternalRating_A = Convert.ToInt32((sdr["InternalRating_A"] == DBNull.Value) ? 1 : sdr["InternalRating_A"]),
                            AverageRating_A = Convert.ToInt32((sdr["AverageRating_A"] == DBNull.Value) ? 1 : sdr["AverageRating_A"]),
                            LengthOFAbrasion_A = Convert.ToDecimal((sdr["LengthOFAbrasion_A"] == DBNull.Value) ? 0.00 : sdr["LengthOFAbrasion_A"]),
                            DistanceOutboard_A = Convert.ToDecimal((sdr["DistanceOutboard_A"] == DBNull.Value) ? 0.00 : sdr["DistanceOutboard_A"]),
                            CutYarnCount_A = Convert.ToDecimal((sdr["CutYarnCount_A"] == DBNull.Value) ? 0.00 : sdr["CutYarnCount_A"]),
                            LengthOFGlazing_A = Convert.ToDecimal((sdr["LengthOFGlazing_A"] == DBNull.Value) ? 0.00 : sdr["LengthOFGlazing_A"]),

                            ExternalRating_B = Convert.ToInt32((sdr["ExternalRating_B"] == DBNull.Value) ? 1 : sdr["ExternalRating_B"]),
                            InternalRating_B = Convert.ToInt32((sdr["InternalRating_B"] == DBNull.Value) ? 1 : sdr["InternalRating_B"]),
                            AverageRating_B = Convert.ToInt32((sdr["AverageRating_B"] == DBNull.Value) ? 1 : sdr["AverageRating_B"]),
                            LengthOFAbrasion_B = Convert.ToDecimal((sdr["LengthOFAbrasion_B"] == DBNull.Value) ? 0.00 : sdr["LengthOFAbrasion_B"]),
                            DistanceOutboard_B = Convert.ToDecimal((sdr["DistanceOutboard_B"] == DBNull.Value) ? 0.00 : sdr["DistanceOutboard_B"]),
                            CutYarnCount_B = Convert.ToDecimal((sdr["CutYarnCount_B"] == DBNull.Value) ? 0.00 : sdr["CutYarnCount_B"]),
                            LengthOFGlazing_B = Convert.ToDecimal((sdr["LengthOFGlazing_B"] == DBNull.Value) ? 0.00 : sdr["LengthOFGlazing_B"]),
                            Chafe_guard_condition = Convert.ToString((sdr["Chafe_guard_condition"] == DBNull.Value) ? "" : sdr["Chafe_guard_condition"]),
                            Photo11 = Convert.ToString((sdr["Photo11"] == DBNull.Value) ? "" : sdr["Photo11"]),
                            Photo12 = Convert.ToString((sdr["Photo12"] == DBNull.Value) ? "" : sdr["Photo12"]),
                            Image1 = Convert.ToString((sdr["Image1"] == DBNull.Value) ? "" : sdr["Image1"]),
                            Image2 = Convert.ToString((sdr["Image2"] == DBNull.Value) ? "" : sdr["Image2"]),
                            InspectBy = Convert.ToString((sdr["InspectBy"] == DBNull.Value) ? "" : sdr["InspectBy"]),
                            InspectDate = Convert.ToDateTime((sdr["InspectDate"] == DBNull.Value) ? DateTime.Now : sdr["InspectDate"]),

                        });
                    }
                }
                con.Close();
                return mropd;
            }

        }

        public static List<MooringRopeDetail> MooringRopeDisposalListCommon(int ropetail)
        {
            //VesselID = CommonClass.VesselSessionID;
            using (SqlCommand cmd = new SqlCommand("GetAllRope", con))
            {
                List<MooringRopeDetail> mropd = new List<MooringRopeDetail>();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@RopeTail", ropetail);
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


        public static List<MooringRopeDetail> MooringRopeDiscardListCommon(int ropetail)
        {
            //VesselID = CommonClass.VesselSessionID;
            using (SqlCommand cmd = new SqlCommand("GetActiveAssignRopeType1", con))
            {
                List<MooringRopeDetail> mropd = new List<MooringRopeDetail>();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@RopeTail", ropetail);
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
        public static List<MooringRopeDetail> MooringRopeDetailListCommon(int ropetail)
        {
            //VesselID = CommonClass.VesselSessionID;
            using (SqlCommand cmd = new SqlCommand("RopeBinding", con))
            {
                List<MooringRopeDetail> mropd = new List<MooringRopeDetail>();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@RopeTail", ropetail);
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

        public static List<MooringRopeDetail> MooringRopeDetailListResidual(int ropetail)
        {
            // VesselID = CommonClass.VesselSessionID;
            using (SqlCommand cmd = new SqlCommand("GetResidualLine", con))
            {
                List<MooringRopeDetail> mropd = new List<MooringRopeDetail>();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@RopeTail", ropetail);
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


        public static List<LeadListClass> LeadBind()
        {
            //VesselID = CommonClass.VesselSessionID;
            using (SqlCommand cmd = new SqlCommand("GetLeadList", con))
            {
                List<LeadListClass> mropd = new List<LeadListClass>();
                cmd.CommandType = CommandType.StoredProcedure;
                con.Open();
                using (SqlDataReader sdr = cmd.ExecuteReader())
                {
                    while (sdr.Read())
                    {
                        mropd.Add(new LeadListClass
                        {
                            Id = Convert.ToInt32(sdr["Id"]),
                            Lead = sdr["Lead"].ToString()
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
            //VesselID = CommonClass.VesselSessionID;
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

        public static List<CroppingR> CroppingReasonList()
        {
            List<CroppingR> crprsn = new List<CroppingR>();
            //VesselID = CommonClass.VesselSessionID;
            using (SqlDataAdapter cmd = new SqlDataAdapter("select * from CroppingReasons", con))
            {
                DataTable dt = new DataTable();
                cmd.Fill(dt);
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    crprsn.Add(new CroppingR
                    {
                        Id = Convert.ToInt32(dt.Rows[i]["Id"]),
                        CroppingReason = dt.Rows[i]["Reasons"].ToString()
                    });
                }
                con.Close();
            }
            return crprsn;
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
            //VesselID = CommonClass.VesselSessionID;
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
            // VesselID = CommonClass.VesselSessionID;
            using (SqlCommand cmd = new SqlCommand("select a.id,a.UniqueID + ' - ' +  a.certificatenumber as certificatenumber, b.ropeid from MooringRopeDetail a inner join AssignRopeToWinch b  on a.RopeId=b.ropeid  where a.DeleteStatus=0 and a.VesselId='" + VesselSessionID + "' and b.VesselId='" + VesselSessionID + "' and  OutofServiceDate is null and b.RopeTail=0 and  b.IsActive=1 order by a.UniqueID asc", con))
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
        public static List<MooringRopeDetail> MooringRopeDetailList(int ropetail)
        {
            // VesselID = CommonClass.VesselSessionID;
            using (SqlCommand cmd = new SqlCommand("select Id,UniqueId +' - ' + CertificateNumber as CertificateNumber from MooringRopeDetail where ropetail=" + ropetail + " and OutofServiceDate is null and DeleteStatus=0 and VesselId='" + VesselSessionID + "' and InstalledDate is not null order by UniqueID asc", con))
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
                // SqlDataAdapter adp = new SqlDataAdapter("SELECT IDENT_CURRENT('Notifications') + IDENT_INCR('Notifications') as Notiid", con);
                SqlDataAdapter adp = new SqlDataAdapter("select MAX(Id) + 1 as Notiid    from  tblNotification where VesselID=" + VesselSessionID + "", con);
                DataTable dt = new DataTable();
                adp.Fill(dt);
                notiid = Convert.ToInt32(dt.Rows[0][0]);

                return notiid;

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


        //--Kuldip Methods below

        public static void InspectNotification(int RopeID, string NotiMsg, int NotiAlertType)
        {
            int VesselID = Convert.ToInt32(VesselSessionID);
            using (MorringOfficeEntities context = new MorringOfficeEntities())
            {
                var result = context.tblNotifications.Where(x => x.VesselId == VesselID && x.RopeId == RopeID & x.NotificationAlertType == NotiAlertType).FirstOrDefault();
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
        public static void WinchrotationSetting_and_Notifications()
        {
            try
            {


                string qry = @"select distinct a.WinchId,b.AssignedNumber,b.Location, b.lead,a.AssignedDate,R.RopeId as RopeID,R.CurrentLeadRunningHours,R.ManufacturerId,R.RopeTypeId,R.UniqueID,R.CertificateNumber,
	T.RopeType,M.Name from AssignRopeToWinch a inner join MooringWinchDetail b on a.WinchId=b.Id and a.VesselID=b.VesselID
and a.IsActive=1 and a.RopeTail=0 join MooringRopeDetail R on R.RopeId=a.RopeId and R.VesselID=a.VesselID
and R.RopeTail=0 and R.DeleteStatus=0  and R.OutofServiceDate is null
join tblCommon M on M.Id=R.ManufacturerId join MooringRopeType T on T.Id=R.RopeTypeId
where a.VesselID = " + VesselSessionID + "";   // and R.RopeId = " + Ropeid + "
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

                            string Lead = tbls.Rows[i]["lead"].ToString();
                            //Lead = Lead.Replace(System.Environment.NewLine, "").Trim();
                            DateTime AssignedDate = Convert.ToDateTime(tbls.Rows[i]["AssignedDate"]);
                            string UniqueID = tbls.Rows[i]["UniqueID"].ToString();
                            string CertificateNum = tbls.Rows[i]["CertificateNumber"].ToString();
                            string WinchAssignedNumber = tbls.Rows[i]["AssignedNumber"].ToString();

                            using (SqlDataAdapter pp1 = new SqlDataAdapter("select * from tblWinchRotationSetting where VesselID = " + VesselSessionID + " and mooringropetype = " + RopeTypeId + " and ManufacturerType= " + ManufacturerId + " and LeadFrom='" + Lead + "'", con))
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

                                        var winchrotation = "Winch Rotation is Approaching for Line " + CertificateNum + " - " + UniqueID + " currently on Winch " + WinchAssignedNumber + " lead  " + Lead + " , Current " + WinchMonthdiff + " month / Rotation was due at " + maxmnthallowed + " month,  Please shift from " + Lead + " to " + leadto + "";

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
                        }
                    }
                }

            }
            catch (Exception ex)
            {

                // sc.ErrorLog(ex);
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
                    jst.Add(new SelectListItem());
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
            using (SqlDataAdapter sda = new SqlDataAdapter("select distinct " + ColmunName + " from BerthOptionList", con))
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
            using (SqlDataAdapter sda = new SqlDataAdapter("select distinct FacilityName from PortList where PortName='" + PortName + "'", con))
            {
                DataTable tbl = new DataTable();

                //if(con.State == ConnectionState.Closed)
                //{
                //    con.Open();
                //}
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

        public static List<SelectListItem> GetRopeUsedinOperation(int OperationId, int VesselID)
        {
            List<SelectListItem> jst = new List<SelectListItem>();
            using (SqlDataAdapter sda = new SqlDataAdapter("RopeinMopDamage", con))
            {
                sda.SelectCommand.CommandType = CommandType.StoredProcedure;
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

        public static void SaveNotification(int VesselID, int RopeID, string NotiMsg, int NotiAlertType)
        {
            using (MorringOfficeEntities context = new MorringOfficeEntities())
            {
                var result = context.tblNotifications.Where(x => x.VesselId == VesselID & x.RopeId == RopeID & x.NotificationType == NotiAlertType).FirstOrDefault();
                if (result == null)
                {
                    int IdPK = ((from asn in context.tblNotifications.Where(x => x.VesselId == VesselID) select (int?)asn.Id).Max() ?? 0) + 1;
                    tblNotification noti = new tblNotification();
                    noti.Id = IdPK;
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

        public static string NotificationCounter()
        {
            string counter = "";
            string query = @"select count(*) as totalNotA from tblNotification where (AckRecord = 'Not yet acknowledged' 
or AckRecord = 'This notification cannot be acknowledged, kindly discard it' or
NotificationAlertType = 17 or NotificationAlertType = 18) and isactive = 1 and VesselID = @VesselId";
            using (SqlDataAdapter sda = new SqlDataAdapter(query, con))
            {
                sda.SelectCommand.Parameters.AddWithValue("@VesselId", VesselSessionID);
                DataTable tbl = new DataTable();

                sda.Fill(tbl);
                if (tbl.Rows.Count > 0)
                {
                    counter = tbl.Rows[0][0].ToString();
                }
            }

            return counter;
        }

        public void InsertTrAtt(int id, string attachmentname, string attachmentpath)
        {
            try
            {
                SqlDataAdapter adp = new SqlDataAdapter("InsertTrainginAttachment", con);
                adp.SelectCommand.CommandType = CommandType.StoredProcedure;
                adp.SelectCommand.Parameters.AddWithValue("@Id", id);
                adp.SelectCommand.Parameters.AddWithValue("@AttachmentName", attachmentname);
                adp.SelectCommand.Parameters.AddWithValue("@AttachmentPath", attachmentpath);
                adp.SelectCommand.Parameters.AddWithValue("@VesselID", VesselSessionID);
                DataTable dt = new DataTable();
                adp.Fill(dt);
            }
            catch { }
        }

        public void InsertAtuoLogout(string email, string myip)
        {
            try
            {
                SqlDataAdapter adp = new SqlDataAdapter("InsertAutoLogout", con);
                adp.SelectCommand.CommandType = CommandType.StoredProcedure;

                adp.SelectCommand.Parameters.AddWithValue("@Email", email);
                adp.SelectCommand.Parameters.AddWithValue("@IP_Address", myip);

                DataTable dt = new DataTable();
                adp.Fill(dt);
            }
            catch { }
        }

        public void DeleteTrAtt(int id)
        {
            try
            {
                SqlDataAdapter adp = new SqlDataAdapter("DeleteTrngAtt", con);
                adp.SelectCommand.CommandType = CommandType.StoredProcedure;
                adp.SelectCommand.Parameters.AddWithValue("@Id", id);
                adp.SelectCommand.Parameters.AddWithValue("@VesselId", VesselSessionID);
                DataTable dt = new DataTable();
                adp.Fill(dt);
            }
            catch { }
        }

        public void CheckEmailAuto(string email)
        {
            try
            {
                SqlDataAdapter adp = new SqlDataAdapter("EmailcheckAuto", con);
                adp.SelectCommand.CommandType = CommandType.StoredProcedure;
                adp.SelectCommand.Parameters.AddWithValue("@Email", email);

                DataTable dt = new DataTable();
                adp.Fill(dt);
            }
            catch { }
        }

        public int MaxID(int maxid)
        {

            try
            {
                SqlDataAdapter adp = new SqlDataAdapter("MaxIdGet", con);
                adp.SelectCommand.CommandType = CommandType.StoredProcedure;

                adp.SelectCommand.Parameters.AddWithValue("@VesselId", VesselSessionID);
                DataTable dt = new DataTable();
                adp.Fill(dt);

                maxid = Convert.ToInt32(dt.Rows[0][0]);
            }
            catch { }

            return maxid;
        }


        public void lead_check(int? ropeid, DateTime? assigneddate, int? winchid, int vesselid)
        {
            MorringOfficeEntities context = new MorringOfficeEntities();
            try
            {
                if (winchid == 0)
                {
                    using (SqlDataAdapter adp = new SqlDataAdapter("select * from MOUsedWinchTbl where ropeid=" + ropeid + " and VesselID = '" + vesselid + "'", con))
                    {
                        DataTable dt = new DataTable();
                        adp.Fill(dt);


                        if (dt.Rows.Count > 0)
                        {

                            //var leedd = sc.MooringWinch.Where(x => x.Id == winchid).Select(x => x.Lead).SingleOrDefault();

                            SqlDataAdapter pp1 = new SqlDataAdapter("select RopeId,SUM(runninghours) as RunningHours, Lead  from MOUsedWinchTbl where RopeId=" + ropeid + " and VesselID = '" + vesselid + "' group by ropeid,runninghours, Lead", con);

                            //SqlDataAdapter pp1 = new SqlDataAdapter("select RopeId,SUM(runninghours) as RunningHours, Lead  from MOUsedWinchTbl where RopeId=" + ropeid + " and lead ='" + leedd + "' group by ropeid, Lead", sc.con);
                            DataTable dd1 = new DataTable();
                            pp1.Fill(dd1);

                            for (int i = 0; i < dd1.Rows.Count; i++)
                            //if (dd1.Rows.Count > 0)
                            {
                                int rpid = Convert.ToInt32(dd1.Rows[i]["RopeId"]);
                                string lead = dd1.Rows[i]["Lead"].ToString();
                                decimal rnghrs = Convert.ToDecimal(dd1.Rows[i]["RunningHours"]);

                                decimal rnghrs1 = 0;

                                SqlDataAdapter pp2 = new SqlDataAdapter("select * from WinchRotation where RopeId=" + rpid + " and Lead='" + lead + "' and VesselID = '" + vesselid + "'", con);
                                DataTable dd2 = new DataTable();
                                pp2.Fill(dd2);
                                if (dd2.Rows.Count > 0)
                                {
                                    rnghrs1 = Convert.ToDecimal(dd2.Rows[0]["RunningHours"]);

                                    if (rnghrs == rnghrs1)
                                    {
                                        rnghrs = 0;
                                    }
                                    else
                                    {
                                        if (rnghrs1 > rnghrs)
                                        {
                                            rnghrs = rnghrs1 - rnghrs;
                                        }
                                        if (rnghrs1 < rnghrs)
                                        {
                                            rnghrs = rnghrs - rnghrs1;
                                        }

                                    }
                                }


                                if (rnghrs != 0)
                                {

                                    SqlDataAdapter pp = new SqlDataAdapter("INSERT INTO WinchRotation (RopeId, AssignedDate, WinchId,Lead,RunningHours,IsActive,VesselID)VALUES(" + ropeid + ", '" + assigneddate + "', " + winchid + ",'" + lead + "','" + rnghrs + "','" + true + "','" + vesselid + "' ); ", con);
                                    DataTable dd = new DataTable();
                                    pp.Fill(dd);


                                    try
                                    {
                                        SqlDataAdapter adpt = new SqlDataAdapter("update MooringRopeDetail set CurrentLeadRunningHours = 0 where ID=" + ropeid + " and VesselID = '" + vesselid + "'", con);
                                        DataTable ddt = new DataTable();
                                        adpt.Fill(ddt);
                                    }
                                    catch { }
                                }
                                //if (rnghrs == 0)
                                //{

                                //    SqlDataAdapter pp = new SqlDataAdapter("INSERT INTO WinchRotation (RopeId, AssignedDate, WinchId,Lead,RunningHours,IsActive)VALUES(" + ropeid + ", '" + assigneddate + "', " + winchid + ",'" + lead + "','" + rnghrs + "','" + true + "' ); ", sc.con);
                                //    DataTable dd = new DataTable();
                                //    pp.Fill(dd);
                                //}
                            }


                        }
                        //else
                        //{
                        //    try
                        //    {
                        //        var leadcheck = sc.MooringWinch.Where(x => x.Id == winchid && x.IsActive == true).Select(x => x.Lead).SingleOrDefault();
                        //        decimal rnghrs = 0;
                        //        SqlDataAdapter pp = new SqlDataAdapter("INSERT INTO WinchRotation (RopeId, AssignedDate, WinchId,Lead,RunningHours,IsActive)VALUES(" + ropeid + ", '" + assigneddate + "', " + winchid + ",'" + leadcheck + "','" + rnghrs + "','" + true + "' ); ", sc.con);
                        //        DataTable dd = new DataTable();
                        //        pp.Fill(dd);
                        //    }
                        //    catch { }
                        //}
                    }
                }
                else
                {

                    using (SqlDataAdapter adp = new SqlDataAdapter("select * from MOUsedWinchTbl where ropeid=" + ropeid + " and VesselID = '" + vesselid + "'", con))
                    {
                        DataTable dt = new DataTable();
                        adp.Fill(dt);


                        if (dt.Rows.Count > 0)
                        {

                            var leedd = context.MooringWinchDetails.Where(x => x.Id == winchid && x.VesselID == vesselid).Select(x => x.Lead).SingleOrDefault();
                            leedd = leedd.Replace(Environment.NewLine, "").Trim();
                            //SqlDataAdapter pp1 = new SqlDataAdapter("select RopeId,SUM(runninghours) as RunningHours, Lead  from MOUsedWinchTbl where RopeId=" + ropeid + " group by ropeid,runninghours, Lead", sc.con);

                            SqlDataAdapter pp1 = new SqlDataAdapter("select RopeId,SUM(runninghours) as RunningHours, Lead  from MOUsedWinchTbl where RopeId=" + ropeid + " and lead ='" + leedd + "' and VesselID = '" + vesselid + "' group by ropeid, Lead", con);
                            DataTable dd1 = new DataTable();
                            pp1.Fill(dd1);

                            //for (int i = 0; i < dd1.Rows.Count; i++)
                            if (dd1.Rows.Count > 0)
                            {
                                int rpid = Convert.ToInt32(dd1.Rows[0]["RopeId"]);
                                string lead = dd1.Rows[0]["Lead"].ToString();
                                decimal rnghrs = Convert.ToDecimal(dd1.Rows[0]["RunningHours"]);

                                decimal rnghrs1 = 0;

                                SqlDataAdapter pp2 = new SqlDataAdapter("select * from WinchRotation where RopeId=" + rpid + " and Lead='" + lead + "' and VesselID = '" + vesselid + "'", con);
                                DataTable dd2 = new DataTable();
                                pp2.Fill(dd2);
                                if (dd2.Rows.Count > 0)
                                {
                                    rnghrs1 = Convert.ToDecimal(dd2.Rows[0]["RunningHours"]);

                                    if (rnghrs == rnghrs1)
                                    {
                                        rnghrs = 0;
                                    }
                                    else
                                    {
                                        if (rnghrs1 > rnghrs)
                                        {
                                            rnghrs = rnghrs1 - rnghrs;
                                        }
                                        if (rnghrs1 < rnghrs)
                                        {
                                            rnghrs = rnghrs - rnghrs1;
                                        }

                                    }
                                }


                                if (rnghrs != 0)
                                {

                                    SqlDataAdapter pp = new SqlDataAdapter("INSERT INTO WinchRotation (RopeId, AssignedDate, WinchId,Lead,RunningHours,IsActive,VesselID)VALUES(" + ropeid + ", '" + assigneddate + "', " + winchid + ",'" + lead + "','" + rnghrs + "','" + true + "','" + vesselid + "' ); ", con);
                                    DataTable dd = new DataTable();
                                    pp.Fill(dd);


                                    try
                                    {
                                        SqlDataAdapter adpt = new SqlDataAdapter("update MooringRopeDetail set CurrentLeadRunningHours = 0 where ID=" + ropeid + " and VesselID = '" + vesselid + "'", con);
                                        DataTable ddt = new DataTable();
                                        adpt.Fill(ddt);
                                    }
                                    catch { }
                                }
                                //if (rnghrs == 0)
                                //{

                                //    SqlDataAdapter pp = new SqlDataAdapter("INSERT INTO WinchRotation (RopeId, AssignedDate, WinchId,Lead,RunningHours,IsActive)VALUES(" + ropeid + ", '" + assigneddate + "', " + winchid + ",'" + lead + "','" + rnghrs + "','" + true + "' ); ", sc.con);
                                //    DataTable dd = new DataTable();
                                //    pp.Fill(dd);
                                //}
                            }


                        }
                        //else
                        //{
                        //    try
                        //    {
                        //        var leadcheck = sc.MooringWinch.Where(x => x.Id == winchid && x.IsActive == true).Select(x => x.Lead).SingleOrDefault();
                        //        decimal rnghrs = 0;
                        //        SqlDataAdapter pp = new SqlDataAdapter("INSERT INTO WinchRotation (RopeId, AssignedDate, WinchId,Lead,RunningHours,IsActive)VALUES(" + ropeid + ", '" + assigneddate + "', " + winchid + ",'" + leadcheck + "','" + rnghrs + "','" + true + "' ); ", sc.con);
                        //        DataTable dd = new DataTable();
                        //        pp.Fill(dd);
                        //    }
                        //    catch { }
                        //}
                    }


                }
            }
            catch (Exception ex) { }
        }

        public DateTime? updateinspecionduedate(DateTime? installdate, int? ropetypeid, int? manufacid)
        {
            SqlConnection con = ConnectionBulder.con;
            try
            {
                SqlDataAdapter adp = new SqlDataAdapter("select * from tblRopeInspectionSetting where MooringRopeType=" + ropetypeid + " and ManufacturerType=" + manufacid + "", con);
                DataTable dt = new DataTable();
                adp.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    //decimal rating1 = Convert.ToDecimal(dt.Rows[0]["Rating1"]);
                    //int rat = Convert.ToInt32(rating1);

                    decimal datecheck = Convert.ToDecimal(dt.Rows[0]["Rating1"]);
                    decimal perchk = Convert.ToDecimal(dt.Rows[0]["Rating1"]) * 30 / 100;
                    perchk = perchk * 100;
                    int near = Convert.ToInt32(perchk);
                    //DateTime inspectduedate = Convert.ToDateTime(moorwinchrope.InstalledDate).AddDays(near);


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
                    DateTime date = Convert.ToDateTime(installdate);
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
                    TimeSpan t = nextMonth - date;
                    double NrOfDays = t.TotalDays;

                    DateTime inspectduedate = Convert.ToDateTime(installdate).AddDays(NrOfDays);



                    DateTime crntdt = DateTime.Now;
                    if (inspectduedate <= crntdt)
                    {
                        inspectduedate = DateTime.Now;
                    }

                    installdate = inspectduedate;
                }
                else
                {
                    installdate = null;
                }

            }
            catch { }
            return installdate;
        }
    }
}

