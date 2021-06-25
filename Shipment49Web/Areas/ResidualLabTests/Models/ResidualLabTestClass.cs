using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Shipment49Web.Areas.ResidualLabTests.Models
{
    public class ResidualLabTestClass
    {
        public string Location { get; set; }      
        public string CertificateNumber { get; set; }
        public string UniqueId { get; set; }
        public string Name { get; set; }
        public string LabTestDate { get; set; }
        public decimal RunningHours { get; set; }
        public decimal TestResults { get; set; }
        public int VesselID { get; set; }
        public int Id { get; set; }
        public string Remarks { get; set; }
        public string attachment { get; set; }
        public int RopeId { get; set; }
        public List<ResidualLabTestClass> ResidualLabTestList { get; set; }
    }
}