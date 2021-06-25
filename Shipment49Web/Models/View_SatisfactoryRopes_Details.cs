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
    
    public partial class View_SatisfactoryRopes_Details
    {
        public int RopeId { get; set; }
        public Nullable<int> RopeTypeId { get; set; }
        public string RopeConstruction { get; set; }
        public Nullable<decimal> DiaMeter { get; set; }
        public Nullable<decimal> Length { get; set; }
        public Nullable<decimal> MBL { get; set; }
        public Nullable<decimal> LDBF { get; set; }
        public Nullable<decimal> WLL { get; set; }
        public Nullable<int> ManufacturerId { get; set; }
        public string CertificateNumber { get; set; }
        public Nullable<System.DateTime> ReceivedDate { get; set; }
        public Nullable<System.DateTime> InstalledDate { get; set; }
        public string RopeTagging { get; set; }
        public Nullable<System.DateTime> OutofServiceDate { get; set; }
        public string ReasonOutofService { get; set; }
        public string OtherReason { get; set; }
        public string DamageObserved { get; set; }
        public string IncidentReport { get; set; }
        public Nullable<int> MooringOperationID { get; set; }
        public Nullable<int> CurrentRunningHours { get; set; }
        public Nullable<int> MaxRunningHours { get; set; }
        public Nullable<int> MaxMonthsAllowed { get; set; }
        public Nullable<System.DateTime> InspectionDueDate { get; set; }
        public Nullable<int> RopeTail { get; set; }
        public Nullable<bool> DeleteStatus { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public int VesselID { get; set; }
        public string RopeType { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
        public string AssignedWinch { get; set; }
    }
}
