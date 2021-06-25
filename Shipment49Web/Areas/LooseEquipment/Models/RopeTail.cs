using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Shipment49Web.Areas.LooseEquipment.Models
{   // 3 MessengerRope /  4 RopeStopper / 6 FireWire
    public class RopeTail
    {
        public RopeTail()
        {
            MessengerRopeList = new List<RopeTail>();
            RopeStopperList = new List<RopeTail>();
            ChainStopperList = new List<RopeTail>();
        }
        public int Id { get; set; }
        public int LooseETypeId { get; set; }
        public string ManufactureName { get; set; }
        public string RopeConstruction { get; set; }
        public decimal Diameter { get; set; }
        public decimal Length { get; set; }
        public decimal MBL { get; set; }
        public decimal LDBF { get; set; }
        public decimal WLL { get; set; }
        public int MaxRunHours { get; set; }
        public int MaxYearServiceMonth { get; set; }
        public string CertificateNumber { get; set; }
        public DateTime ReceivedDate { get; set; }
        public DateTime InstalledDate { get; set; }
        public string RopeTagging { get; set; }
        public DateTime OutofServiceDate { get; set; }
        public DateTime ReasonOutofService { get; set; }
        public string OtherReason { get; set; }
        public string DamageObserved { get; set; }
        public int MOpId { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime ModifiedDate { get; set; }
        public string ModifiedBy { get; set; }
        public bool IsActive { get; set; }
        public DateTime InspectionDueDate { get; set; }
        public string Remarks { get; set; }
        public bool DeleteStatus { get; set; }
        public string UniqueID { get; set; }
        public int VesselID { get; set; }

        public List<RopeTail> MessengerRopeList { get; set; }
        public List<RopeTail> RopeStopperList { get; set; }
        public List<RopeTail> ChainStopperList { get; set; }
    }
}