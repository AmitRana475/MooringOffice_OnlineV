using System;
using System.Linq;
using CertificationLayer;
using CompanyLayer;
using CrewReportLayer;
using NotificationLayer;
using UserLayer;
using VesselLayer;
using System.Collections.Generic;
using System.Web.Mvc;
using AnalysisLayer;

namespace MenuLayer
{
    public class EFMenuRepository : IMenuRepository
    {

        private readonly ShipmentContaxt contaxt;

        public EFMenuRepository()
        {
            contaxt = new ShipmentContaxt();

            contaxt.Configuration.AutoDetectChangesEnabled = false;
            contaxt.Configuration.ProxyCreationEnabled = false;
        }

        public IQueryable<Menu> Menus
        {
            get
            {
                //return contaxt.Menus.Include("Menus1").Include("Menus2");
                return contaxt.Menus.Include("Menus1");
            }


        }
        public IQueryable<SubMenu> SubMenus
        {
            get
            {
                return contaxt.SubMenus.Include("Menus2");
            }


        }
        //public IQueryable<SubSubMenu> SubSubMenus
        //{
        //    get
        //    {
        //        return contaxt.SubSubMenus.Include("Menus2");
        //    }


        //}
        public IQueryable<CertificateList> CertificateLists
        {
            get
            {
                return contaxt.CertificateLists;
            }

        }

        public IQueryable<CertificationClass> Certifications
        {
            get
            {
                return contaxt.Certifications.Include("Comment1");
            }

        }

        public IQueryable<CommentCirtificate> Comments
        {
            get
            {
                return contaxt.Comments;
            }

        }

        public IQueryable<CommentClass> Comments1
        {
            get
            {
                return contaxt.Comments1;
            }


        }

        public IQueryable<Company> Companys
        {
            get
            {
                return contaxt.Companys;
            }

        }

        public IQueryable<CreReportClass> CreReports
        {
            get
            {
                return contaxt.CreReports;
            }


        }

        public IQueryable<CrewDetailClass> CrewDetails
        {
            get
            {
                return contaxt.CrewDetails;
            }

        }

        public IQueryable<FleetNameClass> FleetNames
        {
            get
            {
                return contaxt.FleetNames;
            }

        }

        public IQueryable<FleetTypeClass> FleetTypes
        {
            get
            {
                return contaxt.FleetTypes;
            }


        }

        public IQueryable<ImportLogClass> ImportLogs
        {
            get
            {
                return contaxt.ImportLogs;
            }


        }
        public IQueryable<LoginEmp> LoginEmps
        {
            get
            {
                return contaxt.LoginEmps;
            }
        }



        public IQueryable<NotificationClass> Notifications
        {
            get
            {
                return contaxt.Notifications.Include("Comment1");
            }


        }

        public IQueryable<CreReportClass> CrewWorkReport
        {
            get
            {
                return contaxt.CreReports;
            }
        }

        public IQueryable<UserClass> Users
        {
            get
            {
                return contaxt.Users;
            }


        }



        public IQueryable<Vessel> Vessels
        {
            get
            {
                return contaxt.Vessels;
            }


        }

        public IQueryable<MasterCommon> MasterCommons
        {
            get
            {
                return contaxt.MasterCommons;
            }
        }

        public IEnumerable<NotificationDashboard> getNotiDashbord
        {
            get
            {
                var notilist = new List<NotificationDashboard>();

                var dateCriteria = DateTime.Now.Date.AddDays(-7);
                var dateMonth = DateTime.Now.Date.AddMonths(-1);
                var startDate = new DateTime(dateMonth.Year, dateMonth.Month, 1);
                var endDate = startDate.AddMonths(1).AddDays(-1);
                //using (ShipmentContaxt sc1 = new ShipmentContaxt())
                //{

                //var kk = contaxt.Notifications.Where(x=> x.VesselId==9371593).ToList();


                //var emps = from em in Notifications.ToList() group em by em.VesselId;
               

                var emps = from em in contaxt.CreReports.ToList() group em by em.Vessel_ID;
                //var emps1 = from em in contaxt.CreReports.ToList() select em;
                 
                decimal val = 0.00m;

                //foreach (var item in emps)
                //{
                //    int vsid = item.Key;
                //    var vesselDetail = contaxt.Vessels.Where(x => x.Id == vsid).FirstOrDefault();
                //    // var emps1 = from em in Notifications.ToList().Where(x=> x.VesselId == vsid) select em;
                //    var emps1 = contaxt.Notifications.OrderByDescending(t => t.EDate).Where(x => x.VesselId == vsid).ToList();
                //    NotificationDashboard nots = new NotificationDashboard();
                //    var implog = contaxt.ImportLogs.Where(x => x.VesselName == vesselDetail.VesselName).OrderByDescending(t => t.DateImported).FirstOrDefault();

                //    if (emps1.Count != 0)
                //    {
                       
                       
                //        nots.VesselId = emps1.OrderByDescending(t => t.EDate).Where(x => x.VesselId == vsid).FirstOrDefault().VesselId;
                //        nots.VesselName = emps1.OrderByDescending(t => t.EDate).Where(x => x.VesselId == vsid).FirstOrDefault().VesselName;
                //        nots.FleetName = emps1.OrderByDescending(t => t.EDate).Where(x => x.VesselId == vsid).FirstOrDefault().FleetName;
                //        nots.FleetType = emps1.OrderByDescending(t => t.EDate).Where(x => x.VesselId == vsid).FirstOrDefault().FleetType;

                //        //nots.LastImport = Convert.ToDateTime(emps1.OrderByDescending(t => t.EDate).Where(x => x.VesselId == vsid).FirstOrDefault().EDate);
                //        nots.LastImport = Convert.ToDateTime(implog.DateImported);
                //        // nots.DataAvailableTill = Convert.ToDateTime(emps1.OrderByDescending(t => t.NcDate.Date).Where(x => x.VesselId == vsid).FirstOrDefault().NcDate.Date); //Convert.ToDateTime(item.OrderBy(t => t.NcDate).Where(x => x.VesselId == vsid).FirstOrDefault().NcDate);
                //        //var dataAvail = contaxt.CreReports.Where(x => x.TotalHours != val && x.Vessel_ID == vsid).OrderByDescending().
                //        nots.DataAvailableTill = Convert.ToDateTime(contaxt.CreReports.OrderByDescending(t => t.Dates).Where(x => x.Vessel_ID == vsid && x.TotalHours != val).FirstOrDefault().Dates.Date);

                //        var lastempnon = emps1.OrderByDescending(p => p.EDate).Where(p => p.Acknowledge != null && p.VesselId == vsid && p.EDate == nots.LastImport).Count();
                //        var lastemp = emps1.OrderByDescending(p => p.EDate).Where(p => p.NonConfirmity != null && p.VesselId == vsid && p.EDate == nots.LastImport).Count();

                //        nots.NCsLastImport = lastempnon + "/" + lastemp;

                //        var last7daysnon = emps1.OrderByDescending(p => p.NcDate.Date).Where(p => p.Acknowledge != null && p.VesselId == vsid && p.NcDate >= dateCriteria).Count();
                //        var last7days = emps1.OrderByDescending(p => p.NcDate.Date).Where(p => p.NonConfirmity != null && p.VesselId == vsid && p.NcDate >= dateCriteria).Count();

                //        nots.NCslast7Days = last7daysnon + "/" + last7days;

                //        var lastmonthnon = emps1.OrderByDescending(p => p.NcDate.Date).Where(p => p.Acknowledge != null && p.VesselId == vsid && p.NcDate >= startDate && p.NcDate <= endDate).Count();
                //        var lastmonth = emps1.OrderByDescending(p => p.NcDate.Date).Where(p => p.NonConfirmity != null && p.VesselId == vsid && p.NcDate >= startDate && p.NcDate <= endDate).Count();

                //        nots.NCsLastMonth = lastmonthnon + "/" + lastmonth;

                //        var totalncnon = emps1.Where(p => p.Acknowledge != null && p.VesselId == vsid).Count();
                //        var totalnc = emps1.Where(p => p.NonConfirmity != null && p.VesselId == vsid).Count();

                //        nots.NCsTotal = totalncnon + "/" + totalnc;
                //    }
                //    else
                //    {
                //        var mydata = contaxt.CreReports.Where(x => x.Vessel_ID == vsid).ToList();

                //        nots.VesselId = mydata.OrderByDescending(t => t.Dates).Where(x => x.Vessel_ID == vsid).FirstOrDefault().Vessel_ID;
                //        nots.VesselName = mydata.OrderByDescending(t => t.Dates).Where(x => x.Vessel_ID == vsid).FirstOrDefault().VesselName;
                //        nots.FleetName = mydata.OrderByDescending(t => t.Dates).Where(x => x.Vessel_ID == vsid).FirstOrDefault().FleetName;
                //        nots.FleetType = mydata.OrderByDescending(t => t.Dates).Where(x => x.Vessel_ID == vsid).FirstOrDefault().FleetType;

                //        if (implog != null)
                //        {
                //            nots.LastImport = Convert.ToDateTime(implog.DateImported);
                //        }
                //        else
                //        {
                //            nots.LastImport = Convert.ToDateTime(mydata.OrderByDescending(t => t.Dates).Where(x => x.Vessel_ID == vsid).FirstOrDefault().Dates);
                //        }

                //        // nots.DataAvailableTill = Convert.ToDateTime(mydata.OrderByDescending(t => t.Dates).Where(x => x.Vessel_ID == vsid).FirstOrDefault().NcDate.Date); //Convert.ToDateTime(item.OrderBy(t => t.NcDate).Where(x => x.VesselId == vsid).FirstOrDefault().NcDate);
                //        //var dataAvail = contaxt.CreReports.Where(x => x.TotalHours != val && x.Vessel_ID == vsid).OrderByDescending().
                //        nots.DataAvailableTill = Convert.ToDateTime(mydata.OrderByDescending(t => t.Dates).Where(x => x.Vessel_ID == vsid && x.TotalHours != val).FirstOrDefault().Dates.Date);

                //        var nncc = "0/0";

                //        var lastempnon = nncc;
                //        var lastemp = nncc;

                //        nots.NCsLastImport = nncc;

                //        var last7daysnon = nncc;
                //        var last7days = nncc;

                //        nots.NCslast7Days = nncc;

                //        var lastmonthnon = nncc;
                //        var lastmonth = nncc;

                //        nots.NCsLastMonth = nncc;

                //        var totalncnon = nncc;
                //        var totalnc = nncc;

                //        nots.NCsTotal = nncc;

                //        //var lastempnon = mydata.OrderByDescending(p => p.Dates).Where(p =>  p.Vessel_ID == vsid && p.Dates == nots.LastImport).Count();
                //        //var lastemp = mydata.OrderByDescending(p => p.Dates).Where(p => p.NonConfirmities != "" && p.Vessel_ID == vsid && p.EDate == nots.LastImport).Count();

                //        //nots.NCsLastImport = lastempnon + "/" + lastemp;

                //        //var last7daysnon = mydata.OrderByDescending(p => p.NcDate.Date).Where(p => p.Acknowledge != null && p.VesselId == vsid && p.NcDate >= dateCriteria).Count();
                //        //var last7days = mydata.OrderByDescending(p => p.NcDate.Date).Where(p => p.NonConfirmity != null && p.VesselId == vsid && p.NcDate >= dateCriteria).Count();

                //        //nots.NCslast7Days = last7daysnon + "/" + last7days;

                //        //var lastmonthnon = mydata.OrderByDescending(p => p.NcDate.Date).Where(p => p.Acknowledge != null && p.VesselId == vsid && p.NcDate >= startDate && p.NcDate <= endDate).Count();
                //        //var lastmonth = mydata.OrderByDescending(p => p.NcDate.Date).Where(p => p.NonConfirmity != null && p.VesselId == vsid && p.NcDate >= startDate && p.NcDate <= endDate).Count();

                //        //nots.NCsLastMonth = lastmonthnon + "/" + lastmonth;

                //        //var totalncnon = mydata.Where(p => p.Acknowledge != null && p.VesselId == vsid).Count();
                //        //var totalnc = mydata.Where(p => p.NonConfirmity != null && p.VesselId == vsid).Count();

                //        //nots.NCsTotal = totalncnon + "/" + totalnc;
                //    }
                //    notilist.Add(nots);
                //}

                //foreach (var item in emps)
                //{
                //    int vsid = item.Key;
                //    // var kul = contaxt.CreReports.ToList();
                //    NotificationDashboard nots = new NotificationDashboard();
                //    nots.VesselId = emps1.OrderByDescending(t => t.EDate).Where(x => x.VesselId == vsid).FirstOrDefault().VesselId;
                //    nots.VesselName = emps1.OrderByDescending(t => t.EDate).Where(x => x.VesselId == vsid).FirstOrDefault().VesselName;
                //    nots.FleetName = emps1.OrderByDescending(t => t.EDate).Where(x => x.VesselId == vsid).FirstOrDefault().FleetName;
                //    nots.FleetType = emps1.OrderByDescending(t => t.EDate).Where(x => x.VesselId == vsid).FirstOrDefault().FleetType;

                //    nots.LastImport = Convert.ToDateTime(emps1.OrderByDescending(t => t.EDate).Where(x => x.VesselId == vsid).FirstOrDefault().EDate);
                //    nots.DataAvailableTill = Convert.ToDateTime(emps1.OrderByDescending(t => t.NcDate.Date).Where(x => x.VesselId == vsid).FirstOrDefault().NcDate.Date); //Convert.ToDateTime(item.OrderBy(t => t.NcDate).Where(x => x.VesselId == vsid).FirstOrDefault().NcDate);
                //    //var dataAvail = contaxt.CreReports.Where(x => x.TotalHours != val && x.Vessel_ID == vsid).OrderByDescending().
                //    nots.DataAvailableTill = Convert.ToDateTime(contaxt.CreReports.OrderByDescending(t => t.Dates).Where(x => x.Vessel_ID == vsid && x.TotalHours != val).FirstOrDefault().Dates.Date);

                //    var lastempnon = emps1.OrderByDescending(p => p.EDate).Where(p => p.Acknowledge != null && p.VesselId == vsid && p.EDate == nots.LastImport).Count();
                //    var lastemp = emps1.OrderByDescending(p => p.EDate).Where(p => p.NonConfirmity != null && p.VesselId == vsid && p.EDate == nots.LastImport).Count();

                //    nots.NCsLastImport = lastempnon + "/" + lastemp;

                //    var last7daysnon = emps1.OrderByDescending(p => p.NcDate.Date).Where(p => p.Acknowledge != null && p.VesselId == vsid && p.NcDate >= dateCriteria).Count();
                //    var last7days = emps1.OrderByDescending(p => p.NcDate.Date).Where(p => p.NonConfirmity != null && p.VesselId == vsid && p.NcDate >= dateCriteria).Count();

                //    nots.NCslast7Days = last7daysnon + "/" + last7days;

                //    var lastmonthnon = emps1.OrderByDescending(p => p.NcDate.Date).Where(p => p.Acknowledge != null && p.VesselId == vsid && p.NcDate >= startDate && p.NcDate <= endDate).Count();
                //    var lastmonth = emps1.OrderByDescending(p => p.NcDate.Date).Where(p => p.NonConfirmity != null && p.VesselId == vsid && p.NcDate >= startDate && p.NcDate <= endDate).Count();

                //    nots.NCsLastMonth = lastmonthnon + "/" + lastmonth;

                //    var totalncnon = emps1.Where(p => p.Acknowledge != null && p.VesselId == vsid).Count();
                //    var totalnc = emps1.Where(p => p.NonConfirmity != null && p.VesselId == vsid).Count();

                //    nots.NCsTotal = totalncnon + "/" + totalnc;


                //    notilist.Add(nots);
                //}


                return notilist;
            }
        }


        public IEnumerable<ChartData> getGraphicalView(List<CreReportClass> data1, DateTime datefrom, DateTime dateto)
        {

            var data = new List<ChartData>();
            var emps = from em in data1.ToList() group em by em.Vessel_ID;
            foreach (var item in emps)
            {
                int vsid = item.Key;

                ChartData nots = new ChartData();

                nots.From = Convert.ToDateTime(item.Where(x => x.Vessel_ID == vsid).OrderBy(x => x.Dates).FirstOrDefault().Dates);
                nots.To = Convert.ToDateTime(item.Where(x => x.Vessel_ID == vsid).OrderByDescending(x => x.Dates).FirstOrDefault().Dates);
                nots.VesselId = item.Where(x => x.Vessel_ID == vsid).FirstOrDefault().Vessel_ID;
                nots.VesselName = item.Where(x => x.Vessel_ID == vsid).FirstOrDefault().VesselName;
                nots.FleetName = item.Where(x => x.Vessel_ID == vsid).FirstOrDefault().FleetName;
                nots.FleetType = item.Where(x => x.Vessel_ID == vsid).FirstOrDefault().FleetType;

                //nots.Deviation = Notifications.Where(x => x.VesselId == item.Key && (x.NcDate >= datefrom && x.NcDate <= dateto)).ToList().Count();

                //var bb = item.Where(x => x.Vessel_ID == item.Key && !string.IsNullOrEmpty(x.NonConfirmities)).ToList();
                //var Notis = new List<string>();
                //foreach (var b in bb)
                //{
                //    var dds = b.NonConfirmities.Split('\r');
                //    foreach (var ss in dds)
                //    {
                //        if (ss != "\n")
                //            Notis.Add(ss.Replace("\n",string.Empty));
                //    }
                //}
                //nots.Deviation = Notis.Count();
                //nots.Deviation = item.Where(x => x.Vessel_ID == item.Key && !string.IsNullOrEmpty(x.NonConfirmities)).Count();
                nots.Work = item.Where(x => x.Vessel_ID == item.Key).Sum(p => p.TotalHours);
                nots.Rest = item.Where(x => x.Vessel_ID == item.Key).Sum(p => p.RestHours);

                data.Add(nots);
            }

            return data;
        }


        //        public IEnumerable<ImportLogClass> getImportLog
        //        {
        //            get
        //            {
        //                var notilist = new List<ImportLogClass>();


        //                var emps = from em in CreReports.ToList() group em by em.ImportDate;


        ////                var relevantVMObjects =
        //// (from a in CreReports
        //// join   b in Notifications on a.ImportDate equals b.EDate
        ////  join c in CrewDetails on a.ImportDate equals c.ImportDate
        ////  select new
        ////  {
        ////      VesselId = a.Vessel_ID,
        ////      VesselName = a.VesselName,
        ////      ImportDate = b.EDate,
        ////      FileNames = b.FileNames
        ////  }).ToList();



        ////var sss= from em in relevantVMObjects.ToList() group em by em.ImportDate;


        //                foreach (var item in emps)
        //                {
        //                    var vsid = item.Key;

        //                    ImportLogClass nots = new ImportLogClass();
        //                    nots.DateImported = Convert.ToDateTime(item.OrderByDescending(t => t.ImportDate).Where(x => x.ImportDate == vsid).FirstOrDefault().ImportDate);
        //                    nots.DateImportFrom = Convert.ToDateTime(item.OrderBy(t => t.Dates).Where(x => x.ImportDate == vsid).FirstOrDefault().Dates);
        //                    nots.DateImportTo = Convert.ToDateTime(item.OrderByDescending(t => t.Dates).Where(x => x.ImportDate == vsid).FirstOrDefault().Dates);
        //                    nots.VesselName = item.OrderByDescending(t => t.ImportDate).Where(x => x.ImportDate == vsid).FirstOrDefault().VesselName;

        //                    nots.Filenames = item.OrderByDescending(t => t.ImportDate).Where(x => x.ImportDate == vsid).FirstOrDefault().FileNames;
        //                    nots.ImportedBy = item.OrderByDescending(t => t.ImportDate).Where(x => x.ImportDate == vsid).FirstOrDefault().ImportedBy;
        //                    nots.DataAvailbleTill = Convert.ToDateTime(item.OrderBy(t => t.Dates).Where(x => x.ImportDate == vsid).FirstOrDefault().Dates);


        //                    notilist.Add(nots);
        //                }

        //                return notilist;
        //            }
        //        }


        public IEnumerable<SelectListItem> AutoCompletedFleetType
        {
            get
            {
                var studentsvv = CreReports.Select(x => x.FleetType).Distinct().ToList();
                if (UserRole.username.ToLower() != "admin")
                {
                    var vsname = Users.Where(x => x.EmailId.Equals(UserRole.username1)).Select(x => x.VesselName).FirstOrDefault();
                    string[] vesselname = vsname.TrimEnd(',').Split(',');
                    studentsvv = CreReports.Where(x => vesselname.Contains(x.VesselName)).Select(x => x.FleetType).Distinct().ToList();
                }

                var sportsList = new List<SelectListItem>();
                foreach (var item in studentsvv)
                {
                    sportsList.Add(new SelectListItem { Text = item, Value = item.ToString() });
                }

                return sportsList;
            }
        }

        public IEnumerable<SelectListItem> AutoCompletedfleetname
        {
            get
            {
                var studentsvv = CreReports.Select(x => x.FleetName).Distinct().ToList();

                if (UserRole.username.ToLower() != "admin")
                {
                    var vsname = Users.Where(x => x.EmailId.Equals(UserRole.username1)).Select(x => x.VesselName).FirstOrDefault();
                    string[] vesselname = vsname.TrimEnd(',').Split(',');
                    studentsvv = CreReports.Where(x => vesselname.Contains(x.VesselName)).Select(x => x.FleetName).Distinct().ToList();
                }

                var sportsList = new List<SelectListItem>();
                foreach (var item in studentsvv)
                {
                    sportsList.Add(new SelectListItem { Text = item, Value = item.ToString() });
                }

                return sportsList;
            }
        }

        public IEnumerable<SelectListItem> AutoCompletevessel
        {
            get
            {
                var studentsvv = CreReports.Select(x => x.VesselName).Distinct().ToList();
                if (UserRole.username.ToLower() != "admin")
                {
                    var vsname = Users.Where(x => x.EmailId.Equals(UserRole.username1)).Select(x => x.VesselName).FirstOrDefault();
                    string[] vesselname = vsname.TrimEnd(',').Split(',');
                    studentsvv = CreReports.Where(x => vesselname.Contains(x.VesselName)).Select(x => x.VesselName).Distinct().ToList();
                }

                var sportsList = new List<SelectListItem>();
                foreach (var item in studentsvv)
                {
                    sportsList.Add(new SelectListItem { Text = item, Value = item.ToString() });
                }

                return sportsList;
            }
        }


        public IEnumerable<SelectListItem> AutoCompleterank
        {
            get
            {
                var bb = (from a in CreReports
                          join b in CrewDetails on a.UserName equals b.UserName
                          select new { b.rid, a.Position, a.VesselName }
                          ).Distinct().OrderBy(x => x.rid).ToList();

                var bb1 = bb.Select(x => x.Position).Distinct().ToList();

                if (UserRole.username.ToLower() != "admin")
                {
                    var vsname = Users.Where(x => x.EmailId.Equals(UserRole.username1)).Select(x => x.VesselName).FirstOrDefault();
                    string[] vesselname = vsname.TrimEnd(',').Split(',');
                    bb1 = bb.Where(x => vesselname.Contains(x.VesselName)).Select(x => x.Position).Distinct().ToList();
                }





                var sportsList = new List<SelectListItem>();
                foreach (var item in bb1)
                {
                    sportsList.Add(new SelectListItem { Text = item, Value = item.ToString() });
                }

                return sportsList;
            }
        }


        //public Func<ShipmentContaxt, IQueryable<Menu>> MenusBms
        //{
        //    get

        //    {
        //        return (contaxt) => contaxt.Menus.Include("Menus1");
        //    }
        //}


    }
}
