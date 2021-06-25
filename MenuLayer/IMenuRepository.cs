using AnalysisLayer;
using CertificationLayer;
using CompanyLayer;
using CrewReportLayer;
using NotificationLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using UserLayer;
using VesselLayer;

namespace MenuLayer
{
    public interface IMenuRepository
    {
        IQueryable<Menu> Menus { get; }
        IQueryable<SubMenu> SubMenus { get; }
        //IQueryable<SubSubMenu> SubSubMenus { get; }
        IQueryable<LoginEmp> LoginEmps { get; }
        IQueryable<Company> Companys { get; }
        IQueryable<Vessel> Vessels { get; }
        IQueryable<CertificationClass> Certifications { get; }
        IQueryable<CommentCirtificate> Comments { get; }
        IQueryable<CommentClass> Comments1 { get; }
        IQueryable<CertificateList> CertificateLists { get; }
        IQueryable<CreReportClass> CreReports { get; }
        IQueryable<CrewDetailClass> CrewDetails { get; }
        IQueryable<NotificationClass> Notifications { get; }

        IQueryable<UserClass> Users { get; }
        IQueryable<FleetNameClass> FleetNames { get; }
        IQueryable<FleetTypeClass> FleetTypes { get; }
        IQueryable<ImportLogClass> ImportLogs { get; }
        IEnumerable<NotificationDashboard> getNotiDashbord { get; }
        IEnumerable<ChartData> getGraphicalView(List<CreReportClass> data, DateTime datefrom, DateTime dateto);


        //IEnumerable<ImportLogClass> getImportLog { get; }

        IEnumerable<SelectListItem> AutoCompletedFleetType { get; }
        IEnumerable<SelectListItem> AutoCompletedfleetname { get; }
        IEnumerable<SelectListItem> AutoCompletevessel { get; }
        //IEnumerable<SelectListItem> AutoCompletevessel1 { get; }
        IEnumerable<SelectListItem> AutoCompleterank { get; }

        IQueryable<MasterCommon> MasterCommons { get; }

        //Func<ShipmentContaxt, IQueryable<Menu>> MenusBms { get; }


    }
}
