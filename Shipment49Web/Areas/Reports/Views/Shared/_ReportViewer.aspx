<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="_ReportViewer.aspx" Inherits="System.Web.Mvc.ViewPage" %>

<%@ Register Assembly="Microsoft.ReportViewer.WebForms" Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script src="../Scripts/jquery-1.7.1.js"></script>
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

                    reports.ForEach(x => { if (!string.IsNullOrEmpty(x.Remarks)) { x.Remarks = (x.Options + " : " + x.Remarks); } else { x.Remarks = x.Options; } });
                    
                    var bms = reports.Select(z => z.youngseafearer).FirstOrDefault();
                    if (bms)
                    {

                        if (reports.Count == 0)
                        {
                            ReportViewer1.LocalReport.DataSources.Clear();
                            ReportViewer1.LocalReport.Refresh();
                            ReportViewer1.Visible = false;
                            TempData["barmala"] = "Data Not Available For Selected Criteria! Please Select The Vessel, Crew Name and Month to Check Report.";
                        }
                        else
                        {
                            ReportViewer1.Visible = true;
                            ReportViewer1.LocalReport.ReportPath = Server.MapPath("~/RDLCReport/CrewReportYoung.rdlc");
                            ReportViewer1.LocalReport.DataSources.Clear();
                            ReportDataSource rdc = new ReportDataSource("DataSetReport", reports);

                            ReportParameter bbv = new ReportParameter("CompanyName", companyname);
                            ReportViewer1.LocalReport.SetParameters(new ReportParameter[] { bbv });

                            object repotname = "WorkHours " + CrewReportLayer.bmsuploaddat.vesselname + " " + CrewReportLayer.bmsuploaddat.fullname + " " + DateTime.Now.Date.ToString("dd MMM yy");
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
                            TempData["barmala"] = "Data Not Available For Selected Criteria! Please Select The Vessel, Crew Name and Month to Check Report.";

                        }
                        else
                        {
                            ReportViewer1.Visible = true;
                            ReportViewer1.LocalReport.ReportPath = Server.MapPath("~/RDLCReport/CrewReport.rdlc");
                            ReportViewer1.LocalReport.DataSources.Clear();
                            ReportDataSource rdc = new ReportDataSource("DataSetReport", reports);

                            ReportParameter bbv = new ReportParameter("CompanyName", companyname);
                            ReportViewer1.LocalReport.SetParameters(new ReportParameter[] { bbv });

                            object repotname = "WorkHours " + CrewReportLayer.bmsuploaddat.vesselname + " " + CrewReportLayer.bmsuploaddat.fullname + " " + DateTime.Now.Date.ToString("dd MMM yy");
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
    <script type="text/javascript" lang="javascript">

        $(document).ready(function () {

            //if ($.browser.mozilla || $.browser.webkit) {

            try {

                showPrintButton();

            }

            catch (e) { alert(e); }

            //}

        });



        function showPrintButton() {

            var table = $("table[title='Refresh']");

            var parentTable = $(table).parents('table');

            var parentDiv = $(parentTable).parents('div').parents('div').first();

            parentDiv.append('<input type="image" style="border-width: 0px; padding: 3px;margin-top:2px; height:25px; width: 25px;" alt="Print" src="/Reserved.ReportViewerWebControl.axd?OpType=Resource&amp;Version=11.0.0.0&amp;Name=Microsoft.Reporting.WebForms.Icons.Print.gif";title="Print" onclick="PrintReport();">');



        }

        // Print Report function

        function PrintReport() {



            //get the ReportViewer Id

            var rv1 = $('#ReportViewer1');

            var iDoc = rv1.parents('html');



            // Reading the report styles

            var styles = iDoc.find("head style[id$='ReportControl_styles']").html();


            if ((styles == undefined) || (styles == '')) {

                iDoc.find('head script').each(function () {

                    var cnt = $(this).html();

                    var p1 = cnt.indexOf('ReportStyles":"');

                    if (p1 > 0) {

                        p1 += 15;

                        var p2 = cnt.indexOf('"', p1);

                        styles = cnt.substr(p1, p2 - p1);

                    }

                });

            }

            if (styles == '') { alert("Cannot generate styles, Displaying without styles.."); }

            styles = '<style type="text/css">' + styles + "</style>";



            // Reading the report html

            var table = rv1.find("div[id$='_oReportDiv']");


            if (table == undefined) {

                alert("Report source not found.");

                return;

            }



            // Generating a copy of the report in a new window

            var docType = '<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.01//EN" "http://www.w3.org/TR/html4/loose.dtd">';

            var docCnt = styles + table.parent().html();

            var docHead = '<head><style>body{margin:5;padding:0;}</style></head>';

            var winAttr = "location=yes, statusbar=no, directories=no, menubar=no, titlebar=no, toolbar=no, dependent=no, width=920, height=500, resizable=yes, screenX=200, screenY=200, personalbar=no, scrollbars=yes";;

            var newWin = window.open("", "_blank", winAttr);

            writeDoc = newWin.document;

            writeDoc.open();

            writeDoc.write(docType + '<html>' + docHead + '<body onload="window.print();">' + docCnt + '</body></html>');

            writeDoc.close();

            newWin.focus();

            // uncomment to autoclose the preview window when printing is confirmed or canceled.

            //newWin.close();

        };

    </script>


</head>
<body>
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
        <rsweb:ReportViewer ID="ReportViewer1" runat="server" AsyncRendering="false" SizeToReportContent="false" Height="700px" Width="100%" BorderColor="#CCCCCC" BorderStyle="Solid" ShowBackButton="False" ShowFindControls="False" ShowZoomControl="False"></rsweb:ReportViewer>
    </form>
</body>
</html>
