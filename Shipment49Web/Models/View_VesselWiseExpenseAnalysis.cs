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
    
    public partial class View_VesselWiseExpenseAnalysis
    {
        public int VesselID { get; set; }
        public int RopeId { get; set; }
        public Nullable<decimal> Cost { get; set; }
        public Nullable<int> ManufacturerId { get; set; }
        public string Manufacturer { get; set; }
        public Nullable<int> RopeTypeId { get; set; }
        public string RopeType { get; set; }
        public Nullable<int> RunningHours { get; set; }
        public Nullable<decimal> Avg_Months { get; set; }
        public Nullable<System.DateTime> OutofServiceDate { get; set; }
        public Nullable<bool> DeleteStatus { get; set; }
        public string FleetName { get; set; }
        public int FleetNameID { get; set; }
        public string FleetType { get; set; }
        public int FleetTypeID { get; set; }
        public string VesselName { get; set; }
        public int ImoNo { get; set; }
        public System.DateTime DateBuilt { get; set; }
        public string PortName { get; set; }
        public string FacilityName { get; set; }
        public string BirthType { get; set; }
        public string MooringType { get; set; }
        public string Lead { get; set; }
        public string Lead1 { get; set; }
        public Nullable<int> WindSpeed { get; set; }
        public string AnySquall { get; set; }
        public Nullable<decimal> CurrentSpeed { get; set; }
        public string Berth_exposed_SeaSwell { get; set; }
        public string SurgingObserved { get; set; }
        public string Any_Affect_Passing_Traffic { get; set; }
        public string Ship_was_continuously_contact_with_fender { get; set; }
        public Nullable<int> AirTemprature { get; set; }
        public string Any_Rope_Damaged { get; set; }
        public Nullable<System.DateTime> InstalledDate { get; set; }
        public int TradeAreaID { get; set; }
        public string TradeArea { get; set; }
    }
}
