using ClosedXML.Excel;
using MenuLayer;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Reports;
using Shipment49Web.Areas.MooringLine.Models;
using Shipment49Web.Areas.MSPS.Models;
using Shipment49Web.Common;
using Shipment49Web.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace Shipment49Web.Areas.MooringLine.Controllers
{
    [Authorize]
    [UserAuthenticationFilter]
    [ErrorClass]
    public class LineInspectionController : BaseController
    {
        MorringOfficeEntities context = new MorringOfficeEntities();
        SqlConnection con = ConnectionBulder.con;
        //CommonClass cls = new CommonClass();
        MooringRopeInspection assR = new MooringRopeInspection();
        DamageR ss = new DamageR();
        public static int? inspectionid = 0;
        public static bool outboardEndinuse = true;
        //public static string VesselID;
        // GET: MooringLine/LineInspection 

        //public static int VesselID { get; set; }
        public LineInspectionController()
        {
           // VesselID = Convert.ToInt32(CommonClass.VesselSessionID);
        }
        public ActionResult Index(int? page)
        {
           int VesselID = Convert.ToInt32(Session["VesselSessionID"].ToString());
            MooringRopeInspections model = new MooringRopeInspections();
            string year = DateTime.Now.Year.ToString();

            model.RopeInspectionList = CommonClass.MooringInspectionList(year,0,VesselID);
            model.YearList = CommonClass.GetYearsList();

            var record = model.RopeInspectionList.Count();
            //int currPage = TempData["CurrentPage"] == null ? 1 : Convert.ToInt32(TempData["CurrentPage"]);

            int currPage = page == null ? 1 : Convert.ToInt32(page);

            TempData["CurrentPage"] = currPage;
            TempData["TotalRecords"] = record;

            model.RopeInspectionList = model.RopeInspectionList.Skip((CommonMethods.PAGESIZE * currPage) - CommonMethods.PAGESIZE).Take(CommonMethods.PAGESIZE).ToList();

            return View(model);

            //VesselID = CommonClass.VesselSessionID;
            //using (ShipmentContaxt sc1 = new ShipmentContaxt())
            //{

            //    var ilist = new ShipmentContaxt()
            //         .MultipleResults("[dbo].[GetRopeInspection] '"+ year + "', 0,'" + VesselID + "'")
            //       .With<MooringRopeInspections>()
            //       .Execute();
            //    model.RopeInspectionList = (List<MooringRopeInspections>)ilist[0];

            //    model.YearList = cls.GetYearsList();
            //    return View(model);
            //}
        }
        [HttpPost]
        public ActionResult Index(int?page, MooringRopeInspections ins)
        {
            int VesselID = Convert.ToInt32(Session["VesselSessionID"].ToString());
            MooringRopeInspections model = new MooringRopeInspections();
            model.RopeInspectionList = CommonClass.MooringInspectionList(ins.Years,0,VesselID);
            model.YearList = CommonClass.GetYearsList();

            var record = model.RopeInspectionList.Count();
            int currPage = page == null ? 1 : Convert.ToInt32(page);

            TempData["CurrentPage"] = currPage;
            TempData["TotalRecords"] = record;

            TempData["Error"] = 1;

            model.RopeInspectionList = model.RopeInspectionList.Skip((CommonMethods.PAGESIZE * currPage) - CommonMethods.PAGESIZE).Take(CommonMethods.PAGESIZE).ToList();


            return View(model);
        }

        public ActionResult addlineinspection()
        {
            int VesselID = Convert.ToInt32(Session["VesselSessionID"].ToString());
            //VesselID = CommonClass.VesselSessionID;
            // MooringRopeInspections model = new MooringRopeInspections();
            assR.ChafeGuard = CommonClass.chafeGuardConditions();
            assR.AddInspectionList = CommonClass.AddLineInsList(0,VesselID);
            return View(assR);
        }
        //[HttpPost]
        //public ActionResult addlineinspection(MooringRopeInspection morins)
        //{
        //    return View();
        //}

[HttpGet]
        public ActionResult editinspection(int InspectionId)
        {
            int VesselID = Convert.ToInt32(Session["VesselSessionID"].ToString());
            inspectionid = InspectionId;
            //VesselID = CommonClass.VesselSessionID;
            int vesselid = Convert.ToInt32(VesselID);
            assR.ChafeGuard = CommonClass.chafeGuardConditions();
            assR.AddInspectionList = CommonClass.EditLineInsList(InspectionId,vesselid);

            assR.InspectBy = context.MooringRopeInspections.Where(x => x.InspectionId == InspectionId && x.VesselID == vesselid).Select(x => x.InspectBy).Distinct().SingleOrDefault();
            assR.InspectDate = context.MooringRopeInspections.Where(x => x.InspectionId == InspectionId && x.VesselID == vesselid).Select(x => x.InspectDate).Distinct().SingleOrDefault();
          
            return View(assR);
        }


        [HttpGet]
        public ActionResult viewinspection(int InspectionId)
        {
            int VesselID = Convert.ToInt32(Session["VesselSessionID"].ToString());
            inspectionid = InspectionId;
            //VesselID = CommonClass.VesselSessionID;
            int vesselid = Convert.ToInt32(VesselID);
            assR.ChafeGuard = CommonClass.chafeGuardConditions();
            assR.AddInspectionList = CommonClass.EditLineInsList(InspectionId, vesselid);

            assR.InspectBy = context.MooringRopeInspections.Where(x => x.InspectionId == InspectionId && x.VesselID == vesselid).Select(x => x.InspectBy).Distinct().SingleOrDefault();
            assR.InspectDate = context.MooringRopeInspections.Where(x => x.InspectionId == InspectionId && x.VesselID == vesselid).Select(x => x.InspectDate).Distinct().SingleOrDefault();

            return View(assR);
        }

        [HttpPost]
        [PreventSpam("InsertInspection", 1, 1)]
        public ActionResult InsertInspection(string inspections)
        {
            try
            {
                int VesselID = Convert.ToInt32(Session["VesselSessionID"].ToString());
                MooringRopeInspection morIns = new MooringRopeInspection();
                var Result = inspections;
                string json = Result.ToString();
                string dd = json;
                int vesselid = Convert.ToInt32(VesselID);
                int nxtinspctid = CommonClass.NextInspectionId(vesselid);


                var Json = JsonConvert.DeserializeObject<List<MooringRopeInspection>>(dd);
                foreach (var rootObject in Json)
                {
                    var IdPK = ((from asn in context.MooringRopeInspections.Where(x => x.VesselID == vesselid) select (int?)asn.Id).Max() ?? 0) + 1;


                    var image_1 = "";
                    var image_2 = "";

                    //==== Image save in folder

                    if (rootObject.Photo1 != "")
                    {
                        image_1 = VesselID + "_" + rootObject.Image1;

                        string xt = rootObject.Photo1.Replace("data:image/png;base64,", "");
                        // Convert Base64 String to byte[]
                        byte[] imageBytes = Convert.FromBase64String(xt);
                        MemoryStream ms = new MemoryStream(imageBytes, 0, imageBytes.Length);

                       // Stream strm = new MemoryStream(imageBytes);

                        string[] str1;
                        str1 = new string[4] { "jpg", "jpeg", "png", "GIF" };
                        string FileExtension = rootObject.Image1.Substring(rootObject.Image1.LastIndexOf('.') + 1).ToLower();

                        if (str1.Contains(FileExtension))
                        {
                            int compare = imageBytes.Length;
                            if (compare > 2000000)
                            {
                                TempData["Error"] = "Image size not to be greater than 2MB";
                                return RedirectToAction(Url.Action("Index", "LineInspection"));
                            }
                            // Convert byte[] to Image
                            ms.Write(imageBytes, 0, imageBytes.Length);
                            System.Drawing.Image image = System.Drawing.Image.FromStream(ms, true);
                            // image = image.GetThumbnailImage(400, 400, null, IntPtr.Zero);

                            //var image = Image.FromStream(strm);
                            //    var newWidth = (int)(image.Width * 0.5);
                            //    var newHeight = (int)(image.Height * 0.5);
                            //    var thumbnailImg = new Bitmap(newWidth, newHeight);
                            //    var thumbGraph = Graphics.FromImage(thumbnailImg);
                            //    thumbGraph.CompositingQuality = CompositingQuality.HighQuality;
                            //    thumbGraph.SmoothingMode = SmoothingMode.HighQuality;
                            //    thumbGraph.InterpolationMode = InterpolationMode.HighQualityBicubic;
                            //    var imageRectangle = new Rectangle(0, 0, newWidth, newHeight);
                            //    thumbGraph.DrawImage(image, imageRectangle);
                            //    //thumbnailImg.Save(targetPath, image.RawFormat);

                            //thumbnailImg.Save(Server.MapPath("~/images/inspectionimages/" + image_1 + ""), System.Drawing.Imaging.ImageFormat.Png);
                            image.Save(Server.MapPath("~/images/inspectionimages/" + image_1 + ""), System.Drawing.Imaging.ImageFormat.Png);
                        }
                        else
                        {
                            TempData["Error"] = "Invalid File Format !";

                            return RedirectToAction(Url.Action("Index", "LineInspection"));
                        }
                    }

                    if (rootObject.Photo2 != "")
                    {
                        image_2 = VesselID + "_" + rootObject.Image2;

                        string xt1 = rootObject.Photo2.Replace("data:image/png;base64,", "");
                        // Convert Base64 String to byte[]
                        byte[] imageBytes1 = Convert.FromBase64String(xt1);
                        MemoryStream ms1 = new MemoryStream(imageBytes1, 0, imageBytes1.Length);


                        string[] str1;
                        str1 = new string[4] { "jpg", "jpeg", "png", "GIF" };
                        string FileExtension = rootObject.Image2.Substring(rootObject.Image2.LastIndexOf('.') + 1).ToLower();

                        if (str1.Contains(FileExtension))
                        {
                            int compare = imageBytes1.Length;
                            if (compare > 2000000)
                            {
                                TempData["Error"] = "Image size not to be greater than 2MB";
                                return RedirectToAction(Url.Action("Index", "LineInspection"));
                            }
                            // Convert byte[] to Image
                            ms1.Write(imageBytes1, 0, imageBytes1.Length);
                            System.Drawing.Image image1 = System.Drawing.Image.FromStream(ms1, true);
                            image1.Save(Server.MapPath("~/images/inspectionimages/" + image_2 + ""), System.Drawing.Imaging.ImageFormat.Png);
                        }
                        else
                        {
                            TempData["Error"] = "Invalid File Format !";

                            return RedirectToAction (Url.Action("Index", "LineInspection"));

                        }
                    }

                    //==== Image save in folder



                    var ropeid = rootObject.RopeId;
                    int rpid = Convert.ToInt32(ropeid);

                    var winchid = rootObject.WinchId;
                    int wnchid = Convert.ToInt32(winchid);

                    using (SqlDataAdapter adp = new SqlDataAdapter("InsertInspection", con))
                    {
                        adp.SelectCommand.CommandType = CommandType.StoredProcedure;
                        adp.SelectCommand.Parameters.AddWithValue("@Id", IdPK);
                        adp.SelectCommand.Parameters.AddWithValue("@InspectBy", rootObject.InspectBy);
                        adp.SelectCommand.Parameters.AddWithValue("@InspectDate", rootObject.InspectDate);
                        adp.SelectCommand.Parameters.AddWithValue("@RopeId", rpid);
                        adp.SelectCommand.Parameters.AddWithValue("@WinchId", wnchid);
                        adp.SelectCommand.Parameters.AddWithValue("@ExternalRating_A", rootObject.ExternalRating_A);
                        adp.SelectCommand.Parameters.AddWithValue("@InternalRating_A", rootObject.InternalRating_A);
                        adp.SelectCommand.Parameters.AddWithValue("@AverageRating_A", rootObject.AverageRating_A);
                        adp.SelectCommand.Parameters.AddWithValue("@LengthOFAbrasion_A", rootObject.LengthOFAbrasion_A);
                        adp.SelectCommand.Parameters.AddWithValue("@DistanceOutboard_A", rootObject.DistanceOutboard_A);
                        adp.SelectCommand.Parameters.AddWithValue("@CutYarnCount_A", rootObject.CutYarnCount_A);
                        adp.SelectCommand.Parameters.AddWithValue("@LengthOFGlazing_A", rootObject.LengthOFGlazing_A);
                        adp.SelectCommand.Parameters.AddWithValue("@RopeTail", 0);
                        adp.SelectCommand.Parameters.AddWithValue("@ExternalRating_B", rootObject.ExternalRating_B);
                        adp.SelectCommand.Parameters.AddWithValue("@InternalRating_B", rootObject.InternalRating_B);
                        adp.SelectCommand.Parameters.AddWithValue("@AverageRating_B", rootObject.AverageRating_B);
                        adp.SelectCommand.Parameters.AddWithValue("@LengthOFAbrasion_B", rootObject.LengthOFAbrasion_B);
                        adp.SelectCommand.Parameters.AddWithValue("@DistanceOutboard_B", rootObject.DistanceOutboard_B);
                        adp.SelectCommand.Parameters.AddWithValue("@CutYarnCount_B", rootObject.CutYarnCount_B);
                        adp.SelectCommand.Parameters.AddWithValue("@LengthOFGlazing_B", rootObject.LengthOFGlazing_B);
                        adp.SelectCommand.Parameters.AddWithValue("@Chafe_guard_condition", rootObject.Chafe_guard_condition);
                        adp.SelectCommand.Parameters.AddWithValue("@Twist", rootObject.Twist);
                        adp.SelectCommand.Parameters.AddWithValue("@Photo1", DBNull.Value);
                        adp.SelectCommand.Parameters.AddWithValue("@Photo2", DBNull.Value);
                        adp.SelectCommand.Parameters.AddWithValue("@CreatedDate", DateTime.Now);
                        adp.SelectCommand.Parameters.AddWithValue("@CreatedBy", "Admin");

                        adp.SelectCommand.Parameters.AddWithValue("@ModifiedDate", DateTime.Now);
                        adp.SelectCommand.Parameters.AddWithValue("@ModifiedBy", "Admin");
                        adp.SelectCommand.Parameters.AddWithValue("@IsActive", true);
                        adp.SelectCommand.Parameters.AddWithValue("@NotificationId", 1);
                        //adp.SelectCommand.Parameters.AddWithValue("@Image1", rootObject.Image1);
                        //adp.SelectCommand.Parameters.AddWithValue("@Image2", rootObject.Image2);

                        adp.SelectCommand.Parameters.AddWithValue("@Image1", image_1);
                        adp.SelectCommand.Parameters.AddWithValue("@Image2", image_2);

                        adp.SelectCommand.Parameters.AddWithValue("@InspectionId", nxtinspctid);
                        adp.SelectCommand.Parameters.AddWithValue("@VesselID", vesselid);
                        DataTable dt = new DataTable();
                        adp.Fill(dt);

                        updateInspectionDueDate(rpid, vesselid, rootObject.InspectDate);
                    }

                }
            }
            catch { }

            TempData["Success"] = "Record successfully saved !";
           // RedirectToAction("Index");

            return Json(Url.Action("Index", "LineInspection"));
            //string[] Jsonn = Json.Select(x => x.ToString()).ToArray();



        }


        private void GenerateThumbnails(double scaleFactor, Stream sourcePath, string targetPath)
        {
            using (var image = Image.FromStream(sourcePath))
            {
                var newWidth = (int)(image.Width * scaleFactor);
                var newHeight = (int)(image.Height * scaleFactor);
                var thumbnailImg = new Bitmap(newWidth, newHeight);
                var thumbGraph = Graphics.FromImage(thumbnailImg);
                thumbGraph.CompositingQuality = CompositingQuality.HighQuality;
                thumbGraph.SmoothingMode = SmoothingMode.HighQuality;
                thumbGraph.InterpolationMode = InterpolationMode.HighQualityBicubic;
                var imageRectangle = new Rectangle(0, 0, newWidth, newHeight);
                thumbGraph.DrawImage(image, imageRectangle);
                thumbnailImg.Save(targetPath, image.RawFormat);
            }
        }

            private void updateInspectionDueDate(int ropeid, int vesseid,DateTime? inspectdt)
        {
            decimal avg = 0; string test = ""; int insdtcheck = 0;
            try
            {
                int VesselID = Convert.ToInt32(Session["VesselSessionID"].ToString());
                var Ropelist = context.MooringRopeDetails.Where(x => x.DeleteStatus == false && x.OutofServiceDate == null && x.VesselID==vesseid && x.RopeTail == 0).ToList();
                var RopeId = Ropelist.Where(x => x.RopeId == ropeid && x.OutofServiceDate == null && x.VesselID == vesseid && x.DeleteStatus == false).FirstOrDefault();
                var InspecSetting = context.tblRopeInspectionSettings.ToList();

                var ratingcheck = InspecSetting.Where(x => x.MooringRopeType == RopeId.RopeTypeId && x.ManufacturerType == RopeId.ManufacturerId).FirstOrDefault();

                if (ratingcheck != null)
                {
                    try
                    {
                        decimal[] array = new decimal[7] { ratingcheck.Rating1, ratingcheck.Rating2, ratingcheck.Rating3, ratingcheck.Rating4, ratingcheck.Rating5, ratingcheck.Rating6, ratingcheck.Rating7 };

                        var nearest = array.OrderBy(x => Math.Abs((long)x - avg)).First();

                        string rating = "Rating" + avg;

                        int near = Convert.ToInt32(nearest);
                        SqlDataAdapter pp = new SqlDataAdapter("select " + rating + " from tblRopeInspectionSetting where mooringropetype=" + RopeId.RopeTypeId + " and manufacturertype=" + RopeId.ManufacturerId + "", con);
                        System.Data.DataTable rtc = new System.Data.DataTable();
                        pp.Fill(rtc);
                        if (rtc.Rows.Count > 0)
                        {
                            decimal perchk = Convert.ToDecimal(rtc.Rows[0][0]) * 30 / 100;
                            perchk = perchk * 100;
                            //near = Convert.ToInt32(rtc.Rows[0][0]);
                            near = Convert.ToInt32(perchk);
                        }



                        DateTime notidueMonth = Convert.ToDateTime(inspectdt).Date.AddDays(near);

                        SqlDataAdapter adp = new SqlDataAdapter("update MooringRopeDetail set InspectionDueDate='" + notidueMonth + "' where RopeId=" + ropeid + " and VesselID=" + vesseid + "", con);
                        DataTable dt = new DataTable();
                        adp.Fill(dt);
                       
                    }
                    catch { }
                }
            }
            catch { }
        }

        [HttpPost]
        //[PreventSpam("editinspection", 1, 1)]
        public ActionResult editinspection(string inspections)
        {
            try
            {
                int VesselID = Convert.ToInt32(Session["VesselSessionID"].ToString());
                MooringRopeInspection morIns = new MooringRopeInspection();
                var Result = inspections;
                string json = Result.ToString();
                string dd = json;
                int vesselid = Convert.ToInt32(VesselID);


                try
                {
                    SqlDataAdapter adp123 = new SqlDataAdapter("drop table tempInspection", con);
                    DataTable dd123 = new DataTable();
                    adp123.Fill(dd123);
                }
                catch { }

                SqlDataAdapter adp2 = new SqlDataAdapter("select * into tempInspection from ( select * from MooringRopeInspection where VesselID=" + vesselid + " and InspectionId=" + inspectionid + ") as dd", con);
                //adp2.SelectCommand.CommandType = CommandType.StoredProcedure;
                //adp2.SelectCommand.Parameters.AddWithValue("@InspectionId", inspectionid);
                //adp2.SelectCommand.Parameters.AddWithValue("@VesselId", vesselid);
                DataTable pp2 = new DataTable();
                adp2.Fill(pp2);


             


                SqlDataAdapter adp1 = new SqlDataAdapter("delete from mooringropeinspection where inspectionid=" + inspectionid + " and VesselId=" + vesselid + "", con);
                DataTable dt1 = new DataTable();
                adp1.Fill(dt1);




                //int nxtinspctid = cls.NextInspectionId(vesselid);
                int? nxtinspctid = inspectionid;
                var Json = JsonConvert.DeserializeObject<List<MooringRopeInspection>>(dd);
                foreach (var rootObject in Json)
                {
                    var IdPK = ((from asn in context.MooringRopeInspections.Where(x => x.VesselID == vesselid) select (int?)asn.Id).Max() ?? 0) + 1;

                    var image_1 = "";
                    var image_2 = "";

                    //==== Image save in folder

                    if (rootObject.Photo1 != "")
                    {
                        image_1 = VesselID + "_" + rootObject.Image1;

                        string xt = rootObject.Photo1.Replace("data:image/png;base64,", "");
                        // Convert Base64 String to byte[]
                        byte[] imageBytes = Convert.FromBase64String(xt);
                        MemoryStream ms = new MemoryStream(imageBytes, 0, imageBytes.Length);


                        string[] str1;
                        str1 = new string[4] { "jpg", "jpeg", "png", "GIF" };
                        string FileExtension = rootObject.Image1.Substring(rootObject.Image1.LastIndexOf('.') + 1).ToLower();

                        if (str1.Contains(FileExtension))
                        {
                            int compare = imageBytes.Length;
                            if (compare > 2000000)
                            {
                                TempData["Error"] = "Image size not to be greater than 2MB";
                                return RedirectToAction(Url.Action("Index", "LineInspection"));
                            }

                            // Convert byte[] to Image
                            ms.Write(imageBytes, 0, imageBytes.Length);
                            System.Drawing.Image image = System.Drawing.Image.FromStream(ms, true);
                            image.Save(Server.MapPath("~/images/inspectionimages/" + image_1 + ""), System.Drawing.Imaging.ImageFormat.Png);
                        }
                        else
                        {
                            TempData["Error"] = "Invalid File Format !";

                            return RedirectToAction(Url.Action("Index", "LineInspection"));
                        }
                    }

                    if (rootObject.Photo2 != "")
                    {
                        image_2 = VesselID + "_" + rootObject.Image2;
                        string xt1 = rootObject.Photo2.Replace("data:image/png;base64,", "");
                        // Convert Base64 String to byte[]
                        byte[] imageBytes1 = Convert.FromBase64String(xt1);
                        MemoryStream ms1 = new MemoryStream(imageBytes1, 0, imageBytes1.Length);


                        string[] str1;
                        str1 = new string[4] { "jpg", "jpeg", "png", "GIF" };
                        string FileExtension = rootObject.Image2.Substring(rootObject.Image2.LastIndexOf('.') + 1).ToLower();

                        if (str1.Contains(FileExtension))
                        {
                            int compare = imageBytes1.Length;
                            if (compare > 2000000)
                            {
                                TempData["Error"] = "Image size not to be greater than 2MB";
                                return RedirectToAction(Url.Action("Index", "LineInspection"));
                            }
                            // Convert byte[] to Image
                            ms1.Write(imageBytes1, 0, imageBytes1.Length);
                            System.Drawing.Image image1 = System.Drawing.Image.FromStream(ms1, true);
                           
                            image1.Save(Server.MapPath("~/images/inspectionimages/" + image_2 + ""), System.Drawing.Imaging.ImageFormat.Png);
                        }
                        else
                        {
                            TempData["Error"] = "Invalid File Format !";

                            return RedirectToAction(Url.Action("Index", "LineInspection"));

                        }
                    }

                    //==== Image save in folder



                    var ropeid = rootObject.RopeId;
                    int rpid = Convert.ToInt32(ropeid);

                    var winchid = rootObject.WinchId;
                    int wnchid = Convert.ToInt32(winchid);

                    using (SqlDataAdapter adp = new SqlDataAdapter("InsertInspection", con))
                    {
                        adp.SelectCommand.CommandType = CommandType.StoredProcedure;
                        adp.SelectCommand.Parameters.AddWithValue("@Id", IdPK);
                        adp.SelectCommand.Parameters.AddWithValue("@InspectBy", rootObject.InspectBy);
                        adp.SelectCommand.Parameters.AddWithValue("@InspectDate", rootObject.InspectDate);
                        adp.SelectCommand.Parameters.AddWithValue("@RopeId", rpid);
                        adp.SelectCommand.Parameters.AddWithValue("@WinchId", wnchid);
                        adp.SelectCommand.Parameters.AddWithValue("@ExternalRating_A", rootObject.ExternalRating_A);
                        adp.SelectCommand.Parameters.AddWithValue("@InternalRating_A", rootObject.InternalRating_A);
                        adp.SelectCommand.Parameters.AddWithValue("@AverageRating_A", rootObject.AverageRating_A);
                        adp.SelectCommand.Parameters.AddWithValue("@LengthOFAbrasion_A", rootObject.LengthOFAbrasion_A);
                        adp.SelectCommand.Parameters.AddWithValue("@DistanceOutboard_A", rootObject.DistanceOutboard_A);
                        adp.SelectCommand.Parameters.AddWithValue("@CutYarnCount_A", rootObject.CutYarnCount_A);
                        adp.SelectCommand.Parameters.AddWithValue("@LengthOFGlazing_A", rootObject.LengthOFGlazing_A);
                        adp.SelectCommand.Parameters.AddWithValue("@RopeTail", 0);
                        adp.SelectCommand.Parameters.AddWithValue("@ExternalRating_B", rootObject.ExternalRating_B);
                        adp.SelectCommand.Parameters.AddWithValue("@InternalRating_B", rootObject.InternalRating_B);
                        adp.SelectCommand.Parameters.AddWithValue("@AverageRating_B", rootObject.AverageRating_B);
                        adp.SelectCommand.Parameters.AddWithValue("@LengthOFAbrasion_B", rootObject.LengthOFAbrasion_B);
                        adp.SelectCommand.Parameters.AddWithValue("@DistanceOutboard_B", rootObject.DistanceOutboard_B);
                        adp.SelectCommand.Parameters.AddWithValue("@CutYarnCount_B", rootObject.CutYarnCount_B);
                        adp.SelectCommand.Parameters.AddWithValue("@LengthOFGlazing_B", rootObject.LengthOFGlazing_B);
                        adp.SelectCommand.Parameters.AddWithValue("@Chafe_guard_condition", rootObject.Chafe_guard_condition);
                        adp.SelectCommand.Parameters.AddWithValue("@Twist", rootObject.Twist);
                        adp.SelectCommand.Parameters.AddWithValue("@Photo1", DBNull.Value);
                        adp.SelectCommand.Parameters.AddWithValue("@Photo2", DBNull.Value);
                        adp.SelectCommand.Parameters.AddWithValue("@CreatedDate", DateTime.Now);
                        adp.SelectCommand.Parameters.AddWithValue("@CreatedBy", "Admin");

                        adp.SelectCommand.Parameters.AddWithValue("@ModifiedDate", DateTime.Now);
                        adp.SelectCommand.Parameters.AddWithValue("@ModifiedBy", "Admin");
                        adp.SelectCommand.Parameters.AddWithValue("@IsActive", true);
                        adp.SelectCommand.Parameters.AddWithValue("@NotificationId", 1);
                        //adp.SelectCommand.Parameters.AddWithValue("@Image1", rootObject.Image1);
                        //adp.SelectCommand.Parameters.AddWithValue("@Image2", rootObject.Image2);

                        adp.SelectCommand.Parameters.AddWithValue("@Image1", image_1);
                        adp.SelectCommand.Parameters.AddWithValue("@Image2", image_2);

                        adp.SelectCommand.Parameters.AddWithValue("@InspectionId", nxtinspctid);
                        adp.SelectCommand.Parameters.AddWithValue("@VesselID", vesselid);
                        DataTable dt = new DataTable();
                        adp.Fill(dt);

                        updateInspectionDueDate(rpid, vesselid, rootObject.InspectDate);
                    }

                }


                //=== Update Images in DB

           

                SqlDataAdapter adp11 = new SqlDataAdapter("select * from mooringropeinspection where VesselID=" + vesselid + " and InspectionId=" + inspectionid + "", con);
                DataTable dd11 = new DataTable();
                adp11.Fill(dd11);

                for (int i = 0; i < dd11.Rows.Count; i++)
                {
                    string image1 = string.IsNullOrEmpty(dd11.Rows[i]["Image1"].ToString()).ToString();
                    string image2 = string.IsNullOrEmpty(dd11.Rows[i]["Image2"].ToString()).ToString();
                    int ropeid = Convert.ToInt32(dd11.Rows[i]["RopeId"]);

                    if(image1 == "True")
                    {
                        SqlDataAdapter adp115 = new SqlDataAdapter("select * from tempinspection where RopeId=" + ropeid + "", con);
                        DataTable dd115 = new DataTable();
                        adp115.Fill(dd115);

                        string img1 = string.IsNullOrEmpty(dd115.Rows[0]["Image1"].ToString()).ToString();
                        if (img1 == "False")
                        {
                            string img = dd115.Rows[0]["Image1"].ToString();
                            SqlDataAdapter adp1151 = new SqlDataAdapter("Update mooringropeinspection set Image1='"+ img + "'  where RopeId=" + ropeid + " and vesselId="+vesselid+" and inspectionId="+inspectionid+"", con);
                            DataTable dd1151 = new DataTable();
                            adp1151.Fill(dd1151);
                        }

                    }
                    if (image2 == "True")
                    {
                        SqlDataAdapter adp115 = new SqlDataAdapter("select * from tempinspection where RopeId=" + ropeid + "", con);
                        DataTable dd115 = new DataTable();
                        adp115.Fill(dd115);

                        string img2 = string.IsNullOrEmpty(dd115.Rows[0]["Image2"].ToString()).ToString();
                        if (img2 == "False")
                        {
                            string img = dd115.Rows[0]["Image2"].ToString();
                            SqlDataAdapter adp1151 = new SqlDataAdapter("Update mooringropeinspection set Image2='" + img + "'  where RopeId=" + ropeid + "and vesselId=" + vesselid + " and inspectionId=" + inspectionid + "", con);
                            DataTable dd1151 = new DataTable();
                            adp1151.Fill(dd1151);
                        }

                    }

                }


                SqlDataAdapter adp12 = new SqlDataAdapter("drop table tempInspection", con);
                DataTable dd12 = new DataTable();
                adp12.Fill(dd12);

            }
            catch (Exception ex) { }

            TempData["Success"] = "Record successfully Updated !";
            //return  RedirectToAction("Index");
            return Json(Url.Action("Index", "LineInspection"));
            //string[] Jsonn = Json.Select(x => x.ToString()).ToArray();
        }

        [HttpPost]
        public JsonResult DeleteImage(int RopeId,int Image)
        {

            int VesselID = Convert.ToInt32(Session["VesselSessionID"].ToString());
            int vesselid = Convert.ToInt32(VesselID);


            if (Image == 1)
            {
                SqlDataAdapter adp = new SqlDataAdapter("Update mooringropeinspection set Image1=null  where RopeId=" + RopeId + "and vesselId=" + vesselid + " and inspectionId=" + inspectionid + "", con);
                DataTable dt = new DataTable();
                adp.Fill(dt);
            }
            if (Image == 2)
            {
                SqlDataAdapter adp = new SqlDataAdapter("Update mooringropeinspection set Image2=null  where RopeId=" + RopeId + "and vesselId=" + vesselid + " and inspectionId=" + inspectionid + "", con);
                DataTable dt = new DataTable();
                adp.Fill(dt);
            }

            return Json(new { Result = true }, JsonRequestBehavior.AllowGet);
          

            //return Json(new { Result = true, outboard = "", asswinch = "", location = "", outboard1 = "", noofOp = "", rnghrs = "" }, JsonRequestBehavior.AllowGet);
        }

       
        public ActionResult Delete(int InspectionId)
        {
            try
            {
                int VesselID = Convert.ToInt32(Session["VesselSessionID"].ToString());
                int vesselid = Convert.ToInt32(VesselID);
                SqlDataAdapter adp1 = new SqlDataAdapter("select InspectDate from MooringRopeInspection where inspectionid='" + InspectionId + "' and IsActive=1 and RopeTail=0 and vesselId=" + vesselid + "", con);
                DataTable dt1 = new DataTable();
                adp1.Fill(dt1);
                DateTime inspectdate = Convert.ToDateTime(dt1.Rows[0][0]);


                var dd = Convert.ToDateTime(inspectdate).ToString("yyyy-MM-dd");

                SqlDataAdapter adp = new SqlDataAdapter("update MooringRopeInspection set IsActive=0,ModifiedDate=GETDATE(),ModifiedBy='Admin' where InspectionId='" + InspectionId + "' and vesselId=" + vesselid + "", con);
                DataTable dt = new DataTable();
                adp.Fill(dt);



                UpdInspDate(InspectionId, dd);


                TempData["Success"] = "Record successfully deleted !";
            }
            catch { }
            return RedirectToAction("Index");
        }


        private void UpdInspDate(int inspectionid, string inspectdate)
        {
            try
            {
                int VesselID = Convert.ToInt32(Session["VesselSessionID"].ToString());
                int vesselid = Convert.ToInt32(VesselID);
                //SqlDataAdapter adp = new SqlDataAdapter("select * from mooringropeinspection where InspectionId=" + inspectionid + "", sc.con);
                SqlDataAdapter adp = new SqlDataAdapter("select * from mooringropeinspection where InspectDate='" + inspectdate + "' and RopeTail = 0 and VesselID="+ vesselid + "", con);
                DataTable dt = new DataTable();
                adp.Fill(dt);
                decimal avg = 0;
                var Ropelist = context.MooringRopeDetails.Where(x => x.DeleteStatus == false && x.OutofServiceDate == null && x.RopeTail == 0 && x.VesselID==vesselid).ToList();
                var InspecSetting = context.tblRopeInspectionSettings.ToList();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    int ropeid = Convert.ToInt32(dt.Rows[i]["RopeId"]);
                    int firstvalue = Convert.ToInt32(dt.Rows[i]["ExternalRating_A"]);
                    int secondvalue = Convert.ToInt32(dt.Rows[i]["InternalRating_A"]);

                    decimal finalvalue = (firstvalue + secondvalue);
                    finalvalue = finalvalue / 2;

                    finalvalue = Math.Round(finalvalue, 0, MidpointRounding.AwayFromZero);

                    int avrgratingA = Convert.ToInt32(finalvalue);


                    //int firstvalue1 = Convert.ToInt32(item.ExternalRating_B);
                    // int secondvalue1 = Convert.ToInt32(item.InternalRating_B);
                    int firstvalue1 = Convert.ToInt32(dt.Rows[i]["ExternalRating_B"]);
                    int secondvalue1 = Convert.ToInt32(dt.Rows[i]["InternalRating_B"]);

                    decimal finalvalue1 = (firstvalue1 + secondvalue1);
                    finalvalue1 = finalvalue1 / 2;

                    finalvalue1 = Math.Round(finalvalue1, 0, MidpointRounding.AwayFromZero);

                    int avrgratingB = Convert.ToInt32(finalvalue1);

                    //DateTime addmonth =Convert.ToDateTime(ss.InspectDate);

                    decimal avRA = avrgratingA;
                    decimal avRB = avrgratingB;

                    if (avRA >= avRB)
                    {
                        avg = avRA;
                    }
                    if (avRB >= avRA)
                    {
                        avg = avRB;
                    }

                    var RopeId = Ropelist.Where(x => x.RopeId == Convert.ToInt32(dt.Rows[i]["RopeId"]) && x.RopeTail == 0 && x.VesselID == vesselid).FirstOrDefault();
                    //var ratingcheck = InspecSetting.Where(x => x.MooringRopeType == RopeId.Id && x.ManufacturerType == RopeId.ManufacturerId).FirstOrDefault();

                    var ratingcheck = InspecSetting.Where(x => x.MooringRopeType == RopeId.RopeTypeId && x.ManufacturerType == RopeId.ManufacturerId).FirstOrDefault();


                    var certinumber = RopeId.CertificateNumber; //Ropelist.Where(x => x.Id == Convert.ToInt32(dt.Rows[i]["RopeId"]) && x.RopeTail == 0).Select(x => x.CertificateNumber).SingleOrDefault();




                    if (ratingcheck != null)
                    {
                        try
                        {
                            decimal[] array = new decimal[7] { ratingcheck.Rating1, ratingcheck.Rating2, ratingcheck.Rating3, ratingcheck.Rating4, ratingcheck.Rating5, ratingcheck.Rating6, ratingcheck.Rating7 };

                            var nearest = array.OrderBy(x => Math.Abs((long)x - avg)).First();

                            string rating = "Rating" + avg;

                            int near = Convert.ToInt32(nearest);
                            SqlDataAdapter pp = new SqlDataAdapter("select " + rating + " from tblRopeInspectionSetting where mooringropetype=" + RopeId.RopeTypeId + " and manufacturertype=" + RopeId.ManufacturerId + "", con);
                            System.Data.DataTable rtc = new System.Data.DataTable();
                            pp.Fill(rtc);
                            if (rtc.Rows.Count > 0)
                            {
                                decimal perchk = Convert.ToDecimal(rtc.Rows[0][0]) * 30 / 100;
                                perchk = perchk * 100;
                                //near = Convert.ToInt32(rtc.Rows[0][0]);
                                near = Convert.ToInt32(perchk);
                            }

                            try
                            {
                                var inspectdate1 = "";
                                SqlDataAdapter adp11 = new SqlDataAdapter("select  Max(InspectDate) from MooringRopeInspection where RopeId ='" + Convert.ToInt32(dt.Rows[i]["RopeId"]) + "' and InspectionId !='" + Convert.ToInt32(dt.Rows[i]["InspectionId"]) + "' and IsActive=1 and RopeTail=0 and vesselId=" + vesselid + "", con);
                                DataTable dt11 = new DataTable();
                                adp11.Fill(dt11);
                                if (dt11.Rows.Count > 0)
                                {
                                    //inspectdate1 = Convert.ToDateTime(dt11.Rows[0][0]);
                                    inspectdate1 = dt11.Rows[0][0] == DBNull.Value ? null : dt11.Rows[0][0].ToString();

                                    if (inspectdate1 == null)
                                    {
                                        var ssss = Ropelist.Where(x => x.Id == Convert.ToInt32(dt.Rows[i]["RopeId"])).Select(x => x.InstalledDate).SingleOrDefault();

                                        inspectdate1 = (ssss) == null ? null : (ssss).ToString();
                                    }
                                }
                                else
                                {
                                    var ssss = Ropelist.Where(x => x.Id == Convert.ToInt32(dt.Rows[i]["RopeId"])).Select(x => x.InstalledDate).SingleOrDefault();

                                    inspectdate1 = (ssss) == null ? null : (ssss).ToString();
                                    //inspectdate1 = Convert.ToDateTime(ssss);
                                }




                                DateTime notidueMonth = Convert.ToDateTime(inspectdate1).Date.AddDays(near);
                                int rpid = Convert.ToInt32(dt.Rows[i]["RopeId"]);


                                DateTime crntdt = DateTime.Now;
                                if (notidueMonth <= crntdt)
                                {
                                    notidueMonth = DateTime.Now;
                                }
                                var result = context.MooringRopeDetails.SingleOrDefault(b => b.Id == rpid && b.IsActive == true && b.RopeTail == 0 && b.VesselID==vesselid);
                                if (result != null)
                                {
                                    result.InspectionDueDate = notidueMonth;
                                    result.ModifiedBy = "Admin";
                                    result.ModifiedDate = DateTime.Now;
                                    context.SaveChanges();
                                }
                            }
                            catch { }
                        }
                        catch (Exception ex) { }
                    }

                }
            }
            catch { }
        }

        public ActionResult DownloadExcelSheet()
        {
            int VesselID = Convert.ToInt32(Session["VesselSessionID"].ToString());

            //DataSet dataSet = new DataSet();
            DataSet dataSet = null;
            dataSet = new DataSet("General");
            dataSet.Locale = System.Threading.Thread.CurrentThread.CurrentCulture;

            string qry = "RopeInspection";
            SqlDataAdapter sda = new SqlDataAdapter(qry, con);
            sda.SelectCommand.CommandType = CommandType.StoredProcedure;
            sda.SelectCommand.Parameters.AddWithValue("@RopeTail", 0);
            sda.SelectCommand.Parameters.AddWithValue("@VesselId", VesselID);
            System.Data.DataTable datatbl = new System.Data.DataTable();

            sda.Fill(datatbl);

            datatbl.Columns.Remove("WinchId");
            datatbl.Columns.Remove("RopeId");

            datatbl.Columns.Add("External_A", typeof(string));
            datatbl.Columns.Add("Internal_ A", typeof(string));
            datatbl.Columns.Add("Average_A", typeof(string));
            datatbl.Columns.Add("Length_of_Abrasion_A", typeof(string));
            datatbl.Columns.Add("Distance_from_outboard_eye_A", typeof(string));
            datatbl.Columns.Add("Cut_yard_counted_A", typeof(string));
            datatbl.Columns.Add("Length_of_glazing_A", typeof(string));

            datatbl.Columns.Add("External_B", typeof(string));
            datatbl.Columns.Add("Internal_B", typeof(string));
            datatbl.Columns.Add("Average_B", typeof(string));
            datatbl.Columns.Add("Length_of_Abrasion_B", typeof(string));
            datatbl.Columns.Add("Distance_from_outboard_eye_B", typeof(string));
            datatbl.Columns.Add("Cut_yard_counted_B", typeof(string));
            datatbl.Columns.Add("Length_of_glazing_B", typeof(string));

            datatbl.Columns.Add("Chafe_Guard", typeof(string));
            datatbl.Columns.Add("Twist", typeof(string));

            dataSet.Tables.Add(datatbl);

           // string fileName = "DigiMoor_X7_InspectionSheet_" + DateTime.Now.ToString("dd-MMM-yyyy")+".xlsx";
            string fileName = "DigiMoor_X7_InspectionSheet";
            using (XLWorkbook wb = new XLWorkbook())
                {
                    foreach (DataTable dt in dataSet.Tables)
                    {
                        //Add DataTable as Worksheet.
                        wb.Worksheets.Add(dt);
                    }

                    //Export the Excel file.

                    DateTime today = DateTime.Today;
                    string HeaderName = fileName + "_" + today.ToString("dd-MMM-yyyy");

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

           
            return RedirectToAction("Index");
        }
    }
}