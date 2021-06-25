<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="_ReportViewerWorkschedule.aspx" Inherits="System.Web.Mvc.ViewPage" %>

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

                List<CrewReportLayer.CrewDetailClass> reports = null;
                using (MenuLayer.ShipmentContaxt sc = new MenuLayer.ShipmentContaxt())
                {
                    sc.Configuration.ProxyCreationEnabled = false;
                    String companyname = sc.Companys.Select(x => x.CompanyName).FirstOrDefault();
                    reports = sc.CrewDetails.OrderBy(a => a.Position).Where(p => p.VesselName.ToLower() == CrewReportLayer.bmsuploaddat.vesselname.ToLower()).ToList();
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

                        ReportViewer1.LocalReport.ReportPath = Server.MapPath("~/RDLCReport/CrewDetailReport.rdlc");
                        ReportViewer1.LocalReport.DataSources.Clear();
                        ReportDataSource rdc = new ReportDataSource("DataSetCrewDetail", reports);

                        ReportParameter bbv = new ReportParameter("CompanyName", companyname);
                        ReportViewer1.LocalReport.SetParameters(new ReportParameter[] { bbv });

                        object repotname = "IndividualWorkSchedule " + CrewReportLayer.bmsuploaddat.vesselname +" " + DateTime.Now.Date.ToString("MMM yy");

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
        <rsweb:ReportViewer ID="ReportViewer1" runat="server" asyncrendering="false" sizetoreportcontent="false" height="680px" width="1150px" bordercolor="#CCCCCC" borderstyle="Solid" showbackbutton="False" showfindcontrols="False" showzoomcontrol="False"></rsweb:ReportViewer>
    </div>
    </form>
</body>
</html>
