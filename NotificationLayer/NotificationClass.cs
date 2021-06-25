using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NotificationLayer
{

    [MetadataType(typeof(notificationMetaData))]
    [Table("Notifications")]
    public partial class NotificationClass
    {
        public NotificationClass()
        {
            Comment1 = new List<CommentClass>();
        }

        [Key]
        public int Nid { get; set; }
        public int wid { get; set; }
        public int VesselId { get; set; }
        public string VesselName { get; set; }
        public string FleetName { get; set; }
        public string FleetType { get; set; }
        public DateTime NcDate { get; set; }
        public string NCtype { get; set; }
        public string NonConfirmity { get; set; }
        public string FullName { get; set; }
        public string UserName { get; set; }
        public string Rank { get; set; }
        public string AdminAkn { get; set; }
        public string MasterAkn { get; set; }
        public string HODAkn { get; set; }
        public string Acknowledge { get; set; }
        public DateTime? AcknDate { get; set; }
        public DateTime EDate { get; set; }
        public string FileNames { get; set; }
        public List<CommentClass> Comment1 { get; set; }
    }
    public class notificationMetaData
    {
        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}")]
        public DateTime NcDate { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}")]
        public DateTime EDate { get; set; }
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

    public class NotificationExport
    {
        public int Vessel_ID { get; set; }
        public DateTime NC_Date { get; set; }
        public string FullName { get; set; }
        public string UserName { get; set; }
        public string Position { get; set; }
        public string DepartmentName { get; set; }
        public string Office_Comment { get; set; }
        public DateTime Comment_Date { get; set; }

    }

}
