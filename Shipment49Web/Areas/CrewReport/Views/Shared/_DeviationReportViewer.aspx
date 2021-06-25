<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="_DeviationReportViewer.aspx" Inherits="System.Web.Mvc.ViewPage" %>

<%@ Register Assembly="Microsoft.ReportViewer.WebForms" Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script runat="server">

        void Page_Load(object sender, EventArgs e)
        {

            if (!IsPostBack)
            {

                //List<CrewReportLayer.CreReportClass> reports = null;
                using (MenuLayer.ShipmentContaxt sc = new MenuLayer.ShipmentContaxt())
                {
                    sc.Configuration.ProxyCreationEnabled = false;

                    var  reports = sc.CreReports.OrderBy(a => a.Dates).Where(p => p.VesselName.ToLower() == CrewReportLayer.bmsuploaddat.vesselname.ToLower() &&
                      p.FullName.ToLower() == CrewReportLayer.bmsuploaddat.fullname.ToLower() && p.Dates.Month == CrewReportLayer.bmsuploaddat.datefrom &&
                      p.Dates.Year == CrewReportLayer.bmsuploaddat.dateto).Select(a=> new {a.Wid,a.ID,a.Vessel_ID,a.VesselName,a.UserName,a.FullName,a.Position,a.Department,a.Dates,a.NonConfirmities,a.Remarks}).ToList();

                    if (reports.Count == 0)
                    {
                        ReportViewer1.LocalReport.DataSources.Clear();
                        ReportViewer1.LocalReport.Refresh();
                        ReportViewer1.Visible = false;
                        TempData["barmala"] = "Data not available for selected criteria! please select the other criteria.";
                    }
                    else
                    {
                        ReportViewer1.Visible = true;
                        ReportViewer1.LocalReport.ReportPath = Server.MapPath("~/RDLCReport/DeviationReport.rdlc");
                        ReportViewer1.LocalReport.DataSources.Clear();
                        ReportDataSource rdc = new ReportDataSource("DataSetDeviationReport", reports);

                        object repotname = "Deviation "  + CrewReportLayer.bmsuploaddat.vesselname + " "+CrewReportLayer.bmsuploaddat.fullname + " "  + DateTime.Now.Date.ToString("dd MMM yy");
                        ReportViewer1.LocalReport.DisplayName = repotname.ToString();

                        ReportViewer1.LocalReport.DataSources.Add(rdc);
                        ReportViewer1.LocalReport.Refresh();
                    }

                }
            }
        }
    </script>

</head>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
     <rsweb:ReportViewer ID="ReportViewer1" runat="server" asyncrendering="false" sizetoreportcontent="false" height="680px" width="1030px" bordercolor="#CCCCCC" borderstyle="Solid" showbackbutton="False" showfindcontrols="False" showzoomcontrol="False"></rsweb:ReportViewer>
    </div>
    </form>
</body>
</html>
