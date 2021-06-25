using MenuLayer;
using Reports;
using Shipment49Web.Common;
using Shipment49Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Shipment49Web.Areas.MSPS.Models;

namespace Shipment49Web.Areas.Data.Controllers
{
    [Authorize]
    [UserAuthenticationFilter]
    [ErrorClass]
    public class AnalysisController : BaseController
    {
        //private readonly IMenuRepository sc;
        MorringOfficeEntities context = new MorringOfficeEntities();
        ReportAnalysisFilterModel analysisFilterModel;

        //public AnalysisController(IMenuRepository repo)
        public AnalysisController()
        {
            //sc = repo;

            //if (UserRole.username == null)
            //{
            //    UserRole.username = string.Join("", Roles.GetRolesForUser());
            //}

            //ViewBag.GetMenu = UserRole.username == "Admin" ? sc.Menus.ToList() : sc.Menus.Where(x => x.Role == "User").ToList();
            //ViewBag.GetSubMenu = UserRole.username == "Admin" ? sc.SubMenus.ToList() : sc.SubMenus.Where(x => x.Role == "User").ToList();

            using (ShipmentContaxt sc1 = new ShipmentContaxt())
            {

                var _smartmenu = sc1.SmartMenus.ToList().FirstOrDefault();
                if (_smartmenu != null)
                {
                    ViewBag.MenuContent = _smartmenu.SmartMenuContent;
                }
            }
        }

        private void InitializeAnalysisFilterModel()
        {
            analysisFilterModel = new ReportAnalysisFilterModel
            {
                FleetNameList = base.PermittedFleetNames,
                FleetTypeList = base.PermittedFleetTypes,

                AgeRangeFrom = 0,
                AgeRangeTo = 50,

                WindSpeedRangeFrom = 1,
                WindSpeedRangeTo = 200,

                CurrentSpeedRangeFrom = 0,
                CurrentSpeedRangeTo = 20,

                AirTempRangeFrom = -50,
                AirTempRangeTo = 60
            };


        }

        public ActionResult Rope()
        {
            var ropeAnalysis = new RopeAnalysis
            {
                FleetNames = context.tblCommons.Where(u => u.Type == (int)CommonType.FleetName).ToList(),
                FleetTypes = context.tblCommons.Where(u => u.Type == (int)CommonType.FleetType).ToList(),
                TradePlatforms = context.tblCommons.Where(u => u.Type == (int)CommonType.TradePlatform).ToList(),
                DateFrom = DateTime.Today.AddMonths(-3),
                DateUpto = DateTime.Today,
                RunningHoursFrom = 0,
                RunningHoursTo = 100000,
                ChartData = new List<GraphData>(),
                SelectedManufacturers = string.Empty,
                SelectedRopeTypes = string.Empty,
                SelectedVessels = string.Empty
            };

            ropeAnalysis.RopeAnalysis = CommonMethods.GetRopeAnalysis(ropeAnalysis.SelectedVessels, ropeAnalysis.SelectedManufacturers,
                ropeAnalysis.SelectedRopeTypes, ropeAnalysis.RunningHoursFrom, ropeAnalysis.RunningHoursTo, (DateTime)ropeAnalysis.DateFrom, (DateTime)ropeAnalysis.DateUpto);

            for (int i = 1; i < 8; i++)
                ropeAnalysis.ChartData.Add(new GraphData() { chartId = i, data = ropeAnalysis.GetChartData(ropeAnalysis.RopeAnalysis, i) });

            return View(ropeAnalysis);
        }

        [HttpPost]
        public ActionResult Rope(RopeAnalysis ropeAnalysis)
        {
            foreach (var vid in ropeAnalysis.VesselIDs)
                ropeAnalysis.SelectedVessels = vid + "," + ropeAnalysis.SelectedVessels;

            foreach (var mid in ropeAnalysis.ManufacturerIDs)
                ropeAnalysis.SelectedManufacturers = mid + "," + ropeAnalysis.SelectedManufacturers;

            foreach (var rtid in ropeAnalysis.RopeTypeIDs)
                ropeAnalysis.SelectedRopeTypes = rtid + "," + ropeAnalysis.SelectedRopeTypes;

            if (!string.IsNullOrEmpty(ropeAnalysis.SelectedVessels))
                ropeAnalysis.SelectedVessels = ropeAnalysis.SelectedVessels.Trim(',');
            else
                ropeAnalysis.SelectedVessels = string.Empty;

            if (!string.IsNullOrEmpty(ropeAnalysis.SelectedManufacturers))
                ropeAnalysis.SelectedManufacturers = ropeAnalysis.SelectedManufacturers.Trim(',');
            else
                ropeAnalysis.SelectedManufacturers = string.Empty;

            if (!string.IsNullOrEmpty(ropeAnalysis.SelectedRopeTypes))
                ropeAnalysis.SelectedRopeTypes = ropeAnalysis.SelectedRopeTypes.Trim(',');
            else
                ropeAnalysis.SelectedRopeTypes = string.Empty;

            ropeAnalysis.RopeAnalysis = CommonMethods.GetRopeAnalysis(ropeAnalysis.SelectedVessels, ropeAnalysis.SelectedManufacturers,
                ropeAnalysis.SelectedRopeTypes, ropeAnalysis.RunningHoursFrom, ropeAnalysis.RunningHoursTo, (DateTime)ropeAnalysis.DateFrom, (DateTime)ropeAnalysis.DateUpto);

            ropeAnalysis.ChartData = new List<GraphData>();

            for (int i = 1; i < 8; i++)
                ropeAnalysis.ChartData.Add(new GraphData() { chartId = i, data = ropeAnalysis.GetChartData(ropeAnalysis.RopeAnalysis, i) });

            ropeAnalysis.FleetNames = base.PermittedFleetNames;
            ropeAnalysis.FleetTypes = base.PermittedFleetTypes;

            ropeAnalysis.VesselList = context.VesselDetails.Where(p => ropeAnalysis.FleetTypeIDs.Contains(p.FleetTypeID) && ropeAnalysis.FleetNameIDs.Contains(p.FleetNameID) && ropeAnalysis.TradeIDs.Contains(p.TradeAreaID)).ToList();

            return View(ropeAnalysis);
        }

        public ActionResult ExpenseDuration(int? id)
        {
            InitializeAnalysisFilterModel();

            analysisFilterModel.DateDiscardedFrom = DateTime.Today.AddYears(-10);
            analysisFilterModel.DateDiscardedUpto = DateTime.Today;

            var results = context.View_AnalysysExpenseVsDuration.Where(p => p.OutofServiceDate != null)
                .GroupBy(p => new { p.RopeType, p.RopeTypeId, p.ManufacturerId, p.Manufacturer })
                .Select(u => new AnalysisResult
                {
                    RopeType = u.Key.RopeType,
                    RopeTypeId = u.Key.RopeTypeId,
                    ManufacturerId = u.Key.ManufacturerId,
                    Manufacturer = u.Key.Manufacturer,
                    Cost = u.Average(p => p.Cost),
                    RunningHours = (int?)u.Average(p => p.RunningHours),
                    Avg_Months = u.Average(p => p.Avg_Months)
                });

            //var results = analysis.GroupBy(p => new { p.ImoNo, p.Cost, p.RopeType, p.RopeTypeId, p.ManufacturerId, p.Manufacturer, p.VesselName, p.RunningHours, p.Avg_Months }).
            //  Select(u => new AnalysisResult
            //  {
            //      ImoNo = u.Key.ImoNo,
            //      Cost = u.Key.Cost,
            //      RopeType = u.Key.RopeType,
            //      RopeTypeId = u.Key.RopeTypeId,
            //      ManufacturerId = u.Key.ManufacturerId,
            //      Manufacturer = u.Key.Manufacturer,
            //      VesselName = u.Key.VesselName,
            //      RunningHours = u.Key.RunningHours,
            //      Avg_Months = u.Key.Avg_Months
            //  });

            var recordsFound = results.OrderBy(u => u.Manufacturer).ThenBy(u => u.RopeType).ToList();

            int currPage = id == null ? 1 : Convert.ToInt32(id);

            TempData["CurrentPage"] = currPage;
            TempData["TotalRecords"] = recordsFound.Count();

            analysisFilterModel.AnalysisResults = recordsFound.Skip((CommonMethods.PAGESIZE * currPage) - CommonMethods.PAGESIZE).Take(CommonMethods.PAGESIZE).ToList();

            return View(analysisFilterModel);
        }

        [HttpPost]
        public ActionResult ExpenseDuration(ReportAnalysisFilterModel analysisFilterModel)
        {
            var analysysExpDuration = context.View_AnalysysExpenseVsDuration.
                Where(p => p.OutofServiceDate >= analysisFilterModel.DateDiscardedFrom && p.OutofServiceDate <= analysisFilterModel.DateDiscardedUpto).
                AsQueryable();

            if (analysisFilterModel.VesselIDs?.Count > 0)
                analysysExpDuration = analysysExpDuration.Where(p => analysisFilterModel.VesselIDs.Contains(p.ImoNo));

            if (analysisFilterModel.FleetNameIDs?.Count > 0)
                analysysExpDuration = analysysExpDuration.Where(p => analysisFilterModel.FleetNameIDs.Contains(p.FleetNameID));

            if (analysisFilterModel.FleetTypeIDs?.Count > 0)
                analysysExpDuration = analysysExpDuration.Where(p => analysisFilterModel.FleetTypeIDs.Contains(p.FleetTypeID));

            if (analysisFilterModel.TradeIDs?.Count > 0)
                analysysExpDuration = analysysExpDuration.Where(p => analysisFilterModel.TradeIDs.Contains(p.TradeAreaID));

            if (analysisFilterModel.ManufacturerIDs?.Count > 0)
                analysysExpDuration = analysysExpDuration.Where(p => analysisFilterModel.ManufacturerIDs.Contains(p.ManufacturerId ?? 0));

            if (analysisFilterModel.RopeTypeIDs?.Count > 0)
                analysysExpDuration = analysysExpDuration.Where(p => analysisFilterModel.RopeTypeIDs.Contains(p.RopeTypeId ?? 0));

            if (!string.IsNullOrEmpty(analysisFilterModel.PortNames))
                analysysExpDuration = analysysExpDuration.Where(p => analysisFilterModel.PortNames.Contains(p.PortName));

            analysisFilterModel.PortFacilityNames = analysisFilterModel.PortFacilityNames == "None Selected" ? null : analysisFilterModel.PortFacilityNames;

            if (!string.IsNullOrEmpty(analysisFilterModel.PortFacilityNames))
                analysysExpDuration = analysysExpDuration.Where(p => analysisFilterModel.PortFacilityNames.Contains(p.FacilityName));

            if (!string.IsNullOrEmpty(analysisFilterModel.BirthTypeNames))
                analysysExpDuration = analysysExpDuration.Where(p => p.BirthType == analysisFilterModel.BirthTypeNames);

            if (!string.IsNullOrEmpty(analysisFilterModel.MooringTypeNames))
                analysysExpDuration = analysysExpDuration.Where(p => p.MooringType == analysisFilterModel.MooringTypeNames);

            if (!string.IsNullOrEmpty(analysisFilterModel.LeadTypeNames))
                analysysExpDuration = analysysExpDuration.Where(p => p.Lead == analysisFilterModel.LeadTypeNames);

            if (!string.IsNullOrEmpty(analysisFilterModel.LeadNames))
                analysysExpDuration = analysysExpDuration.Where(p => p.Lead1 == analysisFilterModel.LeadNames);

            if (analysisFilterModel.Squal_Gusts.IndexOf("ALL", StringComparison.OrdinalIgnoreCase) < 0)
                analysysExpDuration = analysysExpDuration.Where(p => p.AnySquall == analysisFilterModel.Squal_Gusts);

            if (analysisFilterModel.BerthExposesToSwell.IndexOf("ALL", StringComparison.OrdinalIgnoreCase) < 0)
                analysysExpDuration = analysysExpDuration.Where(p => p.Berth_exposed_SeaSwell == analysisFilterModel.BerthExposesToSwell);

            if (analysisFilterModel.SurgingObserved.IndexOf("ALL", StringComparison.OrdinalIgnoreCase) < 0)
                analysysExpDuration = analysysExpDuration.Where(p => p.SurgingObserved == analysisFilterModel.SurgingObserved);

            if (analysisFilterModel.TrafficPassingEffect.IndexOf("ALL", StringComparison.OrdinalIgnoreCase) < 0)
                analysysExpDuration = analysysExpDuration.Where(p => p.Any_Affect_Passing_Traffic == analysisFilterModel.TrafficPassingEffect);

            if (analysisFilterModel.ShipFenderContact.IndexOf("ALL", StringComparison.OrdinalIgnoreCase) < 0)
                analysysExpDuration = analysysExpDuration.Where(p => p.Ship_was_continuously_contact_with_fender == analysisFilterModel.ShipFenderContact);

            if (analysisFilterModel.RopeDamagedAnytime.IndexOf("ALL", StringComparison.OrdinalIgnoreCase) < 0)
                analysysExpDuration = analysysExpDuration.Where(p => p.Any_Rope_Damaged == analysisFilterModel.RopeDamagedAnytime);

            DateTime builtFrom = DateTime.Today.AddYears(-analysisFilterModel.AgeRangeFrom);
            DateTime builtUpto = DateTime.Today.AddYears(-analysisFilterModel.AgeRangeTo);

            analysysExpDuration = analysysExpDuration.Where(p => p.DateBuilt <= builtFrom);
            analysysExpDuration = analysysExpDuration.Where(p => p.DateBuilt >= builtUpto);

            analysysExpDuration = analysysExpDuration.Where(p => p.WindSpeed >= analysisFilterModel.WindSpeedRangeFrom);
            analysysExpDuration = analysysExpDuration.Where(p => p.WindSpeed <= analysisFilterModel.WindSpeedRangeTo);

            analysysExpDuration = analysysExpDuration.Where(p => p.CurrentSpeed >= analysisFilterModel.CurrentSpeedRangeFrom);
            analysysExpDuration = analysysExpDuration.Where(p => p.CurrentSpeed <= analysisFilterModel.CurrentSpeedRangeTo);

            analysysExpDuration = analysysExpDuration.Where(p => p.AirTemprature >= analysisFilterModel.AirTempRangeFrom);
            analysysExpDuration = analysysExpDuration.Where(p => p.AirTemprature <= analysisFilterModel.AirTempRangeTo);

            //var results = analysysExpDuration.GroupBy(p => new { p.ImoNo, p.Cost, p.RopeType, p.RopeTypeId, p.ManufacturerId, p.Manufacturer, p.VesselName, p.RunningHours, p.Avg_Months }).
            //  Select(u => new AnalysisResult
            //  {
            //      ImoNo = u.Key.ImoNo,
            //      Cost = u.Key.Cost,
            //      RopeType = u.Key.RopeType,
            //      RopeTypeId = u.Key.RopeTypeId,
            //      ManufacturerId = u.Key.ManufacturerId,
            //      Manufacturer = u.Key.Manufacturer,
            //      VesselName = u.Key.VesselName,
            //      RunningHours = u.Key.RunningHours,
            //      Avg_Months = u.Key.Avg_Months
            //  });


            var results = analysysExpDuration.GroupBy(p => new { p.RopeType, p.RopeTypeId, p.ManufacturerId, p.Manufacturer })
                .Select(u => new AnalysisResult
                {
                    RopeType = u.Key.RopeType,
                    RopeTypeId = u.Key.RopeTypeId,
                    ManufacturerId = u.Key.ManufacturerId,
                    Manufacturer = u.Key.Manufacturer,
                    Cost = u.Average(p => p.Cost),
                    RunningHours = (int?)u.Average(p => p.RunningHours),
                    Avg_Months = u.Average(p => p.Avg_Months)
                });

            int currPage = TempData["CurrentPage"] == null ? 1 : Convert.ToInt32(TempData["CurrentPage"]);

            TempData["TotalRecords"] = results.Count();

            var recordsFound = results.OrderBy(u => u.Manufacturer).ThenBy(u => u.RopeType).AsQueryable();

            analysisFilterModel.AnalysisResults = recordsFound.Skip((CommonMethods.PAGESIZE * currPage) - CommonMethods.PAGESIZE).Take(CommonMethods.PAGESIZE).ToList();

            analysisFilterModel.FleetNameList = base.PermittedFleetNames;
            analysisFilterModel.FleetTypeList = base.PermittedFleetTypes;

            analysisFilterModel.VesselList = context.VesselDetails.Where(p => analysisFilterModel.FleetTypeIDs.Contains(p.FleetTypeID) && analysisFilterModel.FleetNameIDs.Contains(p.FleetNameID) && analysisFilterModel.TradeIDs.Contains(p.TradeAreaID)).ToList();

            return View(analysisFilterModel);
        }

        public ActionResult ManufacturerPerformanceComparison(int? id)
        {
            InitializeAnalysisFilterModel();

            analysisFilterModel.DateDiscardedFrom = DateTime.Today.AddYears(-10);
            analysisFilterModel.DateDiscardedUpto = DateTime.Today;

            var analysis = context.View_ManufacturerPerformanceComparison.Where(p => p.OutofServiceDate != null).ToList();

            //var results = context.View_ManufacturerPerformanceComparison.Where(p => p.OutofServiceDate != null).
            //    GroupBy(p => new { p.ImoNo, p.VesselName, p.ManufacturerId, p.Manufacturer }).Select(u => new AnalysisResult
            //    {
            //        ManufacturerId = u.Key.ManufacturerId,
            //        Manufacturer = u.Key.Manufacturer,
            //        VesselName = u.Key.VesselName,
            //        ImoNo = u.Key.ImoNo,
            //        Cost = u.Average(p => p.Cost),
            //        RunningHours = (int?)u.Average(p => p.RunningHours),
            //        Avg_Months = u.Average(p => p.Avg_Months)
            //    });


            var results = context.View_ManufacturerPerformanceComparison.Where(p => p.OutofServiceDate != null).
                GroupBy(p => new { p.ManufacturerId, p.Manufacturer }).Select(u => new AnalysisResult
                {
                    ManufacturerId = u.Key.ManufacturerId,
                    Manufacturer = u.Key.Manufacturer,
                    Cost = u.Average(p => p.Cost),
                    RunningHours = (int?)u.Average(p => p.RunningHours),
                    Avg_Months = u.Average(p => p.Avg_Months)
                });

            //var results = analysis.GroupBy(p => new { p.ManufacturerId }).Select
            //    (u => new AnalysisResult
            //    {
            //        ImoNo = u.First().ImoNo,
            //        Cost = u.First().Cost,
            //        ManufacturerId = u.Key.ManufacturerId,
            //        Manufacturer = u.First().Manfacturer,
            //        VesselName = u.First().VesselName,
            //        RunningHours = u.First().RunningHours,
            //        Avg_Months = u.First().Avg_Months
            //    });

            //analysisFilterModel.AnalysisResults = results.OrderBy(u => u.VesselName).ThenBy(p => p.Manufacturer).ToList();

            var recordsFound = results.OrderBy(u => u.Manufacturer).ToList();

            int currPage = id == null ? 1 : Convert.ToInt32(id);

            TempData["CurrentPage"] = currPage;
            TempData["TotalRecords"] = recordsFound.Count();

            analysisFilterModel.AnalysisResults = recordsFound.Skip((CommonMethods.PAGESIZE * currPage) - CommonMethods.PAGESIZE).Take(CommonMethods.PAGESIZE).ToList();

            return View(analysisFilterModel);
        }

        [HttpPost]
        public ActionResult ManufacturerPerformanceComparison(ReportAnalysisFilterModel analysisFilterModel)
        {
            var manu_perf_comparison = context.View_ManufacturerPerformanceComparison.
                Where(p => p.OutofServiceDate >= analysisFilterModel.DateDiscardedFrom && p.OutofServiceDate <= analysisFilterModel.DateDiscardedUpto).
                AsQueryable();

            if (analysisFilterModel.VesselIDs?.Count > 0)
                manu_perf_comparison = manu_perf_comparison.Where(p => analysisFilterModel.VesselIDs.Contains(p.ImoNo));

            if (analysisFilterModel.FleetNameIDs?.Count > 0)
                manu_perf_comparison = manu_perf_comparison.Where(p => analysisFilterModel.FleetNameIDs.Contains(p.FleetNameID));

            if (analysisFilterModel.FleetTypeIDs?.Count > 0)
                manu_perf_comparison = manu_perf_comparison.Where(p => analysisFilterModel.FleetTypeIDs.Contains(p.FleetTypeID));

            if (analysisFilterModel.TradeIDs?.Count > 0)
                manu_perf_comparison = manu_perf_comparison.Where(p => analysisFilterModel.TradeIDs.Contains(p.TradeAreaID));

            if (analysisFilterModel.ManufacturerIDs?.Count > 0)
                manu_perf_comparison = manu_perf_comparison.Where(p => analysisFilterModel.ManufacturerIDs.Contains(p.ManufacturerId ?? 0));

            if (analysisFilterModel.RopeTypeIDs?.Count > 0)
                manu_perf_comparison = manu_perf_comparison.Where(p => analysisFilterModel.RopeTypeIDs.Contains(p.RopeTypeId ?? 0));

            if (!string.IsNullOrEmpty(analysisFilterModel.PortNames))
                manu_perf_comparison = manu_perf_comparison.Where(p => analysisFilterModel.PortNames.Contains(p.PortName));

            analysisFilterModel.PortFacilityNames = analysisFilterModel.PortFacilityNames == "None Selected" ? null : analysisFilterModel.PortFacilityNames;

            if (!string.IsNullOrEmpty(analysisFilterModel.PortFacilityNames))
                manu_perf_comparison = manu_perf_comparison.Where(p => analysisFilterModel.PortFacilityNames.Contains(p.FacilityName));

            if (!string.IsNullOrEmpty(analysisFilterModel.BirthTypeNames))
                manu_perf_comparison = manu_perf_comparison.Where(p => p.BirthType == analysisFilterModel.BirthTypeNames);

            if (!string.IsNullOrEmpty(analysisFilterModel.MooringTypeNames))
                manu_perf_comparison = manu_perf_comparison.Where(p => p.MooringType == analysisFilterModel.MooringTypeNames);

            if (!string.IsNullOrEmpty(analysisFilterModel.LeadTypeNames))
                manu_perf_comparison = manu_perf_comparison.Where(p => p.Lead == analysisFilterModel.LeadTypeNames);

            if (!string.IsNullOrEmpty(analysisFilterModel.LeadNames))
                manu_perf_comparison = manu_perf_comparison.Where(p => p.Lead1 == analysisFilterModel.LeadNames);

            if (analysisFilterModel.Squal_Gusts.IndexOf("ALL", StringComparison.OrdinalIgnoreCase) < 0)
                manu_perf_comparison = manu_perf_comparison.Where(p => p.AnySquall == analysisFilterModel.Squal_Gusts);

            if (analysisFilterModel.BerthExposesToSwell.IndexOf("ALL", StringComparison.OrdinalIgnoreCase) < 0)
                manu_perf_comparison = manu_perf_comparison.Where(p => p.Berth_exposed_SeaSwell == analysisFilterModel.BerthExposesToSwell);

            if (analysisFilterModel.SurgingObserved.IndexOf("ALL", StringComparison.OrdinalIgnoreCase) < 0)
                manu_perf_comparison = manu_perf_comparison.Where(p => p.SurgingObserved == analysisFilterModel.SurgingObserved);

            if (analysisFilterModel.TrafficPassingEffect.IndexOf("ALL", StringComparison.OrdinalIgnoreCase) < 0)
                manu_perf_comparison = manu_perf_comparison.Where(p => p.Any_Affect_Passing_Traffic == analysisFilterModel.TrafficPassingEffect);

            if (analysisFilterModel.ShipFenderContact.IndexOf("ALL", StringComparison.OrdinalIgnoreCase) < 0)
                manu_perf_comparison = manu_perf_comparison.Where(p => p.Ship_was_continuously_contact_with_fender == analysisFilterModel.ShipFenderContact);

            if (analysisFilterModel.RopeDamagedAnytime.IndexOf("ALL", StringComparison.OrdinalIgnoreCase) < 0)
                manu_perf_comparison = manu_perf_comparison.Where(p => p.Any_Rope_Damaged == analysisFilterModel.RopeDamagedAnytime);

            DateTime builtFrom = DateTime.Today.AddYears(-analysisFilterModel.AgeRangeFrom);
            DateTime builtUpto = DateTime.Today.AddYears(-analysisFilterModel.AgeRangeTo);

            manu_perf_comparison = manu_perf_comparison.Where(p => p.DateBuilt <= builtFrom);
            manu_perf_comparison = manu_perf_comparison.Where(p => p.DateBuilt >= builtUpto);

            manu_perf_comparison = manu_perf_comparison.Where(p => p.WindSpeed >= analysisFilterModel.WindSpeedRangeFrom);
            manu_perf_comparison = manu_perf_comparison.Where(p => p.WindSpeed <= analysisFilterModel.WindSpeedRangeTo);

            manu_perf_comparison = manu_perf_comparison.Where(p => p.CurrentSpeed >= analysisFilterModel.CurrentSpeedRangeFrom);
            manu_perf_comparison = manu_perf_comparison.Where(p => p.CurrentSpeed <= analysisFilterModel.CurrentSpeedRangeTo);

            manu_perf_comparison = manu_perf_comparison.Where(p => p.AirTemprature >= analysisFilterModel.AirTempRangeFrom);
            manu_perf_comparison = manu_perf_comparison.Where(p => p.AirTemprature <= analysisFilterModel.AirTempRangeTo);

            //var results = manu_perf_comparison.GroupBy(p => new { p.ManufacturerId }).Select
            //    (u => new AnalysisResult
            //    {
            //        ManufacturerId = u.Key.ManufacturerId,
            //        Manufacturer = u.FirstOrDefault() == null ? string.Empty : u.FirstOrDefault().Manufacturer,
            //        Cost = u.FirstOrDefault() == null ? 0 : u.FirstOrDefault().Cost,
            //        VesselName = u.FirstOrDefault() == null ? string.Empty : u.FirstOrDefault().VesselName,
            //        ImoNo = u.FirstOrDefault() == null ? 0 : u.FirstOrDefault().ImoNo,
            //        RunningHours = u.FirstOrDefault() == null ? 0 : u.FirstOrDefault().RunningHours,
            //        Avg_Months = u.FirstOrDefault() == null ? 0 : u.FirstOrDefault().Avg_Months
            //    });

            var results = manu_perf_comparison.GroupBy(p => new { p.ManufacturerId, p.Manufacturer }).Select(u => new AnalysisResult
            {
                ManufacturerId = u.Key.ManufacturerId,
                Manufacturer = u.Key.Manufacturer,
                Cost = u.Average(p => p.Cost),
                RunningHours = (int?)u.Average(p => p.RunningHours),
                Avg_Months = u.Average(p => p.Avg_Months)
            });

            int currPage = TempData["CurrentPage"] == null ? 1 : Convert.ToInt32(TempData["CurrentPage"]);

            TempData["TotalRecords"] = results.Count();

            var recordsFound = results.OrderBy(u => u.Manufacturer).AsQueryable();

            analysisFilterModel.AnalysisResults = recordsFound.Skip((CommonMethods.PAGESIZE * currPage) - CommonMethods.PAGESIZE).Take(CommonMethods.PAGESIZE).ToList();

            //analysisFilterModel.AnalysisResults = results.OrderBy(u => u.VesselName).ThenBy(p => p.Manufacturer).ToList();

            //analysisFilterModel.ManufacturerPerformanceComparison = manu_perf_comparison.ToList();

            analysisFilterModel.FleetNameList = base.PermittedFleetNames;
            analysisFilterModel.FleetTypeList = base.PermittedFleetTypes;

            analysisFilterModel.VesselList = context.VesselDetails.Where(p => analysisFilterModel.FleetTypeIDs.Contains(p.FleetTypeID) && analysisFilterModel.FleetNameIDs.Contains(p.FleetNameID) && analysisFilterModel.TradeIDs.Contains(p.TradeAreaID)).ToList();

            return View(analysisFilterModel);
        }

        public ActionResult AverageRunningHours(int? id)
        {
            InitializeAnalysisFilterModel();

            analysisFilterModel.DateDiscardedFrom = DateTime.Today.AddYears(-10);
            analysisFilterModel.DateDiscardedUpto = DateTime.Today;

            var results = context.View_AverageRunningRopeHours.Where(p => p.OutofServiceDate != null)
                .GroupBy(p => new { p.RopeType, p.RopeTypeId })
                .Select(u => new AnalysisResult
                {
                    RopeType = u.Key.RopeType,
                    RopeTypeId = u.Key.RopeTypeId,
                    Cost = u.Average(p => p.Cost),
                    RunningHours = (int?)u.Average(p => p.RunningHours),
                    Avg_Months = u.Average(p => p.Avg_Months)
                });

            //var results = analysis.GroupBy(p => new { p.ImoNo, p.Cost, p.RopeType, p.RopeTypeId, p.VesselName, p.RunningHours, p.Avg_Months }).
            //  Select(u => new AnalysisResult
            //  {
            //      ImoNo = u.Key.ImoNo,
            //      Cost = u.Key.Cost,
            //      RopeType = u.Key.RopeType,
            //      RopeTypeId = u.Key.RopeTypeId,
            //      VesselName = u.Key.VesselName,
            //      RunningHours = u.Key.RunningHours,
            //      Avg_Months = u.Key.Avg_Months
            //  });

            //analysisFilterModel.AnalysisResults = results.OrderBy(u => u.VesselName).ThenBy(u => u.RopeType).ToList();

            var recordsFound = results.OrderBy(u => u.RopeType).ToList();

            int currPage = id == null ? 1 : Convert.ToInt32(id);

            TempData["CurrentPage"] = currPage;
            TempData["TotalRecords"] = recordsFound.Count();

            analysisFilterModel.AnalysisResults = recordsFound.Skip((CommonMethods.PAGESIZE * currPage) - CommonMethods.PAGESIZE).Take(CommonMethods.PAGESIZE).ToList();

            return View(analysisFilterModel);
        }

        [HttpPost]
        public ActionResult AverageRunningHours(ReportAnalysisFilterModel analysisFilterModel)
        {
            var avgRunningRopeHours = context.View_AverageRunningRopeHours.
                Where(p => p.OutofServiceDate >= analysisFilterModel.DateDiscardedFrom && p.OutofServiceDate <= analysisFilterModel.DateDiscardedUpto).
                AsQueryable();

            if (analysisFilterModel.VesselIDs?.Count > 0)
                avgRunningRopeHours = avgRunningRopeHours.Where(p => analysisFilterModel.VesselIDs.Contains(p.ImoNo));

            if (analysisFilterModel.FleetNameIDs?.Count > 0)
                avgRunningRopeHours = avgRunningRopeHours.Where(p => analysisFilterModel.FleetNameIDs.Contains(p.FleetNameID));

            if (analysisFilterModel.FleetTypeIDs?.Count > 0)
                avgRunningRopeHours = avgRunningRopeHours.Where(p => analysisFilterModel.FleetTypeIDs.Contains(p.FleetTypeID));

            if (analysisFilterModel.TradeIDs?.Count > 0)
                avgRunningRopeHours = avgRunningRopeHours.Where(p => analysisFilterModel.TradeIDs.Contains(p.TradeAreaID));

            if (analysisFilterModel.ManufacturerIDs?.Count > 0)
                avgRunningRopeHours = avgRunningRopeHours.Where(p => analysisFilterModel.ManufacturerIDs.Contains(p.ManufacturerId ?? 0));

            if (analysisFilterModel.RopeTypeIDs?.Count > 0)
                avgRunningRopeHours = avgRunningRopeHours.Where(p => analysisFilterModel.RopeTypeIDs.Contains(p.RopeTypeId ?? 0));

            if (!string.IsNullOrEmpty(analysisFilterModel.PortNames))
                avgRunningRopeHours = avgRunningRopeHours.Where(p => analysisFilterModel.PortNames.Contains(p.PortName));

            analysisFilterModel.PortFacilityNames = analysisFilterModel.PortFacilityNames == "None Selected" ? null : analysisFilterModel.PortFacilityNames;

            if (!string.IsNullOrEmpty(analysisFilterModel.PortFacilityNames))
                avgRunningRopeHours = avgRunningRopeHours.Where(p => analysisFilterModel.PortFacilityNames.Contains(p.FacilityName));

            if (!string.IsNullOrEmpty(analysisFilterModel.BirthTypeNames))
                avgRunningRopeHours = avgRunningRopeHours.Where(p => p.BirthType == analysisFilterModel.BirthTypeNames);

            if (!string.IsNullOrEmpty(analysisFilterModel.MooringTypeNames))
                avgRunningRopeHours = avgRunningRopeHours.Where(p => p.MooringType == analysisFilterModel.MooringTypeNames);

            if (!string.IsNullOrEmpty(analysisFilterModel.LeadTypeNames))
                avgRunningRopeHours = avgRunningRopeHours.Where(p => p.Lead == analysisFilterModel.LeadTypeNames);

            if (!string.IsNullOrEmpty(analysisFilterModel.LeadNames))
                avgRunningRopeHours = avgRunningRopeHours.Where(p => p.Lead1 == analysisFilterModel.LeadNames);

            if (analysisFilterModel.Squal_Gusts.IndexOf("ALL", StringComparison.OrdinalIgnoreCase) < 0)
                avgRunningRopeHours = avgRunningRopeHours.Where(p => p.AnySquall == analysisFilterModel.Squal_Gusts);

            if (analysisFilterModel.BerthExposesToSwell.IndexOf("ALL", StringComparison.OrdinalIgnoreCase) < 0)
                avgRunningRopeHours = avgRunningRopeHours.Where(p => p.Berth_exposed_SeaSwell == analysisFilterModel.BerthExposesToSwell);

            if (analysisFilterModel.SurgingObserved.IndexOf("ALL", StringComparison.OrdinalIgnoreCase) < 0)
                avgRunningRopeHours = avgRunningRopeHours.Where(p => p.SurgingObserved == analysisFilterModel.SurgingObserved);

            if (analysisFilterModel.TrafficPassingEffect.IndexOf("ALL", StringComparison.OrdinalIgnoreCase) < 0)
                avgRunningRopeHours = avgRunningRopeHours.Where(p => p.Any_Affect_Passing_Traffic == analysisFilterModel.TrafficPassingEffect);

            if (analysisFilterModel.ShipFenderContact.IndexOf("ALL", StringComparison.OrdinalIgnoreCase) < 0)
                avgRunningRopeHours = avgRunningRopeHours.Where(p => p.Ship_was_continuously_contact_with_fender == analysisFilterModel.ShipFenderContact);

            if (analysisFilterModel.RopeDamagedAnytime.IndexOf("ALL", StringComparison.OrdinalIgnoreCase) < 0)
                avgRunningRopeHours = avgRunningRopeHours.Where(p => p.Any_Rope_Damaged == analysisFilterModel.RopeDamagedAnytime);

            DateTime builtFrom = DateTime.Today.AddYears(-analysisFilterModel.AgeRangeFrom);
            DateTime builtUpto = DateTime.Today.AddYears(-analysisFilterModel.AgeRangeTo);

            avgRunningRopeHours = avgRunningRopeHours.Where(p => p.DateBuilt <= builtFrom);
            avgRunningRopeHours = avgRunningRopeHours.Where(p => p.DateBuilt >= builtUpto);

            avgRunningRopeHours = avgRunningRopeHours.Where(p => p.WindSpeed >= analysisFilterModel.WindSpeedRangeFrom);
            avgRunningRopeHours = avgRunningRopeHours.Where(p => p.WindSpeed <= analysisFilterModel.WindSpeedRangeTo);

            avgRunningRopeHours = avgRunningRopeHours.Where(p => p.CurrentSpeed >= analysisFilterModel.CurrentSpeedRangeFrom);
            avgRunningRopeHours = avgRunningRopeHours.Where(p => p.CurrentSpeed <= analysisFilterModel.CurrentSpeedRangeTo);

            avgRunningRopeHours = avgRunningRopeHours.Where(p => p.AirTemprature >= analysisFilterModel.AirTempRangeFrom);
            avgRunningRopeHours = avgRunningRopeHours.Where(p => p.AirTemprature <= analysisFilterModel.AirTempRangeTo);

            //var results = avgRunningRopeHours.GroupBy(p => new { p.ImoNo, p.Cost, p.RopeType, p.RopeTypeId, p.VesselName, p.RunningHours, p.Avg_Months }).
            //  Select(u => new AnalysisResult
            //  {
            //      ImoNo = u.Key.ImoNo,
            //      Cost = u.Key.Cost,
            //      RopeType = u.Key.RopeType,
            //      RopeTypeId = u.Key.RopeTypeId,
            //      VesselName = u.Key.VesselName,
            //      RunningHours = u.Key.RunningHours,
            //      Avg_Months = u.Key.Avg_Months
            //  });

            var results = avgRunningRopeHours.GroupBy(p => new { p.RopeType, p.RopeTypeId })
                .Select(u => new AnalysisResult
                {
                    RopeType = u.Key.RopeType,
                    RopeTypeId = u.Key.RopeTypeId,
                    Cost = u.Average(p => p.Cost),
                    RunningHours = (int?)u.Average(p => p.RunningHours),
                    Avg_Months = u.Average(p => p.Avg_Months)
                });

            int currPage = TempData["CurrentPage"] == null ? 1 : Convert.ToInt32(TempData["CurrentPage"]);

            TempData["TotalRecords"] = results.Count();

            var recordsFound = results.OrderBy(u => u.RopeType).AsQueryable();

            analysisFilterModel.AnalysisResults = recordsFound.Skip((CommonMethods.PAGESIZE * currPage) - CommonMethods.PAGESIZE).Take(CommonMethods.PAGESIZE).ToList();

            //analysisFilterModel.AnalysisResults = results.OrderBy(u => u.VesselName).ThenBy(u => u.RopeType).ToList();

            analysisFilterModel.FleetNameList = base.PermittedFleetNames;
            analysisFilterModel.FleetTypeList = base.PermittedFleetTypes;

            analysisFilterModel.VesselList = context.VesselDetails.Where(p => analysisFilterModel.FleetTypeIDs.Contains(p.FleetTypeID) && analysisFilterModel.FleetNameIDs.Contains(p.FleetNameID) && analysisFilterModel.TradeIDs.Contains(p.TradeAreaID)).ToList();

            return View(analysisFilterModel);
        }

        public ActionResult PredictionExpectency(int? id)
        {
            InitializeAnalysisFilterModel();

            analysisFilterModel.DateDiscardedFrom = DateTime.Today.AddYears(-10);
            analysisFilterModel.DateDiscardedUpto = DateTime.Today;

            var results = context.View_PredictionExpectency.Where(p => p.OutofServiceDate != null)
            .GroupBy(p => new { p.RopeType, p.RopeTypeId, p.ManufacturerId, p.Manufacturer })
            .Select(u => new AnalysisResult
            {
                RopeType = u.Key.RopeType,
                RopeTypeId = u.Key.RopeTypeId,
                ManufacturerId = u.Key.ManufacturerId,
                Manufacturer = u.Key.Manufacturer,
                RunningHours = (int?)u.Average(p => p.RunningHours),
                Avg_Months = u.Average(p => p.Avg_Months)
            });

            //var results = context.View_PredictionExpectency.Where(p => p.OutofServiceDate != null)
            //    .GroupBy(p => new { p.RopeId, p.RopeType, p.RopeTypeId, p.ManufacturerId, p.Manufacturer, p.VesselName })
            //    .Select(u => new AnalysisResult
            //    {
            //        VesselName = u.Key.VesselName,
            //        RopeType = u.Key.RopeType,
            //        RopeTypeId = u.Key.RopeTypeId,
            //        ManufacturerId = u.Key.ManufacturerId,
            //        Manufacturer = u.Key.Manufacturer,
            //        RunningHours = (int?)u.Average(p => p.RunningHours),
            //        Avg_Months = u.Average(p => p.Avg_Months)
            //    });

            //var results = analysis.GroupBy(p => new { p.ImoNo, p.RopeType, p.RopeTypeId, p.ManufacturerId, p.Manufacturer, p.VesselName, p.RunningHours, p.Avg_Months }).
            //  Select(u => new AnalysisResult
            //  {
            //      ImoNo = u.Key.ImoNo,
            //      RopeType = u.Key.RopeType,
            //      RopeTypeId = u.Key.RopeTypeId,
            //      ManufacturerId = u.Key.ManufacturerId,
            //      Manufacturer = u.Key.Manufacturer,
            //      VesselName = u.Key.VesselName,
            //      RunningHours = u.Key.RunningHours,
            //      Avg_Months = u.Key.Avg_Months
            //  });

            //analysisFilterModel.AnalysisResults = results.OrderBy(u => u.VesselName).ThenBy(u => u.Manufacturer).ThenBy(u => u.RopeType).ToList();

            var recordsFound = results.OrderBy(u => u.Manufacturer).ThenBy(u => u.RopeType).ToList();

            int currPage = id == null ? 1 : Convert.ToInt32(id);

            TempData["CurrentPage"] = currPage;
            TempData["TotalRecords"] = recordsFound.Count();

            analysisFilterModel.AnalysisResults = recordsFound.Skip((CommonMethods.PAGESIZE * currPage) - CommonMethods.PAGESIZE).Take(CommonMethods.PAGESIZE).ToList();

            return View(analysisFilterModel);
        }

        [HttpPost]
        public ActionResult PredictionExpectency(ReportAnalysisFilterModel analysisFilterModel)
        {
            var predExpectency = context.View_PredictionExpectency.
                Where(p => p.OutofServiceDate >= analysisFilterModel.DateDiscardedFrom && p.OutofServiceDate <= analysisFilterModel.DateDiscardedUpto).
                AsQueryable();

            if (analysisFilterModel.VesselIDs?.Count > 0)
                predExpectency = predExpectency.Where(p => analysisFilterModel.VesselIDs.Contains(p.ImoNo));

            if (analysisFilterModel.FleetNameIDs?.Count > 0)
                predExpectency = predExpectency.Where(p => analysisFilterModel.FleetNameIDs.Contains(p.FleetNameID));

            if (analysisFilterModel.FleetTypeIDs?.Count > 0)
                predExpectency = predExpectency.Where(p => analysisFilterModel.FleetTypeIDs.Contains(p.FleetTypeID));

            if (analysisFilterModel.TradeIDs?.Count > 0)
                predExpectency = predExpectency.Where(p => analysisFilterModel.TradeIDs.Contains(p.TradeAreaID));

            if (analysisFilterModel.ManufacturerIDs?.Count > 0)
                predExpectency = predExpectency.Where(p => analysisFilterModel.ManufacturerIDs.Contains(p.ManufacturerId ?? 0));

            if (analysisFilterModel.RopeTypeIDs?.Count > 0)
                predExpectency = predExpectency.Where(p => analysisFilterModel.RopeTypeIDs.Contains(p.RopeTypeId ?? 0));

            if (!string.IsNullOrEmpty(analysisFilterModel.PortNames))
                predExpectency = predExpectency.Where(p => analysisFilterModel.PortNames.Contains(p.PortName));

            analysisFilterModel.PortFacilityNames = analysisFilterModel.PortFacilityNames == "None Selected" ? null : analysisFilterModel.PortFacilityNames;

            if (!string.IsNullOrEmpty(analysisFilterModel.PortFacilityNames))
                predExpectency = predExpectency.Where(p => analysisFilterModel.PortFacilityNames.Contains(p.FacilityName));

            if (!string.IsNullOrEmpty(analysisFilterModel.BirthTypeNames))
                predExpectency = predExpectency.Where(p => p.BirthType == analysisFilterModel.BirthTypeNames);

            if (!string.IsNullOrEmpty(analysisFilterModel.MooringTypeNames))
                predExpectency = predExpectency.Where(p => p.MooringType == analysisFilterModel.MooringTypeNames);

            if (!string.IsNullOrEmpty(analysisFilterModel.LeadTypeNames))
                predExpectency = predExpectency.Where(p => p.Lead == analysisFilterModel.LeadTypeNames);

            if (!string.IsNullOrEmpty(analysisFilterModel.LeadNames))
                predExpectency = predExpectency.Where(p => p.Lead1 == analysisFilterModel.LeadNames);

            if (!analysisFilterModel.Squal_Gusts.ToUpper().Contains("ALL"))
                predExpectency = predExpectency.Where(p => p.AnySquall == analysisFilterModel.Squal_Gusts);

            if (!analysisFilterModel.BerthExposesToSwell.ToUpper().Contains("ALL"))
                predExpectency = predExpectency.Where(p => p.Berth_exposed_SeaSwell == analysisFilterModel.BerthExposesToSwell);

            if (!analysisFilterModel.SurgingObserved.ToUpper().Contains("ALL"))
                predExpectency = predExpectency.Where(p => p.SurgingObserved == analysisFilterModel.SurgingObserved);

            if (!analysisFilterModel.TrafficPassingEffect.ToUpper().Contains("ALL"))
                predExpectency = predExpectency.Where(p => p.Any_Affect_Passing_Traffic == analysisFilterModel.TrafficPassingEffect);

            if (!analysisFilterModel.ShipFenderContact.ToUpper().Contains("ALL"))
                predExpectency = predExpectency.Where(p => p.Ship_was_continuously_contact_with_fender == analysisFilterModel.ShipFenderContact);

            if (!analysisFilterModel.RopeDamagedAnytime.ToUpper().Contains("ALL"))
                predExpectency = predExpectency.Where(p => p.Any_Rope_Damaged == analysisFilterModel.RopeDamagedAnytime);

            DateTime builtFrom = DateTime.Today.AddYears(-analysisFilterModel.AgeRangeFrom);
            DateTime builtUpto = DateTime.Today.AddYears(-analysisFilterModel.AgeRangeTo);

            predExpectency = predExpectency.Where(p => p.DateBuilt <= builtFrom);
            predExpectency = predExpectency.Where(p => p.DateBuilt >= builtUpto);

            predExpectency = predExpectency.Where(p => p.WindSpeed >= analysisFilterModel.WindSpeedRangeFrom);
            predExpectency = predExpectency.Where(p => p.WindSpeed <= analysisFilterModel.WindSpeedRangeTo);

            predExpectency = predExpectency.Where(p => p.CurrentSpeed >= analysisFilterModel.CurrentSpeedRangeFrom);
            predExpectency = predExpectency.Where(p => p.CurrentSpeed <= analysisFilterModel.CurrentSpeedRangeTo);

            predExpectency = predExpectency.Where(p => p.AirTemprature >= analysisFilterModel.AirTempRangeFrom);
            predExpectency = predExpectency.Where(p => p.AirTemprature <= analysisFilterModel.AirTempRangeTo);

            //var results = predExpectency.GroupBy(p => new { p.ImoNo, p.RopeType, p.RopeTypeId, p.ManufacturerId, p.Manufacturer, p.VesselName, p.RunningHours, p.Avg_Months }).
            //  Select(u => new AnalysisResult
            //  {
            //      ImoNo = u.Key.ImoNo,
            //      RopeType = u.Key.RopeType,
            //      RopeTypeId = u.Key.RopeTypeId,
            //      ManufacturerId = u.Key.ManufacturerId,
            //      Manufacturer = u.Key.Manufacturer,
            //      VesselName = u.Key.VesselName,
            //      RunningHours = u.Key.RunningHours,
            //      Avg_Months = u.Key.Avg_Months
            //  });

            var results = predExpectency.GroupBy(p => new { p.RopeType, p.RopeTypeId, p.ManufacturerId, p.Manufacturer })
               .Select(u => new AnalysisResult
               {
                   RopeType = u.Key.RopeType,
                   RopeTypeId = u.Key.RopeTypeId,
                   ManufacturerId = u.Key.ManufacturerId,
                   Manufacturer = u.Key.Manufacturer,
                   RunningHours = (int?)u.Average(p => p.RunningHours),
                   Avg_Months = u.Average(p => p.Avg_Months)
               });

            int currPage = TempData["CurrentPage"] == null ? 1 : Convert.ToInt32(TempData["CurrentPage"]);

            TempData["TotalRecords"] = results.Count();

            var recordsFound = results.OrderBy(u => u.Manufacturer).ThenBy(u => u.RopeType).AsQueryable();

            analysisFilterModel.AnalysisResults = recordsFound.Skip((CommonMethods.PAGESIZE * currPage) - CommonMethods.PAGESIZE).Take(CommonMethods.PAGESIZE).ToList();

            //analysisFilterModel.AnalysisResults = results.OrderBy(u => u.VesselName).ThenBy(u => u.Manufacturer).ThenBy(u => u.RopeType).ToList();

            analysisFilterModel.FleetNameList = base.PermittedFleetNames;
            analysisFilterModel.FleetTypeList = base.PermittedFleetTypes;

            analysisFilterModel.VesselList = context.VesselDetails.Where(p => analysisFilterModel.FleetTypeIDs.Contains(p.FleetTypeID) && analysisFilterModel.FleetNameIDs.Contains(p.FleetNameID) && analysisFilterModel.TradeIDs.Contains(p.TradeAreaID)).ToList();

            return View(analysisFilterModel);
        }

        public ActionResult VesselWise(int? id)
        {
            InitializeAnalysisFilterModel();

            analysisFilterModel.DateDiscardedFrom = DateTime.Today.AddYears(-10);
            analysisFilterModel.DateDiscardedUpto = DateTime.Today;

            var tttt = TempData["AactiveNonActicve"];
            if (tttt == null)
            {
                analysisFilterModel.AactiveNonActicve = true;
                TempData["AactiveNonActicve"] = true;
            }
            else
            {
                analysisFilterModel.AactiveNonActicve = false;
                TempData["AactiveNonActicve"] = false;
            }


            var analysis = new List<View_VesselWiseExpenseAnalysis>();
            // List<MooringRopeDetail> MooringRopes;

            //context.View_VesselWiseExpenseAnalysis.Where(p => p.OutofServiceDate != null && (p.Avg_Months != null || p.Avg_Months > 0) && (p.RunningHours != null || p.RunningHours >= 0)).ToList();
            // TempData["AactiveNonActicve"] = true;


            if (analysisFilterModel.AactiveNonActicve == true)
            {
                analysis = context.View_VesselWiseExpenseAnalysis.
             Where(p => (p.OutofServiceDate != null) && (p.Avg_Months != null || p.Avg_Months > 0) && (p.RunningHours != null || p.RunningHours >= 0) && (p.OutofServiceDate >= analysisFilterModel.DateDiscardedFrom && p.OutofServiceDate <= analysisFilterModel.DateDiscardedUpto)).
             ToList();
                // MooringRopes = context.MooringRopeDetails.Where(p => p.OutofServiceDate >= analysisFilterModel.DateDiscardedFrom && p.OutofServiceDate <= analysisFilterModel.DateDiscardedUpto).ToList();

            }
            else
            {
                analysis = context.View_VesselWiseExpenseAnalysis.
               Where(p => p.OutofServiceDate == null && (p.Avg_Months != null || p.Avg_Months > 0) && (p.RunningHours != null || p.RunningHours >= 0) && (p.InstalledDate >= analysisFilterModel.DateDiscardedFrom && p.InstalledDate <= analysisFilterModel.DateDiscardedUpto)).
               ToList();

                //  MooringRopes = context.MooringRopeDetails.Where(p => p.InstalledDate >= analysisFilterModel.DateDiscardedFrom && p.InstalledDate <= analysisFilterModel.DateDiscardedUpto).ToList();
            }


            //var results = analysis.GroupBy(p => new { p.ImoNo, p.RopeType, p.RopeTypeId, p.ManufacturerId, p.Manufacturer, p.VesselName }).
            //    Select(u => new AnalysisResult
            //    {
            //        ImoNo = u.Key.ImoNo,
            //        RopeType = u.Key.RopeType,
            //        RopeTypeId = u.Key.RopeTypeId,
            //        ManufacturerId = u.Key.ManufacturerId,
            //        Manufacturer = u.Key.Manufacturer,
            //        VesselName = u.Key.VesselName,
            //        Cost = u.Average(p => p.Cost),
            //        RunningHours = (int?)u.Average(p => p.RunningHours),
            //        Avg_Months = u.Average(p => p.Avg_Months)
            //    });

            var ResultSet = analysis.GroupBy(p => new
            {
                p.VesselID,
                p.ImoNo,
                p.RopeId,
                p.RopeType,
                p.RopeTypeId,
                p.ManufacturerId,
                p.Manufacturer,
                p.VesselName,
                p.Cost,
                p.RunningHours,
                p.Avg_Months,
                p.InstalledDate,
                p.OutofServiceDate,
                p.FleetNameID,
                p.FleetTypeID,
                p.TradeAreaID,
                //p.ManufacturerId,
                //p.RopeTypeId,

                //p.PortName,
                //p.FacilityName,
                //p.BirthType,
                //p.MooringType,
                //p.Lead,
                //p.Lead1,
                //p.AnySquall,
                //p.Berth_exposed_SeaSwell,
                //p.SurgingObserved,
                //p.Any_Affect_Passing_Traffic,
                //p.Ship_was_continuously_contact_with_fender,
                //p.Any_Rope_Damaged,
                //p.DateBuilt,
                //p.WindSpeed,
                //p.CurrentSpeed,
                //p.AirTemprature
            }).
             Select(u => u.FirstOrDefault());

            var results = ResultSet.GroupBy(p => new { p.ImoNo, p.RopeType, p.RopeTypeId, p.ManufacturerId, p.Manufacturer, p.VesselName }).
                Select(u => new AnalysisResult
                {
                    ImoNo = u.Key.ImoNo,
                    //RopeId = u.Count(),
                    RopeType = u.Key.RopeType,
                    RopeTypeId = u.Key.RopeTypeId,
                    ManufacturerId = u.Key.ManufacturerId,
                    Manufacturer = u.Key.Manufacturer,
                    VesselName = u.Key.VesselName,
                    TotalCost = (int?)u.Sum(p => p.Cost),
                    Cost = u.Average(p => p.Cost),
                    RunningHours = (int?)u.Average(p => p.RunningHours),
                    Avg_Months = u.Average(p => p.Avg_Months),
                    LineCount = u.Count()
                });

            //var resultMain = from x in results
            //                 join y in MooringRopes
            //                 on new { X1 = x.ImoNo, X2 = x.RopeId } equals new { X1 = y.VesselID, X2 = y.RopeId }
            //                 select new AnalysisResult
            //                 {
            //                     ImoNo = x.ImoNo,
            //                     RopeTail = y.RopeTail,
            //                     RopeType = x.RopeType,
            //                     RopeTypeId = x.RopeTypeId,
            //                     ManufacturerId = x.ManufacturerId,
            //                     Manufacturer = x.Manufacturer,
            //                     VesselName = x.VesselName,
            //                     TotalCost = x.TotalCost,
            //                     Cost = x.Cost,
            //                     RunningHours = x.RunningHours,
            //                     Avg_Months = x.Avg_Months
            //                 };

            var recordsFound = results.OrderBy(u => u.VesselName).ThenBy(u => u.Manufacturer).ThenBy(u => u.RopeType).AsQueryable();

            int currPage = id == null ? 1 : Convert.ToInt32(id);

            TempData["CurrentPage"] = currPage;
            TempData["TotalRecords"] = recordsFound.Count();

            analysisFilterModel.AnalysisResults = recordsFound.Skip((CommonMethods.PAGESIZE * currPage) - CommonMethods.PAGESIZE).Take(CommonMethods.PAGESIZE).ToList();
            //List<string> none = new List<string>() { "None Selected" };
            // new SelectList(facilityList, "Port_Facility_Name");
            //analysisFilterModel.PortFacilities = new SelectListItem { Text = "Breast", Value = "Breast" };
            return View(analysisFilterModel);
        }

        [HttpPost]
        public ActionResult VesselWise(int? id, ReportAnalysisFilterModel analysisFilterModel)
        {
            IQueryable<View_VesselWiseExpenseAnalysis> vesselWiseExpAnalysis;
            //  List<MooringRopeDetail> MooringRopes;

            if (analysisFilterModel.AactiveNonActicve == true)
            {
                vesselWiseExpAnalysis = context.View_VesselWiseExpenseAnalysis.
             Where(p => (p.OutofServiceDate != null) && (p.Avg_Months != null || p.Avg_Months > 0) && (p.RunningHours != null || p.RunningHours >= 0) && (p.OutofServiceDate >= analysisFilterModel.DateDiscardedFrom && p.OutofServiceDate <= analysisFilterModel.DateDiscardedUpto)).
             AsQueryable();
                TempData["AactiveNonActicve"] = true;
                analysisFilterModel.AactiveNonActicve = true;

                // MooringRopes =  context.MooringRopeDetails.Where(p=> p.OutofServiceDate >= analysisFilterModel.DateDiscardedFrom && p.OutofServiceDate <= analysisFilterModel.DateDiscardedUpto).ToList();
            }
            else
            {
                vesselWiseExpAnalysis = context.View_VesselWiseExpenseAnalysis.
               Where(p => p.OutofServiceDate == null && (p.Avg_Months != null || p.Avg_Months > 0) && (p.RunningHours != null || p.RunningHours >= 0) && (p.InstalledDate >= analysisFilterModel.DateDiscardedFrom && p.InstalledDate <= analysisFilterModel.DateDiscardedUpto)).
               AsQueryable();
                TempData["AactiveNonActicve"] = false;
                analysisFilterModel.AactiveNonActicve = false;

                // MooringRopes = context.MooringRopeDetails.Where(p => p.InstalledDate >= analysisFilterModel.DateDiscardedFrom && p.InstalledDate <= analysisFilterModel.DateDiscardedUpto).ToList();
            }



            // TempData["AactiveNonActicve"] = analysisFilterModel.AactiveNonActicve;

            if (analysisFilterModel.VesselIDs?.Count > 0)
                vesselWiseExpAnalysis = vesselWiseExpAnalysis.Where(p => analysisFilterModel.VesselIDs.Contains(p.ImoNo));

            if (analysisFilterModel.FleetNameIDs?.Count > 0)
                vesselWiseExpAnalysis = vesselWiseExpAnalysis.Where(p => analysisFilterModel.FleetNameIDs.Contains(p.FleetNameID));

            if (analysisFilterModel.FleetTypeIDs?.Count > 0)
                vesselWiseExpAnalysis = vesselWiseExpAnalysis.Where(p => analysisFilterModel.FleetTypeIDs.Contains(p.FleetTypeID));

            if (analysisFilterModel.TradeIDs?.Count > 0)
                vesselWiseExpAnalysis = vesselWiseExpAnalysis.Where(p => analysisFilterModel.TradeIDs.Contains(p.TradeAreaID));

            if (analysisFilterModel.ManufacturerIDs?.Count > 0)
                vesselWiseExpAnalysis = vesselWiseExpAnalysis.Where(p => analysisFilterModel.ManufacturerIDs.Contains(p.ManufacturerId ?? 0));

            if (analysisFilterModel.RopeTypeIDs?.Count > 0)
                vesselWiseExpAnalysis = vesselWiseExpAnalysis.Where(p => analysisFilterModel.RopeTypeIDs.Contains(p.RopeTypeId ?? 0));

            if (!string.IsNullOrEmpty(analysisFilterModel.PortNames))
                vesselWiseExpAnalysis = vesselWiseExpAnalysis.Where(p => analysisFilterModel.PortNames.Contains(p.PortName));

            analysisFilterModel.PortFacilityNames = analysisFilterModel.PortFacilityNames == "None Selected" ? null : analysisFilterModel.PortFacilityNames;

            if (!string.IsNullOrEmpty(analysisFilterModel.PortFacilityNames))
                vesselWiseExpAnalysis = vesselWiseExpAnalysis.Where(p => analysisFilterModel.PortFacilityNames.Contains(p.FacilityName));

            if (!string.IsNullOrEmpty(analysisFilterModel.BirthTypeNames))
                vesselWiseExpAnalysis = vesselWiseExpAnalysis.Where(p => p.BirthType == analysisFilterModel.BirthTypeNames);

            if (!string.IsNullOrEmpty(analysisFilterModel.MooringTypeNames))
                vesselWiseExpAnalysis = vesselWiseExpAnalysis.Where(p => p.MooringType == analysisFilterModel.MooringTypeNames);

            if (!string.IsNullOrEmpty(analysisFilterModel.LeadTypeNames))
                vesselWiseExpAnalysis = vesselWiseExpAnalysis.Where(p => p.Lead == analysisFilterModel.LeadTypeNames);

            if (!string.IsNullOrEmpty(analysisFilterModel.LeadNames))
                vesselWiseExpAnalysis = vesselWiseExpAnalysis.Where(p => p.Lead1 == analysisFilterModel.LeadNames);

            if (!analysisFilterModel.Squal_Gusts.ToUpper().Contains("ALL"))
                vesselWiseExpAnalysis = vesselWiseExpAnalysis.Where(p => p.AnySquall == analysisFilterModel.Squal_Gusts);

            if (!analysisFilterModel.BerthExposesToSwell.ToUpper().Contains("ALL"))
                vesselWiseExpAnalysis = vesselWiseExpAnalysis.Where(p => p.Berth_exposed_SeaSwell == analysisFilterModel.BerthExposesToSwell);

            if (!analysisFilterModel.SurgingObserved.ToUpper().Contains("ALL"))
                vesselWiseExpAnalysis = vesselWiseExpAnalysis.Where(p => p.SurgingObserved == analysisFilterModel.SurgingObserved);

            if (!analysisFilterModel.TrafficPassingEffect.ToUpper().Contains("ALL"))
                vesselWiseExpAnalysis = vesselWiseExpAnalysis.Where(p => p.Any_Affect_Passing_Traffic == analysisFilterModel.TrafficPassingEffect);

            if (!analysisFilterModel.ShipFenderContact.ToUpper().Contains("ALL"))
                vesselWiseExpAnalysis = vesselWiseExpAnalysis.Where(p => p.Ship_was_continuously_contact_with_fender == analysisFilterModel.ShipFenderContact);

            if (!analysisFilterModel.RopeDamagedAnytime.ToUpper().Contains("ALL"))
                vesselWiseExpAnalysis = vesselWiseExpAnalysis.Where(p => p.Any_Rope_Damaged == analysisFilterModel.RopeDamagedAnytime);

            DateTime builtFrom = DateTime.Today.AddYears(-analysisFilterModel.AgeRangeFrom);
            DateTime builtUpto = DateTime.Today.AddYears(-analysisFilterModel.AgeRangeTo);

            vesselWiseExpAnalysis = vesselWiseExpAnalysis.Where(p => p.DateBuilt <= builtFrom);
            vesselWiseExpAnalysis = vesselWiseExpAnalysis.Where(p => p.DateBuilt >= builtUpto);

            vesselWiseExpAnalysis = vesselWiseExpAnalysis.Where(p => p.WindSpeed >= analysisFilterModel.WindSpeedRangeFrom);
            vesselWiseExpAnalysis = vesselWiseExpAnalysis.Where(p => p.WindSpeed <= analysisFilterModel.WindSpeedRangeTo);

            vesselWiseExpAnalysis = vesselWiseExpAnalysis.Where(p => p.CurrentSpeed >= analysisFilterModel.CurrentSpeedRangeFrom);
            vesselWiseExpAnalysis = vesselWiseExpAnalysis.Where(p => p.CurrentSpeed <= analysisFilterModel.CurrentSpeedRangeTo);

            vesselWiseExpAnalysis = vesselWiseExpAnalysis.Where(p => p.AirTemprature >= analysisFilterModel.AirTempRangeFrom);
            vesselWiseExpAnalysis = vesselWiseExpAnalysis.Where(p => p.AirTemprature <= analysisFilterModel.AirTempRangeTo);

            //var results = vesselWiseExpAnalysis.GroupBy(p => new { p.ImoNo, p.Cost, p.RopeType, p.RopeTypeId, p.ManufacturerId, p.Manufacturer, p.VesselName, p.RunningHours, p.Avg_Months }).
            //    Select(u => new AnalysisResult
            //    {
            //        ImoNo = u.Key.ImoNo,
            //        Cost = u.Key.Cost,
            //        RopeType = u.Key.RopeType,
            //        RopeTypeId = u.Key.RopeTypeId,
            //        ManufacturerId = u.Key.ManufacturerId,
            //        Manufacturer = u.Key.Manufacturer,
            //        VesselName = u.Key.VesselName,
            //        RunningHours = u.Key.RunningHours,
            //        Avg_Months = u.Key.Avg_Months
            //    }).AsQueryable();

            //var results = vesselWiseExpAnalysis.GroupBy(p => new { p.RopeId, p.ImoNo, p.RopeType, p.RopeTypeId, p.ManufacturerId, p.Manufacturer, p.VesselName }).

            //var ResultSet = vesselWiseExpAnalysis.Select(x => new View_VesselWiseExpenseAnalysis 
            //{ ImoNo = x.ImoNo, RopeType = x.RopeType, RopeTypeId = x.RopeTypeId,  ManufacturerId = x.ManufacturerId, Manufacturer = x.Manufacturer, VesselName = x.VesselName,
            //Cost=x.Cost,RunningHours=x.RunningHours,Avg_Months=x.Avg_Months
            //}).Distinct();
            //ResultSet = ResultSet.Distinct().ToList();

            var ResultSet = vesselWiseExpAnalysis.GroupBy(p => new
            {
                p.VesselID,
                p.ImoNo,
                p.RopeId,
                p.RopeType,
                p.RopeTypeId,
                p.ManufacturerId,
                p.Manufacturer,
                p.VesselName,
                p.Cost,
                p.RunningHours,
                p.Avg_Months,
                p.InstalledDate,
                p.OutofServiceDate,
                p.FleetNameID,
                p.FleetTypeID,
                p.TradeAreaID,
                //p.ManufacturerId,
                //p.RopeTypeId,

                //p.PortName,
                //p.FacilityName,
                //p.BirthType,
                //p.MooringType,
                //p.Lead,
                //p.Lead1,
                //p.AnySquall,
                //p.Berth_exposed_SeaSwell,
                //p.SurgingObserved,
                //p.Any_Affect_Passing_Traffic,
                //p.Ship_was_continuously_contact_with_fender,
                //p.Any_Rope_Damaged,
                //p.DateBuilt,
                //p.WindSpeed,
                //p.CurrentSpeed,
                //p.AirTemprature
            }).
               Select(u => u.FirstOrDefault());

            var results = ResultSet.GroupBy(p => new { p.ImoNo, p.RopeType, p.RopeTypeId, p.ManufacturerId, p.Manufacturer, p.VesselName }).
                Select(u => new AnalysisResult
                {
                    ImoNo = u.Key.ImoNo,
                    //RopeId = u.Key.RopeId,
                    RopeType = u.Key.RopeType,
                    RopeTypeId = u.Key.RopeTypeId,
                    ManufacturerId = u.Key.ManufacturerId,
                    Manufacturer = u.Key.Manufacturer,
                    VesselName = u.Key.VesselName,
                    TotalCost = (int?)u.Sum(p => p.Cost),
                    Cost = u.Average(p => p.Cost),
                    RunningHours = (int?)u.Average(p => p.RunningHours),
                    Avg_Months = u.Average(p => p.Avg_Months),
                    LineCount = u.Count()
                });



            //var resultMain = from x in results
            //             join y in MooringRopes
            //             on new { X1 = x.ImoNo, X2 = x.RopeId } equals new { X1 = y.VesselID, X2 = y.RopeId }
            //             select new AnalysisResult
            //             {
            //                 ImoNo = x.ImoNo,
            //                 RopeTail = y.RopeTail,
            //                 RopeType = x.RopeType,
            //                 RopeTypeId =x.RopeTypeId,
            //                 ManufacturerId = x.ManufacturerId,
            //                 Manufacturer = x.Manufacturer,
            //                 VesselName =x.VesselName,
            //                 TotalCost = x.TotalCost,
            //                 Cost = x.Cost,
            //                 RunningHours = x.RunningHours,
            //                 Avg_Months = x.Avg_Months
            //             };



            var recordsFound = results.OrderBy(u => u.VesselName).ThenBy(u => u.Manufacturer).ThenBy(u => u.RopeType).ToList();

            int currPage = TempData["CurrentPage"] == null ? 1 : Convert.ToInt32(TempData["CurrentPage"]);
            TempData["CurrentPage"] = currPage;
            TempData["TotalRecords"] = recordsFound.Count();

            analysisFilterModel.AnalysisResults = recordsFound.Skip((CommonMethods.PAGESIZE * currPage) - CommonMethods.PAGESIZE).Take(CommonMethods.PAGESIZE).ToList();

            analysisFilterModel.FleetNameList = base.PermittedFleetNames;
            analysisFilterModel.FleetTypeList = base.PermittedFleetTypes;

            analysisFilterModel.VesselList = context.VesselDetails.Where(p => analysisFilterModel.FleetTypeIDs.Contains(p.FleetTypeID) && analysisFilterModel.FleetNameIDs.Contains(p.FleetNameID) && analysisFilterModel.TradeIDs.Contains(p.TradeAreaID)).ToList();

            return View(analysisFilterModel);
        }

        public ActionResult OperationRecords(int? vid, int? cp)
        {

            //TempData["VsId"]=vid;

            if (vid != null)
                PagePost = 1;

            InitializeAnalysisFilterModel();

            analysisFilterModel.VesselList = context.VesselDetails.OrderBy(u => u.VesselName).ToList();
            analysisFilterModel.WindSpeedRangeFrom = 1;
            analysisFilterModel.WindSpeedRangeTo = 200;

            analysisFilterModel.CurrentSpeedRangeFrom = 0;
            analysisFilterModel.CurrentSpeedRangeTo = 20;

            analysisFilterModel.AirTempRangeFrom = -50;
            analysisFilterModel.AirTempRangeTo = 60;

            var opRecords = context.View_OperationRecords.AsQueryable();




            if (vid != null && PagePost == 1)
            {
                int vesselID = Convert.ToInt32(vid);
                List<int> vvv = new List<int>() { vesselID };
                analysisFilterModel.VesselIDs = vvv;

                // analysisFilterModel.VesselIDs.Add(vesselID);
                opRecords = opRecords.Where(p => p.ImoNo == vesselID);


                var operationRecords = opRecords.GroupBy(p => new { p.OPId, p.CastDatetime, p.FastDatetime, p.BirthName, p.PortName, p.FacilityName, p.RopeDamaged, p.ImoNo, p.VesselName }).
                    Select(u => new OperationResult
                    {
                        OperationID = u.Key.OPId,
                        BirthName = u.Key.BirthName,
                        CastDateTime = u.Key.CastDatetime,
                        FacilityName = u.Key.FacilityName,
                        FastDateTime = u.Key.FastDatetime,
                        PortName = u.Key.PortName,
                        RopeDamaged = (int)u.Key.RopeDamaged,
                        IMO = u.Key.ImoNo,
                        VesselName = u.Key.VesselName
                    }).OrderBy(u => u.CastDateTime);
                PagePost = 1;
                SearchOperation = operationRecords.ToList();
            }
            int currPage = cp == null ? 1 : Convert.ToInt32(cp);

            TempData["CurrentPage"] = currPage;
            TempData["TotalRecords"] = SearchOperation.Count();

            analysisFilterModel.OperationResultList = SearchOperation.OrderByDescending(u => u.CastDateTime).AsQueryable().
                Skip((CommonMethods.PAGESIZE * currPage) - CommonMethods.PAGESIZE).Take(CommonMethods.PAGESIZE).ToList();

            return View(analysisFilterModel);
        }

        public static int PagePost { get; set; } = 1;
        public static List<OperationResult> SearchOperation { get; set; }

        [HttpPost]
        public ActionResult OperationRecords(ReportAnalysisFilterModel filterModel)
        {
            PagePost = 2;
            var opRecords = context.View_OperationRecords.OrderBy(u => u.CastDatetime).AsQueryable();
            // TempData["VsId"] = filterModel.VesselIDs;
            if (filterModel.VesselIDs?.Count > 0)
                opRecords = opRecords.Where(p => filterModel.VesselIDs.Contains(p.ImoNo));

            if (filterModel.ManufacturerIDs?.Count > 0)
                opRecords = opRecords.Where(p => filterModel.ManufacturerIDs.Contains(p.ManufacturerId ?? 0));

            if (filterModel.RopeTypeIDs?.Count > 0)
                opRecords = opRecords.Where(p => filterModel.RopeTypeIDs.Contains(p.RopeTypeId ?? 0));

            if (!string.IsNullOrEmpty(filterModel.PortNames))
                opRecords = opRecords.Where(p => filterModel.PortNames.Contains(p.PortName));

            // None Selected

            filterModel.PortFacilityNames = filterModel.PortFacilityNames == "None Selected" ? null : filterModel.PortFacilityNames;

            if (!string.IsNullOrEmpty(filterModel.PortFacilityNames))
                opRecords = opRecords.Where(p => filterModel.PortFacilityNames.Contains(p.FacilityName));

            if (!string.IsNullOrEmpty(filterModel.BirthTypeNames))
                opRecords = opRecords.Where(p => p.BirthType == filterModel.BirthTypeNames);

            if (!string.IsNullOrEmpty(filterModel.MooringTypeNames))
                opRecords = opRecords.Where(p => p.MooringType == filterModel.MooringTypeNames);

            if (!string.IsNullOrEmpty(filterModel.LeadTypeNames))
                opRecords = opRecords.Where(p => p.Lead == filterModel.LeadTypeNames);

            if (!string.IsNullOrEmpty(filterModel.LeadNames))
                opRecords = opRecords.Where(p => p.Lead1 == filterModel.LeadNames);

            if (!filterModel.Squal_Gusts.ToUpper().Contains("ALL"))
                opRecords = opRecords.Where(p => p.AnySquall == filterModel.Squal_Gusts);

            if (!filterModel.BerthExposesToSwell.ToUpper().Contains("ALL"))
                opRecords = opRecords.Where(p => p.Berth_exposed_SeaSwell == filterModel.BerthExposesToSwell);

            if (!filterModel.SurgingObserved.ToUpper().Contains("ALL"))
                opRecords = opRecords.Where(p => p.SurgingObserved == filterModel.SurgingObserved);

            if (!filterModel.TrafficPassingEffect.ToUpper().Contains("ALL"))
                opRecords = opRecords.Where(p => p.Any_Affect_Passing_Traffic == filterModel.TrafficPassingEffect);

            if (!filterModel.ShipFenderContact.ToUpper().Contains("ALL"))
                opRecords = opRecords.Where(p => p.Ship_was_continuously_contact_with_fender == filterModel.ShipFenderContact);

            if (!filterModel.RopeDamagedAnytime.ToUpper().Contains("ALL"))
                opRecords = opRecords.Where(p => p.Any_Rope_Damaged == filterModel.RopeDamagedAnytime);

            opRecords = opRecords.Where(p => p.WindSpeed >= filterModel.WindSpeedRangeFrom);
            opRecords = opRecords.Where(p => p.WindSpeed <= filterModel.WindSpeedRangeTo);

            opRecords = opRecords.Where(p => p.CurrentSpeed >= filterModel.CurrentSpeedRangeFrom);
            opRecords = opRecords.Where(p => p.CurrentSpeed <= filterModel.CurrentSpeedRangeTo);

            opRecords = opRecords.Where(p => p.AirTemprature >= filterModel.AirTempRangeFrom);
            opRecords = opRecords.Where(p => p.AirTemprature <= filterModel.AirTempRangeTo);

            var operationRecords = opRecords.GroupBy(p => new { p.OPId, p.CastDatetime, p.FastDatetime, p.BirthName, p.PortName, p.FacilityName, p.ImoNo, p.VesselName }).
                Select(u => new OperationResult
                {
                    OperationID = u.Key.OPId,
                    BirthName = u.Key.BirthName,
                    CastDateTime = u.Key.CastDatetime,
                    FacilityName = u.Key.FacilityName,
                    FastDateTime = u.Key.FastDatetime,
                    PortName = u.Key.PortName,
                    IMO = u.Key.ImoNo,
                    VesselName = u.Key.VesselName
                });

            int currPage = 1;

            TempData["CurrentPage"] = currPage;
            TempData["TotalRecords"] = operationRecords.Count();
            SearchOperation = operationRecords.ToList();
            filterModel.OperationResultList = operationRecords.OrderByDescending(u => u.FastDateTime).AsQueryable().
                Skip((CommonMethods.PAGESIZE * currPage) - CommonMethods.PAGESIZE).Take(CommonMethods.PAGESIZE).ToList();

            filterModel.VesselList = context.VesselDetails.OrderBy(u => u.VesselName).ToList();

            filterModel.FleetNameList = base.PermittedFleetNames;
            filterModel.FleetTypeList = base.PermittedFleetTypes;

            // filterModel.VesselList = context.VesselDetails.Where(p => filterModel.FleetTypeIDs.Contains(p.FleetTypeID) && filterModel.FleetNameIDs.Contains(p.FleetNameID) && filterModel.TradeIDs.Contains(p.TradeAreaID)).ToList();

            return View(filterModel);
        }

        public ActionResult RopeLabResults(int? id)
        {
            ResidualLabFilter labFilter = new ResidualLabFilter();
            labFilter.FleetNameList = base.PermittedFleetNames;
            labFilter.FleetTypeList = base.PermittedFleetTypes;

            labFilter.DateInstalledFrom = DateTime.Today.AddYears(-1);
            labFilter.DateInstalledTo = DateTime.Today;
            labFilter.AgeRangeFrom = 0;
            labFilter.AgeRangeTo = 50;

            var recordsFound = context.View_ResidualStrength_RopeLabResults.Where(p => p.LabTestDate >= labFilter.DateInstalledFrom && p.LabTestDate <= labFilter.DateInstalledTo).
                OrderBy(u => u.VesselName).ThenBy(u => u.Manufacturer).ThenBy(u => u.RopeType).AsQueryable();

            int currPage = id == null ? 1 : Convert.ToInt32(id);

            TempData["CurrentPage"] = currPage;
            TempData["TotalRecords"] = recordsFound.Count();

            labFilter.ResidualStrength_RopeLabResults = recordsFound.Skip((CommonMethods.PAGESIZE * currPage) - CommonMethods.PAGESIZE).Take(CommonMethods.PAGESIZE).ToList();

            return View(labFilter);
        }

        [HttpPost]
        public ActionResult RopeLabResults(ResidualLabFilter labFilter)
        {
            var results = context.View_ResidualStrength_RopeLabResults.Where(p => p.LabTestDate >= labFilter.DateInstalledFrom && p.LabTestDate <= labFilter.DateInstalledTo).
                OrderBy(u => u.VesselName).ThenBy(u => u.Manufacturer).ThenBy(u => u.RopeType).AsQueryable();

            if (labFilter.VesselIDs?.Count > 0)
                results = results.Where(p => labFilter.VesselIDs.Contains(p.VesselID));

            if (labFilter.ManufacturerIDs?.Count > 0)
                results = results.Where(p => labFilter.ManufacturerIDs.Contains(p.ManufacturerId ?? 0));

            if (labFilter.RopeTypeIDs?.Count > 0)
                results = results.Where(p => labFilter.RopeTypeIDs.Contains(p.RopeTypeId ?? 0));

            results = results.Where(p => p.ServiceMonths >= labFilter.MonthsServiceFrom);
            results = results.Where(p => p.ServiceMonths >= labFilter.MonthsServiceTo);

            results = results.Where(p => p.RunningHours >= labFilter.RunningHoursFrom);
            results = results.Where(p => p.RunningHours >= labFilter.RunningHoursTo);

            results = results.Where(p => p.TestResults >= labFilter.ResidualStrengthFrom);
            results = results.Where(p => p.TestResults >= labFilter.ResidualStrengthTo);

            int currPage = TempData["CurrentPage"] == null ? 1 : Convert.ToInt32(TempData["CurrentPage"]);

            TempData["TotalRecords"] = results.Count();

            var recordsFound = results.OrderBy(u => u.VesselName).ThenBy(u => u.Manufacturer).ThenBy(u => u.RopeType).AsQueryable();

            labFilter.ResidualStrength_RopeLabResults = recordsFound.Skip((CommonMethods.PAGESIZE * currPage) - CommonMethods.PAGESIZE).Take(CommonMethods.PAGESIZE).ToList();

            labFilter.FleetNameList = base.PermittedFleetNames;
            labFilter.FleetTypeList = base.PermittedFleetTypes;

            labFilter.VesselList = context.VesselDetails.Where(p => labFilter.FleetTypeIDs.Contains(p.FleetTypeID) && labFilter.FleetNameIDs.Contains(p.FleetNameID) && labFilter.TradeIDs.Contains(p.TradeAreaID)).ToList();

            return View(labFilter);
        }

        public ActionResult ResidualStrenghtPredictor(int? id)
        {
            ResidualLabFilter labFilter = new ResidualLabFilter();
            labFilter.FleetNameList = base.PermittedFleetNames;
            labFilter.FleetTypeList = base.PermittedFleetTypes;

            labFilter.TestDateFrom = DateTime.Today.AddYears(-1);
            labFilter.TestDateTo = DateTime.Today;
            labFilter.AgeRangeFrom = 0;
            labFilter.AgeRangeTo = 50;

            var recordsFound = context.View_ResidualStrength_RopeLabResults.Where(p => p.LabTestDate >= labFilter.TestDateFrom && p.LabTestDate <= labFilter.TestDateTo).
                OrderBy(u => u.VesselName).ThenBy(u => u.Manufacturer).ThenBy(u => u.RopeType).AsQueryable();

            int currPage = id == null ? 1 : Convert.ToInt32(id);

            TempData["CurrentPage"] = currPage;
            TempData["TotalRecords"] = recordsFound.Count();

            labFilter.ResidualStrength_RopeLabResults = recordsFound.Skip((CommonMethods.PAGESIZE * currPage) - CommonMethods.PAGESIZE).Take(CommonMethods.PAGESIZE).ToList();

            return View(labFilter);
        }

        [HttpPost]
        public ActionResult ResidualStrenghtPredictor(ResidualLabFilter labFilter)
        {
            var results = context.View_ResidualStrength_RopeLabResults.Where(p => p.LabTestDate >= labFilter.TestDateFrom && p.LabTestDate <= labFilter.TestDateTo).
               OrderBy(u => u.VesselName).ThenBy(u => u.Manufacturer).ThenBy(u => u.RopeType).AsQueryable();

            if (labFilter.VesselIDs?.Count > 0)
                results = results.Where(p => labFilter.VesselIDs.Contains(p.VesselID));

            if (labFilter.ManufacturerIDs?.Count > 0)
                results = results.Where(p => labFilter.ManufacturerIDs.Contains(p.ManufacturerId ?? 0));

            if (labFilter.RopeTypeIDs?.Count > 0)
                results = results.Where(p => labFilter.RopeTypeIDs.Contains(p.RopeTypeId ?? 0));

            results = results.Where(p => p.ServiceMonths >= labFilter.MonthsServiceFrom);
            results = results.Where(p => p.ServiceMonths >= labFilter.MonthsServiceTo);

            results = results.Where(p => p.RunningHours >= labFilter.RunningHoursFrom);
            results = results.Where(p => p.RunningHours >= labFilter.RunningHoursTo);

            results = results.Where(p => p.TestResults >= labFilter.ResidualStrengthFrom);
            results = results.Where(p => p.TestResults >= labFilter.ResidualStrengthTo);

            int currPage = TempData["CurrentPage"] == null ? 1 : Convert.ToInt32(TempData["CurrentPage"]);

            TempData["TotalRecords"] = results.Count();

            var recordsFound = results.OrderBy(u => u.VesselName).ThenBy(u => u.Manufacturer).ThenBy(u => u.RopeType).AsQueryable();

            labFilter.ResidualStrength_RopeLabResults = recordsFound.Skip((CommonMethods.PAGESIZE * currPage) - CommonMethods.PAGESIZE).Take(CommonMethods.PAGESIZE).ToList();

            labFilter.FleetNameList = base.PermittedFleetNames;
            labFilter.FleetTypeList = base.PermittedFleetTypes;

            labFilter.VesselList = context.VesselDetails.Where(p => labFilter.FleetTypeIDs.Contains(p.FleetTypeID) && labFilter.FleetNameIDs.Contains(p.FleetNameID) && labFilter.TradeIDs.Contains(p.TradeAreaID)).ToList();

            return View(labFilter);
        }

        public ActionResult FleetWideIncidents(int? id)
        {
            ResidualLabFilter labFilter = new ResidualLabFilter();
            labFilter.FleetNameList = base.PermittedFleetNames;
            labFilter.FleetTypeList = base.PermittedFleetTypes;

            labFilter.TestDateFrom = DateTime.Today.AddYears(-10);
            labFilter.TestDateTo = DateTime.Today;
            labFilter.AgeRangeFrom = 0;
            labFilter.AgeRangeTo = 50;
            // var sp = context.SP_View_FleetWiseIncidents
            //var recordsFound = context.View_FleetWiseIncidents2.Where(p => p.InstalledDate >= labFilter.TestDateFrom && p.InstalledDate <= labFilter.TestDateTo).
            //    OrderBy(u => u.VesselName).ThenBy(u => u.Manufacturer).ThenBy(u => u.RopeType).AsQueryable();

            var recordsFound = CommonMethods.ExecStoredPro_FleetWiseIncidents("SP_View_FleetWiseIncidents", null, labFilter.TestDateFrom, labFilter.TestDateTo);
            int currPage = id == null ? 1 : Convert.ToInt32(id);

            TempData["CurrentPage"] = currPage;
            TempData["TotalRecords"] = recordsFound.Count();

            labFilter.FleetWiseIncidents = recordsFound.Skip((CommonMethods.PAGESIZE * currPage) - CommonMethods.PAGESIZE).Take(CommonMethods.PAGESIZE).ToList();

            return View(labFilter);
        }

        [HttpPost]
        public ActionResult FleetWideIncidents(ResidualLabFilter labFilter)
        {
            //var results = context.View_FleetWiseIncidents2.Where(p => p.InstalledDate >= labFilter.TestDateFrom && p.InstalledDate <= labFilter.TestDateTo).
            //    OrderBy(u => u.VesselName).ThenBy(u => u.Manufacturer).ThenBy(u => u.RopeType).AsQueryable();

            var results = CommonMethods.ExecStoredPro_FleetWiseIncidents("SP_View_FleetWiseIncidents", labFilter.StatusName, labFilter.TestDateFrom, labFilter.TestDateTo);

            if (labFilter.VesselIDs?.Count > 0)
                results = results.Where(p => labFilter.VesselIDs.Contains(p.VesselID)).ToList();

            if (labFilter.ManufacturerIDs?.Count > 0)
                results = results.Where(p => labFilter.ManufacturerIDs.Contains(p.ManufacturerId ?? 0)).ToList();

            if (labFilter.RopeTypeIDs?.Count > 0)
                results = results.Where(p => labFilter.RopeTypeIDs.Contains(p.RopeTypeId ?? 0)).ToList();

            //if (labFilter.StatusName != null)
            //{
            //    if (labFilter.StatusName.IndexOf("DISCARDED", StringComparison.OrdinalIgnoreCase) < 0)
            //        results = results.Where(p => p.OutofServiceDate != null);
            //    else if (labFilter.StatusName.IndexOf("INSERVICE", StringComparison.OrdinalIgnoreCase) < 0)
            //        results = results.Where(p => p.OutofServiceDate == null);
            //}

            int currPage = TempData["CurrentPage"] == null ? 1 : Convert.ToInt32(TempData["CurrentPage"]);

            TempData["TotalRecords"] = results.Count();

            var recordsFound = results.OrderBy(u => u.VesselName).ThenBy(u => u.Manufacturer).ThenBy(u => u.RopeType).AsQueryable();

            labFilter.FleetWiseIncidents = recordsFound.Skip((CommonMethods.PAGESIZE * currPage) - CommonMethods.PAGESIZE).Take(CommonMethods.PAGESIZE).ToList();

            labFilter.FleetNameList = base.PermittedFleetNames;
            labFilter.FleetTypeList = base.PermittedFleetTypes;

            labFilter.VesselList = context.VesselDetails.Where(p => labFilter.FleetTypeIDs.Contains(p.FleetTypeID) && labFilter.FleetNameIDs.Contains(p.FleetNameID) && labFilter.TradeIDs.Contains(p.TradeAreaID)).ToList();

            return View(labFilter);
        }

        public ActionResult RopeStatus(int? id)
        {
            ResidualLabFilter labFilter = new ResidualLabFilter
            {
                FleetNameList = base.PermittedFleetNames,
                FleetTypeList = base.PermittedFleetTypes,
                DateInstalledFrom = DateTime.Today.AddYears(-1),
                DateInstalledTo = DateTime.Today,
                AgeRangeFrom = 0,
                AgeRangeTo = 50
            };

            // var recordsFound = context.View_RopesDamagedCroppedSplicedUsed.OrderBy(u => u.VesselName).ThenBy(u => u.Manufacturer).ThenBy(u => u.RopeType).AsQueryable();
            var recordsFound = context.View_RopesDamagedCroppedSplicedUsed2.OrderBy(u => u.VesselName).AsQueryable();

            int currPage = id == null ? 1 : Convert.ToInt32(id);

            TempData["CurrentPage"] = currPage;
            TempData["TotalRecords"] = recordsFound.Count();

            labFilter.RopesDamagedCroppedSplicedUsed2 = recordsFound.Skip((CommonMethods.PAGESIZE * currPage) - CommonMethods.PAGESIZE).Take(CommonMethods.PAGESIZE).ToList();

            return View(labFilter);
        }

        [HttpPost]
        public ActionResult RopeStatus(ResidualLabFilter labFilter)
        {



            var results = context.View_RopesDamagedCroppedSplicedUsed2.OrderBy(u => u.VesselName).AsQueryable();

            if (labFilter.VesselIDs?.Count > 0)
                results = results.Where(p => labFilter.VesselIDs.Contains(p.VesselID));



            int currPage = TempData["CurrentPage"] == null ? 1 : Convert.ToInt32(TempData["CurrentPage"]);

            TempData["TotalRecords"] = results.Count();

            var recordsFound = results.OrderBy(u => u.VesselName).AsQueryable();

            labFilter.RopesDamagedCroppedSplicedUsed2 = recordsFound.Skip((CommonMethods.PAGESIZE * currPage) - CommonMethods.PAGESIZE).Take(CommonMethods.PAGESIZE).ToList();

            labFilter.FleetNameList = base.PermittedFleetNames;
            labFilter.FleetTypeList = base.PermittedFleetTypes;
            labFilter.VesselList = base.PermittedVessels;
            //labFilter.VesselList = context.VesselDetails.Where(p => labFilter.FleetTypeIDs.Contains(p.FleetTypeID) && labFilter.FleetNameIDs.Contains(p.FleetNameID) && labFilter.TradeIDs.Contains(p.TradeAreaID)).ToList();

            return View(labFilter);
        }

        public JsonResult GetVesselDetails(string fleetNames, string fleetTypes, string tradeAreas)
        {
            fleetNames = fleetNames.EndsWith(",") ? fleetNames.Substring(0, fleetNames.Length - 1) : fleetNames;
            fleetTypes = fleetTypes.EndsWith(",") ? fleetTypes.Substring(0, fleetTypes.Length - 1) : fleetTypes;
            tradeAreas = tradeAreas.EndsWith(",") ? tradeAreas.Substring(0, tradeAreas.Length - 1) : tradeAreas;

            List<int> fleets = new List<int>();
            int id = 0;

            if (!string.IsNullOrEmpty(fleetNames))
            {
                foreach (string s in fleetNames.Split(','))
                {
                    int.TryParse(s, out id);
                    fleets.Add(id);
                }
            }

            List<int> ftypes = new List<int>();

            if (!string.IsNullOrEmpty(fleetTypes))
            {
                foreach (string s in fleetTypes.Split(','))
                {
                    int.TryParse(s, out id);
                    ftypes.Add(id);
                }
            }

            List<int> areas = new List<int>();

            if (!string.IsNullOrEmpty(tradeAreas))
            {
                foreach (string s in tradeAreas.Split(','))
                {
                    int.TryParse(s, out id);
                    areas.Add(id);
                }
            }

            var vessels = base.PermittedVessels; // context.VesselDetails.AsEnumerable();

            if (fleets.Count > 0)
                vessels = vessels.Where(p => fleets.Contains(p.FleetNameID)).ToList();

            if (ftypes.Count > 0)
                vessels = vessels.Where(p => ftypes.Contains(p.FleetTypeID)).ToList();

            if (areas.Count > 0)
                vessels = vessels.Where(p => areas.Contains(p.TradeAreaID)).ToList();

            if ((fleets.Count == 0) && (ftypes.Count == 0) && (areas.Count == 0))
                vessels = new List<VesselDetail>();

            List<VesselModel> lstVessels = new List<VesselModel>();

            foreach (var v in vessels)
                lstVessels.Add(new VesselModel { VesselID = v.ImoNo, VesselName = v.VesselName });

            return Json(new { Result = true, Data = lstVessels }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetFacilities(string portName)
        {
            var facilityList = context.PortLists.Where(p => p.PortName.Equals(portName)).Select(u => u.FacilityName).ToList();
            // ViewBag.FacilityList = new SelectList(facilityList, "Port_Facility_Name");
            //ViewBag.FacilityList = facilityList;
            return Json(new { Result = true, Data = facilityList }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ViewDamageRecords(int opid, int imo)
        {
            var filterModel = new FilterModel
            {
                FleetNames = base.PermittedFleetNames,
                FleetTypes = base.PermittedFleetTypes,
                ListDamagedRopes = context.View_RopeDamages.Where(p => p.MOPId == opid && p.VesselID == imo).ToList()
            };

            return Json(new { Result = true, Data = filterModel.ListDamagedRopes }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetOperationDetails(int opid, int imo)
        {
            // string start = Request.QueryString["vid"].ToString();
            // string vsid = TempData["VsId"].ToString();

            int vesselid = imo;

            var filterModel = new FilterModel
            {
                FleetNames = base.PermittedFleetNames,
                FleetTypes = base.PermittedFleetTypes,
                MooringOperationDetails = context.MOperationBirthDetails.FirstOrDefault(p => p.OPId == opid && p.VesselID == vesselid)
            };

            filterModel.MooringOperationDetails.RopeUsedInOperation = new List<View_OperationWiseRopes>();
            filterModel.MooringOperationDetails.RopeTailsUsedInOperation = new List<View_OperationWiseRopes>();

            // var operations = context.View_OperationWiseRopes.Where(p => p.OperationID == opid && p.VesselID == vesselid).ToList();
            var operations = new List<View_OperationWiseRopes>();
            var ilist = new ShipmentContaxt().MultipleResults("[dbo].[View_OperationWiseRopes_SP] " + opid + "," + vesselid + "").With<View_OperationWiseRopes>().Execute();

            operations = (List<View_OperationWiseRopes>)ilist[0];



            filterModel.MooringOperationDetails.RopeUsedInOperation = operations.Where(p => p.RopeTail == 0).ToList();
            filterModel.MooringOperationDetails.RopeTailsUsedInOperation = operations.Where(p => p.RopeTail == 1).ToList();
            // TempData["VsId"] = vesselid.ToString();
            return Json(new { Result = true, Data = filterModel.MooringOperationDetails }, JsonRequestBehavior.AllowGet);
        }


    }
}