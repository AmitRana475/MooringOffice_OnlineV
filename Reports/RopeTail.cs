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
    
    public partial class RopeTail
    {
        public int Id { get; set; }
        public Nullable<int> LooseETypeId { get; set; }
        public string ManufactureName { get; set; }
        public string RopeConstruction { get; set; }
        public Nullable<decimal> Diameter { get; set; }
        public Nullable<decimal> Length { get; set; }
        public Nullable<decimal> MBL { get; set; }
        public Nullable<decimal> LDBF { get; set; }
        public Nullable<decimal> WLL { get; set; }
        public Nullable<int> MaxRunHours { get; set; }
        public Nullable<int> MaxYearServiceMonth { get; set; }
        public string CertificateNumber { get; set; }
        public Nullable<System.DateTime> ReceivedDate { get; set; }
        public Nullable<System.DateTime> InstalledDate { get; set; }
        public string RopeTagging { get; set; }
        public Nullable<System.DateTime> OutofServiceDate { get; set; }
        public string ReasonOutofService { get; set; }
        public string OtherReason { get; set; }
        public string DamageObserved { get; set; }
        public Nullable<int> MOpId { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public Nullable<System.DateTime> InspectionDueDate { get; set; }
        public string Remarks { get; set; }
        public Nullable<bool> DeleteStatus { get; set; }
        public string UniqueID { get; set; }
        public int VesselID { get; set; }
    }
}