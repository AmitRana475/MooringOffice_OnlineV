using MenuLayer;
using Newtonsoft.Json;
using Reports;
using Shipment49Web.Common;
using Shipment49Web.Controllers;
using Shipment49Web.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;

namespace Shipment49Web.Areas.Analysis.Controllers
{
    [Authorize]
    [UserAuthenticationFilter]
    [ErrorClass]
    public class RopeAnalysisController : BaseController
    {
        MorringOfficeEntities context = new MorringOfficeEntities();

        public RopeAnalysisController()
        {
            using (ShipmentContaxt sc1 = new ShipmentContaxt())
            {
                var _smartmenu = sc1.SmartMenus.ToList().FirstOrDefault();
                if (_smartmenu != null)
                {
                    ViewBag.MenuContent = _smartmenu.SmartMenuContent;
                }
            }
        }

        public ActionResult Index()
        {
            var ropeAnalysis = new RopeAnalysis
            {
                FleetNames = context.tblCommons.Where(u => u.Type == (int)CommonType.FleetName).ToList(),
                FleetTypes = context.tblCommons.Where(u => u.Type == (int)CommonType.FleetType).ToList(),
                TradePlatforms = context.tblCommons.Where(u => u.Type == (int)CommonType.TradePlatform).ToList(),
                Year = DateTime.Now.Year,
                RunningHoursFrom = 0,
                RunningHoursTo = 100000,
                ChartData = new List<GraphData>(),
                SelectedManufacturers = string.Empty,
                SelectedRopeTypes = string.Empty,
                SelectedVessels = string.Empty
            };

            ropeAnalysis.DateFrom = new DateTime(ropeAnalysis.Year, 1, 1);
            ropeAnalysis.DateUpto = new DateTime(ropeAnalysis.Year, 12, 31);

            ropeAnalysis.RopeAnalysis = CommonMethods.GetRopeAnalysis(ropeAnalysis.SelectedVessels, ropeAnalysis.SelectedManufacturers,
                ropeAnalysis.SelectedRopeTypes, ropeAnalysis.RunningHoursFrom, ropeAnalysis.RunningHoursTo, (DateTime)ropeAnalysis.DateFrom, (DateTime)ropeAnalysis.DateUpto);

            for (int i = 1; i < 8; i++)
                ropeAnalysis.ChartData.Add(new GraphData() { chartId = i, data = ropeAnalysis.GetChartData(ropeAnalysis.RopeAnalysis, i) });

            return View(ropeAnalysis);
        }


        [HttpPost]
        public ActionResult Index(RopeAnalysis ropeAnalysis)
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

            ropeAnalysis.DateFrom = new DateTime(ropeAnalysis.Year, 1, 1);
            ropeAnalysis.DateUpto = new DateTime(ropeAnalysis.Year, 12, 31);

            ropeAnalysis.RopeAnalysis = CommonMethods.GetRopeAnalysis(ropeAnalysis.SelectedVessels, ropeAnalysis.SelectedManufacturers,
                ropeAnalysis.SelectedRopeTypes, ropeAnalysis.RunningHoursFrom, ropeAnalysis.RunningHoursTo, (DateTime)ropeAnalysis.DateFrom, (DateTime)ropeAnalysis.DateUpto);

            ropeAnalysis.ChartData = new List<GraphData>();

            for (int i = 1; i < 8; i++)
                ropeAnalysis.ChartData.Add(new GraphData() { chartId = i, data = ropeAnalysis.GetChartData(ropeAnalysis.RopeAnalysis, i) });

            return View(ropeAnalysis);
        }

        //private string GetChartData(DataTable analysisData, int rating)
        //{
        //    List<ChartDetails> lstGraphData_1 = new List<ChartDetails>
        //    {
        //        GetInternalAbrasionData(analysisData, rating, true, System.Drawing.Color.LightBlue),
        //        GetInternalAbrasionData(analysisData, rating, false, System.Drawing.Color.Orange)
        //    };

        //    return JsonConvert.SerializeObject(lstGraphData_1);
        //}

        //private ChartDetails GetInternalAbrasionData(DataTable analysisData, int rating, bool internalRating, System.Drawing.Color colorName)
        //{
        //    ChartDetails graphData = new ChartDetails
        //    {
        //        backgroundColor = colorName,
        //        borderWidth = 1,
        //    };

        //    foreach (DataRow row in analysisData.Rows)
        //    {
        //        if (internalRating)
        //        {
        //            graphData.label = string.Format("Internal Abrasion - {0}", rating);
        //            graphData.data.Add(Convert.ToInt32(row[string.Format("A_{0}", rating)]));
        //        }
        //        else
        //        {
        //            graphData.label = string.Format("External Abrasion - {0}", rating);
        //            graphData.data.Add(Convert.ToInt32(row[string.Format("B_{0}", rating)]));
        //        }
        //    }

        //    return graphData;
        //}

        //public class ChartDetails
        //{
        //    public ChartDetails()
        //    {
        //        data = new List<int>();
        //    }

        //    public string label { get; set; }
        //    public System.Drawing.Color backgroundColor { get; set; }
        //    public int borderWidth { get; set; }
        //    public List<int> data { get; set; }
        //}
    }
}