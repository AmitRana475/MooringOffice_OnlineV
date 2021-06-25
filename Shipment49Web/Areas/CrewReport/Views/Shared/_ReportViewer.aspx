<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="_ReportViewer.aspx" Inherits="System.Web.Mvc.ViewPage" %>

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

                 List<CrewReportLayer.CreReportClass> reports = null;
                 using (MenuLayer.ShipmentContaxt sc = new MenuLayer.ShipmentContaxt())
                 {
                     sc.Configuration.ProxyCreationEnabled = false;

                     String companyname = sc.Companys.Select(x => x.CompanyName).FirstOrDefault();
                     reports = sc.CreReports.OrderBy(a => a.Dates).Where(p => p.VesselName.ToLower() == CrewReportLayer.bmsuploaddat.vesselname.ToLower() &&
                     p.FullName.ToLower() == CrewReportLayer.bmsuploaddat.fullname.ToLower() && p.Dates.Month == CrewReportLayer.bmsuploaddat.datefrom &&
                     p.Dates.Year == CrewReportLayer.bmsuploaddat.dateto).ToList();
                     var bms = reports.Select(z => z.youngseafearer).FirstOrDefault();
                     if (bms)
                     {

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
                             ReportViewer1.LocalReport.ReportPath = Server.MapPath("~/RDLCReport/CrewReportYoung.rdlc");
                             ReportViewer1.LocalReport.DataSources.Clear();
                             ReportDataSource rdc = new ReportDataSource("DataSetReport", reports);

                             ReportParameter bbv = new ReportParameter("CompanyName", companyname);
                             ReportViewer1.LocalReport.SetParameters(new ReportParameter[] { bbv });

                             object repotname = "WorkHours "  + CrewReportLayer.bmsuploaddat.vesselname + " "+ CrewReportLayer.bmsuploaddat.fullname + " " + DateTime.Now.Date.ToString("dd MMM yy");
                             ReportViewer1.LocalReport.DisplayName = repotname.ToString();

                             ReportViewer1.LocalReport.DataSources.Add(rdc);
                             ReportViewer1.LocalReport.Refresh();
                         }
                     }
                     else
                     {

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
                             ReportViewer1.LocalReport.ReportPath = Server.MapPath("~/RDLCReport/CrewReport.rdlc");
                             ReportViewer1.LocalReport.DataSources.Clear();
                             ReportDataSource rdc = new ReportDataSource("DataSetReport", reports);

                             ReportParameter bbv = new ReportParameter("CompanyName",companyname);
                             ReportViewer1.LocalReport.SetParameters(new ReportParameter[] { bbv });

                             object repotname = "WorkHours "  + CrewReportLayer.bmsuploaddat.vesselname + " "+ CrewReportLayer.bmsuploaddat.fullname + " " + DateTime.Now.Date.ToString("dd MMM yy");
                             ReportViewer1.LocalReport.DisplayName = repotname.ToString();

                             ReportViewer1.LocalReport.DataSources.Add(rdc);
                             ReportViewer1.LocalReport.Refresh();
                             TempData["barmala"] = "";
                         }
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
    
        
     <rsweb:ReportViewer ID="ReportViewer1" runat="server" asyncrendering="false" sizetoreportcontent="false" height="700px" width="1030px" bordercolor="#CCCCCC" borderstyle="Solid" showbackbutton="False" showfindcontrols="False" showzoomcontrol="False"></rsweb:ReportViewer>
    </div>
        


    </form>
</body>
</html>
