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
    
    public partial class MooringLooseEquipInspection
    {
        public int Id { get; set; }
        public string InspectBy { get; set; }
        public Nullable<System.DateTime> InspectDate { get; set; }
        public Nullable<int> LooseETypeId { get; set; }
        public string Number { get; set; }
        public string Condition { get; set; }
        public string Remarks { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public Nullable<int> NotificationId { get; set; }
        public string Photo1 { get; set; }
        public string Photo2 { get; set; }
        public string Image1 { get; set; }
        public string Image2 { get; set; }
        public Nullable<int> LooseEtbPK { get; set; }
        public int VesselID { get; set; }
    }
}
