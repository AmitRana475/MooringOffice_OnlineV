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
    
    public partial class LooseEDamageRecord
    {
        public int Id { get; set; }
        public Nullable<int> LooseETypeId { get; set; }
        public string CertificateNumber { get; set; }
        public string DamageObserved { get; set; }
        public Nullable<int> MOpId { get; set; }
        public Nullable<int> NotificationId { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public Nullable<System.DateTime> DamageDate { get; set; }
        public string IncidentReport { get; set; }
        public string DamageReason { get; set; }
        public string Remarks { get; set; }
        public int VesselID { get; set; }
    }
}
