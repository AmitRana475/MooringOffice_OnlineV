using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Shipment49Web.Areas.LooseEquipment.Models
{
    public class ChafeGaurd
    {
        public ChafeGaurd()
        {
            ChafeGaurdList = new List<ChafeGaurd>();
        }
        public int Id { get; set; }
        public string ManufacturerName { get; set; }
        public string Remarks { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsActive { get; set; }
        public DateTime InstalledDate { get; set; }
        public DateTime InspectionDueDate { get; set; }
        public DateTime OutofServiceDate { get; set; }
        public bool DeleteStatus { get; set; }
        public string CertificateNumber { get; set; }
        public DateTime ReceivedDate { get; set; }
        public string UniqueID { get; set; }
        public string ReasonOutofService { get; set; }
        public string OtherReason { get; set; }
        public int VesselID { get; set; }

        public List<ChafeGaurd> ChafeGaurdList { get; set; }

    }
}