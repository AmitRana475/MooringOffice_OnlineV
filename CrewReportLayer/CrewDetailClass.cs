using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CrewReportLayer
{
    [Table("CrewDetail")]
    [MetadataType(typeof(crewmetadata))]
    public partial class CrewDetailClass
    {
        [Key]
        public int Id { get; set; }
        public int Vessel_ID { get; set; }
        public string VesselName { get; set; }
        public string FleetName { get; set; }
        public string FleetType { get; set; }
        public int Crew_ID { get; set; }
        public string FullName { get; set; }
        public string UserName { get; set; }
        public string Position { get; set; }
        public int rid { get; set; }
        public DateTime ServiceFrom { get; set; }
        public DateTime ServiceTo { get; set; }
        public string CDCNumber { get; set; }
        public string Emp_Number { get; set; }
        public string Comments { get; set; }
        public bool Overtime { get; set; }
        public string Department { get; set; }
        public decimal SeaWH { get; set; }
        public decimal PortWH { get; set; }
        public string SeaWk1 { get; set; }
        public string SeaNWK1 { get; set; }
        public string PortWk1 { get; set; }
        public string PortNWK1 { get; set; }
        public string Remarks { get; set; }
        public string Watchkeeper { get; set; }
        public int NrmlWHrs { get; set; }
        public int SatWHrs { get; set; }
        public int SunWhrs { get; set; }
        public int HolidayWHrs { get; set; }
        public int FixedOverTime { get; set; }
        public decimal HourlyRate { get; set; }
        public string Currency { get; set; }
        public string holidays { get; set; }
        public string Flag { get; set; }
        public DateTime ImportDate { get; set; }
        public string FileNames { get; set; }

    }

    public class crewmetadata
    {
        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}")]
        public DateTime ServiceFrom { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}")]
        public DateTime ServiceTo { get; set; }
    }
}
