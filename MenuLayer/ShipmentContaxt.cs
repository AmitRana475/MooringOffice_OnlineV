using AnalysisLayer;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using NotificationLayer;
using System.Data.SqlClient;

namespace MenuLayer
{
    public class ShipmentContaxt : BaseShipmentContaxt
    {
        public SqlConnection con = ConnectionBulder.con;
        public ShipmentContaxt()
        {
            base.Configuration.AutoDetectChangesEnabled = false;
            this.Configuration.ProxyCreationEnabled = false;
            this.Configuration.LazyLoadingEnabled = false;
        }


        public override List<ChartData> getChartDataVN(DateTime? datefrom, DateTime? dateto, string[] vesselname)
        {

            List<ChartData> notilist = new List<ChartData>();
            var emps1 = from em in CreReports.OrderBy(x => x.Dates).Where(x => vesselname.Contains(x.VesselName) && (x.Dates >= datefrom && x.Dates <= dateto)).ToList() group em by em.Position;
            foreach (var item in emps1)
            {
                ChartData nots = new ChartData();
                nots.VesselName = CreReports.Where(x => x.Position.Equals(item.Key)).FirstOrDefault().VesselName;


                nots.Deviation = Notifications.Where(x => vesselname.Contains(x.VesselName) && (x.NcDate >= datefrom && x.NcDate <= dateto) && x.Rank == item.Key && !string.IsNullOrEmpty(x.NonConfirmity)).ToList().Count();



                nots.Rank = item.Key;
                var workSum = (from emp in CreReports.Where(x => vesselname.Contains(x.VesselName) && (x.Dates >= datefrom && x.Dates <= dateto) && x.Position == item.Key)
                               select emp.TotalHours);
                nots.Work = workSum.Sum();
                var RestSum = (from emp in CreReports.Where(x => vesselname.Contains(x.VesselName) && (x.Dates >= datefrom && x.Dates <= dateto) && x.Position == item.Key)
                               select emp.RestHours).Sum();
                nots.Rest = RestSum;


                //nots.Months = item.Key.ToString();


                notilist.Add(nots);
            }


            return notilist;

        }

        public override List<ChartData> getChartDataVNRK(DateTime? datefrom, DateTime? dateto, string[] vesselname, string[] rank)
        {

            List<ChartData> notilist = new List<ChartData>();

            var emps1 = from em in CreReports.OrderBy(x => x.Dates).Where(x => vesselname.Contains(x.VesselName) && rank.Contains(x.Position) && (x.Dates >= datefrom && x.Dates <= dateto)).ToList() group em by em.Position;
            foreach (var item in emps1)
            {
                ChartData nots = new ChartData();
                nots.VesselName = CreReports.Where(x => vesselname.Contains(x.VesselName) && rank.Contains(x.Position)).FirstOrDefault().VesselName;

                //nots.Deviation = Notifications.Where(x => vesselname.Contains(x.VesselName) && (x.NcDate >= datefrom && x.NcDate <= dateto) && x.Rank == item.Key && !string.IsNullOrEmpty(x.NonConfirmity)).ToList().Count();



                nots.Rank = item.Key;
                var workSum = (from emp in CreReports.Where(x => vesselname.Contains(x.VesselName) && (x.Dates >= datefrom && x.Dates <= dateto) && x.Position == item.Key)
                               select emp.TotalHours);
                nots.Work = workSum.Sum();
                var RestSum = (from emp in CreReports.Where(x => vesselname.Contains(x.VesselName) && (x.Dates >= datefrom && x.Dates <= dateto) && x.Position == item.Key)
                               select emp.RestHours).Sum();
                nots.Rest = RestSum;


                //nots.Months = item.Key.ToString();


                notilist.Add(nots);
            }



            return notilist;

        }

        public override List<ChartData> getChartDataVNRK_Bar(DateTime? datefrom, DateTime? dateto, string[] vesselname, string[] rank, string[] FleetType, string[] FleetName)
        {

            List<ChartData> notilist = new List<ChartData>();
            ////List<NotificationClass> AllNotifications = null;// = new List<NotificationClass>();

            // var devi = from em in Notifications.OrderBy(x => x.NcDate).Where(x => vesselname.Contains(x.VesselName) && rank.Contains(x.Rank) && (x.NcDate >= datefrom && x.NcDate <= dateto)).ToList() group em by em.VesselName;
            //var devi = from em in Notifications.OrderBy(x => x.NcDate).ToList() group em by em.NonConfirmity ;

            //var Dev = (from em in Notifications
            //           group em by em.NonConfirmity into temp
            //           select new
            //           {
            //               T1 = temp.Key.Count(),
            //               T2 = temp
            //           }).ToList();

            int a = FleetType.Length;
            int b = FleetName.Length;

            //if (string.IsNullOrEmpty(FleetType[0]) && string.IsNullOrEmpty(FleetName[0]))
            //{
            //    // AllNotifications = Notifications.Where(x=> FleetType.Contains(x.FleetType) && FleetName.Contains(x.FleetName) && (x.NcDate >= datefrom && x.NcDate <= dateto)).ToList();
            //    // AllNotifications = Notifications.Where(x => vesselname.Contains(x.VesselName) && rank.Contains(x.Rank) && (x.NcDate >= datefrom && x.NcDate <= dateto)).ToList();

            //    AllNotifications = Notifications.Where(x => (x.NcDate >= datefrom && x.NcDate <= dateto)).ToList();
            //}
            //else
            //{
            //    AllNotifications = Notifications.Where(x => FleetType.Contains(x.FleetType) && FleetName.Contains(x.FleetName) && (x.NcDate >= datefrom && x.NcDate <= dateto)).ToList();
            //}
            //if (!string.IsNullOrEmpty(vesselname[0]))
            //{
            //   // AllNotifications = null;
            //    AllNotifications = AllNotifications.Where(x => vesselname.Contains(x.VesselName) && (x.NcDate >= datefrom && x.NcDate <= dateto)).ToList();
            //}
            //if (!string.IsNullOrEmpty(rank[0]))
            //{
            //    // by default all
            //    AllNotifications = AllNotifications.Where(x => rank.Contains(x.Rank) && (x.NcDate >= datefrom && x.NcDate <= dateto)).ToList();
            //}

           

            //var DDD = AllNotifications
            //       .GroupBy(p => p.NonConfirmity)
            //       .Select(g => new { Deavition = g.Key, count = g.Count() });

            //// var emps1 = from em in CreReports.OrderBy(x => x.Dates).Where(x => vesselname.Contains(x.VesselName) && rank.Contains(x.Position) && (x.Dates >= datefrom && x.Dates <= dateto)).ToList() group em by em.Position;
            //foreach (var item in DDD)
            //{
            //    ChartData nots = new ChartData();

            //    nots.Deviation = item.count;

            //    nots.DeviationType = item.Deavition;

            //    //Notifications.Where(x => vesselname.Contains(x.VesselName) && (x.NcDate >= datefrom && x.NcDate <= dateto) && x.Rank == item.Key && !string.IsNullOrEmpty(x.NonConfirmity)).ToList().Count();

            //    //nots.VesselName = CreReports.Where(x => vesselname.Contains(x.VesselName) && rank.Contains(x.Position)).FirstOrDefault().VesselName;

            //    //nots.Deviation = Notifications.Where(x => vesselname.Contains(x.VesselName) && (x.NcDate >= datefrom && x.NcDate <= dateto) && x.Rank == item.Key && !string.IsNullOrEmpty(x.NonConfirmity)).ToList().Count();



            //    //nots.Rank = item.Key;
            //    //var workSum = (from emp in CreReports.Where(x => vesselname.Contains(x.VesselName) && (x.Dates >= datefrom && x.Dates <= dateto) && x.Position == item.Key)
            //    //               select emp.TotalHours);
            //    //nots.Work = workSum.Sum();
            //    //var RestSum = (from emp in CreReports.Where(x => vesselname.Contains(x.VesselName) && (x.Dates >= datefrom && x.Dates <= dateto) && x.Position == item.Key)
            //    //               select emp.RestHours).Sum();
            //    //nots.Rest = RestSum;


            //    //nots.Months = item.Key.ToString();


            //    notilist.Add(nots);
            //}



            return notilist;

        }


        public override List<ChartData> getChartData(DateTime? datefrom, DateTime? dateto, int? noofday)
        {
            int days = DateTime.DaysInMonth(Convert.ToDateTime(datefrom).Year, Convert.ToDateTime(datefrom).Month);

            List<ChartData> notilist = new List<ChartData>();
            //if (!UserRole.CheckAnalysis)
            //{

            if (noofday >= 335)
            {
                //var emps2 = from em in CreReports.Where(x => (x.Dates >= datefrom && x.Dates <= dateto)).ToList() group em by em.Dates.Year;
                var emps2 = CreReports.Where(x => (x.Dates >= datefrom && x.Dates <= dateto)).GroupBy(s => new { VesselName = s.VesselName, Month = s.Dates.Year, Position = s.Position, FleetType = s.FleetType, FleetName = s.FleetName }).ToList();
                var Notification = Notifications.OrderByDescending(t => t.EDate).Where(x => !string.IsNullOrEmpty(x.NonConfirmity)).ToList();
                foreach (var item in emps2)
                {
                    ChartData nots = new ChartData();
                    //nots.VesselId = CreReports.OrderByDescending(t => t.ImportDate).Where(x => x.Dates.Year == item.Key).FirstOrDefault().Vessel_ID;
                    //nots.VesselName = CreReports.OrderByDescending(t => t.ImportDate).Where(x => x.Dates.Year == item.Key).FirstOrDefault().VesselName;
                    //nots.Rank = CreReports.OrderByDescending(t => t.ImportDate).Where(x => x.Dates.Year == item.Key).FirstOrDefault().Position;
                    //nots.Deviation = Notifications.OrderByDescending(t => t.EDate).Where(x => x.NcDate.Year == item.Key && !string.IsNullOrEmpty(x.NonConfirmity)).ToList().Count();

                    nots.VesselId = item.OrderByDescending(t => t.ImportDate).Where(x => x.VesselName == item.Key.VesselName).FirstOrDefault().Vessel_ID;
                    nots.VesselName = item.Key.VesselName;
                    nots.FleetType = item.Key.FleetType;
                    nots.FleetName = item.Key.FleetName;
                    nots.Months1 = item.Where(x => (x.Dates >= datefrom && x.Dates <= dateto) && x.Dates.Year == item.Key.Month).FirstOrDefault().Dates;
                    nots.Deviation = Notification.Where(x => x.NcDate.Year == item.Key.Month && x.VesselName == item.Key.VesselName && x.Rank == item.Key.Position && x.FleetType == item.Key.FleetType && x.FleetName == item.Key.FleetName).ToList().Count();
                    nots.Rank = item.Key.Position;
                    


                    //var workSum = (from emp in CreReports.OrderByDescending(t => t.ImportDate).Where(x => x.Dates.Year == item.Key)
                    //               select emp.TotalHours).Sum();
                    //nots.Work = workSum;
                    //var RestSum = (from emp in CreReports.OrderByDescending(t => t.ImportDate).Where(x => x.Dates.Year == item.Key)
                    //               select emp.RestHours).Sum();
                    //nots.Rest = RestSum;


                    var workSum = (from emp in item.OrderByDescending(t => t.ImportDate)
                                   select emp.TotalHours).Sum();
                    nots.Work = workSum;
                    var RestSum = (from emp in item.OrderByDescending(t => t.ImportDate)
                                   select emp.RestHours).Sum();
                    nots.Rest = RestSum;



                    //var monthnumber = sc1.CreReports.OrderByDescending(t => t.ImportDate).Where(x => x.Dates.Month == item.Key).FirstOrDefault().Dates.Month;

                    nots.Months = item.Key.Month.ToString();
                    notilist = notilist.OrderBy(m => m.Months1).ToList();
                    notilist.Add(nots);
                }

            }
            else if (noofday > days && noofday < 335)
            {
                //var emps = from em in CreReports.Where(x => (x.Dates >= datefrom && x.Dates <= dateto)).ToList() group em by em.Dates.Month;
                var emps = CreReports.Where(x => (x.Dates >= datefrom && x.Dates <= dateto)).GroupBy(s => new { VesselName = s.VesselName, Month = s.Dates.Month, Position = s.Position, FleetType = s.FleetType, FleetName = s.FleetName }).ToList();
                //var Notification = Notifications.OrderByDescending(t => t.EDate).Where(x => !string.IsNullOrEmpty(x.NonConfirmity)).ToList();

                foreach (var item in emps)
                {


                    ChartData nots = new ChartData();
                    //nots.VesselName = CreReports.OrderByDescending(t => t.ImportDate).Where(x => x.Dates.Month == item.Key).FirstOrDefault().VesselName;
                    //nots.Months1 = CreReports.Where(x => (x.Dates >= datefrom && x.Dates <= dateto) && x.Dates.Month == item.Key).FirstOrDefault().Dates;

                    nots.VesselName = item.Key.VesselName;
                    nots.FleetType = item.Key.FleetType;
                    nots.FleetName = item.Key.FleetName;
                    nots.Months1 = item.Where(x => (x.Dates >= datefrom && x.Dates <= dateto) && x.Dates.Month == item.Key.Month).FirstOrDefault().Dates;
                    //nots.Deviation = Notification.Where(x => x.NcDate.Year == nots.Months1.Year && x.NcDate.Month == item.Key.Month && x.VesselName == item.Key.VesselName && x.Rank == item.Key.Position && x.FleetType == item.Key.FleetType && x.FleetName == item.Key.FleetName).ToList().Count();
                    //nots.Rank = CreReports.OrderByDescending(t => t.ImportDate).Where(x => x.Dates.Month == item.Key).FirstOrDefault().Position;
                    nots.Rank = item.Key.Position;

                    var workSum = (from emp in item.OrderByDescending(t => t.ImportDate)
                                   select emp.TotalHours).Sum();
                    nots.Work = workSum;
                    var RestSum = (from emp in item.OrderByDescending(t => t.ImportDate)
                                   select emp.RestHours).Sum();
                    nots.Rest = RestSum;

                    //var monthnumber = sc1.CreReports.OrderByDescending(t => t.ImportDate).Where(x => x.Dates.Month == item.Key).FirstOrDefault().Dates.Month;

                    nots.Months = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(item.Key.Month);

                    notilist.Add(nots);
                }

                //notilist = notilist.OrderBy(m => DateTime.ParseExact(m.Months, "MMMM", CultureInfo.InvariantCulture)).ToList();

                notilist = notilist.OrderBy(m => m.Months1).ToList();

            }
            else
            {

                //var bb1 = Convert.ToDateTime(datefrom).Month;
                //var bb2 = Convert.ToDateTime(dateto).Month;

                //if (Convert.ToDateTime(datefrom).Month.Equals(Convert.ToDateTime(dateto).Month))
                //{
                //var emps1 = from em in CreReports.OrderBy(x => x.Dates).Where(x => (x.Dates >= datefrom && x.Dates <= dateto)).ToList() group em by em.Dates.Day;
                var emps1 = CreReports.Where(x => (x.Dates >= datefrom && x.Dates <= dateto)).GroupBy(s => new { s.VesselName, s.Dates.Day, Position = s.Position, FleetType = s.FleetType, FleetName = s.FleetName }).ToList();
                //var Notification = Notifications.OrderByDescending(t => t.EDate).Where(x => !string.IsNullOrEmpty(x.NonConfirmity)).ToList();


                foreach (var item in emps1)
                {
                    ChartData nots = new ChartData();
                    //nots.VesselName = CreReports.Where(x => x.Dates.Day.Equals(item.Key)).FirstOrDefault().VesselName;
                    //nots.Deviation = Notifications.Where(x => x.NcDate.Day == item.Key && !string.IsNullOrEmpty(x.NonConfirmity)).ToList().Count();
                    //nots.Rank = CreReports.Where(x => x.Dates.Day == item.Key).FirstOrDefault().Position;

                    nots.VesselName = item.Key.VesselName;
                    nots.FleetType = item.Key.FleetType;
                    nots.FleetName = item.Key.FleetName;
                    nots.Months1 = item.Where(x => (x.Dates >= datefrom && x.Dates <= dateto) && x.Dates.Day == item.Key.Day).FirstOrDefault().Dates;
                    //nots.Deviation = Notification.Where(x => x.NcDate == nots.Months1 && x.VesselName == item.Key.VesselName && x.Rank == item.Key.Position && x.FleetType == item.Key.FleetType && x.FleetName == item.Key.FleetName).ToList().Count();
                    nots.Rank = item.Key.Position;


                    //var workSum = (from emp in CreReports.Where(x => x.Dates.Day == item.Key)
                    //               select emp.TotalHours);
                    //nots.Work = workSum.Sum();
                    //var RestSum = (from emp in CreReports.Where(x => x.Dates.Day == item.Key)
                    //               select emp.RestHours).Sum();
                    //nots.Rest = RestSum;

                    var workSum = (from emp in item.OrderByDescending(t => t.ImportDate)
                                   select emp.TotalHours).Sum();
                    nots.Work = workSum;
                    var RestSum = (from emp in item.OrderByDescending(t => t.ImportDate)
                                   select emp.RestHours).Sum();
                    nots.Rest = RestSum;

                    nots.Months = item.Key.Day.ToString();

                    notilist = notilist.OrderBy(m => m.Months1).ToList();

                    notilist.Add(nots);
                }
                //}
                //else
                //{
                //    var emps = from em in CreReports.Where(x => (x.Dates >= datefrom && x.Dates <= dateto)).ToList() group em by em.Dates.Month;
                //    foreach (var item in emps)
                //    {
                //        ChartData nots = new ChartData();
                //        nots.VesselName = CreReports.OrderByDescending(t => t.ImportDate).Where(x => x.Dates.Month == item.Key).FirstOrDefault().VesselName;
                //        nots.Deviation = CreReports.OrderByDescending(t => t.ImportDate).Where(x => x.Dates.Month == item.Key && x.NonConfirmities != "").ToList().Count();
                //        nots.Rank = CreReports.OrderByDescending(t => t.ImportDate).Where(x => x.Dates.Month == item.Key).FirstOrDefault().Position;
                //        var workSum = (from emp in CreReports.OrderByDescending(t => t.ImportDate).Where(x => x.Dates.Month == item.Key)
                //                       select emp.TotalHours).Sum();
                //        nots.Work = workSum;
                //        var RestSum = (from emp in CreReports.OrderByDescending(t => t.ImportDate).Where(x => x.Dates.Month == item.Key)
                //                       select emp.RestHours).Sum();
                //        nots.Rest = RestSum;

                //        //var monthnumber = sc1.CreReports.OrderByDescending(t => t.ImportDate).Where(x => x.Dates.Month == item.Key).FirstOrDefault().Dates.Month;

                //        nots.Months = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(item.Key);

                //        notilist.Add(nots);
                //    }

                //    notilist = notilist.OrderBy(m => DateTime.ParseExact(m.Months, "MMMM", CultureInfo.InvariantCulture)).ToList();

                //}
            }
            return notilist;


        }


        /*
        public override List<ChartData> getChartDataVNRK(DateTime? datefrom, DateTime? dateto, string[] vesselname, string[] rank, int? noofday)
        {
            int days = DateTime.DaysInMonth(Convert.ToDateTime(datefrom).Year, Convert.ToDateTime(datefrom).Month);

            List<ChartData> notilist = new List<ChartData>();

            if (noofday >= 335)
            {
                var emps = from em in CreReports.Where(x => vesselname.Contains(x.VesselName) && rank.Contains(x.Position) && (x.Dates >= datefrom && x.Dates <= dateto)).ToList() group em by em.Dates.Year;
                foreach (var item in emps)
                {
                    ChartData nots = new ChartData();
                    nots.VesselName = CreReports.OrderByDescending(t => t.ImportDate).Where(x => vesselname.Contains(x.VesselName) && rank.Contains(x.Position) && x.Dates.Year == item.Key).FirstOrDefault().VesselName;
                    nots.Deviation = CreReports.OrderByDescending(t => t.ImportDate).Where(x => vesselname.Contains(x.VesselName) && rank.Contains(x.Position) && x.Dates.Year == item.Key && x.NonConfirmities != "").ToList().Count();
                    nots.Rank = CreReports.OrderByDescending(t => t.ImportDate).Where(x => vesselname.Contains(x.VesselName) && rank.Contains(x.Position) && x.Dates.Year == item.Key).FirstOrDefault().Position;
                    var workSum = (from emp in CreReports.OrderByDescending(t => t.ImportDate).Where(x => vesselname.Contains(x.VesselName) && rank.Contains(x.Position) && x.Dates.Year == item.Key)
                                   select emp.TotalHours).Sum();
                    nots.Work = workSum;
                    var RestSum = (from emp in CreReports.OrderByDescending(t => t.ImportDate).Where(x => vesselname.Contains(x.VesselName) && rank.Contains(x.Position) && x.Dates.Year == item.Key)
                                   select emp.RestHours).Sum();
                    nots.Rest = RestSum;

                    nots.Months = item.Key.ToString();

                    notilist.Add(nots);
                }
            }
            else if (noofday > days && noofday < 335)
            {
                var emps = from em in CreReports.Where(x => vesselname.Contains(x.VesselName) && rank.Contains(x.Position) && (x.Dates >= datefrom && x.Dates <= dateto)).ToList() group em by em.Dates.Month;
                foreach (var item in emps)
                {
                    ChartData nots = new ChartData();
                    nots.VesselName = CreReports.OrderByDescending(t => t.ImportDate).Where(x => vesselname.Contains(x.VesselName) && rank.Contains(x.Position) && x.Dates.Month == item.Key).FirstOrDefault().VesselName;
                    nots.Deviation = CreReports.OrderByDescending(t => t.ImportDate).Where(x => vesselname.Contains(x.VesselName) && rank.Contains(x.Position) && x.Dates.Month == item.Key && x.NonConfirmities != "").ToList().Count();
                    nots.Rank = CreReports.OrderByDescending(t => t.ImportDate).Where(x => vesselname.Contains(x.VesselName) && rank.Contains(x.Position) && x.Dates.Month == item.Key).FirstOrDefault().Position;
                    var workSum = (from emp in CreReports.OrderByDescending(t => t.ImportDate).Where(x => vesselname.Contains(x.VesselName) && rank.Contains(x.Position) && x.Dates.Month == item.Key)
                                   select emp.TotalHours).Sum();
                    nots.Work = workSum;
                    var RestSum = (from emp in CreReports.OrderByDescending(t => t.ImportDate).Where(x => vesselname.Contains(x.VesselName) && rank.Contains(x.Position) && x.Dates.Month == item.Key)
                                   select emp.RestHours).Sum();
                    nots.Rest = RestSum;

                    //var monthnumber = sc1.CreReports.OrderByDescending(t => t.ImportDate).Where(x => x.Dates.Month == item.Key).FirstOrDefault().Dates.Month;

                    nots.Months = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(item.Key);

                    notilist.Add(nots);
                }

                notilist = notilist.OrderBy(m => DateTime.ParseExact(m.Months, "MMMM", CultureInfo.InvariantCulture)).ToList();

            }
            else
            {
                if (Convert.ToDateTime(datefrom).Month.Equals(Convert.ToDateTime(dateto).Month))
                {
                    var emps1 = from em in CreReports.OrderBy(x => x.Dates).Where(x => vesselname.Contains(x.VesselName) && rank.Contains(x.Position) && (x.Dates >= datefrom && x.Dates <= dateto)).ToList() group em by em.Dates.Day;
                    foreach (var item in emps1)
                    {
                        ChartData nots = new ChartData();
                        nots.VesselName = CreReports.Where(x => vesselname.Contains(x.VesselName) && rank.Contains(x.Position) && x.Dates.Day.Equals(item.Key)).FirstOrDefault().VesselName;
                        nots.Deviation = CreReports.Where(x => vesselname.Contains(x.VesselName) && rank.Contains(x.Position) && x.Dates.Day == item.Key && x.NonConfirmities != "").ToList().Count();
                        nots.Rank = CreReports.Where(x => vesselname.Contains(x.VesselName) && rank.Contains(x.Position) && x.Dates.Day == item.Key).FirstOrDefault().Position;
                        var workSum = (from emp in CreReports.Where(x => vesselname.Contains(x.VesselName) && rank.Contains(x.Position) && x.Dates.Day == item.Key)
                                       select emp.TotalHours);
                        nots.Work = workSum.Sum();
                        var RestSum = (from emp in CreReports.Where(x => vesselname.Contains(x.VesselName) && rank.Contains(x.Position) && x.Dates.Day == item.Key)
                                       select emp.RestHours).Sum();
                        nots.Rest = RestSum;


                        nots.Months = item.Key.ToString();


                        notilist.Add(nots);
                    }
                }
                else
                {
                    var emps = from em in CreReports.Where(x => vesselname.Contains(x.VesselName) && rank.Contains(x.Position) && (x.Dates >= datefrom && x.Dates <= dateto)).ToList() group em by em.Dates.Month;
                    foreach (var item in emps)
                    {
                        ChartData nots = new ChartData();
                        nots.VesselName = CreReports.OrderByDescending(t => t.ImportDate).Where(x => vesselname.Contains(x.VesselName) && rank.Contains(x.Position) && x.Dates.Month == item.Key).FirstOrDefault().VesselName;
                        nots.Deviation = CreReports.OrderByDescending(t => t.ImportDate).Where(x => vesselname.Contains(x.VesselName) && rank.Contains(x.Position) && x.Dates.Month == item.Key && x.NonConfirmities != "").ToList().Count();
                        nots.Rank = CreReports.OrderByDescending(t => t.ImportDate).Where(x => vesselname.Contains(x.VesselName) && rank.Contains(x.Position) && x.Dates.Month == item.Key).FirstOrDefault().Position;
                        var workSum = (from emp in CreReports.OrderByDescending(t => t.ImportDate).Where(x => vesselname.Contains(x.VesselName) && rank.Contains(x.Position) && x.Dates.Month == item.Key)
                                       select emp.TotalHours).Sum();
                        nots.Work = workSum;
                        var RestSum = (from emp in CreReports.OrderByDescending(t => t.ImportDate).Where(x => vesselname.Contains(x.VesselName) && rank.Contains(x.Position) && x.Dates.Month == item.Key)
                                       select emp.RestHours).Sum();
                        nots.Rest = RestSum;

                        //var monthnumber = sc1.CreReports.OrderByDescending(t => t.ImportDate).Where(x => x.Dates.Month == item.Key).FirstOrDefault().Dates.Month;

                        nots.Months = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(item.Key);

                        notilist.Add(nots);
                    }

                    notilist = notilist.OrderBy(m => DateTime.ParseExact(m.Months, "MMMM", CultureInfo.InvariantCulture)).ToList();


                }


            }


            return notilist;

        }
        
        public override List<ChartData> getChartDataFT(DateTime? datefrom, DateTime? dateto, string[] fleettype, int? noofday)
        {
            int days = DateTime.DaysInMonth(Convert.ToDateTime(datefrom).Year, Convert.ToDateTime(datefrom).Month);
            List<ChartData> notilist = new List<ChartData>();

            if (noofday >= 335)
            {
                var emps = from em in CreReports.Where(x => fleettype.Contains(x.FleetType) && (x.Dates >= datefrom && x.Dates <= dateto)).ToList() group em by em.Dates.Year;
                foreach (var item in emps)
                {
                    ChartData nots = new ChartData();
                    nots.VesselName = CreReports.OrderByDescending(t => t.ImportDate).Where(x => x.Dates.Year == item.Key).FirstOrDefault().VesselName;
                    nots.Deviation = CreReports.OrderByDescending(t => t.ImportDate).Where(x => fleettype.Contains(x.FleetType) && x.Dates.Year == item.Key && x.NonConfirmities != "").ToList().Count();
                    nots.Rank = CreReports.OrderByDescending(t => t.ImportDate).Where(x => x.Dates.Year == item.Key).FirstOrDefault().Position;
                    var workSum = (from emp in CreReports.OrderByDescending(t => t.ImportDate).Where(x => fleettype.Contains(x.FleetType) && x.Dates.Year == item.Key)
                                   select emp.TotalHours).Sum();
                    nots.Work = workSum;
                    var RestSum = (from emp in CreReports.OrderByDescending(t => t.ImportDate).Where(x => fleettype.Contains(x.FleetType) && x.Dates.Year == item.Key)
                                   select emp.RestHours).Sum();
                    nots.Rest = RestSum;

                    nots.Months = item.Key.ToString();

                    notilist.Add(nots);
                }

            }
            else if (noofday > days && noofday < 335)
            {
                var emps = from em in CreReports.Where(x => fleettype.Contains(x.FleetType) && (x.Dates >= datefrom && x.Dates <= dateto)).ToList() group em by em.Dates.Month;
                foreach (var item in emps)
                {
                    ChartData nots = new ChartData();
                    nots.VesselName = CreReports.OrderByDescending(t => t.ImportDate).Where(x => x.Dates.Month == item.Key).FirstOrDefault().VesselName;
                    nots.Deviation = CreReports.OrderByDescending(t => t.ImportDate).Where(x => fleettype.Contains(x.FleetType) && x.Dates.Month == item.Key && x.NonConfirmities != "").ToList().Count();
                    nots.Rank = CreReports.OrderByDescending(t => t.ImportDate).Where(x => x.Dates.Month == item.Key).FirstOrDefault().Position;
                    var workSum = (from emp in CreReports.OrderByDescending(t => t.ImportDate).Where(x => fleettype.Contains(x.FleetType) && x.Dates.Month == item.Key)
                                   select emp.TotalHours).Sum();
                    nots.Work = workSum;
                    var RestSum = (from emp in CreReports.OrderByDescending(t => t.ImportDate).Where(x => fleettype.Contains(x.FleetType) && x.Dates.Month == item.Key)
                                   select emp.RestHours).Sum();
                    nots.Rest = RestSum;

                    //var monthnumber = sc1.CreReports.OrderByDescending(t => t.ImportDate).Where(x => x.Dates.Month == item.Key).FirstOrDefault().Dates.Month;

                    nots.Months = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(item.Key);

                    notilist.Add(nots);
                }

                notilist = notilist.OrderBy(m => DateTime.ParseExact(m.Months, "MMMM", CultureInfo.InvariantCulture)).ToList();

            }
            else
            {
                if (Convert.ToDateTime(datefrom).Month.Equals(Convert.ToDateTime(dateto).Month))
                {
                    var emps1 = from em in CreReports.OrderBy(x => x.Dates).Where(x => fleettype.Contains(x.FleetType) && (x.Dates >= datefrom && x.Dates <= dateto)).ToList() group em by em.Dates.Day;
                    foreach (var item in emps1)
                    {
                        ChartData nots = new ChartData();
                        nots.VesselName = CreReports.Where(x => x.Dates.Day.Equals(item.Key)).FirstOrDefault().VesselName;
                        nots.Deviation = CreReports.Where(x => fleettype.Contains(x.FleetType) && x.Dates.Day == item.Key && x.NonConfirmities != "").ToList().Count();
                        nots.Rank = CreReports.Where(x => x.Dates.Day == item.Key).FirstOrDefault().Position;
                        var workSum = (from emp in CreReports.Where(x => fleettype.Contains(x.FleetType) && x.Dates.Day == item.Key)
                                       select emp.TotalHours);
                        nots.Work = workSum.Sum();
                        var RestSum = (from emp in CreReports.Where(x => fleettype.Contains(x.FleetType) && x.Dates.Day == item.Key)
                                       select emp.RestHours).Sum();
                        nots.Rest = RestSum;


                        nots.Months = item.Key.ToString();


                        notilist.Add(nots);
                    }
                }
                else
                {
                    var emps = from em in CreReports.Where(x => fleettype.Contains(x.FleetType) && (x.Dates >= datefrom && x.Dates <= dateto)).ToList() group em by em.Dates.Month;
                    foreach (var item in emps)
                    {
                        ChartData nots = new ChartData();
                        nots.VesselName = CreReports.OrderByDescending(t => t.ImportDate).Where(x => x.Dates.Month == item.Key).FirstOrDefault().VesselName;
                        nots.Deviation = CreReports.OrderByDescending(t => t.ImportDate).Where(x => fleettype.Contains(x.FleetType) && x.Dates.Month == item.Key && x.NonConfirmities != "").ToList().Count();
                        nots.Rank = CreReports.OrderByDescending(t => t.ImportDate).Where(x => x.Dates.Month == item.Key).FirstOrDefault().Position;
                        var workSum = (from emp in CreReports.OrderByDescending(t => t.ImportDate).Where(x => fleettype.Contains(x.FleetType) && x.Dates.Month == item.Key)
                                       select emp.TotalHours).Sum();
                        nots.Work = workSum;
                        var RestSum = (from emp in CreReports.OrderByDescending(t => t.ImportDate).Where(x => fleettype.Contains(x.FleetType) && x.Dates.Month == item.Key)
                                       select emp.RestHours).Sum();
                        nots.Rest = RestSum;

                        //var monthnumber = sc1.CreReports.OrderByDescending(t => t.ImportDate).Where(x => x.Dates.Month == item.Key).FirstOrDefault().Dates.Month;

                        nots.Months = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(item.Key);

                        notilist.Add(nots);
                    }

                    notilist = notilist.OrderBy(m => DateTime.ParseExact(m.Months, "MMMM", CultureInfo.InvariantCulture)).ToList();

                }

            }



            return notilist;

        }

        public override List<ChartData> getChartDataFN(DateTime? datefrom, DateTime? dateto, string[] fleetname, int? noofday)
        {
            int days = DateTime.DaysInMonth(Convert.ToDateTime(datefrom).Year, Convert.ToDateTime(datefrom).Month);
            List<ChartData> notilist = new List<ChartData>();

            if (noofday >= 335)
            {
                var emps = from em in CreReports.Where(x => fleetname.Contains(x.FleetName) && (x.Dates >= datefrom && x.Dates <= dateto)).ToList() group em by em.Dates.Year;
                foreach (var item in emps)
                {
                    ChartData nots = new ChartData();
                    nots.VesselName = CreReports.OrderByDescending(t => t.ImportDate).Where(x => x.Dates.Year == item.Key).FirstOrDefault().VesselName;
                    nots.Deviation = CreReports.OrderByDescending(t => t.ImportDate).Where(x => fleetname.Contains(x.FleetName) && x.Dates.Year == item.Key && x.NonConfirmities != "").ToList().Count();
                    nots.Rank = CreReports.OrderByDescending(t => t.ImportDate).Where(x => x.Dates.Year == item.Key).FirstOrDefault().Position;
                    var workSum = (from emp in CreReports.OrderByDescending(t => t.ImportDate).Where(x => fleetname.Contains(x.FleetName) && x.Dates.Year == item.Key)
                                   select emp.TotalHours).Sum();
                    nots.Work = workSum;
                    var RestSum = (from emp in CreReports.OrderByDescending(t => t.ImportDate).Where(x => fleetname.Contains(x.FleetName) && x.Dates.Year == item.Key)
                                   select emp.RestHours).Sum();
                    nots.Rest = RestSum;

                    nots.Months = item.Key.ToString();

                    notilist.Add(nots);
                }

            }
            else if (noofday > days && noofday < 335)
            {
                var emps = from em in CreReports.Where(x => fleetname.Contains(x.FleetName) && (x.Dates >= datefrom && x.Dates <= dateto)).ToList() group em by em.Dates.Month;
                foreach (var item in emps)
                {
                    ChartData nots = new ChartData();
                    nots.VesselName = CreReports.OrderByDescending(t => t.ImportDate).Where(x => x.Dates.Month == item.Key).FirstOrDefault().VesselName;
                    nots.Deviation = CreReports.OrderByDescending(t => t.ImportDate).Where(x => fleetname.Contains(x.FleetName) && x.Dates.Month == item.Key && x.NonConfirmities != "").ToList().Count();
                    nots.Rank = CreReports.OrderByDescending(t => t.ImportDate).Where(x => x.Dates.Month == item.Key).FirstOrDefault().Position;
                    var workSum = (from emp in CreReports.OrderByDescending(t => t.ImportDate).Where(x => fleetname.Contains(x.FleetName) && x.Dates.Month == item.Key)
                                   select emp.TotalHours).Sum();
                    nots.Work = workSum;
                    var RestSum = (from emp in CreReports.OrderByDescending(t => t.ImportDate).Where(x => fleetname.Contains(x.FleetName) && x.Dates.Month == item.Key)
                                   select emp.RestHours).Sum();
                    nots.Rest = RestSum;

                    //var monthnumber = sc1.CreReports.OrderByDescending(t => t.ImportDate).Where(x => x.Dates.Month == item.Key).FirstOrDefault().Dates.Month;

                    nots.Months = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(item.Key);

                    notilist.Add(nots);
                }

                notilist = notilist.OrderBy(m => DateTime.ParseExact(m.Months, "MMMM", CultureInfo.InvariantCulture)).ToList();

            }
            else
            {
                if (Convert.ToDateTime(datefrom).Month.Equals(Convert.ToDateTime(dateto).Month))
                {
                    var emps1 = from em in CreReports.OrderBy(x => x.Dates).Where(x => fleetname.Contains(x.FleetName) && (x.Dates >= datefrom && x.Dates <= dateto)).ToList() group em by em.Dates.Day;
                    foreach (var item in emps1)
                    {
                        ChartData nots = new ChartData();
                        nots.VesselName = CreReports.Where(x => x.Dates.Day.Equals(item.Key)).FirstOrDefault().VesselName;
                        nots.Deviation = CreReports.Where(x => fleetname.Contains(x.FleetName) && x.Dates.Day == item.Key && x.NonConfirmities != "").ToList().Count();
                        nots.Rank = CreReports.Where(x => x.Dates.Day == item.Key).FirstOrDefault().Position;
                        var workSum = (from emp in CreReports.Where(x => fleetname.Contains(x.FleetName) && x.Dates.Day == item.Key)
                                       select emp.TotalHours);
                        nots.Work = workSum.Sum();
                        var RestSum = (from emp in CreReports.Where(x => fleetname.Contains(x.FleetName) && x.Dates.Day == item.Key)
                                       select emp.RestHours).Sum();
                        nots.Rest = RestSum;


                        nots.Months = item.Key.ToString();


                        notilist.Add(nots);
                    }
                }
                else
                {
                    var emps = from em in CreReports.Where(x => fleetname.Contains(x.FleetName) && (x.Dates >= datefrom && x.Dates <= dateto)).ToList() group em by em.Dates.Month;
                    foreach (var item in emps)
                    {
                        ChartData nots = new ChartData();
                        nots.VesselName = CreReports.OrderByDescending(t => t.ImportDate).Where(x => x.Dates.Month == item.Key).FirstOrDefault().VesselName;
                        nots.Deviation = CreReports.OrderByDescending(t => t.ImportDate).Where(x => fleetname.Contains(x.FleetName) && x.Dates.Month == item.Key && x.NonConfirmities != "").ToList().Count();
                        nots.Rank = CreReports.OrderByDescending(t => t.ImportDate).Where(x => x.Dates.Month == item.Key).FirstOrDefault().Position;
                        var workSum = (from emp in CreReports.OrderByDescending(t => t.ImportDate).Where(x => fleetname.Contains(x.FleetName) && x.Dates.Month == item.Key)
                                       select emp.TotalHours).Sum();
                        nots.Work = workSum;
                        var RestSum = (from emp in CreReports.OrderByDescending(t => t.ImportDate).Where(x => fleetname.Contains(x.FleetName) && x.Dates.Month == item.Key)
                                       select emp.RestHours).Sum();
                        nots.Rest = RestSum;

                        //var monthnumber = sc1.CreReports.OrderByDescending(t => t.ImportDate).Where(x => x.Dates.Month == item.Key).FirstOrDefault().Dates.Month;

                        nots.Months = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(item.Key);

                        notilist.Add(nots);
                    }

                    notilist = notilist.OrderBy(m => DateTime.ParseExact(m.Months, "MMMM", CultureInfo.InvariantCulture)).ToList();


                }
            }



            return notilist;
        }

        public override List<ChartData> getChartDataVN(DateTime? datefrom, DateTime? dateto, string[] vesselname, int? noofday)
        {
            int days = DateTime.DaysInMonth(Convert.ToDateTime(datefrom).Year, Convert.ToDateTime(datefrom).Month);
            List<ChartData> notilist = new List<ChartData>();

            if (noofday >= 335)
            {
                var emps = from em in CreReports.Where(x => vesselname.Contains(x.VesselName) && (x.Dates >= datefrom && x.Dates <= dateto)).ToList() group em by em.Dates.Year;
                foreach (var item in emps)
                {
                    ChartData nots = new ChartData();
                    nots.VesselName = CreReports.OrderByDescending(t => t.ImportDate).Where(x => x.Dates.Year == item.Key).FirstOrDefault().VesselName;
                    nots.Deviation = CreReports.OrderByDescending(t => t.ImportDate).Where(x => vesselname.Contains(x.VesselName) && x.Dates.Year == item.Key && x.NonConfirmities != "").ToList().Count();
                    nots.Rank = CreReports.OrderByDescending(t => t.ImportDate).Where(x => x.Dates.Year == item.Key).FirstOrDefault().Position;
                    var workSum = (from emp in CreReports.OrderByDescending(t => t.ImportDate).Where(x => vesselname.Contains(x.VesselName) && x.Dates.Year == item.Key)
                                   select emp.TotalHours).Sum();
                    nots.Work = workSum;
                    var RestSum = (from emp in CreReports.OrderByDescending(t => t.ImportDate).Where(x => vesselname.Contains(x.VesselName) && x.Dates.Year == item.Key)
                                   select emp.RestHours).Sum();
                    nots.Rest = RestSum;

                    nots.Months = item.Key.ToString();

                    notilist.Add(nots);
                }

            }
            else if (noofday > days && noofday < 335)
            {
                var emps = from em in CreReports.Where(x => vesselname.Contains(x.VesselName) && (x.Dates >= datefrom && x.Dates <= dateto)).ToList() group em by em.Dates.Month;
                foreach (var item in emps)
                {
                    ChartData nots = new ChartData();
                    nots.VesselName = CreReports.OrderByDescending(t => t.ImportDate).Where(x => x.Dates.Month == item.Key).FirstOrDefault().VesselName;
                    nots.Deviation = CreReports.OrderByDescending(t => t.ImportDate).Where(x => vesselname.Contains(x.VesselName) && x.Dates.Month == item.Key && x.NonConfirmities != "").ToList().Count();
                    nots.Rank = CreReports.OrderByDescending(t => t.ImportDate).Where(x => x.Dates.Month == item.Key).FirstOrDefault().Position;
                    var workSum = (from emp in CreReports.OrderByDescending(t => t.ImportDate).Where(x => vesselname.Contains(x.VesselName) && x.Dates.Month == item.Key)
                                   select emp.TotalHours).Sum();
                    nots.Work = workSum;
                    var RestSum = (from emp in CreReports.OrderByDescending(t => t.ImportDate).Where(x => vesselname.Contains(x.VesselName) && x.Dates.Month == item.Key)
                                   select emp.RestHours).Sum();
                    nots.Rest = RestSum;

                    //var monthnumber = sc1.CreReports.OrderByDescending(t => t.ImportDate).Where(x => x.Dates.Month == item.Key).FirstOrDefault().Dates.Month;

                    nots.Months = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(item.Key);

                    notilist.Add(nots);
                }
                notilist = notilist.OrderBy(m => DateTime.ParseExact(m.Months, "MMMM", CultureInfo.InvariantCulture)).ToList();

            }
            else
            {
                if (Convert.ToDateTime(datefrom).Month.Equals(Convert.ToDateTime(dateto).Month))
                {
                    var emps1 = from em in CreReports.OrderBy(x => x.Dates).Where(x => vesselname.Contains(x.VesselName) && (x.Dates >= datefrom && x.Dates <= dateto)).ToList() group em by em.Dates.Day;
                    foreach (var item in emps1)
                    {
                        ChartData nots = new ChartData();
                        nots.VesselName = CreReports.Where(x => x.Dates.Day.Equals(item.Key)).FirstOrDefault().VesselName;
                        nots.Deviation = CreReports.Where(x => vesselname.Contains(x.VesselName) && x.Dates.Day == item.Key && x.NonConfirmities != "").ToList().Count();
                        nots.Rank = CreReports.Where(x => x.Dates.Day == item.Key).FirstOrDefault().Position;
                        var workSum = (from emp in CreReports.Where(x => vesselname.Contains(x.VesselName) && x.Dates.Day == item.Key)
                                       select emp.TotalHours);
                        nots.Work = workSum.Sum();
                        var RestSum = (from emp in CreReports.Where(x => vesselname.Contains(x.VesselName) && x.Dates.Day == item.Key)
                                       select emp.RestHours).Sum();
                        nots.Rest = RestSum;


                        nots.Months = item.Key.ToString();


                        notilist.Add(nots);
                    }
                }
                else
                {
                    var emps = from em in CreReports.Where(x => vesselname.Contains(x.VesselName) && (x.Dates >= datefrom && x.Dates <= dateto)).ToList() group em by em.Dates.Month;
                    foreach (var item in emps)
                    {
                        ChartData nots = new ChartData();
                        nots.VesselName = CreReports.OrderByDescending(t => t.ImportDate).Where(x => x.Dates.Month == item.Key).FirstOrDefault().VesselName;
                        nots.Deviation = CreReports.OrderByDescending(t => t.ImportDate).Where(x => vesselname.Contains(x.VesselName) && x.Dates.Month == item.Key && x.NonConfirmities != "").ToList().Count();
                        nots.Rank = CreReports.OrderByDescending(t => t.ImportDate).Where(x => x.Dates.Month == item.Key).FirstOrDefault().Position;
                        var workSum = (from emp in CreReports.OrderByDescending(t => t.ImportDate).Where(x => vesselname.Contains(x.VesselName) && x.Dates.Month == item.Key)
                                       select emp.TotalHours).Sum();
                        nots.Work = workSum;
                        var RestSum = (from emp in CreReports.OrderByDescending(t => t.ImportDate).Where(x => vesselname.Contains(x.VesselName) && x.Dates.Month == item.Key)
                                       select emp.RestHours).Sum();
                        nots.Rest = RestSum;

                        //var monthnumber = sc1.CreReports.OrderByDescending(t => t.ImportDate).Where(x => x.Dates.Month == item.Key).FirstOrDefault().Dates.Month;

                        nots.Months = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(item.Key);

                        notilist.Add(nots);
                    }
                    notilist = notilist.OrderBy(m => DateTime.ParseExact(m.Months, "MMMM", CultureInfo.InvariantCulture)).ToList();

                }
            }

            return notilist;

        }

        public override List<ChartData> getChartDataRK(DateTime? datefrom, DateTime? dateto, string[] rank, int? noofday)
        {
            int days = DateTime.DaysInMonth(Convert.ToDateTime(datefrom).Year, Convert.ToDateTime(datefrom).Month);
            List<ChartData> notilist = new List<ChartData>();

            if (noofday >= 335)
            {
                var emps2 = from em in CreReports.Where(x => (x.Dates >= datefrom && x.Dates <= dateto)).ToList() group em by em.Dates.Year;
                foreach (var item in emps2)
                {
                    ChartData nots = new ChartData();
                    nots.VesselName = CreReports.OrderByDescending(t => t.ImportDate).Where(x => rank.Contains(x.Position) && x.Dates.Year == item.Key).FirstOrDefault().VesselName;
                    nots.Deviation = CreReports.OrderByDescending(t => t.ImportDate).Where(x => rank.Contains(x.Position) && x.Dates.Year == item.Key && x.NonConfirmities != "").ToList().Count();
                    nots.Rank = CreReports.OrderByDescending(t => t.ImportDate).Where(x => rank.Contains(x.Position) && x.Dates.Year == item.Key).FirstOrDefault().Position;
                    var workSum = (from emp in CreReports.OrderByDescending(t => t.ImportDate).Where(x => rank.Contains(x.Position) && x.Dates.Year == item.Key)
                                   select emp.TotalHours).Sum();
                    nots.Work = workSum;
                    var RestSum = (from emp in CreReports.OrderByDescending(t => t.ImportDate).Where(x => rank.Contains(x.Position) && x.Dates.Year == item.Key)
                                   select emp.RestHours).Sum();
                    nots.Rest = RestSum;
                    nots.Months = item.Key.ToString();

                    notilist.Add(nots);
                }


            }
            else if (noofday > days && noofday < 335)
            {
                var emps = from em in CreReports.Where(x => (x.Dates >= datefrom && x.Dates <= dateto)).ToList() group em by em.Dates.Month;
                foreach (var item in emps)
                {
                    ChartData nots = new ChartData();
                    nots.VesselName = CreReports.OrderByDescending(t => t.ImportDate).Where(x => rank.Contains(x.Position) && x.Dates.Month == item.Key).FirstOrDefault().VesselName;
                    nots.Deviation = CreReports.OrderByDescending(t => t.ImportDate).Where(x => rank.Contains(x.Position) && x.Dates.Month == item.Key && x.NonConfirmities != "").ToList().Count();
                    nots.Rank = CreReports.OrderByDescending(t => t.ImportDate).Where(x => rank.Contains(x.Position) && x.Dates.Month == item.Key).FirstOrDefault().Position;
                    var workSum = (from emp in CreReports.OrderByDescending(t => t.ImportDate).Where(x => rank.Contains(x.Position) && x.Dates.Month == item.Key)
                                   select emp.TotalHours).Sum();
                    nots.Work = workSum;
                    var RestSum = (from emp in CreReports.OrderByDescending(t => t.ImportDate).Where(x => rank.Contains(x.Position) && x.Dates.Month == item.Key)
                                   select emp.RestHours).Sum();
                    nots.Rest = RestSum;

                    //var monthnumber = sc1.CreReports.OrderByDescending(t => t.ImportDate).Where(x => x.Dates.Month == item.Key).FirstOrDefault().Dates.Month;

                    nots.Months = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(item.Key);

                    notilist.Add(nots);
                }
                notilist = notilist.OrderBy(m => DateTime.ParseExact(m.Months, "MMMM", CultureInfo.InvariantCulture)).ToList();

            }
            else
            {
                if (Convert.ToDateTime(datefrom).Month.Equals(Convert.ToDateTime(dateto).Month))
                {
                    var emps1 = from em in CreReports.OrderBy(x => x.Dates).Where(x => rank.Contains(x.Position) && (x.Dates >= datefrom && x.Dates <= dateto)).ToList() group em by em.Dates.Day;
                    foreach (var item in emps1)
                    {
                        ChartData nots = new ChartData();
                        nots.VesselName = CreReports.Where(x => rank.Contains(x.Position) && x.Dates.Day.Equals(item.Key)).FirstOrDefault().VesselName;
                        nots.Deviation = CreReports.Where(x => rank.Contains(x.Position) && x.Dates.Day == item.Key && x.NonConfirmities != "").ToList().Count();
                        nots.Rank = CreReports.Where(x => rank.Contains(x.Position) && x.Dates.Day == item.Key).FirstOrDefault().Position;
                        var workSum = (from emp in CreReports.Where(x => rank.Contains(x.Position) && x.Dates.Day == item.Key)
                                       select emp.TotalHours);
                        nots.Work = workSum.Sum();
                        var RestSum = (from emp in CreReports.Where(x => rank.Contains(x.Position) && x.Dates.Day == item.Key)
                                       select emp.RestHours).Sum();
                        nots.Rest = RestSum;


                        nots.Months = item.Key.ToString();


                        notilist.Add(nots);
                    }

                }
                else
                {
                    var emps = from em in CreReports.Where(x => (x.Dates >= datefrom && x.Dates <= dateto)).ToList() group em by em.Dates.Month;
                    foreach (var item in emps)
                    {
                        ChartData nots = new ChartData();
                        nots.VesselName = CreReports.OrderByDescending(t => t.ImportDate).Where(x => rank.Contains(x.Position) && x.Dates.Month == item.Key).FirstOrDefault().VesselName;
                        nots.Deviation = CreReports.OrderByDescending(t => t.ImportDate).Where(x => rank.Contains(x.Position) && x.Dates.Month == item.Key && x.NonConfirmities != "").ToList().Count();
                        nots.Rank = CreReports.OrderByDescending(t => t.ImportDate).Where(x => rank.Contains(x.Position) && x.Dates.Month == item.Key).FirstOrDefault().Position;
                        var workSum = (from emp in CreReports.OrderByDescending(t => t.ImportDate).Where(x => rank.Contains(x.Position) && x.Dates.Month == item.Key)
                                       select emp.TotalHours).Sum();
                        nots.Work = workSum;
                        var RestSum = (from emp in CreReports.OrderByDescending(t => t.ImportDate).Where(x => rank.Contains(x.Position) && x.Dates.Month == item.Key)
                                       select emp.RestHours).Sum();
                        nots.Rest = RestSum;

                        //var monthnumber = sc1.CreReports.OrderByDescending(t => t.ImportDate).Where(x => x.Dates.Month == item.Key).FirstOrDefault().Dates.Month;

                        nots.Months = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(item.Key);

                        notilist.Add(nots);
                    }
                    notilist = notilist.OrderBy(m => DateTime.ParseExact(m.Months, "MMMM", CultureInfo.InvariantCulture)).ToList();

                }
            }


            return notilist;

        }

        public override List<ChartData> getChartDataFTFN(DateTime? datefrom, DateTime? dateto, string[] fleettype, string[] fleetname, int? noofday)
        {
            int days = DateTime.DaysInMonth(Convert.ToDateTime(datefrom).Year, Convert.ToDateTime(datefrom).Month);
            List<ChartData> notilist = new List<ChartData>();

            if (noofday >= 335)
            {
                var emps = from em in CreReports.Where(x => fleettype.Contains(x.FleetType) && fleetname.Contains(x.FleetName) && (x.Dates >= datefrom && x.Dates <= dateto)).ToList() group em by em.Dates.Year;
                foreach (var item in emps)
                {
                    ChartData nots = new ChartData();
                    nots.VesselName = CreReports.OrderByDescending(t => t.ImportDate).Where(x => x.Dates.Year == item.Key).FirstOrDefault().VesselName;
                    nots.Deviation = CreReports.OrderByDescending(t => t.ImportDate).Where(x => fleettype.Contains(x.FleetType) && fleetname.Contains(x.FleetName) && x.Dates.Year == item.Key && x.NonConfirmities != "").ToList().Count();
                    nots.Rank = CreReports.OrderByDescending(t => t.ImportDate).Where(x => x.Dates.Year == item.Key).FirstOrDefault().Position;
                    var workSum = (from emp in CreReports.OrderByDescending(t => t.ImportDate).Where(x => fleettype.Contains(x.FleetType) && fleetname.Contains(x.FleetName) && x.Dates.Year == item.Key)
                                   select emp.TotalHours).Sum();
                    nots.Work = workSum;
                    var RestSum = (from emp in CreReports.OrderByDescending(t => t.ImportDate).Where(x => fleettype.Contains(x.FleetType) && fleetname.Contains(x.FleetName) && x.Dates.Year == item.Key)
                                   select emp.RestHours).Sum();
                    nots.Rest = RestSum;

                    nots.Months = item.Key.ToString();

                    notilist.Add(nots);
                }
            }
            else if (noofday > days && noofday < 335)
            {
                var emps = from em in CreReports.Where(x => fleettype.Contains(x.FleetType) && fleetname.Contains(x.FleetName) && (x.Dates >= datefrom && x.Dates <= dateto)).ToList() group em by em.Dates.Month;
                foreach (var item in emps)
                {
                    ChartData nots = new ChartData();
                    nots.VesselName = CreReports.OrderByDescending(t => t.ImportDate).Where(x => x.Dates.Month == item.Key).FirstOrDefault().VesselName;
                    nots.Deviation = CreReports.OrderByDescending(t => t.ImportDate).Where(x => fleettype.Contains(x.FleetType) && fleetname.Contains(x.FleetName) && x.Dates.Month == item.Key && x.NonConfirmities != "").ToList().Count();
                    nots.Rank = CreReports.OrderByDescending(t => t.ImportDate).Where(x => x.Dates.Month == item.Key).FirstOrDefault().Position;
                    var workSum = (from emp in CreReports.OrderByDescending(t => t.ImportDate).Where(x => fleettype.Contains(x.FleetType) && fleetname.Contains(x.FleetName) && x.Dates.Month == item.Key)
                                   select emp.TotalHours).Sum();
                    nots.Work = workSum;
                    var RestSum = (from emp in CreReports.OrderByDescending(t => t.ImportDate).Where(x => fleettype.Contains(x.FleetType) && fleetname.Contains(x.FleetName) && x.Dates.Month == item.Key)
                                   select emp.RestHours).Sum();
                    nots.Rest = RestSum;

                    //var monthnumber = sc1.CreReports.OrderByDescending(t => t.ImportDate).Where(x => x.Dates.Month == item.Key).FirstOrDefault().Dates.Month;

                    nots.Months = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(item.Key);

                    notilist.Add(nots);
                }

                notilist = notilist.OrderBy(m => DateTime.ParseExact(m.Months, "MMMM", CultureInfo.InvariantCulture)).ToList();

            }
            else
            {
                if (Convert.ToDateTime(datefrom).Month.Equals(Convert.ToDateTime(dateto).Month))
                {
                    var emps1 = from em in CreReports.OrderBy(x => x.Dates).Where(x => fleettype.Contains(x.FleetType) && fleetname.Contains(x.FleetName) && (x.Dates >= datefrom && x.Dates <= dateto)).ToList() group em by em.Dates.Day;
                    foreach (var item in emps1)
                    {
                        ChartData nots = new ChartData();
                        nots.VesselName = CreReports.Where(x => x.Dates.Day.Equals(item.Key)).FirstOrDefault().VesselName;
                        nots.Deviation = CreReports.Where(x => fleettype.Contains(x.FleetType) && fleetname.Contains(x.FleetName) && x.Dates.Day == item.Key && x.NonConfirmities != "").ToList().Count();
                        nots.Rank = CreReports.Where(x => x.Dates.Day == item.Key).FirstOrDefault().Position;
                        var workSum = (from emp in CreReports.Where(x => fleettype.Contains(x.FleetType) && fleetname.Contains(x.FleetName) && x.Dates.Day == item.Key)
                                       select emp.TotalHours);
                        nots.Work = workSum.Sum();
                        var RestSum = (from emp in CreReports.Where(x => fleettype.Contains(x.FleetType) && fleetname.Contains(x.FleetName) && x.Dates.Day == item.Key)
                                       select emp.RestHours).Sum();
                        nots.Rest = RestSum;


                        nots.Months = item.Key.ToString();


                        notilist.Add(nots);
                    }
                }
                else
                {
                    var emps = from em in CreReports.Where(x => fleettype.Contains(x.FleetType) && fleetname.Contains(x.FleetName) && (x.Dates >= datefrom && x.Dates <= dateto)).ToList() group em by em.Dates.Month;
                    foreach (var item in emps)
                    {
                        ChartData nots = new ChartData();
                        nots.VesselName = CreReports.OrderByDescending(t => t.ImportDate).Where(x => x.Dates.Month == item.Key).FirstOrDefault().VesselName;
                        nots.Deviation = CreReports.OrderByDescending(t => t.ImportDate).Where(x => fleettype.Contains(x.FleetType) && fleetname.Contains(x.FleetName) && x.Dates.Month == item.Key && x.NonConfirmities != "").ToList().Count();
                        nots.Rank = CreReports.OrderByDescending(t => t.ImportDate).Where(x => x.Dates.Month == item.Key).FirstOrDefault().Position;
                        var workSum = (from emp in CreReports.OrderByDescending(t => t.ImportDate).Where(x => fleettype.Contains(x.FleetType) && fleetname.Contains(x.FleetName) && x.Dates.Month == item.Key)
                                       select emp.TotalHours).Sum();
                        nots.Work = workSum;
                        var RestSum = (from emp in CreReports.OrderByDescending(t => t.ImportDate).Where(x => fleettype.Contains(x.FleetType) && fleetname.Contains(x.FleetName) && x.Dates.Month == item.Key)
                                       select emp.RestHours).Sum();
                        nots.Rest = RestSum;

                        //var monthnumber = sc1.CreReports.OrderByDescending(t => t.ImportDate).Where(x => x.Dates.Month == item.Key).FirstOrDefault().Dates.Month;

                        nots.Months = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(item.Key);

                        notilist.Add(nots);
                    }

                    notilist = notilist.OrderBy(m => DateTime.ParseExact(m.Months, "MMMM", CultureInfo.InvariantCulture)).ToList();


                }
            }

            return notilist;

        }

        public override List<ChartData> getChartDataFTFNVN(DateTime? datefrom, DateTime? dateto, string[] fleettype, string[] fleetname, string[] vesselname, int? noofday)
        {
            int days = DateTime.DaysInMonth(Convert.ToDateTime(datefrom).Year, Convert.ToDateTime(datefrom).Month);

            List<ChartData> notilist = new List<ChartData>();

            if (noofday >= 335)
            {
                var emps = from em in CreReports.Where(x => fleettype.Contains(x.FleetType) && fleetname.Contains(x.FleetName) && vesselname.Contains(x.VesselName) && (x.Dates >= datefrom && x.Dates <= dateto)).ToList() group em by em.Dates.Year;
                foreach (var item in emps)
                {
                    ChartData nots = new ChartData();
                    nots.VesselName = CreReports.OrderByDescending(t => t.ImportDate).Where(x => x.Dates.Year == item.Key).FirstOrDefault().VesselName;
                    nots.Deviation = CreReports.OrderByDescending(t => t.ImportDate).Where(x => fleettype.Contains(x.FleetType) && fleetname.Contains(x.FleetName) && vesselname.Contains(x.VesselName) && x.Dates.Year == item.Key && x.NonConfirmities != "").ToList().Count();
                    nots.Rank = CreReports.OrderByDescending(t => t.ImportDate).Where(x => x.Dates.Year == item.Key).FirstOrDefault().Position;
                    var workSum = (from emp in CreReports.OrderByDescending(t => t.ImportDate).Where(x => fleettype.Contains(x.FleetType) && fleetname.Contains(x.FleetName) && vesselname.Contains(x.VesselName) && x.Dates.Year == item.Key)
                                   select emp.TotalHours).Sum();
                    nots.Work = workSum;
                    var RestSum = (from emp in CreReports.OrderByDescending(t => t.ImportDate).Where(x => fleettype.Contains(x.FleetType) && fleetname.Contains(x.FleetName) && vesselname.Contains(x.VesselName) && x.Dates.Year == item.Key)
                                   select emp.RestHours).Sum();
                    nots.Rest = RestSum;

                    nots.Months = item.Key.ToString();

                    notilist.Add(nots);
                }
            }
            else if (noofday > days && noofday < 335)
            {
                var emps = from em in CreReports.Where(x => fleettype.Contains(x.FleetType) && fleetname.Contains(x.FleetName) && vesselname.Contains(x.VesselName) && (x.Dates >= datefrom && x.Dates <= dateto)).ToList() group em by em.Dates.Month;
                foreach (var item in emps)
                {
                    ChartData nots = new ChartData();
                    nots.VesselName = CreReports.OrderByDescending(t => t.ImportDate).Where(x => x.Dates.Month == item.Key).FirstOrDefault().VesselName;
                    nots.Deviation = CreReports.OrderByDescending(t => t.ImportDate).Where(x => fleettype.Contains(x.FleetType) && fleetname.Contains(x.FleetName) && vesselname.Contains(x.VesselName) && x.Dates.Month == item.Key && x.NonConfirmities != "").ToList().Count();
                    nots.Rank = CreReports.OrderByDescending(t => t.ImportDate).Where(x => x.Dates.Month == item.Key).FirstOrDefault().Position;
                    var workSum = (from emp in CreReports.OrderByDescending(t => t.ImportDate).Where(x => fleettype.Contains(x.FleetType) && fleetname.Contains(x.FleetName) && vesselname.Contains(x.VesselName) && x.Dates.Month == item.Key)
                                   select emp.TotalHours).Sum();
                    nots.Work = workSum;
                    var RestSum = (from emp in CreReports.OrderByDescending(t => t.ImportDate).Where(x => fleettype.Contains(x.FleetType) && fleetname.Contains(x.FleetName) && vesselname.Contains(x.VesselName) && x.Dates.Month == item.Key)
                                   select emp.RestHours).Sum();
                    nots.Rest = RestSum;

                    //var monthnumber = sc1.CreReports.OrderByDescending(t => t.ImportDate).Where(x => x.Dates.Month == item.Key).FirstOrDefault().Dates.Month;

                    nots.Months = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(item.Key);

                    notilist.Add(nots);
                }

                notilist = notilist.OrderBy(m => DateTime.ParseExact(m.Months, "MMMM", CultureInfo.InvariantCulture)).ToList();

            }
            else
            {
                if (Convert.ToDateTime(datefrom).Month.Equals(Convert.ToDateTime(dateto).Month))
                {
                    var emps1 = from em in CreReports.OrderBy(x => x.Dates).Where(x => fleettype.Contains(x.FleetType) && fleetname.Contains(x.FleetName) && vesselname.Contains(x.VesselName) && (x.Dates >= datefrom && x.Dates <= dateto)).ToList() group em by em.Dates.Day;
                    foreach (var item in emps1)
                    {
                        ChartData nots = new ChartData();
                        nots.VesselName = CreReports.Where(x => x.Dates.Day.Equals(item.Key)).FirstOrDefault().VesselName;
                        nots.Deviation = CreReports.Where(x => fleettype.Contains(x.FleetType) && fleetname.Contains(x.FleetName) && vesselname.Contains(x.VesselName) && x.Dates.Day == item.Key && x.NonConfirmities != "").ToList().Count();
                        nots.Rank = CreReports.Where(x => x.Dates.Day == item.Key).FirstOrDefault().Position;
                        var workSum = (from emp in CreReports.Where(x => fleettype.Contains(x.FleetType) && fleetname.Contains(x.FleetName) && vesselname.Contains(x.VesselName) && x.Dates.Day == item.Key)
                                       select emp.TotalHours);
                        nots.Work = workSum.Sum();
                        var RestSum = (from emp in CreReports.Where(x => fleettype.Contains(x.FleetType) && fleetname.Contains(x.FleetName) && vesselname.Contains(x.VesselName) && x.Dates.Day == item.Key)
                                       select emp.RestHours).Sum();
                        nots.Rest = RestSum;


                        nots.Months = item.Key.ToString();


                        notilist.Add(nots);
                    }
                }
                else
                {
                    var emps = from em in CreReports.Where(x => fleettype.Contains(x.FleetType) && fleetname.Contains(x.FleetName) && vesselname.Contains(x.VesselName) && (x.Dates >= datefrom && x.Dates <= dateto)).ToList() group em by em.Dates.Month;
                    foreach (var item in emps)
                    {
                        ChartData nots = new ChartData();
                        nots.VesselName = CreReports.OrderByDescending(t => t.ImportDate).Where(x => x.Dates.Month == item.Key).FirstOrDefault().VesselName;
                        nots.Deviation = CreReports.OrderByDescending(t => t.ImportDate).Where(x => fleettype.Contains(x.FleetType) && fleetname.Contains(x.FleetName) && vesselname.Contains(x.VesselName) && x.Dates.Month == item.Key && x.NonConfirmities != "").ToList().Count();
                        nots.Rank = CreReports.OrderByDescending(t => t.ImportDate).Where(x => x.Dates.Month == item.Key).FirstOrDefault().Position;
                        var workSum = (from emp in CreReports.OrderByDescending(t => t.ImportDate).Where(x => fleettype.Contains(x.FleetType) && fleetname.Contains(x.FleetName) && vesselname.Contains(x.VesselName) && x.Dates.Month == item.Key)
                                       select emp.TotalHours).Sum();
                        nots.Work = workSum;
                        var RestSum = (from emp in CreReports.OrderByDescending(t => t.ImportDate).Where(x => fleettype.Contains(x.FleetType) && fleetname.Contains(x.FleetName) && vesselname.Contains(x.VesselName) && x.Dates.Month == item.Key)
                                       select emp.RestHours).Sum();
                        nots.Rest = RestSum;

                        //var monthnumber = sc1.CreReports.OrderByDescending(t => t.ImportDate).Where(x => x.Dates.Month == item.Key).FirstOrDefault().Dates.Month;

                        nots.Months = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(item.Key);

                        notilist.Add(nots);
                    }

                    notilist = notilist.OrderBy(m => DateTime.ParseExact(m.Months, "MMMM", CultureInfo.InvariantCulture)).ToList();


                }
            }

            return notilist;

        }

        public override List<ChartData> getChartDataFTFNVNRK(DateTime? datefrom, DateTime? dateto, string[] fleettype, string[] fleetname, string[] vesselname, string[] rank, int? noofday)
        {
            int days = DateTime.DaysInMonth(Convert.ToDateTime(datefrom).Year, Convert.ToDateTime(datefrom).Month);
            List<ChartData> notilist = new List<ChartData>();

            if (noofday >= 335)
            {
                var emps = from em in CreReports.Where(x => fleettype.Contains(x.FleetType) && fleetname.Contains(x.FleetName) && vesselname.Contains(x.VesselName) && rank.Contains(x.Position) && (x.Dates >= datefrom && x.Dates <= dateto)).ToList() group em by em.Dates.Year;
                foreach (var item in emps)
                {
                    ChartData nots = new ChartData();
                    nots.VesselName = CreReports.OrderByDescending(t => t.ImportDate).Where(x => x.Dates.Year == item.Key).FirstOrDefault().VesselName;
                    nots.Deviation = CreReports.OrderByDescending(t => t.ImportDate).Where(x => fleettype.Contains(x.FleetType) && fleetname.Contains(x.FleetName) && vesselname.Contains(x.VesselName) && rank.Contains(x.Position) && x.Dates.Year == item.Key && x.NonConfirmities != "").ToList().Count();
                    nots.Rank = CreReports.OrderByDescending(t => t.ImportDate).Where(x => x.Dates.Year == item.Key).FirstOrDefault().Position;
                    var workSum = (from emp in CreReports.OrderByDescending(t => t.ImportDate).Where(x => fleettype.Contains(x.FleetType) && fleetname.Contains(x.FleetName) && vesselname.Contains(x.VesselName) && rank.Contains(x.Position) && x.Dates.Year == item.Key)
                                   select emp.TotalHours).Sum();
                    nots.Work = workSum;
                    var RestSum = (from emp in CreReports.OrderByDescending(t => t.ImportDate).Where(x => fleettype.Contains(x.FleetType) && fleetname.Contains(x.FleetName) && vesselname.Contains(x.VesselName) && rank.Contains(x.Position) && x.Dates.Year == item.Key)
                                   select emp.RestHours).Sum();
                    nots.Rest = RestSum;

                    nots.Months = item.Key.ToString();

                    notilist.Add(nots);
                }
            }
            else if (noofday > days && noofday < 335)
            {
                var emps = from em in CreReports.Where(x => fleettype.Contains(x.FleetType) && fleetname.Contains(x.FleetName) && vesselname.Contains(x.VesselName) && rank.Contains(x.Position) && (x.Dates >= datefrom && x.Dates <= dateto)).ToList() group em by em.Dates.Month;
                foreach (var item in emps)
                {
                    ChartData nots = new ChartData();
                    nots.VesselName = CreReports.OrderByDescending(t => t.ImportDate).Where(x => x.Dates.Month == item.Key).FirstOrDefault().VesselName;
                    nots.Deviation = CreReports.OrderByDescending(t => t.ImportDate).Where(x => fleettype.Contains(x.FleetType) && fleetname.Contains(x.FleetName) && vesselname.Contains(x.VesselName) && rank.Contains(x.Position) && x.Dates.Month == item.Key && x.NonConfirmities != "").ToList().Count();
                    nots.Rank = CreReports.OrderByDescending(t => t.ImportDate).Where(x => x.Dates.Month == item.Key).FirstOrDefault().Position;
                    var workSum = (from emp in CreReports.OrderByDescending(t => t.ImportDate).Where(x => fleettype.Contains(x.FleetType) && fleetname.Contains(x.FleetName) && vesselname.Contains(x.VesselName) && rank.Contains(x.Position) && x.Dates.Month == item.Key)
                                   select emp.TotalHours).Sum();
                    nots.Work = workSum;
                    var RestSum = (from emp in CreReports.OrderByDescending(t => t.ImportDate).Where(x => fleettype.Contains(x.FleetType) && fleetname.Contains(x.FleetName) && vesselname.Contains(x.VesselName) && rank.Contains(x.Position) && x.Dates.Month == item.Key)
                                   select emp.RestHours).Sum();
                    nots.Rest = RestSum;

                    //var monthnumber = sc1.CreReports.OrderByDescending(t => t.ImportDate).Where(x => x.Dates.Month == item.Key).FirstOrDefault().Dates.Month;

                    nots.Months = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(item.Key);

                    notilist.Add(nots);
                }

                notilist = notilist.OrderBy(m => DateTime.ParseExact(m.Months, "MMMM", CultureInfo.InvariantCulture)).ToList();

            }
            else
            {
                if (Convert.ToDateTime(datefrom).Month.Equals(Convert.ToDateTime(dateto).Month))
                {
                    var emps1 = from em in CreReports.OrderBy(x => x.Dates).Where(x => fleettype.Contains(x.FleetType) && fleetname.Contains(x.FleetName) && vesselname.Contains(x.VesselName) && rank.Contains(x.Position) && (x.Dates >= datefrom && x.Dates <= dateto)).ToList() group em by em.Dates.Day;
                    foreach (var item in emps1)
                    {
                        ChartData nots = new ChartData();
                        nots.VesselName = CreReports.Where(x => x.Dates.Day.Equals(item.Key)).FirstOrDefault().VesselName;
                        nots.Deviation = CreReports.Where(x => fleettype.Contains(x.FleetType) && fleetname.Contains(x.FleetName) && vesselname.Contains(x.VesselName) && rank.Contains(x.Position) && x.Dates.Day == item.Key && x.NonConfirmities != "").ToList().Count();
                        nots.Rank = CreReports.Where(x => x.Dates.Day == item.Key).FirstOrDefault().Position;
                        var workSum = (from emp in CreReports.Where(x => fleettype.Contains(x.FleetType) && fleetname.Contains(x.FleetName) && vesselname.Contains(x.VesselName) && rank.Contains(x.Position) && x.Dates.Day == item.Key)
                                       select emp.TotalHours);
                        nots.Work = workSum.Sum();
                        var RestSum = (from emp in CreReports.Where(x => fleettype.Contains(x.FleetType) && fleetname.Contains(x.FleetName) && vesselname.Contains(x.VesselName) && rank.Contains(x.Position) && x.Dates.Day == item.Key)
                                       select emp.RestHours).Sum();
                        nots.Rest = RestSum;


                        nots.Months = item.Key.ToString();


                        notilist.Add(nots);
                    }
                }
                else
                {
                    var emps = from em in CreReports.Where(x => fleettype.Contains(x.FleetType) && fleetname.Contains(x.FleetName) && vesselname.Contains(x.VesselName) && rank.Contains(x.Position) && (x.Dates >= datefrom && x.Dates <= dateto)).ToList() group em by em.Dates.Month;
                    foreach (var item in emps)
                    {
                        ChartData nots = new ChartData();
                        nots.VesselName = CreReports.OrderByDescending(t => t.ImportDate).Where(x => x.Dates.Month == item.Key).FirstOrDefault().VesselName;
                        nots.Deviation = CreReports.OrderByDescending(t => t.ImportDate).Where(x => fleettype.Contains(x.FleetType) && fleetname.Contains(x.FleetName) && vesselname.Contains(x.VesselName) && rank.Contains(x.Position) && x.Dates.Month == item.Key && x.NonConfirmities != "").ToList().Count();
                        nots.Rank = CreReports.OrderByDescending(t => t.ImportDate).Where(x => x.Dates.Month == item.Key).FirstOrDefault().Position;
                        var workSum = (from emp in CreReports.OrderByDescending(t => t.ImportDate).Where(x => fleettype.Contains(x.FleetType) && fleetname.Contains(x.FleetName) && vesselname.Contains(x.VesselName) && rank.Contains(x.Position) && x.Dates.Month == item.Key)
                                       select emp.TotalHours).Sum();
                        nots.Work = workSum;
                        var RestSum = (from emp in CreReports.OrderByDescending(t => t.ImportDate).Where(x => fleettype.Contains(x.FleetType) && fleetname.Contains(x.FleetName) && vesselname.Contains(x.VesselName) && rank.Contains(x.Position) && x.Dates.Month == item.Key)
                                       select emp.RestHours).Sum();
                        nots.Rest = RestSum;

                        //var monthnumber = sc1.CreReports.OrderByDescending(t => t.ImportDate).Where(x => x.Dates.Month == item.Key).FirstOrDefault().Dates.Month;

                        nots.Months = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(item.Key);

                        notilist.Add(nots);
                    }

                    notilist = notilist.OrderBy(m => DateTime.ParseExact(m.Months, "MMMM", CultureInfo.InvariantCulture)).ToList();

                }
            }

            return notilist;

        }

        public override List<ChartData> getChartDataFNRK(DateTime? datefrom, DateTime? dateto, string[] fleetname, string[] rank, int? noofday)
        {
            int days = DateTime.DaysInMonth(Convert.ToDateTime(datefrom).Year, Convert.ToDateTime(datefrom).Month);
            List<ChartData> notilist = new List<ChartData>();

            if (noofday >= 335)
            {
                var emps = from em in CreReports.Where(x => fleetname.Contains(x.FleetName) && rank.Contains(x.Position) && (x.Dates >= datefrom && x.Dates <= dateto)).ToList() group em by em.Dates.Year;
                foreach (var item in emps)
                {
                    ChartData nots = new ChartData();
                    nots.VesselName = CreReports.OrderByDescending(t => t.ImportDate).Where(x => x.Dates.Year == item.Key).FirstOrDefault().VesselName;
                    nots.Deviation = CreReports.OrderByDescending(t => t.ImportDate).Where(x => fleetname.Contains(x.FleetName) && rank.Contains(x.Position) && x.Dates.Year == item.Key && x.NonConfirmities != "").ToList().Count();
                    nots.Rank = CreReports.OrderByDescending(t => t.ImportDate).Where(x => x.Dates.Year == item.Key).FirstOrDefault().Position;
                    var workSum = (from emp in CreReports.OrderByDescending(t => t.ImportDate).Where(x => fleetname.Contains(x.FleetName) && rank.Contains(x.Position) && x.Dates.Year == item.Key)
                                   select emp.TotalHours).Sum();
                    nots.Work = workSum;
                    var RestSum = (from emp in CreReports.OrderByDescending(t => t.ImportDate).Where(x => fleetname.Contains(x.FleetName) && rank.Contains(x.Position) && x.Dates.Year == item.Key)
                                   select emp.RestHours).Sum();
                    nots.Rest = RestSum;

                    nots.Months = item.Key.ToString();

                    notilist.Add(nots);
                }
            }
            else if (noofday > days && noofday < 335)
            {
                var emps = from em in CreReports.Where(x => fleetname.Contains(x.FleetName) && rank.Contains(x.Position) && (x.Dates >= datefrom && x.Dates <= dateto)).ToList() group em by em.Dates.Month;
                foreach (var item in emps)
                {
                    ChartData nots = new ChartData();
                    nots.VesselName = CreReports.OrderByDescending(t => t.ImportDate).Where(x => x.Dates.Month == item.Key).FirstOrDefault().VesselName;
                    nots.Deviation = CreReports.OrderByDescending(t => t.ImportDate).Where(x => fleetname.Contains(x.FleetName) && rank.Contains(x.Position) && x.Dates.Month == item.Key && x.NonConfirmities != "").ToList().Count();
                    nots.Rank = CreReports.OrderByDescending(t => t.ImportDate).Where(x => x.Dates.Month == item.Key).FirstOrDefault().Position;
                    var workSum = (from emp in CreReports.OrderByDescending(t => t.ImportDate).Where(x => fleetname.Contains(x.FleetName) && rank.Contains(x.Position) && x.Dates.Month == item.Key)
                                   select emp.TotalHours).Sum();
                    nots.Work = workSum;
                    var RestSum = (from emp in CreReports.OrderByDescending(t => t.ImportDate).Where(x => fleetname.Contains(x.FleetName) && rank.Contains(x.Position) && x.Dates.Month == item.Key)
                                   select emp.RestHours).Sum();
                    nots.Rest = RestSum;

                    //var monthnumber = sc1.CreReports.OrderByDescending(t => t.ImportDate).Where(x => x.Dates.Month == item.Key).FirstOrDefault().Dates.Month;

                    nots.Months = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(item.Key);

                    notilist.Add(nots);
                }

                notilist = notilist.OrderBy(m => DateTime.ParseExact(m.Months, "MMMM", CultureInfo.InvariantCulture)).ToList();

            }
            else
            {
                if (Convert.ToDateTime(datefrom).Month.Equals(Convert.ToDateTime(dateto).Month))
                {
                    var emps1 = from em in CreReports.OrderBy(x => x.Dates).Where(x => fleetname.Contains(x.FleetName) && rank.Contains(x.Position) && (x.Dates >= datefrom && x.Dates <= dateto)).ToList() group em by em.Dates.Day;
                    foreach (var item in emps1)
                    {
                        ChartData nots = new ChartData();
                        nots.VesselName = CreReports.Where(x => x.Dates.Day.Equals(item.Key)).FirstOrDefault().VesselName;
                        nots.Deviation = CreReports.Where(x => fleetname.Contains(x.FleetName) && rank.Contains(x.Position) && x.Dates.Day == item.Key && x.NonConfirmities != "").ToList().Count();
                        nots.Rank = CreReports.Where(x => x.Dates.Day == item.Key).FirstOrDefault().Position;
                        var workSum = (from emp in CreReports.Where(x => fleetname.Contains(x.FleetName) && rank.Contains(x.Position) && x.Dates.Day == item.Key)
                                       select emp.TotalHours);
                        nots.Work = workSum.Sum();
                        var RestSum = (from emp in CreReports.Where(x => fleetname.Contains(x.FleetName) && rank.Contains(x.Position) && x.Dates.Day == item.Key)
                                       select emp.RestHours).Sum();
                        nots.Rest = RestSum;


                        nots.Months = item.Key.ToString();


                        notilist.Add(nots);
                    }
                }
                else
                {
                    var emps = from em in CreReports.Where(x => fleetname.Contains(x.FleetName) && rank.Contains(x.Position) && (x.Dates >= datefrom && x.Dates <= dateto)).ToList() group em by em.Dates.Month;
                    foreach (var item in emps)
                    {
                        ChartData nots = new ChartData();
                        nots.VesselName = CreReports.OrderByDescending(t => t.ImportDate).Where(x => x.Dates.Month == item.Key).FirstOrDefault().VesselName;
                        nots.Deviation = CreReports.OrderByDescending(t => t.ImportDate).Where(x => fleetname.Contains(x.FleetName) && rank.Contains(x.Position) && x.Dates.Month == item.Key && x.NonConfirmities != "").ToList().Count();
                        nots.Rank = CreReports.OrderByDescending(t => t.ImportDate).Where(x => x.Dates.Month == item.Key).FirstOrDefault().Position;
                        var workSum = (from emp in CreReports.OrderByDescending(t => t.ImportDate).Where(x => fleetname.Contains(x.FleetName) && rank.Contains(x.Position) && x.Dates.Month == item.Key)
                                       select emp.TotalHours).Sum();
                        nots.Work = workSum;
                        var RestSum = (from emp in CreReports.OrderByDescending(t => t.ImportDate).Where(x => fleetname.Contains(x.FleetName) && rank.Contains(x.Position) && x.Dates.Month == item.Key)
                                       select emp.RestHours).Sum();
                        nots.Rest = RestSum;

                        //var monthnumber = sc1.CreReports.OrderByDescending(t => t.ImportDate).Where(x => x.Dates.Month == item.Key).FirstOrDefault().Dates.Month;

                        nots.Months = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(item.Key);

                        notilist.Add(nots);
                    }

                    notilist = notilist.OrderBy(m => DateTime.ParseExact(m.Months, "MMMM", CultureInfo.InvariantCulture)).ToList();

                }
            }

            return notilist;

        }

        public override List<ChartData> getChartDataFNVN(DateTime? datefrom, DateTime? dateto, string[] fleetname, string[] vesselname, int? noofday)
        {
            int days = DateTime.DaysInMonth(Convert.ToDateTime(datefrom).Year, Convert.ToDateTime(datefrom).Month);
            List<ChartData> notilist = new List<ChartData>();

            if (noofday >= 335)
            {
                var emps = from em in CreReports.Where(x => fleetname.Contains(x.FleetName) && vesselname.Contains(x.VesselName) && (x.Dates >= datefrom && x.Dates <= dateto)).ToList() group em by em.Dates.Year;
                foreach (var item in emps)
                {
                    ChartData nots = new ChartData();
                    nots.VesselName = CreReports.OrderByDescending(t => t.ImportDate).Where(x => x.Dates.Year == item.Key).FirstOrDefault().VesselName;
                    nots.Deviation = CreReports.OrderByDescending(t => t.ImportDate).Where(x => fleetname.Contains(x.FleetName) && vesselname.Contains(x.VesselName) && x.Dates.Year == item.Key && x.NonConfirmities != "").ToList().Count();
                    nots.Rank = CreReports.OrderByDescending(t => t.ImportDate).Where(x => x.Dates.Year == item.Key).FirstOrDefault().Position;
                    var workSum = (from emp in CreReports.OrderByDescending(t => t.ImportDate).Where(x => fleetname.Contains(x.FleetName) && vesselname.Contains(x.VesselName) && x.Dates.Year == item.Key)
                                   select emp.TotalHours).Sum();
                    nots.Work = workSum;
                    var RestSum = (from emp in CreReports.OrderByDescending(t => t.ImportDate).Where(x => fleetname.Contains(x.FleetName) && vesselname.Contains(x.VesselName) && x.Dates.Year == item.Key)
                                   select emp.RestHours).Sum();
                    nots.Rest = RestSum;

                    nots.Months = item.Key.ToString();

                    notilist.Add(nots);
                }
            }
            else if (noofday > days && noofday < 335)
            {
                var emps = from em in CreReports.Where(x => fleetname.Contains(x.FleetName) && vesselname.Contains(x.VesselName) && (x.Dates >= datefrom && x.Dates <= dateto)).ToList() group em by em.Dates.Month;
                foreach (var item in emps)
                {
                    ChartData nots = new ChartData();
                    nots.VesselName = CreReports.OrderByDescending(t => t.ImportDate).Where(x => x.Dates.Month == item.Key).FirstOrDefault().VesselName;
                    nots.Deviation = CreReports.OrderByDescending(t => t.ImportDate).Where(x => fleetname.Contains(x.FleetName) && vesselname.Contains(x.VesselName) && x.Dates.Month == item.Key && x.NonConfirmities != "").ToList().Count();
                    nots.Rank = CreReports.OrderByDescending(t => t.ImportDate).Where(x => x.Dates.Month == item.Key).FirstOrDefault().Position;
                    var workSum = (from emp in CreReports.OrderByDescending(t => t.ImportDate).Where(x => fleetname.Contains(x.FleetName) && vesselname.Contains(x.VesselName) && x.Dates.Month == item.Key)
                                   select emp.TotalHours).Sum();
                    nots.Work = workSum;
                    var RestSum = (from emp in CreReports.OrderByDescending(t => t.ImportDate).Where(x => fleetname.Contains(x.FleetName) && vesselname.Contains(x.VesselName) && x.Dates.Month == item.Key)
                                   select emp.RestHours).Sum();
                    nots.Rest = RestSum;

                    //var monthnumber = sc1.CreReports.OrderByDescending(t => t.ImportDate).Where(x => x.Dates.Month == item.Key).FirstOrDefault().Dates.Month;

                    nots.Months = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(item.Key);

                    notilist.Add(nots);
                }

                notilist = notilist.OrderBy(m => DateTime.ParseExact(m.Months, "MMMM", CultureInfo.InvariantCulture)).ToList();

            }
            else
            {
                if (Convert.ToDateTime(datefrom).Month.Equals(Convert.ToDateTime(dateto).Month))
                {
                    var emps1 = from em in CreReports.OrderBy(x => x.Dates).Where(x => fleetname.Contains(x.FleetName) && vesselname.Contains(x.VesselName) && (x.Dates >= datefrom && x.Dates <= dateto)).ToList() group em by em.Dates.Day;
                    foreach (var item in emps1)
                    {
                        ChartData nots = new ChartData();
                        nots.VesselName = CreReports.Where(x => x.Dates.Day.Equals(item.Key)).FirstOrDefault().VesselName;
                        nots.Deviation = CreReports.Where(x => fleetname.Contains(x.FleetName) && vesselname.Contains(x.VesselName) && x.Dates.Day == item.Key && x.NonConfirmities != "").ToList().Count();
                        nots.Rank = CreReports.Where(x => x.Dates.Day == item.Key).FirstOrDefault().Position;
                        var workSum = (from emp in CreReports.Where(x => fleetname.Contains(x.FleetName) && vesselname.Contains(x.VesselName) && x.Dates.Day == item.Key)
                                       select emp.TotalHours);
                        nots.Work = workSum.Sum();
                        var RestSum = (from emp in CreReports.Where(x => fleetname.Contains(x.FleetName) && vesselname.Contains(x.VesselName) && x.Dates.Day == item.Key)
                                       select emp.RestHours).Sum();
                        nots.Rest = RestSum;


                        nots.Months = item.Key.ToString();


                        notilist.Add(nots);
                    }
                }
                else
                {
                    var emps = from em in CreReports.Where(x => fleetname.Contains(x.FleetName) && vesselname.Contains(x.VesselName) && (x.Dates >= datefrom && x.Dates <= dateto)).ToList() group em by em.Dates.Month;
                    foreach (var item in emps)
                    {
                        ChartData nots = new ChartData();
                        nots.VesselName = CreReports.OrderByDescending(t => t.ImportDate).Where(x => x.Dates.Month == item.Key).FirstOrDefault().VesselName;
                        nots.Deviation = CreReports.OrderByDescending(t => t.ImportDate).Where(x => fleetname.Contains(x.FleetName) && vesselname.Contains(x.VesselName) && x.Dates.Month == item.Key && x.NonConfirmities != "").ToList().Count();
                        nots.Rank = CreReports.OrderByDescending(t => t.ImportDate).Where(x => x.Dates.Month == item.Key).FirstOrDefault().Position;
                        var workSum = (from emp in CreReports.OrderByDescending(t => t.ImportDate).Where(x => fleetname.Contains(x.FleetName) && vesselname.Contains(x.VesselName) && x.Dates.Month == item.Key)
                                       select emp.TotalHours).Sum();
                        nots.Work = workSum;
                        var RestSum = (from emp in CreReports.OrderByDescending(t => t.ImportDate).Where(x => fleetname.Contains(x.FleetName) && vesselname.Contains(x.VesselName) && x.Dates.Month == item.Key)
                                       select emp.RestHours).Sum();
                        nots.Rest = RestSum;

                        //var monthnumber = sc1.CreReports.OrderByDescending(t => t.ImportDate).Where(x => x.Dates.Month == item.Key).FirstOrDefault().Dates.Month;

                        nots.Months = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(item.Key);

                        notilist.Add(nots);
                    }

                    notilist = notilist.OrderBy(m => DateTime.ParseExact(m.Months, "MMMM", CultureInfo.InvariantCulture)).ToList();

                }
            }

            return notilist;

        }

        public override List<ChartData> getChartDataFTVN(DateTime? datefrom, DateTime? dateto, string[] fleettype, string[] vesselname, int? noofday)
        {
            int days = DateTime.DaysInMonth(Convert.ToDateTime(datefrom).Year, Convert.ToDateTime(datefrom).Month);
            List<ChartData> notilist = new List<ChartData>();

            if (noofday >= 335)
            {
                var emps = from em in CreReports.Where(x => fleettype.Contains(x.FleetType) && vesselname.Contains(x.VesselName) && (x.Dates >= datefrom && x.Dates <= dateto)).ToList() group em by em.Dates.Year;
                foreach (var item in emps)
                {
                    ChartData nots = new ChartData();
                    nots.VesselName = CreReports.OrderByDescending(t => t.ImportDate).Where(x => x.Dates.Year == item.Key).FirstOrDefault().VesselName;
                    nots.Deviation = CreReports.OrderByDescending(t => t.ImportDate).Where(x => fleettype.Contains(x.FleetType) && vesselname.Contains(x.VesselName) && x.Dates.Year == item.Key && x.NonConfirmities != "").ToList().Count();
                    nots.Rank = CreReports.OrderByDescending(t => t.ImportDate).Where(x => x.Dates.Year == item.Key).FirstOrDefault().Position;
                    var workSum = (from emp in CreReports.OrderByDescending(t => t.ImportDate).Where(x => fleettype.Contains(x.FleetType) && vesselname.Contains(x.VesselName) && x.Dates.Year == item.Key)
                                   select emp.TotalHours).Sum();
                    nots.Work = workSum;
                    var RestSum = (from emp in CreReports.OrderByDescending(t => t.ImportDate).Where(x => fleettype.Contains(x.FleetType) && vesselname.Contains(x.VesselName) && x.Dates.Year == item.Key)
                                   select emp.RestHours).Sum();
                    nots.Rest = RestSum;

                    nots.Months = item.Key.ToString();

                    notilist.Add(nots);
                }
            }
            else if (noofday > days && noofday < 335)
            {
                var emps = from em in CreReports.Where(x => fleettype.Contains(x.FleetType) && vesselname.Contains(x.VesselName) && (x.Dates >= datefrom && x.Dates <= dateto)).ToList() group em by em.Dates.Month;
                foreach (var item in emps)
                {
                    ChartData nots = new ChartData();
                    nots.VesselName = CreReports.OrderByDescending(t => t.ImportDate).Where(x => x.Dates.Month == item.Key).FirstOrDefault().VesselName;
                    nots.Deviation = CreReports.OrderByDescending(t => t.ImportDate).Where(x => fleettype.Contains(x.FleetType) && vesselname.Contains(x.VesselName) && x.Dates.Month == item.Key && x.NonConfirmities != "").ToList().Count();
                    nots.Rank = CreReports.OrderByDescending(t => t.ImportDate).Where(x => x.Dates.Month == item.Key).FirstOrDefault().Position;
                    var workSum = (from emp in CreReports.OrderByDescending(t => t.ImportDate).Where(x => fleettype.Contains(x.FleetType) && vesselname.Contains(x.VesselName) && x.Dates.Month == item.Key)
                                   select emp.TotalHours).Sum();
                    nots.Work = workSum;
                    var RestSum = (from emp in CreReports.OrderByDescending(t => t.ImportDate).Where(x => fleettype.Contains(x.FleetType) && vesselname.Contains(x.VesselName) && x.Dates.Month == item.Key)
                                   select emp.RestHours).Sum();
                    nots.Rest = RestSum;

                    //var monthnumber = sc1.CreReports.OrderByDescending(t => t.ImportDate).Where(x => x.Dates.Month == item.Key).FirstOrDefault().Dates.Month;

                    nots.Months = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(item.Key);

                    notilist.Add(nots);
                }

                notilist = notilist.OrderBy(m => DateTime.ParseExact(m.Months, "MMMM", CultureInfo.InvariantCulture)).ToList();

            }
            else
            {
                if (Convert.ToDateTime(datefrom).Month.Equals(Convert.ToDateTime(dateto).Month))
                {
                    var emps1 = from em in CreReports.OrderBy(x => x.Dates).Where(x => fleettype.Contains(x.FleetType) && vesselname.Contains(x.VesselName) && (x.Dates >= datefrom && x.Dates <= dateto)).ToList() group em by em.Dates.Day;
                    foreach (var item in emps1)
                    {
                        ChartData nots = new ChartData();
                        nots.VesselName = CreReports.Where(x => x.Dates.Day.Equals(item.Key)).FirstOrDefault().VesselName;
                        nots.Deviation = CreReports.Where(x => fleettype.Contains(x.FleetType) && vesselname.Contains(x.VesselName) && x.Dates.Day == item.Key && x.NonConfirmities != "").ToList().Count();
                        nots.Rank = CreReports.Where(x => x.Dates.Day == item.Key).FirstOrDefault().Position;
                        var workSum = (from emp in CreReports.Where(x => fleettype.Contains(x.FleetType) && vesselname.Contains(x.VesselName) && x.Dates.Day == item.Key)
                                       select emp.TotalHours);
                        nots.Work = workSum.Sum();
                        var RestSum = (from emp in CreReports.Where(x => fleettype.Contains(x.FleetType) && vesselname.Contains(x.VesselName) && x.Dates.Day == item.Key)
                                       select emp.RestHours).Sum();
                        nots.Rest = RestSum;


                        nots.Months = item.Key.ToString();


                        notilist.Add(nots);
                    }
                }
                else
                {
                    var emps = from em in CreReports.Where(x => fleettype.Contains(x.FleetType) && vesselname.Contains(x.VesselName) && (x.Dates >= datefrom && x.Dates <= dateto)).ToList() group em by em.Dates.Month;
                    foreach (var item in emps)
                    {
                        ChartData nots = new ChartData();
                        nots.VesselName = CreReports.OrderByDescending(t => t.ImportDate).Where(x => x.Dates.Month == item.Key).FirstOrDefault().VesselName;
                        nots.Deviation = CreReports.OrderByDescending(t => t.ImportDate).Where(x => fleettype.Contains(x.FleetType) && vesselname.Contains(x.VesselName) && x.Dates.Month == item.Key && x.NonConfirmities != "").ToList().Count();
                        nots.Rank = CreReports.OrderByDescending(t => t.ImportDate).Where(x => x.Dates.Month == item.Key).FirstOrDefault().Position;
                        var workSum = (from emp in CreReports.OrderByDescending(t => t.ImportDate).Where(x => fleettype.Contains(x.FleetType) && vesselname.Contains(x.VesselName) && x.Dates.Month == item.Key)
                                       select emp.TotalHours).Sum();
                        nots.Work = workSum;
                        var RestSum = (from emp in CreReports.OrderByDescending(t => t.ImportDate).Where(x => fleettype.Contains(x.FleetType) && vesselname.Contains(x.VesselName) && x.Dates.Month == item.Key)
                                       select emp.RestHours).Sum();
                        nots.Rest = RestSum;

                        //var monthnumber = sc1.CreReports.OrderByDescending(t => t.ImportDate).Where(x => x.Dates.Month == item.Key).FirstOrDefault().Dates.Month;

                        nots.Months = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(item.Key);

                        notilist.Add(nots);
                    }

                    notilist = notilist.OrderBy(m => DateTime.ParseExact(m.Months, "MMMM", CultureInfo.InvariantCulture)).ToList();

                }
            }

            return notilist;

        }

        public override List<ChartData> getChartDataFTRK(DateTime? datefrom, DateTime? dateto, string[] fleettype, string[] rank, int? noofday)
        {
            int days = DateTime.DaysInMonth(Convert.ToDateTime(datefrom).Year, Convert.ToDateTime(datefrom).Month);
            List<ChartData> notilist = new List<ChartData>();

            if (noofday >= 335)
            {
                var emps = from em in CreReports.Where(x => fleettype.Contains(x.FleetType) && rank.Contains(x.Position) && (x.Dates >= datefrom && x.Dates <= dateto)).ToList() group em by em.Dates.Year;
                foreach (var item in emps)
                {
                    ChartData nots = new ChartData();
                    nots.VesselName = CreReports.OrderByDescending(t => t.ImportDate).Where(x => x.Dates.Year == item.Key).FirstOrDefault().VesselName;
                    nots.Deviation = CreReports.OrderByDescending(t => t.ImportDate).Where(x => fleettype.Contains(x.FleetType) && rank.Contains(x.Position) && x.Dates.Year == item.Key && x.NonConfirmities != "").ToList().Count();
                    nots.Rank = CreReports.OrderByDescending(t => t.ImportDate).Where(x => x.Dates.Year == item.Key).FirstOrDefault().Position;
                    var workSum = (from emp in CreReports.OrderByDescending(t => t.ImportDate).Where(x => fleettype.Contains(x.FleetType) && rank.Contains(x.Position) && x.Dates.Year == item.Key)
                                   select emp.TotalHours).Sum();
                    nots.Work = workSum;
                    var RestSum = (from emp in CreReports.OrderByDescending(t => t.ImportDate).Where(x => fleettype.Contains(x.FleetType) && rank.Contains(x.Position) && x.Dates.Year == item.Key)
                                   select emp.RestHours).Sum();
                    nots.Rest = RestSum;

                    nots.Months = item.Key.ToString();

                    notilist.Add(nots);
                }
            }
            else if (noofday > days && noofday < 335)
            {
                var emps = from em in CreReports.Where(x => fleettype.Contains(x.FleetType) && rank.Contains(x.Position) && (x.Dates >= datefrom && x.Dates <= dateto)).ToList() group em by em.Dates.Month;
                foreach (var item in emps)
                {
                    ChartData nots = new ChartData();
                    nots.VesselName = CreReports.OrderByDescending(t => t.ImportDate).Where(x => x.Dates.Month == item.Key).FirstOrDefault().VesselName;
                    nots.Deviation = CreReports.OrderByDescending(t => t.ImportDate).Where(x => fleettype.Contains(x.FleetType) && rank.Contains(x.Position) && x.Dates.Month == item.Key && x.NonConfirmities != "").ToList().Count();
                    nots.Rank = CreReports.OrderByDescending(t => t.ImportDate).Where(x => x.Dates.Month == item.Key).FirstOrDefault().Position;
                    var workSum = (from emp in CreReports.OrderByDescending(t => t.ImportDate).Where(x => fleettype.Contains(x.FleetType) && rank.Contains(x.Position) && x.Dates.Month == item.Key)
                                   select emp.TotalHours).Sum();
                    nots.Work = workSum;
                    var RestSum = (from emp in CreReports.OrderByDescending(t => t.ImportDate).Where(x => fleettype.Contains(x.FleetType) && rank.Contains(x.Position) && x.Dates.Month == item.Key)
                                   select emp.RestHours).Sum();
                    nots.Rest = RestSum;

                    //var monthnumber = sc1.CreReports.OrderByDescending(t => t.ImportDate).Where(x => x.Dates.Month == item.Key).FirstOrDefault().Dates.Month;

                    nots.Months = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(item.Key);

                    notilist.Add(nots);
                }

                notilist = notilist.OrderBy(m => DateTime.ParseExact(m.Months, "MMMM", CultureInfo.InvariantCulture)).ToList();

            }
            else
            {
                if (Convert.ToDateTime(datefrom).Month.Equals(Convert.ToDateTime(dateto).Month))
                {
                    var emps1 = from em in CreReports.OrderBy(x => x.Dates).Where(x => fleettype.Contains(x.FleetType) && rank.Contains(x.Position) && (x.Dates >= datefrom && x.Dates <= dateto)).ToList() group em by em.Dates.Day;
                    foreach (var item in emps1)
                    {
                        ChartData nots = new ChartData();
                        nots.VesselName = CreReports.Where(x => x.Dates.Day.Equals(item.Key)).FirstOrDefault().VesselName;
                        nots.Deviation = CreReports.Where(x => fleettype.Contains(x.FleetType) && rank.Contains(x.Position) && x.Dates.Day == item.Key && x.NonConfirmities != "").ToList().Count();
                        nots.Rank = CreReports.Where(x => x.Dates.Day == item.Key).FirstOrDefault().Position;
                        var workSum = (from emp in CreReports.Where(x => fleettype.Contains(x.FleetType) && rank.Contains(x.Position) && x.Dates.Day == item.Key)
                                       select emp.TotalHours);
                        nots.Work = workSum.Sum();
                        var RestSum = (from emp in CreReports.Where(x => fleettype.Contains(x.FleetType) && rank.Contains(x.Position) && x.Dates.Day == item.Key)
                                       select emp.RestHours).Sum();
                        nots.Rest = RestSum;


                        nots.Months = item.Key.ToString();


                        notilist.Add(nots);
                    }
                }
                else
                {
                    var emps = from em in CreReports.Where(x => fleettype.Contains(x.FleetType) && rank.Contains(x.Position) && (x.Dates >= datefrom && x.Dates <= dateto)).ToList() group em by em.Dates.Month;
                    foreach (var item in emps)
                    {
                        ChartData nots = new ChartData();
                        nots.VesselName = CreReports.OrderByDescending(t => t.ImportDate).Where(x => x.Dates.Month == item.Key).FirstOrDefault().VesselName;
                        nots.Deviation = CreReports.OrderByDescending(t => t.ImportDate).Where(x => fleettype.Contains(x.FleetType) && rank.Contains(x.Position) && x.Dates.Month == item.Key && x.NonConfirmities != "").ToList().Count();
                        nots.Rank = CreReports.OrderByDescending(t => t.ImportDate).Where(x => x.Dates.Month == item.Key).FirstOrDefault().Position;
                        var workSum = (from emp in CreReports.OrderByDescending(t => t.ImportDate).Where(x => fleettype.Contains(x.FleetType) && rank.Contains(x.Position) && x.Dates.Month == item.Key)
                                       select emp.TotalHours).Sum();
                        nots.Work = workSum;
                        var RestSum = (from emp in CreReports.OrderByDescending(t => t.ImportDate).Where(x => fleettype.Contains(x.FleetType) && rank.Contains(x.Position) && x.Dates.Month == item.Key)
                                       select emp.RestHours).Sum();
                        nots.Rest = RestSum;

                        //var monthnumber = sc1.CreReports.OrderByDescending(t => t.ImportDate).Where(x => x.Dates.Month == item.Key).FirstOrDefault().Dates.Month;

                        nots.Months = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(item.Key);

                        notilist.Add(nots);
                    }

                    notilist = notilist.OrderBy(m => DateTime.ParseExact(m.Months, "MMMM", CultureInfo.InvariantCulture)).ToList();

                }
            }

            return notilist;

        }

        public override List<ChartData> getChartDataFNVNRK(DateTime? datefrom, DateTime? dateto, string[] fleetname, string[] vesselname, string[] rank, int? noofday)
        {
            int days = DateTime.DaysInMonth(Convert.ToDateTime(datefrom).Year, Convert.ToDateTime(datefrom).Month);
            List<ChartData> notilist = new List<ChartData>();

            if (noofday >= 335)
            {
                var emps = from em in CreReports.Where(x => fleetname.Contains(x.FleetName) && vesselname.Contains(x.VesselName) && rank.Contains(x.Position) && (x.Dates >= datefrom && x.Dates <= dateto)).ToList() group em by em.Dates.Year;
                foreach (var item in emps)
                {
                    ChartData nots = new ChartData();
                    nots.VesselName = CreReports.OrderByDescending(t => t.ImportDate).Where(x => x.Dates.Year == item.Key).FirstOrDefault().VesselName;
                    nots.Deviation = CreReports.OrderByDescending(t => t.ImportDate).Where(x => fleetname.Contains(x.FleetName) && vesselname.Contains(x.VesselName) && rank.Contains(x.Position) && x.Dates.Year == item.Key && x.NonConfirmities != "").ToList().Count();
                    nots.Rank = CreReports.OrderByDescending(t => t.ImportDate).Where(x => x.Dates.Year == item.Key).FirstOrDefault().Position;
                    var workSum = (from emp in CreReports.OrderByDescending(t => t.ImportDate).Where(x => fleetname.Contains(x.FleetName) && vesselname.Contains(x.VesselName) && rank.Contains(x.Position) && x.Dates.Year == item.Key)
                                   select emp.TotalHours).Sum();
                    nots.Work = workSum;
                    var RestSum = (from emp in CreReports.OrderByDescending(t => t.ImportDate).Where(x => fleetname.Contains(x.FleetName) && vesselname.Contains(x.VesselName) && rank.Contains(x.Position) && x.Dates.Year == item.Key)
                                   select emp.RestHours).Sum();
                    nots.Rest = RestSum;

                    nots.Months = item.Key.ToString();

                    notilist.Add(nots);
                }
            }
            else if (noofday > days && noofday < 335)
            {
                var emps = from em in CreReports.Where(x => fleetname.Contains(x.FleetName) && vesselname.Contains(x.VesselName) && rank.Contains(x.Position) && (x.Dates >= datefrom && x.Dates <= dateto)).ToList() group em by em.Dates.Month;
                foreach (var item in emps)
                {
                    ChartData nots = new ChartData();
                    nots.VesselName = CreReports.OrderByDescending(t => t.ImportDate).Where(x => x.Dates.Month == item.Key).FirstOrDefault().VesselName;
                    nots.Deviation = CreReports.OrderByDescending(t => t.ImportDate).Where(x => fleetname.Contains(x.FleetName) && vesselname.Contains(x.VesselName) && rank.Contains(x.Position) && x.Dates.Month == item.Key && x.NonConfirmities != "").ToList().Count();
                    nots.Rank = CreReports.OrderByDescending(t => t.ImportDate).Where(x => x.Dates.Month == item.Key).FirstOrDefault().Position;
                    var workSum = (from emp in CreReports.OrderByDescending(t => t.ImportDate).Where(x => fleetname.Contains(x.FleetName) && vesselname.Contains(x.VesselName) && rank.Contains(x.Position) && x.Dates.Month == item.Key)
                                   select emp.TotalHours).Sum();
                    nots.Work = workSum;
                    var RestSum = (from emp in CreReports.OrderByDescending(t => t.ImportDate).Where(x => fleetname.Contains(x.FleetName) && vesselname.Contains(x.VesselName) && rank.Contains(x.Position) && x.Dates.Month == item.Key)
                                   select emp.RestHours).Sum();
                    nots.Rest = RestSum;

                    //var monthnumber = sc1.CreReports.OrderByDescending(t => t.ImportDate).Where(x => x.Dates.Month == item.Key).FirstOrDefault().Dates.Month;

                    nots.Months = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(item.Key);

                    notilist.Add(nots);
                }

                notilist = notilist.OrderBy(m => DateTime.ParseExact(m.Months, "MMMM", CultureInfo.InvariantCulture)).ToList();

            }
            else
            {
                if (Convert.ToDateTime(datefrom).Month.Equals(Convert.ToDateTime(dateto).Month))
                {
                    var emps1 = from em in CreReports.OrderBy(x => x.Dates).Where(x => fleetname.Contains(x.FleetName) && vesselname.Contains(x.VesselName) && rank.Contains(x.Position) && (x.Dates >= datefrom && x.Dates <= dateto)).ToList() group em by em.Dates.Day;
                    foreach (var item in emps1)
                    {
                        ChartData nots = new ChartData();
                        nots.VesselName = CreReports.Where(x => x.Dates.Day.Equals(item.Key)).FirstOrDefault().VesselName;
                        nots.Deviation = CreReports.Where(x => fleetname.Contains(x.FleetName) && vesselname.Contains(x.VesselName) && rank.Contains(x.Position) && x.Dates.Day == item.Key && x.NonConfirmities != "").ToList().Count();
                        nots.Rank = CreReports.Where(x => x.Dates.Day == item.Key).FirstOrDefault().Position;
                        var workSum = (from emp in CreReports.Where(x => fleetname.Contains(x.FleetName) && vesselname.Contains(x.VesselName) && rank.Contains(x.Position) && x.Dates.Day == item.Key)
                                       select emp.TotalHours);
                        nots.Work = workSum.Sum();
                        var RestSum = (from emp in CreReports.Where(x => fleetname.Contains(x.FleetName) && vesselname.Contains(x.VesselName) && rank.Contains(x.Position) && x.Dates.Day == item.Key)
                                       select emp.RestHours).Sum();
                        nots.Rest = RestSum;


                        nots.Months = item.Key.ToString();


                        notilist.Add(nots);
                    }
                }
                else
                {
                    var emps = from em in CreReports.Where(x => fleetname.Contains(x.FleetName) && vesselname.Contains(x.VesselName) && rank.Contains(x.Position) && (x.Dates >= datefrom && x.Dates <= dateto)).ToList() group em by em.Dates.Month;
                    foreach (var item in emps)
                    {
                        ChartData nots = new ChartData();
                        nots.VesselName = CreReports.OrderByDescending(t => t.ImportDate).Where(x => x.Dates.Month == item.Key).FirstOrDefault().VesselName;
                        nots.Deviation = CreReports.OrderByDescending(t => t.ImportDate).Where(x => fleetname.Contains(x.FleetName) && vesselname.Contains(x.VesselName) && rank.Contains(x.Position) && x.Dates.Month == item.Key && x.NonConfirmities != "").ToList().Count();
                        nots.Rank = CreReports.OrderByDescending(t => t.ImportDate).Where(x => x.Dates.Month == item.Key).FirstOrDefault().Position;
                        var workSum = (from emp in CreReports.OrderByDescending(t => t.ImportDate).Where(x => fleetname.Contains(x.FleetName) && vesselname.Contains(x.VesselName) && rank.Contains(x.Position) && x.Dates.Month == item.Key)
                                       select emp.TotalHours).Sum();
                        nots.Work = workSum;
                        var RestSum = (from emp in CreReports.OrderByDescending(t => t.ImportDate).Where(x => fleetname.Contains(x.FleetName) && vesselname.Contains(x.VesselName) && rank.Contains(x.Position) && x.Dates.Month == item.Key)
                                       select emp.RestHours).Sum();
                        nots.Rest = RestSum;

                        //var monthnumber = sc1.CreReports.OrderByDescending(t => t.ImportDate).Where(x => x.Dates.Month == item.Key).FirstOrDefault().Dates.Month;

                        nots.Months = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(item.Key);

                        notilist.Add(nots);
                    }

                    notilist = notilist.OrderBy(m => DateTime.ParseExact(m.Months, "MMMM", CultureInfo.InvariantCulture)).ToList();


                }
            }

            return notilist;

        }

        public override List<ChartData> getChartDataFTFNRK(DateTime? datefrom, DateTime? dateto, string[] fleettype, string[] fleetname, string[] rank, int? noofday)
        {
            int days = DateTime.DaysInMonth(Convert.ToDateTime(datefrom).Year, Convert.ToDateTime(datefrom).Month);
            List<ChartData> notilist = new List<ChartData>();

            if (noofday >= 335)
            {
                var emps = from em in CreReports.Where(x => fleettype.Contains(x.FleetType) && fleetname.Contains(x.FleetName) && rank.Contains(x.Position) && (x.Dates >= datefrom && x.Dates <= dateto)).ToList() group em by em.Dates.Year;
                foreach (var item in emps)
                {
                    ChartData nots = new ChartData();
                    nots.VesselName = CreReports.OrderByDescending(t => t.ImportDate).Where(x => x.Dates.Year == item.Key).FirstOrDefault().VesselName;
                    nots.Deviation = CreReports.OrderByDescending(t => t.ImportDate).Where(x => fleettype.Contains(x.FleetType) && fleetname.Contains(x.FleetName) && rank.Contains(x.Position) && x.Dates.Year == item.Key && x.NonConfirmities != "").ToList().Count();
                    nots.Rank = CreReports.OrderByDescending(t => t.ImportDate).Where(x => x.Dates.Year == item.Key).FirstOrDefault().Position;
                    var workSum = (from emp in CreReports.OrderByDescending(t => t.ImportDate).Where(x => fleettype.Contains(x.FleetType) && fleetname.Contains(x.FleetName) && rank.Contains(x.Position) && x.Dates.Year == item.Key)
                                   select emp.TotalHours).Sum();
                    nots.Work = workSum;
                    var RestSum = (from emp in CreReports.OrderByDescending(t => t.ImportDate).Where(x => fleettype.Contains(x.FleetType) && fleetname.Contains(x.FleetName) && rank.Contains(x.Position) && x.Dates.Year == item.Key)
                                   select emp.RestHours).Sum();
                    nots.Rest = RestSum;

                    nots.Months = item.Key.ToString();

                    notilist.Add(nots);
                }
            }
            else if (noofday > days && noofday < 335)
            {
                var emps = from em in CreReports.Where(x => fleettype.Contains(x.FleetType) && fleetname.Contains(x.FleetName) && rank.Contains(x.Position) && (x.Dates >= datefrom && x.Dates <= dateto)).ToList() group em by em.Dates.Month;
                foreach (var item in emps)
                {
                    ChartData nots = new ChartData();
                    nots.VesselName = CreReports.OrderByDescending(t => t.ImportDate).Where(x => x.Dates.Month == item.Key).FirstOrDefault().VesselName;
                    nots.Deviation = CreReports.OrderByDescending(t => t.ImportDate).Where(x => fleettype.Contains(x.FleetType) && fleetname.Contains(x.FleetName) && rank.Contains(x.Position) && x.Dates.Month == item.Key && x.NonConfirmities != "").ToList().Count();
                    nots.Rank = CreReports.OrderByDescending(t => t.ImportDate).Where(x => x.Dates.Month == item.Key).FirstOrDefault().Position;
                    var workSum = (from emp in CreReports.OrderByDescending(t => t.ImportDate).Where(x => fleettype.Contains(x.FleetType) && fleetname.Contains(x.FleetName) && rank.Contains(x.Position) && x.Dates.Month == item.Key)
                                   select emp.TotalHours).Sum();
                    nots.Work = workSum;
                    var RestSum = (from emp in CreReports.OrderByDescending(t => t.ImportDate).Where(x => fleettype.Contains(x.FleetType) && fleetname.Contains(x.FleetName) && rank.Contains(x.Position) && x.Dates.Month == item.Key)
                                   select emp.RestHours).Sum();
                    nots.Rest = RestSum;

                    //var monthnumber = sc1.CreReports.OrderByDescending(t => t.ImportDate).Where(x => x.Dates.Month == item.Key).FirstOrDefault().Dates.Month;

                    nots.Months = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(item.Key);

                    notilist.Add(nots);
                }

                notilist = notilist.OrderBy(m => DateTime.ParseExact(m.Months, "MMMM", CultureInfo.InvariantCulture)).ToList();

            }
            else
            {
                if (Convert.ToDateTime(datefrom).Month.Equals(Convert.ToDateTime(dateto).Month))
                {
                    var emps1 = from em in CreReports.OrderBy(x => x.Dates).Where(x => fleettype.Contains(x.FleetType) && fleetname.Contains(x.FleetName) && rank.Contains(x.Position) && (x.Dates >= datefrom && x.Dates <= dateto)).ToList() group em by em.Dates.Day;
                    foreach (var item in emps1)
                    {
                        ChartData nots = new ChartData();
                        nots.VesselName = CreReports.Where(x => x.Dates.Day.Equals(item.Key)).FirstOrDefault().VesselName;
                        nots.Deviation = CreReports.Where(x => fleettype.Contains(x.FleetType) && fleetname.Contains(x.FleetName) && rank.Contains(x.Position) && x.Dates.Day == item.Key && x.NonConfirmities != "").ToList().Count();
                        nots.Rank = CreReports.Where(x => x.Dates.Day == item.Key).FirstOrDefault().Position;
                        var workSum = (from emp in CreReports.Where(x => fleettype.Contains(x.FleetType) && fleetname.Contains(x.FleetName) && rank.Contains(x.Position) && x.Dates.Day == item.Key)
                                       select emp.TotalHours);
                        nots.Work = workSum.Sum();
                        var RestSum = (from emp in CreReports.Where(x => fleettype.Contains(x.FleetType) && fleetname.Contains(x.FleetName) && rank.Contains(x.Position) && x.Dates.Day == item.Key)
                                       select emp.RestHours).Sum();
                        nots.Rest = RestSum;


                        nots.Months = item.Key.ToString();
                        notilist.Add(nots);
                    }
                }
                else
                {
                    var emps = from em in CreReports.Where(x => fleettype.Contains(x.FleetType) && fleetname.Contains(x.FleetName) && rank.Contains(x.Position) && (x.Dates >= datefrom && x.Dates <= dateto)).ToList() group em by em.Dates.Month;
                    foreach (var item in emps)
                    {
                        ChartData nots = new ChartData();
                        nots.VesselName = CreReports.OrderByDescending(t => t.ImportDate).Where(x => x.Dates.Month == item.Key).FirstOrDefault().VesselName;
                        nots.Deviation = CreReports.OrderByDescending(t => t.ImportDate).Where(x => fleettype.Contains(x.FleetType) && fleetname.Contains(x.FleetName) && rank.Contains(x.Position) && x.Dates.Month == item.Key && x.NonConfirmities != "").ToList().Count();
                        nots.Rank = CreReports.OrderByDescending(t => t.ImportDate).Where(x => x.Dates.Month == item.Key).FirstOrDefault().Position;
                        var workSum = (from emp in CreReports.OrderByDescending(t => t.ImportDate).Where(x => fleettype.Contains(x.FleetType) && fleetname.Contains(x.FleetName) && rank.Contains(x.Position) && x.Dates.Month == item.Key)
                                       select emp.TotalHours).Sum();
                        nots.Work = workSum;
                        var RestSum = (from emp in CreReports.OrderByDescending(t => t.ImportDate).Where(x => fleettype.Contains(x.FleetType) && fleetname.Contains(x.FleetName) && rank.Contains(x.Position) && x.Dates.Month == item.Key)
                                       select emp.RestHours).Sum();
                        nots.Rest = RestSum;

                        //var monthnumber = sc1.CreReports.OrderByDescending(t => t.ImportDate).Where(x => x.Dates.Month == item.Key).FirstOrDefault().Dates.Month;

                        nots.Months = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(item.Key);

                        notilist.Add(nots);
                    }

                    notilist = notilist.OrderBy(m => DateTime.ParseExact(m.Months, "MMMM", CultureInfo.InvariantCulture)).ToList();

                }

            }

            return notilist;

        }
        */

    }
}
