using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Shipment49Web.Areas.LooseEquipment.Models
{
    public class ChainStopper
    {
        public ChainStopper()
        {
            ChainStopperList = new List<ChainStopper>();
        }
        public int Id { get; set; }
        public int LooseETypeId { get; set; }
        public string ManufactureName { get; set; }
        public string CertificateNumber { get; set; }
        public decimal MBL { get; set; }
        public decimal Length { get; set; }
        public DateTime DateReceived { get; set; }
        public DateTime DateInstalled { get; set; }
        public DateTime OutofServiceDate { get; set; }
        public string ReasonOutofService { get; set; }
        public string OtherReason { get; set; }
        public string DamagedObserved { get; set; }
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
        public List<ChainStopper> ChainStopperList { get; set; }

    }
}