using MenuLayer;
using Reports;
using Shipment49Web.Areas.MooringTail.Models;
using Shipment49Web.Areas.MSPS.Models;
using Shipment49Web.Common;
using Shipment49Web.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Shipment49Web.Areas.MooringTail.Controllers
{
    [Authorize]
    public class MooringTailController : BaseController
    {
        MorringOfficeEntities context = new MorringOfficeEntities();
        SqlConnection con = ConnectionBulder.con;
       // CommonClass cls = new CommonClass();
        // GET: MooringLine/MooringLine
        //public static string VesselID;

       // public static int VesselID { get; set; }
        public MooringTailController()
        {
            
        }

        public ActionResult Index()
        {
            int VesselID = Convert.ToInt32(Session["VesselSessionID"].ToString());
            using (ShipmentContaxt sc1 = new ShipmentContaxt())
            {
               
                MooringRopeDetails model = new MooringRopeDetails();
                var ilist = new ShipmentContaxt()
                      .MultipleResults("[dbo].[GetMooringRopeDetailList] 1,'" + VesselID + "','RopeTail'")
                   .With<MooringRopeDetails>()
                   .With<MooringRopeDetails>()
                   .Execute();
                model.MooringLineList = (List<MooringRopeDetails>)ilist[0];
                model.MooringLineList1 = (List<MooringRopeDetails>)ilist[1];
                //List<TotalCount> totobj = (List<TotalCount>)ilist[1];
                //model.Total = totobj.FirstOrDefault().Total;
                //ViewBag.TotalCount = model.Total;
                //var pager = new Pager(Convert.ToInt32(model.Total), 0);
                //model.Pager = pager;
                return View(model);
            }
        }

        public ActionResult addmooringinfo()
        {
            //using (MorringOfficeEntities entities = new MorringOfficeEntities())
            //{
            //    MooringRopeDetails mropdetls = new MooringRopeDetails();

            //    foreach (var v in entities.MooringRopeTypes.OrderBy(p => p.RopeType).ToList())
            //        mropdetls.MooringRopeTypeLists.Add(new SelectListItem() { Text = v.RopeType, Value = v.Id.ToString() });

            //    foreach (var v in entities.tblCommons.Where(u => u.Type == 1).OrderBy(p => p.Name).ToList())
            //        mropdetls.ManufacturerTypeLists.Add(new SelectListItem() { Text = v.Name, Value = v.Id.ToString() });

            //    mropdetls.RopeTaggings = mropdetls.RopeTaggings;

            //    return View(mropdetls);
            //}
            ModelState.Clear();
            var userinfo = new Reports.MooringRopeDetail();
            return View(userinfo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [PreventSpam("addmooringinfotail", 3, 1)]
        public ActionResult addmooringinfo(MooringRopeDetail mropdtls, HttpPostedFileBase file)
        {
            int VesselID = Convert.ToInt32(Session["VesselSessionID"].ToString());
            int vesselid = Convert.ToInt32(VesselID);
            using (MorringOfficeEntities entities = new MorringOfficeEntities())
            {

                //---Add
                if (mropdtls.Id == 0)
                {
                    var duplicatecheck = entities.MooringRopeDetails.Where(x => x.UniqueID == mropdtls.UniqueID && x.RopeTail == 1 && x.VesselID== vesselid).Count();

                    if (duplicatecheck == 0)
                    {

                        MooringRopeDetail addinDB = new MooringRopeDetail();


                        var ropeid = ((from mrop in context.MooringRopeDetails.Where(x => x.VesselID == vesselid) select (int?)mrop.Id).Max() ?? 0) + 1;

                        addinDB.RopeTypeId = mropdtls.RopeTypeId;
                        addinDB.RopeConstruction = mropdtls.RopeConstruction;
                        addinDB.DiaMeter = mropdtls.DiaMeter;
                        addinDB.Length = mropdtls.Length;
                        addinDB.MBL = mropdtls.MBL;
                        addinDB.LDBF = mropdtls.LDBF;
                        addinDB.WLL = mropdtls.WLL;

                        addinDB.ManufacturerId = mropdtls.ManufacturerId;
                        addinDB.CertificateNumber = mropdtls.CertificateNumber;
                        addinDB.UniqueID = mropdtls.UniqueID;
                        addinDB.ReceivedDate = mropdtls.ReceivedDate;


                        if (mropdtls.IsRopeInstalled == "Yes")
                        {
                            if (mropdtls.InstalledDate == null)
                            {
                                TempData["Error"] = "Please fill install date !";
                                return View(mropdtls);
                            }
                            else
                            {
                                addinDB.InstalledDate = mropdtls.InstalledDate;

                                addinDB.InspectionDueDate = CommonClass.updateinspecionduedate1(addinDB.InstalledDate, mropdtls.RopeTypeId, mropdtls.ManufacturerId);
                            }
                        }
                        if (mropdtls.IsRopeInstalled == "No")
                        {
                            addinDB.InstalledDate = null;
                        }
                        addinDB.RopeTagging = mropdtls.RopeTagging;
                        addinDB.StartCounterHours = mropdtls.StartCounterHours;
                        addinDB.CurrentRunningHours = mropdtls.StartCounterHours;
                        addinDB.Remarks = mropdtls.Remarks;
                        addinDB.VesselID = Convert.ToInt32(VesselID);
                        addinDB.CreatedDate = DateTime.Now;
                        addinDB.RopeTail = 1;
                        addinDB.IsActive = true;
                        addinDB.DeleteStatus = false;
                        addinDB.Id = ropeid;
                        addinDB.RopeId = ropeid;
                        entities.MooringRopeDetails.Add(addinDB);


                        if (file != null)
                        {

                            Random randon = new Random();
                            int num = randon.Next(10000);
                            string FileName = Path.GetFileNameWithoutExtension(file.FileName);
                            //string FileExtension = Path.GetExtension(wbtRecord.ImageFile.FileName);        
                            //FileName = DateTime.Now.ToString("yyyyMMdd") + "-" + FileName.Trim() + FileExtension;   

                            string fileExtention = Path.GetExtension(file.FileName);
                            string fileName = Path.GetFileName(file.FileName);
                            var withoutextnsn = Path.GetFileNameWithoutExtension(fileName) + num;

                            string[] str1;
                            str1 = new string[26] { "jpg", "jpeg", "png", "pps", "ppt", "pptx", "xls", "xlsm", "xlsx", "doc", "docx", "pdf", "rtf", "txt", "gif", "wav", "mid", "midi", "wma", "mp3", "mp4", "ogg", "rma", "avi", "divx", "wmv" };
                            string FileExtension = file.FileName.Substring(file.FileName.LastIndexOf('.') + 1).ToLower();

                            if (str1.Contains(FileExtension))
                            {

                                // string size = FormatSize(trngattach.ImageFile.ContentLength);
                                //size = size.Remove(size.Length - 2);
                                //int sizecheck = trngattach.ImageFile.ContentLength;

                                int compare = file.ContentLength;

                                if (compare > 5000000)
                                {
                                    TempData["Error"] = "Attachment size not to be greater than 5MB";
                                    return RedirectToAction("Index");
                                }

                                fileName = withoutextnsn + fileExtention;
                                string UploadPath = "~/images/AttachFiles/";

                                string pth = fileName;
                                //trngattach.AttachmentPath = UploadPath + fileName;
                                string origPath = Server.MapPath("~/images/AttachFiles/");
                                var originalFilePath = Path.Combine(origPath, fileName);

                                file.SaveAs(originalFilePath);

                                var tuple = CommonClass.Getmaxid(0, 0, 0, VesselID);

                                int idPk = tuple.Item1;
                                int rpid = tuple.Item2;

                                CommonClass.InsertRopeAttachment(idPk, pth, rpid, 1, "RopeTail",0, VesselID);
                            }
                        }
                    }
                    else
                    {
                        TempData["Error"] = "UniqueID already exists !";
                        return View(mropdtls);
                    }
                }
                //Update
                else
                {

                    ///copy code from desktop
                    ///
                    #region Desktop Code as it is

                    DateTime? instaldate = null; DateTime? instaldate1 = null; DateTime? recedate3 = null;
                    DateTime? recedate = null; DateTime? recedate1 = null; DateTime? recedate2 = null;

                    decimal crntRhrs = 0;
                    SqlDataAdapter adp11 = new SqlDataAdapter("select StartCounterHours,CurrentRunningHours from mooringropedetail where id =" + mropdtls.Id + " and VesselId='" + VesselID + "'", con);
                    DataTable dt11 = new DataTable();
                    adp11.Fill(dt11);
                    if (dt11.Rows.Count > 0)
                    {
                        // int cntr = Convert.ToInt32(dt11.Rows[0][0] == DBNull.Value ? 0 : dt11.Rows[0][0]);
                        decimal cntr = Convert.ToDecimal(dt11.Rows[0][0] == DBNull.Value ? 0 : dt11.Rows[0][0]);
                        //int cntr = Convert.ToInt32(dt11.Rows[0][0]);
                        crntRhrs = Convert.ToDecimal(dt11.Rows[0][1] == DBNull.Value ? 0 : dt11.Rows[0][1]);
                        // crntRhrs = Convert.ToInt32(dt11.Rows[0][1]);
                        //int cntr1 = Convert.ToInt32(moorwinch.CurrentRunningHours);
                        decimal cntr1 = Convert.ToDecimal(mropdtls.StartCounterHours);

                        if (cntr > cntr1)
                        {
                            cntr = cntr - cntr1;
                            crntRhrs = crntRhrs - cntr;
                        }
                        else if (cntr < cntr1)
                        {
                            cntr = cntr1 - cntr;
                            crntRhrs = crntRhrs + cntr;
                        }
                    }

                    //SqlDataAdapter adp2 = new SqlDataAdapter("select * from MooringRopeInspection where RopeId=" + moorwinch.Id + "", sc.con);
                    //DataTable dt2 = new DataTable();
                    //adp2.Fill(dt2);
                    //if (dt2.Rows.Count > 0)
                    //{
                    //    MessageBox.Show("This rope not update because this rope already inspect", "Mooring Rope", MessageBoxButton.OK, MessageBoxImage.Information);
                    //}
                    //else
                    //{

                    if (mropdtls.IsRopeInstalled == "No")
                    {
                        mropdtls.InstalledDate = null;
                        mropdtls.InspectionDueDate = null; ;
                    }
                    else
                    {
                        try
                        {
                            SqlDataAdapter adp = new SqlDataAdapter("select * from tblRopeInspectionSetting where MooringRopeType=" + mropdtls.RopeTypeId + " and ManufacturerType=" + mropdtls.ManufacturerId + "", con);
                            DataTable dt = new DataTable();
                            adp.Fill(dt);
                            if (dt.Rows.Count > 0)
                            {
                                decimal rating1 = Convert.ToDecimal(dt.Rows[0]["Rating1"]);
                                int rat = Convert.ToInt32(rating1);

                                decimal datecheck = Convert.ToDecimal(dt.Rows[0]["Rating1"]);
                                decimal perchk = Convert.ToDecimal(dt.Rows[0]["Rating1"]) * 30 / 100;
                                perchk = perchk * 100;
                                int near = Convert.ToInt32(perchk);
                                //DateTime inspectduedate = Convert.ToDateTime(moorwinch.InstalledDate).AddDays(near);

                                int firstValue = 0; int secondValue = 0; DateTime nextMonth;
                                double value = Convert.ToDouble(datecheck);
                                string a = value.ToString();
                                string[] b = a.Split('.');
                                firstValue = int.Parse(b[0]);
                                if (b.Length == 2)
                                {
                                    secondValue = int.Parse(b[1]);
                                }
                                int chekint = Convert.ToInt32(datecheck);
                                DateTime date = Convert.ToDateTime(mropdtls.InstalledDate);
                                if (secondValue == 0)
                                {
                                    nextMonth = date.AddMonths(chekint);
                                }
                                else
                                {
                                    if (chekint != 0)
                                    {
                                        nextMonth = date.AddMonths(chekint).AddDays(-15);
                                    }
                                    else
                                    {
                                        nextMonth = date.AddDays(15);
                                    }
                                }


                                //int chekint = Convert.ToInt32(datecheck);
                                //DateTime date = Convert.ToDateTime(moorwinch.InstalledDate);
                                //DateTime nextMonth = date.AddMonths(chekint);
                                TimeSpan t = nextMonth - date;
                                double NrOfDays = t.TotalDays;
                                DateTime inspectduedate = Convert.ToDateTime(mropdtls.InstalledDate).AddDays(NrOfDays);

                                DateTime crntdt = DateTime.Now;
                                if (inspectduedate <= crntdt)
                                {
                                    inspectduedate = DateTime.Now;
                                }

                                mropdtls.InspectionDueDate = inspectduedate;
                            }
                        }
                        catch { }
                    }

                    try
                    {

                        try
                        {
                            if (mropdtls.IsRopeInstalled == "Yes")
                            {

                                SqlDataAdapter adpp = new SqlDataAdapter("select max(EndtoEndDoneDate) as EndtoEndDoneDate from RopeEndtoEnd2 where RopeId=" + mropdtls.Id + " and IsActive=1 and VesselId='" + VesselID + "'", con);
                                DataTable ddt = new DataTable();
                                adpp.Fill(ddt);
                                if (ddt.Rows.Count > 0)
                                {
                                    // var dd = ddt.Rows[0][0].ToString();

                                    instaldate = ddt.Rows[0][0].ToString() == "" ? mropdtls.InstalledDate : Convert.ToDateTime(ddt.Rows[0][0]);

                                    if (mropdtls.InstalledDate > instaldate)
                                    {
                                        TempData["Error"] = "Assign to Winch & End to End dates are already inserted for a future date, please remove these entries first to edit the installation date !";
                                        //MessageBox.Show("Assign to Winch & End to End dates are already inserted for a future date, please remove these entries first to edit the installation date", "Mooring Rope", MessageBoxButton.OK, MessageBoxImage.Information);
                                        //MainViewModelWorkHours.CommonValue = true;
                                        return View(mropdtls);
                                    }
                                    else
                                    {
                                        mropdtls.InstalledDate = mropdtls.InstalledDate;
                                        // moorwinch.InstalledDate = instaldate;
                                    }

                                }

                                SqlDataAdapter adpp1 = new SqlDataAdapter("select MAX(AssignedDate) as AssignedDate  from AssignRopeToWinch where RopeId=" + mropdtls.Id + " and IsActive=1 and RopeTail=1 and VesselId='" + VesselID + "'", con);
                                DataTable ddt1 = new DataTable();
                                adpp1.Fill(ddt1);
                                if (ddt1.Rows.Count > 0)
                                {
                                    instaldate1 = ddt1.Rows[0][0] == DBNull.Value ? mropdtls.InstalledDate : Convert.ToDateTime(ddt1.Rows[0][0]);

                                    if (mropdtls.InstalledDate > instaldate1)
                                    {
                                        TempData["Error"] = "Assign to Winch & End to End dates are already inserted for a future date, please remove these entries first to edit the installation date";
                                        return View(mropdtls);
                                        //MessageBox.Show("Assign to Winch & End to End dates are already inserted for a future date, please remove these entries first to edit the installation date", "Mooring Rope", MessageBoxButton.OK, MessageBoxImage.Information);
                                        //MainViewModelWorkHours.CommonValue = true;
                                        //return;
                                    }
                                    else
                                    {
                                        mropdtls.InstalledDate = mropdtls.InstalledDate;
                                        //moorwinch.InstalledDate = instaldate1;

                                    }

                                }
                            }
                        }
                        catch { }


                        SqlDataAdapter adpp2 = new SqlDataAdapter("select MAX(SplicingDoneDate) as SplicingDoneDate from RopeSplicingRecord where RopeId=" + mropdtls.Id + " and IsActive=1 and RopeTail=1 and VesselId='" + VesselID + "'", con);
                        DataTable ddt2 = new DataTable();
                        adpp2.Fill(ddt2);
                        if (ddt2.Rows.Count > 0)
                        {
                            // instaldate = Convert.ToDateTime(ddt2.Rows[0][0]);
                            recedate1 = ddt2.Rows[0][0].ToString() == "" ? mropdtls.ReceivedDate : Convert.ToDateTime(ddt2.Rows[0][0]);


                            if (mropdtls.ReceivedDate > recedate1)
                            {
                                TempData["Error"] = "SplicingDone Date are already inserted for a future date, please remove these entries first to edit the received date";
                                return View(mropdtls);
                                //MessageBox.Show("SplicingDone Date are already inserted for a future date, please remove these entries first to edit the received date", "Mooring Rope", MessageBoxButton.OK, MessageBoxImage.Information);
                                //MainViewModelWorkHours.CommonValue = true;
                                //return;
                            }
                            else
                            {
                                mropdtls.ReceivedDate = mropdtls.ReceivedDate;
                            }
                        }

                        SqlDataAdapter adpp3 = new SqlDataAdapter("select MAX(DamageDate) as DamageDate from ropedamagerecord where RopeId=" + mropdtls.Id + " and IsActive=1 and RopeTail=1 and VesselId='" + VesselID + "'", con);
                        DataTable ddt3 = new DataTable();
                        adpp3.Fill(ddt3);
                        if (ddt3.Rows.Count > 0)
                        {
                            //instaldate = Convert.ToDateTime(ddt3.Rows[0][0]);

                            recedate2 = ddt3.Rows[0][0].ToString() == "" ? mropdtls.ReceivedDate : Convert.ToDateTime(ddt3.Rows[0][0]);

                            if (mropdtls.ReceivedDate > recedate2)
                            {
                                TempData["Error"] = "Damage Date are already inserted for a future date, please remove these entries first to edit the received date";
                                return View(mropdtls);
                                //MessageBox.Show("Damage Date are already inserted for a future date, please remove these entries first to edit the received date", "Mooring Rope", MessageBoxButton.OK, MessageBoxImage.Information);
                                //MainViewModelWorkHours.CommonValue = true;
                                //return;
                            }
                            else
                            {
                                mropdtls.ReceivedDate = mropdtls.ReceivedDate;
                            }
                        }

                        SqlDataAdapter adpp4 = new SqlDataAdapter("select MAX(CroppedDate) as CroppedDate from RopeCropping where RopeId=" + mropdtls.Id + " and IsActive=1 and RopeTail=1 and VesselId='" + VesselID + "'", con);
                        DataTable ddt4 = new DataTable();
                        adpp4.Fill(ddt4);
                        if (ddt4.Rows.Count > 0)
                        {
                            //instaldate = Convert.ToDateTime(ddt4.Rows[0][0]);

                            recedate3 = ddt4.Rows[0][0].ToString() == "" ? mropdtls.ReceivedDate : Convert.ToDateTime(ddt4.Rows[0][0]);

                            if (mropdtls.ReceivedDate > recedate3)
                            {
                                TempData["Error"] = "Cropped Date are already inserted for a future date, please remove these entries first to edit the received date";
                                return View(mropdtls);
                                //MessageBox.Show("Cropped Date are already inserted for a future date, please remove these entries first to edit the received date", "Mooring Rope", MessageBoxButton.OK, MessageBoxImage.Information);
                                //MainViewModelWorkHours.CommonValue = true;
                                //return;
                            }
                            else
                            {
                                mropdtls.ReceivedDate = mropdtls.ReceivedDate;
                            }
                        }
                    }
                    catch (Exception ex) { }

                    #endregion

                    ///copy code from desktop    
                    ///

                    var obj = entities.MooringRopeDetails.FirstOrDefault(e => e.Id == mropdtls.Id && e.VesselID==vesselid);
                    if (obj != null)
                    {

                        obj.RopeTypeId = mropdtls.RopeTypeId;
                        obj.RopeConstruction = mropdtls.RopeConstruction;
                        obj.DiaMeter = mropdtls.DiaMeter;
                        obj.Length = mropdtls.Length;
                        obj.MBL = mropdtls.MBL;
                        obj.LDBF = mropdtls.LDBF;
                        obj.WLL = mropdtls.WLL;

                        obj.ManufacturerId = mropdtls.ManufacturerId;
                        obj.CertificateNumber = mropdtls.CertificateNumber;
                        obj.UniqueID = mropdtls.UniqueID;
                        obj.ReceivedDate = mropdtls.ReceivedDate;
                        if (mropdtls.IsRopeInstalled == "Yes")
                        {
                            if (mropdtls.InstalledDate == null)
                            {
                                TempData["Error"] = "Please fill install date !";
                                return View(mropdtls);
                            }
                            else
                            {
                                obj.InstalledDate = mropdtls.InstalledDate;

                                mropdtls.InspectionDueDate = CommonClass.updateinspecionduedate1(mropdtls.InstalledDate, mropdtls.RopeTypeId, mropdtls.ManufacturerId);
                            }
                        }
                        if (mropdtls.IsRopeInstalled == "No")
                        {
                            obj.InstalledDate = null;
                        }
                        obj.RopeTagging = mropdtls.RopeTagging;
                        obj.StartCounterHours = mropdtls.StartCounterHours;
                        obj.CurrentRunningHours = crntRhrs ;
                        obj.Remarks = mropdtls.Remarks;
                        obj.VesselID = Convert.ToInt32(VesselID);
                        obj.CreatedDate = DateTime.Now;
                        obj.RopeTail = 1;
                        obj.IsActive = true;
                        obj.DeleteStatus = false;
                        //obj.Id = ropeid;
                        //obj.RopeId = ropeid;
                        entities.Entry(obj).State = EntityState.Modified;

                    }

                    if (file != null)
                    {

                        Random randon = new Random();
                        int num = randon.Next(10000);
                        string FileName = Path.GetFileNameWithoutExtension(file.FileName);
                        //string FileExtension = Path.GetExtension(wbtRecord.ImageFile.FileName);        
                        //FileName = DateTime.Now.ToString("yyyyMMdd") + "-" + FileName.Trim() + FileExtension;   

                        string fileExtention = Path.GetExtension(file.FileName);
                        string fileName = Path.GetFileName(file.FileName);
                        var withoutextnsn = Path.GetFileNameWithoutExtension(fileName) + num;

                        string[] str1;
                        str1 = new string[26] { "jpg", "jpeg", "png", "pps", "ppt", "pptx", "xls", "xlsm", "xlsx", "doc", "docx", "pdf", "rtf", "txt", "gif", "wav", "mid", "midi", "wma", "mp3", "mp4", "ogg", "rma", "avi", "divx", "wmv" };
                        string FileExtension = file.FileName.Substring(file.FileName.LastIndexOf('.') + 1).ToLower();

                        if (str1.Contains(FileExtension))
                        {

                            // string size = FormatSize(trngattach.ImageFile.ContentLength);
                            //size = size.Remove(size.Length - 2);
                            //int sizecheck = trngattach.ImageFile.ContentLength;

                            int compare = file.ContentLength;

                            if (compare > 5000000)
                            {
                                TempData["Error"] = "Attachment size not to be greater than 5MB";
                                return RedirectToAction("Index");
                            }

                            fileName = withoutextnsn + fileExtention;
                            string UploadPath = "~/images/AttachFiles/";

                            string pth = fileName;
                            //trngattach.AttachmentPath = UploadPath + fileName;
                            string origPath = Server.MapPath("~/images/AttachFiles/");
                            var originalFilePath = Path.Combine(origPath, fileName);

                            file.SaveAs(originalFilePath);

                            var tuple = CommonClass.Getmaxid(0, 0, 0, VesselID);

                            int idPk = tuple.Item1;
                            // int rpid = tuple.Item2;

                            CommonClass.UpdateRopeAttachment(idPk, pth, obj.RopeId, 0, "RopeTail",0, VesselID);



                        }
                    }


                }
                try
                {
                    entities.SaveChanges();
                    TempData["Success"] = "Record successfully saved !";
                }
                catch (Exception ex)
                {
                    throw ex;
                }

                return RedirectToAction("Index");

            }

        }

        public ActionResult Edit(int id)
        {
            int VesselID = Convert.ToInt32(Session["VesselSessionID"].ToString());
            int vesselid = Convert.ToInt32(VesselID);
            using (MorringOfficeEntities entities = new MorringOfficeEntities())
            {
                var mropedetail = entities.MooringRopeDetails.Where(x => x.VesselID == vesselid).FirstOrDefault(e => e.Id == id);


                //foreach (var v in context.MooringRopeTypes.OrderBy(p => p.RopeType).ToList())
                //    MooringRopeTypeLists.Add(new SelectListItem() { Text = v.RopeType, Value = v.Id.ToString() });

                foreach (var v in context.MooringRopeTypes.OrderBy(p => p.RopeType).ToList())
                    mropedetail.MooringRopeTypeLists.Add(new SelectListItem() { Text = v.RopeType, Value = v.Id.ToString() });

                foreach (var v in context.tblCommons.Where(u => u.Type == 1).OrderBy(p => p.Name).ToList())
                    mropedetail.ManufacturerTypeLists.Add(new SelectListItem() { Text = v.Name, Value = v.Id.ToString() });




                return View("addmooringinfo", mropedetail);
            }
        }

        //private DateTime? updateinspecionduedate(DateTime? installdate, int? ropetypeid, int? manufacid)
        //{
        //    SqlConnection con = ConnectionBulder.con;
        //    try
        //    {
        //        SqlDataAdapter adp = new SqlDataAdapter("select * from tblRopeInspectionSetting where MooringRopeType=" + ropetypeid + " and ManufacturerType=" + manufacid + "", con);
        //        DataTable dt = new DataTable();
        //        adp.Fill(dt);
        //        if (dt.Rows.Count > 0)
        //        {
        //            //decimal rating1 = Convert.ToDecimal(dt.Rows[0]["Rating1"]);
        //            //int rat = Convert.ToInt32(rating1);

        //            decimal datecheck = Convert.ToDecimal(dt.Rows[0]["Rating1"]);
        //            decimal perchk = Convert.ToDecimal(dt.Rows[0]["Rating1"]) * 30 / 100;
        //            perchk = perchk * 100;
        //            int near = Convert.ToInt32(perchk);
        //            //DateTime inspectduedate = Convert.ToDateTime(moorwinchrope.InstalledDate).AddDays(near);


        //            int firstValue = 0; int secondValue = 0; DateTime nextMonth;
        //            double value = Convert.ToDouble(datecheck);
        //            string a = value.ToString();
        //            string[] b = a.Split('.');
        //            firstValue = int.Parse(b[0]);
        //            if (b.Length == 2)
        //            {
        //                secondValue = int.Parse(b[1]);
        //            }
        //            int chekint = Convert.ToInt32(datecheck);
        //            DateTime date = Convert.ToDateTime(installdate);
        //            if (secondValue == 0)
        //            {
        //                nextMonth = date.AddMonths(chekint);
        //            }
        //            else
        //            {
        //                if (chekint != 0)
        //                {
        //                    nextMonth = date.AddMonths(chekint).AddDays(-15);
        //                }
        //                else
        //                {
        //                    nextMonth = date.AddDays(15);
        //                }
        //            }
        //            TimeSpan t = nextMonth - date;
        //            double NrOfDays = t.TotalDays;

        //            DateTime inspectduedate = Convert.ToDateTime(installdate).AddDays(NrOfDays);



        //            DateTime crntdt = DateTime.Now;
        //            if (inspectduedate <= crntdt)
        //            {
        //                inspectduedate = DateTime.Now;
        //            }

        //            installdate = inspectduedate;
        //        }
        //        else
        //        {
        //            installdate = null;
        //        }

        //    }
        //    catch { }
        //    return installdate;
        //}
        static string msgcheck = "";

        //[PreventSpam("Deletemooringinfotail", 3, 1)]
        public ActionResult delete(int id)
        {
            try
            {
                int VesselID = Convert.ToInt32(Session["VesselSessionID"].ToString());
                int vesselid = Convert.ToInt32(VesselID);
                int ropeid = context.MooringRopeDetails.Where(x => x.Id == id && x.VesselID == vesselid).Select(x => x.RopeId).SingleOrDefault();


                SqlDataAdapter adp2 = new SqlDataAdapter("select distinct OperationId from MOUsedWinchTbl where RopeId=" + ropeid + " and VesselId='" + VesselID + "' ", con);
                DataTable dt2 = new DataTable();
                adp2.Fill(dt2);
                for (int i = 0; i < dt2.Rows.Count; i++)
                {
                    int operationid = Convert.ToInt32(dt2.Rows[i][0]);
                    SqlDataAdapter adp22 = new SqlDataAdapter("select * from MOperationBirthDetail where opid=" + operationid + " and IsActive=1 and VesselId='" + VesselID + "' ", con);
                    DataTable dt22 = new DataTable();
                    adp22.Fill(dt22);

                    if (dt22.Rows.Count > 0)
                    {
                        TempData["Error"] = "This line cannot be deleted, as already marked as operation, please delete from operation records first ";

                        return RedirectToAction("Index");

                    }
                }

                SqlDataAdapter adp228 = new SqlDataAdapter("DeleteCheckInMooringRopeDetail", con);
                adp228.SelectCommand.CommandType = CommandType.StoredProcedure;
                adp228.SelectCommand.Parameters.AddWithValue("@RopeId", ropeid);
                adp228.SelectCommand.Parameters.AddWithValue("@Ropetail", "RopeTail");
                adp228.SelectCommand.Parameters.AddWithValue("@VesselID", VesselID);
                DataTable dt228 = new DataTable();
                adp228.Fill(dt228);

                if (dt228.Rows.Count > 0)
                {
                    msgcheck = dt228.Rows[0][0].ToString();
                }

                if (msgcheck != "")
                {
                    TempData["Error"] = msgcheck;
                    msgcheck = "";
                    return RedirectToAction("Index");


                }
                else
                {

                    MooringRopeDetail findrank = context.MooringRopeDetails.Where(x => x.Id == id && x.DeleteStatus == false && x.VesselID==vesselid).FirstOrDefault();
                    if (findrank != null)
                    {                       

                        var result = context.MooringRopeDetails.SingleOrDefault(b => b.Id == id && b.DeleteStatus == false && b.VesselID == vesselid);
                        if (result != null)
                        {

                            result.DeleteStatus = true;
                            result.ModifiedDate = DateTime.Now;
                            context.SaveChanges();


                        }

                        try
                        {
                            SqlDataAdapter adp1 = new SqlDataAdapter("update notifications set IsActive=0 where ropeid=" + ropeid + "", con);
                            DataTable dt1 = new DataTable();
                            adp1.Fill(dt1);
                        }
                        catch { }
                        TempData["Success"] = "Record deleted successfully ";

                    }
                    else
                    {
                        TempData["Error"] = "Record not found ";
                    }

                }


            }
            catch (Exception ex) { }
            return RedirectToAction("Index");
        }
        public JsonResult DetailCheck(int RopeId)
        {
            int VesselID = Convert.ToInt32(Session["VesselSessionID"].ToString());
            int vesselid = Convert.ToInt32(VesselID);
            var lineD = context.MooringRopeDetails.Where(x => x.RopeId == RopeId && x.VesselID == vesselid).FirstOrDefault();

            var ropetype = context.MooringRopeTypes.Where(x => x.Id == lineD.RopeTypeId).Select(x => x.RopeType).SingleOrDefault();
            string rptype = ropetype;

            var manname = context.tblCommons.Where(x => x.Id == lineD.ManufacturerId).Select(x => x.Name).SingleOrDefault();
            string Mname = manname;

            string recdt = lineD.ReceivedDate.ToString();
            DateTime Var = new DateTime();
            Var = Convert.ToDateTime(recdt);
            string ss = Var.ToString("yyyy-MM-dd");

            string ss1 = "";
            string insdt = lineD.InstalledDate.ToString();


            if (insdt != "")
            {
                DateTime Var1 = new DateTime();
                Var1 = Convert.ToDateTime(insdt);
                ss1 = Var1.ToString("yyyy-MM-dd");
            }

            // return Json(lineD, rptype = ropetype, JsonRequestBehavior.AllowGet);

            return Json(new
            {
                RopeConstruction = lineD.RopeConstruction,
                DiaMeter = lineD.DiaMeter,
                Length = lineD.Length,
                MBL = lineD.MBL,
                LDBF = lineD.LDBF,
                WLL = lineD.WLL,
                ManufacturerId = Mname,
                CertificateNumber = lineD.CertificateNumber,
                UniqueID = lineD.UniqueID,
                ReceivedDate = ss,
                InstalledDate = ss1,
                RopeTagging = lineD.RopeTagging,
                StartCounterHours = lineD.StartCounterHours,
                Remarks = lineD.Remarks,
                rptype = ropetype
            }, JsonRequestBehavior.AllowGet);
        }
    }
}