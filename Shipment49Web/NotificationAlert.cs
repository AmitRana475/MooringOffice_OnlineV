using MenuLayer;
using Reports;
using Shipment49Web.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Shipment49Web
{

    public enum NotificationAlertType    {
        //Minimum21RopeTales = 0,
        Inspection7Day = 1,        Inspection1Day = 2,        InspectionOver = 3,        Over_Cropping = 4,        Max_running_hours_Approaching = 5,        Max_running_hours_Exceeded = 6,        Max_allowable_time_Approaching = 7,        Max_allowable_time_Exceeded = 8,        Rating_5Rope = 9,        Rating_6Rope = 95,        Rating_7Rope = 10,        End_to_End_Approaching = 11,        End_to_End_Overdue = 12,
        //Rotation_Approaching = 13,
        //Rotation_Overdue = 14,
        RopeDamage = 15,        RopeSplicing = 16,        Minimum21RopeCount = 17,        Minimum21TailCount = 18,        JoiningShackle_LooseEquipment7Day = 21,        JoiningShackle_LooseEquipment1Day = 22,        JoiningShackle_LooseEquipmentOver = 23,        RopeStopper_FireWire_Messanger_LooseEq7Day = 24,        RopeStopper_FireWire_Messanger_LooseEq1Day = 25,        RopeStopper_FireWire_Messanger_LooseEqOver = 26,        ChainStopper_LooseEq7Day = 27,        ChainStopper_LooseEq1Day = 28,        ChainStopper_LooseEqOver = 29,        ChafeGuard_LooseEq7Day = 30,        ChafeGuard_LooseEq1Day = 31,        ChafeGuard_LooseEqOver = 32,        WinchBreakTest_LooseEq7Day = 33,        WinchBreakTest_LooseEq1Day = 34,        WinchBreakTest_LooseEqOver = 35,        OutofService_discarded_Rope = 41,        OutofService_discarded_Tail = 42,        RopeResidual_StrengthCheck = 43,        Winch_Rotation_approching = 44,        Winch_Rotation_exceed = 45,


        //============== All LooseEq Approching Begin =============
        All_LooseEquipmentDamaged = 50,        All_LooseEquipmentExceeded = 100,        JoiningShackle_LooseEquipmentDisCard = 36,        RopeStopper_FireWire_Messanger_LooseEquipmentDisCard = 37,        ChainStopper_LooseEquipmentDisCard = 38,        ChafeGuard_LooseEquipmentDisCard = 39,        WinchBreakTest_LooseEquipmentDisCard = 40,        TowingRope_LooseEquipmentDisCard = 51,        SuezRope_LooseEquipmentDisCard = 52,        PennantRope_LooseEquipmentDisCard = 53,        GrommetRope_LooseEquipmentDisCard = 54,

        //============== All LooseEq Exceeded Begin {Approching ID + Exceeded ID} =============
        //JoiningShackle_ChainStopper_RopeStopper_ChafeGuard_WinchBreakTest_FireWire_Messanger



    }
    public class NotificationAlert : Alerts
    {
        SqlConnection con = ConnectionBulder.con;
        // MorringOfficeEntities context = new MorringOfficeEntities();
        // VesselID = Convert.ToInt32(CommonClass.VesselSessionID);
        private async Task InspectionDue7daysOROverdue(int VesselIDs)
        {
            try
            {
                using (SqlDataAdapter adp = new SqlDataAdapter("Select * from MooringRopeDetail where VesselID=" + VesselIDs + " and IsActive=1 and DeleteStatus = 0  and OutofServiceDate is null and CAST( InspectionDueDate AS date) < DATEADD(day,1+7,CAST(GETDATE() AS date))", con))
                {
                    DataTable dt = new DataTable();
                    adp.Fill(dt);
                    if (dt.Rows.Count > 0)
                    {
                        // RopeTail = 0 then Rope, else RopeTail

                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            var RT = Convert.ToInt32(dt.Rows[i]["RopeTail"]);
                            string Tail = RT == 0 ? "Line" : "RopeTail";
                            string UniqueID = dt.Rows[i]["UniqueID"].ToString();

                            int RopeId = Convert.ToInt32(dt.Rows[i]["RopeId"]);
                            string assignednumber = ""; string location = "Not Assigned";


                            using (SqlDataAdapter pp = new SqlDataAdapter("select b.AssignedNumber,b.Location from AssignRopeToWinch a join MooringWinchDetail b on a.WinchId = b.Id and a.VesselID=b.VesselID where a.IsActive=1 and a.VesselID=" + VesselIDs + " and a.RopeId = " + RopeId + "", con))
                            {
                                DataTable dtt = new DataTable();
                                pp.Fill(dtt);
                                if (dtt.Rows.Count > 0)
                                {

                                    assignednumber = dtt.Rows[0]["AssignedNumber"].ToString();
                                    location = dtt.Rows[0]["Location"].ToString();

                                }
                            }




                            DateTime DueDateInsp = Convert.ToDateTime(dt.Rows[i]["InspectionDueDate"]).Date;

                            if (assignednumber != null && assignednumber != "")
                            {


                                var notification7 = "Inspection Due in 7 days- For " + Tail + " #" + dt.Rows[i]["CertificateNumber"] + " located at " + assignednumber + " Winch (" + location + ")";
                                // var notification7 = "Inspection Due in 7 days- For Rope #" + dt.Rows[i]["CertificateNumber"] + " on Winch " + assignednumber + " located at " + location;
                                int NotiAlertType = (int)NotificationAlertType.Inspection7Day;
                                await InspectNotification(VesselIDs, RopeId, notification7, NotiAlertType, DueDateInsp);


                                var notification1 = "Inspection Due in 1 day- For " + Tail + " #" + dt.Rows[i]["CertificateNumber"] + " located at " + assignednumber + " Winch (" + location + ")";
                                int NotiAlertType1 = (int)NotificationAlertType.Inspection1Day;
                                await InspectNotification(VesselIDs, RopeId, notification1, NotiAlertType1, DueDateInsp);


                                var notificationOv = "Inspection Overdue- For " + Tail + " #" + dt.Rows[i]["CertificateNumber"] + " located at " + assignednumber + " Winch (" + location + ")";
                                int NotiAlertType3 = (int)NotificationAlertType.InspectionOver;
                                await InspectNotification(VesselIDs, RopeId, notificationOv, NotiAlertType3, DueDateInsp);

                            }
                            else
                            {
                                var notification7 = "Inspection Due in 7 days- For " + Tail + " #" + dt.Rows[i]["CertificateNumber"] + "";
                                // var notification7 = "Inspection Due in 7 days- For Rope #" + dt.Rows[i]["CertificateNumber"] + " on Winch " + assignednumber + " located at " + location;
                                int NotiAlertType = (int)NotificationAlertType.Inspection7Day;
                                await InspectNotification(VesselIDs, RopeId, notification7, NotiAlertType, DueDateInsp);


                                var notification1 = "Inspection Due in 1 day- For " + Tail + " #" + dt.Rows[i]["CertificateNumber"] + "";
                                int NotiAlertType1 = (int)NotificationAlertType.Inspection1Day;
                                await InspectNotification(VesselIDs, RopeId, notification1, NotiAlertType1, DueDateInsp);


                                var notificationOv = "Inspection Overdue- For " + Tail + " #" + dt.Rows[i]["CertificateNumber"] + "";
                                int NotiAlertType3 = (int)NotificationAlertType.InspectionOver;
                                await InspectNotification(VesselIDs, RopeId, notificationOv, NotiAlertType3, DueDateInsp);
                            }
                        }
                    }
                }
            }
            catch (Exception ex) { }
            //replace Task.CompletedTask with Task.FromResult(0).
            //await Task.FromResult(0);
        }

         public void GetAllNotification(int IMONum)
        {
             InspectionDue7daysOROverdue(IMONum);
             LooseEquipmentNoti(IMONum);
             NorificationRopeDiscard(IMONum);
             WinchrotationSetting_and_Notifications(IMONum);
        }

     

        public async Task MasterCall_Notification()
        {
           
            
            using (SqlDataAdapter adp = new SqlDataAdapter("select VesselName,ImoNo,MinimumRopes,MinimumRopeTails from VesselDetail", con))
            {
                DataTable dt = new DataTable();
                adp.Fill(dt);
                if(dt.Rows.Count>0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        int ImoNo = Convert.ToInt32(dt.Rows[i]["ImoNo"]);

                        // ******* Call of Functions*********
                        await InspectionDue7daysOROverdue(ImoNo);
                        await LooseEquipmentNoti(ImoNo);
                        await NorificationRopeDiscard(ImoNo);
                        await WinchrotationSetting_and_Notifications(ImoNo);
                        // ******* Call of Functions ***********
                    }

                }
            }

            
        }

        private async Task LooseEquipmentNoti(int VesselID)
        {
            //// JoiningShackle Notification
            try
            {

                using (SqlDataAdapter adp = new SqlDataAdapter("Select * from JoiningShackle where VesselID=" + VesselID + " and DeleteStatus = 0  and OutofServiceDate is null and CAST( InspectionDueDate AS date) < DATEADD(day,1+7,CAST(GETDATE() AS date))", con))
                {
                    DataTable dt = new DataTable();
                    adp.Fill(dt);
                    if (dt.Rows.Count > 0)
                    {
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {

                            string JoiningShackleID = dt.Rows[i]["Id"].ToString();
                            int LoosType = 1;
                            string InstallDate = dt.Rows[i]["DateInstalled"].ToString();
                            string InspectionDueDate = dt.Rows[i]["InspectionDueDate"].ToString();
                            string CertificatNum = dt.Rows[i]["CertificateNumber"].ToString();
                            if (!string.IsNullOrEmpty(InspectionDueDate))
                            {
                                DateTime insduedt = Convert.ToDateTime(InspectionDueDate);



                                int AlertTp7 = (int)NotificationAlertType.JoiningShackle_LooseEquipment7Day;
                                var notification7 = "Inspection Due in 7 days- For JoiningShackle Loose Equipment #" + CertificatNum + "";
                                await InspectionDue7DayOrOerLooseEquipment(VesselID, notification7, insduedt, JoiningShackleID, LoosType, AlertTp7);

                                int AlertTp1 = (int)NotificationAlertType.JoiningShackle_LooseEquipment1Day;
                                var notification1 = "Inspection Due in 1 day- For JoiningShackle Loose Equipment #" + CertificatNum + "";
                                await InspectionDue7DayOrOerLooseEquipment(VesselID, notification1, insduedt, JoiningShackleID, LoosType, AlertTp1);

                                int AlertTpOv = (int)NotificationAlertType.JoiningShackle_LooseEquipmentOver;
                                var notificationOv = "Inspection Overdue- For JoiningShackle Loose Equipment #" + CertificatNum + "";
                                await InspectionDue7DayOrOerLooseEquipment(VesselID, notificationOv, insduedt, JoiningShackleID, LoosType, AlertTpOv);

                                //}
                            }


                        }

                    }
                }

                using (SqlDataAdapter adp5 = new SqlDataAdapter("Select * from JoiningShackle where VesselID=" + VesselID + " and DeleteStatus = 0  and OutofServiceDate is null ", con))
                {
                    DataTable dt5 = new DataTable();
                    adp5.Fill(dt5);
                    if (dt5.Rows.Count > 0)
                    {
                        for (int i = 0; i < dt5.Rows.Count; i++)
                        {

                            string JoiningShackleID = dt5.Rows[i]["Id"].ToString();
                            int LoosType = 1;
                            string InstallDate = dt5.Rows[i]["DateInstalled"].ToString();

                            string CertificatNum = dt5.Rows[i]["CertificateNumber"].ToString();

                            await LooseEquipmentDisCard(VesselID, InstallDate, LoosType, Convert.ToInt32(JoiningShackleID), CertificatNum);

                        }
                    }
                }


            }
            catch (Exception ex) { }


            //////////// RopeStopper,FireWire,Messanger Rope Notification

            try
            {

                using (SqlDataAdapter adp = new SqlDataAdapter("Select * from RopeTail where VesselID=" + VesselID + " and DeleteStatus = 0  and OutofServiceDate is null and CAST( InspectionDueDate AS date) < DATEADD(day,1+7,CAST(GETDATE() AS date))", con))
                {
                    DataTable dt = new DataTable();
                    adp.Fill(dt);
                    if (dt.Rows.Count > 0)
                    {
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {

                            string InspectionDueDate = dt.Rows[i]["InspectionDueDate"].ToString();
                            string RopeStopper_FireWire_Messanger = dt.Rows[i]["Id"].ToString();
                            int LoosType = Convert.ToInt32(dt.Rows[i]["LooseETypeId"]);
                            string InstallDate = dt.Rows[i]["InstalledDate"].ToString();
                            string LoosEqpName = "";

                            if (LoosType == 3)
                            {
                                LoosEqpName = "Messenger Rope";

                            }
                            else if (LoosType == 4)
                            {
                                LoosEqpName = "Rope Stopper";

                            }
                            else if (LoosType == 6)
                            {
                                LoosEqpName = "FireWire";

                            }
                            else if (LoosType == 9)
                            {
                                LoosEqpName = "Towing Rope";

                            }
                            else if (LoosType == 10)
                            {
                                LoosEqpName = "Suez Rope";

                            }
                            else if (LoosType == 11)
                            {
                                LoosEqpName = "Pennant Rope";

                            }
                            else if (LoosType == 12)
                            {
                                LoosEqpName = "Grommet Rope";

                            }

                            string CertificatNum = dt.Rows[i]["CertificateNumber"].ToString();
                            if (!string.IsNullOrEmpty(InspectionDueDate))
                            {
                                DateTime insduedt = Convert.ToDateTime(InspectionDueDate);


                                int AlertTp7 = (int)NotificationAlertType.RopeStopper_FireWire_Messanger_LooseEq7Day;
                                var notification7 = "Inspection Due in 7 days- For " + LoosEqpName + " Loose Equipment #" + CertificatNum + "";
                                await InspectionDue7DayOrOerLooseEquipment(VesselID, notification7, insduedt, RopeStopper_FireWire_Messanger, LoosType, AlertTp7);

                                int AlertTp1 = (int)NotificationAlertType.RopeStopper_FireWire_Messanger_LooseEq1Day;
                                var notification1 = "Inspection Due in 1 day- For " + LoosEqpName + " Loose Equipment #" + CertificatNum + "";
                                await InspectionDue7DayOrOerLooseEquipment(VesselID, notification1, insduedt, RopeStopper_FireWire_Messanger, LoosType, AlertTp1);

                                int AlertTpOv = (int)NotificationAlertType.RopeStopper_FireWire_Messanger_LooseEqOver;
                                var notificationOv = "Inspection Overdue- For " + LoosEqpName + " Loose Equipment #" + CertificatNum + "";
                                await InspectionDue7DayOrOerLooseEquipment(VesselID, notificationOv, insduedt, RopeStopper_FireWire_Messanger, LoosType, AlertTpOv);


                                //}
                            }


                        }

                    }

                }

                using (SqlDataAdapter adp5 = new SqlDataAdapter("Select * from RopeTail where VesselID=" + VesselID + " and DeleteStatus = 0  and OutofServiceDate is null ", con))
                {
                    DataTable dt5 = new DataTable();
                    adp5.Fill(dt5);
                    if (dt5.Rows.Count > 0)
                    {
                        for (int i = 0; i < dt5.Rows.Count; i++)
                        {

                            string RopeStopper_FireWire_Messanger = dt5.Rows[i]["Id"].ToString();
                            int LoosType = Convert.ToInt32(dt5.Rows[i]["LooseETypeId"]);
                            string InstallDate = dt5.Rows[i]["InstalledDate"].ToString();

                            string CertificatNum = dt5.Rows[i]["CertificateNumber"].ToString();

                            await LooseEquipmentDisCard(VesselID, InstallDate, LoosType, Convert.ToInt32(RopeStopper_FireWire_Messanger), CertificatNum);

                        }
                    }
                }



            }
            catch (Exception exc)
            {
            }

            //////////// ChainStopper Notification


            try
            {

                SqlDataAdapter adp = new SqlDataAdapter("Select * from ChainStopper where VesselID=" + VesselID + " and  DeleteStatus = 0  and OutofServiceDate is null and CAST( InspectionDueDate AS date) < DATEADD(day,1+7,CAST(GETDATE() AS date))", con);
                DataTable dt = new DataTable();
                adp.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {

                        string ChainStopperID = dt.Rows[i]["Id"].ToString();

                        int LoosType = Convert.ToInt32(dt.Rows[i]["LooseETypeId"]);
                        string InstallDate = dt.Rows[i]["DateInstalled"].ToString();

                        string InspectionDueDate = dt.Rows[i]["InspectionDueDate"].ToString();

                        string CertificatNum = dt.Rows[i]["CertificateNumber"].ToString();

                        if (!string.IsNullOrEmpty(InspectionDueDate))
                        {
                            DateTime insduedt = Convert.ToDateTime(InspectionDueDate);


                            int AlertTp7 = (int)NotificationAlertType.ChainStopper_LooseEq7Day;
                            var notification7 = "Inspection Due in 7 days- For ChainStopper Loose Equipment #" + CertificatNum + "";
                            await InspectionDue7DayOrOerLooseEquipment(VesselID, notification7, insduedt, ChainStopperID, LoosType, AlertTp7);

                            int AlertTp1 = (int)NotificationAlertType.ChainStopper_LooseEq1Day;
                            var notification1 = "Inspection Due in 1 day- For ChainStopper Loose Equipment #" + CertificatNum + "";
                            await InspectionDue7DayOrOerLooseEquipment(VesselID, notification1, insduedt, ChainStopperID, LoosType, AlertTp1);

                            int AlertTpOv = (int)NotificationAlertType.ChainStopper_LooseEqOver;
                            var notificationOv = "Inspection Overdue- For ChainStopper Loose Equipment #" + CertificatNum + "";
                            await InspectionDue7DayOrOerLooseEquipment(VesselID, notificationOv, insduedt, ChainStopperID, LoosType, AlertTpOv);

                            //}
                        }



                    }

                }

                adp.Dispose();
                dt.Dispose();

                using (SqlDataAdapter adp5 = new SqlDataAdapter("Select * from ChainStopper where VesselID=" + VesselID + " and  DeleteStatus = 0  and OutofServiceDate is null ", con))
                {
                    DataTable dt5 = new DataTable();
                    adp5.Fill(dt5);
                    if (dt5.Rows.Count > 0)
                    {
                        for (int i = 0; i < dt5.Rows.Count; i++)
                        {

                            string ChainStopperID = dt5.Rows[i]["Id"].ToString();
                            int LoosType = Convert.ToInt32(dt5.Rows[i]["LooseETypeId"]);
                            string InstallDate = dt5.Rows[i]["DateInstalled"].ToString();

                            string CertificatNum = dt5.Rows[i]["CertificateNumber"].ToString();


                            await LooseEquipmentDisCard(VesselID, InstallDate, LoosType, Convert.ToInt32(ChainStopperID), CertificatNum);

                        }
                    }
                }

            }
            catch (Exception ex) { }


            //////////// ChafeGuard Notification


            try
            {

                using (SqlDataAdapter adp = new SqlDataAdapter("Select * from ChafeGuard where  VesselID=" + VesselID + " and DeleteStatus = 0  and OutofServiceDate is null and CAST( InspectionDueDate AS date) < DATEADD(day,1+7,CAST(GETDATE() AS date))", con))
                {
                    DataTable dt = new DataTable();
                    adp.Fill(dt);
                    if (dt.Rows.Count > 0)
                    {
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {

                            string ChafeGuardID = dt.Rows[i]["Id"].ToString();
                            string InspectionDueDate = dt.Rows[i]["InspectionDueDate"].ToString();

                            int LoosType = 7;
                            string InstallDate = dt.Rows[i]["InstalledDate"].ToString();
                            string CertificatNum = dt.Rows[i]["CertificateNumber"].ToString();

                            if (!string.IsNullOrEmpty(InspectionDueDate))
                            {
                                DateTime insduedt = Convert.ToDateTime(InspectionDueDate);


                                int AlertTp7 = (int)NotificationAlertType.ChafeGuard_LooseEq7Day;
                                var notification7 = "Inspection Due in 7 days- For ChafeGuard Loose Equipment #" + CertificatNum + "";
                                await InspectionDue7DayOrOerLooseEquipment(VesselID, notification7, insduedt, ChafeGuardID, LoosType, AlertTp7);

                                int AlertTp1 = (int)NotificationAlertType.ChafeGuard_LooseEq1Day;
                                var notification1 = "Inspection Due in 1 day- For ChafeGuard Loose Equipment #" + CertificatNum + "";
                                await InspectionDue7DayOrOerLooseEquipment(VesselID, notification1, insduedt, ChafeGuardID, LoosType, AlertTp1);

                                int AlertTpOv = (int)NotificationAlertType.ChafeGuard_LooseEqOver;
                                var notificationOv = "Inspection Overdue- For ChafeGuard Loose Equipment #" + CertificatNum + "";
                                await InspectionDue7DayOrOerLooseEquipment(VesselID, notificationOv, insduedt, ChafeGuardID, LoosType, AlertTpOv);

                                //}
                            }


                        }

                    }

                }

                using (SqlDataAdapter adp5 = new SqlDataAdapter("Select * from ChafeGuard where VesselID=" + VesselID + " and DeleteStatus = 0  and OutofServiceDate is null ", con))
                {
                    DataTable dt5 = new DataTable();
                    adp5.Fill(dt5);
                    if (dt5.Rows.Count > 0)
                    {
                        for (int i = 0; i < dt5.Rows.Count; i++)
                        {
                            string ChafeGuardID = dt5.Rows[i]["Id"].ToString();
                            int LoosType = 7;
                            string InstallDate = dt5.Rows[i]["InstalledDate"].ToString();

                            string CertificatNum = dt5.Rows[i]["CertificateNumber"].ToString();

                            await LooseEquipmentDisCard(VesselID, InstallDate, LoosType, Convert.ToInt32(ChafeGuardID), CertificatNum);

                        }
                    }
                }
            }
            catch (Exception ex) { }


            //////////// WinchBreakTestKit Notification


            try
            {

                SqlDataAdapter adp = new SqlDataAdapter("Select * from WinchBreakTestKit where VesselID=" + VesselID + " and  DeleteStatus = 0  and OutofServiceDate is null and CAST( InspectionDueDate AS date) < DATEADD(day,1+7,CAST(GETDATE() AS date))", con);
                DataTable dt = new DataTable();
                adp.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {

                        string WinchBreakTestKitID = dt.Rows[i]["Id"].ToString();
                        string InspectionDueDate = dt.Rows[i]["InspectionDueDate"].ToString();

                        int LoosType = 8;
                        string InstallDate = dt.Rows[i]["InstalledDate"].ToString();


                        string CertificatNum = dt.Rows[i]["CertificateNumber"].ToString();

                        if (!string.IsNullOrEmpty(InspectionDueDate))
                        {
                            DateTime insduedt = Convert.ToDateTime(InspectionDueDate);


                            int AlertTp7 = (int)NotificationAlertType.WinchBreakTest_LooseEq7Day;
                            var notification7 = "Inspection Due in 7 days- For Winch Break-Test-Kit Loose Equipment #" + CertificatNum + "";
                            await InspectionDue7DayOrOerLooseEquipment(VesselID, notification7, insduedt, WinchBreakTestKitID, LoosType, AlertTp7);

                            int AlertTp1 = (int)NotificationAlertType.WinchBreakTest_LooseEq1Day;
                            var notification1 = "Inspection Due in 1 day- For Winch Break-Test-Kit Loose Equipment #" + CertificatNum + "";
                            await InspectionDue7DayOrOerLooseEquipment(VesselID, notification1, insduedt, WinchBreakTestKitID, LoosType, AlertTp1);

                            int AlertTpOv = (int)NotificationAlertType.WinchBreakTest_LooseEqOver;
                            var notificationOv = "Inspection Overdue- For Winch Break-Test-Kit Loose Equipment #" + CertificatNum + "";
                            await InspectionDue7DayOrOerLooseEquipment(VesselID, notificationOv, insduedt, WinchBreakTestKitID, LoosType, AlertTpOv);

                            //}
                        }


                    }

                }

                adp.Dispose();
                dt.Dispose();

                using (SqlDataAdapter adp5 = new SqlDataAdapter("Select * from WinchBreakTestKit where VesselID=" + VesselID + " and DeleteStatus = 0  and OutofServiceDate is null ", con))
                {
                    DataTable dt5 = new DataTable();
                    adp5.Fill(dt5);
                    if (dt5.Rows.Count > 0)
                    {
                        for (int i = 0; i < dt5.Rows.Count; i++)
                        {

                            string WinchBreakTestKitID = dt5.Rows[i]["Id"].ToString();
                            int LoosType = 8;
                            string InstallDate = dt5.Rows[i]["InstalledDate"].ToString();

                            string CertificatNum = dt5.Rows[i]["CertificateNumber"].ToString();

                            await LooseEquipmentDisCard(VesselID, InstallDate, LoosType, Convert.ToInt32(WinchBreakTestKitID), CertificatNum);
                            // LooseEquipmentDisCard(InstallDate, LoosType, Convert.ToInt32(ChafeGuardID), CertificatNum);

                        }
                    }
                }
            }
            catch (Exception ex) { }
        }

        private async Task LooseEquipmentDisCard(int VesselID, string CheckIntallDT, int LooseEqType, int LID, string CertificatNum)
        {
            try
            {


                string LoosEqpName = ""; int NotiAlertTypeApproching = 0; int NotiAlertTypeApproching_Exceeded = 0;
                if (LooseEqType == 1)
                {
                    LoosEqpName = "Joining Shackle";
                    NotiAlertTypeApproching = (int)NotificationAlertType.JoiningShackle_LooseEquipmentDisCard;
                }
                else if (LooseEqType == 2)
                {
                    LoosEqpName = "Rope Tail";
                }
                else if (LooseEqType == 3)
                {
                    LoosEqpName = "Messenger Rope";
                    NotiAlertTypeApproching = (int)NotificationAlertType.RopeStopper_FireWire_Messanger_LooseEquipmentDisCard;
                }
                else if (LooseEqType == 4)
                {
                    LoosEqpName = "Rope Stopper";
                    NotiAlertTypeApproching = (int)NotificationAlertType.RopeStopper_FireWire_Messanger_LooseEquipmentDisCard;
                }
                else if (LooseEqType == 5)
                {
                    LoosEqpName = "Chain Stopper";
                    NotiAlertTypeApproching = (int)NotificationAlertType.ChainStopper_LooseEquipmentDisCard;
                }
                else if (LooseEqType == 6)
                {
                    LoosEqpName = "FireWire";
                    NotiAlertTypeApproching = (int)NotificationAlertType.RopeStopper_FireWire_Messanger_LooseEquipmentDisCard;
                }
                else if (LooseEqType == 7)
                {
                    LoosEqpName = "Chafe Guard";
                    NotiAlertTypeApproching = (int)NotificationAlertType.ChafeGuard_LooseEquipmentDisCard;
                }
                else if (LooseEqType == 8)
                {
                    LoosEqpName = "Winch Break Test Kit";
                    NotiAlertTypeApproching = (int)NotificationAlertType.WinchBreakTest_LooseEquipmentDisCard;
                }
                else if (LooseEqType == 9)
                {
                    LoosEqpName = "Towing Rope";
                    NotiAlertTypeApproching = (int)NotificationAlertType.TowingRope_LooseEquipmentDisCard;
                }
                else if (LooseEqType == 10)
                {
                    LoosEqpName = "Suez Rope";
                    NotiAlertTypeApproching = (int)NotificationAlertType.SuezRope_LooseEquipmentDisCard;
                }
                else if (LooseEqType == 11)
                {
                    LoosEqpName = "Pennant Rope";
                    NotiAlertTypeApproching = (int)NotificationAlertType.PennantRope_LooseEquipmentDisCard;
                }
                else if (LooseEqType == 12)
                {
                    LoosEqpName = "Grommet Rope";
                    NotiAlertTypeApproching = (int)NotificationAlertType.GrommetRope_LooseEquipmentDisCard;
                }

                int InspectionFrequency = 0; int MaximumMonthsAllowed = 0; int MaximumRunningHours = 0;
                SqlDataAdapter pp = new SqlDataAdapter(" select EquipmentType,InspectionFrequency,MaximumMonthsAllowed,MaximumRunningHours from tblLooseEquipInspectionSetting where EquipmentType = " + LooseEqType + "", con);
                DataTable dtt = new DataTable();
                pp.Fill(dtt);
                if (dtt.Rows.Count > 0)
                {

                    InspectionFrequency = Convert.ToInt32(dtt.Rows[0]["InspectionFrequency"]);
                    MaximumRunningHours = Convert.ToInt32(dtt.Rows[0]["MaximumRunningHours"]);
                    MaximumMonthsAllowed = Convert.ToInt32(dtt.Rows[0]["MaximumMonthsAllowed"]);

                    if (MaximumRunningHours == 0)
                    {
                        return;
                    }
                    if (MaximumMonthsAllowed == 0)
                    {
                        return;
                    }
                    //// max month allowed notification

                    if (!string.IsNullOrEmpty(CheckIntallDT))
                    {
                        DateTime installdt = Convert.ToDateTime(CheckIntallDT);
                        int monthdiff = 0;
                        //SqlDataAdapter uu = new SqlDataAdapter("SELECT DATEDIFF(MONTH, '" + installdt.ToString("yyyy-MM-dd") + "', GETDATE()) AS DateDiff", sc.con);
                        //DataTable kk = new DataTable();
                        //uu.Fill(kk);
                        //if (kk.Rows.Count > 0)
                        //{
                        monthdiff = CommonClass.DateDiffInMonths(installdt, DateTime.Now.Date);// Convert.ToInt32(kk.Rows[0][0]);
                        int approachingmonthdiff = MaximumMonthsAllowed - 6;


                        if (monthdiff >= approachingmonthdiff)
                        {

                            var Max_allowable_time_approaching = "Max allowable time approaching (Current - " + monthdiff + " month / Max - " + MaximumMonthsAllowed + " Months) For " + LoosEqpName + " Loose Equipment # " + CertificatNum + "";

                            using (SqlDataAdapter adp = new SqlDataAdapter("select * from tblNotification where VesselID=" + VesselID + " and LooseEqType = " + LooseEqType + " and LooseCertificateNum = '" + LID + "' and NotificationAlertType = " + NotiAlertTypeApproching + "", con))
                            {
                                DataTable dt = new DataTable();
                                adp.Fill(dt);
                                if (dt.Rows.Count == 0)
                                {
                                    using (MorringOfficeEntities context = new MorringOfficeEntities())
                                    {
                                        int IdPK = ((from asn in context.tblNotifications.Where(x => x.VesselId == VesselID) select (int?)asn.Id).Max() ?? 0) + 1;
                                        tblNotification noti = new tblNotification();
                                        noti.Id = IdPK;
                                        noti.VesselId = VesselID;
                                        noti.Acknowledge = false;
                                        noti.AckRecord = "Not yet acknowledged";
                                        noti.Notification = Max_allowable_time_approaching;
                                        noti.RopeId = 0;
                                        noti.IsActive = true;
                                        // noti.NotificationType = 1;
                                        noti.NotificationDueDate = DateTime.Now.Date;
                                        noti.CreatedDate = DateTime.Now;
                                        noti.CreatedBy = "Admin";
                                        noti.NotificationType = NotiAlertTypeApproching;
                                        noti.LooseCertificateNum = LID.ToString();
                                        noti.LooseEqType = LooseEqType;
                                        context.tblNotifications.Add(noti);
                                        await context.SaveChangesAsync();
                                    }


                                }
                            }



                        }

                        if (monthdiff > MaximumMonthsAllowed)
                        {

                            var Max_allowable_time_exceeded = LoosEqpName + " Max allowable time of " + monthdiff + " month exceeded For " + LoosEqpName + " Loose Equipment # " + CertificatNum + "";
                            NotiAlertTypeApproching_Exceeded = NotiAlertTypeApproching + (int)NotificationAlertType.All_LooseEquipmentExceeded;
                            using (SqlDataAdapter adp = new SqlDataAdapter("select * from tblNotification where VesselID=" + VesselID + " and LooseEqType = " + LooseEqType + " and LooseCertificateNum = '" + LID + "' and NotificationAlertType = " + NotiAlertTypeApproching_Exceeded + "", con))
                            {
                                DataTable dt = new DataTable();
                                adp.Fill(dt);
                                if (dt.Rows.Count == 0)
                                {
                                    using (MorringOfficeEntities context = new MorringOfficeEntities())
                                    {
                                        int IdPK = ((from asn in context.tblNotifications.Where(x => x.VesselId == VesselID) select (int?)asn.Id).Max() ?? 0) + 1;
                                        tblNotification noti = new tblNotification();
                                        noti.Id = IdPK;
                                        noti.VesselId = VesselID;
                                        noti.Acknowledge = false;
                                        //noti.AckRecord = "Not yet acknowledged";
                                        noti.AckRecord = "This notification cannot be acknowledged, kindly discard it";
                                        noti.Notification = Max_allowable_time_exceeded;
                                        noti.RopeId = 0;
                                        noti.IsActive = true;
                                        //noti.NotificationType = 1;
                                        noti.NotificationDueDate = DateTime.Now.Date;
                                        noti.CreatedDate = DateTime.Now;
                                        noti.CreatedBy = "Admin";
                                        noti.NotificationType = NotiAlertTypeApproching_Exceeded;
                                        noti.LooseCertificateNum = LID.ToString();
                                        noti.LooseEqType = LooseEqType;
                                        context.tblNotifications.Add(noti);
                                        await context.SaveChangesAsync();

                                    }
                                }
                            }


                        }

                        //}

                    }
                }

            }
            catch (Exception ex) { }
        }

        private async Task InspectionDue7DayOrOerLooseEquipment(int VesselID, string Notification, DateTime DueDate, string LoosEqID, int LooseEqType, int AlertType)
        {
            int[] N7Day = { 21, 24, 27, 30, 33 };
            int[] N1Day = { 22, 25, 28, 31, 34 };
            int[] NOver = { 23, 26, 29, 32, 35 };
            int[] Nin = { 21, 24, 27, 30, 33, 22, 25, 28, 31, 34, 23, 26, 29, 32, 35 };
            string Certifinum = LoosEqID;

            using (MorringOfficeEntities context = new MorringOfficeEntities())
            {

                if (N7Day.Contains(AlertType) == true && DateTime.Now.Date >= DueDate.Date.AddDays(-7))
                {

                    using (SqlDataAdapter adp = new SqlDataAdapter("select * from tblNotification where VesselID=" + VesselID + " and LooseEqType = " + LooseEqType + " and LooseCertificateNum = '" + LoosEqID + "' and NotificationAlertType = " + AlertType + "", con))
                    {
                        DataTable dt = new DataTable();
                        adp.Fill(dt);
                        if (dt.Rows.Count == 0)
                        {
                            int IdPK = ((from asn in context.tblNotifications.Where(x => x.VesselId == VesselID) select (int?)asn.Id).Max() ?? 0) + 1;
                            tblNotification noti = new tblNotification();
                            noti.Id = IdPK;
                            noti.VesselId = VesselID;
                            noti.Acknowledge = false;
                            noti.AckRecord = "Not yet acknowledged";
                            noti.Notification = Notification;
                            noti.RopeId = 0;
                            noti.IsActive = true;
                            //noti.NotificationType = 1;
                            noti.NotificationDueDate = DueDate;
                            noti.CreatedDate = DateTime.Now;
                            noti.CreatedBy = "Admin";
                            noti.NotificationType = AlertType;
                            noti.LooseCertificateNum = Certifinum;
                            noti.LooseEqType = LooseEqType;
                            context.tblNotifications.Add(noti);
                            await context.SaveChangesAsync();



                        }
                    }
                }
                else if (N1Day.Contains(AlertType) == true && DateTime.Now.Date >= DueDate.Date.AddDays(-1))
                {
                    using (SqlDataAdapter adp = new SqlDataAdapter("select * from tblNotification where VesselID=" + VesselID + " and LooseEqType = " + LooseEqType + " and LooseCertificateNum = '" + LoosEqID + "' and NotificationAlertType = " + AlertType + "", con))
                    {
                        DataTable dt = new DataTable();
                        adp.Fill(dt);
                        if (dt.Rows.Count == 0)
                        {
                            int IdPK = ((from asn in context.tblNotifications.Where(x => x.VesselId == VesselID) select (int?)asn.Id).Max() ?? 0) + 1;
                            tblNotification noti = new tblNotification();
                            noti.Id = IdPK;
                            noti.VesselId = VesselID;
                            noti.LooseCertificateNum = Certifinum;
                            noti.LooseEqType = LooseEqType;
                            noti.Acknowledge = false;
                            noti.AckRecord = "Not yet acknowledged";
                            noti.Notification = Notification;
                            noti.RopeId = 0;
                            noti.IsActive = true;
                            //noti.NotificationType = 1;
                            noti.NotificationDueDate = DueDate;
                            noti.CreatedDate = DateTime.Now;
                            noti.CreatedBy = "Admin";
                            noti.NotificationType = AlertType;
                            context.tblNotifications.Add(noti);
                            await context.SaveChangesAsync();


                        }
                    }
                }
                else if (NOver.Contains(AlertType) == true && DateTime.Now.Date >= DueDate.Date.AddDays(1))
                {
                    using (SqlDataAdapter adp = new SqlDataAdapter("select * from tblNotification where VesselID=" + VesselID + " and LooseEqType = " + LooseEqType + " and LooseCertificateNum = '" + LoosEqID + "' and NotificationAlertType = " + AlertType + "", con))
                    {
                        DataTable dt = new DataTable();
                        adp.Fill(dt);
                        if (dt.Rows.Count == 0)
                        {
                            int IdPK = ((from asn in context.tblNotifications.Where(x => x.VesselId == VesselID) select (int?)asn.Id).Max() ?? 0) + 1;
                            tblNotification noti = new tblNotification();
                            noti.Id = IdPK;
                            noti.VesselId = VesselID;
                            noti.LooseCertificateNum = Certifinum;
                            noti.LooseEqType = LooseEqType;
                            noti.Acknowledge = false;
                            noti.AckRecord = "Not yet acknowledged";
                            noti.Notification = Notification;
                            noti.RopeId = 0;
                            noti.IsActive = true;
                            // noti.NotificationType = 1;
                            noti.NotificationDueDate = DueDate;
                            noti.CreatedDate = DateTime.Now;
                            noti.CreatedBy = "Admin";
                            noti.NotificationType = AlertType;
                            context.tblNotifications.Add(noti);
                            await context.SaveChangesAsync();


                        }
                    }
                }
                else if (Nin.Contains(AlertType) == false)
                {
                    using (SqlDataAdapter adp = new SqlDataAdapter("select * from tblNotification where VesselID=" + VesselID + " and LooseEqType = " + LooseEqType + " and LooseCertificateNum = '" + LoosEqID + "' and NotificationAlertType = " + AlertType + "", con))
                    {
                        DataTable dt = new DataTable();
                        adp.Fill(dt);
                        if (dt.Rows.Count == 0)
                        {
                            int IdPK = ((from asn in context.tblNotifications.Where(x => x.VesselId == VesselID) select (int?)asn.Id).Max() ?? 0) + 1;
                            tblNotification noti = new tblNotification();
                            noti.Id = IdPK;
                            noti.VesselId = VesselID;
                            noti.LooseCertificateNum = Certifinum;
                            noti.LooseEqType = LooseEqType;
                            noti.Acknowledge = false;
                            noti.AckRecord = "Not yet acknowledged";
                            noti.Notification = Notification;
                            noti.RopeId = 0;
                            noti.IsActive = true;
                            // noti.NotificationType = 1;
                            noti.NotificationDueDate = DueDate;
                            noti.CreatedDate = DateTime.Now;
                            noti.CreatedBy = "Admin";
                            noti.NotificationType = AlertType;
                            context.tblNotifications.Add(noti);
                            await context.SaveChangesAsync();


                        }
                    }
                }

            }


        }


        private async Task NorificationRopeDiscard(int VesselID)
        {
            try
            {
                // Rope Discard Required Only...........

                //SqlDataAdapter dd = new SqlDataAdapter("select * from mooringropedetail where OutofServiceDate is null and DeleteStatus = 0 and RopeTail = 0", sc.con);
                string qrys = @"SELECT distinct rope.Id,rope.RopeId,rope.RopeTypeId,rope.ManufacturerId,rope.CertificateNumber,rope.UniqueID, rope.InstalledDate,rope.CurrentRunningHours,rope.InspectionDueDate,
seting.MooringRopeType,seting.ManufacturerType,seting.MaximumRunningHours,seting.MaximumMonthsAllowed,seting.EndToEndMonth from MooringRopeDetail rope INNER JOIN tblRopeInspectionSetting seting on rope.RopeTypeId = seting.MooringRopeType 
INNER JOIN  tblRopeInspectionSetting Setter on Setter.ManufacturerType = rope.ManufacturerId
 where rope.OutofServiceDate is null and rope.DeleteStatus = 0 and rope.RopeTail = 0 and rope.VesselID=" + VesselID + "";
                SqlDataAdapter dd = new SqlDataAdapter(qrys, con);
                DataTable dds = new DataTable();
                dd.Fill(dds);
                for (int j = 0; j < dds.Rows.Count; j++)
                {
                    decimal CurrentRunningHours = 0;
                    int Ropeid = Convert.ToInt32(dds.Rows[j]["RopeId"]);
                    int RopeTid = Convert.ToInt32(dds.Rows[j]["RopeTypeId"]);
                    int ManuFId = Convert.ToInt32(dds.Rows[j]["ManufacturerId"]);
                    string certificatenumber = dds.Rows[j]["CertificateNumber"].ToString();
                    string uniqueId = dds.Rows[j]["UniqueID"].ToString();
                    int EndToEndMonth = string.IsNullOrEmpty(dds.Rows[j]["EndToEndMonth"].ToString()) == true ? 0 : Convert.ToInt32(dds.Rows[j]["EndToEndMonth"]);
                    string rnghr = dds.Rows[j]["CurrentRunningHours"].ToString();
                    if (!string.IsNullOrEmpty(rnghr))
                    {
                        CurrentRunningHours = Convert.ToDecimal(rnghr);
                    }

                    //if(Ropeid==54)
                    //{
                    //    int kl = 0;
                    //}

                    int ropetypeid = Convert.ToInt32(dds.Rows[j]["MooringRopeType"]);
                    int mid = Convert.ToInt32(dds.Rows[j]["ManufacturerType"]);

                    int MaxFixMonth = Convert.ToInt32(dds.Rows[j]["MaximumMonthsAllowed"]);

                    int maxrnghrs = Convert.ToInt32(dds.Rows[j]["MaximumRunningHours"]);

                    if (MaxFixMonth == 0)
                    {
                        continue;
                    }
                    if (maxrnghrs == 0)
                    {
                        continue;
                    }

                    try
                    {


                        string assignednumber = ""; string location = "Not Assigned"; //string certificatenumber = "";
                                                                                      // using (SqlDataAdapter pp = new SqlDataAdapter(" select b.AssignedNumber,b.Location from AssignRopeToWinch a join MooringWinchDetail b on a.WinchId = b.Id and a.VesselID=b.VesselID where a.IsActive=1 and a.VesselID=" + VesselID + " and a.RopeId = " + Ropeid + "", con))
                        using (SqlDataAdapter pp = new SqlDataAdapter("select a.*,b.AssignedNumber,b.Location from AssignRopeToWinch a inner join MooringWinchDetail b on a.WinchId = b.Id and a.VesselID=b.VesselID where a.IsActive=1 and a.VesselID=" + VesselID + " and a.RopeId = " + Ropeid + "", con))
                        {
                            DataTable dtt = new DataTable();
                            pp.Fill(dtt);
                            if (dtt.Rows.Count > 0)
                            {
                                DateTime AssignDate = Convert.ToDateTime(dtt.Rows[0]["AssignedDate"].ToString());
                                assignednumber = dtt.Rows[0]["AssignedNumber"].ToString();
                                location = dtt.Rows[0]["Location"].ToString();

                               await RopeEndToEndNotification(EndToEndMonth, certificatenumber, uniqueId, AssignDate, assignednumber, location, Ropeid, VesselID);
                            }
                        }




                        int total = (maxrnghrs * 80) / 100;


                        if (CurrentRunningHours >= total)
                        {     // approaching
                            if (assignednumber != null && assignednumber != "")
                            {

                                var Max_running_hours_Approaching = "Max running hours approaching (Current - " + CurrentRunningHours + "hrs / Max - " + maxrnghrs + " hrs) for Line '" + certificatenumber + " - " + uniqueId + "' located at " + assignednumber + " (" + location + ") ";
                                int NotiAlertType = (int)NotificationAlertType.Max_running_hours_Approaching;
                                await InspectNotification(VesselID, Ropeid, Max_running_hours_Approaching, NotiAlertType);
                            }
                            else
                            {
                                var Max_running_hours_Approaching = "Max running hours approaching (Current - " + CurrentRunningHours + "hrs / Max - " + maxrnghrs + " hrs) for Line '" + certificatenumber + " - " + uniqueId + "'";
                                int NotiAlertType = (int)NotificationAlertType.Max_running_hours_Approaching;
                                await InspectNotification(VesselID, Ropeid, Max_running_hours_Approaching, NotiAlertType);
                            }


                        }
                        else
                        {
                            approaching_exceeded_check(VesselID, Ropeid);
                        }

                        if (CurrentRunningHours >= maxrnghrs)
                        { // exceeded
                            if (assignednumber != null && assignednumber != "")
                            {
                                var Max_running_hours_exceeded = "Max running hours exceeded (Current - " + CurrentRunningHours + "hrs / Max - " + maxrnghrs + " hrs) for Line '" + certificatenumber + " - " + uniqueId + "' located at " + assignednumber + " (" + location + ") ";
                                int NotiAlertType = (int)NotificationAlertType.Max_running_hours_Exceeded;
                                await InspectNotification3(VesselID, Ropeid, Max_running_hours_exceeded, NotiAlertType);
                            }
                            else
                            {
                                var Max_running_hours_exceeded = "Max running hours exceeded (Current - " + CurrentRunningHours + "hrs / Max - " + maxrnghrs + " hrs) for Line '" + certificatenumber + " - " + uniqueId + "'";
                                int NotiAlertType = (int)NotificationAlertType.Max_running_hours_Exceeded;
                                await InspectNotification3(VesselID, Ropeid, Max_running_hours_exceeded, NotiAlertType);
                            }
                        }
                        else
                        {
                            approaching_exceeded_check(VesselID, Ropeid);
                        }


                        //// max month allowed notification
                        string CheckIntallDT = dds.Rows[j]["InstalledDate"].ToString();
                        if (!string.IsNullOrEmpty(CheckIntallDT))
                        {
                            DateTime installdt = Convert.ToDateTime(CheckIntallDT).Date;
                            int monthdiff = 0;
                            //SqlDataAdapter uu = new SqlDataAdapter("SELECT DATEDIFF(MONTH, '" + installdt.ToString("yyyy-MM-dd") + "', GETDATE()) AS DateDiff", sc.con);
                            //DataTable kk = new DataTable();
                            //uu.Fill(kk);
                            //if (kk.Rows.Count > 0)
                            //{
                            monthdiff = CommonClass.DateDiffInMonths(installdt, DateTime.Now.Date);// Convert.ToInt32(kk.Rows[0][0]);
                            int approachingmonthdiff = MaxFixMonth - 6;


                            if (monthdiff >= approachingmonthdiff)
                            {

                                if (assignednumber != null && assignednumber != "")
                                {
                                    var Max_allowable_time_approaching = "Max allowable time approaching (Current - " + monthdiff + " month / Max - " + MaxFixMonth + " Months) for Line '" + certificatenumber + " - " + uniqueId + "' located at " + assignednumber + " (" + location + ") ";
                                    int NotiAlertType = (int)NotificationAlertType.Max_allowable_time_Approaching;
                                    await InspectNotification(VesselID, Ropeid, Max_allowable_time_approaching, NotiAlertType);
                                }
                                else
                                {
                                    var Max_allowable_time_approaching = "Max allowable time approaching (Current - " + monthdiff + " month / Max - " + MaxFixMonth + " Months) for Line '" + certificatenumber + " - " + uniqueId + "'";
                                    int NotiAlertType = (int)NotificationAlertType.Max_allowable_time_Approaching;
                                    await InspectNotification(VesselID, Ropeid, Max_allowable_time_approaching, NotiAlertType);
                                }
                            }
                            else
                            {
                                time_approaching_exceeded_check(VesselID, Ropeid);
                            }

                            if (monthdiff > MaxFixMonth)
                            {
                                if (assignednumber != null && assignednumber != "")
                                {
                                    var Max_allowable_time_exceeded = "Max allowable time exceeded (Current - " + monthdiff + " month / Max - " + MaxFixMonth + " Months) for Line '" + certificatenumber + " - " + uniqueId + "' located at " + assignednumber + " (" + location + ")";
                                    // var Max_allowable_time_exceeded = "Max allowable time of " + monthdiff + " month exceeded for Rope '" + certificatenumber + "' located at " + assignednumber + " (" + location + ") ";
                                    int NotiAlertType = (int)NotificationAlertType.Max_allowable_time_Exceeded;
                                    await InspectNotification3(VesselID, Ropeid, Max_allowable_time_exceeded, NotiAlertType);
                                }
                                else
                                {
                                    var Max_allowable_time_exceeded = "Max allowable time exceeded (Current - " + monthdiff + " month / Max - " + MaxFixMonth + " Months) for Line '" + certificatenumber + " - " + uniqueId + "'";
                                    // var Max_allowable_time_exceeded = "Max allowable time of " + monthdiff + " month exceeded for Rope '" + certificatenumber + "'";
                                    int NotiAlertType = (int)NotificationAlertType.Max_allowable_time_Exceeded;
                                   await InspectNotification3(VesselID, Ropeid, Max_allowable_time_exceeded, NotiAlertType);
                                }
                            }
                            else
                            {
                                time_approaching_exceeded_check(VesselID, Ropeid);
                            }

                            //}
                        }
                        //}
                    }
                    catch { }




                    //  }
                    //}
                }
            }
            catch { }

            //=========================================================================================

            try
            {
                // RopeTail Discard Required Only...........

                //SqlDataAdapter dd = new SqlDataAdapter("select * from mooringropedetail where OutofServiceDate is null and DeleteStatus = 0 and RopeTail = 0", sc.con);
                string qrys = @" SELECT distinct rope.Id,rope.RopeId,rope.RopeTypeId,rope.ManufacturerId,rope.CertificateNumber,rope.UniqueID, rope.InstalledDate,rope.CurrentRunningHours,rope.InspectionDueDate,
seting.MooringRopeType,seting.ManufacturerType,seting.MaximumRunningHours,seting.MaximumMonthsAllowed from MooringRopeDetail rope INNER JOIN tblRopeTailInspectionSetting seting on rope.RopeTypeId = seting.MooringRopeType 
INNER JOIN  tblRopeTailInspectionSetting Setter on Setter.ManufacturerType = rope.ManufacturerId
 where rope.OutofServiceDate is null and rope.DeleteStatus = 0 and rope.RopeTail = 1 and rope.VesselID="+VesselID+"";
                SqlDataAdapter dd = new SqlDataAdapter(qrys, con);
                DataTable dds = new DataTable();
                dd.Fill(dds);
                for (int j = 0; j < dds.Rows.Count; j++)
                {
                    decimal CurrentRunningHours = 0;
                    int Ropeid = Convert.ToInt32(dds.Rows[j]["RopeId"]);
                    int RopeTid = Convert.ToInt32(dds.Rows[j]["RopeTypeId"]);
                    int ManuFId = Convert.ToInt32(dds.Rows[j]["ManufacturerId"]);
                    string certificatenumber = dds.Rows[j]["CertificateNumber"].ToString();
                    string uniqueId = dds.Rows[j]["UniqueID"].ToString();
                    string rnghr = dds.Rows[j]["CurrentRunningHours"].ToString();

                    if (!string.IsNullOrEmpty(rnghr))
                    {
                        CurrentRunningHours = Convert.ToDecimal(rnghr);
                    }


                    int ropetypeid = Convert.ToInt32(dds.Rows[j]["MooringRopeType"]);
                    int mid = Convert.ToInt32(dds.Rows[j]["ManufacturerType"]);

                    int MaxFixMonth = Convert.ToInt32(dds.Rows[j]["MaximumMonthsAllowed"]);

                    int maxrnghrs = Convert.ToInt32(dds.Rows[j]["MaximumRunningHours"]);

                    if (MaxFixMonth == 0)
                    {
                        continue;
                    }
                    if (maxrnghrs == 0)
                    {
                        continue;
                    }

                    try
                    {


                        string assignednumber = ""; string location = "Not Assigned"; //string certificatenumber = "";
                        using (SqlDataAdapter pp = new SqlDataAdapter(" select b.AssignedNumber,b.Location from AssignRopeToWinch a join MooringWinchDetail b on a.WinchId = b.Id and a.VesselID=b.VesselID where a.IsActive=1 and a.VesselID=" + VesselID + " and a.RopeId = " + Ropeid + "", con))
                        {
                            DataTable dtt = new DataTable();
                            pp.Fill(dtt);
                            if (dtt.Rows.Count > 0)
                            {

                                assignednumber = dtt.Rows[0]["AssignedNumber"].ToString();
                                location = dtt.Rows[0]["Location"].ToString();

                            }
                        }



                        int total = (maxrnghrs * 80) / 100;

                        if (CurrentRunningHours >= total)
                        {     // approaching
                            if (assignednumber != null && assignednumber != "")
                            {

                                var Max_running_hours_Approaching = "Max running hours approaching (Current - " + CurrentRunningHours + "hrs / Max - " + maxrnghrs + " hrs) for RopeTail '" + certificatenumber + " - " + uniqueId + "' located at " + assignednumber + " (" + location + ") ";
                                int NotiAlertType = (int)NotificationAlertType.Max_running_hours_Approaching;
                               await InspectNotification(VesselID, Ropeid, Max_running_hours_Approaching, NotiAlertType);
                            }
                            else
                            {
                                var Max_running_hours_Approaching = "Max running hours approaching (Current - " + CurrentRunningHours + "hrs / Max - " + maxrnghrs + " hrs) for RopeTail '" + certificatenumber + " - " + uniqueId + "'";
                                int NotiAlertType = (int)NotificationAlertType.Max_running_hours_Approaching;
                                await InspectNotification(VesselID, Ropeid, Max_running_hours_Approaching, NotiAlertType);
                            }


                        }
                        else
                        {
                            approaching_exceeded_check(VesselID, Ropeid);
                        }

                        if (CurrentRunningHours >= maxrnghrs)
                        { // exceeded
                            if (assignednumber != null && assignednumber != "")
                            {
                                var Max_running_hours_exceeded = "Max running hours exceeded (Current - " + CurrentRunningHours + "hrs / Max - " + maxrnghrs + " hrs) for RopeTail '" + certificatenumber + " - " + uniqueId + "' located at " + assignednumber + " (" + location + ") ";
                                int NotiAlertType = (int)NotificationAlertType.Max_running_hours_Exceeded;
                                await InspectNotification3(VesselID, Ropeid, Max_running_hours_exceeded, NotiAlertType);
                            }
                            else
                            {
                                var Max_running_hours_exceeded = "Max running hours exceeded (Current - " + CurrentRunningHours + "hrs / Max - " + maxrnghrs + " hrs) for RopeTail '" + certificatenumber + " - " + uniqueId + "'";
                                int NotiAlertType = (int)NotificationAlertType.Max_running_hours_Exceeded;
                                await InspectNotification3(VesselID, Ropeid, Max_running_hours_exceeded, NotiAlertType);
                            }
                        }
                        else
                        {
                            approaching_exceeded_check(VesselID, Ropeid);
                        }


                        //// max month allowed notification
                        string CheckIntallDT = dds.Rows[j]["InstalledDate"].ToString();
                        if (!string.IsNullOrEmpty(CheckIntallDT))
                        {
                            DateTime installdt = Convert.ToDateTime(CheckIntallDT).Date;
                            int monthdiff = 0;
                            //SqlDataAdapter uu = new SqlDataAdapter("SELECT DATEDIFF(MONTH, '" + installdt.ToString("yyyy-MM-dd") + "', GETDATE()) AS DateDiff", sc.con);
                            //DataTable kk = new DataTable();
                            //uu.Fill(kk);
                            //if (kk.Rows.Count > 0)
                            //{

                            monthdiff = CommonClass.DateDiffInMonths(installdt, DateTime.Now.Date);// Convert.ToInt32(kk.Rows[0][0]);
                            int approachingmonthdiff = MaxFixMonth - 6;


                            if (monthdiff >= approachingmonthdiff)
                            {

                                if (assignednumber != null && assignednumber != "")
                                {
                                    var Max_allowable_time_approaching = "Max allowable time approaching (Current - " + monthdiff + " month / Max - " + MaxFixMonth + " Months) for RopeTail '" + certificatenumber + " - " + uniqueId + "' located at " + assignednumber + " (" + location + ") ";
                                    int NotiAlertType = (int)NotificationAlertType.Max_allowable_time_Approaching;
                                    await InspectNotification(VesselID, Ropeid, Max_allowable_time_approaching, NotiAlertType);
                                }
                                else
                                {
                                    var Max_allowable_time_approaching = "Max allowable time approaching (Current - " + monthdiff + " month / Max - " + MaxFixMonth + " Months) for RopeTail '" + certificatenumber + " - " + uniqueId + "'";
                                    int NotiAlertType = (int)NotificationAlertType.Max_allowable_time_Approaching;
                                    await InspectNotification(VesselID, Ropeid, Max_allowable_time_approaching, NotiAlertType);

                                }

                            }
                            //else
                            //{
                            //    time_approaching_exceeded_check(Ropeid);
                            //}

                            if (monthdiff > MaxFixMonth)
                            {
                                if (assignednumber != null && assignednumber != "")
                                {
                                    //string ddd = "Max allowable time exceeded (Current - " + monthdiff + " month / Max - " + MaxFixMonth + " Months) for RopeTail '" + certificatenumber + "' located at " + assignednumber + " (" + location + ")";
                                    //var Max_allowable_time_exceeded = "Max allowable time of " + monthdiff + " month exceeded for RopeTail '" + certificatenumber + "' located at " + assignednumber + " (" + location + ") ";
                                    var Max_allowable_time_exceeded = "Max allowable time exceeded (Current - " + monthdiff + " month / Max - " + MaxFixMonth + " Months) for RopeTail '" + certificatenumber + " - " + uniqueId + "' located at " + assignednumber + " (" + location + ")";
                                    int NotiAlertType = (int)NotificationAlertType.Max_allowable_time_Exceeded;
                                    await InspectNotification3(VesselID, Ropeid, Max_allowable_time_exceeded, NotiAlertType);
                                }
                                else
                                {
                                    var Max_allowable_time_exceeded = "Max allowable time exceeded (Current - " + monthdiff + " month / Max - " + MaxFixMonth + " Months) for RopeTail '" + certificatenumber + " - " + uniqueId + "'";
                                    //var Max_allowable_time_exceeded = "Max allowable time of " + monthdiff + " month exceeded for RopeTail '" + certificatenumber + "'";
                                    int NotiAlertType = (int)NotificationAlertType.Max_allowable_time_Exceeded;
                                    await InspectNotification3(VesselID, Ropeid, Max_allowable_time_exceeded, NotiAlertType);
                                }
                            }
                            //else
                            //{
                            //    time_approaching_exceeded_check(Ropeid);
                            //}

                            //}
                        }
                        //}
                    }
                    catch (Exception ex)
                    { }

                    //  }
                    //}
                }
            }
            catch (Exception ex)
            { }
        }
        private void time_approaching_exceeded_check(int VesselID, int ropeId)
        {
            try
            {
                using (SqlDataAdapter adp = new SqlDataAdapter("delete from tblNotification where VesselId=" + VesselID + " and Notification like '%time approaching%' and RopeId=" + ropeId + "", con))
                {
                    DataTable dt = new DataTable();
                    adp.Fill(dt);
                    //if (dt.Rows.Count > 0)
                    //{
                    //    int notiid = Convert.ToInt32(dt.Rows[0]["Id"]);

                    //    SqlDataAdapter pp = new SqlDataAdapter("delete from Notifications where id=" + notiid + "", sc.con);
                    //    DataTable dd = new DataTable();
                    //    pp.Fill(dd);
                    //}
                }

                using (SqlDataAdapter adp1 = new SqlDataAdapter("delete from tblNotification where VesselId=" + VesselID + " and  Notification like '%time exceeded%' and RopeId=" + ropeId + "", con))
                {
                    DataTable dt1 = new DataTable();
                    adp1.Fill(dt1);
                    //if (dt1.Rows.Count > 0)
                    //{
                    //    int notiid1 = Convert.ToInt32(dt1.Rows[0]["Id"]);

                    //    SqlDataAdapter pp1 = new SqlDataAdapter("delete from Notifications where id=" + notiid1 + "", sc.con);
                    //    DataTable dd1 = new DataTable();
                    //    pp1.Fill(dd1);
                    //}
                }
            }
            catch { }
        }
        private void approaching_exceeded_check(int VesselID, int ropeId)
        {
            try
            {
                //using (SqlDataAdapter adp = new SqlDataAdapter("select * from tblNotification where VesselId=" + VesselID + " and Notification like '%hours approaching%' and RopeId=" + ropeId + "", con))
                using (SqlDataAdapter adp = new SqlDataAdapter("delete from tblNotification where VesselId=" + VesselID + " and Notification like '%hours approaching%' and RopeId=" + ropeId + "", con))
                {
                    DataTable dt = new DataTable();
                    adp.Fill(dt);
                    //if (dt.Rows.Count > 0)
                    //{
                    //    int notiid = Convert.ToInt32(dt.Rows[0]["Id"]);

                    //    SqlDataAdapter pp = new SqlDataAdapter("delete from tblNotification where VesselId=" + VesselID + " and id=" + notiid + "", con);
                    //    DataTable dd = new DataTable();
                    //    pp.Fill(dd);
                    //}
                }

                using (SqlDataAdapter adp1 = new SqlDataAdapter("delete from tblNotification where VesselId=" + VesselID + " and  Notification like '%hours exceeded%' and RopeId=" + ropeId + "", con))
                {
                    DataTable dt1 = new DataTable();
                    adp1.Fill(dt1);
                    //if (dt1.Rows.Count > 0)
                    //{
                    //    int notiid1 = Convert.ToInt32(dt1.Rows[0]["Id"]);

                    //    SqlDataAdapter pp1 = new SqlDataAdapter("delete from tblNotification where VesselId=" + VesselID + " and id=" + notiid1 + "", con);
                    //    DataTable dd1 = new DataTable();
                    //    pp1.Fill(dd1);
                    //}
                }
            }
            catch { }
        }

        
        public async Task RopeEndToEndNotification(int End2EndMonth,string certificatenumber, string uniqueId, DateTime AssignDate, string assignednumber, string location, int Ropeid,int VesselID)
        {
            //AssignDate = Convert.ToDateTime(dtt.Rows[k]["AssignedDate"]);
            //assignednumber = dtt.Rows[k]["AssignedNumber"].ToString();
            //location = dtt.Rows[k]["Location"].ToString();

            DateTime AssignEndTOEndDate; //DateTime rotationdt;

            SqlDataAdapter hh = new SqlDataAdapter("select MAX(EndtoEndDoneDate) as MaxEndtoEndDoneDate from RopeEndtoEnd2 where IsActive=1 and VesselID="+ VesselID + " and RopeId=" + Ropeid + "", con);
            DataTable dh = new DataTable();
            hh.Fill(dh);
            if (dh.Rows.Count > 0)
            {
                string MaxE2E = dh.Rows[0][0].ToString();
                if (!string.IsNullOrEmpty(MaxE2E))
                {
                    AssignEndTOEndDate = Convert.ToDateTime(MaxE2E);
                }
                else
                {
                    AssignEndTOEndDate = AssignDate;
                }
            }
            else
            {
                AssignEndTOEndDate = AssignDate; //Convert.ToDateTime(dds.Rows[j]["InstalledDate"]);

            }


            int monthdiff = 0;
            monthdiff = CommonClass.DateDiffInMonths(AssignEndTOEndDate, DateTime.Now.Date);// Convert.ToInt32(kk.Rows[0][0]);

            if (monthdiff >= (End2EndMonth - 1))
            {
                var End_to_end_approaching = "Line End to End Approaching (Current - " + monthdiff + "/ Reqd - " + End2EndMonth + " Months) for Line '" + certificatenumber + " - " + uniqueId + "' located at " + assignednumber + " (" + location + ") ";
                int NotiAlertType = (int)NotificationAlertType.End_to_End_Approaching;
                await InspectNotification(VesselID, Ropeid, End_to_end_approaching, NotiAlertType);
            }
            if (monthdiff > End2EndMonth)
            {
                var End_to_end_overdue = "Line End to End Overdue (Current - " + monthdiff + "/ Reqd - " + End2EndMonth + " Months) for Line '" + certificatenumber + " - " + uniqueId + "' located at " + assignednumber + " (" + location + ") ";
                int NotiAlertType = (int)NotificationAlertType.End_to_End_Overdue;
               await InspectNotification(VesselID, Ropeid, End_to_end_overdue, NotiAlertType);
            }
        }

        public async Task WinchrotationSetting_and_Notifications(int VesselID)
        {
            try
            {


                string qry = @"select distinct a.WinchId,b.AssignedNumber,b.Location, b.lead,a.AssignedDate,R.RopeId as RopeID,R.CurrentLeadRunningHours,R.ManufacturerId,R.RopeTypeId,R.UniqueID,R.CertificateNumber,
	T.RopeType,M.Name from AssignRopeToWinch a inner join MooringWinchDetail b on a.WinchId=b.Id and a.VesselID=b.VesselID
and a.IsActive=1 and a.RopeTail=0 join MooringRopeDetail R on R.RopeId=a.RopeId and R.VesselID=a.VesselID
and R.RopeTail=0 and R.DeleteStatus=0  and R.OutofServiceDate is null
join tblCommon M on M.Id=R.ManufacturerId join MooringRopeType T on T.Id=R.RopeTypeId
where a.VesselID = " + VesselID + "";   // and R.RopeId = " + Ropeid + "
                using (SqlDataAdapter ssda = new SqlDataAdapter(qry, con))
                {
                    DataTable tbls = new DataTable();
                    ssda.Fill(tbls);
                    if (tbls.Rows.Count > 0)
                    {
                        //int ropetypeid = Convert.ToInt32(tbls.Rows[0]["RopeTypeId"]);
                        //int manuFid = Convert.ToInt32(tbls.Rows[0]["ManufacturerId"]);
                        //decimal CurrentLeadRunningHours = Convert.ToInt32(tbls.Rows[0]["ManufacturerId"]);
                        //string lead = tbls.Rows[0]["lead"].ToString();
                        //int winchid = Convert.ToInt32(tbls.Rows[0]["WinchId"]);
                        int ApprochingCount = 0; int ExceedCount = 0;
                        for (int i = 0; i < tbls.Rows.Count; i++)
                        {
                            int ManufacturerId = Convert.ToInt32(tbls.Rows[i]["ManufacturerId"]);
                            int RopeTypeId = Convert.ToInt32(tbls.Rows[i]["RopeTypeId"]);
                            int ropeid = Convert.ToInt32(tbls.Rows[i]["RopeID"]);
                            string TestLeadRunningHours = tbls.Rows[i]["CurrentLeadRunningHours"].ToString();
                            decimal CurrentLeadRunningHours = 0;
                            if (!string.IsNullOrEmpty(TestLeadRunningHours))
                            {
                                CurrentLeadRunningHours = Convert.ToDecimal(tbls.Rows[i]["CurrentLeadRunningHours"]);
                            }

                            string Lead = tbls.Rows[i]["lead"].ToString();
                            //Lead = Lead.Replace(System.Environment.NewLine, "").Trim();
                            DateTime AssignedDate = Convert.ToDateTime(tbls.Rows[i]["AssignedDate"]);
                            string UniqueID = tbls.Rows[i]["UniqueID"].ToString();
                            string CertificateNum = tbls.Rows[i]["CertificateNumber"].ToString();
                            string WinchAssignedNumber = tbls.Rows[i]["AssignedNumber"].ToString();

                            using (SqlDataAdapter pp1 = new SqlDataAdapter("select * from tblWinchRotationSetting where VesselID = " + VesselID + " and mooringropetype = " + RopeTypeId + " and ManufacturerType= " + ManufacturerId + " and LeadFrom='" + Lead + "'", con))
                            {
                                DataTable WRSetting = new DataTable();
                                pp1.Fill(WRSetting);
                                if (WRSetting.Rows.Count > 0)
                                {
                                    //int maxrunhrs = Convert.ToInt32(dd1.Rows[0]["MaximumRunningHours"]);
                                    //int maxmnthallowed = Convert.ToInt32(dd1.Rows[0]["MaximumMonthsAllowed"]);
                                    string leadFrom = WRSetting.Rows[0]["LeadFrom"].ToString();
                                    string leadto = WRSetting.Rows[0]["LeadTo"].ToString();

                                    int maxrunhrs = Convert.ToInt32(WRSetting.Rows[0]["MaximumRunningHours"]);
                                    int maxmnthallowed = Convert.ToInt32(WRSetting.Rows[0]["MaximumMonthsAllowed"]);

                                    var AssignedDateAppro = AssignedDate.AddMonths(maxmnthallowed - 2);
                                    var AssignedDateExceed = AssignedDate.AddMonths(maxmnthallowed);
                                    var CurrentDate = DateTime.Now.Date;//.AddMonths(maxmnthallowed);



                                    //Approching Count Start
                                    #region
                                    int RB = 0; int MA = 0;
                                    var WinchMonthdiff = CommonClass.DateDiffInMonths(AssignedDate, DateTime.Now.Date);//
                                    if (CurrentDate >= AssignedDateAppro && CurrentDate < AssignedDateExceed)
                                    {

                                        var winchrotation = "Winch Rotation is Approaching for Line " + CertificateNum + " - " + UniqueID + " currently on Winch " + WinchAssignedNumber + " lead  " + Lead + " , Current " + WinchMonthdiff + " month / Rotation was due at " + maxmnthallowed + " month,  Please shift from " + Lead + " to " + leadto + "";

                                        int NotiAlertType = (int)NotificationAlertType.Winch_Rotation_approching;
                                       await InspectNotification(VesselID , ropeid, winchrotation, NotiAlertType);
                                        MA++;

                                    }

                                    if (maxrunhrs > 0)
                                    {
                                        var maxrunhrs1 = maxrunhrs * 90 / 100;
                                        if (CurrentLeadRunningHours > maxrunhrs1)
                                        {

                                            var winchrotation = "Winch Rotation is Approaching for Line " + CertificateNum + " - " + UniqueID + " currently on Winch " + WinchAssignedNumber + " lead  " + Lead + " , Current " + CurrentLeadRunningHours + " hrs / Rotation was due at " + maxrunhrs + " hrs,  Please shift from " + Lead + " to " + leadto + "";

                                            int NotiAlertType = (int)NotificationAlertType.Winch_Rotation_approching;
                                          await InspectNotification(VesselID,ropeid, winchrotation, NotiAlertType);
                                            RB++;
                                        }
                                    }

                                    if (RB + MA > 0)
                                    {
                                        ApprochingCount++;
                                    }

                                    //Approching Count End
                                    #endregion
                                    //*********************************************************

                                    //Exceeded Count Start
                                    #region
                                    int RB2 = 0; int MA2 = 0;
                                    if (CurrentDate >= AssignedDateExceed)
                                    {
                                        var winchrotation = "Winch Rotation was Exceeded for Line " + CertificateNum + " - " + UniqueID + " currently on Winch " + WinchAssignedNumber + " lead  " + Lead + " , Current " + WinchMonthdiff + " month / Rotation was due at " + maxmnthallowed + " month,  Please shift from " + Lead + " to " + leadto + "";

                                        int NotiAlertType = (int)NotificationAlertType.Winch_Rotation_exceed;
                                       await InspectNotification(VesselID, ropeid, winchrotation, NotiAlertType);
                                        MA2++;
                                    }

                                    if (maxrunhrs > 0)
                                    {
                                        //var maxrunhrs1 = maxrunhrs * 90 / 100;
                                        if (CurrentLeadRunningHours > maxrunhrs)
                                        {
                                            var winchrotation = "Winch Rotation was Exceeded for Line " + CertificateNum + " - " + UniqueID + " currently on Winch " + WinchAssignedNumber + " lead  " + Lead + " , Current " + CurrentLeadRunningHours + " hrs / Rotation was due at " + maxrunhrs + " hrs,  Please shift from " + Lead + " to " + leadto + "";

                                            int NotiAlertType = (int)NotificationAlertType.Winch_Rotation_exceed;
                                           await InspectNotification(VesselID, ropeid, winchrotation, NotiAlertType);
                                            RB2++;
                                        }
                                    }

                                    if (RB2 + MA2 > 0)
                                    {
                                        ExceedCount++;
                                    }

                                    //Exceeded Count End
                                    #endregion


                                }
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {

                // sc.ErrorLog(ex);
            }
        }

    }

    public class Alerts
    {
        static int VesselID;
        //public Alerts()
        //{
        //    VesselID = Convert.ToInt32(CommonClass.VesselSessionID);

        //}
        public async Task InspectNotification(int VesselID, int RopeID, string NotiMsg, int NotiAlertType, DateTime DueDate)
        {
            using (MorringOfficeEntities context = new MorringOfficeEntities())
            {
                var result = context.tblNotifications.Where(x => x.VesselId == VesselID & x.RopeId == RopeID & x.NotificationDueDate == DueDate & x.NotificationType == NotiAlertType).FirstOrDefault();
                if (result == null)
                {
                    int IdPK = ((from asn in context.tblNotifications.Where(x => x.VesselId == VesselID) select (int?)asn.Id).Max() ?? 0) + 1;
                    if (NotiAlertType == 1 && DateTime.Now.Date >= DueDate.Date.AddDays(-7))
                    {
                        tblNotification noti = new tblNotification();
                        noti.Id = IdPK;
                        noti.VesselId = VesselID;
                        noti.Acknowledge = false;
                        noti.AckRecord = "Not yet acknowledged";
                        noti.Notification = NotiMsg;
                        noti.RopeId = RopeID;
                        noti.IsActive = true;
                        noti.NotificationDueDate = DueDate;

                        // var notiDate = NotiAlertType == 1 ? DateTime.Now : NotiAlertType == 2 ? DueDate.AddDays(-1) : DueDate.AddDays(-1);
                        noti.CreatedDate = DateTime.Now;
                        noti.CreatedBy = "Admin";
                        //noti.NotificationAlertType = NotiAlertType;
                        noti.NotificationType = NotiAlertType;
                        context.tblNotifications.Add(noti);
                        await context.SaveChangesAsync();
                        //Task.FromResult(0);
                        // StaticHelper.AlarmFunction(1, StaticHelper.AlarmCheck);
                    }
                    if (NotiAlertType == 2 && DateTime.Now.Date >= DueDate.Date.AddDays(-1))
                    {
                        tblNotification noti = new tblNotification();
                        noti.Id = IdPK;
                        noti.VesselId = VesselID;
                        noti.Acknowledge = false;
                        noti.AckRecord = "Not yet acknowledged";
                        noti.Notification = NotiMsg;
                        noti.RopeId = RopeID;
                        noti.IsActive = true;
                        noti.NotificationDueDate = DueDate;

                        // var notiDate = NotiAlertType == 1 ? DateTime.Now : NotiAlertType == 2 ? DueDate.AddDays(-1) : DueDate.AddDays(-1);
                        noti.CreatedDate = DateTime.Now;
                        noti.CreatedBy = "Admin";
                        //noti.NotificationAlertType = NotiAlertType;
                        noti.NotificationType = NotiAlertType;
                        context.tblNotifications.Add(noti);
                        await context.SaveChangesAsync();
                        //context.SaveChanges();

                        // StaticHelper.AlarmFunction(1, StaticHelper.AlarmCheck);
                    }
                    if (NotiAlertType == 3 && DateTime.Now.Date >= DueDate.Date.AddDays(1))
                    {
                        tblNotification noti = new tblNotification();
                        noti.Id = IdPK;
                        noti.VesselId = VesselID;
                        noti.Acknowledge = false;
                        noti.AckRecord = "Not yet acknowledged";
                        noti.Notification = NotiMsg;
                        noti.RopeId = RopeID;
                        noti.IsActive = true;
                        noti.NotificationDueDate = DueDate;

                        // var notiDate = NotiAlertType == 1 ? DateTime.Now : NotiAlertType == 2 ? DueDate.AddDays(-1) : DueDate.AddDays(-1);
                        noti.CreatedDate = DateTime.Now;
                        noti.CreatedBy = "Admin";
                        // noti.NotificationAlertType = NotiAlertType;
                        noti.NotificationType = NotiAlertType;
                        context.tblNotifications.Add(noti);
                        await context.SaveChangesAsync();

                        // StaticHelper.AlarmFunction(1, StaticHelper.AlarmCheck);
                    }
                    if (NotiAlertType > 3)
                    {
                        tblNotification noti = new tblNotification();
                        noti.Id = IdPK;
                        noti.VesselId = VesselID;
                        noti.Acknowledge = false;
                        noti.AckRecord = "Not yet acknowledged";
                        noti.Notification = NotiMsg;
                        noti.RopeId = RopeID;
                        noti.IsActive = true;
                        noti.NotificationDueDate = DueDate;

                        // var notiDate = NotiAlertType == 1 ? DateTime.Now : NotiAlertType == 2 ? DueDate.AddDays(-1) : DueDate.AddDays(-1);
                        noti.CreatedDate = DateTime.Now;
                        noti.CreatedBy = "Admin";
                        // noti.NotificationAlertType = NotiAlertType;
                        noti.NotificationType = NotiAlertType;
                        context.tblNotifications.Add(noti);
                        await context.SaveChangesAsync();

                        // StaticHelper.AlarmFunction(1, StaticHelper.AlarmCheck);
                    }

                }
            }
        }
        public async Task InspectNotification(int VesselID, int RopeID, string NotiMsg, int NotiAlertType)
        {
            using (MorringOfficeEntities context = new MorringOfficeEntities())
            {
                var result = context.tblNotifications.Where(x => x.VesselId == VesselID & x.RopeId == RopeID & x.NotificationType == NotiAlertType).FirstOrDefault();
                if (result == null)
                {
                    int IdPK = ((from asn in context.tblNotifications.Where(x => x.VesselId == VesselID) select (int?)asn.Id).Max() ?? 0) + 1;
                    tblNotification noti = new tblNotification();
                    noti.Id = IdPK;
                    noti.VesselId = VesselID;
                    noti.Acknowledge = false;
                    noti.AckRecord = "Not yet acknowledged";
                    noti.Notification = NotiMsg;
                    noti.RopeId = RopeID;
                    noti.IsActive = true;
                    noti.NotificationDueDate = DateTime.Now.Date;
                    noti.CreatedDate = DateTime.Now;
                    noti.CreatedBy = "Admin";
                    // noti.NotificationAlertType = NotiAlertType;
                    noti.NotificationType = NotiAlertType;
                    context.tblNotifications.Add(noti);
                    await context.SaveChangesAsync();

                }
            }
        }

        public async Task InspectNotification3(int VesselID, int RopeID, string NotiMsg, int NotiAlertType)
        {
            using (MorringOfficeEntities context = new MorringOfficeEntities())
            {
                var result = context.tblNotifications.Where(x => x.VesselId == VesselID & x.RopeId == RopeID & x.NotificationType == NotiAlertType).FirstOrDefault();
                if (result == null)
                {
                    int IdPK = ((from asn in context.tblNotifications.Where(x => x.VesselId == VesselID) select (int?)asn.Id).Max() ?? 0) + 1;
                    tblNotification noti = new tblNotification();
                    noti.Id = IdPK;
                    noti.VesselId = VesselID;
                    noti.Acknowledge = false;
                    //noti.AckRecord = "Not yet acknowledged";
                    noti.AckRecord = "This notification cannot be acknowledged, kindly discard it";
                    noti.Notification = NotiMsg;
                    noti.RopeId = RopeID;
                    noti.IsActive = true;
                    noti.NotificationDueDate = DateTime.Now.Date;
                    noti.CreatedDate = DateTime.Now;
                    noti.CreatedBy = "Admin";
                    // noti.NotificationAlertType = NotiAlertType;
                    noti.NotificationType = NotiAlertType;
                    context.tblNotifications.Add(noti);
                    await context.SaveChangesAsync();

                    //StaticHelper.AlarmFunction(1, StaticHelper.AlarmCheck);
                }
            }
        }
    }
}