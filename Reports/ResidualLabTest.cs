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
    
    public partial class ResidualLabTest
    {
        public int Id { get; set; }
        public Nullable<int> RopeId { get; set; }
        public Nullable<int> RopeTypeId { get; set; }
        public Nullable<int> ManufacturerId { get; set; }
        public string Location { get; set; }
        public Nullable<decimal> RunningHours { get; set; }
        public Nullable<decimal> ServiceTestDate { get; set; }
        public Nullable<System.DateTime> LabTestDate { get; set; }
        public Nullable<decimal> TestResults { get; set; }
        public string Remarks { get; set; }
        public Nullable<int> RopeTail { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public int VesselID { get; set; }
    }
}
