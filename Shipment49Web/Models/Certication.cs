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
    
    public partial class Certication
    {
        public int Nid { get; set; }
        public Nullable<int> Id { get; set; }
        public Nullable<System.DateTime> ImportDate { get; set; }
        public Nullable<int> VesselId { get; set; }
        public string VesselName { get; set; }
        public string FleetName { get; set; }
        public string FleetType { get; set; }
        public string CName { get; set; }
        public Nullable<System.DateTime> DOI { get; set; }
        public Nullable<System.DateTime> DOS { get; set; }
        public Nullable<System.DateTime> DOE { get; set; }
        public string AlertFrequency { get; set; }
        public string AdminAck { get; set; }
        public string MasterAck { get; set; }
        public string HODAck { get; set; }
        public string Acknowledge { get; set; }
        public Nullable<System.DateTime> AcknDate { get; set; }
        public string FileNames { get; set; }
    }
}
