using Reports;
using System.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;
using MenuLayer;
using Newtonsoft.Json;
using System.Reflection;
using System.ComponentModel;
using Shipment49Web.Areas.ResidualLabTests.Models;

namespace Shipment49Web.Common
{
    public static class CommonMethods
    {
        static CommonMethods()
        {
            InspectionList_Details = GetInspectionInformation();
        }

        public static List<InspectionList> InspectionList_Details { get; set; }
        public static List<SpWinchRotationUpcomingNDue_Result> AllWinchRotationList { get; set; }
        public static string VesselName { get; set; }
        public static int PAGESIZE
        {
            get
            {
                string pageSize = System.Configuration.ConfigurationManager.AppSettings["PageSize"];

                if (string.IsNullOrEmpty(pageSize))
                    return 10;
                else
                    return Convert.ToInt32(pageSize);
            }
        }

        public static List<ResidualLabTestClass> ResdualData(int ropeid,int ropetail,int vesselid)
        {
            //connection.ConnectionString = Convert.ToString(ConfigurationManager.ConnectionStrings["ShipmentContaxt"]);

            using (SqlConnection con = new SqlConnection())
            {
                con.ConnectionString = Convert.ToString(ConfigurationManager.ConnectionStrings["ShipmentContaxt"]);

                using (SqlCommand cmd = new SqlCommand("select a.*,(select c.AttachmentPath  from MooringRopeAttachment c where a.Id=c.ResidualID and a.VesselID=c.VesselId) as attachment from residuallabtest a  where a.ropeid=" + ropeid + " and a.ropetail=" + ropetail + "  and a.VesselID=" + vesselid + " and a.IsActive=1", con))
                {
                    List<ResidualLabTestClass> reslist = new List<ResidualLabTestClass>();
                    cmd.CommandType = CommandType.Text;
                    con.Open();
                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            DateTime ss = Convert.ToDateTime(sdr["LabTestDate"]);
                            reslist.Add(new ResidualLabTestClass
                            {
                                
                                LabTestDate = ss.ToString("yyyy-MM-dd"),
                                RunningHours = Convert.ToDecimal(sdr["RunningHours"]),
                                TestResults = Convert.ToDecimal(sdr["TestResults"]),
                                attachment = Convert.ToString(sdr["attachment"]),

                            });
                        }
                    }
                    con.Close();
                    return reslist;
                }
            }
        }

        public static string saveImage(string SavePath, int i)
        {
            var req = new RequiredMethods();
            //String mainPath = req.RemoveSpecialChars(softwareName) + MessageDisplay.underScore + Guid.NewGuid().ToString() + Path.GetFileName(Request.Files[i].FileName);
            string mainPath = Guid.NewGuid().ToString() + Path.GetFileName(HttpContext.Current.Request.Files[i].FileName);

            mainPath = mainPath.Replace(" ", string.Empty);
            string path = Path.Combine(System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath + SavePath + mainPath);

            HttpContext.Current.Request.Files[i].SaveAs(path);
            return mainPath;
        }

        public static List<AnomaliesChartData> GetAnomaliesChartList(List<View_VesselWiseMooringOP_Detail2> records)
        {
            List<AnomaliesChartData> listCombinedAnomalies = new List<AnomaliesChartData>()
            {
               new AnomaliesChartData(){
                        label = 1,
                        value1 =  records.Where(x=>x.RangOfTide>3.0m).Count(),
                        value2 = records.Where(x=>x.RangOfTide>3.0m).GroupBy(p => new {p.FacilityName, p.PortName }).Select(u => new View_VesselWiseMooringOP_Detail2 { FacilityName = u.Key.FacilityName, PortName = u.Key.PortName }).Count(),

                      },
               new AnomaliesChartData(){
                        label = 2,
                        value1 =  records.Where(x=>x.CurrentSpeed> 0.5m).Count(),
                        value2 = records.Where(x=>x.CurrentSpeed> 0.5m).GroupBy(p => new {p.FacilityName, p.PortName }).Select(u => new View_VesselWiseMooringOP_Detail2 { FacilityName = u.Key.FacilityName, PortName = u.Key.PortName }).Count(),
                      },
                new AnomaliesChartData(){
                        label = 3,
                        value1 =  records.Where(x=>x.WindSpeed>20).Count(),
                        value2 = records.Where(x=>x.WindSpeed>20).GroupBy(p => new {p.FacilityName, p.PortName }).Select(u => new View_VesselWiseMooringOP_Detail2 { FacilityName = u.Key.FacilityName, PortName = u.Key.PortName }).Count(),
                      },
                 new AnomaliesChartData(){
                        label = 4,
                        value1 =  records.Where(x=>x.AirTemprature<10).Count(),
                        value2 = records.Where(x=>x.AirTemprature<10).GroupBy(p => new {p.FacilityName, p.PortName }).Select(u => new View_VesselWiseMooringOP_Detail2 { FacilityName = u.Key.FacilityName, PortName = u.Key.PortName }).Count(),
                      },
                  new AnomaliesChartData(){
                        label = 5,
                        value1 =  records.Where(x=>x.AnySquall=="Yes").Count(),
                        value2 = records.Where(x=>x.AnySquall=="Yes").GroupBy(p => new {p.FacilityName, p.PortName }).Select(u => new View_VesselWiseMooringOP_Detail2 { FacilityName = u.Key.FacilityName, PortName = u.Key.PortName }).Count(),
                      },
                   new AnomaliesChartData(){
                        label = 6,
                        value1 =  records.Where(x=>x.SurgingObserved=="Yes").Count(),
                        value2 = records.Where(x=>x.SurgingObserved=="Yes").GroupBy(p => new {p.FacilityName, p.PortName }).Select(u => new View_VesselWiseMooringOP_Detail2 { FacilityName = u.Key.FacilityName, PortName = u.Key.PortName }).Count(),
                      },
                    new AnomaliesChartData(){
                        label = 7,
                         value1 =  records.Where(x=>x.Berth_exposed_SeaSwell=="Yes").Count(),
                        value2 = records.Where(x=>x.Berth_exposed_SeaSwell=="Yes").GroupBy(p => new {p.FacilityName, p.PortName }).Select(u => new View_VesselWiseMooringOP_Detail2 { FacilityName = u.Key.FacilityName, PortName = u.Key.PortName }).Count(),
                      },
                    new AnomaliesChartData(){
                        label = 8,
                         value1 =  records.Where(x=>x.Any_Affect_Passing_Traffic=="Yes").Count(),
                        value2 = records.Where(x=>x.Any_Affect_Passing_Traffic=="Yes").GroupBy(p => new {p.FacilityName, p.PortName }).Select(u => new View_VesselWiseMooringOP_Detail2 { FacilityName = u.Key.FacilityName, PortName = u.Key.PortName }).Count(),
                      },
                    new AnomaliesChartData(){
                        label = 9,
                        value1 =  records.Where(x=>x.Ship_was_continuously_contact_with_fender=="No").Count(),
                        value2 = records.Where(x=>x.Ship_was_continuously_contact_with_fender=="No").GroupBy(p => new {p.FacilityName, p.PortName }).Select(u => new View_VesselWiseMooringOP_Detail2 { FacilityName = u.Key.FacilityName, PortName = u.Key.PortName }).Count(),
                      },
                    new AnomaliesChartData(){
                        label = 10,
                       value1 =  records.Where(x=>x.Any_Rope_Damaged=="Yes").Count(),
                        value2 = records.Where(x=>x.Any_Rope_Damaged=="Yes").GroupBy(p => new {p.FacilityName, p.PortName }).Select(u => new View_VesselWiseMooringOP_Detail2 { FacilityName = u.Key.FacilityName, PortName = u.Key.PortName }).Count(),
                      },
            };

            return listCombinedAnomalies;

        }

        public class RequiredMethods
        {
            #region Remove Special Characters
            public string RemoveSpecialChars(string input)
            {
                var final = Regex.Replace(input, @"[^0-9a-zA-Z\._]", @" ");
                final = Regex.Replace(final.Trim(), @"\s+", "-");
                return final;
            }
            #endregion
        }

        public static DataTable GetWinchRotationSetting(int ManufacturerId, int RopeTypeid, int VesselId, string LeadFrom)
        {
            using (SqlConnection connection = new SqlConnection())
            {
                connection.ConnectionString = Convert.ToString(ConfigurationManager.ConnectionStrings["ShipmentContaxt"]);

                SqlCommand command = new SqlCommand("select * from tblWinchRotationSetting where VesselID=@VesselID and mooringropetype = @RopeTypes and ManufacturerType = @Manufacturers and LeadFrom=@LeadFrom", connection)
                {
                    CommandType = CommandType.Text
                };

                command.Parameters.AddWithValue("@VesselID", VesselId);
                command.Parameters.AddWithValue("@Manufacturers", ManufacturerId);
                command.Parameters.AddWithValue("@RopeTypes", RopeTypeid);
                command.Parameters.AddWithValue("@LeadFrom", LeadFrom);

                SqlDataAdapter adapter = new SqlDataAdapter(command);

                DataTable dataTable = new DataTable();

                adapter.Fill(dataTable);

                return dataTable;
            }
        }


        public static DataTable GetRopeAnalysis(string pVessels, string pManufacturers, string pRopeTypes, long pRunningHoursFrom, long pRunningHoursTo, DateTime pDateFrom, DateTime pDateUpto)
        {
            using (SqlConnection connection = new SqlConnection())
            {
                connection.ConnectionString = Convert.ToString(ConfigurationManager.ConnectionStrings["ShipmentContaxt"]);

                SqlCommand command = new SqlCommand("spRopeAnalysis", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                command.Parameters.AddWithValue("@Vessels", pVessels);
                command.Parameters.AddWithValue("@Manufacturers", pManufacturers);
                command.Parameters.AddWithValue("@RopeTypes", pRopeTypes);
                command.Parameters.AddWithValue("@RunningHoursFrom", pRunningHoursFrom);
                command.Parameters.AddWithValue("@RunningHoursTo", pRunningHoursTo);
                command.Parameters.AddWithValue("@DateFrom", pDateFrom);
                command.Parameters.AddWithValue("@DateUpto", pDateUpto);

                SqlDataAdapter adapter = new SqlDataAdapter(command);

                DataTable dataTable = new DataTable();

                adapter.Fill(dataTable);

                return dataTable;
            }
        }

        public static DataSet GetAbrasionDetails(List<int> pVessels, List<int> pManufacturers, List<int> pRopeTypes, List<int> pInspectionRating, long pRunningHoursFrom, long pRunningHoursTo)
        {
            using (SqlConnection connection = new SqlConnection())
            {
                connection.ConnectionString = Convert.ToString(ConfigurationManager.ConnectionStrings["ShipmentContaxt"]);

                SqlCommand command = new SqlCommand("spAbrasionDetails", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                string vessels = string.Empty;
                foreach (int vid in pVessels)
                    vessels = vid + "," + vessels;

                string manufacturers = string.Empty;
                foreach (int mid in pManufacturers)
                    manufacturers = mid + "," + manufacturers;

                string ropetypes = string.Empty;
                foreach (int rid in pRopeTypes)
                    ropetypes = rid + "," + ropetypes;

                string ratings = string.Empty;
                foreach (int ratingid in pInspectionRating)
                    ratings = ratingid + "," + ratings;

                command.Parameters.AddWithValue("@Vessels", vessels.Trim(','));
                command.Parameters.AddWithValue("@Manufacturers", manufacturers.Trim(','));
                command.Parameters.AddWithValue("@RopeTypes", ropetypes.Trim(','));
                command.Parameters.AddWithValue("@InspectionRating", ratings.Trim(','));
                command.Parameters.AddWithValue("@RunningHoursFrom", pRunningHoursFrom);
                command.Parameters.AddWithValue("@RunningHoursTo", pRunningHoursTo);

                SqlDataAdapter adapter = new SqlDataAdapter(command);

                DataSet dataTables = new DataSet();

                adapter.Fill(dataTables);

                return dataTables;
            }
        }

        public static List<View_FleetWiseIncidents2> ExecStoredPro_FleetWiseIncidents(string proceedureName, string Status, DateTime FromDate, DateTime Todate)
        {
            List<View_FleetWiseIncidents2> list = new List<View_FleetWiseIncidents2>();
            using (SqlConnection connection = new SqlConnection())
            {
                connection.ConnectionString = Convert.ToString(ConfigurationManager.ConnectionStrings["ShipmentContaxt"]);

                SqlCommand command = new SqlCommand(proceedureName, connection)
                {
                    CommandType = CommandType.StoredProcedure

                };

                SqlDataAdapter adapter = new SqlDataAdapter(command);
                if (Status == null)
                {
                    adapter.SelectCommand.Parameters.AddWithValue("@Status", DBNull.Value);
                }
                else
                {
                    adapter.SelectCommand.Parameters.AddWithValue("@Status", Status);
                }
                adapter.SelectCommand.Parameters.AddWithValue("@Datefrom", FromDate.ToString("yyyy-MM-dd"));
                adapter.SelectCommand.Parameters.AddWithValue("@DateTo", Todate.ToString("yyyy-MM-dd"));
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);
                if (dataTable.Rows.Count > 0)
                {
                    for (int i = 0; i < dataTable.Rows.Count; i++)
                    {
                        var data = new View_FleetWiseIncidents2()
                        {
                            VesselID = Convert.ToInt32(dataTable.Rows[i]["VesselID"]),
                            VesselName = dataTable.Rows[i]["VesselName"].ToString(),
                            ManufacturerId = Convert.ToInt32(dataTable.Rows[i]["ManufacturerId"]),
                            Manufacturer = dataTable.Rows[i]["Manufacturer"].ToString(),
                            RopeTypeId = Convert.ToInt32(dataTable.Rows[i]["RopeTypeId"]),
                            RopeType = dataTable.Rows[i]["RopeType"].ToString(),
                            Damaged = Convert.ToInt32(dataTable.Rows[i]["Damaged"]),
                        };

                        list.Add(data);
                    }
                }


                return list;
            }
        }

        public static DataTable ExecStoredProceedure(string proceedureName)
        {
            using (SqlConnection connection = new SqlConnection())
            {
                connection.ConnectionString = Convert.ToString(ConfigurationManager.ConnectionStrings["ShipmentContaxt"]);

                SqlCommand command = new SqlCommand(proceedureName, connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                SqlDataAdapter adapter = new SqlDataAdapter(command);
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);
                return dataTable;
            }
        }

        public static DataTable ExecStoredProceedureBetweenDates(string proceedureName, DateTime FromDate, DateTime Todate)
        {
            using (SqlConnection connection = new SqlConnection())
            {
                connection.ConnectionString = Convert.ToString(ConfigurationManager.ConnectionStrings["ShipmentContaxt"]);

                SqlCommand command = new SqlCommand(proceedureName, connection)
                {
                    CommandType = CommandType.StoredProcedure

                };

                SqlDataAdapter adapter = new SqlDataAdapter(command);
                adapter.SelectCommand.Parameters.AddWithValue("@FastDateFrom", FromDate.ToString("yyyy-MM-dd"));
                adapter.SelectCommand.Parameters.AddWithValue("@FastDateTo", Todate.ToString("yyyy-MM-dd"));
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);
                return dataTable;
            }
        }

        //public static DataTable ExecStoredProceedureWithVessel(string proceedureName, int vesselID)
        //{
        //    using (SqlConnection connection = new SqlConnection())
        //    {
        //        connection.ConnectionString = Convert.ToString(ConfigurationManager.ConnectionStrings["ShipmentContaxt"]);

        //        SqlCommand command = new SqlCommand(proceedureName, connection)
        //        {
        //            CommandType = CommandType.StoredProcedure

        //        };

        //        SqlDataAdapter adapter = new SqlDataAdapter(command);
        //        adapter.SelectCommand.Parameters.AddWithValue("@vesselID", vesselID);
        //        DataTable dataTable = new DataTable();
        //        adapter.Fill(dataTable);
        //        return dataTable;
        //    }
        //}

        public static DataSet ExecStoredProceedureWithDataSet(string proceedureName)//, DateTime FromDate, DateTime Todate)
        {
            using (SqlConnection connection = new SqlConnection())
            {
                connection.ConnectionString = Convert.ToString(ConfigurationManager.ConnectionStrings["ShipmentContaxt"]);

                SqlCommand command = new SqlCommand(proceedureName, connection)
                {
                    CommandType = CommandType.StoredProcedure

                };

                SqlDataAdapter adapter = new SqlDataAdapter(command);
                //adapter.SelectCommand.Parameters.AddWithValue("@sdf", FromDate);
                //adapter.SelectCommand.Parameters.AddWithValue("@sdf", FromDate);
                DataSet dataset = new DataSet();
                adapter.Fill(dataset);
                return dataset;
            }
        }


        public static DataSet ExecStoredProceedureWithDataSet(string proceedureName, int VesselID)//, DateTime FromDate, DateTime Todate)
        {
            using (SqlConnection connection = new SqlConnection())
            {
                connection.ConnectionString = Convert.ToString(ConfigurationManager.ConnectionStrings["ShipmentContaxt"]);

                SqlCommand command = new SqlCommand(proceedureName, connection)
                {
                    CommandType = CommandType.StoredProcedure

                };

                SqlDataAdapter adapter = new SqlDataAdapter(command);
                adapter.SelectCommand.Parameters.AddWithValue("@VesselID", VesselID);
                //adapter.SelectCommand.Parameters.AddWithValue("@sdf", FromDate);
                DataSet dataset = new DataSet();
                adapter.Fill(dataset);
                return dataset;
            }
        }

        public static string GetColor(int label)
        {
            switch (label)
            {
                case 0:
                    return "White";
                case 1:
                case 2:
                case 3:
                    return "Green";
                case 4:
                case 5:
                    return "Orange";
                case 6:
                case 7:
                default:
                    return "Red";
            }
        }

        public static List<SpWinchRotationUpcomingNDue_Result> FindWinchRotation()
        {
            List<SpWinchRotationUpcomingNDue_Result> Wlist = new List<SpWinchRotationUpcomingNDue_Result>();
            DataTable GetWinchRotationRopes = ExecStoredProceedure("spWinchRotationRopes");
            int ApprochingCount = 0; int ExceedCount = 0;

            for (int i = 0; i < GetWinchRotationRopes.Rows.Count; i++)
            {
                int ManufacturerId = Convert.ToInt32(GetWinchRotationRopes.Rows[i]["ManufacturerId"]);
                int RopeTypeId = Convert.ToInt32(GetWinchRotationRopes.Rows[i]["RopeTypeId"]);
                int VesselID = Convert.ToInt32(GetWinchRotationRopes.Rows[i]["VesselID"]);
                decimal CurrentLeadRunningHours = 0;
                string currentRH = GetWinchRotationRopes.Rows[i]["CurrentLeadRunningHours"].ToString();
                if (!string.IsNullOrEmpty(currentRH))
                {
                    CurrentLeadRunningHours = Convert.ToDecimal(GetWinchRotationRopes.Rows[i]["CurrentLeadRunningHours"]);
                }
                string Lead = GetWinchRotationRopes.Rows[i]["lead"].ToString();
                DateTime AssignedDate = Convert.ToDateTime(GetWinchRotationRopes.Rows[i]["AssignedDate"]);


                //string Lead1 = Regex.Replace(Lead, @"\t\n\r", "");

                string Lead1 = Lead.Replace(System.Environment.NewLine, "").Trim();

                DataTable WRSetting = CommonMethods.GetWinchRotationSetting(ManufacturerId, RopeTypeId, VesselID, Lead1);
                if (WRSetting.Rows.Count > 0)
                {
                    int maxrunhrs = Convert.ToInt32(WRSetting.Rows[0]["MaximumRunningHours"]);
                    int maxmnthallowed = Convert.ToInt32(WRSetting.Rows[0]["MaximumMonthsAllowed"]);

                    var AssignedDateAppro = AssignedDate.AddMonths(maxmnthallowed - 2);
                    var AssignedDateExceed = AssignedDate.AddMonths(maxmnthallowed);
                    var CurrentDate = DateTime.Now.Date;//.AddMonths(maxmnthallowed);



                    //Approching Count Start
                    #region
                    int RB = 0; int MA = 0;
                    // if (AssignedDateAppro >= DueDate  && AssignedDate <= DueDate)
                    if (CurrentDate >= AssignedDateAppro && CurrentDate < AssignedDateExceed)
                    {

                        MA++;
                    }

                    if (maxrunhrs > 0)
                    {
                        var maxrunhrs1 = maxrunhrs * 90 / 100;
                        if (CurrentLeadRunningHours > maxrunhrs1)
                            RB++;
                    }

                    if (RB + MA > 0)
                    {
                        ApprochingCount++;
                        SpWinchRotationUpcomingNDue_Result Data = new SpWinchRotationUpcomingNDue_Result()
                        {
                            VesselID = Convert.ToInt32(GetWinchRotationRopes.Rows[i]["VesselID"]),
                            WinchId = Convert.ToInt32(GetWinchRotationRopes.Rows[i]["WinchId"]),
                            RopeId = Convert.ToInt32(GetWinchRotationRopes.Rows[i]["RopeId"]),
                            VesselName = GetWinchRotationRopes.Rows[i]["VesselName"].ToString(),
                            ManufacturerName = GetWinchRotationRopes.Rows[i]["Name"].ToString(),
                            RopeType = GetWinchRotationRopes.Rows[i]["RopeType"].ToString(),
                            AssignedNumber = GetWinchRotationRopes.Rows[i]["AssignedNumber"].ToString(),

                            Location = GetWinchRotationRopes.Rows[i]["Location"].ToString(),
                            lead = GetWinchRotationRopes.Rows[i]["lead"].ToString(),
                            AssignedDate = Convert.ToDateTime(GetWinchRotationRopes.Rows[i]["AssignedDate"]),
                            CurrentLeadRunningHours = GetWinchRotationRopes.Rows[i]["CurrentLeadRunningHours"].ToString() == "" ? 0 : Convert.ToDecimal(GetWinchRotationRopes.Rows[i]["CurrentLeadRunningHours"]),
                            WUpcoming = AssignedDateAppro,
                            StatusUpDue = "Upcoming",
                        };

                        Wlist.Add(Data);

                    }

                    //Approching Count End
                    #endregion
                    //*********************************************************

                    //Exceeded Count Start
                    #region
                    int RB2 = 0; int MA2 = 0;
                    if (CurrentDate >= AssignedDateExceed)
                    {

                        MA2++;
                    }

                    if (maxrunhrs > 0)
                    {
                        //var maxrunhrs1 = maxrunhrs * 90 / 100;
                        if (CurrentLeadRunningHours > maxrunhrs)
                            RB2++;
                    }

                    if (RB2 + MA2 > 0)
                    {
                        ExceedCount++;
                        SpWinchRotationUpcomingNDue_Result Data = new SpWinchRotationUpcomingNDue_Result()
                        {
                            VesselID = Convert.ToInt32(GetWinchRotationRopes.Rows[i]["VesselID"]),
                            WinchId = Convert.ToInt32(GetWinchRotationRopes.Rows[i]["WinchId"]),
                            RopeId = Convert.ToInt32(GetWinchRotationRopes.Rows[i]["RopeId"]),
                            VesselName = GetWinchRotationRopes.Rows[i]["VesselName"].ToString(),
                            ManufacturerName = GetWinchRotationRopes.Rows[i]["Name"].ToString(),
                            RopeType = GetWinchRotationRopes.Rows[i]["RopeType"].ToString(),
                            AssignedNumber = GetWinchRotationRopes.Rows[i]["AssignedNumber"].ToString(),

                            Location = GetWinchRotationRopes.Rows[i]["Location"].ToString(),
                            lead = GetWinchRotationRopes.Rows[i]["lead"].ToString(),
                            AssignedDate = Convert.ToDateTime(GetWinchRotationRopes.Rows[i]["AssignedDate"]),
                            CurrentLeadRunningHours = GetWinchRotationRopes.Rows[i]["CurrentLeadRunningHours"].ToString() == "" ? 0 : Convert.ToDecimal(GetWinchRotationRopes.Rows[i]["CurrentLeadRunningHours"]),
                            WOverDue = AssignedDateExceed,
                            StatusUpDue = "Overdue",
                        };

                        Wlist.Add(Data);
                    }


                    //Exceeded Count End
                    #endregion

                }
            }

            return Wlist;
        }

        public static List<InspectionList> GetInspectionInformation()
        {
            using (SqlConnection connection = new SqlConnection())
            {
                connection.ConnectionString = Convert.ToString(ConfigurationManager.ConnectionStrings["ShipmentContaxt"]);

                //string sqlQuery = @"SELECT MRI.RopeId, MRI.VesselID, MRD.ManufacturerId, MRD.RopeTypeId, MRD.RopeTail, Max(InspectDate) as InspectDate,
                //              CASE
                //               WHEN AverageRating_A >= AverageRating_B THEN AverageRating_A
                //               WHEN AverageRating_A < AverageRating_B THEN AverageRating_B
                //              END AS AverageRating
                //                 FROM MooringRopeInspection MRI INNER JOIN MooringRopeDetail MRD ON MRI.RopeId = MRD.RopeId AND MRI.VesselID = MRD.VesselID 
                //                 Group by MRI.RopeId, MRI.VesselID, MRD.ManufacturerId, MRD.RopeTail, MRD.RopeTypeId, AverageRating_A, AverageRating_B
                //                 Order by MRI.RopeId, MRI.VesselID;";

                //string sqlQuery = @"SELECT MRI.RopeId, MRI.VesselID, MRD.ManufacturerId, MRD.RopeTypeId, MRD.RopeTail, Max(InspectDate) as InspectDate,
                //              Max(AverageRating_A) as AverageRating_A, Max(AverageRating_B) as AverageRating_B 
                //                 FROM MooringRopeInspection MRI INNER JOIN MooringRopeDetail MRD ON MRI.RopeId = MRD.RopeId AND MRI.VesselID = MRD.VesselID 
                //                 Group by MRI.RopeId, MRI.VesselID, MRD.ManufacturerId, MRD.RopeTail, MRD.RopeTypeId
                //                 Order by MRI.RopeId, MRI.VesselID;";

                string sqlQuery = @"SELECT MRI.RopeId, MRI.VesselID, MRD.ManufacturerId, MRD.RopeTypeId, MRD.RopeTail, Max(InspectDate) as InspectDate,
		                            Max(AverageRating_A) as AverageRating_A, Max(AverageRating_B) as AverageRating_B 
	                                FROM MooringRopeInspection MRI INNER JOIN MooringRopeDetail MRD ON MRI.RopeId = MRD.RopeId AND MRI.VesselID = MRD.VesselID 
									Where MRD.OutofServiceDate is null and MRD.DeleteStatus = 0
	                                Group by MRI.RopeId, MRI.VesselID, MRD.ManufacturerId, MRD.RopeTail, MRD.RopeTypeId
	                                Order by MRI.RopeId, MRI.VesselID";

                SqlCommand command = new SqlCommand(sqlQuery, connection)
                {
                    CommandType = CommandType.Text
                };

                SqlDataAdapter adapter = new SqlDataAdapter(command);
                DataTable dataTable1 = new DataTable();
                adapter.Fill(dataTable1);

                DataTable dataTable = new DataTable();
                DataTable dt;

                foreach (DataRow dataRow in dataTable1.Rows)
                {
                    int AverageRating = 1;
                    int ratingA = Convert.ToInt32(dataRow["AverageRating_A"]);
                    int ratingB = Convert.ToInt32(dataRow["AverageRating_B"]);

                    if (ratingA >= ratingB)
                        AverageRating = ratingA;
                    else
                        AverageRating = ratingB;

                    int ropeTail = Convert.ToInt32(dataRow["RopeTail"]);

                    if (ropeTail==1)
                    {
                        sqlQuery = string.Format("Select Top 1 Rating{0} as RatingMonth from tblRopeTailInspectionSetting Where ManufacturerType = {1} and MooringRopeType = {2}",
                            AverageRating, dataRow["ManufacturerId"], dataRow["RopeTypeId"]);
                    }
                    else
                    {
                        sqlQuery = string.Format("Select Top 1 Rating{0} as RatingMonth from tblRopeInspectionSetting Where ManufacturerType = {1} and MooringRopeType = {2}",
                            AverageRating, dataRow["ManufacturerId"], dataRow["RopeTypeId"]);
                    }

                    command = new SqlCommand(sqlQuery, connection)
                    {
                        CommandType = CommandType.Text
                    };

                    if (connection.State != ConnectionState.Open)
                        connection.Open();

                    decimal ratingMonths = 0;
                    SqlDataReader dataReader = command.ExecuteReader();
                    if (dataReader.HasRows)
                    {
                        dataReader.Read();
                        ratingMonths = Convert.ToDecimal(dataReader["RatingMonth"]);
                    }

                    dataReader.Close();

                    //sqlQuery = @"SELECT MRD.InspectionDueDate, MRD.RopeId, MRD.RopeTail, MRD.VesselID, VD.VesselName, MRD.OutofServiceDate, MRD.CertificateNumber, MRT.RopeType, 
                    //                        MRD.InstalledDate FROM dbo.MooringRopeDetail AS MRD 
                    //                        Inner JOIN MooringRopeType MRT on MRD.RopeTypeId = MRT.Id
                    //                        Inner JOIN VesselDetail VD on MRD.VesselID = VD.ImoNo
                    //                        WHERE(MRD.RopeTail = 0 OR MRD.RopeTail IS NULL) AND(MRD.OutofServiceDate IS NULL) AND(MRD.DeleteStatus = 0) 
                    //                        AND VesselID = @VesselID AND RopeId = @RopeID AND MRD.InspectionDueDate < (GETDATE() - @Months)";
                    //sqlQuery = @"SELECT MRD.RopeId, MRD.RopeTail, MRD.VesselID, VD.VesselName, MRD.OutofServiceDate, MRD.CertificateNumber, MRT.RopeType, MRD.InstalledDate,  
                    //                Max(MRI.InspectDate) as LastInspectionDate, MRD.ManufacturerId, C.Name as ManufacturerName, (DATEADD(DAY, @Days, Max(MRI.InspectDate))) As InspectionDueDate,                                    
                    //                CASE
                    //                    WHEN AverageRating_A >= AverageRating_B THEN AverageRating_A
                    //           WHEN AverageRating_A < AverageRating_B THEN AverageRating_B
                    //          END AS AverageRating FROM dbo.MooringRopeDetail AS MRD 
                    //                Inner JOIN MooringRopeType MRT on MRD.RopeTypeId = MRT.Id
                    //                Left Outer Join MooringRopeInspection MRI on MRI.RopeId = MRD.RopeId AND MRI.VesselID = MRD.VesselID 
                    //                Inner JOIN VesselDetail VD on MRD.VesselID = VD.ImoNo
                    //                Inner Join tblCommon C on C.Id = MRD.ManufacturerId
                    //                WHERE MRD.OutofServiceDate IS NULL AND MRD.DeleteStatus = 0 AND MRD.IsActive = 1 AND MRD.VesselID = @VesselID AND MRD.RopeId = @RopeID
                    //                AND MRD.RopeTail = @RopeTail
                    //                GROUP By MRD.RopeId, MRD.RopeTail, MRD.VesselID, VD.VesselName, MRD.OutofServiceDate, MRD.CertificateNumber, MRT.RopeType, MRD.InstalledDate,
                    //                MRD.ManufacturerId, C.Name, MRI.AverageRating_A, MRI.AverageRating_B";

                    if (ratingMonths < 1)
                    {
                        sqlQuery = @"SELECT MRD.RopeId, MRD.RopeTail, MRD.VesselID, VD.VesselName, MRD.OutofServiceDate, MRD.CertificateNumber, MRT.RopeType, MRD.InstalledDate,  
                                    Max(MRI.InspectDate) as LastInspectionDate, MRD.ManufacturerId, C.Name as ManufacturerName, (DATEADD(DAY, @Days, Max(MRI.InspectDate))) As InspectionDueDate,                                    
                                    Max(AverageRating_A) as AverageRating_A, Max(AverageRating_B) as AverageRating_B FROM dbo.MooringRopeDetail AS MRD 
                                    Inner JOIN MooringRopeType MRT on MRD.RopeTypeId = MRT.Id
                                    Left Outer Join MooringRopeInspection MRI on MRI.RopeId = MRD.RopeId AND MRI.VesselID = MRD.VesselID 
                                    Inner JOIN VesselDetail VD on MRD.VesselID = VD.ImoNo
                                    Inner Join tblCommon C on C.Id = MRD.ManufacturerId
                                    WHERE MRD.OutofServiceDate IS NULL AND MRD.DeleteStatus = 0 AND MRD.IsActive = 1 AND MRD.VesselID = @VesselID AND MRD.RopeId = @RopeID
                                    AND MRD.RopeTail = @RopeTail
                                    GROUP By MRD.RopeId, MRD.RopeTail, MRD.VesselID, VD.VesselName, MRD.OutofServiceDate, MRD.CertificateNumber, MRT.RopeType, MRD.InstalledDate,
                                    MRD.ManufacturerId, C.Name";

                        command = new SqlCommand(sqlQuery, connection)
                        {
                            CommandType = CommandType.Text
                        };

                        command.Parameters.AddWithValue("@Days", ratingMonths * 30);
                    }
                    else
                    {
                        sqlQuery = @"SELECT MRD.RopeId, MRD.RopeTail, MRD.VesselID, VD.VesselName, MRD.OutofServiceDate, MRD.CertificateNumber, MRT.RopeType, MRD.InstalledDate,  
                                    Max(MRI.InspectDate) as LastInspectionDate, MRD.ManufacturerId, C.Name as ManufacturerName, (DATEADD(MONTH, @Months, Max(MRI.InspectDate))) As InspectionDueDate,                                    
                                    Max(AverageRating_A) as AverageRating_A, Max(AverageRating_B) as AverageRating_B FROM dbo.MooringRopeDetail AS MRD 
                                    Inner JOIN MooringRopeType MRT on MRD.RopeTypeId = MRT.Id
                                    Left Outer Join MooringRopeInspection MRI on MRI.RopeId = MRD.RopeId AND MRI.VesselID = MRD.VesselID 
                                    Inner JOIN VesselDetail VD on MRD.VesselID = VD.ImoNo
                                    Inner Join tblCommon C on C.Id = MRD.ManufacturerId
                                    WHERE MRD.OutofServiceDate IS NULL AND MRD.DeleteStatus = 0 AND MRD.IsActive = 1 AND MRD.VesselID = @VesselID AND MRD.RopeId = @RopeID
                                    AND MRD.RopeTail = @RopeTail
                                    GROUP By MRD.RopeId, MRD.RopeTail, MRD.VesselID, VD.VesselName, MRD.OutofServiceDate, MRD.CertificateNumber, MRT.RopeType, MRD.InstalledDate,
                                    MRD.ManufacturerId, C.Name";

                        command = new SqlCommand(sqlQuery, connection)
                        {
                            CommandType = CommandType.Text
                        };

                        command.Parameters.AddWithValue("@Months", ratingMonths);
                    }

                    command.Parameters.AddWithValue("@VesselID", dataRow["VesselID"]);
                    command.Parameters.AddWithValue("@RopeID", dataRow["RopeId"]);
                    command.Parameters.AddWithValue("@RopeTail", ropeTail);

                   
                    adapter = new SqlDataAdapter(command);
                    dt = new DataTable();
                    adapter.Fill(dt);

                    if (dt.Rows.Count > 0)
                        dataTable.Merge(dt,true);
                }

                return dataTable.DataTableToList<InspectionList>();
            }
        }

        public static List<T> DataTableToList<T>(this DataTable table) where T : class, new()
        {
            try
            {
                List<T> list = new List<T>();

                foreach (var row in table.AsEnumerable())
                {
                    T obj = new T();

                    foreach (var prop in obj.GetType().GetProperties())
                    {
                        try
                        {
                            PropertyInfo propertyInfo = obj.GetType().GetProperty(prop.Name);
                            propertyInfo.SetValue(obj, Convert.ChangeType(row[prop.Name], propertyInfo.PropertyType), null);
                        }
                        catch
                        {
                            continue;
                        }
                    }

                    list.Add(obj);
                }

                return list;
            }
            catch
            {
                return null;
            }
        }

        public static DataTable LINQResultToDataTable<T>(IEnumerable<T> Linqlist)
        {
            PropertyDescriptorCollection props = TypeDescriptor.GetProperties(typeof(T));

            DataTable table = new DataTable();
            /*
            for (int i = 0; i < props.Count; i++)
            {
                PropertyDescriptor prop = props[i];

                if (i == 1)
                {
                    DataColumn dataColumn = new DataColumn(prop.Name, prop.PropertyType);
                    table.Columns.Add(dataColumn);
                }
                else
                    table.Columns.Add(prop.Name, prop.PropertyType);
            }
            object[] values = new object[props.Count];
            foreach (T item in Linqlist)
            {
                for (int i = 0; i < values.Length; i++)
                {
                    values[i] = props[i].GetValue(item);
                }
                table.Rows.Add(values);
            }
            */

            for (int i = 0; i < props.Count; i++)
            {
                PropertyDescriptor prop = props[i];

                if (i == 1)
                {
                    // DataColumn dataColumn = new DataColumn(prop.Name, prop.PropertyType);
                    DataColumn dataColumn = new DataColumn(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
                    table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
                    //table.Columns.Add(dataColumn);
                }
                else
                    table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
                //table.Columns.Add(prop.Name, prop.PropertyType);
            }
            object[] values = new object[props.Count];
            foreach (T item in Linqlist)
            {
                for (int i = 0; i < values.Length; i++)
                {
                    //values[i] = props[i].GetValue(item);
                    values[i] = props[i].GetValue(item) ?? DBNull.Value; ;
                }
                table.Rows.Add(values);
            }

            return table;
        }

    }


    public class FilterModel
    {
        MorringOfficeEntities context = new MorringOfficeEntities();

        public FilterModel()
        {
            StatusList = new List<SelectListItem>();
            StatusIDs = new List<int>();
            VesselIDs = new List<int>();
            FleetNameIDs = new List<int>();
            FleetTypeIDs = new List<int>();
            TradeIDs = new List<int>();

            MooringOperationDetails = new MOperationBirthDetail();
            FleetNames = new List<tblCommon>();
            FleetTypes = new List<tblCommon>();
            TradePlatforms = new List<tblCommon>();
            Vessels = new List<VesselDetail>();
            ResultList = new List<View_ReportInspectionStatus>();
            ListMooringRopeDetails = new List<View_MooringRopeDetails>();
            ListLooseEquipmentInspection = new List<View_ReportLooseEquipmentInspection>();
            ListOperationRecords = new List<MOperationBirthDetail>();
            RopeSummary = new List<View_MooringRopeDetails>();
            ListAssignRopeToWinch = new List<WinchAssignedRopesTails>();
            ViewInspectionDetails = new List<spInspectionDetail_Result>();
            VesselList = new List<VesselDetail>();

            ListRopeSplicing = new List<RopeSplicingRecord>();
            ListRopeCropping = new List<RopeCropping>();
            ListDamagedRopes = new List<View_RopeDamages>();
            ListRopeDisposalList = new List<RopeDisposal>();
            ListRopeEndtoEndList = new List<RopeEndtoEnd2>();
            ListRopeDiscardedList = new List<MooringRopeDetail>();

            RopeUsedInOperation = new List<View_OperationWiseRopes>();

            StatusList = new List<SelectListItem>
            {
                new SelectListItem { Text = "All", Value = "0" },
                new SelectListItem { Text = "In Service", Value = "2" },
                new SelectListItem { Text = "Out Of Service", Value = "1" }
            };

            TradePlatforms = context.tblCommons.Where(u => u.Type == (int)CommonType.TradePlatform).OrderBy(p => p.Name).ToList();
        }

        [Range(0, 50)]
        public int AgeRangeFrom { get; set; }
        [Range(0, 50)]
        public int AgeRangeTo { get; set; }
        public long RunningHours { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateUpto { get; set; }
        public List<tblCommon> FleetNames { get; set; }
        public List<tblCommon> FleetTypes { get; set; }
        public List<tblCommon> TradePlatforms { get; set; }
        public List<VesselDetail> Vessels { get; set; }
        public List<View_ReportInspectionStatus> ResultList { get; set; }
        public List<View_MooringRopeDetails> ListMooringRopeDetails { get; set; }
        public List<WinchAssignedRopesTails> ListAssignRopeToWinch { get; set; }
        public List<spInspectionDetail_Result> ViewInspectionDetails { get; set; }
        public List<View_ReportLooseEquipmentInspection> ListLooseEquipmentInspection { get; set; }
        public List<MOperationBirthDetail> ListOperationRecords { get; set; }
        public MOperationBirthDetail MooringOperationDetails { get; set; }
        public List<View_MooringRopeDetails> RopeSummary { get; set; }
        public List<VesselDetail> VesselList { get; set; }
        public List<RopeSplicingRecord> ListRopeSplicing { get; set; }
        public List<RopeCropping> ListRopeCropping { get; set; }
        public List<View_RopeDamages> ListDamagedRopes { get; set; }
        public List<RopeDisposal> ListRopeDisposalList { get; set; }
        public List<MooringRopeDetail> ListRopeDiscardedList { get; set; }
        public List<RopeEndtoEnd2> ListRopeEndtoEndList { get; set; }
        public List<View_OperationWiseRopes> RopeUsedInOperation { get; set; }
        public DataTable RopeAnalysis { get; set; }
        public List<SelectListItem> StatusList { get; set; }
        public List<int> StatusIDs { get; set; }
        public List<int> VesselIDs { get; set; }
        public List<int> FleetNameIDs { get; set; }
        public List<int> FleetTypeIDs { get; set; }
        public List<int> TradeIDs { get; set; }
    }

    public class RopeAnalysis : MooringRopeDetailsReport
    {
        public RopeAnalysis()
        {
            YearList = new List<SelectListItem>();

            int currYear = DateTime.Now.Year;

            for (int i = currYear; i >= (currYear - 15); i--)
                YearList.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString() });
        }

        public int Year { get; set; }
        public List<SelectListItem> YearList { get; set; }
        public List<GraphData> ChartData { get; set; }
        public string SelectedVessels { get; set; }
        public string SelectedManufacturers { get; set; }
        public string SelectedRopeTypes { get; set; }

        public string GetChartData(DataTable analysisData, int rating)
        {
            List<ChartDetails> lstGraphData_1 = new List<ChartDetails>
            {
                GetInternalAbrasionData(analysisData, rating, true, System.Drawing.Color.Navy),
                GetInternalAbrasionData(analysisData, rating, false, System.Drawing.Color.Orange)
            };

            return JsonConvert.SerializeObject(lstGraphData_1);
        }

        public static string GetChartDataCom(List<AnomaliesChartData> analysisData, string Lable1, string Lable2)
        {
            List<ChartDetails> lstGraphData_1 = new List<ChartDetails>
            {
                GetInternalAnomaliesData(analysisData,true, Lable1, Lable2, System.Drawing.Color.Orange),
                GetInternalAnomaliesData(analysisData,false, Lable1, Lable2,  System.Drawing.Color.Navy)

                //GetInternalAnomaliesData(analysisData,true, Lable1, Lable2, System.Drawing.ColorTranslator.FromHtml("#F8BB46")),
                //GetInternalAnomaliesData(analysisData,false, Lable1, Lable2,  System.Drawing.ColorTranslator.FromHtml("#223B74"))
            };

            return JsonConvert.SerializeObject(lstGraphData_1);
        }

      
        public static ChartDetails GetAnomaliesGraphs(List<int> analysisData, bool internalRating, string Lable1, System.Drawing.Color colorName)
        {
            ChartDetails graphData = new ChartDetails
            {
                backgroundColor = colorName,
                borderWidth = 1,
            };

            foreach (var row in analysisData)
            {
                if (internalRating)
                {
                    graphData.label = Lable1; // string.Format("No of Times reported", rating);
                    graphData.data.Add(Convert.ToInt32(row));
                    //graphData.data.Add(Convert.ToInt32(row[string.Format("A_{0}", rating)]));

                }
                //else
                //{
                //    graphData.label = string.Format("No of Ports reported", rating);
                //    graphData.data.Add(Convert.ToInt32(row.value2));
                //    //graphData.data.Add(Convert.ToInt32(row[string.Format("B_{0}", rating)]));
                //}
            }

            return graphData;
        }
        public static ChartDetails GetInternalAnomaliesData(List<AnomaliesChartData> analysisData, bool internalRating, string Lable1, string Lable2, System.Drawing.Color colorName)
        {
            ChartDetails graphData = new ChartDetails
            {
                backgroundColor = colorName,
                borderWidth = 1,
            };

            foreach (var row in analysisData)
            {
                if (internalRating)
                {
                    // graphData.label = string.Format("No of Times reported", rating);
                    graphData.label = Lable1;
                    graphData.data.Add(Convert.ToInt32(row.value1));
                    //graphData.data.Add(Convert.ToInt32(row[string.Format("A_{0}", rating)]));

                }
                else
                {
                    // graphData.label = string.Format("No of Ports reported", rating);
                    graphData.label = Lable2;
                    graphData.data.Add(Convert.ToInt32(row.value2));
                    //graphData.data.Add(Convert.ToInt32(row[string.Format("B_{0}", rating)]));
                }
            }

            return graphData;
        }

        public ChartDetails GetInternalAbrasionData(DataTable analysisData, int rating, bool internalRating, System.Drawing.Color colorName)
        {
            ChartDetails graphData = new ChartDetails
            {
                backgroundColor = colorName,
                borderWidth = 1,
            };

            foreach (DataRow row in analysisData.Rows)
            {
                if (internalRating)
                {
                    graphData.label = string.Format("Internal Abrasion - {0}", rating);
                    graphData.data.Add(Convert.ToInt32(row[string.Format("A_{0}", rating)]));
                }
                else
                {
                    graphData.label = string.Format("External Abrasion - {0}", rating);
                    graphData.data.Add(Convert.ToInt32(row[string.Format("B_{0}", rating)]));
                }
            }

            return graphData;
        }

        public class ChartDetails
        {
            public ChartDetails()
            {
                data = new List<int>();
            }

            public string label { get; set; }
            public System.Drawing.Color backgroundColor { get; set; }
            public int borderWidth { get; set; }
            public List<int> data { get; set; }
        }
    }

    public class GraphData
    {
        public int chartId { get; set; }
        public string data { get; set; }
    }



    public class MooringRopeDetailsReport : FilterModel
    {
        MorringOfficeEntities officeEntities = new MorringOfficeEntities();

        public MooringRopeDetailsReport() : base()
        {
            InspectionRatingIDs = new List<int>();
            ManufacturerIDs = new List<int>();
            RopeTypeIDs = new List<int>();
            RopeFilterIDs = new List<int>();

            RopeFilters = new List<SelectListItem>
            {
                new SelectListItem { Text = "All", Value = "0" },
                new SelectListItem { Text = "Satisfactory", Value = "1" },
                new SelectListItem { Text = "NotSatisfactory", Value = "2" }
            };

            InspectionRatingList = new List<SelectListItem>();

            for (int i = 1; i <= 7; i++)
                InspectionRatingList.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString() });

            RopeType = officeEntities.MooringRopeTypes.ToList();

            ManufacturerList = officeEntities.tblCommons.Where(u => u.Type == (int)CommonType.RopeManufacturer).OrderBy(p => p.Name).ToList();

            //officeEntities.VesselDetails.ToList().ForEach(u => VesselIDs.Add(u.ImoNo));
        }

        public List<SelectListItem> InspectionRatingList { get; set; }
        public List<tblCommon> ManufacturerList { get; set; }
        public List<Reports.MooringRopeType> RopeType { get; set; }
        public List<SelectListItem> RopeFilters { get; set; }

        public List<int> InspectionRatingIDs { get; set; }
        public List<int> ManufacturerIDs { get; set; }
        public List<int> RopeTypeIDs { get; set; }
        public List<int> RopeFilterIDs { get; set; }

        [Range(0, long.MaxValue)]
        public long RunningHoursFrom { get; set; }

        [Range(0, long.MaxValue)]
        public long RunningHoursTo { get; set; }
    }

    public class RopeDetailsReport : FilterModel
    {
        MorringOfficeEntities officeEntities = new MorringOfficeEntities();

        public RopeDetailsReport() : base()
        {
            InspectionRatingIDs = new List<int>();
            ManufacturerIDs = new List<int>();
            RopeTypeIDs = new List<int>();
            RopeFilterIDs = new List<int>();
            ListRopeDetails = new List<View_RopeDetails>();

            RopeFilters = new List<SelectListItem>
            {
                new SelectListItem { Text = "All", Value = "0" },
                new SelectListItem { Text = "Satisfactory", Value = "1" },
                new SelectListItem { Text = "NotSatisfactory", Value = "2" }
            };

            InspectionRatingList = new List<SelectListItem>();

            for (int i = 1; i <= 7; i++)
                InspectionRatingList.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString() });

            RopeType = officeEntities.MooringRopeTypes.OrderBy(u => u.RopeType).ToList();

            ManufacturerList = officeEntities.tblCommons.OrderBy(p => p.Name).Where(u => u.Type == (int)CommonType.RopeManufacturer).ToList();
        }

        public List<SelectListItem> InspectionRatingList { get; set; }
        public List<tblCommon> ManufacturerList { get; set; }
        public List<Reports.MooringRopeType> RopeType { get; set; }
        public List<SelectListItem> RopeFilters { get; set; }
        public List<View_RopeDetails> ListRopeDetails { get; set; }

        public List<int> InspectionRatingIDs { get; set; }
        public List<int> ManufacturerIDs { get; set; }
        public List<int> RopeTypeIDs { get; set; }
        public List<int> RopeFilterIDs { get; set; }

        [Range(0, long.MaxValue)]
        public long RunningHoursFrom { get; set; }

        [Range(0, long.MaxValue)]
        public long RunningHoursTo { get; set; }
    }

    public class RopeInspectionReport : MooringRopeDetailsReport
    {
        public List<RopeInspectionReportList> RopeInspectionList { get; set; }
        public List<RopeInspectionReportList> RopeTailInspectionList { get; set; }
    }

    public class RopeInspectionReportList
    {
        public int ID { get; set; }
        public int? InspectionID { get; set; }
        public DateTime? InspectDate { get; set; }
        public string InspectBy { get; set; }
        public int? RopeTail { get; set; }
    }

    public class LooseEquipDetailReport : FilterModel
    {
        public List<LooseEType> LooseEquipments { get; set; }
        public List<int> LooseEquipmentIDs { get; set; }
    }

    public class LooseEquDetailsList
    {
        public LooseEquDetailsList()
        {
            JoiningShackle = new List<LooseEquDetailClass>();
            ChainStopper = new List<LooseEquDetailClass>();
            ChafeGuard = new List<LooseEquDetailClass>();
            WinchBreakTestKit = new List<LooseEquDetailClass>();
            RopeStopper = new List<LooseEquDetailClass>();
            MessengerRope = new List<LooseEquDetailClass>();
            FireWire = new List<LooseEquDetailClass>();
            TowingRope = new List<LooseEquDetailClass>();
            SuezRope = new List<LooseEquDetailClass>();
        }

        public List<LooseEquDetailClass> JoiningShackle { get; set; }
        public List<LooseEquDetailClass> ChainStopper { get; set; }
        public List<LooseEquDetailClass> ChafeGuard { get; set; }
        public List<LooseEquDetailClass> WinchBreakTestKit { get; set; }
        public List<LooseEquDetailClass> RopeStopper { get; set; }
        public List<LooseEquDetailClass> MessengerRope { get; set; }
        public List<LooseEquDetailClass> FireWire { get; set; }
        public List<LooseEquDetailClass> TowingRope { get; set; }
        public List<LooseEquDetailClass> SuezRope { get; set; }
    }
    public class LooseEquDetailClass
    {
        public int Id { get; set; }
        public int LooseETypeId { get; set; }

        public string UniqueID { get; set; }
        public string IdentificationNumber { get; set; }
        public string ManufactureName { get; set; }

        public decimal MBL { get; set; }
        public string Type { get; set; }
        public string CertificateNumber { get; set; }


        public decimal Diameter { get; set; }
        public decimal Length { get; set; }
        public decimal LDBF { get; set; }
        public decimal WLL { get; set; }

        public string RopeTagging { get; set; }
        public string RopeConstruction { get; set; }


        public Nullable<DateTime> ReceivedDate { get; set; }
        public Nullable<DateTime> InstalledDate { get; set; }
        public Nullable<DateTime> InspectionDueDate { get; set; }

        public string VesselName { get; set; }


    }

    public class RopeSummaryReport
    {
        public RopeSummaryReport()
        {
            RopesInspectedList = new List<MooringRopeInspection>();
            RopesDiscardedList = new List<MooringRopeDetail>();
            RopesCroppedList = new List<vRopesCropped>();
            RopesDamagedList = new List<vRopesDamaged>();
            RopesDetailList = new List<vRopesDetail>();
            RopesDisposedList = new List<vRopesDisposed>();
            RopesEndToEndList = new List<vRopesEndToEnd>();
            RopesSplicedList = new List<vRopesSpliced>();

            WinchRotationUpcoming_Detail = new List<SpWinchRotationUpcomingNDue_Result>();

            WinchRotationDue_Detail = new List<SpWinchRotationUpcomingNDue_Result>();
        }

        public List<MooringRopeInspection> RopesInspectedList { get; set; }
        public List<MooringRopeDetail> RopesDiscardedList { get; set; }
        public List<vRopesCropped> RopesCroppedList { get; set; }
        public List<vRopesDamaged> RopesDamagedList { get; set; }
        public List<vRopesDetail> RopesDetailList { get; set; }
        public List<vRopesDisposed> RopesDisposedList { get; set; }
        public List<vRopesEndToEnd> RopesEndToEndList { get; set; }
        public List<vRopesSpliced> RopesSplicedList { get; set; }

        public List<ResidualLabTestClass> ResidualLabTestList { get; set; }

        public string RopeAttachment { get; set; }

        //public List<InspectionList> OverdueInspectionList { get; set; }
        //public List<InspectionList> PendingInspectionList { get; set; }

        public List<View_SatisfactoryRopes_Details> SatisfactoryRopesList { get; set; }
        public List<View_UnSatisfactoryRopes_Details> UnSatisfactoryRopesList { get; set; }

        //public List<spRopesRequiringDiscard_Details_Result> RopeDiscardDetails { get; set; }

        //public List<spTailsRequiringDiscard_Details_Result> TailDiscardDetails { get; set; }

        public List<SpUpcomingRopeDiscard_Result> RopeDiscardDetails { get; set; }
        public List<SpUpcomingRopeDiscard_Result> TailDiscardDetails { get; set; }

        //public List<View_RopesOverdueInspection_Details> OverdueInspectionList { get; set; }
        //public List<View_RopesPendingInspection_Details> PendingInspectionList { get; set; }

        public List<InspectionList> OverdueInspectionList { get; set; }
        public List<InspectionList> PendingInspectionList { get; set; }

        public List<SpUpcomingRopeDiscard_Result> UpcomingRopeDiscard_Detail { get; set; }
        public List<SpUpcomingRopeDiscard_Result> UpcomingTailDiscard_Detail { get; set; }

        public List<SpEndtoEndUpcomingNDue_Result> End2EndUpcoming_Detail { get; set; }
        public List<SpEndtoEndUpcomingNDue_Result> End2EndDue_Detail { get; set; }

        public List<SpWinchRotationUpcomingNDue_Result> WinchRotationUpcoming_Detail { get; set; }
        public List<SpWinchRotationUpcomingNDue_Result> WinchRotationDue_Detail { get; set; }
        //SpWinchRotationUpcomingNDue_Result

    }

    #region Analysis_Models

    public class ReportAnalysisFilterModel
    {
        public ReportAnalysisFilterModel()
        {
            FleetNameIDs = new List<int>();
            FleetTypeIDs = new List<int>();
            TradeIDs = new List<int>();
            VesselIDs = new List<int>();

            VesselList = new List<VesselDetail>();
            AnalysisResults = new List<AnalysisResult>();

            //&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&
            VesselWiseMooringOP_Details = new List<View_VesselWiseMooringOP_Detail2>();
            AnomaliesAllResult = new List<AnomaliesDetail>();
            AnomaliesRangeOFTide = new List<AnomaliesDetail>();
            AnomaliesCurrentForce = new List<AnomaliesDetail>();
            AnomaliesWindForce = new List<AnomaliesDetail>();
            AnomaliesAnySquall = new List<AnomaliesDetail>();
            AnomaliesAnySurgingObserved = new List<AnomaliesDetail>();
            AnomaliesAnySeaSwell = new List<AnomaliesDetail>();
            AnomaliesAny_Affect_Passing_Traffic = new List<AnomaliesDetail>();
            Anomaliesfender_Contact = new List<AnomaliesDetail>();
            AnomaliesOFAirTemprature = new List<AnomaliesDetail>();
            //&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&


            AnalysysExpenseVsDuration = new List<View_AnalysysExpenseVsDuration>();
            AverageRunningRopeHours = new List<View_AverageRunningRopeHours>();
            ManufacturerPerformanceComparison = new List<View_ManufacturerPerformanceComparison>();
            PredictionExpectency = new List<View_PredictionExpectency>();
            VesselWiseExpenseAnalysis = new List<View_VesselWiseExpenseAnalysis>();
            OperationRecords = new List<View_OperationRecords>();

            BirthTypes = new List<SelectListItem>
            {
                new SelectListItem { Text = "Anchorage", Value = "Anchorage" },
                new SelectListItem { Text = "CBM", Value = "CBM" },
                new SelectListItem { Text = "Dolphin", Value = "Dolphin" },
                new SelectListItem { Text = "Double-banking at Berth", Value = "Double-banking at Berth" },
                new SelectListItem { Text = "DP Operations", Value = "DP Operations" },
                new SelectListItem { Text = "SBM / SPM", Value = "SBM / SPM" },
                new SelectListItem { Text = "Straight", Value = "Straight" },
                new SelectListItem { Text = "STS (incldung reverse STS)", Value = "STS (incldung reverse STS)" },
                new SelectListItem { Text = "Tandem mooring to F(P)SO", Value = "Tandem mooring to F(P)SO" },
                new SelectListItem { Text = "Wooden", Value = "Wooden" },
            };

            MooringTypes = new List<SelectListItem>
            {
                new SelectListItem { Text = "At Anchor", Value = "At Anchor" },
                new SelectListItem { Text = "At Sea - Drifting", Value = "At Sea - Drifting" },
                new SelectListItem { Text = "At Sea - Manoeuvring", Value = "At Sea - Manoeuvring" },
                new SelectListItem { Text = "CBM", Value = "CBM" },
                new SelectListItem { Text = "Open to Sea berth", Value = "Open to Sea berth" },
                new SelectListItem { Text = "River Channel Berth", Value = "River Channel Berth" },
                new SelectListItem { Text = "SBM / SPM", Value = "SBM / SPM" },
                new SelectListItem { Text = "Sheltered Berth", Value = "Sheltered Berth" },
                new SelectListItem { Text = "Tandem mooring to F(P)SO", Value = "Tandem mooring to F(P)SO" },
            };

            LeadTypes = new List<SelectListItem>
            {
                new SelectListItem { Text = "Direct", Value = "Direct" },
                new SelectListItem { Text = "Indirect", Value = "Indirect" },
            };

            Leads = new List<SelectListItem>();
            //{
            //    new SelectListItem { Text = "Breast", Value = "Breast" },
            //    new SelectListItem { Text = "Headline", Value = "Headline" },
            //    new SelectListItem { Text = "Spring", Value = "Spring" },
            //    new SelectListItem { Text = "Sternline", Value = "Sternline" },
            //};


            Ports = new List<SelectListItem>();
            PortFacilities = new List<SelectListItem>() { new SelectListItem { Text = "None Selected", Value = "None Selected" } };

            using (MorringOfficeEntities context = new MorringOfficeEntities())
            {
                foreach (var v in context.tblCommons.Where(x => x.Type == 6).OrderBy(p => p.Id).ToList())
                    Leads.Add(new SelectListItem() { Text = v.Name, Value = v.Name.ToString() });

                var portDetails = context.PortLists.OrderBy(p => p.PortName).GroupBy(p => p.PortName).Select(u => new { PortName = u.Key }).ToList();

                foreach (var portinfo in portDetails)
                    Ports.Add(new SelectListItem { Text = portinfo.PortName.Trim(), Value = portinfo.PortName.Trim() });

                TradePlatformList = context.tblCommons.Where(u => u.Type == (int)CommonType.TradePlatform).OrderBy(u => u.Name).ToList();
                ManufacturerList = context.tblCommons.Where(u => u.Type == (int)CommonType.RopeManufacturer).OrderBy(u => u.Name).ToList();
                RopeTypeList = context.MooringRopeTypes.OrderBy(u => u.RopeType).ToList();
            }
        }

        public List<SelectListItem> Ports { get; set; }
        public string PortNames { get; set; }

        public List<SelectListItem> PortFacilities { get; set; }
        public string PortFacilityNames { get; set; }
        public List<SelectListItem> BirthTypes { get; set; }
        public string BirthTypeNames { get; set; }
        public List<SelectListItem> MooringTypes { get; set; }
        public string MooringTypeNames { get; set; }
        public List<SelectListItem> LeadTypes { get; set; }
        public string LeadTypeNames { get; set; }
        public List<SelectListItem> Leads { get; set; }
        public string LeadNames { get; set; }

        [Range(0, 200)]
        public int WindSpeedRangeFrom { get; set; }
        [Range(0, 200)]
        public int WindSpeedRangeTo { get; set; }


        public string Squal_Gusts { get; set; }

        [Range(0, 50)]
        public int CurrentSpeedRangeFrom { get; set; }
        [Range(0, 50)]
        public int CurrentSpeedRangeTo { get; set; }

        [Range(0, 30)]
        public int TidalRangeFrom { get; set; }
        [Range(0, 30)]
        public int TidalRangeTo { get; set; }

        public string BerthExposesToSwell { get; set; }
        public string SurgingObserved { get; set; }


        public string TrafficPassingEffect { get; set; }
        public string ShipFenderContact { get; set; }

        [Range(-50, 60)]
        public int AirTempRangeFrom { get; set; }

        [Range(-50, 60)]
        public int AirTempRangeTo { get; set; }

        public string RopeDamagedAnytime { get; set; }



        [Range(0, 50)]
        public int AgeRangeFrom { get; set; }
        [Range(0, 50)]
        public int AgeRangeTo { get; set; }
        public DateTime DateDiscardedFrom { get; set; }
        public DateTime DateDiscardedUpto { get; set; }

        public bool AactiveNonActicve { get; set; } = true;

        public List<tblCommon> FleetNameList { get; set; }
        public List<int> FleetNameIDs { get; set; }

        public List<tblCommon> FleetTypeList { get; set; }
        public List<int> FleetTypeIDs { get; set; }

        public List<tblCommon> TradePlatformList { get; set; }
        public List<int> TradeIDs { get; set; }

        public List<VesselDetail> VesselList { get; set; }
        public List<int> VesselIDs { get; set; }



        public List<Reports.tblCommon> ManufacturerList { get; set; }
        public List<long> ManufacturerIDs { get; set; }
        public List<Reports.MooringRopeType> RopeTypeList { get; set; }
        public List<long> RopeTypeIDs { get; set; }


        public List<View_AnalysysExpenseVsDuration> AnalysysExpenseVsDuration { get; set; }
        public List<View_AverageRunningRopeHours> AverageRunningRopeHours { get; set; }
        public List<View_ManufacturerPerformanceComparison> ManufacturerPerformanceComparison { get; set; }
        public List<View_PredictionExpectency> PredictionExpectency { get; set; }
        public List<View_VesselWiseExpenseAnalysis> VesselWiseExpenseAnalysis { get; set; }
        public List<View_OperationRecords> OperationRecords { get; set; }

        public List<OperationResult> OperationResultList { get; set; }

        public List<SelectListItem> GetListBoolean()
        {
            List<SelectListItem> items = new List<SelectListItem>();
            items.Add(new SelectListItem { Text = "All", Value = "All" });
            items.Add(new SelectListItem { Text = "Yes", Value = "Yes" });
            items.Add(new SelectListItem { Text = "No", Value = "No" });
            return items;
        }

        public List<View_VesselWiseMooringOP_Detail2> VesselWiseMooringOP_Details { get; set; }
        public List<AnalysisResult> AnalysisResults { get; set; }
        public List<AnomaliesDetail> AnomaliesAllResult { get; set; }
        public List<AnomaliesDetail> AnomaliesRangeOFTide { get; set; }
        public List<AnomaliesDetail> AnomaliesCurrentForce { get; set; }
        public List<AnomaliesDetail> AnomaliesWindForce { get; set; }
        public List<AnomaliesDetail> AnomaliesAnySquall { get; set; }
        public List<AnomaliesDetail> AnomaliesAnySurgingObserved { get; set; }
        public List<AnomaliesDetail> AnomaliesAnySeaSwell { get; set; }
        public List<AnomaliesDetail> AnomaliesAny_Affect_Passing_Traffic { get; set; }
        public List<AnomaliesDetail> Anomaliesfender_Contact { get; set; }
        public List<AnomaliesDetail> AnomaliesOFAirTemprature { get; set; }
    }

    public class AnomaliesDetail
    {
        public string PortName { get; set; }
        public string FacilityName { get; set; }
        public string BirthName { get; set; }
        public string BirthType { get; set; }

        public DateTime? FastDatetime { get; set; }
        public decimal RangeOFTide { get; set; }
        public decimal CurrentSpeed { get; set; }
        public decimal WindSpeed { get; set; }

        public string MooringType { get; set; }
        public string Any_Rope_Damaged { get; set; }
        public string AnySquall { get; set; }

        public string SurgingObserved { get; set; }
        public string Berth_exposed_SeaSwell { get; set; }

        public string Any_Affect_Passing_Traffic { get; set; }

        public string Ship_was_continuously_contact_with_fender { get; set; }
        public decimal AirTemprature { get; set; }

        public string FleetName { get; set; }
        public int FleetNameID { get; set; }
        public string FleetType { get; set; }
        public int FleetTypeID { get; set; }
        public string VesselName { get; set; }
        public int ImoNo { get; set; }
        public DateTime DateBuilt { get; set; }
        public int TradeAreaID { get; set; }
        public string TradeArea { get; set; }

        public int RopeTypeId { get; set; }
        public int ManufacturerId { get; set; }
        public string Lead { get; set; }
        public string Lead1 { get; set; }


        // VD.FleetName, VD.FleetNameID, VD.FleetType, VD.FleetTypeID, VD.VesselName, VD.ImoNo, VD.DateBuilt,VD.TradeAreaID, VD.TradeArea,

    }

    public class OperationResult
    {
        public int OperationID { get; set; }
        public DateTime? CastDateTime { get; set; }
        public DateTime? FastDateTime { get; set; }
        public string BirthName { get; set; }
        public string PortName { get; set; }
        public string FacilityName { get; set; }
        public int RopeDamaged { get; set; }
        public int IMO { get; set; }
        public string VesselName { get; set; }
    }

    public class AnalysisResult
    {
        public int ImoNo { get; set; }
        public decimal? Cost { get; set; }
        public string RopeType { get; set; }
        public int? RopeTypeId { get; set; }
        public int? ManufacturerId { get; set; }
        public string Manufacturer { get; set; }
        public string VesselName { get; set; }
        public int? RunningHours { get; set; }
        public decimal? Avg_Months { get; set; }

        public int RopeId { get; set; }
        public int? TotalCost { get; set; }
        public int? RopeTail { get; set; }
        public int LineCount { get; set; }
    }

    #endregion

    public class RopeDamageCropSplicFilter
    {
        public RopeDamageCropSplicFilter()
        {
            using (MorringOfficeEntities context = new MorringOfficeEntities())
            {
                //TradePlatformList = context.tblCommons.Where(u => u.Type == (int)CommonType.TradePlatform).OrderBy(u => u.Name).ToList();
                ManufacturerList = context.tblCommons.Where(u => u.Type == (int)CommonType.RopeManufacturer).OrderBy(u => u.Name).ToList();
                RopeTypeList = context.MooringRopeTypes.OrderBy(u => u.RopeType).ToList();
            }

            DamagedList = new List<spDamage1_Result>();
            CroppedList = new List<spCropping1_Result>();
            SplicedList = new List<spSplicing1_Result>();
        }

        public List<tblCommon> ManufacturerList { get; set; }
        public List<long> ManufacturerIDs { get; set; }
        public List<Reports.MooringRopeType> RopeTypeList { get; set; }
        public List<long> RopeTypeIDs { get; set; }

        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }

        public int vessel { get; set; }
        public List<spDamage1_Result> DamagedList { get; set; }
        public List<spCropping1_Result> CroppedList { get; set; }
        public List<spSplicing1_Result> SplicedList { get; set; }

        public string StatusName { get; set; }
        public List<SelectListItem> Status()
        {
            List<SelectListItem> items = new List<SelectListItem>();
            items.Add(new SelectListItem { Text = "All", Value = "All" });
            items.Add(new SelectListItem { Text = "In Service", Value = "INSERVICE" });
            items.Add(new SelectListItem { Text = "Discarded", Value = "DISCARDED" });
            return items;
        }

    }


    public class ResidualLabFilter
    {
        public ResidualLabFilter()
        {
            FleetNameIDs = new List<int>();
            FleetTypeIDs = new List<int>();
            TradeIDs = new List<int>();
            VesselIDs = new List<int>();
            ManufacturerIDs = new List<long>();
            RopeTypeIDs = new List<long>();

            using (MorringOfficeEntities context = new MorringOfficeEntities())
            {
                TradePlatformList = context.tblCommons.Where(u => u.Type == (int)CommonType.TradePlatform).OrderBy(u => u.Name).ToList();
                ManufacturerList = context.tblCommons.Where(u => u.Type == (int)CommonType.RopeManufacturer).OrderBy(u => u.Name).ToList();
                RopeTypeList = context.MooringRopeTypes.OrderBy(u => u.RopeType).ToList();
            }

            VesselList = new List<VesselDetail>();

            FleetWiseIncidents = new List<View_FleetWiseIncidents2>();
            ResidualStrength_RopeLabResults = new List<View_ResidualStrength_RopeLabResults>();
            RopesDamagedCroppedSplicedUsed = new List<View_RopesDamagedCroppedSplicedUsed>();
            RopesDamagedCroppedSplicedUsed2 = new List<View_RopesDamagedCroppedSplicedUsed2>();
            DamagedObservedList = new List<DamagedObserved>();
        }

        public List<tblCommon> FleetNameList { get; set; }
        public List<int> FleetNameIDs { get; set; }
        public List<tblCommon> FleetTypeList { get; set; }
        public List<int> FleetTypeIDs { get; set; }
        public List<tblCommon> TradePlatformList { get; set; }
        public List<int> TradeIDs { get; set; }
        public List<VesselDetail> VesselList { get; set; }
        public List<int> VesselIDs { get; set; }
        public List<tblCommon> ManufacturerList { get; set; }
        public List<long> ManufacturerIDs { get; set; }
        public List<Reports.MooringRopeType> RopeTypeList { get; set; }
        public List<long> RopeTypeIDs { get; set; }
        [Range(0, 50)]
        public int AgeRangeFrom { get; set; }
        [Range(0, 50)]
        public int AgeRangeTo { get; set; }
        [Range(0, 50)]
        public int MonthsServiceFrom { get; set; }
        [Range(0, 50)]
        public int MonthsServiceTo { get; set; }
        [Range(0, 50)]
        public int ResidualStrengthFrom { get; set; }
        [Range(0, 50)]
        public int ResidualStrengthTo { get; set; }
        public DateTime TestDateFrom { get; set; }
        public DateTime TestDateTo { get; set; }
        [Range(0, 100000)]
        public int RunningHoursFrom { get; set; }
        [Range(0, 100000)]
        public int RunningHoursTo { get; set; }
        public DateTime DateInstalledFrom { get; set; }
        public DateTime DateInstalledTo { get; set; }
        public string StatusName { get; set; }
        public List<SelectListItem> Status()
        {
            List<SelectListItem> items = new List<SelectListItem>();
            items.Add(new SelectListItem { Text = "All", Value = "All" });
            items.Add(new SelectListItem { Text = "In Service", Value = "INSERVICE" });
            items.Add(new SelectListItem { Text = "Discarded", Value = "DISCARDED" });
            return items;
        }

        public List<View_FleetWiseIncidents2> FleetWiseIncidents { get; set; }
        public List<View_ResidualStrength_RopeLabResults> ResidualStrength_RopeLabResults { get; set; }
        public List<View_RopesDamagedCroppedSplicedUsed> RopesDamagedCroppedSplicedUsed { get; set; }
        public List<View_RopesDamagedCroppedSplicedUsed2> RopesDamagedCroppedSplicedUsed2 { get; set; }
        public List<DamagedObserved> DamagedObservedList { get; set; }
    }

    public class DamagedObserved 
    {
        public int VesselID { get; set; }
        public string VesselName { get; set; }
        public int FleetNameID { get; set; }
        public int FleetTypeID { get; set; }
        public int TradeAreaID { get; set; }
        public int RopeId { get; set; }
        public string CertificateNumber { get; set; }
        public int RopeTail { get; set; }

        public DateTime? OutofServiceDate { get; set; }

        public int ManufacturerId { get; set; }

        public int RopeTypeId { get; set; }
        public string ManufacturerName { get; set; }
        public string RopeType { get; set; }
        public string DamageObserved { get; set; }
        public string DamageReason { get; set; }
        public string IncidentActlion { get; set; }
        public DateTime? DamageDate { get; set; }

        
    }

    public class VesselModel
    {
        public Int32 VesselID { get; set; }
        public string VesselName { get; set; }
    }

    public class AbrasionChartData
    {
        public int label { get; set; }
        public int value { get; set; }
        public string color { get; set; }
    }

    public class AbrasionData
    {
        public AbrasionData()
        {
            values = new List<AbrasionChartData>();
                      
        }

        public string key { get; set; }
        public List<AbrasionChartData> values { get; set; }

       
    }
    public class AnomaliesChartData
    {
        public int label { get; set; }
        public int value1 { get; set; }
        public int value2 { get; set; }
        public string labelName { get; set; }
        public string color { get; set; }
    }
    public class AbrasionDetails : RopeAnalysis
    {
       
        public DataSet AbrasionDetailsResult { get; set; }
       
        
    }

    public class InspectionList
    {
        public InspectionList()
        { }
        public int RopeID { get; set; }
        public int RopeTail { get; set; }
        public int VesselID { get; set; }
        public string VesselName { get; set; }
        public DateTime? OutOfServiceDate { get; set; }
        public string CertificateNumber { get; set; }
        public string RopeType { get; set; }
        public DateTime InstalledDate { get; set; }
        public DateTime LastInspectionDate { get; set; }
        public int ManufacturerID { get; set; }
        public string ManufacturerName { get; set; }
        public DateTime InspectionDueDate { get; set; }
        public int AverageRating_A { get; set; }
        public int AverageRating_B { get; set; }
    }

    public class WinchAssignedRopesTails
    {
        //Reports/RopeSummary?rope=43&tail=1&vessel=9778296
        //AssignedLocation	AssignedDate	RopeTail
        public WinchAssignedRopesTails()
        {
            TailList = new List<AssignedTailsDetail>();
        }
        public string VesselName { get; set; }
        public int VesselID { get; set; }
        public int RopeId { get; set; }
        public int WinchId { get; set; }
        public bool RopeTail { get; set; }
        public string assignedWinchNumber { get; set; }
        public string AssignedLocation { get; set; }

        public string RopeCertificateNumber { get; set; }

        public string MBL_Max_BHF { get; set; }
        public string Lead { get; set; }

        public List<AssignedTailsDetail> TailList { get; set; }

        public Nullable<DateTime> AssignedDate { get; set; }
        public string Outboard { get; set; }
        public string UniqueID { get; set; }


    }
    public class AssignedTailsDetail
    {
        public int VesselID { get; set; }
        public string UniqueID { get; set; }
        public string TailCertificateNumber { get; set; }
        public int RopeId { get; set; }
        public int WinchId { get; set; }
        public bool RopeTail { get; set; }
    }
    public class Abrasion_DetailsList
    {
       
        public int VesselID { get; set; }
        public string VesselName { get; set; }
        public int RopeId { get; set; }
        public string RopeCertificateNumber { get; set; }
       
        public int Rating { get; set; }
       
        public string AssignedNumber { get; set; }
        public string InspectBy { get; set; }
        public DateTime InspectDate { get; set; }

        public string Image1 { get; set; }

        public string Image2 { get; set; }
    }

    public class View_VesselWiseMooringOP_Detail2
    {
        public int OPId { get; set; }
        public string FleetName { get; set; }
        public int FleetNameID { get; set; }
        public string FleetType { get; set; }
        public int FleetTypeID { get; set; }
        public string VesselName { get; set; }
        public int ImoNo { get; set; }
        public System.DateTime DateBuilt { get; set; }
        public string PortName { get; set; }
        public string FacilityName { get; set; }
        public string BirthName { get; set; }
        public string BirthType { get; set; }
        public string MooringType { get; set; }
        public Nullable<decimal> RangOfTide { get; set; }
        public Nullable<int> WindSpeed { get; set; }
        public string AnySquall { get; set; }
        public Nullable<decimal> CurrentSpeed { get; set; }
        public string Berth_exposed_SeaSwell { get; set; }
        public string SurgingObserved { get; set; }
        public string Any_Affect_Passing_Traffic { get; set; }
        public string Ship_was_continuously_contact_with_fender { get; set; }
        public Nullable<int> AirTemprature { get; set; }
        public string Any_Rope_Damaged { get; set; }
        public Nullable<System.DateTime> FastDatetime { get; set; }
        public int TradeAreaID { get; set; }
        public string TradeArea { get; set; }
    }
}