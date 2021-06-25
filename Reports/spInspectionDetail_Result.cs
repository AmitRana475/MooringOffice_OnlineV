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
    
    public partial class spInspectionDetail_Result
    {
        public int Id { get; set; }
        public string InspectBy { get; set; }
        public Nullable<System.DateTime> InspectDate { get; set; }
        public Nullable<int> RopeId { get; set; }
        public Nullable<int> WinchId { get; set; }
        public Nullable<int> InspectionId { get; set; }
        public Nullable<int> ExternalRating_A { get; set; }
        public Nullable<int> InternalRating_A { get; set; }
        public Nullable<int> AverageRating_A { get; set; }
        public Nullable<decimal> LengthOFAbrasion_A { get; set; }
        public Nullable<decimal> DistanceOutboard_A { get; set; }
        public Nullable<decimal> CutYarnCount_A { get; set; }
        public Nullable<decimal> LengthOFGlazing_A { get; set; }
        public Nullable<int> RopeTail { get; set; }
        public Nullable<int> ExternalRating_B { get; set; }
        public Nullable<int> InternalRating_B { get; set; }
        public Nullable<int> AverageRating_B { get; set; }
        public Nullable<decimal> LengthOFAbrasion_B { get; set; }
        public Nullable<decimal> DistanceOutboard_B { get; set; }
        public Nullable<decimal> CutYarnCount_B { get; set; }
        public Nullable<decimal> LengthOFGlazing_B { get; set; }
        public string Chafe_guard_condition { get; set; }
        public Nullable<int> Twist { get; set; }
        public string Photo1 { get; set; }
        public string Photo2 { get; set; }
        public string Image1 { get; set; }
        public string Image2 { get; set; }
        public Nullable<int> NotificationId { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public int VesselID { get; set; }
        public string certificatenumber { get; set; }
        public string assignednumber { get; set; }
    }
}
