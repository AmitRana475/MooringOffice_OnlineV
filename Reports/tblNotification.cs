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
    
    public partial class tblNotification
    {
        public int Id { get; set; }
        public string Notification { get; set; }
        public Nullable<int> NotificationType { get; set; }
        public string ShipActionTaken { get; set; }
        public Nullable<bool> Acknowledge { get; set; }
        public string AckRecord { get; set; }
        public Nullable<System.DateTime> NotificationDueDate { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public Nullable<int> RopeId { get; set; }
        public Nullable<byte> NotificationAlertType { get; set; }
        public string LooseCertificateNum { get; set; }
        public int LooseEqType { get; set; }
        public Nullable<System.DateTime> AcknDateTime { get; set; }
        public string AcknBy { get; set; }
        public int VesselId { get; set; }
        public Nullable<int> MOP_Id { get; set; }
    }
}
