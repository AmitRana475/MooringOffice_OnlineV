using ClosedXML.Excel;
using CrewReportLayer;
using Elmah;
using MenuLayer;
using Shipment49Web.Common;
using Shipment49Web.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Linq.Dynamic;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace Shipment49Web.Areas.ImportExport.Controllers
{
    [Authorize]
    [UserAuthenticationFilter]
    [ErrorClass]
    public class ImportExportController : BaseController
    {
        private readonly ShipmentContaxt sc = new ShipmentContaxt();
        //private readonly IMenuRepository sc1;

        private int maxHeight
        {
            get
            {
                int height = 150;
                if (ConfigurationManager.AppSettings["MaxHeight"] == null)
                    height = Convert.ToInt32(ConfigurationManager.AppSettings["MaxHeight"]);

                return height;
            }
        }

        private int maxWidth
        {
            get
            {
                int width = 150;
                if (ConfigurationManager.AppSettings["MaxHeight"] == null)
                    width = Convert.ToInt32(ConfigurationManager.AppSettings["MaxHeight"]);

                return width;
            }
        }

        List<string> hrefs = new List<string>
                        {
                            "http://kline.mooringplan.com/",
                            "https://kline.mooringplan.com/",
                            "http://localhost:14642/"
                        };

        public ImportExportController()
        {
            //sc1 = repo;
            //if (UserRole.username == null)
            //{
            //    UserRole.username = string.Join("", Roles.GetRolesForUser());
            //}

            //ViewBag.GetMenu = UserRole.username == "Admin" ? sc1.Menus.ToList() : sc1.Menus.Where(x => x.Role == "User").ToList();
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

        public ActionResult Index(string searchTerm, DateTime? ServiceFrom, DateTime? ServiceTo)
        {
            //bmsuploaddat.bmsname = "";
            ViewBag.vname = searchTerm;
            ViewBag.datefrom = ServiceFrom == null ? DateTime.Parse(DateTime.Now.AddYears(-1).ToString()).ToString("dd-MMM-yyyy", CultureInfo.InvariantCulture) : DateTime.Parse(ServiceFrom.ToString()).ToString("dd-MMM-yyyy", CultureInfo.InvariantCulture);
            ViewBag.dateto = ServiceTo == null ? DateTime.Parse(DateTime.Now.ToString()).ToString("dd-MMM-yyyy", CultureInfo.InvariantCulture) : DateTime.Parse(ServiceTo.ToString()).ToString("dd-MMM-yyyy", CultureInfo.InvariantCulture);
            ViewData["VesselList"] = base.PermittedVessels;

            return View();
        }

        //private void ExportdataInExcel(string searchTerm)
        //{
        //    DataSet dataSet = GetRecordsFromDatabase( );

        //    StringBuilder sb = new StringBuilder();

        //    sb.Append("<table>");

        //    //LINQ to get Column names
        //    var columnName = dataSet.Tables[0].Columns.Cast<DataColumn>()
        //                         .Select(x => x.ColumnName)
        //                         .ToArray();
        //    sb.Append("<tr>");
        //    //Looping through the column names
        //    foreach (var col in columnName)
        //        sb.Append("<td>" + col + "</td>");
        //    sb.Append("</tr>");

        //    //Looping through the records
        //    foreach (DataRow dr in dataSet.Tables[0].Rows)
        //    {
        //        sb.Append("<tr>");
        //        foreach (DataColumn dc in dataSet.Tables[0].Columns)
        //        {
        //            sb.Append("<td>" + dr[dc] + "</td>");
        //        }
        //        sb.Append("</tr>");
        //    }

        //    sb.Append("</table>");




        //    //Writing StringBuilder content to an excel file.
        //    Response.Clear();
        //    Response.ClearContent();
        //    Response.ClearHeaders();

        //    DateTime today = DateTime.Today;
        //    string vsname = searchTerm.Replace(" ", "");
        //    string HeaderName = "Work-Ship_Export" + "_" + vsname + "_" + today.ToString("dd-MMM-yyyy");

        //    Response.Charset = "";
        //    Response.Buffer = true;
        //    Response.BufferOutput = true;
        //    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        //    Response.AddHeader("content-disposition", "attachment;filename= " + HeaderName + ".xlsx");

        //    //Response.ContentType = "application/vnd.ms-excel";
        //    //Response.AddHeader("content-disposition", "attachment;filename= " + HeaderName + ".xls");
        //    Response.Write(sb.ToString());
        //    Response.Flush();
        //    Response.Close();
        //}

        //[HttpGet]
        //public ActionResult ExportData()
        //{
        //    //Fill dataset with records
        //    DataSet dataSet = GetRecordsFromDatabase();

        //    StringBuilder sb = new StringBuilder();

        //    sb.Append("<table>");

        //    //LINQ to get Column names
        //    var columnName = dataSet.Tables[0].Columns.Cast<DataColumn>()
        //                         .Select(x => x.ColumnName)
        //                         .ToArray();
        //    sb.Append("<tr>");
        //    //Looping through the column names
        //    foreach (var col in columnName)
        //        sb.Append("<td>" + col + "</td>");
        //    sb.Append("</tr>");

        //    //Looping through the records
        //    foreach (DataRow dr in dataSet.Tables[0].Rows)
        //    {
        //        sb.Append("<tr>");
        //        foreach (DataColumn dc in dataSet.Tables[0].Columns)
        //        {
        //            sb.Append("<td>" + dr[dc] + "</td>");
        //        }
        //        sb.Append("</tr>");
        //    }

        //    sb.Append("</table>");

        //    //Writing StringBuilder content to an excel file.
        //    Response.Clear();
        //    Response.ClearContent();
        //    Response.ClearHeaders();
        //    Response.Charset = "";
        //    Response.Buffer = true;
        //    Response.ContentType = "application/vnd.ms-excel";
        //    Response.AddHeader("content-disposition", "attachment;filename=UserReport.xls");
        //    Response.Write(sb.ToString());
        //    Response.Flush();
        //    Response.Close();
        //    return View();
        //}
        DataSet GetRecordsFromDatabase(string tablename)
        {
            DataSet dataSet = new DataSet();

            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = ConfigurationManager.ConnectionStrings["ShipmentContaxt"].ConnectionString;
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "Select * FROM " + tablename + "";
            cmd.Connection = conn;

            SqlDataAdapter sqlDataAdapter = new SqlDataAdapter();
            sqlDataAdapter.SelectCommand = cmd;
            sqlDataAdapter.Fill(dataSet);

            return dataSet;
        }

        [HttpPost]
        public async Task<ActionResult> Index(string submit, string download, HttpPostedFileBase photo, string searchTerm, DateTime? ServiceFrom, DateTime? ServiceTo, bmsuploaddat1 bmsuploaddats, FormCollection collection)
        {
            string query = string.Empty;
            int shipID = 0;
            DateTime dateImportFrom = DateTime.Now;
            DateTime dateImportUpto = DateTime.Now;
            string vesselName = string.Empty;
            ViewBag.vname = searchTerm;


            ViewBag.datefrom = ServiceFrom == null ? DateTime.Parse(DateTime.Now.ToString()).ToString("MM/dd/yyyy", CultureInfo.InvariantCulture) : DateTime.Parse(ServiceFrom.ToString()).ToString("MM/dd/yyyy", CultureInfo.InvariantCulture);
            ViewBag.dateto = ServiceTo == null ? DateTime.Parse(DateTime.Now.ToString()).ToString("MM/dd/yyyy", CultureInfo.InvariantCulture) : DateTime.Parse(ServiceTo.ToString()).ToString("MM/dd/yyyy", CultureInfo.InvariantCulture);

            ViewData["VesselList"] = base.PermittedVessels;

            if (submit == "Import Text")
            {
                OleDbConnection excelConnection = new OleDbConnection();

                try
                {
                    if (photo != null)
                    {
                        int numberfiles = 0;

                        if (Request.Files["photo"].ContentLength > 0)
                        {
                            string fileExtension = Path.GetExtension(Request.Files["photo"].FileName);

                            string filenames = Path.GetFileName(Request.Files["photo"].FileName).Replace(".xls", "").Replace(".xlsx", "");

                            if (fileExtension == ".xls" || fileExtension == ".xlsx")
                            {
                                string location = Server.MapPath("~/Content/") + string.Format("{0}{1}", DateTime.Now.Ticks, fileExtension);

                                if (System.IO.File.Exists(location))
                                    System.IO.File.Delete(location);

                                Request.Files["photo"].SaveAs(location);

                                string excelConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + location + ";Extended Properties=Excel 12.0;Persist Security Info=False";
                                excelConnection = new OleDbConnection(excelConnectionString);
                                excelConnection.Open();
                                DataTable dt = new DataTable();

                                dt = excelConnection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);

                                if (dt == null)
                                    return null;

                                List<string> excelSheets = new List<string>();

                                foreach (DataRow row in dt.Rows)
                                {
                                    string sheetName = Convert.ToString(row["TABLE_NAME"]);
                                    sheetName = sheetName.Trim('$');

                                    if (sheetName.Equals("Vessel", StringComparison.OrdinalIgnoreCase))
                                    {
                                        query = string.Format("Select * from [{0}$]", sheetName);

                                        OleDbDataAdapter olda = new OleDbDataAdapter(query, excelConnection);
                                        DataTable dataTable = new DataTable
                                        {
                                            TableName = sheetName
                                        };

                                        olda.Fill(dataTable);

                                        if (dataTable.Rows.Count > 0)
                                        {
                                            shipID = Convert.ToInt32(dataTable.Rows[0]["vessel_ID"]);
                                            vesselName = Convert.ToString(dataTable.Rows[0]["VesselName"]);
                                            dateImportFrom = Convert.ToDateTime(dataTable.Rows[0]["DateFrom"]);
                                            dateImportUpto = Convert.ToDateTime(dataTable.Rows[0]["DateTo"]);
                                        }
                                    }
                                    else
                                    {
                                        excelSheets.Add(sheetName);
                                    }
                                }

                                string importedTables = string.Empty;

                                var vesselDetails = sc.Vessels.FirstOrDefault(p => p.ImoNo == shipID);

                                if (vesselDetails != null)
                                {
                                    excelConnection = new OleDbConnection(excelConnectionString);

                                    foreach (string importedSheet in excelSheets)
                                    {
                                        query = string.Format("Select * from [{0}$]", importedSheet);

                                        OleDbDataAdapter olda = new OleDbDataAdapter(query, excelConnection);
                                        DataTable dataTable = new DataTable
                                        {
                                            TableName = importedSheet
                                        };

                                        olda.Fill(dataTable);

                                        string errorMessage = string.Empty;
                                        ImportData(dataTable, importedSheet.Trim('$'), shipID, out errorMessage);

                                        if (!string.IsNullOrEmpty(errorMessage))
                                            importedTables += errorMessage;

                                        numberfiles++;
                                    }

                                    excelConnection.Close();
                                    excelConnection.Dispose();

                                    // Import Log Generation

                                    Reports.ImportLog importLog = new Reports.ImportLog
                                    {
                                        Filenames = Request.Files["photo"].FileName,
                                        ImportedBy = LoggedInUserFullName, // UserRole.username,
                                        DateImported = DateTime.Now,
                                        DateImportFrom = dateImportFrom,
                                        DateImportTo = dateImportUpto,
                                        VesselName = vesselName
                                    };

                                    Reports.MorringOfficeEntities morringOfficeContext = new Reports.MorringOfficeEntities();
                                    morringOfficeContext.ImportLogs.Add(importLog);
                                    morringOfficeContext.SaveChanges();

                                    if (numberfiles == 0)
                                        bmsuploaddats.bmsname = "Vessel not found anywhere";
                                    else
                                    {
                                        if (importedTables.Length > 0)
                                        {
                                            bmsuploaddats.bmsname = "Data imported successfully except the tables namely - " + importedTables.Substring(0, importedTables.Length - 2);
                                        }
                                        else
                                            bmsuploaddats.bmsname = "Data Imported Successfully";
                                    }
                                }
                                else
                                {
                                    bmsuploaddats.bmsname = "Vessel not found anywhere.";
                                }
                            }
                            else
                            {
                                bmsuploaddats.bmsname = "No file(s) selected";
                            }
                        }
                    }
                    else
                    {
                        bmsuploaddats.bmsname = "No file(s) selected";
                    }
                }
                catch (Exception ex)
                {
                    bmsuploaddats.bmsname = ex.Message;
                    bmsuploaddats.bmsname = "Vessel not found anywhere";
                    ErrorSignal.FromCurrentContext().Raise(ex);
                    excelConnection.Close();
                    excelConnection.Dispose();
                }

                //var usernamess = string.Join("", Roles.GetRolesForUser());
            }
            else if (submit == "Import Attachments")
            {
                if (Request.Files["photo"].ContentLength > 0)
                {
                    for (int i = 0; i < Request.Files.Count; i++)
                    {
                        if (!string.IsNullOrEmpty(Request.Files[i].FileName))
                        {
                            try
                            {
                                //string location = Server.MapPath("~/images/inspectionimages/" + Request.Files[i].FileName);
                                //Request.Files[i].SaveAs(location);
                                GenerateThumbnail(Request.Files[i]);
                            }
                            catch (Exception ex)
                            {
                                bmsuploaddats.bmsname = ex.Message;
                                bmsuploaddats.bmsname = "Vessel not found anywhere";
                                ErrorSignal.FromCurrentContext().Raise(ex);
                            }
                            finally
                            {
                                bmsuploaddats.bmsname = "Attachments Imported Successfully";
                            }
                        }
                    }
                }
                else
                    bmsuploaddats.bmsname = "No file(s) selected";
            }
            else
            {
                string fileLocation = string.Empty;

                if (string.IsNullOrEmpty(searchTerm))
                {
                    bmsuploaddats.bmsname = "Please select vessel name first";
                    return View(bmsuploaddats);
                }

                string shipIMO = searchTerm;


                int VesselID = Convert.ToInt32(searchTerm);

                var vesselData = sc.Vessels.Where(p => p.ImoNo == VesselID).Select(x => new { x.Id, x.VesselName, x.ImoNo, x.Flag, x.FleetName, x.FleetType, x.MinimumRopes, x.MinimumRopeTails }).ToList();

                if (vesselData.ToList().Count > 0)
                {
                    shipIMO = vesselData.First().ImoNo.ToString();
                    VesselID = vesselData.First().ImoNo;
                }

                if (submit == "Export Text")
                {
                    if (!string.IsNullOrEmpty(searchTerm))
                    {

                        DataTable docsPages = new DataTable();
                        DataTable shipSpecificContent = new DataTable();
                        DataTable revisionTable = new DataTable();
                        DataTable shipSpecificAttachments = new DataTable();
                        DataTable looseEquipInspectionSettings = new DataTable();
                        DataTable ropeInspectionSettings = new DataTable();
                        DataTable ropeTailInspectionSettings = new DataTable();
                        DataTable notificationCommentSettings = new DataTable();
                        DataTable WinchRotationSettings = new DataTable();
                        //stored result into datatable  
                        DataTable vesselDetails = LINQResultToDataTable(vesselData);
                        vesselDetails.TableName = "VesselDetails";

                        if (collection["revisionTable"] != null)
                        {
                            var revisionData = sc.Revisions.Where(p => p.ReviseDate >= ServiceFrom && p.ReviseDate <= ServiceTo).ToList();

                            List<RevisionExport> revisionExports = new List<RevisionExport>();
                            foreach (var x in revisionData)
                            {
                                RevisionExport revisionExport = new RevisionExport();

                                revisionExport.ID = x.Id;
                                revisionExport.MID = x.MId;
                                revisionExport.CreatedBy = x.CreateBy;
                                revisionExport.ApproveBy = x.ApprovedBy;
                                revisionExport.Content = string.Empty;
                                revisionExport.ContentPath = string.Empty;
                                revisionExport.RevisionText = string.Empty;
                                revisionExport.RevisionType = x.RevisionType;
                                revisionExport.RPrefix = x.RPrefix;
                                revisionExport.Status = x.Status;

                                if (x.RNumber != null)
                                    revisionExport.RNumber = Convert.ToInt32(x.RNumber);

                                if (x.ReviseDate != null)
                                    revisionExport.ReviseDate = Convert.ToDateTime(x.ReviseDate);

                                if (x.ApproveDate != null)
                                    revisionExport.ApproveDate = Convert.ToDateTime(x.ApproveDate);

                                revisionExports.Add(revisionExport);
                            }

                            //stored result into datatable  
                            revisionTable = LINQResultToDataTable(revisionExports);
                            revisionTable.TableName = "RevisionData";
                        }

                        if (collection["docsPages"] != null)
                        {
                            var docsPageData = sc.DocsPagges.Where(p => p.CreatedDate >= ServiceFrom && p.CreatedDate <= ServiceTo).ToList();

                            List<DocPagesExport> docPagesExports = new List<DocPagesExport>();
                            foreach (var x in docsPageData)
                            {
                                docPagesExports.Add(new DocPagesExport
                                {
                                    ID = x.Id,
                                    Content = x.Content,
                                    ShipID = x.ShipId,
                                    MID = x.Mid,
                                    CreatedBy = x.CreateBy,
                                    CreatedDate = x.CreatedDate == null ? DateTime.Today : Convert.ToDateTime(x.CreatedDate),
                                    ModifiedBy = x.ModifiedBy,
                                    ModifiedDate = x.ModifiedDate == null ? DateTime.Today : Convert.ToDateTime(x.ModifiedDate)
                                });
                            }

                            //stored result into datatable  
                            docsPages = LINQResultToDataTable(docPagesExports);
                            docsPages.TableName = "DocsPages";
                        }

                        if (collection["shipSpecificContent"] != null)
                        {
                            var shipSpecificContentData = sc.ShipSpecificContents.Where(p => p.ShipId == shipIMO && p.CreatedDate >= ServiceFrom && p.CreatedDate <= ServiceTo);

                            var results = from p in shipSpecificContentData
                                          group p by p.MId into g
                                          select new
                                          {
                                              mid = g.Key,
                                              data = g.ToList().OrderByDescending(u => u.Id).FirstOrDefault()
                                          };

                            List<ShipSpecificContentExport> shipSpecificContentExports = new List<ShipSpecificContentExport>();
                            //foreach (var x in shipSpecificContentData)
                            foreach (var x in results)
                            {
                                shipSpecificContentExports.Add(new ShipSpecificContentExport()
                                {
                                    Id = x.data.Id,
                                    MId = x.data.MId,
                                    ShipId = x.data.ShipId,
                                    Content = x.data.Content,
                                    Created_Date = x.data.CreatedDate
                                });
                            }

                            //stored result into datatable  
                            shipSpecificContent = LINQResultToDataTable(shipSpecificContentExports);
                            shipSpecificContent.TableName = "ShipSpecificContent";
                        }

                        if (collection["shipSpecificAttachments"] != null)
                        {
                            var shipSpecificAttachmentData = sc.ShipSpecificAttachments.Where(p => p.ShipId == shipIMO && p.CreatedDate >= ServiceFrom && p.CreatedDate <= ServiceTo).ToList();

                            List<ShipSpecificAttachmentExport> shipSpecificAttachmentExports = new List<ShipSpecificAttachmentExport>();
                            foreach (var x in shipSpecificAttachmentData)
                            {
                                shipSpecificAttachmentExports.Add(new ShipSpecificAttachmentExport
                                {
                                    ID = x.Id,
                                    Attachment = x.Attachment,
                                    AttachmentName = x.AttachmentName,
                                    MID = x.MId,
                                    ShipID = x.ShipId,
                                    CreatedBy = x.CreateBy,
                                    ModifiedBy = x.ModifiedBy,
                                    CreatedDate = x.CreatedDate == null ? DateTime.Today : Convert.ToDateTime(x.CreatedDate),
                                    ModifiedDate = x.ModifiedDate == null ? DateTime.Today : Convert.ToDateTime(x.ModifiedDate)
                                });
                            }

                            //stored result into datatable  
                            shipSpecificAttachments = LINQResultToDataTable(shipSpecificAttachmentExports);
                            shipSpecificAttachments.TableName = "ShipSpecificAttachments";
                        }

                        if (collection["looseEquipInspectionSettings"] != null)
                        {
                            var looseEquipInspectionData = sc.LooseEquipInspectionSettings.ToList();

                            List<LooseEquipmentSettingsExport> looseEquipSettingExports = new List<LooseEquipmentSettingsExport>();
                            foreach (var x in looseEquipInspectionData)
                            {
                                looseEquipSettingExports.Add(new LooseEquipmentSettingsExport
                                {
                                    ID = x.Id,
                                    EquipmentType = (int)x.EquipmentType,
                                    InspectionFrequency = (int)x.InspectionFrequency,
                                    MaximumMonthsAllowed = x.MaximumMonthsAllowed == null ? 0 : Convert.ToInt32(x.MaximumMonthsAllowed),
                                    MaximumRunningHours = x.MaximumRunningHours == null ? 0 : Convert.ToInt32(x.MaximumRunningHours)
                                });
                            }

                            //stored result into datatable  
                            looseEquipInspectionSettings = LINQResultToDataTable(looseEquipSettingExports);
                            looseEquipInspectionSettings.TableName = "LooseEquipInspectionSettings";
                        }

                        if (collection["ropeInspectionSettings"] != null)
                        {
                            var ropeInspectionSettingsData = sc.RopeInspectionSettings.ToList();

                            List<RopeInspectionSettingsExport> ropeInspSettingsExport = new List<RopeInspectionSettingsExport>();
                            foreach (var x in ropeInspectionSettingsData)
                            {
                                ropeInspSettingsExport.Add(new RopeInspectionSettingsExport
                                {
                                    ID = x.Id,
                                    MooringRopeType = Convert.ToInt32(x.MooringRopeType),
                                    ManufacturerType = x.ManufacturerType,
                                    MaximumMonthsAllowed = x.MaximumMonthsAllowed == null ? 0 : Convert.ToInt32(x.MaximumMonthsAllowed),
                                    MaximumRunningHours = x.MaximumRunningHours == null ? 0 : Convert.ToInt32(x.MaximumRunningHours),
                                    EndToEndMonth = x.EndToEndMonth == null ? 0 : Convert.ToInt32(x.EndToEndMonth),
                                    RotationOnWinches = x.RotationOnWinches == null ? 0 : Convert.ToInt32(x.RotationOnWinches),
                                    Rating1 = x.Rating1,
                                    Rating2 = x.Rating2,
                                    Rating3 = x.Rating3,
                                    Rating4 = x.Rating4,
                                    Rating5 = x.Rating5,
                                    Rating6 = x.Rating6,
                                    Rating7 = x.Rating7
                                });
                            }

                            //stored result into datatable  
                            ropeInspectionSettings = LINQResultToDataTable(ropeInspSettingsExport);
                            ropeInspectionSettings.TableName = "RopeInspectionSettingsData";
                        }

                        if (collection["ropeTailInspectionSettings"] != null)
                        {
                            var ropeTailInspectionSettingsData = sc.RopeTailInspectionSettings.ToList();

                            List<RopeTailInspectionSettingsExport> ropeTailInspSettingsExport = new List<RopeTailInspectionSettingsExport>();
                            foreach (var x in ropeTailInspectionSettingsData)
                            {
                                ropeTailInspSettingsExport.Add(new RopeTailInspectionSettingsExport
                                {
                                    ID = x.Id,
                                    MooringRopeType = Convert.ToInt32(x.MooringRopeType),
                                    ManufacturerType = x.ManufacturerType,
                                    MaximumMonthsAllowed = x.MaximumMonthsAllowed == null ? 0 : Convert.ToInt32(x.MaximumMonthsAllowed),
                                    MaximumRunningHours = x.MaximumRunningHours == null ? 0 : Convert.ToInt32(x.MaximumRunningHours),
                                    Rating1 = x.Rating1,
                                    Rating2 = x.Rating2,
                                    Rating3 = x.Rating3,
                                    Rating4 = x.Rating4,
                                    Rating5 = x.Rating5,
                                    Rating6 = x.Rating6,
                                    Rating7 = x.Rating7
                                });
                            }

                            //stored result into datatable  
                            ropeTailInspectionSettings = LINQResultToDataTable(ropeTailInspSettingsExport);
                            ropeTailInspectionSettings.TableName = "RopeTailInspectionSettingsData";
                        }

                        if (collection["WinchRotationSettings"] != null)
                        {
                            var WinchRotationSettingsData = sc.WinchRotationSettings.Where(x => x.VesselID == VesselID).ToList();

                            List<WinchRotationSetting> WinchRotationSettingsExport = new List<WinchRotationSetting>();
                            foreach (var x in WinchRotationSettingsData)
                            {
                                WinchRotationSettingsExport.Add(new WinchRotationSetting
                                {
                                    Id = x.Id,
                                    MooringRopeType = Convert.ToInt32(x.MooringRopeType),
                                    ManufacturerType = x.ManufacturerType,
                                    MaximumRunningHours = x.MaximumRunningHours,
                                    MaximumMonthsAllowed = x.MaximumMonthsAllowed,
                                    LeadFrom = x.LeadFrom,
                                    LeadTo = x.LeadTo,
                                    VesselID = x.VesselID
                                });
                            }

                            //stored result into datatable  
                            WinchRotationSettings = LINQResultToDataTable(WinchRotationSettingsExport);
                            WinchRotationSettings.TableName = "WinchRotationSettingsData";
                        }

                        DataTable smartMenu = new DataTable();
                        var smartMenuData = sc.SmartMenus.ToList();

                        List<SmartMenuExport> smartMenuExports = new List<SmartMenuExport>();
                        foreach (var x in smartMenuData)
                        {
                            var menu = new SmartMenuExport
                            {
                                ID = x.Id,
                            };

                            foreach (string m in hrefs)
                            {
                                if (x.SmartMenuContentExport.Contains(m))
                                    x.SmartMenuContentExport = x.SmartMenuContentExport.Replace(m, string.Empty);
                            }

                            menu.SmartMenuContentExport = x.SmartMenuContentExport.Trim();

                            smartMenuExports.Add(menu);
                        }

                        //stored result into datatable  
                        smartMenu = LINQResultToDataTable(smartMenuExports);
                        smartMenu.TableName = "SmartMenu";

                        var commonTableData = sc.MasterCommons.ToList();
                        DataTable commonTable = LINQResultToDataTable(commonTableData);
                        commonTable.TableName = "CommonTableData";

                        var MooringRopeTypes = sc.RopeTypesCommon.ToList();
                        DataTable RopeTypes = LINQResultToDataTable(MooringRopeTypes);
                        RopeTypes.TableName = "MooringRopeType";

                        if (collection["notificationCommentSettings"] != null)
                        {
                            List<NotificationCommentExport> notificationCommentExport = new List<NotificationCommentExport>();
                            using (Reports.MorringOfficeEntities morringOfficeContext = new Reports.MorringOfficeEntities())
                            {
                                var notoficationCommentsData = morringOfficeContext.NotificationComments.
                                    Where(p => p.CommentsType == 2 && p.VesselID == VesselID && p.CreatedDate >= ServiceFrom && p.CreatedDate <= ServiceTo).ToList();

                                foreach (var x in notoficationCommentsData)
                                {
                                    notificationCommentExport.Add(new NotificationCommentExport
                                    {
                                        Comments = x.Comments,
                                        CommentsType = x.CommentsType == null ? 0 : Convert.ToInt32(x.CommentsType),
                                        CreatedBy = x.CreatedBy,
                                        CreatedDate = x.CreatedDate == null ? DateTime.Now : Convert.ToDateTime(x.CreatedDate),
                                        Id = x.Id,
                                        IsActive = x.IsActive == null ? false : Convert.ToBoolean(x.CommentsType),
                                        NotificationId = x.NotificationId == null ? 0 : Convert.ToInt32(x.CommentsType),
                                        ShipID = x.VesselID
                                    });
                                }
                            }

                            //stored result into datatable  
                            notificationCommentSettings = LINQResultToDataTable(notificationCommentExport);
                            notificationCommentSettings.TableName = "NotificationCommentData";
                        }

                        using (XLWorkbook wb = new XLWorkbook())
                        {
                            var protectedsheet = wb.Worksheets.Add(vesselDetails);

                            if (shipSpecificContent.Rows.Count > 0)
                                protectedsheet = wb.Worksheets.Add(shipSpecificContent);

                            if (shipSpecificAttachments.Rows.Count > 0)
                                protectedsheet = wb.Worksheets.Add(shipSpecificAttachments);

                            if (looseEquipInspectionSettings.Rows.Count > 0)
                                protectedsheet = wb.Worksheets.Add(looseEquipInspectionSettings);

                            if (ropeInspectionSettings.Rows.Count > 0)
                                protectedsheet = wb.Worksheets.Add(ropeInspectionSettings);

                            if (ropeTailInspectionSettings.Rows.Count > 0)
                                protectedsheet = wb.Worksheets.Add(ropeTailInspectionSettings);

                            if (WinchRotationSettings.Rows.Count > 0)
                                protectedsheet = wb.Worksheets.Add(WinchRotationSettings);

                            if (commonTable.Rows.Count > 0)
                                protectedsheet = wb.Worksheets.Add(commonTable);

                            if (RopeTypes.Rows.Count > 0)
                                protectedsheet = wb.Worksheets.Add(RopeTypes);

                            if (revisionTable.Rows.Count > 0)
                                protectedsheet = wb.Worksheets.Add(revisionTable);

                            if (docsPages.Rows.Count > 0)
                                protectedsheet = wb.Worksheets.Add(docsPages);

                            if (notificationCommentSettings.Rows.Count > 0)
                                protectedsheet = wb.Worksheets.Add(notificationCommentSettings);

                            if (smartMenu.Rows.Count > 0)
                                protectedsheet = wb.Worksheets.Add(smartMenu);

                            var projection = protectedsheet.Protect("49WEB$TREET#");
                            projection.InsertColumns = true;
                            projection.InsertRows = true;

                            wb.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                            wb.Style.Font.Bold = true;

                            DateTime today = DateTime.Today;
                            //string vsname = searchTerm.Replace(" ", "");
                            //string HeaderName = "Work-Ship_Export_" + vsname + "_" + today.ToString("dd-MMM-yyyy");

                            Response.Clear();
                            Response.BufferOutput = true;
                            Response.Charset = "";
                            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                            Response.AddHeader("content-disposition", "attachment;filename=DMX7_Export_" + searchTerm.Replace(" ", "") + ".xlsx");

                            using (MemoryStream MyMemoryStream = new MemoryStream())
                            {
                                wb.SaveAs(MyMemoryStream);
                                MyMemoryStream.WriteTo(Response.OutputStream);
                                Response.End();
                            }

                            Response.Clear();

                            Thread.Sleep(300);
                        }
                    }
                    else
                    {
                        bmsuploaddats.bmsname = "Please select vessel name first";
                    }
                }
                else if (submit == "Export Attachments")
                {
                    // Download Attachments in Zip File
                    string startPath = Server.MapPath(string.Format("~/images/AttachFiles/filepath/{0}/", shipIMO));

                    if (Directory.Exists(startPath))
                    {
                        string[] filesFound = Directory.GetFiles(startPath);

                        if (filesFound.Length > 0)
                        {
                            string zipFileName = string.Format("{0}_{1}.zip", shipIMO, DateTime.Now.Ticks);
                            string zipPath = Server.MapPath("~/images/AttachFiles/filepath/" + zipFileName);
                            ZipFile.CreateFromDirectory(startPath, zipPath, CompressionLevel.Fastest, true);
                            Response.ContentType = "application/zip";
                            Response.AppendHeader("Content-Disposition", "attachment; filename=" + zipFileName);
                            Response.TransmitFile(zipPath);
                            Response.End();
                        }
                        else
                        {
                            bmsuploaddats.bmsname = "No Attachments found.";
                        }
                    }
                    else
                    {
                        bmsuploaddats.bmsname = "No Attachments found.";
                    }
                }
                if (submit == "Export History")
                {
                    if (!string.IsNullOrEmpty(searchTerm))
                    {
                        //DataTable vesselDetail = new DataTable();
                        DataSet ds = new DataSet();
                        string constr = ConfigurationManager.ConnectionStrings["ShipmentContaxt"].ConnectionString;
                        if (!string.IsNullOrEmpty(download))
                        {
                            if (download == "Line")
                            {
                                string qry1 = "Select VesselName,ImoNo as [Vessel IMO],Flag from VesselDetail where ImoNo=" + searchTerm + "";
                                SqlDataAdapter adp1 = new SqlDataAdapter(qry1, sc.con);
                                DataTable dtt1 = new DataTable();
                                adp1.Fill(dtt1);
                                dtt1.TableName = "Vessel Details";


                                string qry2 = "Select MRD.RopeId AS [Rope Id],MRD.UniqueID AS [Unique ID],MRD.CertificateNumber AS [Certificate Number],ISNULL(TC.Name,'N/A') AS [Manufacturer], ISNULL(MRT.RopeType,'N/A') AS [Rope Type],ISNULL(MRD.RopeConstruction,'N/A') AS [Rope Construction],MRD.DiaMeter AS[DiaMeter(mm)],MRD.Length AS[Length(m)],MRD.MBL AS[MBL(T)], MRD.LDBF AS[LDBF(T)],MRD.WLL AS[WLL(T)], ISNULL(Convert(varchar,MRD.ReceivedDate,106),'N/A') AS [Received Date],ISNULL(convert(varchar,MRD.InstalledDate , 106),'N/A') AS[Installed Date],MRD.RopeTagging AS[Rope Tagging Done(Yes / No)],ISNULL(convert(varchar, MRD.OutofServiceDate , 106),'N/A') as [Discarded Date],ISNULL(MRD.ReasonOutofService,'N/A') AS[Discard Reason],ISNULL(MRD.OtherReason,'N/A') AS[Discard(Other)],ISNULL(convert(varchar,MRD.InspectionDueDate , 106),'N/A') AS[Next Inspection Due Date],ISNULL(MRD.Remarks,'N/A') AS [Remarks], CASE WHEN MRD.DeleteStatus=0 THEN 'Active' ELSE 'Deleted' END AS [Active Status] from MooringRopeDetail MRD INNER JOIN MooringRopeType MRT ON MRD.RopeTypeId = MRT.Id INNER JOIN tblCommon TC ON MRD.ManufacturerId = TC.Id Where RopeTail=0 and DeleteStatus=0 and VesselID=" + searchTerm + "";
                                SqlDataAdapter adp2 = new SqlDataAdapter(qry2, sc.con);
                                DataTable dtt2 = new DataTable();
                                adp2.Fill(dtt2);
                                dtt2.TableName = "Line Details";


                                string qry3 = "Select a.RopeId AS [Rope Id],  b.UniqueId AS [Unique Id], b.certificatenumber AS [Certificate Number],Case when a.Outboard = 0 then 'A' else 'B' end as Outboard, ISNULL(e.assignednumber,'N/A') AS [Current Assigned Winch],ISNULL(e.Location,'N/A') as [Assigned Location] ,convert(varchar, a.AssignedDate , 106) as [Assigned Date], CASE WHEN a.IsActive=1 THEN 'Active' ELSE 'Deleted' END AS [Active Status] from AssignRopeToWinch a LEFT JOIN MooringRopeDetail b on a.RopeId=b.RopeId and a.VesselID= b.VesselID LEFT JOIN MooringWinchDetail e on a.WinchId=e.Id and e.VesselID= a.VesselID where a.RopeTail=0 and b.DeleteStatus=0 and a.VesselID=" + searchTerm + "";
                                SqlDataAdapter adp3 = new SqlDataAdapter(qry3, sc.con);
                                DataTable dtt3 = new DataTable();
                                adp3.Fill(dtt3);
                                dtt3.TableName = "Winch Assigned";

                                string qry4 = "Select a.RopeId AS [Rope Id],  b.UniqueId AS [Unique Id], b.certificatenumber AS [Certificate Number],ISNULL(convert(varchar, a.AssignedDate , 106),'N/A') as [Assigned Date], ISNULL(e.assignednumber,'N/A') AS [Winch Name],ISNULL(a.Lead,'N/A') AS [Lead],a.RunningHours, CASE WHEN b.DeleteStatus=0 THEN 'Active' ELSE 'Deleted'  END AS [Active Status] from WinchRotation a left join MooringRopeDetail b on a.RopeId=b.RopeId and a.VesselID= b.VesselID left join MooringWinchDetail e on a.WinchId=e.Id and e.VesselID= a.VesselID where b.DeleteStatus=0 and a.VesselID=" + searchTerm + "";
                                SqlDataAdapter adp4 = new SqlDataAdapter(qry4, sc.con);
                                DataTable dtt4 = new DataTable();
                                adp4.Fill(dtt4);
                                dtt4.TableName = "Winch Rotation";

                                string qry5 = "Select a.RopeId AS [Rope Id],  b.UniqueId AS [Unique ID], b.certificatenumber AS [Certificate Number],ISNULL(Convert(varchar,a.EndtoEndDoneDate,106),'N/A') AS [End to End Done Date],ISNULL(a.WinchCrossShifting,'N/A') AS [Winch Cross Shifted],ISNULL(e.assignednumber,'N/A') AS [Winch Name],CASE WHEN b.DeleteStatus=0 THEN 'Active' ELSE 'Deleted' END AS [Active Status] from RopeEndtoEnd2 a left join MooringRopeDetail b on a.RopeId=b.RopeId and a.VesselID= b.VesselID left join MooringWinchDetail e on a.WinchId=e.Id and e.VesselID= a.VesselID  where b.DeleteStatus=0 and a.VesselID=" + searchTerm + "";
                                SqlDataAdapter adp5 = new SqlDataAdapter(qry5, sc.con);
                                DataTable dtt5 = new DataTable();
                                adp5.Fill(dtt5);
                                dtt5.TableName = "Line End To End";

                                string qry6 = "Select ISNULL(Convert(varchar,a.InspectDate,106),'N/A') AS [Inspect Date],ISNULL(a.InspectBy,'N/A') AS [Inspect By], a.RopeId AS [Rope Id],b.UniqueId AS [Unique ID],b.certificatenumber AS [Certificate Number],ISNULL(e.assignednumber,'N/A') AS [Winch Name],ISNULL(Convert(varchar,a.ExternalRating_A,106),'N/A') AS [ExternalRating_A],ISNULL(Convert(varchar,InternalRating_A,106),'N/A') AS [InternalRating_A],a.AverageRating_A,a.LengthOFAbrasion_A AS [LengthOFAbrasion_A (m)],a.DistanceOutboard_A,a.CutYarnCount_A,a.LengthOFGlazing_A AS [LengthOFGlazing_A (m)],a.ExternalRating_B ,a.InternalRating_B,a.AverageRating_B,ISNULL(Convert(varchar,a.LengthOFAbrasion_B,106),'N/A') AS [LengthOFAbrasion_B (m)],ISNULL(Convert(varchar,a.DistanceOutboard_B,106),'N/A'),a.CutYarnCount_B,a.LengthOFGlazing_B AS [LengthOFGlazing_B (m)],a.Chafe_guard_condition,a.Twist,ISNULL(a.Image1,'N/A') AS [Photo1] ,ISNULL(a.Image2,'N/A') AS [Photo2],ISNULL(Convert(varchar,a.NotificationId,106),'N/A') AS [Notification Id] ,CASE WHEN b.DeleteStatus=0 THEN 'Active' ELSE 'Deleted' END AS [Active Status] from MooringRopeInspection a left join MooringRopeDetail b on a.RopeId=b.RopeId and a.VesselID= b.VesselID left join MooringWinchDetail e on a.WinchId=e.Id and e.VesselID= a.VesselID where a.RopeTail=0 and b.DeleteStatus=0 and a.VesselID=" + searchTerm + "";
                                SqlDataAdapter adp6 = new SqlDataAdapter(qry6, sc.con);
                                DataTable dtt6 = new DataTable();
                                adp6.Fill(dtt6);
                                dtt6.TableName = "Line Inspection";

                                string qry7 = "Select a.RopeId AS [Rope Id],  b.UniqueId AS [Unique ID], b.certificatenumber AS [Certificate Number],ISNULL(Convert(varchar,a.CroppedDate,106),'N/A') AS [Cropped Date],ISNULL(a.CroppedOutboardEnd,'N/A') AS [Cropped Outboard End], a.LengthofCroppedRope AS [Cropped Length (m)],ISNULL(a.ReasonofCropping,'N/A') AS [Reason of Cropping] ,a.SplicedId AS [Spliced Id] ,ISNULL(Convert(varchar,a.NotificationId,106),'N/A') AS [Notification Id],ISNULL(Convert(varchar,a.DamageId,106),'N/A') AS [Damage Id],ISNULL(Convert(varchar,a.MOpid,106),'N/A') AS [Mooring Op Id] ,ISNULL(e.assignednumber,'N/A') AS [Winch Name], CASE WHEN b.DeleteStatus=0 THEN 'Active' ELSE 'Deleted' END AS [Active Status] from RopeCropping a left join MooringRopeDetail b on a.RopeId=b.RopeId and a.VesselID= b.VesselID  left join MooringWinchDetail e on a.WinchId=e.Id and e.VesselID= a.VesselID where a.RopeTail=0 and b.DeleteStatus=0 and a.VesselID=" + searchTerm + "";
                                SqlDataAdapter adp7 = new SqlDataAdapter(qry7, sc.con);
                                DataTable dtt7 = new DataTable();
                                adp7.Fill(dtt7);
                                dtt7.TableName = "Line Cropping";

                                string qry8 = "Select a.RopeId AS [Rope Id],  b.UniqueId AS [Unique ID], b.certificatenumber AS [Certificate Number],ISNULL(Convert(varchar,a.SplicingDoneDate,106),'N/A') AS [Splicing Done Date],ISNULL(a.SplicingMethod,'N/A') AS [Splicing Method], ISNULL(a.SplicingDoneBy,'N/A') AS [Splicing Done By], ISNULL(Convert(varchar,a.NotificationId,106),'N/A')  AS [Notification Id],ISNULL(e.assignednumber,'N/A') AS [Winch Name],CASE WHEN b.DeleteStatus=0 THEN 'Active'  ELSE 'Deleted' END AS [Active Status] from RopeSplicingRecord  a left join MooringRopeDetail b on a.RopeId=b.RopeId and a.VesselID= b.VesselID left join MooringWinchDetail e on a.WinchId=e.Id and e.VesselID= a.VesselID where a.RopeTail=0 and b.DeleteStatus=0 and a.VesselID=" + searchTerm + "";
                                SqlDataAdapter adp8 = new SqlDataAdapter(qry8, sc.con);
                                DataTable dtt8 = new DataTable();
                                adp8.Fill(dtt8);
                                dtt8.TableName = "Line Splicing";

                                string qry9 = "Select a.RopeId AS [Rope Id],  b.UniqueId AS [Unique ID], b.certificatenumber AS [Certificate Number],ISNULL(Convert(varchar,a.DamageDate,106),'N/A') AS [Damage Date],ISNULL(a.DamageObserved,'N/A') AS [Damage Observed Event],ISNULL(a.IncidentReport ,'N/A')AS [Incident Reported], ISNULL(a.DamageLocation,'N/A') AS [Damage Location],ISNULL(a.DamageReason,'N/A')  AS [Damage Reason],ISNULL(a.IncidentActlion,'N/A') AS [Incident Action],ISNULL(e.assignednumber,'N/A') AS [Winch Name],ISNULL(Convert(varchar, a.MOPId, 106),'N/A') AS [Mooring Operation Id],ISNULL(Convert(varchar,a.NotificationId),'N/A') AS [Notification Id], CASE WHEN b.DeleteStatus=0 THEN 'Active' ELSE 'Deleted' END AS [Active Status] from RopeDamageRecord  a left join MooringRopeDetail b on a.RopeId=b.RopeId and a.VesselID= b.VesselID left join MooringWinchDetail e on a.WinchId=e.Id and e.VesselID= a.VesselID where a.RopeTail=0 and b.DeleteStatus=0 and a.VesselID=" + searchTerm + "";
                                SqlDataAdapter adp9 = new SqlDataAdapter(qry9, sc.con);
                                DataTable dtt9 = new DataTable();
                                adp9.Fill(dtt9);
                                dtt9.TableName = "Line Damage";

                                string qry10 = "Select a.RopeId AS [Rope Id],  b.UniqueId AS [Unique ID], b.certificatenumber AS [Certificate Number],ISNULL(a.DisposalPortName,'N/A') AS [Disposal Port Name],ISNULL(a.ReceptionFacilityName,'N/A') AS [Reception Facility Name],ISNULL(Convert(varchar,a.DisposalDate,106),'N/A') AS [Disposal Date], CASE WHEN b.DeleteStatus=0 THEN 'Active' ELSE 'Deleted'END AS [Active Status],ISNULL(e.assignednumber,'N/A') AS [Winch Name] from RopeDisposal  a left join MooringRopeDetail b on a.RopeId=b.RopeId and a.VesselID= b.VesselID  left join MooringWinchDetail e on a.WinchId=e.Id and e.VesselID= a.VesselID where  b.DeleteStatus=0 and a.RopeTail=0 and a.VesselID=" + searchTerm + "";
                                SqlDataAdapter adp10 = new SqlDataAdapter(qry10, sc.con);
                                DataTable dtt10 = new DataTable();
                                adp10.Fill(dtt10);
                                dtt10.TableName = "Line Disposal";

                                string qry11 = "Select a.RopeId AS [Rope Id],  b.UniqueId AS [Unique ID], b.certificatenumber AS [Certificate Number],ISNULL(e.RopeType,'N/A') AS [Rope Type],t.Name AS [Manufacturer],ISNULL(a.Location,'N/A') AS Location,a.RunningHours,ISNULL(Convert(varchar,a.ServiceTestDate,106),'N/A') AS [Service Test Date],ISNULL(Convert(varchar,a.LabTestDate,106),'N/A') AS [Lab Test Date],ISNULL(convert(varchar,TestResults, 160),'N/A') AS [Test Results],ISNULL(a.Remarks,'N/A') AS [Remarks],ISNULL(Convert(varchar,a.CreatedDate,106),'N/A') AS [CreatedDate], CASE WHEN b.DeleteStatus=0 THEN 'Active' ELSE 'Deleted'END AS [Active Status] from ResidualLabTest  a  Inner join MooringRopeType e on a.RopeTypeId=e.Id Inner join tblCommon t on a.ManufacturerId=t.Id Inner Join MooringRopeDetail b on a.RopeId=b.RopeId where a.RopeTail=0 and b.DeleteStatus=0 and b.VesselID=" + searchTerm + "";
                                SqlDataAdapter adp11 = new SqlDataAdapter(qry11, sc.con);
                                DataTable dtt11 = new DataTable();
                                adp11.Fill(dtt11);
                                dtt11.TableName = "Line Residual Lab Test";

                                string qry12 = "Select ISNULL(Convert(varchar,a.InspectDate,106),'N/A') AS [Inspect Date],ISNULL(a.InspectBy,'N/A') AS [Inspect By], a.RopeId AS [Rope Id],b.UniqueId AS [Unique ID],b.certificatenumber AS [Certificate Number],ISNULL(e.assignednumber,'N/A') AS [Winch Name],ISNULL(Convert(varchar,a.ExternalRating_A,106),'N/A') AS [ExternalRating_A],ISNULL(Convert(varchar,InternalRating_A,106),'N/A') AS [InternalRating_A],a.AverageRating_A,a.LengthOFAbrasion_A AS [LengthOFAbrasion_A (m)],a.DistanceOutboard_A,a.CutYarnCount_A,a.LengthOFGlazing_A AS [LengthOFGlazing_A (m)],a.ExternalRating_B ,a.InternalRating_B,a.AverageRating_B,ISNULL(Convert(varchar,a.LengthOFAbrasion_B,106),'N/A') AS [LengthOFAbrasion_B (m)],ISNULL(Convert(varchar,a.DistanceOutboard_B,106),'N/A') AS [DistanceOutboard_B],a.CutYarnCount_B,a.LengthOFGlazing_B AS [LengthOFGlazing_B (m)],a.Chafe_guard_condition,a.Twist,ISNULL(a.Image1,'N/A') AS [Photo1] ,ISNULL(a.Image2,'N/A') AS [Photo2],ISNULL(Convert(varchar,a.NotificationId,106),'N/A') AS [Notification Id] ,CASE WHEN b.DeleteStatus=0 THEN 'Active' ELSE 'Deleted' END AS [Active Status] from MooringRopeInspection a left join MooringRopeDetail b on a.RopeId=b.RopeId and a.VesselID= b.VesselID left join MooringWinchDetail e on a.WinchId=e.Id and e.VesselID= a.VesselID where a.RopeTail=0 and b.DeleteStatus=0 and a.VesselID=" + searchTerm + "";
                                SqlDataAdapter adp12 = new SqlDataAdapter(qry12, sc.con);
                                DataTable dtt12 = new DataTable();
                                adp12.Fill(dtt12);
                                dtt12.TableName = "Mooring Op Berth Details";

                                string qry13 = "Select a.OperationID AS [Operation ID],a.GridID AS [Grid ID],a.OPDateFrom AS [Fast Date-Time], a.OPDateTo AS [Cast Off Date-Time],ISNULL(e.assignednumber,'N/A') AS [Winch Name],a.RopeId AS [Rope Id], b.UniqueId AS [Unique Id], b.certificatenumber AS [Certificate Number],ISNULL(e.Location,'N/A') AS [Winch Location],a.RunningHours,a.NotificationId AS [Notification Id],ISNULL(a.Lead,'N/A') AS [Lead Direction],ISNULL(a.Lead1,'N/A') AS [Lead Type] ,Case when a.Outboard = 0 then 'A' else 'B' end as Outboard,Case when b.DeleteStatus = 0 then 'Active' else 'Deleted' end as [Active Status] from MOUsedWinchTbl a INNER join MooringRopeDetail b on a.RopeId=b.RopeId and a.VesselID= b.VesselID INNER join MooringWinchDetail e on a.WinchId=e.Id and e.VesselID= a.VesselID where a.RopeTail=0 and b.DeleteStatus=0 and a.VesselID=" + searchTerm + "";
                                SqlDataAdapter adp13 = new SqlDataAdapter(qry13, sc.con);
                                DataTable dtt13 = new DataTable();
                                adp13.Fill(dtt13);
                                dtt13.TableName = "Mooring Op Line Used";


                                using (XLWorkbook wb = new XLWorkbook())
                                {
                                    var protectedsheet = wb.Worksheets.Add(dtt1);
                                    var protectedsheet1 = wb.Worksheets.Add(dtt2);
                                    var protectedsheet2 = wb.Worksheets.Add(dtt3);
                                    var protectedsheet3 = wb.Worksheets.Add(dtt4);
                                    var protectedsheet4 = wb.Worksheets.Add(dtt5);
                                    var protectedsheet5 = wb.Worksheets.Add(dtt6);
                                    var protectedsheet6 = wb.Worksheets.Add(dtt7);
                                    var protectedsheet7 = wb.Worksheets.Add(dtt8);
                                    var protectedsheet8 = wb.Worksheets.Add(dtt9);
                                    var protectedsheet9 = wb.Worksheets.Add(dtt10);
                                    var protectedsheet10 = wb.Worksheets.Add(dtt11);
                                    var protectedsheet11 = wb.Worksheets.Add(dtt12);
                                    var protectedsheet12 = wb.Worksheets.Add(dtt13);

                                    var projection = protectedsheet.Protect("49WEB$TREET#");
                                    projection.InsertColumns = true;
                                    projection.InsertRows = true;

                                    wb.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                                    wb.Style.Font.Bold = true;

                                    DateTime today = DateTime.Today;
                                    //string vsname = searchTerm.Replace(" ", "");
                                    //string HeaderName = "Work-Ship_Export_" + vsname + "_" + today.ToString("dd-MMM-yyyy");

                                    Response.Clear();
                                    Response.BufferOutput = true;
                                    Response.Charset = "";
                                    //K - Line_Export_9785158_MooringLineDetails
                                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                                    Response.AddHeader("content-disposition", "attachment;filename=DigiMoorX7_Export_" + searchTerm + "_" + "MooringLineDetails".Replace(" ", "") + ".xlsx");

                                    using (MemoryStream MyMemoryStream = new MemoryStream())
                                    {
                                        wb.SaveAs(MyMemoryStream);
                                        MyMemoryStream.WriteTo(Response.OutputStream);
                                        Response.End();
                                    }

                                    Response.Clear();

                                    Thread.Sleep(300);
                                }
                                
                            }
                            else if (download == "RopeTail")
                            {

                                string qryy = "Select VesselName,ImoNo as [Vessel IMO],Flag from VesselDetail where ImoNo=" + searchTerm + "";
                                SqlDataAdapter adapter = new SqlDataAdapter(qryy, sc.con);
                                DataTable dttt = new DataTable();
                                adapter.Fill(dttt);
                                dttt.TableName = "Vessel Details";

                                string qryy1 = "Select MRD.UniqueID AS [Unique ID],MRD.CertificateNumber AS [Certificate Number],TC.Name AS [Manufacturer],MRT.RopeType AS[Rope Type], MRD.DiaMeter AS[DiaMeter(mm)],MRD.Length AS[Length(m)],MRD.MBL AS[MBL(T)], MRD.LDBF AS[LDBF(T)], MRD.WLL AS[WLL(T)],MRD.ReceivedDate AS[Received Date],ISNULL(convert(varchar,MRD.InstalledDate, 106),'N/A') AS[Installed Date],MRD.RopeTagging AS[Rope Tagging(Yes / No)],ISNULL(convert(varchar,MRD.OutofServiceDate,106),'N/A') AS[Discarded Date],ISNULL(convert(varchar,MRD.ReasonOutofService,106),'N/A') AS[Discard Reason],ISNULL(MRD.OtherReason,'N/A') AS[Discard(Other)],ISNULL(convert(varchar,MRD.InspectionDueDate,106),'N/A') AS[Next Inspection Due Date],ISNULL(MRD.Remarks,'N/A') AS [Remarks], CASE WHEN MRD.DeleteStatus=0 THEN 'Active' ELSE 'Deleted' END AS [Active Status] from MooringRopeDetail MRD INNER JOIN MooringRopeType MRT ON MRD.RopeTypeId = MRT.Id INNER JOIN tblCommon TC ON MRD.ManufacturerId = TC.Id Where  MRD.RopeTail=1 and MRD.DeleteStatus=0 and VesselID=" + searchTerm + "";
                                SqlDataAdapter adapter1 = new SqlDataAdapter(qryy1, sc.con);
                                DataTable dttt1 = new DataTable();
                                adapter1.Fill(dttt1);
                                dttt1.TableName = "Rope Tail Details";

                                string qryy2 = "Select  b.UniqueId AS [Unique Id], b.certificatenumber AS [Certificate Number], Case when a.Outboard = 0 then 'A' else 'B' end as Outboard,ISNULL(e.assignednumber,'N/A') AS [Current Assigned Winch], ISNULL(e.Location,'N/A') as [Assigned Location] ,ISNULL(convert(varchar, a.AssignedDate , 106),'N/A') as [Assigned Date], CASE WHEN a.IsActive=1 THEN 'Active' ELSE 'Deleted' END AS [Active Status] from AssignRopeToWinch a LEFT JOIN MooringRopeDetail b on a.RopeId=b.RopeId and a.VesselID= b.VesselID LEFT JOIN MooringWinchDetail e on a.WinchId=e.Id and e.VesselID= a.VesselID where b.DeleteStatus=0 and  b.RopeTail=1 and  a.VesselID=" + searchTerm + "";
                                SqlDataAdapter adapter2 = new SqlDataAdapter(qryy2, sc.con);
                                DataTable dttt2 = new DataTable();
                                adapter2.Fill(dttt2);
                                dttt2.TableName = "Winch Assigned";

                                string qryy3 = "Select b.UniqueId AS [Unique Id], b.certificatenumber AS [Certificate Number],convert(varchar, a.AssignedDate , 106) as [Assigned Date],  ISNULL(e.assignednumber,'N/A') AS [Winch Name],ISNULL(a.Lead,'N/A') AS [Lead],a.RunningHours, CASE WHEN b.DeleteStatus=0 THEN 'Active' ELSE 'Deleted' END AS [Active Status] from WinchRotation a left join MooringRopeDetail b on a.RopeId=b.RopeId and a.VesselID= b.VesselID left join MooringWinchDetail e on a.WinchId=e.Id and e.VesselID= a.VesselID where b.DeleteStatus=0 and b.RopeTail=1 and a.VesselID=" + searchTerm + "";
                                SqlDataAdapter adapter3 = new SqlDataAdapter(qryy3, sc.con);
                                DataTable dttt3 = new DataTable();
                                adapter3.Fill(dttt3);
                                dttt3.TableName = "Winch Rotation";

                                string qryy4 = "Select ISNULL(Convert(varchar,a.InspectDate,106),'N/A') AS [Inspect Date],ISNULL(a.InspectBy,'N/A') AS [Inspect By], b.UniqueId AS [Unique ID], b.certificatenumber AS [Certificate Number],ISNULL(e.assignednumber,'N/A') AS [Winch Name],ISNULL(Convert(varchar,a.ExternalRating_A,106),'N/A') AS [ExternalRating_A] ,ISNULL(Convert(varchar,a.InternalRating_A,106),'N/A') AS [InternalRating_A],ISNULL(Convert(varchar,a.AverageRating_A,106),'N/A') AS [AverageRating_A],ISNULL(Convert(varchar,a.LengthOFAbrasion_A,106),'N/A') AS [LengthOFAbrasion_A (m)],ISNULL(Convert(varchar,a.DistanceOutboard_A,106),'N/A') AS [DistanceOutboard_A],ISNULL(Convert(varchar,a.CutYarnCount_A,106),'N/A') AS [CutYarnCount_A],a.LengthOFGlazing_A AS [LengthOFGlazing_A (m)],a.ExternalRating_B ,a.InternalRating_B,a.AverageRating_B,ISNULL(Convert(varchar,a.LengthOFAbrasion_B,106),'N/A') AS [LengthOFAbrasion_B (m)],ISNULL(Convert(varchar,a.DistanceOutboard_B),'N/A') AS [DistanceOutboard_B],ISNULL(Convert(varchar,a.CutYarnCount_B),'N/A') AS [CutYarnCount_B],ISNULL(Convert(varchar,a.LengthOFGlazing_B),'N/A') AS [LengthOFGlazing_B (m)],ISNULL(Convert(varchar,a.Chafe_guard_condition),'N/A') AS [Chafe_guard_condition],a.Twist,ISNULL(a.Image1,'N/A') AS [Photo1],ISNULL(a.Image2,'N/A') AS Photo2,ISNULL(Convert(varchar,a.NotificationId,106),'N/A') AS [Notification Id],CASE WHEN b.DeleteStatus=0 THEN 'Active' ELSE 'Deleted' END AS [Active Status] from MooringRopeInspection a left join MooringRopeDetail b on a.RopeId=b.RopeId and a.VesselID= b.VesselID left join MooringWinchDetail e on a.WinchId=e.Id and e.VesselID= a.VesselID where b.DeleteStatus=0 and b.RopeTail=1 and a.VesselID=" + searchTerm + "";
                                SqlDataAdapter adapter4 = new SqlDataAdapter(qryy4, sc.con);
                                DataTable dttt4 = new DataTable();
                                adapter4.Fill(dttt4);
                                dttt4.TableName = "Rope Tail Inspection";


                                string qryy5 = "Select b.UniqueId AS [Unique ID], b.certificatenumber AS [Certificate Number],ISNULL(Convert(varchar,a.CroppedDate,106),'N/A') AS [Cropped Date],ISNULL(a.CroppedOutboardEnd,'N/A') AS [Cropped Outboard End],a.LengthofCroppedRope AS [Cropped Length (m)],ISNULL(a.ReasonofCropping,'N/A') AS [Reason of Cropping],ISNULL(Convert(varchar,a.SplicedId,106),'N/A') AS [Spliced Id] ,ISNULL(Convert(varchar,a.NotificationId,106),'N/A') AS [Notification Id],ISNULL(Convert(varchar,a.DamageId,106),'N/A') AS [Damage Id],ISNULL(Convert(varchar,a.MOpid,106),'N/A') AS [Mooring Op Id],ISNULL(e.assignednumber,'N/A') AS [Winch Name], CASE WHEN b.DeleteStatus=0 THEN 'Active' ELSE 'Deleted' END AS [Active Status] from RopeCropping a left join MooringRopeDetail b on a.RopeId=b.RopeId and a.VesselID= b.VesselID left join MooringWinchDetail e on a.WinchId=e.Id and e.VesselID= a.VesselID  where b.DeleteStatus=0 and b.RopeTail=1 and a.VesselID=" + searchTerm + "";
                                SqlDataAdapter adapter5 = new SqlDataAdapter(qryy5, sc.con);
                                DataTable dttt5 = new DataTable();
                                adapter5.Fill(dttt5);
                                dttt5.TableName = "Rope Tail Cropping";

                                string qryy6 = "Select b.UniqueId AS [Unique ID], b.certificatenumber AS [Certificate Number],a.SplicingDoneDate AS [Splicing Done Date],ISNULL(a.SplicingMethod,'N/A') AS [SplicingMethod], a.SplicingDoneBy AS [Splicing Done By],ISNULL(Convert(varchar,a.NotificationId,106),'N/A')  AS [Notification Id], ISNULL(e.assignednumber,'N/A') AS [Winch Name],CASE WHEN b.DeleteStatus=0 THEN 'Active'  ELSE 'Deleted' END AS [Active Status] from RopeSplicingRecord  a left join MooringRopeDetail b on a.RopeId=b.RopeId and a.VesselID= b.VesselID left join MooringWinchDetail e on a.WinchId=e.Id and e.VesselID= a.VesselID where b.DeleteStatus=0 and b.RopeTail=1 and a.VesselID=" + searchTerm + "";
                                SqlDataAdapter adapter6 = new SqlDataAdapter(qryy6, sc.con);
                                DataTable dttt6 = new DataTable();
                                adapter6.Fill(dttt6);
                                dttt6.TableName = "Rope Tail Splicing";

                                string qryy7 = "Select b.UniqueId AS [Unique ID], b.certificatenumber AS [Certificate Number],ISNULL(convert(varchar,a.DamageDate,106),'N/A') AS [Damage Date],ISNULL(a.DamageObserved,'N/A') AS [Damage Observed Event],a.IncidentReport AS [Incident Reported],ISNULL(a.DamageLocation,'N/A') AS [Damage Location],ISNULL(a.DamageReason,'N/A') AS [Damage Reason],ISNULL(a.IncidentActlion,'N/A') AS [Incident Action],ISNULL(e.assignednumber,'N/A') AS [Winch Name],ISNULL(Convert(varchar,a.MOPId,106),'N/A') AS [Mooring Operation Id],ISNULL(Convert(varchar,a.NotificationId,106),'N/A') AS [Notification Id],CASE WHEN b.DeleteStatus=0 THEN 'Active' ELSE 'Deleted' END AS [Active Status]  from RopeDamageRecord  a left join MooringRopeDetail b on a.RopeId=b.RopeId and a.VesselID= b.VesselID left join MooringWinchDetail e on a.WinchId=e.Id and e.VesselID= a.VesselID where b.DeleteStatus=0 and b.RopeTail=1 and a.VesselID=" + searchTerm + "";
                                SqlDataAdapter adapter7 = new SqlDataAdapter(qryy7, sc.con);
                                DataTable dttt7 = new DataTable();
                                adapter7.Fill(dttt7);
                                dttt7.TableName = "Rope Tail Damage";

                                string qryy8 = "Select b.UniqueId AS [Unique ID], b.certificatenumber AS [Certificate Number],ISNULL(a.DisposalPortName,'N/A') AS [Disposal Port Name],ISNULL(a.ReceptionFacilityName,'N/A') AS [Reception Facility Name],ISNULL(Convert(varchar,a.DisposalDate,106),'N/A') AS [Disposal Date], ISNULL(e.assignednumber,'N/A') AS [Winch Name] ,CASE WHEN b.DeleteStatus=0 THEN 'Active' ELSE 'Deleted'END AS [Active Status] from RopeDisposal  a left join MooringRopeDetail b on a.RopeId=b.RopeId and a.VesselID= b.VesselID left join MooringWinchDetail e on a.WinchId=e.Id and e.VesselID= a.VesselID where b.DeleteStatus=0 and b.RopeTail=1 and a.VesselID=" + searchTerm + "";
                                SqlDataAdapter adapter8 = new SqlDataAdapter(qryy8, sc.con);
                                DataTable dttt8 = new DataTable();
                                adapter8.Fill(dttt8);
                                dttt8.TableName = "Rope Tail Disposal";

                                string qryy9 = "Select b.UniqueId AS [Unique ID], b.certificatenumber AS [Certificate Number],e.RopeType AS [Rope Type],ISNULL(t.Name,'N/A') AS [Manufacturer],ISNULL(a.Location,'N/A') AS [Location],a.RunningHours, ISNULL(Convert(varchar,a.ServiceTestDate, 106),'N/A') AS [Service Test Date],ISNULL(Convert(varchar,a.LabTestDate,106),'N/A') AS [Lab Test Date],a.TestResults AS [Test Result],ISNULL(a.Remarks,'N/A') AS [Remarks],a.CreatedDate, CASE WHEN b.DeleteStatus=0 THEN 'Active' ELSE 'Deleted' END AS [Active Status] from ResidualLabTest a Inner join MooringRopeType e on a.RopeTypeId=e.Id  Inner join tblCommon t on a.ManufacturerId=t.Id Inner Join MooringRopeDetail b on a.RopeId=b.RopeId where b.DeleteStatus=0 and b.RopeTail=1 and b.VesselID=" + searchTerm + "";
                                SqlDataAdapter adapter9 = new SqlDataAdapter(qryy9, sc.con);
                                DataTable dttt9 = new DataTable();
                                adapter9.Fill(dttt9);
                                dttt9.TableName = "Tail Residual Lab Test";

                                string qryy10 = "Select a.OPId AS [OP Id] ,a.FastDatetime AS [All Fast Date-Time], a.CastDatetime AS [Cast Off Date-Time],ISNULL(a.PortName,'N/A') AS [Port Name],ISNULL(a.FacilityName,'N/A') AS [Facility Name],ISNULL(a.Others,'N/A') AS [Other Facility Name],ISNULL(a.BirthName,'N/A') AS [Berth Name],ISNULL(a.BirthType,'N/A') AS [Berth Type], ISNULL(a.MooringType,'N/A') AS [Mooring Type], DraftArrivalFWD ,a.DraftArrivalAFT,a.DraftDepartureFWD,a.DraftDepartureAFT,a.DepthAtBerth AS [Depth At Berth], ISNULL(a.BerthSide,'N/A') AS [Berthing Side],ISNULL(a.VesselCondition,'N/A') AS [Vessel Condition],ISNULL(a.ShipAccess,'N/A') AS [Ship Access],a.RangOfTide AS [Rang Of Tide],a.WindDirection AS [Wind Direction],a.WindSpeed AS [Wind Speed],a.AnySquall AS [Squall Observed],a.CurrentSpeed AS [Current Speed],a.Berth_exposed_SeaSwell AS [Berth exposed to Sea Swell],a.SurgingObserved AS [Surging Observed],a.Any_Affect_Passing_Traffic AS [Affected by Passing Traffic],a.Ship_was_continuously_contact_with_fender AS [Ship Continuously in contact with Fender],a.Any_Rope_Damaged AS [Any Line was Damaged],ISNULL(a.PortDetails,'N/A') AS [Master's Remarks],a.AirTemprature AS [Air Temprature], CASE WHEN a.AirTempCentigrate=1 THEN 'Positive' ELSE 'Negative' END AS [Air Temp Positive / Negative], CASE WHEN b.DeleteStatus=0 THEN 'Active' ELSE 'Deleted' END AS [ Active Status] from MOperationBirthDetail  a  INNER JOIN MooringRopeDetail b ON a.OPId=b.Id WHERE b.DeleteStatus=0 and b.VesselID=" + searchTerm + "";
                                SqlDataAdapter adapter10 = new SqlDataAdapter(qryy10, sc.con);
                                DataTable dttt10 = new DataTable();
                                adapter10.Fill(dttt10);
                                dttt10.TableName = "Mooring Op Berth Details";

                                string qryy11 = "Select a.OperationID AS [Operation ID],a.GridID AS [Grid ID],ISNULL(Convert(varchar,a.OPDateFrom ,106),'N/A') AS [Fast Date-Time],ISNULL(Convert(varchar,a.OPDateTo,106),'N/A') AS [Cast Off Date-Time],ISNULL(e.assignednumber,'N/A') AS [Winch Name],b.UniqueId AS [Unique Id], b.certificatenumber AS [Certificate Number],ISNULL(e.Location,'N/A') AS [Winch Location],a.RunningHours,ISNULL(Convert(varchar,a.NotificationId,106),'N/A') AS [Notification Id],ISNULL(a.Lead,'N/A') AS [Lead Direction],ISNULL(a.Lead1,'N/A') AS [Lead Type] ,Case when a.Outboard = 0 then 'A' else 'B' end as Outboard,Case when b.DeleteStatus = 0 then 'Active' else 'Deleted' end as [Active Status]  from MOUsedWinchTbl a LEFT join MooringRopeDetail b on a.RopeId=b.RopeId and a.VesselID= b.VesselID LEFT join MooringWinchDetail e on a.WinchId=e.Id and e.VesselID= a.VesselID where b.DeleteStatus=0 and b.RopeTail=1 and a.VesselID=" + searchTerm + "";
                                SqlDataAdapter adapter11 = new SqlDataAdapter(qryy11, sc.con);
                                DataTable dttt11 = new DataTable();
                                adapter11.Fill(dttt11);
                                dttt11.TableName = "Mooring Op Tail Used";

                                using (XLWorkbook wb = new XLWorkbook())
                                {
                                    var protectedsheet = wb.Worksheets.Add(dttt);
                                    var protectedsheet1 = wb.Worksheets.Add(dttt1);
                                    var protectedsheet2 = wb.Worksheets.Add(dttt2);
                                    var protectedsheet3 = wb.Worksheets.Add(dttt3);
                                    var protectedsheet4 = wb.Worksheets.Add(dttt4);
                                    var protectedsheet5 = wb.Worksheets.Add(dttt5);
                                    var protectedsheet6 = wb.Worksheets.Add(dttt6);
                                    var protectedsheet7 = wb.Worksheets.Add(dttt7);
                                    var protectedsheet8 = wb.Worksheets.Add(dttt8);
                                    var protectedsheet9 = wb.Worksheets.Add(dttt9);
                                    var protectedsheet10 = wb.Worksheets.Add(dttt10);
                                    var protectedsheet11 = wb.Worksheets.Add(dttt11);

                                    var projection = protectedsheet.Protect("49WEB$TREET#");
                                    projection.InsertColumns = true;
                                    projection.InsertRows = true;

                                    wb.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                                    wb.Style.Font.Bold = true;

                                    DateTime today = DateTime.Today;
                                    //string vsname = searchTerm.Replace(" ", "");
                                    //string HeaderName = "Work-Ship_Export_" + vsname + "_" + today.ToString("dd-MMM-yyyy");

                                    Response.Clear();
                                    Response.BufferOutput = true;
                                    Response.Charset = "";
                                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                                    Response.AddHeader("content-disposition", "attachment;filename=DigiMoorX7_Export_" + searchTerm + "_" + "RopeTailDetails".Replace(" ", "") + ".xlsx");

                                    using (MemoryStream MyMemoryStream = new MemoryStream())
                                    {
                                        wb.SaveAs(MyMemoryStream);
                                        MyMemoryStream.WriteTo(Response.OutputStream);
                                        Response.End();
                                    }

                                    Response.Clear();

                                    Thread.Sleep(300);
                                }

                            }
                            else if (download == "LooseEquipment")
                            {
                                string qrry = "Select UniqueId AS [Unique Id],CertificateNumber AS [Certificate Number],ISNULL(IdentificationNumber,'N/A') AS [Identification Number],ISNULL(ManufactureName,'N/A') AS [Manufacturer],ISNULL(Convert(varchar,MBL,106),'N/A') AS [MBL (T)],ISNULL(Type,'N/A') AS [Type],DateReceived AS [Date Received],ISNULL(Convert(varchar,DateInstalled,106),'N/A') AS [Date Installed],ISNULL(Convert(varchar,OutofServiceDate,106),'N/A') AS [Discarded Date],ISNULL(ReasonOutofService,'N/A') AS [Discarded Reason],ISNULL(OtherReason,'N/A') AS [Discard (Other) Reason],ISNULL(Convert(varchar,InspectionDueDate,106),'N/A') AS [Inspection Due Date] ,ISNULL(Remarks,'N/A') AS [Remarks],CASE WHEN DeleteStatus=0  THEN 'Active' ELSE 'Deleted' END AS [Active Status] From JoiningShackle  where DeleteStatus=0 and VesselID=" + searchTerm + "";
                                SqlDataAdapter addp = new SqlDataAdapter(qrry, sc.con);
                                DataTable dt = new DataTable();
                                addp.Fill(dt);
                                dt.TableName = "Joining Shackle";

                                string qrry1 = "Select a.UniqueID As [Unique ID], a.CertificateNumber AS [CertificateNumber], ISNULL(e.LooseEquipmentType,'N/A') AS [Loose Eq Type],ISNULL(a.ManufactureName,'N/A') AS [Manufacturer],ISNULL(Convert(varchar,a.MBL,106),'N/A') AS [MBL (T)],a.Length AS [Length(m)],ISNULL(Convert(varchar,a.DateReceived,106),'N/A') AS [Date Received],ISNULL(Convert(varchar,a.DateInstalled,106),'N/A') AS [Date Installed],ISNULL(Convert(varchar,a.OutofServiceDate,106),'N/A') AS [Discarded Date],ISNULL(a.ReasonOutofService,'N/A') AS [Discarded Reason],ISNULL(a.OtherReason,'N/A') AS[Discard (Other) Reason],ISNULL(Convert(varchar,a.InspectionDueDate,106),'N/A') AS [Inspection Due Date],ISNULL(a.Remarks,'N/A') AS [Remarks], CASE WHEN DeleteStatus=0 THEN 'Active' ELSE 'Deleted' END AS [Active Status] From ChainStopper a INNER JOIN  LooseEType e ON a.LooseETypeId=e.Id Where DeleteStatus=0 and VesselID=" + searchTerm + "";
                                SqlDataAdapter addp1 = new SqlDataAdapter(qrry1, sc.con);
                                DataTable dt1 = new DataTable();
                                addp1.Fill(dt1);
                                dt1.TableName = "Chain Stopper";

                                string qrry2 = "Select m.UniqueID AS [Unique ID],a.CertificateNumber AS [CertificateNumber],ISNULL(a.ManufactureName,'N/A') AS [Manufacturer],ISNULL(e.LooseEquipmentType,'N/A') AS [Loose Eq Type],ISNULL(a.RopeConstruction,'N/A')AS [Line Construction],a.Diameter AS [Diameter (mm)], a.Length AS [Length (m)], a.MBL AS [MBL (T)],a.LDBF AS [LDBF (T)],a.WLL AS [WLL (T)],ISNULL(Convert(varchar,a.ReceivedDate,106),'N/A') AS [Received Date],ISNULL(Convert(varchar,a.InstalledDate,106),'N/A') AS [Installed Date],a.RopeTagging AS [Tagging Done],ISNULL(Convert(varchar,a.OutofServiceDate,106),'N/A') AS [Discarded Date],ISNULL(a.ReasonOutofService,'N/A') AS [Discard Reason],ISNULL(a.OtherReason,'N/A') AS [Discard (Other) Reason],ISNULL(Convert(varchar,a.InspectionDueDate,106),'N/A') AS [Inspection Due Date],ISNULL(a.Remarks,'N/A') AS [Remarks],CASE WHEN m.DeleteStatus=0 THEN 'Active' ELSE 'Deleted' END AS [Active Status] from RopeTail a INNER JOIN LooseEType e ON a.LooseETypeId=e.Id INNER JOIN MooringRopeDetail m ON a.LooseETypeId=m.Id Where  m.DeleteStatus=0 and a.VesselID=" + searchTerm + "";
                                SqlDataAdapter addp2 = new SqlDataAdapter(qrry2, sc.con);
                                DataTable dt2 = new DataTable();
                                addp2.Fill(dt2);
                                dt2.TableName = "Various Loose Eq.";

                                string qryy3 = "Select UniqueID AS [Unique ID],CertificateNumber AS [CertificateNumber],ISNULL(e.LooseEquipmentType,'N/A') AS [Loose Eq Type],a.InspectBy As [Inspect By],ISNULL(Convert(varchar,a.InspectDate,106),'N/A') AS [Inspect Date],ISNULL(Convert(varchar,a.Number,106),'N/A') AS [Number],ISNULL(a.Condition,'N/A') AS [Condition],ISNULL(a.Remarks,'N/A') AS [Remarks],ISNULL(Convert(varchar,a.NotificationId,106),'N/A') AS [Notification Id] ,ISNULL(a.Image1,'N/A') AS [Photo1] ,ISNULL(a.Image2,'N/A') AS [Photo2], CASE WHEN m.DeleteStatus=0 THEN 'Active' ELSE 'Deleted' END AS [Active Status] from MooringLooseEquipInspection a INNER JOIN LooseEType e ON a.LooseETypeId=e.Id INNER JOIN MooringRopeDetail m ON a.LooseETypeId=m.Id Where m.DeleteStatus=0 and a.VesselID=" + searchTerm + "";
                                SqlDataAdapter addp3 = new SqlDataAdapter(qryy3, sc.con);
                                DataTable dt3 = new DataTable();
                                addp3.Fill(dt3);
                                dt3.TableName = "Loose Eq. Inspection";

                                string qrry4 = "Select m.UniqueID AS [Unique ID],a.CertificateNumber AS [CertificateNumber],ISNULL(e.LooseEquipmentType,'N/A') AS [Loose Eq Type],ISNULL(Convert(varchar,a.DamageDate,106),'N/A') AS [Damage Date],ISNULL(a.DamageObserved,'N/A') AS [Damage Observed], ISNULL(DamageReason,'N/A') AS [Damage Reason],ISNULL(a.IncidentReport,'N/A') AS [Incident Reported],ISNULL(Convert(varchar,a.MOpId,106),'N/A') AS [Mooring Op Id],ISNULL(Convert(varchar,a.NotificationId,106),'N/A') AS [Notification Id],CASE WHEN m.DeleteStatus=0 THEN 'Active' ELSE 'Deleted' END AS [Active Status] from LooseEDamageRecord a INNER JOIN LooseEType e ON a.LooseETypeId=e.Id INNER JOIN MooringRopeDetail m ON a.LooseETypeId=m.Id Where m.DeleteStatus=0 and a.VesselID=" + searchTerm + "";
                                SqlDataAdapter addp4 = new SqlDataAdapter(qrry4, sc.con);
                                DataTable dt4 = new DataTable();
                                addp4.Fill(dt4);
                                dt4.TableName = "Loose Eq Damage";

                                string qrry5 = "Select m.UniqueID AS [Unique ID],a.LooseECertiNo AS [CertificateNumber],ISNULL(e.LooseEquipmentType,'N/A') AS [Loose Eq Type],ISNULL(a.DisposalPortName,'N/A') AS [Disposal Port Name],ISNULL(a.ReceptionFacilityName,'N/A') AS [Reception Facility Name], ISNULL(Convert(varchar,a.DisposalDate,106),'N/A') AS [Disposal Date],CASE WHEN m.DeleteStatus=0 THEN 'Active' ELSE 'Deleted' END AS [Active Status] from LooseEDisposal a INNER JOIN LooseEType e ON a.LooseETypeId=e.Id INNER JOIN MooringRopeDetail m ON a.LooseETypeId=m.Id Where m.DeleteStatus=0 and a.VesselID=" + searchTerm + "";
                                SqlDataAdapter addp5 = new SqlDataAdapter(qrry5, sc.con);
                                DataTable dt5 = new DataTable();
                                addp5.Fill(dt5);
                                dt5.TableName = "Loose Eq Disposal";

                                using (XLWorkbook wb = new XLWorkbook())
                                {
                                    var protectedsheet = wb.Worksheets.Add(dt);
                                    var protectedsheet1 = wb.Worksheets.Add(dt1);
                                    var protectedsheet2 = wb.Worksheets.Add(dt2);
                                    var protectedsheet3 = wb.Worksheets.Add(dt3);
                                    var protectedsheet4 = wb.Worksheets.Add(dt4);
                                    var protectedsheet5 = wb.Worksheets.Add(dt5);

                                    var projection = protectedsheet.Protect("49WEB$TREET#");
                                    projection.InsertColumns = true;
                                    projection.InsertRows = true;

                                    wb.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                                    wb.Style.Font.Bold = true;

                                    DateTime today = DateTime.Today;
                                    //string vsname = searchTerm.Replace(" ", "");
                                    //string HeaderName = "Work-Ship_Export_" + vsname + "_" + today.ToString("dd-MMM-yyyy");

                                    Response.Clear();
                                    Response.BufferOutput = true;
                                    Response.Charset = "";
                                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                                    Response.AddHeader("content-disposition", "attachment;filename=DigiMoorX7_Export_" + searchTerm + "_" + "LooseEquipment".Replace(" ", "") + ".xlsx");

                                    using (MemoryStream MyMemoryStream = new MemoryStream())
                                    {
                                        wb.SaveAs(MyMemoryStream);
                                        MyMemoryStream.WriteTo(Response.OutputStream);
                                        Response.End();
                                    }

                                    Response.Clear();

                                    Thread.Sleep(300);
                                }

                            }
                            else if(download=="Attachment")
                            {
                                bmsuploaddats.bmsname = "Please press Export Attachment button";
                            }
                        }
                    }
                    else
                    {
                        bmsuploaddats.bmsname = "Please select vessel name first";
                    }
                }
                else if (submit == "Export Attachment")
                {
                    if (download == "Line")
                    {
                        string qry = "Select Image1,Image2 from MooringRopeInspection where VesselID = " + searchTerm + " and RopeTail=0";
                        SqlDataAdapter adp = new SqlDataAdapter(qry, sc.con);
                        DataTable dt = new DataTable();
                        adp.Fill(dt);

                        if (dt.Rows.Count > 0)
                        {
                            string startPath = Server.MapPath(string.Format("~/images/Lines"));
                            if (!Directory.Exists(startPath))
                            {
                                Directory.CreateDirectory(startPath);
                            }
                            for (int i = 0; i < dt.Rows.Count; i++)
                            {
                                var filename1 = dt.Rows[i]["Image1"].ToString();
                                var filename2 = dt.Rows[i]["Image2"].ToString();

                                string sourcePath = Server.MapPath(string.Format("~/images/InspectionImages/" + filename1));
                                string targetPath = Server.MapPath(string.Format("~/images/Lines/" + filename1));

                                string sourcePath2 = Server.MapPath(string.Format("~/images/InspectionImages/" + filename2));
                                string targetPath2 = Server.MapPath(string.Format("~/images/Lines/" + filename2));

                                if (System.IO.File.Exists(sourcePath))
                                {
                                    System.IO.File.Copy(sourcePath, targetPath, true);
                                }

                                if (System.IO.File.Exists(sourcePath2))
                                {
                                    System.IO.File.Copy(sourcePath2, targetPath2, true);
                                }
                            }
                            //Download Attachments in Zip File
                            if (Directory.Exists(startPath))
                            {
                                string[] fileFound = Directory.GetFiles(startPath);
                                if (fileFound.Length > 0)
                                {
                                    string zipFileName = string.Format("{0}_{1}.zip", shipIMO, DateTime.Now.Ticks);
                                    string zipPath = Server.MapPath("~/images/" + zipFileName);
                                    ZipFile.CreateFromDirectory(startPath, zipPath, CompressionLevel.Fastest, true);
                                    Response.ContentType = "application.zip";
                                    Response.AppendHeader("Content-Disposition", "attachment;filename=" + zipFileName);
                                    Response.TransmitFile(zipPath);
                                    Response.End();

                                    if (System.IO.File.Exists(zipPath))
                                    {
                                        System.IO.File.Delete(zipPath);
                                    }

                                  

                                }
                                else
                                {
                                    bmsuploaddats.bmsname = "No Attachments found.";
                                }

                                new System.IO.DirectoryInfo(startPath).Delete(true);
                               
                            }
                        }
                    }
                    else
                    {
                        bmsuploaddats.bmsname = "No Attachment found";
                    }
                    if (download == "RopeTail")
                    {
                        string qry = "Select Image1,Image2 from MooringRopeInspection Where VesselID=" + searchTerm + " and RopeTail=1";
                        SqlDataAdapter adp = new SqlDataAdapter(qry, sc.con);
                        DataTable dt = new DataTable();
                        adp.Fill(dt);
                        if (dt.Rows.Count > 0)
                        {
                            string startPath = Server.MapPath(string.Format("~/images/RopeTail"));
                            if (!Directory.Exists(startPath))
                            {
                                Directory.CreateDirectory(startPath);
                            }
                            for (int i = 0; i < dt.Rows.Count; i++)
                            {
                                var filename1 = dt.Rows[i]["Image1"].ToString();
                                var filename2 = dt.Rows[i]["Image2"].ToString();

                                string sourcePath = Server.MapPath(string.Format("~/images/InspectionImages/" + filename1));
                                string targetPath = Server.MapPath(string.Format("~/images/RopeTail/" + filename1));

                                string sourcePath2 = Server.MapPath(string.Format("~/images/InspectionImages/" + filename2));
                                string targetPath2 = Server.MapPath(string.Format("~/images/RopeTail/" + filename2));

                                if (System.IO.File.Exists(sourcePath))
                                {
                                    System.IO.File.Copy(sourcePath, targetPath, true);
                                }

                                if (System.IO.File.Exists(sourcePath2))
                                {
                                    System.IO.File.Copy(sourcePath2, targetPath2, true);
                                }
                            }
                            //Download Attachments in Zip File
                            if (Directory.Exists(startPath))
                            {
                                string[] fileFound = Directory.GetFiles(startPath);
                                if (fileFound.Length > 0)
                                {
                                    string zipFileName = string.Format("{0}_{1}.zip", shipIMO, DateTime.Now.Ticks);
                                    string zipPath = Server.MapPath("~/images/" + zipFileName);
                                    ZipFile.CreateFromDirectory(startPath, zipPath, CompressionLevel.Fastest, true);
                                    Response.ContentType = "application.zip";
                                    Response.AppendHeader("Content-Disposition", "attachment;filename=" + zipFileName);
                                    Response.TransmitFile(zipPath);
                                    Response.End();

                                    //Delete Zip folder
                                    if (System.IO.File.Exists(zipPath))
                                    {
                                        System.IO.File.Delete(zipPath);
                                    }



                                }
                                else
                                {
                                    bmsuploaddats.bmsname = "No Attachments found.";
                                }
                                    //Delete Directory Folder
                                new System.IO.DirectoryInfo(startPath).Delete(true);
                            }
                        }
                    }
                    else
                    {
                        bmsuploaddats.bmsname = "No Attachment found";
                    }

                    if (download == "LooseEquipment")
                    {
                        string qry = "Select Image1,Image2 from MooringLooseEquipInspection Where VesselID=" + searchTerm + "";
                        SqlDataAdapter adp = new SqlDataAdapter(qry, sc.con);
                        DataTable dt = new DataTable();
                        adp.Fill(dt);
                        if (dt.Rows.Count > 0)
                        {
                            string startPath = Server.MapPath(string.Format("~/images/LooseEquipment"));
                            if (!Directory.Exists(startPath))
                            {
                                Directory.CreateDirectory(startPath);
                            }
                            for (int i = 0; i < dt.Rows.Count; i++)
                            {
                                var filename1 = dt.Rows[i]["Image1"].ToString();
                                var filename2 = dt.Rows[i]["Image2"].ToString();

                                string sourcePath = Server.MapPath(string.Format("~/images/InspectionImages/" + filename1));
                                string targetPath = Server.MapPath(string.Format("~/images/LooseEquipment/" + filename1));

                                string sourcePath2 = Server.MapPath(string.Format("~/images/InspectionImages/" + filename2));
                                string targetPath2 = Server.MapPath(string.Format("~/images/LooseEquipment/" + filename2));

                                if (System.IO.File.Exists(sourcePath))
                                {
                                    System.IO.File.Copy(sourcePath, targetPath, true);
                                }

                                if (System.IO.File.Exists(sourcePath2))
                                {
                                    System.IO.File.Copy(sourcePath2, targetPath2, true);
                                }
                            }
                            //Download Attachments in Zip File
                            if (Directory.Exists(startPath))
                            {
                                string[] fileFound = Directory.GetFiles(startPath);
                                if (fileFound.Length > 0)
                                {
                                    string zipFileName = string.Format("{0}_{1}.zip", shipIMO, DateTime.Now.Ticks);
                                    string zipPath = Server.MapPath("~/images/" + zipFileName);
                                    ZipFile.CreateFromDirectory(startPath, zipPath, CompressionLevel.Fastest, true);
                                    Response.ContentType = "application.zip";
                                    Response.AppendHeader("Content-Disposition", "attachment;filename=" + zipFileName);
                                    Response.TransmitFile(zipPath);
                                    Response.End();

                                    if (System.IO.File.Exists(zipPath))
                                    {
                                        System.IO.File.Delete(zipPath);
                                    }
                                }
                                else
                                {
                                    bmsuploaddats.bmsname = "No Attachments found.";
                                }

                                new System.IO.DirectoryInfo(startPath).Delete(true);
                            }
                        }
                    }
                    else
                    {
                        bmsuploaddats.bmsname = "No Attachment found";
                    }

                    if (download == "Attachment")
                    {
                        string startPath1 = "";


                        startPath1 = Server.MapPath(string.Format("~/images/Attachments"));
                        if (!Directory.Exists(startPath1))
                        {
                            Directory.CreateDirectory(startPath1);
                        }

                        string qry = "select * from mooringropeattachment where LineResidual='Line' and VesselId= " + searchTerm + "";
                        SqlDataAdapter adp = new SqlDataAdapter(qry, sc.con);
                        DataTable dt = new DataTable();
                        adp.Fill(dt);

                        if (dt.Rows.Count > 0)
                        {
                            string startPath = Server.MapPath(string.Format("~/images/Attachments/LinesAttachment"));
                            if (!Directory.Exists(startPath))
                            {
                                Directory.CreateDirectory(startPath);
                            }
                            for (int i = 0; i < dt.Rows.Count; i++)
                            {
                                var filename1 = dt.Rows[i]["AttachmentPath"].ToString();
                           
                                string sourcePath = Server.MapPath(string.Format("~/images/AttachFiles/" + filename1));
                                string targetPath = Server.MapPath(string.Format("~/images/Attachments/LinesAttachment/" + filename1));

                                if (System.IO.File.Exists(sourcePath))
                                {
                                    System.IO.File.Copy(sourcePath, targetPath, true);
                                }
                            }
                           
                        }

                        string qry1 = "select * from mooringropeattachment where LineResidual='RopeTail' and VesselId= " + searchTerm + "";
                        SqlDataAdapter adp1 = new SqlDataAdapter(qry1, sc.con);
                        DataTable dt1 = new DataTable();
                        adp1.Fill(dt1);

                        if (dt1.Rows.Count > 0)
                        {
                            string startPath = Server.MapPath(string.Format("~/images/Attachments/RopeTailAttachment"));
                            if (!Directory.Exists(startPath))
                            {
                                Directory.CreateDirectory(startPath);
                            }
                            for (int i = 0; i < dt1.Rows.Count; i++)
                            {
                                var filename1 = dt1.Rows[i]["AttachmentPath"].ToString();

                                string sourcePath = Server.MapPath(string.Format("~/images/AttachFiles/" + filename1));
                                string targetPath = Server.MapPath(string.Format("~/images/Attachments/RopeTailAttachment/" + filename1));

                                if (System.IO.File.Exists(sourcePath))
                                {
                                    System.IO.File.Copy(sourcePath, targetPath, true);
                                }
                            }
                            
                        }

                        string qry11 = "select * from mooringropeattachment where LineResidual='ResidualLine' and VesselId= " + searchTerm + "";
                        SqlDataAdapter adp11 = new SqlDataAdapter(qry11, sc.con);
                        DataTable dt11 = new DataTable();
                        adp11.Fill(dt11);

                        if (dt11.Rows.Count > 0)
                        {
                            string startPath = Server.MapPath(string.Format("~/images/Attachments/ResidualLineAttachment"));
                            if (!Directory.Exists(startPath))
                            {
                                Directory.CreateDirectory(startPath);
                            }
                            for (int i = 0; i < dt11.Rows.Count; i++)
                            {
                                var filename1 = dt11.Rows[i]["AttachmentPath"].ToString();

                                string sourcePath = Server.MapPath(string.Format("~/images/AttachFiles/" + filename1));
                                string targetPath = Server.MapPath(string.Format("~/images/Attachments/ResidualLineAttachment/" + filename1));

                                if (System.IO.File.Exists(sourcePath))
                                {
                                    System.IO.File.Copy(sourcePath, targetPath, true);
                                }
                            }
                            
                        }

                        string qry111 = "select * from mooringropeattachment where LineResidual='ResidualRopeTail' and VesselId= " + searchTerm + "";
                        SqlDataAdapter adp111 = new SqlDataAdapter(qry111, sc.con);
                        DataTable dt111 = new DataTable();
                        adp111.Fill(dt111);

                        if (dt111.Rows.Count > 0)
                        {
                           string  startPath = Server.MapPath(string.Format("~/images/Attachments/ResidualRopeTailAttachment"));
                            if (!Directory.Exists(startPath))
                            {
                                Directory.CreateDirectory(startPath);
                            }
                            for (int i = 0; i < dt111.Rows.Count; i++)
                            {
                                var filename1 = dt111.Rows[i]["AttachmentPath"].ToString();

                                string sourcePath = Server.MapPath(string.Format("~/images/AttachFiles/" + filename1));
                                string targetPath = Server.MapPath(string.Format("~/images/Attachments/ResidualRopeTailAttachment/" + filename1));

                                if (System.IO.File.Exists(sourcePath))
                                {
                                    System.IO.File.Copy(sourcePath, targetPath, true);
                                }
                            }
                          
                        }



                       // Download Attachments in Zip File
                        if (Directory.Exists(startPath1))
                        {
                            string[] fileFound = Directory.GetDirectories(startPath1);
                            if (fileFound.Length > 0)
                            {
                                string zipFileName = string.Format("{0}_{1}.zip", shipIMO, DateTime.Now.Ticks);
                                string zipPath = Server.MapPath("~/images/" + zipFileName);
                                ZipFile.CreateFromDirectory(startPath1, zipPath, CompressionLevel.Fastest, true);
                                Response.ContentType = "application.zip";
                                Response.AppendHeader("Content-Disposition", "attachment;filename=" + zipFileName);
                                Response.TransmitFile(zipPath);
                                Response.End();

                                if (System.IO.File.Exists(zipPath))
                                {
                                    System.IO.File.Delete(zipPath);
                                }
                            }
                            else
                            {
                                bmsuploaddats.bmsname = "No Attachments found.";
                            }

                            new System.IO.DirectoryInfo(startPath1).Delete(true);

                        }
                    }
                    else
                    {
                        bmsuploaddats.bmsname = "No Attachment found";
                    }
                }
            }            
            return View(bmsuploaddats);
        }

        private DataTable LINQResultToDataTable<T>(IEnumerable<T> Linqlist)
        {
            PropertyDescriptorCollection props = TypeDescriptor.GetProperties(typeof(T));

            DataTable table = new DataTable();
            for (int i = 0; i < props.Count; i++)
            {
                PropertyDescriptor prop = props[i];

                if (i == 1)
                {
                    DataColumn dataColumn = new DataColumn(prop.Name, prop.PropertyType);
                    table.Columns.Add(dataColumn);
                }
                else
                    table.Columns.Add(prop.Name, prop.PropertyType);
            }
            object[] values = new object[props.Count];
            foreach (T item in Linqlist)
            {
                for (int i = 0; i < values.Length; i++)
                {
                    values[i] = props[i].GetValue(item);
                }
                table.Rows.Add(values);
            }

            return table;
        }

        public ActionResult AutoCompleteim(string term)
        {
            //if (UserRole.username.ToLower() != "admin")
            //{ 
            //    var vsname = sc.Users.Where(x => x.EmailId.Equals(UserRole.username1)).Select(x => x.VesselName).FirstOrDefault();
            //    string[] vesselname1 = vsname.TrimEnd(',').Split(',');

            //    List<string> students = await sc.Vessels.Where(s => s.VesselName.ToLower().Contains(term.ToLower()))
            //     .Select(x => x.VesselName).Distinct().Where(x => vesselname1.Contains(x)).ToListAsync();

            //    return Json(students, JsonRequestBehavior.AllowGet);
            //}
            //else
            //{
            //    List<string> students = await sc.Vessels.Where(s => s.VesselName.ToLower().Contains(term.ToLower()))
            //        .Select(x => x.VesselName).Distinct().ToListAsync();
            //    return Json(students, JsonRequestBehavior.AllowGet);
            //}

            List<string> vessells = base.PermittedVessels.Where(s => s.VesselName.IndexOf(term, StringComparison.OrdinalIgnoreCase) >= 0).Select(p => p.VesselName).Distinct().ToList();

            return Json(vessells, JsonRequestBehavior.AllowGet);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                sc.Dispose();

            base.Dispose(disposing);
        }

        public void ExportdataInExcel(string searchname, int vesseId)
        {
            string[] tablename = new string[1];
            tablename[0] = "tblSmartMenus";
            //tablename[1] = "TblSubMenu";
            //tablename[2] = "TblSubSubMenu";

            DataSet ds = new DataSet();
            for (int i = 0; i < tablename.Length; i++)
            {
                string test = (tablename[i]);
                string constr = ConfigurationManager.ConnectionStrings["ShipmentContaxt"].ConnectionString;
                string query = "select * from " + test + " ";
                //query += "select * from DocsPages";
                using (SqlConnection con = new SqlConnection(constr))
                {
                    using (SqlCommand cmd = new SqlCommand(query))
                    {
                        using (SqlDataAdapter sda = new SqlDataAdapter())
                        {
                            cmd.Connection = con;
                            sda.SelectCommand = cmd;
                            using (ds)
                            {
                                sda.Fill(ds);

                                //Set Name of DataTables.
                                //ds.Tables[0].TableName = "revision";
                                //ds.Tables[1].TableName = "DocsPages";
                                ds.Tables[i].TableName = test;
                            }
                        }
                    }
                }

            }

            using (XLWorkbook wb = new XLWorkbook())
            {
                foreach (DataTable dt in ds.Tables)
                {
                    //Add DataTable as Worksheet.
                    wb.Worksheets.Add(dt);
                }

                //Export the Excel file.

                DateTime today = DateTime.Today;
                string vsname = searchname.Replace(" ", "");
                string HeaderName = "Work-Ship_Export" + "_" + vsname + "_" + today.ToString("dd-MMM-yyyy");

                Response.Clear();
                Response.Buffer = true;
                Response.Charset = "";
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                //Response.AddHeader("content-disposition", "attachment;filename=DataSet.xlsx");
                Response.AddHeader("content-disposition", "attachment;filename= " + HeaderName + ".xlsx");
                using (MemoryStream MyMemoryStream = new MemoryStream())
                {
                    wb.SaveAs(MyMemoryStream);
                    MyMemoryStream.WriteTo(Response.OutputStream);
                    Response.Flush();
                    Response.End();
                }
            }
            //return View();
        }

        public void ImportData(DataTable dataTable, string destinationTableName, int vesselID, out string errorMessage)
        {
            errorMessage = string.Empty;

            try
            {
                List<string> excludedTables = new List<string>
                        {
                            "MOORINGROPETYPE",
                            "LOOSEETYPE"
                        };

                List<string> uniqueIDTables = new List<string>
                        {
                            "CHAINSTOPPER",
                            "JOININGSHACKLE",
                            "MOORINGROPEDETAIL",
                            "ROPETAIL"
                        };

                if (uniqueIDTables.Contains(destinationTableName.ToUpper()))
                {
                    if (!dataTable.Columns.Contains("UniqueID"))
                    {
                        DataColumn colUniqueID = dataTable.Columns.Add("UniqueID", typeof(string));
                        colUniqueID.MaxLength = 50;
                    }
                }

                if (destinationTableName.Equals("CHAFEGUARD", StringComparison.OrdinalIgnoreCase))
                {
                    if (!dataTable.Columns.Contains("ReceivedDate"))
                        dataTable.Columns.Add("ReceivedDate", typeof(DateTime));

                    if (!dataTable.Columns.Contains("UniqueID"))
                    {
                        DataColumn colUniqueID = dataTable.Columns.Add("UniqueID", typeof(string));
                        colUniqueID.MaxLength = 50;
                    }

                    if (!dataTable.Columns.Contains("ReasonOutOfService"))
                    {
                        DataColumn colReasonOutOfService = dataTable.Columns.Add("ReasonOutOfService", typeof(string));
                        colReasonOutOfService.MaxLength = 50;
                    }

                    if (!dataTable.Columns.Contains("OtherReason"))
                    {
                        DataColumn colOtherReason = dataTable.Columns.Add("OtherReason", typeof(string));
                        colOtherReason.MaxLength = 100;
                    }


                }


                if (destinationTableName.Equals("WINCHBREAKTESTKIT", StringComparison.OrdinalIgnoreCase) && !dataTable.Columns.Contains("UniqueID"))
                {
                    dataTable.Columns.Add("ReceivedDate", typeof(DateTime));
                    DataColumn colUniqueID = dataTable.Columns.Add("UniqueID", typeof(string));
                    colUniqueID.MaxLength = 50;

                    DataColumn colReasonOutOfService = dataTable.Columns.Add("ReasonOutOfService", typeof(string));
                    colReasonOutOfService.MaxLength = 50;

                    DataColumn colOtherReason = dataTable.Columns.Add("OtherReason", typeof(string));
                    colOtherReason.MaxLength = 100;
                }

                //======================================= new code ================

                //if (destinationTableName.Equals("AssignRopeToWinch", StringComparison.OrdinalIgnoreCase) && !dataTable.Columns.Contains("IsDelete"))
                //{
                //    dataTable.Columns.Add("IsDelete", typeof(bool));

                //DataColumn colUniqueID = dataTable.Columns.Add("UniqueID", typeof(string));
                //colUniqueID.MaxLength = 50;

                //DataColumn colReasonOutOfService = dataTable.Columns.Add("ReasonOutOfService", typeof(string));
                //colReasonOutOfService.MaxLength = 50;

                //DataColumn colOtherReason = dataTable.Columns.Add("OtherReason", typeof(string));
                //colOtherReason.MaxLength = 100;
                //}
                //================================================================




                string connectionString = Convert.ToString(ConfigurationManager.ConnectionStrings["ShipmentContaxt"]);

                if (dataTable.Rows.Count > 0)
                {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        if (connection.State != ConnectionState.Open)
                            connection.Open();

                        if (destinationTableName.Equals("MOORINGROPEDETAIL", StringComparison.OrdinalIgnoreCase))
                        {
                            DataColumn columnID = new DataColumn
                            {
                                ColumnName = "Id"
                            };

                            dataTable.Columns.Add(columnID);
                            columnID.SetOrdinal(0);

                            bool colAddedRunTime = false; bool colAddedRunTime2 = false;

                            if (!dataTable.Columns.Contains("StartCounterHours"))
                            {
                                DataColumn columnCounters = new DataColumn
                                {
                                    ColumnName = "StartCounterHours"
                                };

                                colAddedRunTime = true;

                                dataTable.Columns.Add(columnCounters);
                                columnCounters.SetOrdinal(dataTable.Columns.Count - 3);
                            }

                            if (!dataTable.Columns.Contains("CurrentLeadRunningHours"))
                            {
                                DataColumn columnCounters = new DataColumn
                                {
                                    ColumnName = "CurrentLeadRunningHours"
                                };

                                colAddedRunTime2 = true;

                                dataTable.Columns.Add(columnCounters);
                                // columnCounters.SetOrdinal(dataTable.Columns.Count - 2);
                            }

                            DataColumn columnCost = new DataColumn
                            {
                                ColumnName = "CostUsd"
                            };

                            dataTable.Columns.Add(columnCost);
                            //columnCost.SetOrdinal(dataTable.Columns.Count - 2);

                            int ID = 0;
                            using (SqlCommand cmd = new SqlCommand(string.Format("SELECT TOP 1 Id FROM {0} ORDER BY Id DESC", destinationTableName)))
                            {
                                cmd.CommandType = CommandType.Text;
                                cmd.Connection = connection;
                                ID = Convert.ToInt32(cmd.ExecuteScalar());
                            }

                            foreach (DataRow row in dataTable.Rows)
                            {
                                if (colAddedRunTime)
                                    row["StartCounterHours"] = 0;

                                if (colAddedRunTime2)
                                    row["CurrentLeadRunningHours"] = 0;

                                row["CostUsd"] = 0;
                                row["Id"] = ID + 1;

                                ID++;
                            }
                        }

                        else if (destinationTableName.Equals("Notifications", StringComparison.OrdinalIgnoreCase))
                        {
                            if (!dataTable.Columns.Contains("MOP_Id"))
                            {
                                DataColumn MOP_Id = new DataColumn
                                {
                                    ColumnName = "MOP_Id"
                                };

                                dataTable.Columns.Add(MOP_Id);
                                MOP_Id.SetOrdinal(dataTable.Columns.Count - 1);

                                DataColumn acknDT = new DataColumn
                                {
                                    ColumnName = "AcknDateTime"
                                };

                                dataTable.Columns.Add(acknDT);
                                // acknDT.SetOrdinal(dataTable.Columns.Count - 2);

                                DataColumn acknBy = new DataColumn
                                {
                                    ColumnName = "AcknBy"
                                };

                                dataTable.Columns.Add(acknBy);
                                // acknBy.SetOrdinal(dataTable.Columns.Count - 2);
                            }

                            else
                            {
                                //After MOP_Id
                                DataColumn acknDT = new DataColumn
                                {
                                    ColumnName = "AcknDateTime"
                                };

                                dataTable.Columns.Add(acknDT);
                                //acknDT.SetOrdinal(dataTable.Columns.Count - 2);

                                DataColumn acknBy = new DataColumn
                                {
                                    ColumnName = "AcknBy"
                                };

                                dataTable.Columns.Add(acknBy);
                                //acknBy.SetOrdinal(dataTable.Columns.Count - 2);
                            }

                            foreach (DataRow row in dataTable.Rows)
                            {
                                string mop = row["MOP_Id"].ToString();
                                if (string.IsNullOrEmpty(mop))
                                    row["MOP_Id"] = 0;

                            }

                        }
                        else if (destinationTableName.Equals("NOTIFICATIONCOMMENT", StringComparison.OrdinalIgnoreCase))
                        {
                            foreach (DataRow row in dataTable.Rows)
                            {
                                string colValue = Convert.ToString(row["Comments"]);
                                if (string.IsNullOrEmpty(colValue))
                                {
                                    row["Comments"] = string.Empty;
                                }
                                //else
                                //{
                                //    row["Comments"].ToString();
                                //}

                            }
                        }

                        if (destinationTableName.Equals("AssignRopeToWinch", StringComparison.OrdinalIgnoreCase) && !dataTable.Columns.Contains("IsDelete"))
                        {
                            //DataColumn IsDelete = new DataColumn
                            //{
                            //    ColumnName = "IsDelete"

                            //};

                            //dataTable.Columns.Add(IsDelete);
                            ////columnID.SetOrdinal(0);

                            dataTable.Columns.Add("IsDelete", typeof(bool));

                            foreach (DataRow row in dataTable.Rows)
                            {
                                row["IsDelete"] = false;

                            }
                        }

                        if (destinationTableName.Equals("MooringWinchDetail", StringComparison.OrdinalIgnoreCase) && !dataTable.Columns.Contains("Lead"))
                        {

                            dataTable.Columns.Add("Lead", typeof(string));

                        }

                        if (destinationTableName.Equals("MOperationBirthDetail", StringComparison.OrdinalIgnoreCase) && !dataTable.Columns.Contains("AirTempCentigrate"))
                        {

                            dataTable.Columns.Add("AirTempCentigrate", typeof(bool));

                            foreach (DataRow row in dataTable.Rows)
                            {
                                row["AirTempCentigrate"] = false;

                            }
                        }

                        if (destinationTableName.Equals("MOUsedWinchTbl", StringComparison.OrdinalIgnoreCase) && !dataTable.Columns.Contains("Outboard"))
                        {

                            dataTable.Columns.Add("Outboard", typeof(bool));

                            foreach (DataRow row in dataTable.Rows)
                            {
                                row["Outboard"] = false;

                            }
                        }

                        if (destinationTableName.Equals("RopeCropping", StringComparison.OrdinalIgnoreCase) && !dataTable.Columns.Contains("WinchId"))
                        {

                            // dataTable.Columns.Add("WinchId", typeof(bool));
                            DataColumn columnCost = new DataColumn
                            {
                                ColumnName = "WinchId"
                            };

                            dataTable.Columns.Add(columnCost);

                            foreach (DataRow row in dataTable.Rows)
                            {
                                row["WinchId"] = 0;

                            }
                        }

                        if (destinationTableName.Equals("RopeDamageRecord", StringComparison.OrdinalIgnoreCase) && !dataTable.Columns.Contains("WinchId"))
                        {
                            DataColumn columnCost = new DataColumn
                            {
                                ColumnName = "WinchId"
                            };

                            dataTable.Columns.Add(columnCost);

                            foreach (DataRow row in dataTable.Rows)
                            {
                                row["WinchId"] = 0;

                            }
                        }

                        if (destinationTableName.Equals("RopeDisposal", StringComparison.OrdinalIgnoreCase) && !dataTable.Columns.Contains("WinchId"))
                        {
                            DataColumn columnCost = new DataColumn
                            {
                                ColumnName = "WinchId"
                            };

                            dataTable.Columns.Add(columnCost);

                            foreach (DataRow row in dataTable.Rows)
                            {
                                row["WinchId"] = 0;

                            }
                        }

                        if (destinationTableName.Equals("RopeEndtoEnd2", StringComparison.OrdinalIgnoreCase) && !dataTable.Columns.Contains("WinchId"))
                        {
                            DataColumn columnCost = new DataColumn
                            {
                                ColumnName = "WinchId"
                            };

                            dataTable.Columns.Add(columnCost);

                            foreach (DataRow row in dataTable.Rows)
                            {
                                row["WinchId"] = 0;
                            }
                        }

                        if (destinationTableName.Equals("RopeSplicingRecord", StringComparison.OrdinalIgnoreCase) && !dataTable.Columns.Contains("WinchId"))
                        {
                            DataColumn columnCost = new DataColumn
                            {
                                ColumnName = "WinchId"
                            };

                            dataTable.Columns.Add(columnCost);

                            foreach (DataRow row in dataTable.Rows)
                            {
                                row["WinchId"] = 0;
                            }
                        }

                        if (destinationTableName.Equals("LooseEDamageRecord", StringComparison.OrdinalIgnoreCase) && !dataTable.Columns.Contains("Remarks"))
                        {

                            dataTable.Columns.Add("Remarks", typeof(string));

                        }

                        if (!excludedTables.Contains(destinationTableName.ToUpper()))
                        {
                            dataTable.Columns.Add(string.Format("VesselID", dataTable.Columns.Count + 1));
                            foreach (DataRow row in dataTable.Rows)
                                row[dataTable.Columns.Count - 1] = vesselID;
                        }

                        // https://www.aspsnippets.com/Articles/SqlBulkCopy--Bulk-Insert-records-and-Update-existing-rows-if-record-exists-using-C-and-VBNet.aspx


                        if (!destinationTableName.Equals("VersionTable", StringComparison.OrdinalIgnoreCase))
                            using (SqlCommand cmd = new SqlCommand(string.Format("Update_{0}", destinationTableName)))
                            {
                                cmd.CommandType = CommandType.StoredProcedure;
                                cmd.Connection = connection;
                                cmd.Parameters.AddWithValue(string.Format("{0}TableType", destinationTableName), dataTable);
                                cmd.ExecuteNonQuery();
                            }
                    }
                }
            }
            catch (Exception ex)
            {
                List<string> ignoreTables = new List<string>
                        {
                            "PRODUCT_DETAIL",
                            "VERSIONTABLE"
                        };

                if (!ignoreTables.Contains(destinationTableName.ToUpper()))
                {
                    errorMessage = destinationTableName + ", ";
                }

                ErrorSignal.FromCurrentContext().Raise(ex);
            }
        }

        //public async Task<ActionResult> Index(string submit, HttpPostedFileBase photo, string searchTerm, DateTime? ServiceFrom, DateTime? ServiceTo, bmsuploaddat1 bmsuploaddats)
        //{
        //    string fileLocation = "";

        //    if (string.IsNullOrEmpty(searchTerm))
        //    {
        //        bmsuploaddats.bmsname = "Please select vessel name first";
        //        return View();
        //    }
        //    else
        //    {
        //        string shipID = searchTerm;

        //        var vesselData = sc.Vessels.Where(p => p.VesselName == searchTerm).Select(x => new { x.Id, x.VesselName, x.ImoNo, x.Flag, x.FleetName, x.FleetType }).ToList();

        //        if (vesselData.Count > 0)
        //        {
        //            shipID = Convert.ToString(vesselData.First().ImoNo);
        //        }

        //        if (submit == "Import")
        //        {
        //            OleDbConnection excelConnection = new OleDbConnection();
        //            OleDbConnection excelConnection1 = new OleDbConnection();

        //            try
        //            {
        //                DateTime edate = Convert.ToDateTime(DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss"));
        //                string filenames = "";


        //                if (photo != null)
        //                {
        //                    int numberfiles = 0;

        //                    if (Request.Files["photo"].ContentLength > 0)
        //                    {
        //                        string fileExtension = System.IO.Path.GetExtension(Request.Files["photo"].FileName);

        //                        filenames = System.IO.Path.GetFileName(Request.Files["photo"].FileName).Replace(".xls", "").Replace(".xlsx", "");

        //                        if (fileExtension == ".xls" || fileExtension == ".xlsx")
        //                        {
        //                            fileLocation = Server.MapPath("~/Content/") + Request.Files["photo"].FileName;

        //                            if (System.IO.File.Exists(fileLocation))
        //                            {

        //                                System.IO.File.Delete(fileLocation);
        //                            }

        //                            Request.Files["photo"].SaveAs(fileLocation);

        //                            string excelConnectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + fileLocation + ";Extended Properties=Excel 12.0;Persist Security Info=False";
        //                            excelConnection = new OleDbConnection(excelConnectionString);
        //                            excelConnection.Open();
        //                            DataTable dt = new DataTable();

        //                            dt = excelConnection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
        //                            if (dt == null)
        //                            {
        //                                return null;
        //                            }

        //                            String[] excelSheets = new String[dt.Rows.Count];
        //                            int t = 0;
        //                            //excel data saves in temp file here.
        //                            var dtrow = dt.Rows;
        //                            foreach (DataRow row in dtrow)
        //                            {
        //                                excelSheets[t] = row["TABLE_NAME"].ToString();
        //                                t++;
        //                            }

        //                            excelConnection1 = new OleDbConnection(excelConnectionString);
        //                            DataTable odt;
        //                            var exlength = excelSheets.Length;



        //                            for (int i = 0; i < exlength; i++)
        //                            {
        //                                string importsheet = excelSheets[i].ToString();

        //                                if (importsheet.TrimEnd('$') == "AssignRopeToWinch")
        //                                {
        //                                    numberfiles = numberfiles + 1;
        //                                    string query = string.Format("Select * from [{0}]", excelSheets[i]);

        //                                    OleDbDataAdapter olda = new OleDbDataAdapter(query, excelConnection1);
        //                                    odt = new DataTable();
        //                                    odt.TableName = "AssignRopeToWinch";
        //                                    olda.Fill(odt);

        //                                    ImportData(odt, "AssignRopeToWinch", shipID);
        //                                }

        //                                if (importsheet.TrimEnd('$') == "CrewDetail")
        //                                {
        //                                    numberfiles = numberfiles + 1;
        //                                    string query = string.Format("Select * from [{0}]", excelSheets[i]);

        //                                    OleDbDataAdapter olda = new OleDbDataAdapter(query, excelConnection1);
        //                                    odt = new DataTable();
        //                                    odt.TableName = "tblCrew";
        //                                    olda.Fill(odt);

        //                                    var dtrowc1 = odt.Rows.Count;
        //                                    for (int k = 0; k < dtrowc1; k++)
        //                                    {

        //                                        int CrewId = Convert.ToInt32(odt.Rows[k]["Crew_ID"].ToString());
        //                                        int VesselID = Convert.ToInt32(odt.Rows[k]["Vessel_ID"].ToString());
        //                                        var vsname = sc.Vessels.Where(x => x.VesselID == VesselID).Select(a => new { a.VesselName, a.Flag, a.FleetName, a.FleetType }).FirstOrDefault();

        //                                        string vesselname = vsname.VesselName;
        //                                        if (UserRole.username.ToLower() != "admin")
        //                                        {
        //                                            var vsname1 = sc.Users.Where(x => x.EmailId.Equals(UserRole.username1)).Select(x => x.VesselName).FirstOrDefault();
        //                                            string[] vesselname1 = vsname1.TrimEnd(',').Split(',');
        //                                            bool ss = vesselname1.Contains(vesselname);
        //                                            if (ss == false)
        //                                                vesselname = null;
        //                                        }


        //                                        string Flags = vsname.Flag;
        //                                        string fleetname = vsname.FleetName;
        //                                        string fleettype = vsname.FleetType;

        //                                        //string vessellist = BMSID.vissellist;
        //                                        //vessellist = vessellist.TrimEnd(',');
        //                                        //string[] vli = vessellist.Split(',');


        //                                        var SeaWk11 = odt.Columns.Contains("SeaWk1") == true ? odt.Rows[k]["SeaWk1"].ToString() : odt.Columns.Contains("SeaWKSchedule") == true ? odt.Rows[k]["SeaWKSchedule"].ToString() : odt.Rows[k]["SeaWk1"].ToString();
        //                                        var SeaNWK11 = odt.Columns.Contains("SeaNWK1") == true ? odt.Rows[k]["SeaNWK1"].ToString() : "";
        //                                        var PortWk11 = odt.Columns.Contains("PortWk1") == true ? odt.Rows[k]["PortWk1"].ToString() : odt.Columns.Contains("PortWKshedule") == true ? odt.Rows[k]["PortWKshedule"].ToString() : odt.Rows[k]["PortWk1"].ToString();
        //                                        var PortNWK11 = odt.Columns.Contains("PortNWK1") == true ? odt.Rows[k]["PortNWK1"].ToString() : "";
        //                                        var Watchkeeper1 = odt.Columns.Contains("Watchkeeper") == true ? odt.Rows[k]["Watchkeeper"].ToString() : "Yes";
        //                                        //if (vli.Contains(VesselID.ToString()))
        //                                        //{
        //                                        if (!string.IsNullOrEmpty(vesselname))
        //                                        {
        //                                            var crew = new CrewDetailClass()
        //                                            {
        //                                                Vessel_ID = VesselID,
        //                                                VesselName = vesselname,
        //                                                FleetName = fleetname,
        //                                                FleetType = fleettype,
        //                                                Crew_ID = CrewId,
        //                                                FullName = odt.Rows[k]["FullName"].ToString(),
        //                                                UserName = odt.Rows[k]["UserName"].ToString(),
        //                                                Position = odt.Rows[k]["Position"].ToString(),
        //                                                rid = 0,//Convert.ToInt32(odt.Rows[k]["rid"].ToString()),
        //                                                ServiceFrom = Convert.ToDateTime(odt.Rows[k]["ServiceFrom"].ToString()),
        //                                                ServiceTo = Convert.ToDateTime(odt.Rows[k]["ServiceTo"].ToString()),
        //                                                CDCNumber = odt.Rows[k]["CDCNumber"].ToString(),
        //                                                Emp_Number = odt.Rows[k]["Emp_Number"].ToString(),
        //                                                Comments = odt.Rows[k]["Comments"].ToString(),
        //                                                Overtime = Convert.ToBoolean(odt.Rows[k]["Overtime"].ToString()),
        //                                                Department = odt.Rows[k]["Department"].ToString(),
        //                                                SeaWH = odt.Rows[k]["SeaWH"] == DBNull.Value ? 0 : Convert.ToDecimal(odt.Rows[k]["SeaWH"].ToString()),
        //                                                PortWH = odt.Rows[k]["PortWH"] == DBNull.Value ? 0 : Convert.ToDecimal(odt.Rows[k]["PortWH"].ToString()),

        //                                                SeaWk1 = SeaWk11,
        //                                                SeaNWK1 = SeaNWK11,
        //                                                PortWk1 = PortWk11,
        //                                                PortNWK1 = PortNWK11,

        //                                                Remarks = odt.Rows[k]["Remarks"].ToString(),
        //                                                Watchkeeper = Watchkeeper1,
        //                                                NrmlWHrs = odt.Rows[k]["NrmlWHrs"] == DBNull.Value ? 0 : Convert.ToInt32(odt.Rows[k]["NrmlWHrs"]),
        //                                                SatWHrs = odt.Rows[k]["SatWHrs"] == DBNull.Value ? 0 : Convert.ToInt32(odt.Rows[k]["SatWHrs"]),
        //                                                SunWhrs = odt.Rows[k]["SunWhrs"] == DBNull.Value ? 0 : Convert.ToInt32(odt.Rows[k]["SunWhrs"]),
        //                                                HolidayWHrs = odt.Rows[k]["HolidayWHrs"] == DBNull.Value ? 0 : Convert.ToInt32(odt.Rows[k]["HolidayWHrs"]),
        //                                                FixedOverTime = odt.Rows[k]["FixedOverTime"] == DBNull.Value ? 0 : Convert.ToInt32(odt.Rows[k]["FixedOverTime"]),
        //                                                HourlyRate = odt.Rows[k]["HourlyRate"] == DBNull.Value ? 0 : Convert.ToDecimal(odt.Rows[k]["HourlyRate"]),
        //                                                Currency = odt.Rows[k]["Currency"].ToString(),
        //                                                holidays = odt.Rows[k]["holidays"].ToString(),
        //                                                Flag = Flags,
        //                                                FileNames = filenames,
        //                                                ImportDate = edate
        //                                            };
        //                                            var checkuser = sc.CrewDetails.Where(p => p.Vessel_ID.Equals(VesselID) && p.UserName.Equals(crew.UserName) && p.Position.Equals(crew.Position)).FirstOrDefault();
        //                                            if (checkuser != null)
        //                                            {

        //                                                crew.Id = checkuser.Id;

        //                                                var local = sc.Set<CrewDetailClass>()
        //                                                 .Local
        //                                                 .FirstOrDefault(f => f.Id == checkuser.Id);
        //                                                if (local != null)
        //                                                {
        //                                                    sc.Entry(checkuser).State = EntityState.Detached;
        //                                                }

        //                                                sc.Entry(crew).State = EntityState.Modified;
        //                                                sc.SaveChanges();


        //                                                //sc.CrewDetails.Add(crew);
        //                                                //await sc.SaveChangesAsync();

        //                                            }
        //                                            else
        //                                            {
        //                                                sc.CrewDetails.Add(crew);
        //                                                await sc.SaveChangesAsync();
        //                                            }


        //                                        }
        //                                        else
        //                                        {

        //                                            bmsuploaddats.bmsname = "Vessel Name/Vessel ID  does not appear anywhere else";
        //                                        }


        //                                    }//foreach



        //                                }
        //                                if (importsheet.TrimEnd('$') == "WorkHours")
        //                                {
        //                                    numberfiles = numberfiles + 1;
        //                                    string query = string.Format("Select * from [{0}]", excelSheets[i]);
        //                                    OleDbDataAdapter olda = new OleDbDataAdapter(query, excelConnection1);
        //                                    odt = new DataTable();
        //                                    odt.TableName = "tblCrew1";
        //                                    olda.Fill(odt);
        //                                    var dtrowc2 = odt.Rows.Count;

        //                                    List<CreReportClass> importlog = new List<CreReportClass>();

        //                                    string username = UserRole.username;
        //                                    string vesselname = "";
        //                                    for (int k = 0; k < dtrowc2; k++)
        //                                    {
        //                                        int CrewId = Convert.ToInt32(odt.Rows[k]["ID"].ToString());
        //                                        int VesselID = Convert.ToInt32(odt.Rows[k]["Vessel_ID"].ToString());
        //                                        var vsname = sc.Vessels.Where(x => x.VesselID == VesselID).FirstOrDefault();
        //                                        vesselname = vsname.VesselName;

        //                                        if (UserRole.username.ToLower() != "admin")
        //                                        {
        //                                            var vsname1 = sc.Users.Where(x => x.EmailId.Equals(UserRole.username1)).Select(x => x.VesselName).FirstOrDefault();
        //                                            string[] vesselname1 = vsname1.TrimEnd(',').Split(',');
        //                                            bool ss = vesselname1.Contains(vesselname);
        //                                            if (ss == false)
        //                                                vesselname = null;
        //                                        }

        //                                        string fleetname = vsname.FleetName;
        //                                        string fleettype = vsname.FleetType;

        //                                        //string vessellist = BMSID.vissellist;
        //                                        //vessellist = vessellist.TrimEnd(',');
        //                                        //string[] vli = vessellist.Split(',');


        //                                        //if (vli.Contains(VesselID.ToString()))
        //                                        //{
        //                                        if (!string.IsNullOrEmpty(vesselname))
        //                                        {

        //                                            CreReportClass crw = new CreReportClass();

        //                                            crw.Vessel_ID = VesselID;
        //                                            crw.VesselName = vesselname;
        //                                            crw.FleetName = fleetname;
        //                                            crw.FleetType = fleettype;
        //                                            crw.ID = CrewId;
        //                                            crw.FullName = odt.Rows[k]["FullName"].ToString();
        //                                            crw.UserName = odt.Rows[k]["UserName"].ToString();
        //                                            crw.Position = odt.Rows[k]["Position"].ToString();
        //                                            crw.TotalHours = odt.Rows[k]["TotalHours"] == DBNull.Value ? 0 : Convert.ToDecimal(odt.Rows[k]["TotalHours"].ToString());
        //                                            crw.RestHours = odt.Rows[k]["RestHours"] == DBNull.Value ? 0 : Convert.ToDecimal(odt.Rows[k]["RestHours"].ToString());
        //                                            crw.Options = odt.Rows[k]["Options"].ToString();
        //                                            crw.Remarks = odt.Rows[k]["Remarks"].ToString();
        //                                            crw.Dates = Convert.ToDateTime(odt.Rows[k]["Dates"].ToString());
        //                                            crw.hrs = odt.Rows[k]["hrs"].ToString();
        //                                            crw.NonConfirmities = odt.Rows[k]["NonConfirmities"].ToString();
        //                                            crw.Department = odt.Rows[k]["Department"].ToString();
        //                                            crw.DaysOfMonth = odt.Rows[k]["DaysOfMonth"].ToString();
        //                                            crw.Overtime = odt.Rows[k]["Overtime"] == DBNull.Value ? 0 : Convert.ToDecimal(odt.Rows[k]["Overtime"].ToString());
        //                                            crw.Opa = Convert.ToBoolean(odt.Rows[k]["Opa"].ToString());
        //                                            crw.RestHourAny24 = odt.Rows[k]["RestHourAny24"] == DBNull.Value ? 0 : Convert.ToDecimal(odt.Rows[k]["RestHourAny24"].ToString());
        //                                            crw.RestHourAny7day = odt.Rows[k]["RestHourAny7day"] == DBNull.Value ? 0 : Convert.ToDecimal(odt.Rows[k]["RestHourAny7day"].ToString());
        //                                            crw.NormalWH = odt.Rows[k]["NormalWH"] == DBNull.Value ? 0 : Convert.ToDecimal(odt.Rows[k]["NormalWH"].ToString());
        //                                            string[] str = odt.Rows[k]["hrs"].ToString().TrimEnd(',').Split(',').ToArray();

        //                                            crw.col1 = str[0].ToString();
        //                                            crw.col2 = str[1].ToString();
        //                                            crw.col3 = str[2].ToString();
        //                                            crw.col4 = str[3].ToString();
        //                                            crw.col5 = str[4].ToString();
        //                                            crw.col6 = str[5].ToString();
        //                                            crw.col7 = str[6].ToString();
        //                                            crw.col8 = str[7].ToString();
        //                                            crw.col9 = str[8].ToString();
        //                                            crw.col10 = str[9].ToString();
        //                                            crw.col11 = str[10].ToString();
        //                                            crw.col12 = str[11].ToString();
        //                                            crw.col13 = str[12].ToString();
        //                                            crw.col14 = str[13].ToString();
        //                                            crw.col15 = str[14].ToString();
        //                                            crw.col16 = str[15].ToString();
        //                                            crw.col17 = str[16].ToString();
        //                                            crw.col18 = str[17].ToString();
        //                                            crw.col19 = str[18].ToString();
        //                                            crw.col20 = str[19].ToString();
        //                                            crw.col21 = str[20].ToString();
        //                                            crw.col22 = str[21].ToString();
        //                                            crw.col23 = str[22].ToString();
        //                                            crw.col24 = str[23].ToString();
        //                                            crw.col25 = str[24].ToString();
        //                                            crw.col26 = str[25].ToString();
        //                                            crw.col27 = str[26].ToString();
        //                                            crw.col28 = str[27].ToString();
        //                                            crw.col29 = str[28].ToString();
        //                                            crw.col30 = str[29].ToString();
        //                                            crw.col31 = str[30].ToString();
        //                                            crw.col32 = str[31].ToString();
        //                                            crw.col33 = str[32].ToString();
        //                                            crw.col34 = str[33].ToString();
        //                                            crw.col35 = str[34].ToString();
        //                                            crw.col36 = str[35].ToString();
        //                                            crw.col37 = str[36].ToString();
        //                                            crw.col38 = str[37].ToString();
        //                                            crw.col39 = str[38].ToString();
        //                                            crw.col40 = str[39].ToString();
        //                                            crw.col41 = str[40].ToString();
        //                                            crw.col42 = str[41].ToString();
        //                                            crw.col43 = str[42].ToString();
        //                                            crw.col44 = str[43].ToString();
        //                                            crw.col45 = str[44].ToString();
        //                                            crw.col46 = str[45].ToString();
        //                                            crw.col47 = str[46].ToString();
        //                                            crw.col48 = str[47].ToString();
        //                                            if (str.Length > 48)
        //                                            {
        //                                                crw.col49 = str[48].ToString();
        //                                                crw.col50 = str[49].ToString();
        //                                                crw.col51 = str[50].ToString();
        //                                                crw.col52 = str[51].ToString();
        //                                                crw.col53 = str[52].ToString();
        //                                                crw.col54 = str[53].ToString();
        //                                                crw.col55 = str[54].ToString();
        //                                                crw.col56 = str[55].ToString();
        //                                                crw.col57 = str[56].ToString();
        //                                                crw.col58 = str[57].ToString();
        //                                                crw.col59 = str[58].ToString();
        //                                                crw.col60 = str[59].ToString();
        //                                                crw.col61 = str[60].ToString();
        //                                                crw.col62 = str[61].ToString();
        //                                                crw.col63 = str[62].ToString();
        //                                                crw.col64 = str[63].ToString();
        //                                                crw.col65 = str[64].ToString();
        //                                                crw.col66 = str[65].ToString();
        //                                                crw.col67 = str[66].ToString();
        //                                                crw.col68 = str[67].ToString();
        //                                                crw.col69 = str[68].ToString();
        //                                                crw.col70 = str[69].ToString();
        //                                                crw.col71 = str[70].ToString();
        //                                                crw.col72 = str[71].ToString();
        //                                                crw.col73 = str[72].ToString();
        //                                                crw.col74 = str[73].ToString();
        //                                                crw.col75 = str[74].ToString();
        //                                                crw.col76 = str[75].ToString();
        //                                                crw.col77 = str[76].ToString();
        //                                                crw.col78 = str[77].ToString();
        //                                                crw.col79 = str[78].ToString();
        //                                                crw.col80 = str[79].ToString();
        //                                                crw.col81 = str[80].ToString();
        //                                                crw.col82 = str[81].ToString();
        //                                                crw.col83 = str[82].ToString();
        //                                                crw.col84 = str[83].ToString();
        //                                                crw.col85 = str[84].ToString();
        //                                                crw.col86 = str[85].ToString();
        //                                                crw.col87 = str[86].ToString();
        //                                                crw.col88 = str[87].ToString();
        //                                                crw.col89 = str[88].ToString();
        //                                                crw.col90 = str[89].ToString();
        //                                                crw.col91 = str[90].ToString();
        //                                                crw.col92 = str[91].ToString();
        //                                                crw.col93 = str[92].ToString();
        //                                                crw.col94 = str[93].ToString();
        //                                                crw.col95 = str[94].ToString();
        //                                                crw.col96 = str[95].ToString();
        //                                            }
        //                                            crw.youngseafearer = Convert.ToBoolean(odt.Rows[k]["chkyoungs"].ToString());
        //                                            crw.ImportDate = edate;
        //                                            crw.ImportedBy = username;
        //                                            crw.FileNames = filenames;

        //                                            importlog.Add(crw);


        //                                            var checkuser = sc.CreReports.Where(p => p.Vessel_ID.Equals(VesselID) && p.ID.Equals(CrewId) && p.UserName.Equals(crw.UserName)).FirstOrDefault();
        //                                            if (checkuser != null)
        //                                            {

        //                                                crw.Wid = checkuser.Wid;

        //                                                var local = sc.Set<CreReportClass>()
        //                                                 .Local
        //                                                 .FirstOrDefault(f => f.Wid == checkuser.Wid);
        //                                                if (local != null)
        //                                                {
        //                                                    sc.Entry(checkuser).State = EntityState.Detached;
        //                                                }

        //                                                //var user = sc.CreReports.Find(crw.Wid);

        //                                                sc.Entry(crw).State = EntityState.Modified;
        //                                                sc.SaveChanges();

        //                                            }
        //                                            else
        //                                            {
        //                                                sc.CreReports.Add(crw);
        //                                                await sc.SaveChangesAsync();
        //                                            }

        //                                        }
        //                                        else
        //                                        {
        //                                            bmsuploaddats.bmsname = "Vessel Name/Vessel ID  does not appear anywhere else";
        //                                        }


        //                                    }
        //                                    //Create logs.....
        //                                    if (importlog.Count > 0)
        //                                    {
        //                                        var nots = new ImportLogClass()
        //                                        {
        //                                            DateImported = Convert.ToDateTime(importlog.OrderByDescending(m => m.ImportDate).FirstOrDefault().ImportDate),
        //                                            DateImportFrom = Convert.ToDateTime(importlog.OrderBy(m => m.Dates).FirstOrDefault().Dates),
        //                                            DateImportTo = Convert.ToDateTime(importlog.OrderByDescending(m => m.Dates).FirstOrDefault().Dates),
        //                                            VesselName = importlog.OrderByDescending(m => m.ImportDate).FirstOrDefault().VesselName,

        //                                            Filenames = importlog.OrderByDescending(m => m.ImportDate).FirstOrDefault().FileNames,
        //                                            ImportedBy = importlog.OrderByDescending(m => m.ImportDate).FirstOrDefault().ImportedBy,
        //                                            DataAvailbleTill = Convert.ToDateTime(importlog.OrderByDescending(m => m.Dates).FirstOrDefault().Dates)
        //                                        };
        //                                        sc.ImportLogs.Add(nots);
        //                                        await sc.SaveChangesAsync();

        //                                    }
        //                                    else
        //                                    {
        //                                        var nots = new ImportLogClass()
        //                                        {
        //                                            DateImported = edate,

        //                                            VesselName = vesselname,

        //                                            Filenames = filenames,
        //                                            ImportedBy = username

        //                                        };

        //                                        sc.ImportLogs.Add(nots);
        //                                        await sc.SaveChangesAsync();
        //                                    }


        //                                }
        //                                if (importsheet.TrimEnd('$') == "CertificateList")
        //                                {
        //                                    numberfiles = numberfiles + 1;
        //                                    string query = string.Format("Select * from [{0}]", excelSheets[i]);
        //                                    OleDbDataAdapter olda = new OleDbDataAdapter(query, excelConnection1);
        //                                    odt = new DataTable();
        //                                    odt.TableName = "tblCrew2";
        //                                    olda.Fill(odt);
        //                                    var dtrowc3 = odt.Rows.Count;
        //                                    for (int k = 0; k < dtrowc3; k++)
        //                                    {

        //                                        int CId = Convert.ToInt32(odt.Rows[k]["CId"].ToString());
        //                                        int VesselID = Convert.ToInt32(odt.Rows[k]["Vessel_ID"].ToString());

        //                                        var vsname = sc.Vessels.Where(x => x.VesselID == VesselID).FirstOrDefault();
        //                                        string vesselname = vsname.VesselName;

        //                                        if (UserRole.username.ToLower() != "admin")
        //                                        {
        //                                            var vsname1 = sc.Users.Where(x => x.EmailId.Equals(UserRole.username1)).Select(x => x.VesselName).FirstOrDefault();
        //                                            string[] vesselname1 = vsname1.TrimEnd(',').Split(',');
        //                                            bool ss = vesselname1.Contains(vesselname);
        //                                            if (ss == false)
        //                                                vesselname = null;
        //                                        }



        //                                        string fleetname = vsname.FleetName;
        //                                        string fleettype = vsname.FleetType;

        //                                        if (!string.IsNullOrEmpty(vesselname))
        //                                        {

        //                                            var crl = new CertificateList()
        //                                            {
        //                                                VesselId = VesselID,
        //                                                VesselName = vesselname,
        //                                                FleetName = fleetname,
        //                                                FleetType = fleettype,
        //                                                Cid = CId,
        //                                                CName = odt.Rows[k]["CName"].ToString(),
        //                                                DOI = Convert.ToDateTime(odt.Rows[k]["DOI"].ToString()),
        //                                                DOS = Convert.ToDateTime(odt.Rows[k]["DOS"].ToString()),
        //                                                DOE = Convert.ToDateTime(odt.Rows[k]["DOE"].ToString()),
        //                                                Remarks = odt.Rows[k]["Remarks"].ToString(),
        //                                                FileNames = filenames,
        //                                                ImportDate = edate
        //                                            };
        //                                            var checkuser = sc.CertificateLists.Where(p => p.VesselId.Equals(VesselID) && p.Cid.Equals(CId) && p.CName.Equals(crl.CName)).FirstOrDefault();
        //                                            if (checkuser != null)
        //                                            {

        //                                                crl.Id = checkuser.Id;

        //                                                var local = sc.Set<CertificateList>()
        //                                                 .Local
        //                                                 .FirstOrDefault(f => f.Id == checkuser.Id);
        //                                                if (local != null)
        //                                                {
        //                                                    sc.Entry(checkuser).State = EntityState.Detached;
        //                                                }

        //                                                sc.Entry(crl).State = EntityState.Modified;
        //                                                sc.SaveChanges();


        //                                            }
        //                                            else
        //                                            {
        //                                                sc.CertificateLists.Add(crl);
        //                                                await sc.SaveChangesAsync();
        //                                            }


        //                                        }
        //                                        else
        //                                        {
        //                                            bmsuploaddats.bmsname = "Vessel Name/Vessel ID  does not appear anywhere else";

        //                                        }

        //                                    }



        //                                }
        //                                if (importsheet.TrimEnd('$') == "CertificateNotificatonList")
        //                                {

        //                                    numberfiles = numberfiles + 1;
        //                                    string query = string.Format("Select * from [{0}]", excelSheets[i]);
        //                                    OleDbDataAdapter olda = new OleDbDataAdapter(query, excelConnection1);
        //                                    odt = new DataTable();
        //                                    odt.TableName = "tblCrew3";
        //                                    olda.Fill(odt);
        //                                    var dtrowc4 = odt.Rows.Count;
        //                                    for (int k = 0; k < dtrowc4; k++)
        //                                    {

        //                                        int CId = Convert.ToInt32(odt.Rows[k]["Id"].ToString());
        //                                        int VesselID = Convert.ToInt32(odt.Rows[k]["Vessel_ID"].ToString());

        //                                        var vsname = sc.Vessels.Where(x => x.VesselID == VesselID).FirstOrDefault();
        //                                        string vesselname = vsname.VesselName; if (UserRole.username.ToLower() != "admin")
        //                                        {
        //                                            var vsname1 = sc.Users.Where(x => x.EmailId.Equals(UserRole.username1)).Select(x => x.VesselName).FirstOrDefault();
        //                                            string[] vesselname1 = vsname1.TrimEnd(',').Split(',');
        //                                            bool ss = vesselname1.Contains(vesselname);
        //                                            if (ss == false)
        //                                                vesselname = null;
        //                                        }


        //                                        string fleetname = vsname.FleetName;
        //                                        string fleettype = vsname.FleetType;

        //                                        if (!string.IsNullOrEmpty(vesselname))
        //                                        {
        //                                            var cts = new CertificationClass()
        //                                            {
        //                                                VesselId = VesselID,
        //                                                VesselName = vesselname,
        //                                                FleetName = fleetname,
        //                                                FleetType = fleettype,
        //                                                Id = CId,
        //                                                CName = odt.Rows[k]["CName"].ToString(),
        //                                                DOI = Convert.ToDateTime(odt.Rows[k]["DOI"].ToString()),
        //                                                DOS = Convert.ToDateTime(odt.Rows[k]["DOS"].ToString()),
        //                                                DOE = Convert.ToDateTime(odt.Rows[k]["DOE"].ToString()),
        //                                                AlertFrequency = odt.Rows[k]["AlertFrequency"].ToString(),
        //                                                AdminAck = odt.Rows[k]["AdminAck"].ToString(),
        //                                                MasterAck = odt.Rows[k]["MasterAck"].ToString(),
        //                                                HODAck = odt.Rows[k]["HODAck"].ToString(),
        //                                                FileNames = filenames,
        //                                                ImportDate = edate
        //                                            };

        //                                            var checkuser = sc.Certifications.Where(p => p.VesselId.Equals(VesselID) && p.Id.Equals(CId) && p.CName.Equals(cts.CName) && p.FileNames.Equals(cts.FileNames)).FirstOrDefault();
        //                                            if (checkuser != null)
        //                                            {

        //                                                cts.Nid = checkuser.Nid;

        //                                                var local = sc.Set<CertificationClass>()
        //                                                  .Local
        //                                                  .FirstOrDefault(f => f.Nid == checkuser.Nid);
        //                                                if (local != null)
        //                                                {
        //                                                    sc.Entry(checkuser).State = EntityState.Detached;
        //                                                }

        //                                                sc.Entry(cts).State = EntityState.Modified;
        //                                                sc.SaveChanges();

        //                                            }
        //                                            else
        //                                            {
        //                                                sc.Certifications.Add(cts);
        //                                                await sc.SaveChangesAsync();
        //                                            }

        //                                        }
        //                                        else
        //                                        {
        //                                            bmsuploaddats.bmsname = "Vessel Name/Vessel ID  does not appear anywhere else";
        //                                        }

        //                                    }
        //                                }

        //                                if (importsheet.TrimEnd('$') == "WorkHoursNotificatonList")
        //                                {

        //                                    numberfiles = numberfiles + 1;
        //                                    string query = string.Format("Select * from [{0}]", excelSheets[i]);
        //                                    OleDbDataAdapter olda = new OleDbDataAdapter(query, excelConnection1);
        //                                    odt = new DataTable();
        //                                    odt.TableName = "tblCrew4";
        //                                    olda.Fill(odt);
        //                                    var dtrowc5 = odt.Rows.Count;
        //                                    for (int k = 0; k < dtrowc5; k++)
        //                                    {
        //                                        int CId = Convert.ToInt32(odt.Rows[k]["ROW_NUM"].ToString());
        //                                        int VesselID = Convert.ToInt32(odt.Rows[k]["Vessel_ID"].ToString());

        //                                        var vsname = sc.Vessels.Where(x => x.VesselID == VesselID).FirstOrDefault();
        //                                        string vesselname = vsname.VesselName;

        //                                        if (UserRole.username.ToLower() != "admin")
        //                                        {
        //                                            var vsname1 = sc.Users.Where(x => x.EmailId.Equals(UserRole.username1)).Select(x => x.VesselName).FirstOrDefault();
        //                                            string[] vesselname1 = vsname1.TrimEnd(',').Split(',');
        //                                            bool ss = vesselname1.Contains(vesselname);
        //                                            if (ss == false)
        //                                                vesselname = null;
        //                                        }


        //                                        string fleetname = vsname.FleetName;
        //                                        string fleettype = vsname.FleetType;


        //                                        if (!string.IsNullOrEmpty(vesselname))
        //                                        {
        //                                            var ntc = new NotificationClass()
        //                                            {
        //                                                VesselId = VesselID,
        //                                                VesselName = vesselname,
        //                                                FleetName = fleetname,
        //                                                FleetType = fleettype,
        //                                                wid = CId,
        //                                                NcDate = Convert.ToDateTime(odt.Rows[k]["dates"].ToString()),
        //                                                NonConfirmity = odt.Rows[k]["NonConfirmities"].ToString(),
        //                                                //NCtype = odt.Rows[k]["NCtype"].ToString(),
        //                                                FullName = odt.Rows[k]["Name"].ToString(),
        //                                                UserName = odt.Rows[k]["UserNames"].ToString(),
        //                                                Rank = odt.Rows[k]["position"].ToString(),
        //                                                AdminAkn = odt.Rows[k]["AckAdmin"].ToString(),
        //                                                MasterAkn = odt.Rows[k]["AckMaster"].ToString(),
        //                                                HODAkn = odt.Rows[k]["AckHOD"].ToString(),
        //                                                FileNames = filenames,
        //                                                EDate = edate
        //                                            };
        //                                            var checkuser = sc.Notifications.Where(p => p.VesselId.Equals(VesselID) && p.wid.Equals(CId) && p.UserName.Equals(ntc.UserName) && p.NCtype.Equals(ntc.NCtype)).FirstOrDefault();
        //                                            if (checkuser != null)
        //                                            {

        //                                                ntc.Nid = checkuser.Nid;

        //                                                var local = sc.Set<NotificationClass>()
        //                                                  .Local
        //                                                  .FirstOrDefault(f => f.Nid == checkuser.Nid);
        //                                                if (local != null)
        //                                                {
        //                                                    sc.Entry(checkuser).State = EntityState.Detached;
        //                                                }

        //                                                sc.Entry(ntc).State = EntityState.Modified;
        //                                                sc.SaveChanges();

        //                                            }
        //                                            else
        //                                            {
        //                                                sc.Notifications.Add(ntc);
        //                                                await sc.SaveChangesAsync();
        //                                            }


        //                                        }
        //                                        else
        //                                        {
        //                                            bmsuploaddats.bmsname = "Vessel Name/Vessel ID  does not appear anywhere else";
        //                                        }

        //                                    }
        //                                }
        //                            }//.....

        //                            excelConnection.Close();
        //                            excelConnection.Dispose();
        //                            excelConnection1.Close();
        //                            excelConnection1.Dispose();
        //                            FileInfo file = new FileInfo(fileLocation);
        //                            file.Delete();

        //                            if (numberfiles == 0)
        //                            {

        //                                bmsuploaddats.bmsname = "Vessel Name/Vessel ID  does not appear anywhere else";
        //                            }
        //                            else
        //                            {

        //                                bmsuploaddats.bmsname = "Data Imported Successfully";
        //                            }

        //                        } //.........
        //                        else
        //                        {

        //                            bmsuploaddats.bmsname = "Please  browse the  excel file";
        //                        }


        //                    }



        //                }
        //                else
        //                {
        //                    bmsuploaddats.bmsname = "Please  browse the  excel file";
        //                }


        //            }
        //            catch (Exception ex)
        //            {

        //                bmsuploaddats.bmsname = ex.Message;
        //                bmsuploaddats.bmsname = "Vessel Name/Vessel ID  does not appear anywhere else";

        //                excelConnection.Close();
        //                excelConnection.Dispose();
        //                excelConnection1.Close();
        //                excelConnection1.Dispose();
        //                FileInfo file = new FileInfo(fileLocation);
        //                if (file.Exists)
        //                    file.Delete();

        //            }

        //            var usernamess = string.Join("", Roles.GetRolesForUser());
        //        }
        //        else if (submit == "Export Text")
        //        {
        //            if (!string.IsNullOrEmpty(searchTerm))
        //            {
        //                //stored result into datatable  
        //                DataTable vesselDetails = LINQResultToDataTable(vesselData);
        //                vesselDetails.TableName = "VesselDetails";

        //                if (vesselData.ToList().Count > 0)
        //                {
        //                    shipID = Convert.ToString(vesselData.First().ImoNo);
        //                }

        //                var docsPageData = sc.DocsPagges.Where(p => p.CreatedDate >= ServiceFrom && p.CreatedDate <= ServiceTo).ToList();

        //                List<DocPagesExport> docPagesExports = new List<DocPagesExport>();
        //                foreach (var x in docsPageData)
        //                {
        //                    docPagesExports.Add(new DocPagesExport
        //                    {
        //                        ID = x.Id,
        //                        Content = x.Content,
        //                        ShipID = x.ShipId,
        //                        MID = x.Mid,
        //                        CreatedBy = x.CreateBy,
        //                        CreatedDate = x.CreatedDate == null ? DateTime.Today : Convert.ToDateTime(x.CreatedDate),
        //                        ModifiedBy = x.ModifiedBy,
        //                        ModifiedDate = x.ModifiedDate == null ? DateTime.Today : Convert.ToDateTime(x.ModifiedDate)
        //                    });
        //                }

        //                //stored result into datatable  
        //                DataTable docsPages = LINQResultToDataTable(docPagesExports);
        //                docsPages.TableName = "DocsPages";

        //                var shipSpecificContentData = sc.ShipSpecificContents.Where(p => p.ShipId == shipID && p.CreatedDate >= ServiceFrom && p.CreatedDate <= ServiceTo).
        //                    Select(x => new { x.Id, x.Content, x.MId, x.ShipId, Created_Date = x.CreatedDate }).ToList();

        //                //stored result into datatable  
        //                DataTable shipSpecificContent = LINQResultToDataTable(shipSpecificContentData);
        //                shipSpecificContent.TableName = "ShipSpecificContent";

        //                var shipSpecificAttachmentData = sc.ShipSpecificAttachments.Where(p => p.ShipId == shipID && p.CreatedDate >= ServiceFrom && p.CreatedDate <= ServiceTo).ToList();

        //                List<ShipSpecificAttachmentExport> shipSpecificAttachmentExports = new List<ShipSpecificAttachmentExport>();
        //                foreach (var x in shipSpecificAttachmentData)
        //                {
        //                    shipSpecificAttachmentExports.Add(new ShipSpecificAttachmentExport
        //                    {
        //                        ID = x.Id,
        //                        Attachment = x.Attachment,
        //                        AttachmentName = x.AttachmentName,
        //                        MID = x.MId,
        //                        ShipID = x.ShipId,
        //                        CreatedBy = x.CreateBy,
        //                        ModifiedBy = x.ModifiedBy,
        //                        CreatedDate = x.CreatedDate == null ? DateTime.Today : Convert.ToDateTime(x.CreatedDate),
        //                        ModifiedDate = x.ModifiedDate == null ? DateTime.Today : Convert.ToDateTime(x.ModifiedDate)
        //                    });
        //                }

        //                //stored result into datatable  
        //                DataTable shipSpecificAttachments = LINQResultToDataTable(shipSpecificAttachmentExports);
        //                shipSpecificAttachments.TableName = "ShipSpecificAttachments";

        //                DataTable smartMenu = new DataTable();
        //                var smartMenuData = sc.SmartMenus.ToList();

        //                List<SmartMenuExport> smartMenuExports = new List<SmartMenuExport>();
        //                foreach (var x in smartMenuData)
        //                {
        //                    var menu = new SmartMenuExport
        //                    {
        //                        ID = x.Id,
        //                        HtmlContent = x.HtmlContent,
        //                        SmartMenuContent = x.SmartMenuContent
        //                    };

        //                    foreach (string m in hrefs)
        //                    {
        //                        if (x.HtmlContentExport.Contains(m))
        //                            x.HtmlContentExport = x.HtmlContentExport.Replace(m, string.Empty);

        //                        if (x.SmartMenuContentExport.Contains(m))
        //                            x.SmartMenuContentExport = x.SmartMenuContentExport.Replace(m, string.Empty);
        //                    }

        //                    menu.HtmlContentExport = x.HtmlContentExport;
        //                    menu.SmartMenuContentExport = x.SmartMenuContentExport;

        //                    smartMenuExports.Add(menu);
        //                }

        //                //stored result into datatable  
        //                smartMenu = LINQResultToDataTable(smartMenuExports);
        //                smartMenu.TableName = "SmartMenu";

        //                var looseEquipInspectionData = sc.LooseEquipInspectionSettings.ToList();

        //                List<LooseEquipmentSettingsExport> looseEquipSettingExports = new List<LooseEquipmentSettingsExport>();
        //                foreach (var x in looseEquipInspectionData)
        //                {
        //                    looseEquipSettingExports.Add(new LooseEquipmentSettingsExport
        //                    {
        //                        ID = x.Id,
        //                        EquipmentType = (int)x.EquipmentType,
        //                        MaximumMonthsAllowed = x.MaximumMonthsAllowed == null ? 0 : Convert.ToInt32(x.MaximumMonthsAllowed),
        //                        MaximumRunningHours = x.MaximumRunningHours == null ? 0 : Convert.ToInt32(x.MaximumRunningHours)
        //                    });
        //                }

        //                //stored result into datatable  
        //                DataTable looseEquipInspectionSettings = LINQResultToDataTable(looseEquipSettingExports);
        //                looseEquipInspectionSettings.TableName = "LooseEquipInspectionSettings";

        //                var ropeInspectionSettingsData = sc.RopeInspectionSettings.ToList();

        //                List<RopeInspectionSettingsExport> ropeInspSettingsExport = new List<RopeInspectionSettingsExport>();
        //                foreach (var x in ropeInspectionSettingsData)
        //                {
        //                    ropeInspSettingsExport.Add(new RopeInspectionSettingsExport
        //                    {
        //                        ID = x.Id,
        //                        ManufacturerType = x.ManufacturerType,
        //                        MaximumMonthsAllowed = x.MaximumMonthsAllowed == null ? 0 : Convert.ToInt32(x.MaximumMonthsAllowed),
        //                        MaximumRunningHours = x.MaximumRunningHours == null ? 0 : Convert.ToInt32(x.MaximumRunningHours),
        //                        Rating1 = x.Rating1,
        //                        Rating2 = x.Rating2,
        //                        Rating3 = x.Rating3,
        //                        Rating4 = x.Rating4,
        //                        Rating5 = x.Rating5,
        //                        Rating6 = x.Rating6,
        //                        Rating7 = x.Rating7
        //                    });
        //                }

        //                //stored result into datatable  
        //                DataTable ropeInspectionSettings = LINQResultToDataTable(ropeInspSettingsExport);
        //                ropeInspectionSettings.TableName = "RopeInspectionSettingsData";

        //                using (XLWorkbook wb = new XLWorkbook())
        //                {
        //                    var protectedsheet = wb.Worksheets.Add(vesselDetails);
        //                    protectedsheet = wb.Worksheets.Add(shipSpecificContent);
        //                    protectedsheet = wb.Worksheets.Add(shipSpecificAttachments);
        //                    protectedsheet = wb.Worksheets.Add(looseEquipInspectionSettings);
        //                    protectedsheet = wb.Worksheets.Add(ropeInspectionSettings);
        //                    protectedsheet = wb.Worksheets.Add(docsPages);
        //                    protectedsheet = wb.Worksheets.Add(smartMenu);

        //                    var projection = protectedsheet.Protect("bms123");
        //                    projection.InsertColumns = true;
        //                    projection.InsertRows = true;

        //                    wb.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        //                    wb.Style.Font.Bold = true;

        //                    DateTime today = DateTime.Today;
        //                    string vsname = searchTerm.Replace(" ", "");
        //                    string HeaderName = "Work-Ship_Export" + "_" + vsname + "_" + today.ToString("dd-MMM-yyyy");

        //                    Response.Clear();
        //                    Response.BufferOutput = true;
        //                    Response.Charset = "";
        //                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        //                    Response.AddHeader("content-disposition", "attachment;filename= " + HeaderName + ".xls");

        //                    using (MemoryStream MyMemoryStream = new MemoryStream())
        //                    {
        //                        wb.SaveAs(MyMemoryStream);
        //                        MyMemoryStream.WriteTo(Response.OutputStream);
        //                        Response.End();
        //                    }
        //                    Response.Clear();

        //                    Thread.Sleep(300);
        //                }


        //                //bind gridview  

        //                //string[] tablename = new string[3];
        //                //tablename[0] = "Revision";
        //                //tablename[1] = "DocsPages";
        //                //tablename[2] = "VesselDetail";

        //                //DataSet dt = new DataSet();
        //                //for (int i = 0; i < tablename.Length; i++)
        //                //{
        //                //    string test = (tablename[i]);

        //                //    DataSet dt1 = GetRecordsFromDatabase(test);

        //                //}
        //                ////DataTable dt = bb;
        //                ////dt.TableName = "CrewDetail";

        //                //    //da.Fill(dt);

        //                //    using (XLWorkbook wb = new XLWorkbook())
        //                //    {
        //                //        //foreach (DataTable dt in ds.Tables)
        //                //        //{
        //                //        //    //Add DataTable as Worksheet.
        //                //        //    wb.Worksheets.Add(dt);
        //                //        //}
        //                //        var protectedsheet = wb.Worksheets.Add(dt.Tables[0]);
        //                //        var projection = protectedsheet.Protect("bms123");
        //                //        projection.InsertColumns = true;
        //                //        projection.InsertRows = true;

        //                //        wb.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        //                //        wb.Style.Font.Bold = true;


        //                //        DateTime today = DateTime.Today;
        //                //        string vsname = searchTerm.Replace(" ", "");
        //                //        string HeaderName = "Work-Ship_Export" + "_" + vsname + "_" + today.ToString("dd-MMM-yyyy");

        //                //       // Response.Clear();
        //                //        //Response.ClearHeaders();
        //                //        //Response.ClearContent();
        //                //        Response.Buffer = true;
        //                //        //Response.Buffer = true;
        //                //        Response.BufferOutput = true;
        //                //        Response.Charset = "";
        //                //        Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        //                //        Response.AddHeader("content-disposition", "attachment;filename= " + HeaderName + ".xlsx");

        //                //        using (MemoryStream MyMemoryStream = new MemoryStream())
        //                //        {

        //                //            wb.SaveAs(MyMemoryStream);

        //                //            MyMemoryStream.WriteTo(Response.OutputStream);
        //                //            Response.Flush();
        //                //            Response.End();



        //                //        }
        //                //    }
        //                //ExportdataInExcel(searchTerm);
        //                //Thread.Sleep(300);

        //                //}


        //            }
        //            else
        //            {
        //                bmsuploaddats.bmsname = "Please select vessel name first";
        //            }
        //        }
        //        else
        //        {
        //            // Download Attachments in Zip File
        //            string startPath = Server.MapPath(string.Format("~/images/AttachFiles/filepath/{0}/", shipID));

        //            if (Directory.Exists(startPath))
        //            {
        //                string[] filesFound = Directory.GetFiles(startPath);

        //                if (filesFound.Length > 0)
        //                {
        //                    string zipFileName = string.Format("{0}_{1}.zip", shipID, DateTime.Now.Ticks);
        //                    string zipPath = Server.MapPath("~/images/AttachFiles/filepath/" + zipFileName);

        //                    ZipFile.CreateFromDirectory(startPath, zipPath, CompressionLevel.Fastest, true);

        //                    Response.ContentType = "application/zip";
        //                    Response.AppendHeader("Content-Disposition", "attachment; filename=" + zipFileName);
        //                    Response.TransmitFile(zipPath);
        //                    Response.End();
        //                }
        //                else
        //                {
        //                    bmsuploaddats.bmsname = "No Attachments found.";
        //                }
        //            }
        //            else
        //            {
        //                bmsuploaddats.bmsname = "No Attachments found.";
        //            }
        //        }

        //        ViewBag.vname = searchTerm;
        //        ViewBag.datefrom = ServiceFrom == null ? DateTime.Parse(DateTime.Now.ToString()).ToString("MM/dd/yyyy", CultureInfo.InvariantCulture) : DateTime.Parse(ServiceFrom.ToString()).ToString("MM/dd/yyyy", CultureInfo.InvariantCulture);
        //        ViewBag.dateto = ServiceTo == null ? DateTime.Parse(DateTime.Now.ToString()).ToString("MM/dd/yyyy", CultureInfo.InvariantCulture) : DateTime.Parse(ServiceTo.ToString()).ToString("MM/dd/yyyy", CultureInfo.InvariantCulture);

        //        //return Redirect(Request.Url.AbsoluteUri);
        //        return View(bmsuploaddats);
        //    }
        //}

        public void GenerateThumbnail(HttpPostedFileBase file)
        {
            if (file?.ContentLength > 0)
            {
                string origPath = Server.MapPath("~/images/inspectionimages/");

                if (!Directory.Exists(origPath))
                    Directory.CreateDirectory(origPath);

                var originalFilePath = Path.Combine(origPath, file.FileName);

                file.SaveAs(originalFilePath);

                System.Drawing.Image img = System.Drawing.Image.FromFile(originalFilePath);

                Size thumbnailSize = GetThumbnailSize();

                // Get thumbnail.
                System.Drawing.Image thumbnail = img.GetThumbnailImage(thumbnailSize.Width, thumbnailSize.Height, null, IntPtr.Zero);

                string thumbPath = Server.MapPath("~/images/inspectionimages/thumbnails");

                if (!Directory.Exists(thumbPath))
                    Directory.CreateDirectory(thumbPath);

                // Save thumbnail.
                var thumbnailPath = Path.Combine(thumbPath, file.FileName);

                thumbnail.Save(thumbnailPath);
                thumbnail.Dispose();

                img.Dispose();

                //System.IO.File.Delete(originalFilePath);
            }
        }

        private Size GetThumbnailSize()
        {
            // Maximum size of any dimension.
            const int maxPixels = 250;

            // Width and height.
            int originalWidth = maxWidth; // original.Width;
            int originalHeight = maxHeight; // original.Height;

            // Compute best factor to scale entire image based on larger dimension.
            double factor;
            if (originalWidth > originalHeight)
                factor = (double)maxPixels / originalWidth;
            else
                factor = (double)maxPixels / originalHeight;

            // Return thumbnail size.
            return new Size((int)(originalWidth * factor), (int)(originalHeight * factor));
        }
    }

    public class RevisionExport
    {
        public int ID { get; set; }
        public string RPrefix { get; set; }
        public int RNumber { get; set; }
        public DateTime ReviseDate { get; set; }
        public DateTime ApproveDate { get; set; }
        public string CreatedBy { get; set; }
        public string ApproveBy { get; set; }
        public int MID { get; set; }
        public string RevisionType { get; set; }
        public string Content { get; set; }
        public string ContentPath { get; set; }
        public string Status { get; set; }
        public string RevisionText { get; set; }
    }

    public class DocPagesExport
    {
        public int ID { get; set; }
        public string Content { get; set; }
        public int MID { get; set; }
        public string ShipID { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime ModifiedDate { get; set; }
    }

    public class ShipSpecificContentExport
    {
        public int Id { get; set; }
        public int MId { get; set; }
        public string ShipId { get; set; }
        public string Content { get; set; }
        public DateTime Created_Date { get; set; }
    }

    public class ShipSpecificAttachmentExport
    {
        public int ID { get; set; }
        public string AttachmentName { get; set; }
        public string Attachment { get; set; }
        public int MID { get; set; }
        public string ShipID { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime ModifiedDate { get; set; }

    }

    public class LooseEquipmentSettingsExport
    {
        public int ID { get; set; }
        public int EquipmentType { get; set; }
        public int InspectionFrequency { get; set; }
        public int MaximumRunningHours { get; set; }
        public int MaximumMonthsAllowed { get; set; }
    }

    public class RopeInspectionSettingsExport
    {
        public int ID { get; set; }
        public int MooringRopeType { get; set; }
        public int ManufacturerType { get; set; }
        public int MaximumRunningHours { get; set; }
        public int MaximumMonthsAllowed { get; set; }
        public int EndToEndMonth { get; set; }
        public int RotationOnWinches { get; set; }
        public decimal Rating1 { get; set; }
        public decimal Rating2 { get; set; }
        public decimal Rating3 { get; set; }
        public decimal Rating4 { get; set; }
        public decimal Rating5 { get; set; }
        public decimal Rating6 { get; set; }
        public decimal Rating7 { get; set; }
    }

    public class RopeTailInspectionSettingsExport
    {
        public int ID { get; set; }
        public int MooringRopeType { get; set; }
        public int ManufacturerType { get; set; }
        public int MaximumRunningHours { get; set; }
        public int MaximumMonthsAllowed { get; set; }
        public decimal Rating1 { get; set; }
        public decimal Rating2 { get; set; }
        public decimal Rating3 { get; set; }
        public decimal Rating4 { get; set; }
        public decimal Rating5 { get; set; }
        public decimal Rating6 { get; set; }
        public decimal Rating7 { get; set; }
    }

    public class SmartMenuExport
    {
        public int ID { get; set; }
        public string SmartMenuContentExport { get; set; }
        //public string HtmlContentExport { get; set; }
    }

    public class NotificationCommentExport
    {
        public int Id { get; set; }
        public int NotificationId { get; set; }
        public int CommentsType { get; set; }
        public string Comments { get; set; }
        public bool IsActive { get; set; }
        public int ShipID { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}