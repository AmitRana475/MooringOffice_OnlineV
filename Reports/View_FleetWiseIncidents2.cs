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
    using System.Collections.Generic;
    
    public partial class View_FleetWiseIncidents2
    {
        public int VesselID { get; set; }
        public string VesselName { get; set; }
        public Nullable<int> ManufacturerId { get; set; }
        public string Manufacturer { get; set; }
        public Nullable<int> RopeTypeId { get; set; }
        public string RopeType { get; set; }
        public Nullable<int> Damaged { get; set; }
        public Nullable<System.DateTime> InstalledDate { get; set; }
        public Nullable<System.DateTime> OutofServiceDate { get; set; }
        public Nullable<System.DateTime> DamageDate { get; set; }
    }
}
