using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CertificationLayer
{
    [MetadataType(typeof(certificationMetaData))]
    [Table("Certications")]
    public class CertificationClass
    {
        public CertificationClass()
        {
            Comment1 = new List<CommentCirtificate>();
        }

        [Key]
        public int Nid { get; set; }
        public int Id { get; set; }
        public DateTime ImportDate { get; set; }
        public string FileNames { get; set; }
        public int VesselId { get; set; }
        public string VesselName { get; set; }
        public string FleetName { get; set; }
        public string FleetType { get; set; }
        public string CName { get; set; }
        public DateTime DOI { get; set; }
        public DateTime DOS { get; set; }
        public DateTime DOE { get; set; }
        public string AlertFrequency { get; set; }
        public string AdminAck { get; set; }
        public string MasterAck { get; set; }
        public string HODAck { get; set; }
        public string Acknowledge { get; set; }
        public DateTime? AcknDate { get; set; }
        public List<CommentCirtificate> Comment1 { get; set; }
    }
    public class certificationMetaData
    {
        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}")]
        public DateTime ImportDate { get; set; }
    
        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}")]
        public DateTime DOI { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}")]
        public DateTime DOS { get; set; }
        
        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}")]
        public DateTime DOE { get; set; }
    }
    public class BmsPageNumber
    {
        public static int Pagenumber { get; set; }
        public static string FleetType { get; set; }
        public static string FleetName { get; set; }
        public static string VesselName { get; set; }
        public static string datefrom { get; set; }
        public static string dateto { get; set; }

    }
}
