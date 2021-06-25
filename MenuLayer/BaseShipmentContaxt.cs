using AnalysisLayer;
using CertificationLayer;
using CompanyLayer;
using CrewReportLayer;
using MSMPmodule;
using NotificationLayer;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using UserLayer;
using VesselLayer;

namespace MenuLayer
{
    public abstract class BaseShipmentContaxt : DbContext
    {
        public DbSet<SmartMenu> SmartMenus { get; set; }
        public DbSet<Menu> Menus { get; set; }
        public DbSet<SubMenu> SubMenus { get; set; }

        public DbSet<ShipSpecificContent> ShipSpecificContents { get; set; }

        public DbSet<MasterRevision> MasterRevisions { get; set; }
        public DbSet<ShipSpecificAttachment> ShipSpecificAttachments { get; set; }
        public DbSet<LoginEmp> LoginEmps { get; set; }
        public DbSet<Membership1> Memberships { get; set; }
        public DbSet<Company> Companys { get; set; }
        public DbSet<Vessel> Vessels { get; set; }

        public DbSet<Document> Documents { get; set; }

        public DbSet<TempDocsPage> TempDocsPages { get; set; }
        public DbSet<CertificationClass> Certifications { get; set; }
        public DbSet<CommentCirtificate> Comments { get; set; }
        public DbSet<CommentClass> Comments1 { get; set; }
        public DbSet<CertificateList> CertificateLists { get; set; }

        public DbSet<CreReportClass> CreReports { get; set; }
        public DbSet<CrewDetailClass> CrewDetails { get; set; }
        public DbSet<NotificationClass> Notifications { get; set; }
        public DbSet<UserClass> Users { get; set; }

        public DbSet<FleetNameClass> FleetNames { get; set; }
        public DbSet<FleetTypeClass> FleetTypes { get; set; }
        public DbSet<ImportLogClass> ImportLogs { get; set; }

        //public abstract List<NotificationDashboard> getNotiDashbord { get; }

        public DbSet<TextEditors> TextEditors { get; set; }

        public DbSet<Revision> Revisions { get; set; }

        public DbSet<DocsPages> DocsPagges { get; set; }
        public DbSet<MasterCommon> MasterCommons { get; set; }
        public DbSet<RopeInspectionSetting> RopeInspectionSettings { get; set; }
        public DbSet<RopeTailInspectionSetting> RopeTailInspectionSettings { get; set; }
        public DbSet<LooseEquipInspectionSetting> LooseEquipInspectionSettings { get; set; }
        public DbSet<WinchRotationSetting> WinchRotationSettings { get; set; }
        public DbSet<NotificationInfo> NotificationInfos { get; set; }
        public DbSet<RopeTypeCommon> RopeTypesCommon { get; set; }

        public abstract List<ChartData> getChartData(DateTime? datefrom, DateTime? dateto, int? noofday);
        public abstract List<ChartData> getChartDataVN(DateTime? datefrom, DateTime? dateto, string[] vesselname);
        public abstract List<ChartData> getChartDataVNRK(DateTime? datefrom, DateTime? dateto, string[] vesselname, string[] rank);
        public abstract List<ChartData> getChartDataVNRK_Bar(DateTime? datefrom, DateTime? dateto, string[] vesselname, string[] rank, string[] FleetType, string[] FleetName);
        // 
        //public abstract List<ChartData> getChartData1(DateTime? datefrom, DateTime? dateto, string[] Rank, int? noofday);

        /*
        public abstract List<ChartData> getChartDataVNRK(DateTime? datefrom, DateTime? dateto, string[] vesselName, string[] Rank, int? noofday);
        public abstract List<ChartData> getChartDataFT(DateTime? datefrom, DateTime? dateto, string[] fleettype, int? noofday);
        public abstract List<ChartData> getChartDataFN(DateTime? datefrom, DateTime? dateto, string[] fleetname, int? noofday);
        public abstract List<ChartData> getChartDataVN(DateTime? datefrom, DateTime? dateto, string[] vesselname, int? noofday);
        public abstract List<ChartData> getChartDataRK(DateTime? datefrom, DateTime? dateto, string[] rank, int? noofday);
        public abstract List<ChartData> getChartDataFTFN(DateTime? datefrom, DateTime? dateto, string[] fleettype, string[] fleetname, int? noofday);
        public abstract List<ChartData> getChartDataFTFNVN(DateTime? datefrom, DateTime? dateto, string[] fleettype, string[] fleetname, string[] vesselname, int? noofday);
        public abstract List<ChartData> getChartDataFTFNVNRK(DateTime? datefrom, DateTime? dateto, string[] fleettype, string[] fleetname, string[] vesselname, string[] rank, int? noofday);
        public abstract List<ChartData> getChartDataFNRK(DateTime? datefrom, DateTime? dateto,  string[] fleetname,  string[] rank, int? noofday);
        public abstract List<ChartData> getChartDataFNVN(DateTime? datefrom, DateTime? dateto, string[] fleetname, string[] vesselname, int? noofday);
        public abstract List<ChartData> getChartDataFTVN(DateTime? datefrom, DateTime? dateto, string[] fleettype, string[] vesselname, int? noofday);
        public abstract List<ChartData> getChartDataFTRK(DateTime? datefrom, DateTime? dateto, string[] fleettype, string[] rank, int? noofday);
        public abstract List<ChartData> getChartDataFNVNRK(DateTime? datefrom, DateTime? dateto, string[] fleetname, string[] vesselname, string[] rank, int? noofday);
        public abstract List<ChartData> getChartDataFTFNRK(DateTime? datefrom, DateTime? dateto, string[] fleettype, string[] fleetname, string[] rank, int? noofday);
        */

    }
}
