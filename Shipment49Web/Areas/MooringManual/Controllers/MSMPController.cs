


using iTextSharp;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.html;
using iTextSharp.text.xml;
using iTextSharp.text.io;

using iTextSharp.text.html.simpleparser;
using iTextSharp.text.xml.simpleparser;
using MenuLayer;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using Reports;
using Shipment49Web.Areas.MooringManual.Models;
using Shipment49Web.Areas.MSPS.Models;
using Shipment49Web.Common;
using Shipment49Web.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using iTextSharp.tool.xml;
using iTextSharp.tool.xml.parser;
using iTextSharp.tool.xml.pipeline.css;
using iTextSharp.tool.xml.pipeline.html;
using iTextSharp.tool.xml.pipeline.end;
using iTextSharp.tool.xml.html;


namespace Shipment49Web.Areas.MooringManual.Controllers
{
    [Authorize]
    [UserAuthenticationFilter]
    [ErrorClass]
    public class MSMPController : BaseController
    {
        MorringOfficeEntities context = new MorringOfficeEntities();
        SqlConnection con = ConnectionBulder.con;
        static int? Menuid = 0;
        // GET: MooringManual/MSMP

        public static int VesselID { get; set; }
        public MSMPController()
        {
           // VesselID = Convert.ToInt32(CommonClass.VesselSessionID);
            CommonClass.TopeMenuID = "Menu2";
        }
        public ActionResult Index(int? id)
        {
            VesselID = Convert.ToInt32(Session["VesselSessionID"].ToString());
            DetailsViewModel model = new DetailsViewModel();

            var menulist = context.tblSmartMenus.Select(x => x.SmartMenuContentExport).SingleOrDefault();
            ViewBag.MenuContent1 = menulist;

           // var Json = JsonConvert.DeserializeObject<List<tblMenu>>(menulist);

            //if (id==null)
            //{
            //    id = 1;
            //}

            //Menuid = id;

            var Json = JsonConvert.DeserializeObject<List<MenuName>>(menulist);
            // MenuName m = JsonConvert.DeserializeObject<MenuName>(menulist);

            //using (SqlDataAdapter adp5 = new SqlDataAdapter("select * from tblmenuname ", con))
            //{
            //    DataTable dt5 = new DataTable();
            //    adp5.Fill(dt5);

            //    List<tblMenu> studentList = new List<tblMenu>();
            //    for (int i = 0; i < dt5.Rows.Count; i++)
            //    {
            //        tblMenu student = new tblMenu();
            //        student.Id = Convert.ToInt32(dt5.Rows[i]["Id"]);
            //        student.Mid = Convert.ToInt32(dt5.Rows[i]["Mid"]);
            //        student.MenuName = dt5.Rows[i]["MenuName"].ToString();
            //        student.Type = Convert.ToInt32(dt5.Rows[i]["Type"]);
            //        studentList.Add(student);
            //    }

            //    //List<MenuName> fundList = dt5;
            //    ViewBag.MenuContent1 = studentList;
            //    //ViewBag.MenuContent1 = dt5;
            //}



            //using (ShipmentContaxt sc1 = new ShipmentContaxt())
            //{

            //    var _smartmenu = sc1.SmartMenus.ToList().FirstOrDefault();
            //    if (_smartmenu != null)
            //    {
            //        ViewBag.MenuContent1 = _smartmenu.SmartMenuContent;
            //    }
            //}



            using (SqlDataAdapter adp = new SqlDataAdapter("truncate table tblmenuname", con))
            {
                DataTable dt = new DataTable();
                adp.Fill(dt);
            }

            foreach (var rootObject in Json)
            {
                var Mid = rootObject.id;
                var Mname = rootObject.text;
                var type = rootObject.type;
                List<MenuName> ss = rootObject.children;



                //if (ss == null)
                //{
                using (SqlDataAdapter adp = new SqlDataAdapter("INSERT INTO tblmenuname (Mid, MenuName, Type) VALUES (" + Mid + ", '" + Mname + "', " + type + ")", con))
                {
                    DataTable dt = new DataTable();
                    adp.Fill(dt);
                }
                //}
                if (ss != null)
                {
                    foreach (var item in ss)
                    {
                        var Mid1 = item.id;
                        var Mname1 = item.text;
                        var type1 = item.type;
                        List<MenuName> ss1 = item.children;

                        //if (ss1 == null)
                        //{

                        using (SqlDataAdapter adp = new SqlDataAdapter("INSERT INTO tblmenuname (Mid, MenuName, Type) VALUES (" + Mid1 + ", '" + Mname1 + "', " + type1 + ")", con))
                        {
                            DataTable dt = new DataTable();
                            adp.Fill(dt);
                        }
                        //}

                        if (ss1 != null)
                        {
                            foreach (var item1 in ss1)
                            {
                                var Mid11 = item1.id;
                                var Mname11 = item1.text;
                                var type11 = item1.type;


                                using (SqlDataAdapter adp = new SqlDataAdapter("INSERT INTO tblmenuname (Mid, MenuName, Type) VALUES (" + Mid11 + ", '" + Mname11 + "', " + type11 + ")", con))
                                {
                                    DataTable dt = new DataTable();
                                    adp.Fill(dt);
                                }
                            }
                        }

                    }
                }
            }
            string qry = "";
            if (id != null)
            {
                qry = "select * from tblmenuname where type = 2 and mid= " + id + "";
            }
            if (id == null)
            {
                qry = "select * from tblmenuname where type = 2";
            }

            using (SqlDataAdapter adp = new SqlDataAdapter(qry, con))
            {
                DataTable dt = new DataTable();
                adp.Fill(dt);

                if(dt.Rows.Count> 0)
                {
                    if (id == null)
                    {
                        Menuid = Convert.ToInt32(dt.Rows[0]["Mid"]);
                        ViewBag.qrstrID = Menuid;
                        //id = Menuid;
                    }
                    else
                    {
                        Menuid = id;
                    }
                    TempData["title"] = dt.Rows[0]["MenuName"].ToString();

                    Session["PDFtitle"] = dt.Rows[0]["MenuName"].ToString() + ".pdf";

                    //string path = @"D:\"+ TempData["PDFtitle"].ToString() + "";

                    //if (!System.IO.File.Exists(path))
                    //{
                    //    System.IO.File.Create(path).Dispose();

                      

                    //}
                }
            }



            //model.Content1 = context.ShipSpecificContents.ToList()
            //    .Where(m => m.MId == id && m.ShipId == VesselID)
            //    .OrderByDescending(p => p.Id)
            //    .Select(x => x.Content).FirstOrDefault();

            //model.Content1 = context.tblShipSpecificContents.Where(x => x.MId == Menuid && x.ShipId == VesselID.ToString()).Select(x => x.Content).SingleOrDefault();
            return View();
            //return RedirectToAction("Index", new { Id = id });
        }

        //[HttpPost]
        //public ActionResult Index(int? Mid)
        //{
        //    //var menulist = context.tblSmartMenus.Select(x => x.SmartMenuContentExport).SingleOrDefault();
        //    //ViewBag.MenuContent1 = menulist;
        //    Menuid = Mid;
        //    return View();
        //}

            [HttpGet]
        public ActionResult CheckMid(int id)
        {
            return View();
        }

        [HttpPost]
        [PreventSpam("_revisiondetails", 3, 1)]
        public ActionResult _revisiondetails( int? page)
        {
           

            int currPage = page == null ? 1 : Convert.ToInt32(page);

            TempData["CurrentPage"] = currPage;

            Reports.MorringOfficeEntities entities = new Reports.MorringOfficeEntities();

            var revisions = entities.Revisions.OrderByDescending(p => p.Id).Where(u => u.Mid == Menuid).AsEnumerable();

            TempData["TotalRecords"] = revisions.Count();
            revisions = revisions.Skip((CommonMethods.PAGESIZE * currPage) - CommonMethods.PAGESIZE).Take(CommonMethods.PAGESIZE).ToList();

            return PartialView("_revisionDetail", revisions);
        }

        //public ActionResult _searchData(string searchText)
        //{
        //    using (ShipmentContaxt sc1 = new ShipmentContaxt())
        //    {
        //        SearchTextClass model = new SearchTextClass();
        //        var ilist = new ShipmentContaxt()
        //             .MultipleResults("[dbo].[GetSearchText] '" + searchText + "'")
        //           .With<SearchTextClass>()                   
        //           .Execute();
        //        model.SearchTextList = (List<SearchTextClass>)ilist[0];
        //        return PartialView("_searchData", model);
        //        //return View(model);
        //    }
        //    //return PartialView("_revisionDetail", revisions);
        //}
        public JsonResult _searchData(string searchText)
        {
            //using (Reports.MorringOfficeEntities officeEntities = new Reports.MorringOfficeEntities())
            //{
            //    var notificationComments = officeEntities.NotificationComments.
            //        Where(u => u.NotificationId == 1 & u.VesselID == VesselID).ToList();

            //    //return Json(new { Result = true, Data = notificationComments }, JsonRequestBehavior.AllowGet);
            //}

            SqlDataAdapter adp = new SqlDataAdapter("GetSearchText", con);
            adp.SelectCommand.CommandType = CommandType.StoredProcedure;
            adp.SelectCommand.Parameters.AddWithValue("@text", searchText);
            DataTable dt = new DataTable();
            adp.Fill(dt);

            List<SearchTextClass> stc = new List<SearchTextClass>();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                SearchTextClass search = new SearchTextClass();
                
                search.MenuName = dt.Rows[i]["MenuName"].ToString();
                string ss = dt.Rows[i]["Content"].ToString();

                search.Mid = Convert.ToInt32(dt.Rows[i]["Mid"]);

                StringBuilder sb = new StringBuilder();

                //string ss="<head>< meta http - equiv = 'Content-Type content' = text / html; charset = UTF - 8 >
                sb.Append("<head><meta http-equiv='Content-Type' content='text/html;charset=UTF-8'>");
                sb.Append(ss);
                sb.Append("</head>");

                var fragment = Regex.Replace(sb.ToString(), "<!--.*?-->", String.Empty, RegexOptions.Multiline);

                string abc = fragment;



                string searchString = "cargo";
                string content = abc;
                string replacePattern = "$1<span style=\"background-color:yellow\">$2</span>$3";
                string searchPattern = String.Format("(>[^<>]*?)({0})([^<>]*?<)", searchString.Trim());
                content = Regex.Replace(content, searchPattern, replacePattern, RegexOptions.IgnoreCase);

                // string ss = (string)row["Content"];

                Regex clearMarkup = new Regex(@"(<!--\[.*\]-->)", RegexOptions.Singleline);
                ss = clearMarkup.Replace(ss, ""); // str is the content as shown above.       

                string pattern = @"font-size\s*?:.*?(;|(?=""|'|;))";
                ss = Regex.Replace(ss, pattern, string.Empty);

                ss = Regex.Replace(ss, "<html><head>.*</head></html>", "", RegexOptions.Singleline);

                ss = Regex.Replace(ss, @"(?s)(?<=<!--).+?(?=-->)", "");

                ss = Regex.Replace(ss, "(<style.+?</style>)|(<script.+?</script>)", "", RegexOptions.IgnoreCase | RegexOptions.Singleline);
                ss = Regex.Replace(ss, "(<img.+?>)", "", RegexOptions.IgnoreCase | RegexOptions.Singleline);
                ss = Regex.Replace(ss, "(<o:.+?</o:.+?>)", "", RegexOptions.IgnoreCase | RegexOptions.Singleline);
                ss = Regex.Replace(ss, "<!--.+?-->", "", RegexOptions.IgnoreCase | RegexOptions.Singleline);
                ss = Regex.Replace(ss, "class=.+?>", ">", RegexOptions.IgnoreCase | RegexOptions.Singleline);
                ss = Regex.Replace(ss, "class=.+?\\s", " ", RegexOptions.IgnoreCase | RegexOptions.Singleline);
                ss = Regex.Replace(ss.Replace(System.Environment.NewLine, "<br/>"), "<[^(a|img|b|i|u|ul|ol|li)][^>]*>", " ");
                ss = ss.Replace("&nbsp;", "");
                ss = ss.Replace("\n;", string.Empty);

                ss = "<p>" + ss
     .Replace(Environment.NewLine + Environment.NewLine, "</p><p>")
     .Replace(Environment.NewLine, "<br />")
     .Replace("</p><p>", "</p>" + Environment.NewLine + "<p>") + "</p>";


              
                ss = Regex.Replace(ss, @"\t|\n|\r", "");

                ss = Regex.Replace(ss, "<.*?>|&.*?;", string.Empty);

                //ss = Regex.Replace(ss, "font-family:\\s*\".*\";?", string.Empty);

                search.Content = ss;


                stc.Add(search);
            }
            return Json(new { Result = true, Data = stc }, JsonRequestBehavior.AllowGet);
            //using (ShipmentContaxt sc1 = new ShipmentContaxt())
            //{
            //    SearchTextClass model = new SearchTextClass();
            //    var ilist = new ShipmentContaxt()
            //         .MultipleResults("[dbo].[GetSearchText] '" + searchText + "'")
            //       .With<SearchTextClass>()
            //       .Execute();
            //    model.SearchTextList = (List<SearchTextClass>)ilist[0];
            //    return Json(new { Result = true, Data = model.SearchTextList }, JsonRequestBehavior.AllowGet);
            //}


        }
        private static string HtmlToPlainText(string html)
        {
            const string tagWhiteSpace = @"(>|$)(\W|\n|\r)+<";//matches one or more (white space or line breaks) between '>' and '<'
            const string stripFormatting = @"<[^>]*(>|$)";//match any character between '<' and '>', even when end tag is missing
            const string lineBreak = @"<(br|BR)\s{0,1}\/{0,1}>";//matches: <br>,<br/>,<br />,<BR>,<BR/>,<BR />
            var lineBreakRegex = new Regex(lineBreak, RegexOptions.Multiline);
            var stripFormattingRegex = new Regex(stripFormatting, RegexOptions.Multiline);
            var tagWhiteSpaceRegex = new Regex(tagWhiteSpace, RegexOptions.Multiline);

            var text = html;
            //Decode html specific characters
            text = System.Net.WebUtility.HtmlDecode(text);
            //Remove tag whitespace/line breaks
            text = tagWhiteSpaceRegex.Replace(text, "><");
            //Replace <br /> with line breaks
            text = lineBreakRegex.Replace(text, Environment.NewLine);
            //Strip formatting
            text = stripFormattingRegex.Replace(text, string.Empty);

            return text;
        }

        public JsonResult Viewattachments()
        {
            using (Reports.MorringOfficeEntities officeEntities = new Reports.MorringOfficeEntities())
            {
                var shipspecattach = officeEntities.tblShipSpecificAttachments.
                    Where(u => u.MId == Menuid & u.ShipId == VesselID.ToString()).ToList();

                return Json(new { Result = true, Data = shipspecattach }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetSpecificContent()
        {
            ShipmentContaxt sc1 = new ShipmentContaxt();
            //List<SelectListItem> docs = new List<SelectListItem>();
            var docscontent = sc1.ShipSpecificContents.ToList()
                .Where(m => m.MId == Menuid && m.ShipId == VesselID.ToString())
                .OrderByDescending(p => p.Id)
                .Select(x => x.Content).FirstOrDefault();

          
            return Json(docscontent, JsonRequestBehavior.AllowGet);
        }

        public JsonResult generatePDF1()
        {
            SqlDataAdapter adp = new SqlDataAdapter("select content from docspages where mid=" + Menuid + "", con);
            DataTable dt = new DataTable();
            adp.Fill(dt);
            if (dt.Rows.Count > 0)
            {
                string content = dt.Rows[0][0].ToString();
                string CSS = @"body {font-size: 12px;} table {border-collapse:collapse; margin:8px;}.light-yellow {background-color:#ffff99;}td {border:1px solid #ccc;padding:4px;}";

                // CreatePDF(content);
                //ConvertHtmlToPdf(content, CSS);

                string HTMLContent = "Hello <b>World</b>";



                //StringReader sr = new StringReader(HTMLContent.ToString());

                //Response.Clear();           // Already have this
                //Response.ClearContent();    // Add this line
                //Response.ClearHeaders();

                //iTextSharp.text.Document pdfDoc = new iTextSharp.text.Document(PageSize.A4, 25, 10, 25, 10);
                //PdfWriter pdfWriter = PdfWriter.GetInstance(pdfDoc, Response.OutputStream);
                //pdfDoc.Open();
                //Paragraph Text = new Paragraph("This is test file");
                //pdfDoc.Add(Text);
                //pdfWriter.CloseStream = false;
                //pdfDoc.Close();
                //Response.Buffer = true;
                //Response.ContentType = "application/pdf";
                //Response.AddHeader("content-disposition", "attachment;filename=Example.pdf");
                //Response.Cache.SetCacheability(HttpCacheability.NoCache);
                //Response.Write(pdfDoc);
                //Response.Flush();
                //Response.End();
                string filename = "test";
                using (StringWriter sw = new StringWriter())
                {
                    using (HtmlTextWriter hw = new HtmlTextWriter(sw))
                    {
                        //StringBuilder sb = new StringBuilder();
                        StringReader sr = new StringReader(HTMLContent.ToString());
                        /*
                        //Generate Invoice (Bill) Header.
                        sb.Append("<table width='100%' cellspacing='0' cellpadding='2'>");
                        sb.Append("<tr><td align='center' style='background-color: #18B5F0' colspan = '2'><b>Order Sheet</b></td></tr>");
                        sb.Append("<tr><td colspan = '2'></td></tr>");
                        sb.Append("<tr><td><b>Order No: </b>");
                        //sb.Append(orderNo);
                        sb.Append("</td><td align = 'right'><b>Date: </b>");
                        sb.Append("<br />");
                        sb.Append(" </td></tr>");
                        sb.Append("<tr><td colspan = '2'><b>Company Name: </b>");
                        //sb.Append(companyName);
                        sb.Append("</td></tr>");
                        sb.Append("</table>");
                        sb.Append("<br />");


                        <head><style> table, th, td { border: 1px solid black; border-collapse: collapse;} th, td { padding: 5px; text-align: center; } </style></head>

                        */
                        //sb.Append("<!DOCTYPE html>");
                        //sb.Append("<html>");
                        ////sb.Append("<head><style> table, th, td { border: 1px solid grey; border-collapse: collapse;} th, td { padding: 5px; text-align: center; } </style></head>");
                        //sb.Append("<h2> jhjhjj</h2>");
                        ////sb.Append(Datefrom);
                        //sb.Append("<br />");
                        ////sb.Append("Vessel : " + VsName + "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; IMO : " + IMO + "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; Flag : " + Flag + "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" + Datefrom);
                        ////sb.Append(VesselInfo);
                        //sb.Append("<br />");
                        //sb.Append("<br />");
                        ////Generate Invoice (Bill) Items Grid.
                        //sb.Append("<body>");
                        //sb.Append("<table style='width: 100% ; border: 1px solid grey ;border-collapse: collapse; padding: 5px; text-align: center; ' border = '1' color = 'solid grey' ");
                        //sb.Append("<tr>");
                        //int i = 0;
                        ////string[] ColHeader = { "Rank/Position", "Seafarer Name", "Watchkeeper", "Total Hours Worked", "Total Hours of Work where NC's occur", "% Breach" };
                        //foreach (DataColumn column in dt.Columns)
                        //{
                        //    //sb.Append("<th style = 'background-color: #D20B0C;color:#ffffff'>");
                        //    sb.Append("<th>");
                        //    // sb.Append(column.Caption);
                        //    sb.Append("<b>");
                        //    sb.Append(ColHeader[i]);
                        //    //sb.Append(column.Caption);
                        //    sb.Append("</b>");
                        //    sb.Append("</th>");
                        //    i++;
                        //}
                        //sb.Append("</tr>");
                        //foreach (DataRow row in dt.Rows)
                        //{
                        //    sb.Append("<tr>");
                        //    foreach (DataColumn column in dt.Columns)
                        //    {
                        //        //sb.Append("<td>");
                        //        //sb.Append(column.ColumnName);
                        //        //sb.Append("</td>");
                        //        //==========
                        //        sb.Append("<td>");
                        //        sb.Append(row[column]);
                        //        sb.Append("</td>");
                        //    }
                        //    sb.Append("</tr>");
                        //}
                        ////sb.Append("<tr><td align = 'right' colspan = '");
                        ////sb.Append(dt.Columns.Count - 1);
                        ////sb.Append("'>Total</td>");
                        ////sb.Append("<td>");
                        ////sb.Append(dt.Compute("sum(Total)", ""));
                        ////sb.Append("</td>");
                        ////sb.Append("</tr></table>");
                        //sb.Append("</table>");
                        //sb.Append("</body>");
                        //sb.Append("</html>");
                        //Export HTML String as PDF.
                        //StringReader sr = new StringReader(sb.ToString());
                        iTextSharp.text.Document pdfDoc = new iTextSharp.text.Document(PageSize.A4, 10f, 10f, 10f, 0f);
                        HTMLWorker htmlparser = new HTMLWorker(pdfDoc);
                        PdfWriter writer = PdfWriter.GetInstance(pdfDoc, Response.OutputStream);
                        pdfDoc.Open();
                        htmlparser.Parse(sr);
                        pdfDoc.Close();
                        Response.ContentType = "application/pdf";
                        Response.AddHeader("content-disposition", "attachment;filename=" + filename + DateTime.Now + ".pdf");
                        Response.Cache.SetCacheability(HttpCacheability.NoCache);
                        Response.Write(pdfDoc);
                        Response.End();
                    }
                }


                //  CreatePDFFromHTMLFile(strHtml, pdfFileName);

                //  Response.ContentType = "application/x-download";
                //  Response.AddHeader("Content-Disposition", string.Format("attachment; filename=\"{0}\"", "GenerateHTMLTOPDF.pdf"));
                //  Response.WriteFile(pdfFileName);
                //  Response.Flush();
                //  Response.End();


                //iTextSharp.text.Document pdfDoc = new iTextSharp.text.Document(PageSize.A4, 10f, 10f, 10f, 0f);
                //HTMLWorker htmlparser = new HTMLWorker(pdfDoc);


                //using (MemoryStream memoryStream = new MemoryStream())
                //{
                //    PdfWriter writer = PdfWriter.GetInstance(pdfDoc, memoryStream);
                //    pdfDoc.Open();

                //    htmlparser.Parse(sr);
                //    pdfDoc.Close();

                //    byte[] bytes = memoryStream.ToArray();
                //    memoryStream.Close();




                ////string HTMLContent = content;
                //Response.ClearContent();
                //Response.ClearHeaders();
                //Response.ContentType = "application/pdf";
                //Response.AddHeader("content-disposition", "attachment;filename=" + "PDFfile.pdf");
                //Response.Cache.SetCacheability(HttpCacheability.NoCache);
                //Response.BinaryWrite(GetPDF(HTMLContent));
                //Response.End();
            }


            return Json( JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        //[ValidateInput(false)]
        public JsonResult generatePDF()
        {
            ShipmentContaxt sc1 = new ShipmentContaxt();

            var docscontent = sc1.DocsPagges.ToList()
                .Where(m => m.Mid == Menuid)
                .OrderByDescending(p => p.Id)
                .Select(x => x.Content).FirstOrDefault();

            string CSS = @"body {font-size: 12px;} table {border-collapse:collapse; margin:8px;}.light-yellow {background-color:#ffff99;}td {border:1px solid #ccc;padding:4px;}";


             ConvertHtmlToPdf(docscontent, CSS);
            return Json(docscontent, JsonRequestBehavior.AllowGet);

            //SqlDataAdapter adp = new SqlDataAdapter("select content from docspages where mid=" + Menuid + "", con);
            //DataTable dt = new DataTable();
            //adp.Fill(dt);
            ////if (dt.Rows.Count > 0)
            ////{
            //    string content = dt.Rows[0][0].ToString();


            //    string CSS = @"body {font-size: 12px;} table {border-collapse:collapse; margin:8px;}.light-yellow {background-color:#ffff99;}td {border:1px solid #ccc;padding:4px;}";


            //    ConvertHtmlToPdf(content, CSS);


            //    return Json("gdfgdfg", JsonRequestBehavior.AllowGet);
            // return Json(new { Result = true }, JsonRequestBehavior.AllowGet);

            //return Json(content, JsonRequestBehavior.AllowGet);
            //}
            //return Json(new { Result = false }, JsonRequestBehavior.AllowGet);
        }



        public void ConvertHtmlToPdf(string xHtml, string css)
        {
            try
            {
                xHtml = xHtml.Replace("<br>", "<br/>");
                xHtml = xHtml.Replace("width: 50%;", "width: 200%;");
                xHtml = Regex.Replace(xHtml, "(?<image><img[^>]+)(?<=[^/])>", new MatchEvaluator(match => match.Groups["image"].Value + "  />"), RegexOptions.IgnoreCase | RegexOptions.Multiline);




                //System.Windows.Forms. SaveFileDialog saveFileDialog1 = new SaveFileDialog();

                //// SaveFileDialog saveFileDialog1 = new SaveFileDialog();
                //Microsoft.Win32.OpenFileDialog ofd = new OpenFileDialog();
                //ofd.Title = "Open a Text File";
                //ofd.Filter = "Text Files (*.txt) | *.txt | All Files(*.*) | *.*"; //Here you can filter which all files you wanted allow to open  
                //DialogResult dr = ofd.ShowDialog();
                //if (dr == DialogResult.OK)
                //{
                //    StreamReader sr = new StreamReader(ofd.FileName);
                //    txtEx.Text = sr.ReadToEnd();
                //    sr.Close();
                //}
                string fileName = Session["PDFtitle"].ToString();

                //string fileName = "samplefile.pdf";
                string UploadPath = "~/images/AttachFiles/";
               // trngattach.AttachmentPath = UploadPath + fileName;
                string origPath = Server.MapPath("~/images/AttachFiles/");
                var originalFilePath = Path.Combine(origPath, fileName);

                //File(filePath, contentType, "Report.pdf");

                // originalFilePath.SaveAs(originalFilePath);

                //Forms.SaveFileDialog dlg = new System.Windows.Forms.SaveFileDialog();
                //dlg.FileName = "DigiMoorX7_MSMPR_PDF_" + DateTime.Now.ToString("dd-MMM-yyyy_HH_mm_ss");

                //using (var stream = new FileStream(@"C:\DigiMoorDB_Backup\Attachmentss", FileMode.Create))

               // FileStream fs = System.IO.File.Open(originalFilePath,FileMode.Create,  FileAccess.Write, FileShare.ReadWrite);
                //Byte[] info = new System.Text.UTF8Encoding(true).GetBytes(sResponseFromServer);
                //fs.Write(info, 0, info.Length);
                //fs.Close();

                //using (var stream = fs)
                using (var stream = new FileStream(originalFilePath, FileMode.Create))
                {
                    using (var document = new iTextSharp.text.Document(iTextSharp.text.PageSize.A4, 10f, 10f, 140f, 10f))
                    {
                        HTMLWorker htmlparser = new HTMLWorker(document);
                        // string mypath = @"C:\DigiMoorDB_Backup\InspectionImages\water-drop-300KB-2.jpg";                       
                        var writer = PdfWriter.GetInstance(document, stream);
                        writer.PageEvent = new MooringOfficeweb.ITextEvents();
                        // document.AddHeader = new HeaderFooter(new Phrase("Header Text"), false);
                        document.NewPage();
                        document.Open();
                        var tagProcessorFactory = (DefaultTagProcessorFactory)Tags.GetHtmlTagProcessorFactory();
                        tagProcessorFactory.RemoveProcessor(HTML.Tag.IMG); // remove the default processor
                        tagProcessorFactory.AddProcessor(HTML.Tag.IMG, new CustomImageTagProcessor()); // use our new processor
                        var htmlPipelineContext = new HtmlPipelineContext(null);
                        htmlPipelineContext.SetTagFactory(tagProcessorFactory);
                        var pdfWriterPipeline = new PdfWriterPipeline(document, writer);
                        var htmlPipeline = new HtmlPipeline(htmlPipelineContext, pdfWriterPipeline);
                        // get an ICssResolver and add the custom CSS
                        var cssResolver = XMLWorkerHelper.GetInstance().GetDefaultCssResolver(true);
                        cssResolver.AddCss(css, "utf-8", true);
                        var cssResolverPipeline = new CssResolverPipeline(cssResolver, htmlPipeline);
                        xHtml = xHtml.Replace("\r", "\n").Replace("\n", "");// you html code (for example table from your page)
                        var memoryStream = new MemoryStream();
                        var worker = new XMLWorker(cssResolverPipeline, true);
                        var charset = Encoding.UTF8;
                        //var xmlParser = new XMLParser(true, worker, charset);
                        var xmlParser = new XMLParser(true, worker, charset);
                        writer.RgbTransparencyBlending = true;
                        PdfPTable table = new PdfPTable(3);
                        table.HeaderRows = 1;
                        using (var stringReader = new StringReader(xHtml))
                        {
                            try
                            {
                                //CreatePDF(xHtml);
                                htmlparser.Parse(stringReader);
                                xmlParser.Parse(stringReader);

                               
                                writer.CloseStream = false;
                                document.Close();
                                memoryStream.Position = 0;



                                


                                // MessageBox.Show("PDF generated successfully !", "Mooring Manual", MessageBoxButton.OK, MessageBoxImage.Information);
                            }
                            catch (Exception ex7)
                            {
                                using (var stringReader1 = new StringReader(xHtml))
                                {
                                    xmlParser.Parse(stringReader1);

                                    writer.CloseStream = false;
                                    document.Close();
                                    memoryStream.Position = 0;
                                }

                               // MessageBox.Show("PDF generated successfully !", "Mooring Manual", MessageBoxButton.OK, MessageBoxImage.Information);
                            }
                            //xmlParser.Parse(stringReader);
                        }
                    }
                }
                download();
            }
            catch (Exception ex)
            {
               // MessageBox.Show("HTML not in proper format !", "HTML ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
      private  void download()
        {
            string name = Session["PDFtitle"].ToString();

            Response.ContentType = "Application/pdf";
            Response.AddHeader("Content-Disposition", "attachment; filename="+ Session["PDFtitle"].ToString() + "");
            Response.TransmitFile(Server.MapPath("~/images/AttachFiles/"+ Session["PDFtitle"].ToString() + ""));
            Response.End();
        }
        public class CustomImageTagProcessor : iTextSharp.tool.xml.html.Image
        {
            public override IList<IElement> End(IWorkerContext ctx, Tag tag, IList<IElement> currentContent)
            {
                IDictionary<string, string> attributes = tag.Attributes;
                string src;
                if (!attributes.TryGetValue(HTML.Attribute.SRC, out src))
                    return new List<IElement>(1);

                if (string.IsNullOrEmpty(src))
                    return new List<IElement>(1);

                if (src.StartsWith("data:image/", StringComparison.InvariantCultureIgnoreCase))
                {
                    // data:[<MIME-type>][;charset=<encoding>][;base64],<data>
                    var base64Data = src.Substring(src.IndexOf(",") + 1);
                    var imagedata = Convert.FromBase64String(base64Data);
                    var image = iTextSharp.text.Image.GetInstance(imagedata);

                    var list = new List<IElement>();
                    var htmlPipelineContext = GetHtmlPipelineContext(ctx);
                    list.Add(GetCssAppliers().Apply(new Chunk((iTextSharp.text.Image)GetCssAppliers().Apply(image, tag, htmlPipelineContext), 0, 0, true), tag, htmlPipelineContext));
                    return list;
                }
                else
                {
                    return base.End(ctx, tag, currentContent);
                }
            }
        }

        //public void CreatePDFFromHTMLFile(string HtmlStream, string FileName)
        //{
        //    try
        //    {
        //        object TargetFile = FileName;
        //        string ModifiedFileName = string.Empty;
        //        string FinalFileName = string.Empty;


        //        GeneratePDF.HtmlToPdfBuilder builder = new GeneratePDF.HtmlToPdfBuilder(iTextSharp.text.PageSize.A4);
        //        GeneratePDF.HtmlPdfPage first = builder.AddPage();
        //        first.AppendHtml(HtmlStream);
        //        byte[] file = builder.RenderPdf();
        //        File.WriteAllBytes(TargetFile.ToString(), file);

        //        iTextSharp.text.pdf.PdfReader reader = new iTextSharp.text.pdf.PdfReader(TargetFile.ToString());
        //        ModifiedFileName = TargetFile.ToString();
        //        ModifiedFileName = ModifiedFileName.Insert(ModifiedFileName.Length - 4, "1");

        //        iTextSharp.text.pdf.PdfEncryptor.Encrypt(reader, new FileStream(ModifiedFileName, FileMode.Append), iTextSharp.text.pdf.PdfWriter.STRENGTH128BITS, "", "", iTextSharp.text.pdf.PdfWriter.AllowPrinting);
        //        reader.Close();
        //        if (File.Exists(TargetFile.ToString()))
        //            File.Delete(TargetFile.ToString());
        //        FinalFileName = ModifiedFileName.Remove(ModifiedFileName.Length - 5, 1);
        //        File.Copy(ModifiedFileName, FinalFileName);
        //        if (File.Exists(ModifiedFileName))
        //            File.Delete(ModifiedFileName);

        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}
        public byte[] GetPDF(string pHTML)
        {
            byte[] bPDF = null;

            








            MemoryStream ms = new MemoryStream();
            TextReader txtReader = new StringReader(pHTML);

            // 1: create object of a itextsharp document class  
            iTextSharp.text.Document doc = new iTextSharp.text.Document(PageSize.A4, 25, 25, 25, 25);

            // 2: we create a itextsharp pdfwriter that listens to the document and directs a XML-stream to a file  
            PdfWriter oPdfWriter = PdfWriter.GetInstance(doc, ms);

            // 3: we create a worker parse the document  
            HTMLWorker htmlWorker = new HTMLWorker(doc);

            // 4: we open document and start the worker on the document  
            doc.Open();
            htmlWorker.StartDocument();


            // 5: parse the html into the document  
            htmlWorker.Parse(txtReader);

            // 6: close the document and the worker  
            htmlWorker.EndDocument();
            htmlWorker.Close();
            doc.Close();

            bPDF = ms.ToArray();

            return bPDF;
        }
        //public void ConvertHtmlToPdf(string xHtml, string css)
        //{
        //    try
        //    {
        //        xHtml = xHtml.Replace("<br>", "<br/>");
        //        xHtml = xHtml.Replace("width: 50%;", "width: 200%;");
        //        xHtml = Regex.Replace(xHtml, "(?<image><img[^>]+)(?<=[^/])>", new MatchEvaluator(match => match.Groups["image"].Value + "  />"), RegexOptions.IgnoreCase | RegexOptions.Multiline);
        //        System.Windows.Forms.SaveFileDialog dlg = new System.Windows.Forms.SaveFileDialog();
        //        dlg.FileName = "DigiMoorX7_MSMPR_PDF_" + DateTime.Now.ToString("dd-MMM-yyyy_HH_mm_ss");
        //        dlg.DefaultExt = ".pdf";
        //        dlg.Filter = "PDF Files|*.pdf";
        //        if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
        //        {
        //            if (File.Exists(dlg.FileName))
        //            {
        //                File.Delete(dlg.FileName);
        //            }
        //        }
        //        using (var stream = new FileStream(dlg.FileName, FileMode.Create))
        //        {
        //            using (var document = new iTextSharp.text.Document(iTextSharp.text.PageSize.A4, 10f, 10f, 140f, 10f))
        //            {
        //                HTMLWorker htmlparser = new HTMLWorker(document);
        //                // string mypath = @"C:\DigiMoorDB_Backup\InspectionImages\water-drop-300KB-2.jpg";                       
        //                var writer = PdfWriter.GetInstance(document, stream);
        //                writer.PageEvent = new WorkShipVersionII.ITextEvents();
        //                // document.AddHeader = new HeaderFooter(new Phrase("Header Text"), false);
        //                document.NewPage();
        //                document.Open();
        //                var tagProcessorFactory = (DefaultTagProcessorFactory)Tags.GetHtmlTagProcessorFactory();
        //                tagProcessorFactory.RemoveProcessor(HTML.Tag.IMG); // remove the default processor
        //                tagProcessorFactory.AddProcessor(HTML.Tag.IMG, new CustomImageTagProcessor()); // use our new processor
        //                var htmlPipelineContext = new HtmlPipelineContext(null);
        //                htmlPipelineContext.SetTagFactory(tagProcessorFactory);
        //                var pdfWriterPipeline = new PdfWriterPipeline(document, writer);
        //                var htmlPipeline = new HtmlPipeline(htmlPipelineContext, pdfWriterPipeline);
        //                // get an ICssResolver and add the custom CSS
        //                var cssResolver = XMLWorkerHelper.GetInstance().GetDefaultCssResolver(true);
        //                cssResolver.AddCss(css, "utf-8", true);
        //                var cssResolverPipeline = new CssResolverPipeline(cssResolver, htmlPipeline);
        //                xHtml = xHtml.Replace("\r", "\n").Replace("\n", "");// you html code (for example table from your page)
        //                var memoryStream = new MemoryStream();
        //                var worker = new XMLWorker(cssResolverPipeline, true);
        //                var charset = Encoding.UTF8;
        //                //var xmlParser = new XMLParser(true, worker, charset);
        //                var xmlParser = new XMLParser(true, worker, charset);
        //                writer.RgbTransparencyBlending = true;
        //                PdfPTable table = new PdfPTable(3);
        //                table.HeaderRows = 1;
        //                using (var stringReader = new StringReader(xHtml))
        //                {
        //                    try
        //                    {
        //                        //CreatePDF(xHtml);
        //                        htmlparser.Parse(stringReader);
        //                        xmlParser.Parse(stringReader);

        //                        writer.CloseStream = false;
        //                        document.Close();
        //                        memoryStream.Position = 0;

        //                        //MessageBox.Show("PDF generated successfully !", "Mooring Manual", MessageBoxButton.OK, MessageBoxImage.Information);
        //                    }
        //                    catch (Exception ex7)
        //                    {
        //                        using (var stringReader1 = new StringReader(xHtml))
        //                        {
        //                            xmlParser.Parse(stringReader1);

        //                            writer.CloseStream = false;
        //                            document.Close();
        //                            memoryStream.Position = 0;
        //                        }

        //                        //MessageBox.Show("PDF generated successfully !", "Mooring Manual", MessageBoxButton.OK, MessageBoxImage.Information);
        //                    }
        //                    //xmlParser.Parse(stringReader);
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //       // MessageBox.Show("HTML not in proper format !", "HTML ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
        //    }
        //}
        public JsonResult GetSearchContent(int Mid,string searchT)
        {
            ShipmentContaxt sc1 = new ShipmentContaxt();
            //List<SelectListItem> docs = new List<SelectListItem>();
            var docscontent = sc1.DocsPagges.ToList()
                .Where(m => m.Mid == Mid )
                .OrderByDescending(p => p.Id)
                .Select(x => x.Content).FirstOrDefault();



            StringBuilder sb = new StringBuilder();

            //string ss="<head>< meta http - equiv = 'Content-Type content' = text / html; charset = UTF - 8 >
            sb.Append("<head><meta http-equiv='Content-Type' content='text/html;charset=UTF-8'>");
            sb.Append(docscontent);
            sb.Append("</head>");

            var fragment = Regex.Replace(sb.ToString(), "<!--.*?-->", String.Empty, RegexOptions.Multiline);

            string abc = fragment;



            string searchString = searchT;
            string content = abc;
            string replacePattern = "$1<span style=\"background-color:yellow\">$2</span>$3";
            string searchPattern = String.Format("(>[^<>]*?)({0})([^<>]*?<)", searchString.Trim());
            content = Regex.Replace(content, searchPattern, replacePattern, RegexOptions.IgnoreCase);




            return Json(content, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetGdata()
        {
            ShipmentContaxt sc1 = new ShipmentContaxt();
            
            var docscontent = sc1.DocsPagges.ToList()
                .Where(m => m.Mid == Menuid )
                .OrderByDescending(p => p.Id)
                .Select(x => x.Content).FirstOrDefault();


            return Json(docscontent, JsonRequestBehavior.AllowGet);
        }


        public JsonResult Nxtbtn(int? Id)
        {
            int nxtid = 0;
            try
            {
                if(Id==0)
                {
                    Id = Menuid;
                    
                }
                
                //SqlDataAdapter adp = new SqlDataAdapter("SELECT LAG(Mid) OVER ( ORDER BY Mid ) AS PreviousID,Mid,LEAD(Mid) OVER ( ORDER BY Mid ) AS NextID FROM tblMenuName where type !=1", sc.con);
                SqlDataAdapter adp = new SqlDataAdapter("GetNextPrevious", con);
                adp.SelectCommand.CommandType = CommandType.StoredProcedure;
                DataTable dt = new DataTable();
                adp.Fill(dt);
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    int mid = Convert.ToInt32(dt.Rows[i]["Mid"]);

                    if (mid == Id)
                    {
                        //TempData["title"] = dt.Rows[i]["MenuName"].ToString();
                        int nextid = Convert.ToInt32(dt.Rows[i]["NextID"]);
                       nxtid = nextid;


                        SqlDataAdapter pp = new SqlDataAdapter("select menuname from tblmenuname where mid=" + nextid + "", con);
                        DataTable ddt = new DataTable();
                        pp.Fill(ddt);
                        if (ddt.Rows.Count > 0)
                        {
                            string mt = ddt.Rows[0]["MenuName"].ToString();
                            TempData["title"] = mt;
                            Session["PDFtitle"] = mt + ".pdf";
                        }

                        

                        break;
                    }
                }
            }
            catch (Exception ex) { }
            return Json(new { Result = true, Id = nxtid }, JsonRequestBehavior.AllowGet);
           // return RedirectToAction("Index", new {id= nxtid });
        }
        public JsonResult Prvbtn(int? Id)
        {
            int prid = 0;
            try
            {
                if (Id == 0)
                {
                    Id = Menuid;
                   // ViewBag.qrstrID = Menuid;
                }

                //SqlDataAdapter adp = new SqlDataAdapter("SELECT LAG(Mid) OVER ( ORDER BY Mid ) AS PreviousID,Mid,LEAD(Mid) OVER ( ORDER BY Mid ) AS NextID FROM tblMenuName where type !=1", sc.con);
                SqlDataAdapter adp = new SqlDataAdapter("GetNextPrevious", con);
                adp.SelectCommand.CommandType = CommandType.StoredProcedure;
                DataTable dt = new DataTable();
                adp.Fill(dt);
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    int mid = Convert.ToInt32(dt.Rows[i]["Mid"]);

                    if (mid == Id)
                    {
                        //TempData["title"] = dt.Rows[i]["MenuName"].ToString();
                        int prvid = Convert.ToInt32(dt.Rows[i]["PreviousID"]);
                        prid = prvid;


                        SqlDataAdapter pp = new SqlDataAdapter("select menuname from tblmenuname where mid=" + prvid + "", con);
                        DataTable ddt = new DataTable();
                        pp.Fill(ddt);
                        if (ddt.Rows.Count > 0)
                        {
                            string mt = ddt.Rows[0]["MenuName"].ToString();
                            TempData["title"] = mt;
                            Session["PDFtitle"] = mt + ".pdf";
                        }



                        break;
                    }
                }
            }
            catch (Exception ex) { }
            return Json(new { Result = true, Id = prid }, JsonRequestBehavior.AllowGet);
            // return RedirectToAction("Index", new {id= nxtid });
        }

        [HttpPost]
        [PreventSpam("addattachmentMSMP", 3, 1)]
        public ActionResult addattachment(DetailsViewModel model, FormCollection collection)
        {
            string IDs = collection["ShipIds"];
            //string[] selectedShips = IDs.Split(',');    
            //string[] selectedShips = VesselID.ToString();

            using (ShipmentContaxt context = new ShipmentContaxt())
            {
                //foreach (string shipID in selectedShips)
                //{
                    for (int count = 0; count < Request.Files.Count; count++)
                    {
                        var uploadedFile = Request.Files[0];
                        string attachmentName = DateTime.Now.Ticks + Path.GetExtension(uploadedFile.FileName);


                        string[] str1;
                        str1 = new string[13] { "jpg", "png", "pps", "ppt", "pptx", "xls", "xlsm", "xlsx", "doc", "docx", "pdf", "rtf", "txt" };
                        string FileExtension = uploadedFile.FileName.Substring(uploadedFile.FileName.LastIndexOf('.') + 1).ToLower();

                        if (str1.Contains(FileExtension))
                        {
                            int compare = uploadedFile.ContentLength;

                            if (compare > 5000000)
                            {
                                TempData["fileformat"] = "Attachment size not to be greater than 5MB";
                                return RedirectToAction("index", "MSMP", new { area = "MooringManual", @id = Menuid });
                            }

                            if (uploadedFile != null && uploadedFile.ContentLength > 0)
                            {
                                string filePath = Server.MapPath(string.Format("~/images/AttachFiles/filepath/{0}/", VesselID));

                                if (!Directory.Exists(filePath))
                                    Directory.CreateDirectory(filePath);

                                var path = Path.Combine(filePath, attachmentName);
                                uploadedFile.SaveAs(path);
                            }
                        }
                        else
                        {
                            TempData["fileformat"] = "Invalid File Format";

                            return RedirectToAction("index", "MSMP", new { area = "MooringManual", @id = Menuid });

                        }

                        ShipSpecificAttachment attachment = new ShipSpecificAttachment();

                        attachment.Attachment = attachmentName;
                        attachment.AttachmentName = model.shipmodel.AttachmentName;
                        attachment.MId = Menuid ?? default(int);
                        attachment.ShipId = VesselID.ToString();
                        attachment.CreatedDate = attachment.ModifiedDate = DateTime.Today;
                        attachment.CreateBy = attachment.ModifiedBy = User.Identity.GetUserName();

                        context.ShipSpecificAttachments.Add(attachment);
                    }
               // }

                context.SaveChanges();
            }

            return RedirectToAction("index", "MSMP", new { area = "MooringManual", @id = Menuid });
        }





        // test 

        public void DownloadPDF()
        {
            string HTMLContent = "Hello <b>World</b>";
            Response.Clear();
            //Response.ContentType = "application/pdf";
            //Response.AddHeader("content-disposition", "attachment;filename=" + "PDFfile.pdf");
            //Response.Cache.SetCacheability(HttpCacheability.NoCache);
            //Response.BinaryWrite(GetPDF1(HTMLContent));
            //Response.End();



            Response.ContentType = "application/pdf";
            Response.AddHeader("content-disposition", "attachment;filename=Panel.pdf");
            Response.Cache.SetCacheability(HttpCacheability.NoCache);

            //StringReader sr = new StringReader(HTMLContent);

            //StringWriter sw = new StringWriter(sr);
            //HtmlTextWriter hw = new HtmlTextWriter(sw);
            //pnlPerson.RenderControl(hw);
            StringReader sr = new StringReader(HTMLContent.ToString());
           iTextSharp.text.Document pdfDoc = new iTextSharp.text.Document(PageSize.A4, 10f, 10f, 100f, 0f);
            HTMLWorker htmlparser = new HTMLWorker(pdfDoc);
            PdfWriter.GetInstance(pdfDoc, Response.OutputStream);
            pdfDoc.Open();
            htmlparser.Parse(sr);
            pdfDoc.Close();
            Response.Write(pdfDoc);
            Response.End();
        }

        public byte[] GetPDF1(string pHTML)
        {
            byte[] bPDF = null;

            MemoryStream ms = new MemoryStream();
            TextReader txtReader = new StringReader(pHTML);

            // 1: create object of a itextsharp document class  
          iTextSharp.text.Document doc = new iTextSharp.text.Document(PageSize.A4, 25, 25, 25, 25);

            // 2: we create a itextsharp pdfwriter that listens to the document and directs a XML-stream to a file  
            PdfWriter oPdfWriter = PdfWriter.GetInstance(doc, ms);

            // 3: we create a worker parse the document  
            HTMLWorker htmlWorker = new HTMLWorker(doc);

            // 4: we open document and start the worker on the document  
            doc.Open();
            htmlWorker.StartDocument();


            // 5: parse the html into the document  
            htmlWorker.Parse(txtReader);

            // 6: close the document and the worker  
            htmlWorker.EndDocument();
            htmlWorker.Close();
            doc.Close();

            bPDF = ms.ToArray();

            return bPDF;
        }


    }
}

public class MenuName
{
    public int deleted { get; set; }
  
    public int slug { get; set; }
    public string href { get; set; }
    public int type { get; set; }
    public string prefix { get; set; }
    public string text { get; set; }
    public int id { get; set; }

    public List<MenuName> children { get; set; }


}

public class tblMenu
{
    public int Id { get; set; }
    public int Mid { get; set; }
    public string MenuName { get; set; }
    public int Type { get; set; }
   

}