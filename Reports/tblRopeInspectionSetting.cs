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
    
    public partial class tblRopeInspectionSetting
    {
        public int Id { get; set; }
        public int MooringRopeType { get; set; }
        public int ManufacturerType { get; set; }
        public Nullable<int> MaximumRunningHours { get; set; }
        public Nullable<int> MaximumMonthsAllowed { get; set; }
        public Nullable<int> EndToEndMonth { get; set; }
        public Nullable<int> RotationOnWinches { get; set; }
        public decimal Rating1 { get; set; }
        public decimal Rating2 { get; set; }
        public decimal Rating3 { get; set; }
        public decimal Rating4 { get; set; }
        public decimal Rating5 { get; set; }
        public decimal Rating6 { get; set; }
        public decimal Rating7 { get; set; }
    
        public virtual tblCommon tblCommon { get; set; }
    }
}
