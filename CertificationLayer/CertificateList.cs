using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CertificationLayer
{
    [Table("CertificateList")]
    [MetadataType(typeof(certificationMetaData))]
    public class CertificateList
    {
        [Key]
        public int Id { get; set; }
        public int Cid { get; set; }
        public int VesselId { get; set; }
        public string VesselName { get; set; }
        public string FleetName { get; set; }
        public string FleetType { get; set; }
        public string CName { get; set; }
        public DateTime DOI { get; set; }
        public DateTime DOS { get; set; }
        public DateTime DOE { get; set; }
        public string Remarks { get; set; }
        public DateTime ImportDate { get; set; }
        public string FileNames { get; set; }
    }
    
}
