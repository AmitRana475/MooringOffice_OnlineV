//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Shipment49Web.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class CrewDetail
    {
        public int Id { get; set; }
        public Nullable<int> Vessel_ID { get; set; }
        public string VesselName { get; set; }
        public string FleetName { get; set; }
        public string FleetType { get; set; }
        public Nullable<int> Crew_ID { get; set; }
        public string FullName { get; set; }
        public string UserName { get; set; }
        public string Position { get; set; }
        public Nullable<int> rid { get; set; }
        public Nullable<System.DateTime> ServiceFrom { get; set; }
        public Nullable<System.DateTime> ServiceTo { get; set; }
        public string CDCNumber { get; set; }
        public string Emp_Number { get; set; }
        public string Comments { get; set; }
        public Nullable<bool> Overtime { get; set; }
        public string Department { get; set; }
        public Nullable<decimal> SeaWH { get; set; }
        public Nullable<decimal> PortWH { get; set; }
        public string SeaWk1 { get; set; }
        public string SeaNWK1 { get; set; }
        public string PortWk1 { get; set; }
        public string PortNWK1 { get; set; }
        public string Remarks { get; set; }
        public string Watchkeeper { get; set; }
        public Nullable<int> NrmlWHrs { get; set; }
        public Nullable<int> SatWHrs { get; set; }
        public Nullable<int> SunWhrs { get; set; }
        public Nullable<int> HolidayWHrs { get; set; }
        public Nullable<int> FixedOverTime { get; set; }
        public Nullable<decimal> HourlyRate { get; set; }
        public string Currency { get; set; }
        public string holidays { get; set; }
        public string Flag { get; set; }
        public Nullable<System.DateTime> ImportDate { get; set; }
        public string FileNames { get; set; }
    }
}