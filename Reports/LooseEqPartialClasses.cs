using MenuLayer;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;

namespace Reports
{
    public partial class JoiningShackle
    {
        SqlConnection con = ConnectionBulder.con;
        public JoiningShackle()
        {
            JSType = GetJoiningSkType();

            //JSType = new List<SelectListItem>
            //{
            //    new SelectListItem { Text = "User", Value = "USER" },
            //    new SelectListItem { Text = "Admin", Value = "ADMIN" },
            //     new SelectListItem { Text = "HOD", Value = "HOD" }
            //};
        }
        public List<JoiningShackle> JoiningShackleList { get; set; }
        public List<SelectListItem> JSType { get; set; }
        public string IsInstalled { get; set; }

        public string InspectionDueDate1 { get; set; }
        public string DateInstalled1 { get; set; }
        public string DateReceived1 { get; set; }
        public List<SelectListItem> GetJoiningSkType()
        {
            List<SelectListItem> jst = new List<SelectListItem>();
            using (SqlDataAdapter sda = new SqlDataAdapter("Select * from JoiningShackleType", con))
            {
                DataTable tbl = new DataTable();
                sda.Fill(tbl);
                if (tbl.Rows.Count > 0)
                {
                    for (int i = 0; i < tbl.Rows.Count; i++)
                    {

                        jst.Add(new SelectListItem() { Text = tbl.Rows[i]["Type"].ToString(), Value = tbl.Rows[i]["Type"].ToString() });

                    }

                }
            }
            return jst;
        }
    }

    public partial class RopeTail
    {
        public RopeTail()
        {
            RopeTaggings = new List<SelectListItem>
                {
                    new SelectListItem { Text = "No", Value = "No" },
                    new SelectListItem { Text = "Yes", Value = "Yes" }
                };
        }


        public string InspectionDueDate1 { get; set; }
        public string InstalledDate1 { get; set; }
        public string ReceivedDate1 { get; set; }

        public List<SelectListItem> RopeTaggings { get; set; }
        public string IsRopeInstalled { get; set; }
    }

    public partial class ChainStopper
    {
        public string InspectionDueDate1 { get; set; }
        public string InstalledDate1 { get; set; }
        public string ReceivedDate1 { get; set; }

        public string IsRopeInstalled { get; set; }


    }

    public partial class ChafeGuard
    {
        public string InspectionDueDate1 { get; set; }
        public string InstalledDate1 { get; set; }
        public string ReceivedDate1 { get; set; }

        public string IsRopeInstalled { get; set; }

        public int LooseETypeId { get; set; } = 7;

    }

    public partial class WinchBreakTestKit
    {
        public string InspectionDueDate1 { get; set; }
        public string InstalledDate1 { get; set; }
        public string ReceivedDate1 { get; set; }

        public string IsRopeInstalled { get; set; }

        public int LooseETypeId { get; set; } = 8;
    }

    public partial class LooseEDamageRecord
    {
        public LooseEDamageRecord()
        {
            IncidentReports = new List<SelectListItem>
                {
                 new SelectListItem { Text = "Yes", Value = "Yes" },
                    new SelectListItem { Text = "No", Value = "No" }

                };

            AllLEDamagedList = new List<LooseEDamageRecord>();
        }
        public string LooseEquipmentType { get; set; }

        public List<LooseEDamageRecord> AllLEDamagedList { get; set; }

        public int LEQuipID { get; set; }
        public List<SelectListItem> GetAllLE_Detail { get; set; }
        public List<SelectListItem> IncidentReports { get; set; }
        public List<DamageR> DamageReasons { get; set; }
        public List<DamageL> DamageLocations { get; set; }
        public List<DamageObserved> DamageObservedLists { get; set; }
        public List<MOperationBirthDetail> MooringOperationsLists { get; set; }

        //assR.DamageReasons = cls.DamageReasonList();
        //    assR.DamageLocations = cls.DamageLocatonList();
        //    assR.DamageObservedLists = cls.DamagObservedList();
        //    assR.MooringOperationsLists = cls.MooringOpListCommon();
    }

    public class LooseEquipmentDiscard

    {
        public LooseEquipmentDiscard()
        {
            GetAllLE_Detail = new List<SelectListItem>();
            ReasonOutofServices = new List<OutofServiceR>();
            DamageObservedLists = new List<DamageObserved>();
            MooringOperationsLists = new List<MOperationBirthDetail>();

        }
        public int LEQuipID { get; set; }
        public Nullable<int> LooseETypeId { get; set; }
        public string CertificateNumber { get; set; }
        public Nullable<DateTime> OutofServiceDate { get; set; }

        public string ReasonOutofService { get; set; }
        public string OtherReason { get; set; }
        public string DamageObserved { get; set; }
        public Nullable<int> MooringOperationID { get; set; }



        public List<SelectListItem> GetAllLE_Detail { get; set; }

        public List<OutofServiceR> ReasonOutofServices { get; set; }

        public List<DamageObserved> DamageObservedLists { get; set; }

        public List<MOperationBirthDetail> MooringOperationsLists { get; set; }
    }

    public partial class LooseEDisposal
    {
        public LooseEDisposal()
        {
            AllDisposalList = new List<LooseEDisposal>();
        }

        public string DiscardedDate1 { get; set; }
        public string LooseEquipmentType { get; set; }
        public int LEQuipID { get; set; }
        public List<SelectListItem> GetAllLE_DiscardedDetail { get; set; }
        public List<LooseEDisposal> AllDisposalList { get; set; }

    }

    public partial class MooringLooseEquipInspection
    {
        SqlConnection con = ConnectionBulder.con;
        public MooringLooseEquipInspection()
        {
            GetLooseEquipInspectionList = new List<MooringLooseEquipInspection>();
            LooseConditions = GetLooseConditions();
        }
        public string LooseEquipmentType { get; set; }
        public string Year { get; set; }

        public List<MooringLooseEquipInspection> AddLeInspectionList { get; set; }
        public List<SelectListItem> LooseConditions { get; set; }
        public List<string> YearList { get; set; }
        public List<MooringLooseEquipInspection> GetLooseEquipInspectionList { get; set; }

        public List<SelectListItem> GetLooseConditions()
        {
            List<SelectListItem> jst = new List<SelectListItem>();
            using (SqlDataAdapter sda = new SqlDataAdapter("Select * from InspectionCondition", con))
            {
                DataTable tbl = new DataTable();
                sda.Fill(tbl);
                if (tbl.Rows.Count > 0)
                {
                    for (int i = 0; i < tbl.Rows.Count; i++)
                    {

                        jst.Add(new SelectListItem() { Text = tbl.Rows[i]["Condition"].ToString(), Value = tbl.Rows[i]["Condition"].ToString() });

                    }

                }
            }
            return jst;
        }

    }

  





}
