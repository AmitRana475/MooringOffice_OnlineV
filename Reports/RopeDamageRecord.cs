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
    
    public partial class RopeDamageRecord
    {
        public int Id { get; set; }
        public Nullable<int> RopeId { get; set; }
        public string DamageObserved { get; set; }
        public string IncidentReport { get; set; }
        public string DamageLocation { get; set; }
        public string DamageReason { get; set; }
        public Nullable<int> MOPId { get; set; }
        public Nullable<int> RopeTail { get; set; }
        public Nullable<int> NotificationId { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public string IncidentActlion { get; set; }
        public Nullable<System.DateTime> DamageDate { get; set; }
        public int VesselID { get; set; }
        public Nullable<int> WinchId { get; set; }
    }
}
