//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Reports
{
    using System;
    
    public partial class SP_DamageObserved_Detail_Result
    {
        public int VesselID { get; set; }
        public string VesselName { get; set; }
        public int FleetNameID { get; set; }
        public int FleetTypeID { get; set; }
        public int TradeAreaID { get; set; }
        public Nullable<int> RopeId { get; set; }
        public string CertificateNumber { get; set; }
        public Nullable<int> RopeTail { get; set; }
        public Nullable<System.DateTime> OutofServiceDate { get; set; }
        public Nullable<int> ManufacturerId { get; set; }
        public Nullable<int> RopeTypeId { get; set; }
        public string Name { get; set; }
        public string RopeType { get; set; }
        public string DamageObserved { get; set; }
        public string DamageReason { get; set; }
        public string IncidentActlion { get; set; }
        public Nullable<System.DateTime> DamageDate { get; set; }
    }
}
